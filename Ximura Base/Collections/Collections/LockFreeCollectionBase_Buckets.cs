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
        private const int cnHiMask = 0x00800000;

        private const int cnLower28BitMask = 0x00FFFFFF;
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
        /// <summary>
        /// This is the current default(T) item capacity. 
        /// </summary>
        private int mDefaultTCount;
        #endregion 
        #region InitializeBuckets(int capacity)
        /// <summary>
        /// This method initializes the data allocation.
        /// </summary>
        protected virtual void InitializeBuckets(int capacity)
        {
            mCurrentBits = (int)(Math.Log(capacity) / elog2) + 1; ;
            mDefaultTCount = 0;
            mRecalculateThreshold = (int)Math.Pow(2, mCurrentBits); ;
            BitSizeCalculate();
            mBuckets = new ExpandableFineGrainedLockArray<int>(capacity);

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
            int newThreshold = (int)Math.Pow(2, newBits);

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

                int hashCode = mEqualityComparer.GetHashCode(item) & cnLower28BitMask;

                //indexSentinel = GetSentinelVertexID(mCurrentBits, hashCode, canCreate);
                indexSentinel = GetSentinelVertexIDNoRecursion(hashCode, canCreate);

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
        #endregion // HashAndSentinel(T item, out int hashCode, out int indexSentinel)

        #region GetSentinelVertexID(int bucketID, bool canCreate)
        /// <summary>
        /// The method returns the position of a specific sentinel ID based on its bucketID.
        /// </summary>
        /// <param name="currentBits">The number of bits we are interested in.</param>
        /// <param name="hashCode">The hash code for the sentinel we require.</param>
        /// <param name="canCreate">The parameter specifies whether we can create a new sentinel.</param>
        /// <returns>Returns the vertexID of the specific sentinel, or the nearest parent ID.</returns>
        protected virtual int GetSentinelVertexID(int currentBits, int hashCode, bool canCreate)
        {
            int lockSentinelWait = 0;
            int lockSentinelCreate = 0;
            int bucketID = 0;

#if (PROFILING)
            int start = Environment.TickCount;
            int insert = 0;
            int timewait2 = 0;
            try
            {
#endif
                if (currentBits == 0)
                    return cnIndexData;

                //OK, calculate the divisor, this is the number of bits that we are currently interested in for
                //the size of the collection.
                int divisor = 2 << (currentBits - 1);

                //OK, the divisor is 0, so we will just return the initial sentinel.
                //This will happen quite often as we recursively call the GetSentinelVertexID function.
                if (divisor == 0) return cnIndexData;

                //Ok, get the specific bucketID for the hashCode and the divisor.
                bucketID = hashCode % divisor;

                //OK, if we have got down to the bottom, just return the root sentinel.
                if (bucketID == 0 || currentBits == 0) return cnIndexData;

                //Wait if the bucket is locked. This will only happen if the sentinel is being created.
                lockSentinelWait += mBuckets.ItemLockWait(bucketID);

                //Get the sentinel slot id.
                int sentID = mBuckets[bucketID];

                //OK, we have the sentinel ID, so finish.
                if (sentID > 0)
                    return sentID - 1;

                if (currentBits == 1)
                    return cnIndexData;

                //OK, we need to get the parent ID, as this will be the start of the insert for the sentinel vertex.
                //This call will also create the parent sentinels if they have not been created already if we can create.
                int parentSentID = GetSentinelVertexID(currentBits - 1, hashCode, canCreate);

                if (!canCreate)
                    //Ok, we cannot create the bucket, so we will just return the first available parent bucket.
                    return parentSentID;

#if (PROFILING)
                insert = Environment.TickCount;
#endif
                //OK, we need to create the sentinel, and its parents. First we need to lock the bucket.
                lockSentinelCreate += mBuckets.ItemLock(bucketID);
#if (PROFILING)
                timewait2 = Environment.TickCount - insert;
#endif
                try
                {
                    //Let's just check whether another thread has created it in the meantime.
                    sentID = mBuckets[bucketID];
                    if (sentID > 0)
                        return sentID - 1;

                    //OK, we need to create the sentinel vertex.
                    int position = AddInternalWithHashAndSentinel(default(T), BitReverse(bucketID), parentSentID, false);

                    if (position == -1)
                        throw new Exception();

                    //OK, set the sentinel position in the bucket.
                    mBuckets[bucketID] = position + 1;

                    //Return the position.
                    return position;
                }
                finally
                {
                    mBuckets.ItemUnlock(bucketID);
                }
#if (PROFILING)
            }
            finally
            {
                Profile(ProfileAction.Time_GetSentinelVertexID, Environment.TickCount - start);

                if (lockSentinelWait > 0)
                    Profile(ProfileAction.Lock_GetSentinelWait, lockSentinelWait);

                if (lockSentinelCreate > 0)
                    Profile(ProfileAction.Lock_GetSentinelCreate, lockSentinelCreate);

                Profile(ProfileAction.Time_GetSentinelVertexID_TimeWait2, timewait2);

                if (insert > 0)
                    Profile(ProfileAction.Time_GetSentinelVertexID_Insert, Environment.TickCount - insert);


                if (lockSentinelWait > 0)
                    ProfileHotspot(ProfileArrayType.BucketsWait, bucketID);

                if (lockSentinelCreate > 0)
                    ProfileHotspot(ProfileArrayType.BucketsCreate, bucketID);
            }
#endif
        }
        #endregion // GetSentinelVertexID(int bucketID, bool canCreate)

        #region GetSentinelVertexIDNoRecursion(int bucketID, bool canCreate)
        /// <summary>
        /// The method returns the position of a specific sentinel ID based on its bucketID.
        /// </summary>
        /// <param name="hashCode">The hash code for the sentinel we require.</param>
        /// <param name="canCreate">The parameter specifies whether we can create a new sentinel.</param>
        /// <returns>Returns the vertexID of the specific sentinel, or the nearest parent ID.</returns>
        protected virtual int GetSentinelVertexIDNoRecursion(int hashCode, bool canCreate)
        {
            //int lockSentinelWait = 0;
            int lockSentinelCreate = 0;

            int startBits = mCurrentBits;
            int currentBits = startBits;
            int sentID = 0;
            int bucketID =0;

            while (true)
            {
                //OK, calculate the divisor, this is the number of bits that we are currently interested in for
                //the size of the collection.
                int divisor = 1 << (currentBits);
                //Ok, get the specific bucketID for the hashCode and the divisor.
                bucketID = hashCode % divisor;

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
                if (canCreate & currentBits<startBits)
                    break;
                //No, then just return the current sentinel ID.
                return sentID - 1;
            }

            int parentSentID;
            //int parentBucketID = bucketID;

            //Ok, let's loop up from the parent and create the child sentinels if they haven't been created anyway.
            for (currentBits++; currentBits <= startBits; currentBits++)
            {
                parentSentID = sentID;
                //OK, calculate the divisor, this is the number of bits that we are currently interested in for
                //the size of the collection.
                int divisor = 1 << (currentBits);
                //Ok, get the specific bucketID for the hashCode and the divisor.
                bucketID = hashCode % divisor;

                //if (bucketID != parentBucketID)
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
                    sentID = AddInternalWithHashAndSentinel(default(T), BitReverse(bucketID), parentSentID - 1, false);

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

            }

            //Ok, return the sentinel.
            return sentID - 1;
        }
        #endregion
    }
}
