#region Copyright
// *******************************************************************************
// Copyright (c) 2000-2009 Paul Stancer.
// All rights reserved. This program and the accompanying materials
// are made available under the terms of the Eclipse Public License v1.0
// which accompanies this distribution, and is available at
// http://www.eclipse.org/legal/epl-v10.html
//
// Contributors:
//     Paul Stancer - initial implementation
// *******************************************************************************
#endregion
#region using
using System;
using System.Linq;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using System.Runtime.Serialization;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.IO;

using Ximura;
using Ximura.Helper;
#endregion // using
namespace Ximura.Collections
{
    public abstract partial class LockFreeCollectionBase<T>
    {
        #region DebugData/DebugEmpty
#if (DEBUG)

        public void DebugDataValidate()
        {
            int previousHashID = -1;
            int slots = 0;
            int sentinels = 0;
            int data = 0;

            int maxHops = 0;
            int sentHops = 0;

            DebugData
                .ForEach(i =>
                {
                    int hashID = i.Value.HashID;
                    
                    slots++;
                    sentHops++;

                    if (i.Value.IsSentinel)
                    {
                        if (sentHops > maxHops)
                        {
                            maxHops = sentHops;
                        }
                        sentHops = 0;
                        sentinels++;
                    }
                    else
                        data++;

                    if (hashID < previousHashID)
                        throw new Exception("Oops!");

                    previousHashID = hashID;
                });

            var results = DebugData
                .Select(i => i.Value)
                .Where(i => i.IsSentinel)
                .GroupBy(i => i.HashID)
                .OrderByDescending(g => g.Count())
                .Where(g => g.Count()>1)
                .ToArray();

        }

        #region DebugData
        /// <summary>
        /// This debug method enumerates through the collection.
        /// </summary>
        public IEnumerable<KeyValuePair<int, Vertex<T>>> DebugData
        {
            get
            {
                return InternalScan(false);
            }
        }
        #endregion // DebugData

        public string DebugSlotStats()
        {
            int slots = 0;
            int slotSentinel = 0;
            int slotData = 0;

            DebugData.ForEach(k =>
                {
                    slots++;
                    if (k.Value.IsSentinel)
                        slotSentinel++;
                    else
                        slotData++;
                });

            return string.Format("Slots={0} |S {1} |D {2}", slots, slotSentinel, slotData);
        }

        #region DebugEmpty
        /// <summary>
        /// This debug method enumerates through the collection.
        /// </summary>
        public IEnumerable<KeyValuePair<int, Vertex<T>>> DebugEmpty
        {
            get
            {
                int currentVersion = mVersion;

                KeyValuePair<int, Vertex<T>> item = new KeyValuePair<int, Vertex<T>>(0, mSlots[cnIndexEmptyQueue]);
                yield return item;

                while (!item.Value.IsTerminator)
                {
                    item = new KeyValuePair<int, Vertex<T>>(item.Value.NextSlotIDPlus1 - 1, mSlots[item.Value.NextSlotIDPlus1 - 1]);
                    yield return item;
                }
            }
        }
        #endregion // DebugEmpty

        public string DebugDump
        {
            get
            {
                StringBuilder sb = new StringBuilder();
#if (PROFILING)
                sb.AppendLine(ProfileStats);
#endif
                return sb.ToString();
            }
        }
#endif
        #endregion // DebugScan

        public void DebugReset()
        {
#if (PROFILING)
            ProfilingSetup();
#endif
        }

#if (PROFILING)
        private enum ProfileAction : int
        {
            Time_FindAndLock = 0,
            Time_AddInternal = 1,
            Time_AddInternalHAS = 2,
            Time_EmptyGet = 3,
            Time_EmptyAdd = 4,
            Time_AddIntHAS = 5,
            Time_AddIntHAS_FindAndLock = 6,
            Time_AddIntHAS_Insert = 7,
            Time_ContainsTot = 8,
            Time_ContainsHAL = 9,
            Count_FindAndLockHopCount = 10,
            Count_FindAndLockSlotLocks = 11,
            Time_GetSentinelVertexID = 12,
            Time_GetSentinelVertexID_Insert = 13,
            Time_GetSentinelVertexID_TimeWait1 = 14,
            Time_GetSentinelVertexID_TimeWait2 = 15,
            Lock_GetSentinelCreate = 16,
            Lock_GetSentinelWait = 17,
            Time_BitSizeCalculate = 18,
            Time_HashAndSentinel = 19,
            Time_HashAndSentinel_GetHashCode = 20,
            Time_HashAndSentinel_FindSentinel = 21,

            Count_HopData = 22,
            Count_HopBucketSkip = 23,
            Count_HopSentinel = 24
        }

        private enum ProfileArrayType : int
        {
            BucketsWait = 0,
            Slots = 1,
            BucketsCreate = 2
        }

        BinaryWriter bwb;
        BinaryWriter bwc;
        BinaryWriter bws;

        int[] pflCount;
        long[] pflTotal;

        private void ProfilingSetup()
        {
            int count = Enum.GetNames(typeof(ProfileAction)).Length;
            pflCount = new int[count];
            pflTotal = new long[count];
            bwb = new BinaryWriter(new MemoryStream());
            bwc = new BinaryWriter(new MemoryStream());
            bws = new BinaryWriter(new MemoryStream());
        }

        private void Profile(ProfileAction action, long timespan)
        {
            int pos = (int)action;
            Interlocked.Increment(ref pflCount[pos]);
            Interlocked.Add(ref pflTotal[pos], timespan);
        }

        private void ProfileHotspot(ProfileArrayType type, int index)
        {
            lock (this)
            {
                switch (type)
                {
                    case ProfileArrayType.BucketsWait:
                        bwb.Write(index);
                        break;
                    case ProfileArrayType.BucketsCreate:
                        bwc.Write(index);
                        break;
                    case ProfileArrayType.Slots:
                        bws.Write(index);
                        break;
                }
            }
        }

        #region ProfileStats
        /// <summary>
        /// This method outputs the profile statistics.
        /// </summary>
        public string ProfileStats
        {
            get
            {
                StringBuilder sb = new StringBuilder();

                Enum.GetNames(typeof(ProfileAction))
                    .ForIndex((pos, name) =>
                    {
                        sb.AppendFormat("{0} -> {1}/{2} = {3}{4}"
                            , name
                            , pflTotal[pos]
                            , pflCount[pos]
                            , name.StartsWith("Time") ? (double)pflTotal[pos] * 1000 / (double)pflCount[pos] : (double)pflTotal[pos] / (double)pflCount[pos]
                            , name.StartsWith("Time") ? "ms" : ""
                             );
                        sb.AppendLine();
                    });
                sb.AppendLine();
                sb.AppendLine(DebugSlotStats());

                BinaryReader brb = new BinaryReader(bwb.BaseStream);
                bwb.BaseStream.Position = 0;
                sb.AppendLine();
                sb.AppendLine("Bucket Wait----------------------------------------------------------");
                bwb.BaseStream
                    .StreamRead<int>()
                    .GroupBy(s => s, s => s, (a, b) => new { Key = a, Count = b.Count() })
                    .OrderByDescending(i => i.Count)
                    .Take(20)
                    .ForEach(g => sb.AppendFormat("{0}-{1}\r\n", g.Key, g.Count));
                sb.AppendLine();
                sb.AppendLine("Bucket Create--------------------------------------------------------");
                bwc.BaseStream.Position = 0;
                bwc.BaseStream
                    .StreamRead<int>()
                    .GroupBy(s => s, s => s, (a, b) => new { Key = a, Count = b.Count() })
                    .OrderByDescending(i => i.Count)
                    .Take(20)
                    .ForEach(g => sb.AppendFormat("{0}-{1}\r\n", g.Key, g.Count));
                sb.AppendLine();
                sb.AppendLine("Slots----------------------------------------------------------------");
                bws.BaseStream.Position = 0;
                bws.BaseStream
                    .StreamRead<int>()
                    .GroupBy(s => s, s => s, (a, b) => new { Key = a, Count = b.Count() })
                    .OrderByDescending(i => i.Count)
                    .Take(20)
                    .ForEach(g => sb.AppendFormat("{0}-{1}\r\n", g.Key, g.Count));
                sb.AppendLine("---------------------------------------------------------------------");

                return sb.ToString();
            }
        }
        #endregion // ProfileStats

#endif
    }
}
