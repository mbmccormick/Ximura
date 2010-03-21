#region Copyright
// *******************************************************************************
// Copyright (c) 2000-2009 Paul Stancer.
// All rights reserved. This program and the accompanying materials
// are made available under the terms of the Eclipse Public License v1.0
// which accompanies this distribution, and is available at
// http://www.eclipse.org/legal/epl-v10.html
//
// For more details see http://ximura.org
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

using Ximura;
using Ximura.Helper;
using Ximura.Collections;
using Ximura.Collections.Data;
#endregion // using
namespace Ximura.Collections
{
    /// <summary>
    /// This is the base class for class based data arrays.
    /// </summary>
    /// <typeparam name="T">The data type.</typeparam>
    public abstract class MultiLevelBucketStructBasedVertexArray<T, B> : StructBasedVertexArray<T>
    {
        #region Declarations
        private object syncBucketExpand = new object();

        /// <summary>
        /// This is the collection of data and sentinels.
        /// </summary>
        protected LockableWrapper<B>[][] mBuckets;
        /// <summary>
        /// This is the maximum permitted buckets.
        /// </summary>
        private const int cnBucketsLevelMax = 31;
        /// <summary>
        /// This is the threshold to recalculate the LevelCurrent value.
        /// </summary>
        private volatile int mRecalculateThreshold;
        /// <summary>
        /// This is the current number of levels supported by the collection.
        /// </summary>
        private volatile int mBucketsLevelCurrent;
        #endregion // Declarations

        #region BucketsLevelMax
        /// <summary>
        /// The maximum number of levels. You should override this value if you wish to change it.
        /// </summary>
        public virtual int BucketsLevelMax
        {
            get { return cnBucketsLevelMax; }
        }
        #endregion
        #region BucketsLevelCurrent
        /// <summary>
        /// The maximum number of levels.
        /// </summary>
        public virtual int BucketsLevelCurrent
        {
            get { return mBucketsLevelCurrent; }
            protected set
            {
                if (value <= mBucketsLevelCurrent)
                    return;

                mBucketsLevelCurrent = value > BucketsLevelMax ? BucketsLevelMax : value;
            }
        }
        #endregion

        #region HashIDMin
        /// <summary>
        /// This is the minimum hash ID value. This will be set on the initial sentinel vertexes for the data collection.
        /// </summary>
        protected virtual int HashIDMin { get { return 0; } }
        #endregion // HashIDMin

        #region InitializeData()
        /// <summary>
        /// This method initializes the data collection.
        /// </summary>
        protected override void InitializeData(int initialCapacity)
        {
            base.InitializeData(initialCapacity);

            InitializeBucketArray(initialCapacity);
        }
        #endregion // InitializeData()

        #region InitializeBucketArray(int initialCapacity)
        /// <summary>
        /// This method initializes the data array.
        /// </summary>
        protected virtual void InitializeBucketArray(int initialCapacity)
        {
            mBuckets = new LockableWrapper<B>[BucketsLevelMax][];
            mBucketsLevelCurrent = -1;
            mRecalculateThreshold = 0;
            BucketSizeRecalculate(initialCapacity, true);
        }
        #endregion // InitializeDataArray()

        #region BucketSizeRecalculate(int total)
        /// <summary>
        /// This method calculates the current number of bits needed to support the current data.
        /// </summary>
        public virtual void BucketSizeRecalculate(int total, bool firstTime)
        {
            if (!firstTime && mIsFixedSize)
                return;

            int currentLevel = mBucketsLevelCurrent;
            int recalculateThreshold = mRecalculateThreshold;

            if (total < recalculateThreshold || currentLevel == BucketsLevelMax)
                return;

            int newLevel = (int)(Math.Log(total) / elog2);
            int newThreshold = 2 << newLevel;

            if (newLevel > BucketsLevelMax || newLevel <= currentLevel)
                return;

            BucketLevelExpand(currentLevel, newLevel);
            Interlocked.CompareExchange(ref mRecalculateThreshold, newThreshold, recalculateThreshold);
        }
        #endregion

        #region BucketLevelExpand(int currentLevel, int newLevel)
        /// <summary>
        /// This method expands the bucket arrays.
        /// </summary>
        /// <param name="currentLevel">The current level.</param>
        /// <param name="newLevel">The new level required.</param>
        protected virtual void BucketLevelExpand(int currentLevel, int newLevel)
        {
            lock (syncBucketExpand)
            {
                if (currentLevel != BucketsLevelCurrent)
                    return;

                for (int level = currentLevel+1; level <= newLevel; level++)
                {
                    mBuckets[level] = new LockableWrapper<B>[BucketLevelCapacityCalculate(level)];
                }

                Interlocked.CompareExchange(ref mBucketsLevelCurrent, newLevel, currentLevel);

#if (LOCKDEBUG)

                Console.WriteLine("{0} Bucket Expand: {1} -> {2} on {3}", Interlocked.Increment(ref mDebugCounter), currentLevel, newLevel, Thread.CurrentThread.ManagedThreadId);
#endif
            }
        }
        #endregion // BucketLevelExpand(int currentLevel, int newLevel)

        #region BucketCalculatePosition(int indexID, out int level, out int levelPosition)
        /// <summary>
        /// This method calculates the specific bucket level and the position within that bucket.
        /// </summary>
        /// <param name="indexID">The bucket index.</param>
        /// <param name="level">The bucket level.</param>
        /// <param name="levelPosition">The bucket position.</param>
        protected abstract void BucketCalculatePosition(int indexID, out int level, out int levelPosition);
        #endregion
        #region BucketLevelCapacityCalculate(int level)
        /// <summary>
        /// This method calculates the size of the bucket array.
        /// </summary>
        /// <param name="level">The level.</param>
        /// <returns>Returns 2n+1 as the size of the array where n is the level.</returns>
        protected abstract int BucketLevelCapacityCalculate(int level);
        #endregion // BucketLevelSize(int level)

        #region Bucket(int index)
        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns>Returns the bucket.</returns>
        protected internal virtual B Bucket(int index)
        {
            int level, levelPosition;
            BucketCalculatePosition(index, out level, out levelPosition);
            return mBuckets[level][levelPosition].Value;
        }
        #endregion // Bucket(int index)
        #region BucketLock(int index)
        /// <summary>
        /// This method locks a bucket.
        /// </summary>
        /// <param name="index">The index of the bucket to lock.</param>
        /// <returns>Returns the number of lock loops before the call was able to lock and get access.</returns>
        protected internal virtual void BucketLock(int index)
        {
            int level, levelPosition;
            BucketCalculatePosition(index, out level, out levelPosition);
            mBuckets[level][levelPosition].Lock();

#if (LOCKDEBUG)
            Console.WriteLine("{0} Bucket: B{1} Lock -> on thread {2}", Interlocked.Increment(ref mDebugCounter), index, Thread.CurrentThread.ManagedThreadId);
#endif
        }
        #endregion // BucketLock(int index)
        #region BucketUnlock(int index)
        /// <summary>
        /// This method unlocks the bucket.
        /// </summary>
        /// <param name="index">The index of the bucket to unlock.</param>
        protected internal virtual void BucketUnlock(int index)
        {
            int level, levelPosition;
            BucketCalculatePosition(index, out level, out levelPosition);
            mBuckets[level][levelPosition].Unlock();
#if (LOCKDEBUG)

            Console.WriteLine("{0} Bucket: B{1} Unlock -> on thread {2}", Interlocked.Increment(ref mDebugCounter), index, Thread.CurrentThread.ManagedThreadId);
#endif
        }
        #endregion // BucketUnlock(int index)

    }
}
