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
            int loMask = cnLoMask;
            int hiMask = cnHiMask;
            int result = 0;

            for (int i = 0; i < cnWordsize; i++)
            {
                if ((data & hiMask) != 0)
                    result |= loMask;

                loMask <<= 1;
                hiMask >>= 1;
            }

            return result;
        }
        
        protected uint BitReverse(uint data, int wordSize, uint hiMask)
        {
            uint loMask = 1;
            uint result = 0;

            for (int i = 0; i < wordSize; i++)
            {
                if ((data & hiMask) != 0)
                    result |= loMask;

                loMask <<= 1;
                hiMask >>= 1;
            }

            return result;
        }
        #endregion // BitReverse(int data, int wordSize)

        #region GetHashCodeInternal(T item)
        /// <summary>
        /// This method returns the hash code for the item.
        /// </summary>
        /// <param name="item">The item to compute.</param>
        /// <returns>The hash code.</returns>
        protected virtual int GetHashCodeInternal(T item)
        {
            //There is a special case for null or default(T) value that exist before the sentinel vertexes.
            //This is because we need some way to show that the vertex is a sentinel node, without adding
            //additional data storage overheads and null values cause confusion during the scan.
            if (mEqualityComparer.Equals(item,default(T)))
                return -1;

            return CalculateHashCode(item);
        }
        #endregion // IncreaseCapacity()
        #region CalculateHashCode(T item)
        /// <summary>
        /// This method calculates the hash code for a particular item. You should override this method if you need
        /// to provide a specific algorithm for your class or structure type to minimize hash set collisions.
        /// </summary>
        /// <param name="item">The collection item.</param>
        /// <returns>Returns the hash code of the item as an integer.</returns>
        protected virtual int CalculateHashCode(T item)
        {
            return mEqualityComparer.GetHashCode(item) & cnLower28BitMask;
        }
        #endregion // CalculateHashCode(T item)

        #region HashAndSentinel(T item, out int hashCode, out int indexSentinel)
        /// <summary>
        /// This method calcualtes the hashcode and finds the sentinel position to start the search.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="canCreate">The boolean property specifies whether new sentinel vertexes can be created.</param>
        /// <param name="hashCode">Returns the item hashcode.</param>
        /// <param name="indexSentinel">Returns the item sentinel position.</param>
        protected virtual void HashAndSentinel(T item, bool canCreate, out int hashCode, out int indexSentinel)
        {
#if (PROFILING)
            int start = Environment.TickCount;
            int midway=0;
            int findSent=0;
            try
            {
#endif
                hashCode = GetHashCodeInternal(item);

#if (PROFILING)
                midway = Environment.TickCount;
#endif

                indexSentinel = FindSentinel(hashCode, canCreate);

#if (PROFILING)
                findSent = Environment.TickCount;
#endif
                if (hashCode == -1)
                    return;

                hashCode = BitReverse(hashCode);
#if (PROFILING)
            }
            finally
            {
                Profile(ProfileAction.Time_HashAndSentinel, Environment.TickCount - start);
                Profile(ProfileAction.Time_HashAndSentinel_GetHashCode, midway - start);
                Profile(ProfileAction.Time_HashAndSentinel_FindSentinel, findSent - midway);
            }
#endif
        }
        #endregion // HashAndSentinel(T item, out int hashCode, out int indexSentinel)

        #region FindSentinel(int hashCode)
        /// <summary>
        /// This method finds the sentinel for the specific hashcode.
        /// </summary>
        /// <param name="hashCode">The hashcode for the item to search.</param>
        /// <param name="canCreate">The boolean property specifies whether new sentinel vertexes can be created.</param>
        /// <returns>Returns the sentinel vertex index.</returns>
        protected virtual int FindSentinel(int hashCode, bool canCreate)
        {
            if (hashCode == -1)
                return -1;

            return GetSentinelVertexID(mCurrentBits, hashCode, canCreate);
        }
        #endregion

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
            int timewait1 = 0;
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
                if (divisor == 0)
                    return cnIndexData;

                //Ok, get the specific bucketID for the hashCode and the divisor.
                bucketID = hashCode % divisor;

                //OK, if we have got down to the bottom, just return the root sentinel.
                if (bucketID == 0 || currentBits == 0)
                    return cnIndexData;
#if (PROFILING)
                timewait1 = Environment.TickCount;
#endif
                //Wait if the bucket is locked. This will only happen if the sentinel is being created.
                lockSentinelWait += mBuckets.ItemLockWait(bucketID);

#if (PROFILING)
                timewait1 = Environment.TickCount - timewait1;
#endif
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

                if (lockSentinelWait>0)
                    Profile(ProfileAction.Lock_GetSentinelWait, lockSentinelWait);

                if (lockSentinelCreate>0)
                    Profile(ProfileAction.Lock_GetSentinelCreate, lockSentinelCreate);

                Profile(ProfileAction.Time_GetSentinelVertexID_TimeWait1, timewait1);
                Profile(ProfileAction.Time_GetSentinelVertexID_TimeWait2, timewait2);

                if (insert>0)
                    Profile(ProfileAction.Time_GetSentinelVertexID_Insert, Environment.TickCount - insert);


                if (lockSentinelWait > 0)
                    ProfileHotspot(ProfileArrayType.BucketsWait, bucketID);

                if (lockSentinelCreate > 0)
                    ProfileHotspot(ProfileArrayType.BucketsCreate, bucketID);
            }
#endif
        }
        #endregion // GetSentinelVertexID(int bucketID, bool canCreate)
    }
}
