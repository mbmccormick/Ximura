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
using Ximura.Collections.Data;
#endregion // using
namespace Ximura.Collections
{
    /// <summary>
    /// The vertex array class provides basic linked list expandable array support.
    /// </summary>
    /// <typeparam name="T">The collection data type.</typeparam>
    public abstract class VertexArray<T> : IVertexArray<T>
    {
        #region Constants
        protected const double elog2 = 0.693147181;
        protected const int cnHiMask = 0x08000000;
        protected const int cnLowerBitMask = 0x07FFFFFF;
        #endregion // Declarations
        #region BitReverse(int data)
        /// <summary>
        /// This method reverses the hashcode so that it is ordered in reverse based on bit value, i.e.
        /// xxx1011 => 1101xxxx => Bucket 1 1xxxxx => Bucket 3 11xxxxx => Bucket 6 110xxx etc.
        /// </summary>
        /// <param name="data">The data to reverse></param>
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
        #region Declarations
        /// <summary>
        /// This is the collection comparer.
        /// </summary>
        protected IEqualityComparer<T> mEqComparer;
        #endregion // Declarations

        #region Initialize(bool isFixedSize, int capacity, bool allowNullValues, bool allowMultipleEntries)
        /// <summary>
        /// This method initializes the data collection.
        /// </summary>
        /// <param name="eqComparer">The equality comparer for the collection.</param>
        /// <param name="isFixedSize">Specifies whether the collection is a fixed size.</param>
        /// <param name="capacity">The initial capacity.</param>
        /// <param name="allowNullValues">This boolean values specifies whether null values are allowed in the collection.</param>
        /// <param name="allowMultipleEntries">This boolean value specicifies whether the collection allows items to exist 
        /// more than once in the collection.</param>
        public virtual void Initialize(IEqualityComparer<T> eqComparer, 
            bool isFixedSize, int capacity, bool allowNullValues, bool allowMultipleEntries)
        {
            mEqComparer = eqComparer;

            mVersion = int.MinValue;
            mCount = 0;
            mDefaultTCount = 0;

            mContainScanMissThreshold = 0;
            mContainScanUnlockedMiss = 0;

            mIsFixedSize = isFixedSize;

            mInitialCapacity = capacity;
            mAllowNullValues = allowNullValues;
            mAllowMultipleEntries = allowMultipleEntries;
        }
        #endregion
        
        #region IEnumerable<KeyValuePair<int,Vertex<T>>> Members
        /// <summary>
        /// This method returns an enumeration through the sentinels and data in the collection.
        /// </summary>
        /// <returns>Returns an enumeration containing the collection data.</returns>
        public abstract IEnumerator<KeyValuePair<int, ICollectionVertex<T>>> GetEnumerator();

        #endregion
        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator)GetEnumerator();
        }

        #endregion

        #region VertexWindowGet(int index)/VertexWindowGet(int hashCode, bool createSentinel)
        /// <summary>
        /// This method returns a vertex window for the first item in the array.
        /// </summary>
        public abstract IVertexWindow<T> VertexWindowGet();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="hashCode"></param>
        /// <param name="createSentinel"></param>
        /// <returns></returns>
        public abstract IVertexWindow<T> VertexWindowGet(T item, bool createSentinel);
        #endregion

        #region SizeRecalculate(int total)
        /// <summary>
        /// This method calculates the current number of bits needed to support the current data.
        /// </summary>
        public abstract void SizeRecalculate(int total);
        #endregion

        #region Fast Access
        public virtual bool SupportsFastContain { get { return false; } }

        public virtual bool? FastContains(T item)
        {
            throw new NotSupportedException();
        }

        public virtual bool? FastContains(IEqualityComparer<T> eqComparer, T item, out T value)
        {
            throw new NotSupportedException();
        }

        public virtual bool SupportsFastAdd { get { return false; } }

        public virtual bool FastAdd(T item, bool add)
        {
            throw new NotSupportedException();
        }        
        
        public virtual bool SupportsFastRemove { get { return false; } }

        public virtual bool FastRemove(T item)
        {
            throw new NotSupportedException();
        }        
        
        public virtual bool SupportsFastClear { get { return false; } }

        public virtual void FastClear()
        {
            throw new NotSupportedException();
        }

        #endregion // Fast Access

        #region State
        #region InitialCapacity
        private int mInitialCapacity;
        /// <summary>
        /// This is the initial capacity of the collection.
        /// </summary>
        public int InitialCapacity { get { return mInitialCapacity; } }
        #endregion // InitialCapacity
        #region IsFixedSize
        /// <summary>
        /// This property specifies whether the collection is a fixed size.
        /// </summary>
        private bool mIsFixedSize;
        /// <summary>
        /// This property determines whether the collection is a fixed size. Fixed size collections will reject new records
        /// when the capacity has been reached.
        /// </summary>
        public bool IsFixedSize { get { return mIsFixedSize; } }
        #endregion // IsFixedSize
        #region AllowNullValues
        private bool mAllowNullValues;
        /// <summary>
        /// This property determines whether the collection will allow null or default(T) values.
        /// </summary>
        public bool AllowNullValues { get { return mAllowNullValues; } }
        #endregion // AllowNullValues
        #region AllowMultipleEntries
        private bool mAllowMultipleEntries;
        /// <summary>
        /// This property specifies whether the collection accepts multiple entries of the same object.
        /// </summary>
        public bool AllowMultipleEntries { get { return mAllowMultipleEntries; } }
        #endregion // AllowMultipleEntries

        #region Version
        /// <summary>
        /// The version value.
        /// </summary>
        protected volatile int mVersion;
        /// <summary>
        /// This is the public version value.
        /// </summary>
        public virtual int Version { get { return mVersion; } }

        public virtual bool VersionCompare(int version)
        {
            return mVersion == version;
        }
        #endregion
        #region Count
        /// <summary>
        /// This is the current item count.
        /// </summary>
        private volatile int mCount;
        /// <summary>
        /// This is the public count value.
        /// </summary>
        public int Count { get { return mCount; } }
        #endregion // Count
        #region CountIncrement()
        /// <summary>
        /// This method increments the count.
        /// </summary>
        public virtual int CountIncrement()
        {
            Interlocked.Increment(ref mVersion);
            return Interlocked.Increment(ref mCount);
        }
        #endregion // CountIncrement()
        #region CountIncrement(int value)
        /// <summary>
        /// This method increments the count by a specific value.
        /// </summary>
        /// <param name="value">The value to increment the count by.</param>
        public virtual int CountIncrement(int value)
        {
            Interlocked.Increment(ref mVersion);
            return Interlocked.Add(ref mCount, value);
        }
        #endregion // CountIncrement(int value)
        #region CountDecrement()
        /// <summary>
        /// This method decrements the count.
        /// </summary>
        public virtual void CountDecrement()
        {
            Interlocked.Increment(ref mVersion);
            Interlocked.Decrement(ref mCount);
        }
        #endregion // CountDecrement()

        #region DefaultTCount
        /// <summary>
        /// This is the current default(T) item capacity. 
        /// </summary>
        private volatile int mDefaultTCount;
        /// <summary>
        /// This is the public DefaultTCount value.
        /// </summary>
        public int DefaultTCount { get { return mDefaultTCount; } }
        #endregion // DefaultTCount
        #region DefaultTAdd()
        /// <summary>
        /// This method increments the defaultT value where appropriate.
        /// </summary>
        /// <returns>Returns true if the operation was successful.</returns>
        public bool DefaultTAdd()
        {
            if (!mAllowNullValues)
                throw new ArgumentNullException("Null values are not accepted in this collection.");

            if (!mAllowMultipleEntries)
            {
                if (mDefaultTCount > 0)
                    return false;

                if (Interlocked.CompareExchange(ref mDefaultTCount, 1, 0) != 0)
                    return false;
            }
            else
                Interlocked.Increment(ref mDefaultTCount);

            CountIncrement();
            return true;
        }
        #endregion // AddDefaultT()
        #region DefaultTDelete()
        /// <summary>
        /// This method returns true if the defaultT count is greater than 0. This method then decreases the count.
        /// </summary>
        /// <returns>Returns true if the operation was successful.</returns>
        public bool DefaultTDelete()
        {
            if (!AllowNullValues)
                return false;

            int currentCount = mDefaultTCount;
            if (currentCount == 0)
                return false;

            //We check whether another thread has changes the value before us.
            while (Interlocked.CompareExchange(ref mDefaultTCount, currentCount - 1, currentCount) != currentCount)
            {
                currentCount = mDefaultTCount;

                if (currentCount == 0)
                    return false;
            }

            CountDecrement();
            return true;
        }
        #endregion // RemoveDefaultT()
        #region DefaultTContains()
        /// <summary>
        /// This method returns true if the collection contains DefaultT data.
        /// </summary>
        /// <returns>Returns true if the DefaultTCount is greater than 0.</returns>
        public bool DefaultTContains()
        {
            return mAllowNullValues ? mDefaultTCount > 0 : false;
        }
        #endregion // DefaultTContains()
        #region DefaultTClear()
        /// <summary>
        /// This method clears the defaultT counters and changes the version number.
        /// </summary>
        public void DefaultTClear()
        {
            if (AllowNullValues)
            {
                int currentCount = mDefaultTCount;
                //We check whether another thread has changes the value before us.
                while (Interlocked.CompareExchange(ref mDefaultTCount, 0, currentCount) != currentCount)
                    currentCount = mDefaultTCount;

                if (currentCount > 0)
                    CountIncrement(currentCount * -1);
            }
        }
        #endregion // DefaultTClear()

        #region ContainScanUnlockedMiss()
        /// <summary>
        /// This variable contains the number of scan misses.
        /// </summary>
        private volatile int mContainScanUnlockedMiss;
        /// <summary>
        /// This method increments the unlocked scan miss count.
        /// </summary>
        public void ContainScanUnlockedMiss()
        {
            Interlocked.Increment(ref mContainScanUnlockedMiss);
        }
        #endregion // ContainScanUnlockedMiss()
        #region ContainScanUnlocked
        /// <summary>
        /// This property specifies whether the contains operation should attempt to scan without locking.
        /// </summary>
        public bool ContainScanUnlocked { get { return mContainScanMissThreshold == -1 ? true : mContainScanUnlockedMiss < mContainScanMissThreshold; } }
        /// <summary>
        /// This is the threshold where locked scans should be processed.
        /// </summary>
        private int mContainScanMissThreshold;
        /// <summary>
        /// This is the threshhold where missed scans will default to a locked scan automatically.
        /// </summary>
        public int ContainScanMissThreshold
        {
            get { return mContainScanMissThreshold; }
            set { mContainScanMissThreshold = value; }
        }
        #endregion // ContainScanUnlocked
        #endregion // State
    }
}
