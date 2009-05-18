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
        private const int cnWordsize = 24;
        private const int cnLoMask = 0x00000001;
        private const int cnHiMask = 0x08000000;

        private const int cnLowerBitMask = 0x07FFFFFF;
        private const int cnLower31BitMask = 0x7fffffff;

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
            mCurrentBits = (int)(Math.Log(capacity) / elog2) + 1;
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

            int newBits = (int)(Math.Log(total) / elog2)+1;
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

        #region BitReverse(int data, int wordSize)
        /// <summary>
        /// This method reverses the hashcode so that it is ordered in reverse based on bit value, i.e.
        /// xxx1011 => 1101xxxx => Bucket 1 1xxxxx => Bucket 3 11xxxxx => Bucket 6 110xxx etc.
        /// </summary>
        /// <param name="data"The data to reverse></param>
        /// <param name="wordSize">The number of bits to reverse.</param>
        /// <returns>Returns the reversed data</returns>
        protected int BitReverse(int data)
        {
            int result = 0;
            int hiMask = cnHiMask;

            for (; data > 0; data >>= 1)
            {
                if ((data & 1) > 0)
                    result |= hiMask;
                hiMask >>= 1;
            }

            return result;
        }
        #endregion // BitReverse(int data, int wordSize)

        #region GetHashAndSentinel(T item, bool canCreate, out int hashID, out int indexSentinel)
        /// <summary>
        /// This method calcualtes the hashcode and finds the sentinel position to start the search.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="canCreate">The boolean property specifies whether new sentinel vertexes can be created.</param>
        /// <param name="hashID">Returns the item hashcode.</param>
        /// <param name="indexSentinel">Returns the item sentinel position.</param>
        /// <returns>Returns false if the item is equal to default(T), otherwise returns true.</returns>
        protected virtual bool GetHashAndSentinel(T item, bool canCreate, out int hashID, out int indexSentinel)
        {
#if (PROFILING)
            int start = Environment.TickCount;
            try
            {
#endif
                if (mEqualityComparer.Equals(item, default(T)))
                {
                    hashID = -1;
                    indexSentinel = -1;
                    return false;
                }

                int hashCode = mEqualityComparer.GetHashCode(item) & cnLowerBitMask;

                indexSentinel = GetSentinelVertexID(hashCode, canCreate);

                hashID = BitReverse(hashCode);

                return true;
#if (PROFILING)
            }
            finally
            {
                Profile(ProfileAction.Time_HashAndSentinel, Environment.TickCount - start);
            }
#endif
        }
        #endregion
        #region GetSentinelVertexID(int hashCode, bool canCreate)
        /// <summary>
        /// The method returns the position of a specific sentinel ID based on its bucketID.
        /// </summary>
        /// <param name="hashCode">The hash code for the sentinel we require.</param>
        /// <param name="canCreate">The parameter specifies whether we can create a new sentinel.</param>
        /// <returns>Returns the vertexID of the specific sentinel, or the nearest parent ID.</returns>
        protected virtual int GetSentinelVertexID(int hashCode, bool canCreate)
        {
            //int lockSentinelWait = 0;
            int lockSentinelCreate = 0;

            int startBits = mCurrentBits;
            int currentBits = startBits;
            int sentID = 0;
            int bucketID = 0;

            while (true)
            {
                bucketID = hashCode % (1 << (currentBits));

                //Get the sentinel slot id. There is no need to check for lock as the value is atomic and will not move once
                //it has been set, and any unnecessary recursion will be skipped in the next stage.
                //Also, there is no need to check for the currentBits going under 0 as the bucketID will be zero and will returns 
                //the root vertex.
                sentID = mBuckets[bucketID];

                if (sentID == 0)
                {
                    currentBits--;
                    continue;
                }

                //OK, we have the sentinel ID, so finish.
                //If we can create new sentinels and have looped down to the parent then continue
                if (canCreate & currentBits < startBits)
                    break;

                //No, then just return the current sentinel ID.
                return sentID - 1;
            }

            int parentSentID = sentID;
            int parentBucketID = bucketID;

            //Ok, let's loop up from the parent and create the parent sentinels if they haven't been created anyway.
            for (currentBits++; currentBits <= startBits; currentBits++)
            {
                bucketID = hashCode % (1 << (currentBits));

                if (bucketID != parentBucketID)
                    try
                    {
                        //OK, we need to lock the bucket
                        lockSentinelCreate += mBuckets.ItemLock(bucketID);

                        //Let's just check whether another thread has created it in the meantime.
                        sentID = mBuckets[bucketID];

                        //Ok, it has already been set so move up to the child sentinel.
                        if (sentID > 0)
                            continue;

                        //OK, we need to create the sentinel vertex.
                        sentID = AddInternalWithHashAndSentinel(true, default(T), BitReverse(bucketID), parentSentID - 1, false);

                        if (sentID == -1)
                            throw new Exception();
                        else
                            sentID++;

                        //OK, set the sentinel position in the bucket.
                        mBuckets[bucketID] = sentID;
                    }
                    finally
                    {
                        //Unlock the bucket we have locked.
                        mBuckets.ItemUnlock(bucketID);
                    }

                parentBucketID = bucketID;
            }

            //Ok, return the sentinel.
            return sentID - 1;
        }
        #endregion
    }
}
