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
    public abstract class MultiLevelStructBasedVertexArray<T, B> : StructBasedVertexArray<T>
    {
        #region Declarations
        /// <summary>
        /// This is the collection of data and sentinels.
        /// </summary>
        protected B[] mBuckets;
        /// <summary>
        /// This is the maximum permitted buckets.
        /// </summary>
        private const int cnMaxBuckets = 31;
        /// <summary>
        /// This is the threshold to recalculate the LevelCurrent value.
        /// </summary>
        private volatile int mRecalculateThreshold;
        /// <summary>
        /// This is the current number of levels supported by the collection.
        /// </summary>
        private volatile int mLevelCurrent;
        #endregion // Declarations

        #region LevelMax
        /// <summary>
        /// The maximum number of levels. You should override this value if you wish to change it.
        /// </summary>
        public virtual int LevelMax
        {
            get { return cnMaxBuckets; }
        }
        #endregion
        #region LevelCurrent
        /// <summary>
        /// The maximum number of levels.
        /// </summary>
        public virtual int LevelCurrent
        {
            get { return mLevelCurrent; }
            protected set
            {
                if (value <= mLevelCurrent)
                    return;

                mLevelCurrent = value > LevelMax ? LevelMax : value;
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
        protected override void InitializeData()
        {
            base.InitializeData();

            mLevelCurrent = 0;
            mRecalculateThreshold = 0;

            InitializeBucketArray();

            SizeRecalculate(InitialCapacity);
        }
        #endregion // InitializeData()

        #region InitializeBucketArray()
        /// <summary>
        /// This method initializes the data array.
        /// </summary>
        protected abstract void InitializeBucketArray();
        #endregion // InitializeDataArray()

        #region SizeRecalculate(int total)
        /// <summary>
        /// This method calculates the current number of bits needed to support the current data.
        /// </summary>
        public override void SizeRecalculate(int total)
        {
            int currentLevel = mLevelCurrent;
            int recalculateThreshold = mRecalculateThreshold;

            if (total < recalculateThreshold || currentLevel == LevelMax)
                return;

            int newLevel = (int)(Math.Log(total) / elog2);
            int newThreshold = 2 << newLevel;

            if (newLevel > LevelMax || newLevel <= currentLevel)
                return;

            BucketLevelExpand(currentLevel, newLevel);
            Interlocked.CompareExchange(ref mRecalculateThreshold, newThreshold, recalculateThreshold);
        }
        #endregion
        #region BucketLevelExpand(int currentLevel, int newLevel)
        /// <summary>
        /// This method changes the current bucket level to the larger value.
        /// </summary>
        /// <param name="currentLevel">The current level.</param>
        /// <param name="newLevel">The new level.</param>
        protected virtual void BucketLevelExpand(int currentLevel, int newLevel)
        {
            Interlocked.CompareExchange(ref mLevelCurrent, newLevel, currentLevel);
        }
        #endregion // SizeLevelExpand(int currentLevel, int newLevel)

    }
}
