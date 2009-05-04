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
using System.Text;
using System.IO;

using Ximura;
using Ximura.Helper;
#endregion // using
namespace Ximura.Collections
{
    public abstract partial class LockFreeCollectionBase<T>
    {
#if (PROFILING)
        private enum ProfileAction:int
        {
            Time_FindAndLock =0,
            Time_AddInternal=1,
            Time_AddInternalHAS = 2,
            Time_HashAndSentinel_GetHashCode = 3,
            Time_HashAndSentinel_FindSentinel = 4,
            Time_AddIntHAS=5,
            Time_AddIntHAS_FindAndLock = 6,
            Time_AddIntHAS_Insert = 7,
            Time_ContainsTot = 8,
            Time_ContainsHAL = 9,
            Count_FindAndLockHopCount = 10,
            Count_FindAndLockSlotLocks = 11,
            Time_GetSentinelVertexID = 12,
            Time_GetSentinelVertexID_Insert = 13,
            Time_GetSentinelVertexID_LockWait1 = 14,
            Time_GetSentinelVertexID_LockWait2 = 15,
            Lock_GetSentinelCreate = 16,
            Lock_GetSentinelWait = 17,
            Time_BitSizeCalculate = 18,
            Time_EmptyGet = 19,
            Time_EmptyAdd = 20
        }

        private enum ProfileArrayType : int
        {
            BucketsWait = 0,
            Slots = 1,
            BucketsCreate = 2
        }

        BinaryWriter bwb = new BinaryWriter(new MemoryStream());
        BinaryWriter bwc = new BinaryWriter(new MemoryStream());
        BinaryWriter bws = new BinaryWriter(new MemoryStream());

        int[] pflCount;
        long[] pflTotal;

        private void ProfilingSetup()
        {
            int count = Enum.GetNames(typeof(ProfileAction)).Length;
            pflCount = new int[count];
            pflTotal = new long[count];
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
                            , (double)pflTotal[pos]*1000 / (double)pflCount[pos]
                            , name.StartsWith("Time")?"ms":""
                             );
                        sb.AppendLine();
                    });

                BinaryReader brb = new BinaryReader(bwb.BaseStream);
                bwb.BaseStream.Position = 0;
                sb.AppendLine("Bucket Wait----------------------------------------------------------");
                bwb.BaseStream
                    .StreamRead<int>()
                    .GroupBy(s => s, s => s, (a, b) => new { Key = a, Count = b.Count() })
                    .OrderByDescending(i => i.Count)
                    .Take(10)
                    .ForEach(g => sb.AppendFormat("{0}-{1}\r\n", g.Key, g.Count));
                sb.AppendLine("Bucket Create--------------------------------------------------------");
                bwc.BaseStream.Position = 0;
                bwc.BaseStream
                    .StreamRead<int>()
                    .GroupBy(s => s, s => s, (a, b) => new { Key = a, Count = b.Count() })
                    .OrderByDescending(i => i.Count)
                    .Take(10)
                    .ForEach(g => sb.AppendFormat("{0}-{1}\r\n", g.Key, g.Count));
                sb.AppendLine("Slots----------------------------------------------------------------");
                bws.BaseStream.Position = 0;
                bws.BaseStream
                    .StreamRead<int>()
                    .GroupBy(s => s, s => s, (a, b) => new { Key = a, Count = b.Count() })
                    .OrderByDescending(i => i.Count)
                    .Take(10)
                    .ForEach(g => sb.AppendFormat("{0}-{1}\r\n", g.Key, g.Count));
                sb.AppendLine("---------------------------------------------------------------------");

                return sb.ToString();
            }
        }
#endif
    }
}
