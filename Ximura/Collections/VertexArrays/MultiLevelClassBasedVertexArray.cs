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

using Ximura.Collections;
using Ximura.Collections.Data;
#endregion // using
namespace Ximura.Collections
{
    /// <summary>
    /// This is the base class for class based data arrays.
    /// </summary>
    /// <typeparam name="T">The data type.</typeparam>
    public abstract class MultiLevelClassBasedVertexArray<T> : ClassBasedVertexArray<T>
    {
        #region Declarations
        /// <summary>
        /// This is the collection of data and sentinels.
        /// </summary>
        protected CollectionVertexClass<T>[] mData;
        /// <summary>
        /// This is the maximum permitted buckets.
        /// </summary>
        private const int cnMaxBuckets = 30;
        /// <summary>
        /// This is the threshold to recalculate the LevelCurrent value.
        /// </summary>
        private int mRecalculateThreshold;
        /// <summary>
        /// This is the current number of levels supported by the collection.
        /// </summary>
        private int mLevelCurrent;
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
            set
            {
                if (value <= mLevelCurrent)
                    return;

                mLevelCurrent = value > LevelMax ? LevelMax : value;
            }
        }
        #endregion
        #region LevelData
        /// <summary>
        /// This is the level where the actual data is stored in the collection.
        /// </summary>
        protected virtual int LevelData { get { return 0; } }
        #endregion // LevelData

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
            mLevelCurrent = 0;
            mRecalculateThreshold = 0;

            InitializeDataSize();

            InitializeDataArray();
        }
        #endregion // InitializeData()
        #region InitializeDataSize()
        /// <summary>
        /// This protected method recalculates the size of the collection based on a standard settings of 1000 items.
        /// You should override this method to change the value.
        /// </summary>
        protected virtual void InitializeDataSize()
        {
            BucketSizeRecalculate(1000);
        }
        #endregion // InitializeDataSize()

        #region InitializeDataArray()
        /// <summary>
        /// This method initializes the data array.
        /// </summary>
        protected virtual void InitializeDataArray()
        {
            mData = new CollectionVertexClass<T>[LevelMax];

            mData[LevelData] = new CollectionVertexClassDataSentinel<T>(HashIDMin);

            for (int index = 1; index < LevelMax; index++)
            {
                mData[index] = new CollectionVertexClassSentinel<T>(HashIDMin, mData[index - 1]);
            }
        }
        #endregion // InitializeDataArray()

        #region SizeRecalculate(int total)
        /// <summary>
        /// This method calculates the current number of bits needed to support the current data.
        /// </summary>
        public virtual void BucketSizeRecalculate(int total)
        {
            int currentLevel = mLevelCurrent;
            int recalculateThreshold = mRecalculateThreshold;

            if (total < recalculateThreshold)
                return;

            if (currentLevel == LevelMax)
                return;

            int newLevel = (int)(Math.Log(total) / elog2);
            int newThreshold = 2 << newLevel;

            if (newLevel > LevelMax)
                return;

            if (newLevel > currentLevel)
                Interlocked.CompareExchange(ref mLevelCurrent, newLevel, currentLevel);

            if (newThreshold > recalculateThreshold)
                Interlocked.CompareExchange(ref mRecalculateThreshold, newThreshold, recalculateThreshold);
        }
        #endregion

        #region Root
        /// <summary>
        /// This method returns the root data vertex which is the last item in the data array.
        /// </summary>
        protected override CollectionVertexClass<T> Root
        {
            get { return mData[LevelData]; }
        }
        #endregion // Root

        protected override CollectionVertexClass<T> GetSentinelID(int hashCode, bool createSentinel, out int hashID)
        {
            throw new NotImplementedException();
        }

        public override int InitialCapacity
        {
            get { throw new NotImplementedException(); }
        }

        public override int Capacity
        {
            get { throw new NotImplementedException(); }
        }
    }
}
