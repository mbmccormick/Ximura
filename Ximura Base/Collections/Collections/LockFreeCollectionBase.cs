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

using Ximura;
using Ximura.Helper;
#endregion // using
namespace Ximura.Collections
{
    /// <summary>
    /// The LockFreeCollectionBase class provides generic multi-threaded collection based functionality. The class is designed
    /// to maximize the throughput of the collection in high speed multi-threaded scenarios.
    /// </summary>
    /// <typeparam name="T">The collection class or structure type.</typeparam>
    public abstract partial class LockFreeCollectionBase<T> : IEnumerable<T>, IDisposable
    {
        #region Declarations
        /// <summary>
        /// This property specifies whether the contains operation should attempt to scan without locking.
        /// </summary>
        private bool mContainScanUnlocked;

        /// <summary>
        /// This variable contains the number of scan misses.
        /// </summary>
        private int mContainScanUnlockedMiss = 0;
        /// <summary>
        /// This variables determines whether the collection has been disposed.
        /// </summary>
        private bool mDisposed;
        /// <summary>
        /// This is the equality comparer for the collection.
        /// </summary>
        protected EqualityComparer<T> mEqualityComparer;

        /// <summary>
        /// The version value is set for integer for 32bit systems.
        /// </summary>
        protected int mVersion;
        /// <summary>
        /// This is the current item count.
        /// </summary>
        protected int mCount;

        /// <summary>
        /// This property determines whether the collection is a fixed size. Fixed size collections will reject new records
        /// when the capacity has been reached.
        /// </summary>
        private bool mIsFixedSize;
        /// <summary>
        /// This property determines whether the collection will allow null or default(T) values.
        /// </summary>
        private bool mAllowNullValues;
        /// <summary>
        /// This property specifies whether the collection accepts multiple entries of the same object.
        /// </summary>
        private bool mAllowMultipleEntries;
        /// <summary>
        /// This is the current default(T) item capacity. 
        /// </summary>
        private int mDefaultTCount;
        #endregion
        #region Constructor
        /// <summary>
        /// This is constructor for the abstract list class.
        /// </summary>
        /// <param name="comparer">The comparer for the collection items.</param>
        /// <param name="capacity">The initial capacity for the collection.</param>
        /// <param name="collection">The initial data to load to the collection.</param>
        /// <param name="isFixedSize">This property determines whether the collection is a fixed size.
        /// Fixed size collections will reject new records when the capacity has been reached, 
        /// although they may deliver performance improvements as they do not need to use a growable data structure.</param>
        protected LockFreeCollectionBase(EqualityComparer<T> comparer, int capacity, IEnumerable<T> collection, bool isFixedSize)
        {
            mIsFixedSize = isFixedSize;
#if (PROFILING)
            ProfilingSetup();
#endif
            mEqualityComparer = (comparer == null) ? EqualityComparer<T>.Default : comparer;

            Initialize(capacity, collection);
        }
        #endregion // Constructor

        #region IDisposable
        #region Finalizer
        /// <summary>
        /// This is the finalizer for the collection.
        /// </summary>
        ~LockFreeCollectionBase()
        {
            this.Dispose(false);
        }
        #endregion
        #region DisposedCheck()
        /// <summary>
        /// This method identifies when the collection has been disposed and throws an ObjectDisposedException.
        /// </summary>
        /// <exception cref="System.ObjectDisposedException">This exception is thrown when the collection has been disposed.</exception>
        protected void DisposedCheck()
        {
            if (mDisposed)
                throw new ObjectDisposedException(GetType().ToString(), "Collection has been disposed.");
        }
        #endregion // DisposedCheck()
        #region Dispose()
        /// <summary>
        /// This method disposes of the collection.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion // Dispose()
        #region Dispose(bool disposing)
        /// <summary>
        /// This method disposes of the data in the collection. You should override this method if you need to add
        /// custom dispose logic to your collection.
        /// </summary>
        /// <param name="disposing">The class is disposing, i.e. this is called by Dispose and not the finalizer.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                mDisposed = true;
            }
        }
        #endregion // Dispose(bool disposing)
        #endregion // IDisposable

        #region Initialize(int capacity, IEnumerable<T> collection)
        /// <summary>
        /// This method initializes the collection.
        /// </summary>
        /// <param name="capacity">The initial capacity.</param>
        /// <param name="collection">The initial data to load in to the array.</param>
        protected virtual void Initialize(int capacity, IEnumerable<T> collection)
        {
            if (capacity < 0)
                throw new ArgumentOutOfRangeException("The capacity cannot be less than 0.");

            mDisposed = false;
            mCount = 0;
            mVersion = int.MinValue;

            mAllowNullValues = typeof(T).IsValueType || CollectionAllowNullValues;
            mAllowMultipleEntries = CollectionAllowMultipleEntries;

            mDefaultTCount = 0;
            mContainScanUnlocked = true;

            InitializeBuckets(capacity, CollectionIsFixedSize);
            InitializeData(capacity, CollectionIsFixedSize);

            if (collection != null)
                collection.ForEach(i => AddInternal(i));
        }
        #endregion // Initialize(int capacity)

        #region CollectionAllowMultipleEntries
        /// <summary>
        /// This setting determines whether the collection allows multiple entries of the same object in the collection.
        /// The default setting is true.
        /// </summary>
        protected virtual bool CollectionAllowMultipleEntries{get{return false;}}
        #endregion
        #region CollectionAllowNullValues
        /// <summary>
        /// This property determines whether the collection will accept null values. The default setting is true.
        /// </summary>
        /// <remarks>This property is ignored if the collection is for a value type such as int.</remarks>
        protected virtual bool CollectionAllowNullValues { get { return true; } }
        #endregion
        #region CollectionIsFixedSize
        /// <summary>
        /// This property determines whether the collection will dynamically expand when new values are added.
        /// </summary>
        protected virtual bool CollectionIsFixedSize { get { return mIsFixedSize; } }
        #endregion

        #region AddInternal(T item)
        /// <summary>
        /// This method adds an item to the collection.
        /// </summary>
        /// <param name="item">The item to add.</param>
        /// <returns>Returns true if the addition is successful.</returns>
        protected virtual bool AddInternal(T item)
        {
#if (PROFILING)
            int hopsData = 0;
            int start = Environment.TickCount;
            int has = 0;
            int hopsSentinel = 0;
            int hopsBucketSkip = 0;
            try
            {
#endif
                try
                {
                    //Get the hash code and sentinel ID for the item.
                    //Is this a null or default value?
                    if (mEqualityComparer.Equals(item, default(T)))
                    {
                        #region Null Checking
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

                        Interlocked.Increment(ref mCount);
                        Interlocked.Increment(ref mVersion);
                        return true;
                        #endregion
                    }

                    int hashCode = mEqualityComparer.GetHashCode(item);

                    //Add any required sentinels and retrieve the nearest sentinel slot ID.
                    Sentinel sent = new Sentinel(hashCode, mCurrentBits, mBuckets);
                    int hashID = sent.HashIDCalculate();

                    VertexWindow<T> vWin = new VertexWindow<T>();

                    int tBitsCurrent = sent.BitsCurrent;
                    int tBitsStart = sent.BitsStart;
                    int tBucketID = sent.BucketID;

                    while (tBitsCurrent < tBitsStart)
                    {
                        tBitsCurrent++;

                        //int newBucketID = HashCode % (1 << (tBitsCurrent));
                        int bucketID = sent.HashCode & (int.MaxValue >> (31 - tBitsCurrent));

                        if (bucketID == tBucketID)
                            continue;

                        tBucketID = bucketID;

                        mBuckets.ItemLock(bucketID);
                        //Check whether the bucket has been created, shouldn't happen but best to be safe.
                        if (mBuckets[bucketID] > 0)
                        {
                            mBuckets.ItemUnlock(bucketID);
                            continue;
                        }

                        int bucketHashID = Sentinel.BitReverse(bucketID);

                        //Ok, insert the sentinel.
                        vWin.SlotsSetCurrentAndLock(mSlots, sent.SlotID);

                        //Scan for the correct position to insert the sentinel.
                        vWin.SlotsScanAndLock(mSlots, bucketHashID);

                        //Get a new slot for the sentinel.
                        int newPosition = EmptyGet();

                        //Insert the new sentinel.
                        vWin.SlotsInsertSentinel(mSlots, newPosition, bucketHashID);

                        //Ok, unlock the bucket.
                        mBuckets[bucketID] = newPosition + 1;

                        sent.SlotID = newPosition;
                        sent.BucketIDParent = bucketID;

                        vWin.SlotsUnlock(mSlots);
                        mBuckets.ItemUnlock(bucketID);
                    }

#if (PROFILING)
                    has = Environment.TickCount;
#endif
                    //Ok, let's add the data from the sentinel position.
                    //Lock the start index and initialize the window.
                    vWin.SlotsSetCurrentAndLock(mSlots, sent.SlotID);

#if (PROFILING)
                    hopsData = vWin.SlotsScanAndLock(mSlots, hashID);
#else
                    vWin.SlotsScanAndLock(mSlots, hashID);
#endif
                    //Ok, we need to scan for hash collisions and multiple entries.
                    while (!vWin.Curr.IsTerminator && vWin.Next.HashID == hashID)
                    {
                        if (!vWin.Next.IsSentinel && mEqualityComparer.Equals(item, vWin.Next.Value))
                        {
                            if (mAllowMultipleEntries)
                                break;
                            else
                            {
                                vWin.SlotsUnlock(mSlots);
                                return false;
                            }
                        }

                        vWin.SlotsMoveUp(mSlots);
#if (PROFILING)
                        hopsData++;
#endif
                    }

                    //Ok, add the data in the collection.
                    vWin.SlotsInsertItem(mSlots, EmptyGet(), hashID, item);
                    vWin.SlotsUnlock(mSlots);

                    //Have we added successfully?
                    Interlocked.Increment(ref mVersion);
                    Interlocked.Increment(ref mCount);

                    if (mCount > mRecalculateThreshold)
                        BitSizeCalculate();

                    return true;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
#if (PROFILING)
            }
            finally
            {
                Profile(ProfileAction.Count_HopData, hopsData);
                Profile(ProfileAction.Count_HopBucketSkip, hopsBucketSkip);
                Profile(ProfileAction.Count_HopSentinel, hopsSentinel); 

                Profile(ProfileAction.Time_AddInternal, Environment.TickCount - start);
                if (has > 0)
                    Profile(ProfileAction.Time_AddInternalHAS, has - start);
            }
#endif
        }
        #endregion // AddInternal(T item)

        #region ContainsInternal(T item)
        /// <summary>
        /// This method checks whether the item exists in the collection.
        /// </summary>
        /// <param name="item">The item to check.</param>
        /// <returns>Returns true if the item is in the collection.</returns>
        protected virtual bool ContainsInternal(T item)
        {
#if (PROFILING)
            int hopCount = 0;
            int start = Environment.TickCount;
            int endhal = 0;
            int slotLocks1 = 0;
            int slotLocks2 = 0;
            try
            {
#endif
                //Is this a null or default value?
                if (mEqualityComparer.Equals(item, default(T)))
                    return mAllowNullValues ? mDefaultTCount > 0 : false;

                //Ok, we are going to search for the item
                Sentinel sent = new Sentinel(mEqualityComparer.GetHashCode(item), mCurrentBits, mBuckets);
                int hashID = sent.HashIDCalculate();
#if (PROFILING)
                endhal = Environment.TickCount - start;
#endif
                //Can we scan without locking?
                if (mContainScanUnlocked)
                {
                    int currVersion = mVersion;

                    //Get the initial sentinel vertex. No need to check locks as sentinels rarely change.
                    int scanPosition = sent.SlotID;
                    Vertex<T> scanVertex = mSlots[scanPosition];

                    //First we will attempt to search without locking. However, should the version ID change 
                    //during the search we will need to complete a locked search to ensure consistency.
                    while (mVersion == currVersion)
                    {
                        //Do we have a match?
                        if (!scanVertex.IsSentinel &&
                            scanVertex.HashID == hashID &&
                            mEqualityComparer.Equals(item, scanVertex.Value))
                            return true;

                        //Is this the end of the line
                        if (scanVertex.IsTerminator || scanVertex.HashID > hashID)
                            return false;
#if (PROFILING)
                        hopCount++;
#endif
                        scanPosition = scanVertex.NextSlotIDPlus1 - 1;
                        //slotLocks1 += mSlots.ItemLockWait(scanPosition);
                        scanVertex = mSlots[scanPosition];
                    }
                }

                //Ok, we have a scan miss.
                Interlocked.Increment(ref mContainScanUnlockedMiss);

                VertexWindow<T> vWin = new VertexWindow<T>();
                //Ok, let's add the data from the sentinel position.
                //Lock the start index and initialize the window.
                vWin.SlotsSetCurrentAndLock(mSlots, sent.SlotID);

                //Ok, find the first instance of the hashID.
#if (PROFILING)
                hopCount = vWin.SlotsScanAndLock(mSlots, hashID);
#else
                vWin.SlotsScanAndLock(mSlots, hashID);
#endif
                //Ok, we need to scan for hash collisions and multiple entries.
                while (!vWin.Curr.IsTerminator && vWin.Next.HashID == hashID)
                {
                    if (!vWin.Next.IsSentinel && mEqualityComparer.Equals(item, vWin.Next.Value))
                    {
                        vWin.SlotsUnlock(mSlots);
                        return true;
                    }

                    vWin.SlotsMoveUp(mSlots);
#if (PROFILING)
                    hopCount++;
#endif
                }

                vWin.SlotsUnlock(mSlots);
                return false;

#if (PROFILING)
            }
            finally
            {
                //Profile(ProfileAction.Time_FindAndLock, Environment.TickCount - start);
                Profile(ProfileAction.Count_FindAndLockHopCount, hopCount);
                Profile(ProfileAction.Count_FindAndLockSlotLocks, slotLocks1 + slotLocks2);
                Profile(ProfileAction.Time_ContainsTot, Environment.TickCount - start);
                Profile(ProfileAction.Time_ContainsHAL, endhal);
            }
#endif
        }
        #endregion // ContainsInternal(T item)

        #region RemoveInternal(T item)
        /// <summary>
        /// The method removes an item from the collection.
        /// </summary>
        /// <param name="item">The item to remove.</param>
        /// <returns>Returns true if the removal is successful.</returns>
        protected virtual bool RemoveInternal(T item)
        {
            //Is this a null or default value?
            if (mEqualityComparer.Equals(item, default(T)))
            {
                #region Null Check
                if (!mAllowNullValues)
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

                Interlocked.Decrement(ref mCount);
                Interlocked.Increment(ref mVersion);
                return true;
                #endregion
            }

            Sentinel sent = new Sentinel(mEqualityComparer.GetHashCode(item), mCurrentBits, mBuckets);
            int hashID = sent.HashIDCalculate();

            VertexWindow<T> vWin = new VertexWindow<T>();

            //Lock the start index and initialize the window.
            vWin.SlotsSetCurrentAndLock(mSlots, sent.SlotID);

            //Ok, we need to scan for hash collisions and multiple entries.
            while (!vWin.Curr.IsTerminator && vWin.Next.HashID == hashID)
            {
                if (!vWin.Next.IsSentinel && mEqualityComparer.Equals(item, vWin.Next.Value))
                {
                    //Remove the item from the linked list.
                    int emptyPos = vWin.SlotsRemoveItem(mSlots);

                    //Add the empty item for re-allocation.
                    EmptyAdd(emptyPos);

                    //Update the version and reduce the item count.
                    Interlocked.Decrement(ref mCount);
                    Interlocked.Increment(ref mVersion);

                    mSlots.ItemUnlock(vWin.CurrSlotIDPlus1 - 1);
                    return true;
                }

                vWin.SlotsMoveUp(mSlots);
            }

            vWin.SlotsUnlock(mSlots);
            //Ok, the item cannot be found.
            return false;

        }
        #endregion // RemoveInternal(T item)

        #region ClearInternal()
        /// <summary>
        /// This method clears the collection.
        /// </summary>
        /// <remarks>This method scans and locks all the items in the collection, but leaves the sentinel data intact.</remarks>
        protected virtual void ClearInternal()
        {
            if (mAllowNullValues)
            {
                int currentCount = mDefaultTCount;
                //We check whether another thread has changes the value before us.
                while (Interlocked.CompareExchange(ref mDefaultTCount, 0, currentCount) != currentCount)
                {
                    currentCount = mDefaultTCount;
                }

                if (currentCount > 0)
                {
                    Interlocked.Add(ref mCount, currentCount * -1);
                    Interlocked.Increment(ref mVersion);
                }
            }

            VertexWindow<T> vWin = new VertexWindow<T>();
            //Lock the start index and initialize the window.
            vWin.SlotsSetCurrentAndLock(mSlots, cnIndexData);

            //Ok, we need to scan for hash collisions and multiple entries.
            while (!vWin.Curr.IsTerminator)
            {
                if (vWin.Next.IsSentinel)
                    vWin.SlotsMoveUp(mSlots);
                else
                {
                    //Remove the item from the linked list and lock and move up the next item.
                    int emptyPos = vWin.SlotsSnip(mSlots);

                    //Add the empty item for re-allocation.
                    EmptyAdd(emptyPos);

                    //Update the version and reduce the item count.
                    Interlocked.Decrement(ref mCount);
                    Interlocked.Increment(ref mVersion);
                }
            }

            vWin.SlotsUnlock(mSlots);
        }
        #endregion // ClearInternal()

        #region CountInternal
        /// <summary>
        /// This is the count of the number of items currently in the collection.
        /// </summary>
        protected virtual int CountInternal
        {
            get { return mCount; }
        }
        #endregion

        #region Version
        /// <summary>
        /// This is the current collection version.
        /// </summary>
        public int Version
        {
            get
            {
                DisposedCheck();
                return mVersion;
            }
        }
        #endregion // Version
    }
}
