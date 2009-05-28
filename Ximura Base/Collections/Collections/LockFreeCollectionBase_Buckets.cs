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

using Ximura;
using Ximura.Helper;
#endregion // using
namespace Ximura.Collections
{
    public abstract partial class LockFreeCollectionBase<T>
    {
        #region Static Declarations
        //private const uint cnMixer1 = 0xA5A5A5A5;
        //private const uint cnMixer2 = 0x5A5A5A5A;
        //private const uint cnMixer3 = 0xF0F0F0F0;
        //private const uint cnMixer4 = 0x0F0F0F0F;

        private const double elog2 = 0.693147181;
        #endregion // Declarations
        #region Declarations
        /// <summary>
        /// This array holds the lookup hashtable.
        /// </summary>
        private ExpandableFineGrainedLockArray<int> mBuckets;
        /// <summary>
        /// This value holds the current number of bits being supported by the collection.
        /// </summary>
        private int mCurrentBits;
        /// <summary>
        /// This is the recalculate threshold, where the system recalculates the system bucket capacity.
        /// </summary>
        private int mRecalculateThreshold;
        #endregion 
        #region InitializeBuckets(int capacity)
        /// <summary>
        /// This method initializes the data allocation.
        /// </summary>
        protected virtual void InitializeBuckets(int capacity)
        {
            mCurrentBits = (int)(Math.Log(capacity) / elog2);
            mRecalculateThreshold = 2 << mCurrentBits;
            mBuckets = new ExpandableFineGrainedLockArray<int>(mRecalculateThreshold);

            //Initialize the first bucket to the root sentinel vertex.
            mBuckets[0] = cnIndexData + 1;
        }
        #endregion // InitializeAllocation(int capacity)

        #region BitSizeCalculate()
        /// <summary>
        /// This method calculates the current number of bits needed to support the current data.
        /// </summary>
        protected virtual void BitSizeCalculate()
        {
#if (PROFILING)
            int start = Environment.TickCount;
#endif
            int total = mCount;
            int currentBits = mCurrentBits;
            int recalculateThreshold = mRecalculateThreshold;

            int newBits = (int)(Math.Log(total) / elog2);
            int newThreshold = 2 << newBits;

            if (newBits>currentBits)
                Interlocked.CompareExchange(ref mCurrentBits, newBits, currentBits);

            if (newThreshold>recalculateThreshold)
                Interlocked.CompareExchange(ref mRecalculateThreshold, newThreshold, recalculateThreshold);

#if (PROFILING)
            Profile(ProfileAction.Time_BitSizeCalculate, Environment.TickCount - start);
#endif
        }
        #endregion

        #region Struct -> Sentinel
        /// <summary>
        /// This is the sentinel data.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        protected struct Sentinel
        {
            #region Constants
            private const int cnHiMask = 0x08000000;
            private const int cnLowerBitMask = 0x07FFFFFF;
            #endregion // Constants
            #region Constructor
            /// <summary>
            /// This method creates the sentinel.
            /// </summary>
            /// <param name="hashCode">The item hashcode.</param>
            /// <param name="bits">The current number of bits for the collection.</param>
            public Sentinel(int hashCode, int bits, ExpandableFineGrainedLockArray<int> buckets)
            {
                HashCode = hashCode & cnLowerBitMask;
                BitsStart = bits;
                BitsCurrent = bits + 1;
                BucketIDParent = -1;

                //BucketID = HashCode % (1 << (BitsCurrent));
                BucketID = HashCode & (int.MaxValue >> (31 - BitsCurrent));

                SlotID = 0;

                int tBitsCurrent = BitsCurrent;
                int tBitsStart = BitsStart;
                int tBucketID = -1;

                while (tBitsCurrent >= 0)
                {
                    //int newBucketID = HashCode % (1 << (tBitsCurrent));
                    int newBucketID = HashCode & (int.MaxValue >> (31-tBitsCurrent));

                    if (newBucketID != tBucketID)
                    {
                        tBucketID = newBucketID;

                        int position = buckets[tBucketID];
                        if (position > 0)
                        {
                            //Ok, we have reached the limit of the sentinels we need to create.
                            BitsCurrent = tBitsCurrent;
                            BucketIDParent = tBucketID;
                            SlotID = position - 1;
                            break;
                        }
                    }

                    tBitsCurrent--;
                }
            }
            #endregion // Constructor

            #region Data
            /// <summary>
            /// The sentinel slot ID.
            /// </summary>
            public int SlotID;
            /// <summary>
            /// The item hashcode.
            /// </summary>
            public int HashCode;

            /// <summary>
            /// The number of bits used for the calculation.
            /// </summary>
            public int BitsStart;
            /// <summary>
            /// The number of bits used for the current bucket ID.
            /// </summary>
            public int BitsCurrent;

            /// <summary>
            /// The current bucket ID.
            /// </summary>
            public int BucketID;
            /// <summary>
            /// The parent bucket ID;
            /// </summary>
            public int BucketIDParent;
            #endregion // Data

            #region ToString()
            /// <summary>
            /// This override provides a quick easy way to read the data.
            /// </summary>
            /// <returns>Returns a string representation of sentinel.</returns>
            public override string ToString()
            {
                return string.Format("Slot={0} / {1:X} |{2}-{3}|B {4:X}/P {5:X}", SlotID, HashCode, BitsStart, BitsCurrent, BucketID, BucketIDParent);
            }
            #endregion // ToString()

            #region HashIDCalculate()
            /// <summary>
            /// This method calculates the hash id, which is the hash code reversed.
            /// </summary>
            /// <returns>Returns the hash ID.</returns>
            public int HashIDCalculate()
            {
                return BitReverse(HashCode);
            }
            #endregion // HashIDCalculate()

            #region BitReverse(int data)
            /// <summary>
            /// This method reverses the hashcode so that it is ordered in reverse based on bit value, i.e.
            /// xxx1011 => 1101xxxx => Bucket 1 1xxxxx => Bucket 3 11xxxxx => Bucket 6 110xxx etc.
            /// </summary>
            /// <param name="data"The data to reverse></param>
            /// <returns>Returns the reversed data</returns>
            public static int BitReverse(int data)
            {
                int result = 0;
                int hiMask = cnHiMask;

                for (; data > 0; data >>= 1)
                {
                    if ((data & 1) > 0)
                        result |= hiMask;
                    hiMask >>= 1;

                    if (hiMask == 0)
                        break;
                }

                return result;
            }
            #endregion // BitReverse(int data, int wordSize)

#if (DEBUG)
            #region DebugBuckets
            /// <summary>
            /// This is the debug buckets enumeration.
            /// </summary>
            public IEnumerable<string> DebugBuckets
            {
                get
                {
                    int tBitsCurrent = BitsCurrent;
                    int tBitsStart = BitsStart;
                    int tBucketID = BucketID;

                    yield return string.Format("HashCode={0:X}", HashCode);

                    while (tBitsCurrent < tBitsStart)
                    {
                        tBitsCurrent++;

                        //int newBucketID = HashCode % (1 << (tBitsCurrent));
                        int newBucketID = HashCode & (int.MaxValue >> (31 - tBitsCurrent));

                        if (newBucketID != tBucketID)
                        {
                            tBucketID = newBucketID;
                            yield return string.Format("Bucket={0} - {1:X} - {2:X}", tBitsCurrent, tBucketID, BitReverse(tBucketID));
                        }
                    }
                }
            }
            #endregion // DebugBuckets
#endif
        }
        #endregion // Sentinel
    }
}
