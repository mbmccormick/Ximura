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
        protected bool mContainScanUnlocked;

        /// <summary>
        /// This variable contains the number of scan misses.
        /// </summary>
        protected int mContainScanUnlockedMiss = 0;
        /// <summary>
        /// This variables determines whether the collection has been disposed.
        /// </summary>
        private bool mDisposed;
        /// <summary>
        /// This is the equality comparer for the collection.
        /// </summary>
        protected IEqualityComparer<T> mEqualityComparer;

        /// <summary>
        /// The version value.
        /// </summary>
        protected int mVersion;
        /// <summary>
        /// This is the current item count.
        /// </summary>
        protected int mCount;
        /// <summary>
        /// This is the current default(T) item capacity. 
        /// </summary>
        protected int mDefaultTCount;

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
        /// This array holds both the slot and vertex data.
        /// </summary>
        protected CombinedVertexArray<T> mData;
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
        protected LockFreeCollectionBase(IEqualityComparer<T> comparer, int capacity, IEnumerable<T> collection, bool isFixedSize)
        {
#if (PROFILING)
            ProfilingSetup();
#endif
            mEqualityComparer = (comparer == null) ? EqualityComparer<T>.Default : comparer;

            Initialize(capacity, collection, isFixedSize);
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
        protected internal void DisposedCheck()
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
                //Clear the collection. This removes all references to any contained objects.
                ClearInternal();
                mData=null;
            }
        }
        #endregion // Dispose(bool disposing)
        #endregion // IDisposable

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
        #region IsFixedSize
        /// <summary>
        /// This property determines whether the collection will dynamically expand when new values are added. 
        /// This property can only be set from the constructor, although this property can be overriden in derived classes to ensure
        /// a particular value.
        /// </summary>
        public virtual bool IsFixedSize { get { return mIsFixedSize; } }
        #endregion

        #region Initialize(int capacity, IEnumerable<T> collection)
        /// <summary>
        /// This method initializes the collection.
        /// </summary>
        /// <param name="capacity">The initial capacity.</param>
        /// <param name="collection">The initial data to load in to the array.</param>
        /// <param name="isFixedSize">This property determines whether the collection is a fixed size.</param>
        protected virtual void Initialize(int capacity, IEnumerable<T> collection, bool isFixedSize)
        {
            if (capacity < 0)
                throw new ArgumentOutOfRangeException("The capacity cannot be less than 0.");

            mDisposed = false;
            mCount = 0;
            mVersion = int.MinValue;

            mIsFixedSize = isFixedSize;

            mAllowNullValues = typeof(T).IsValueType || CollectionAllowNullValues;
            mAllowMultipleEntries = CollectionAllowMultipleEntries;

            mDefaultTCount = 0;
            mContainScanUnlocked = true;

            mData = new CombinedVertexArray<T>(isFixedSize, capacity);

            if (collection != null)
                AddIncomingData(collection);
        }
        #endregion // Initialize(int capacity)
        #region AddIncomingData(IEnumerable<T> collection)
        /// <summary>
        /// This method adds items to the collection that were passed in the constructor.
        /// You should override this method to implement any specific logic for your collection.
        /// </summary>
        /// <param name="collection">The data to add to the collection.</param>
        protected virtual void AddIncomingData(IEnumerable<T> collection)
        {
            collection.ForEach(i => Insert(i, true));
        }
        #endregion // InitializeCollection(IEnumerable<T> collection)

        #region Insert(T item)
        /// <summary>
        /// This method adds an item to the collection.
        /// </summary>
        /// <param name="item">The item to add.</param>
        /// <param name="add">The property specifies whether the item is overwritten or a new item is added. 
        /// If multiple entries are not allowed an exception is thrown.</param>
        /// <returns>Returns true if the addition is successful.</returns>
        protected virtual bool Insert(T item, bool add)
        {
#region Profiling
#if (PROFILING)
            int hopsData = 0;
            int start = Environment.TickCount;
            int has = 0;
            int hopsSentinel = 0;
            int hopsBucketSkip = 0;
            try
            {
#endif 
#endregion
                try
                {
                    #region Null/default(T) check
                    //Is this a null or default value?
                    if (mEqualityComparer.Equals(item, default(T)))
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

                        Interlocked.Increment(ref mVersion);
                        Interlocked.Increment(ref mCount);
                        return true;
                    } 
                    #endregion

                    //Get the hash code for the item.
                    int hashCode = mEqualityComparer.GetHashCode(item);

                    //Add any required sentinels and retrieve the nearest sentinel slot ID.
                    //CombinedVertexArray<T>.Sentinel sent = new Sentinel(hashCode, mCurrentBits, mBuckets);
                    int sentIndexID, hashID;
                    mData.GetSentinelID(hashCode, true, out sentIndexID, out hashID);
                    CombinedVertexArray<T>.VertexWindow<T> vWin = mData.VertexWindowGet(sentIndexID);
#region Profiling
	#if (PROFILING)
                    has = Environment.TickCount;
    #endif 
#endregion
                    //Ok, let's add the data from the sentinel position.
                    //Lock the start index and initialize the window.
#if (PROFILING)
                    hopsData = vWin.ScanAndLock(hashID);
#else 
                    vWin.ScanAndLock(hashID);
#endif
                    //Ok, we need to scan for hash collisions and multiple entries.
                    while (!vWin.Curr.IsTerminator && vWin.Next.HashID == hashID)
                    {
                        if (!vWin.Next.IsSentinel && mEqualityComparer.Equals(item, vWin.Next.Value))
                        {
                            //Ok, we have a match.
                            if (!add)
                            {
                                //This code is to accomodate dictionary type collections where the item is a keyvalue pair.
                                vWin.Next.Value = item;
                                mData[vWin.Curr.NextSlotIDPlus1 - 1] = vWin.Next;
                                vWin.Unlock();
                                return true;
                            }

                            if (mAllowMultipleEntries)
                                break;
                            else
                            {
                                vWin.Unlock();
                                return false;
                            }
                        }
                        vWin.MoveUp();
#if (PROFILING)
                        hopsData++;
#endif
                    }

                    //Ok, add the data in the collection.
                    try
                    {
                        vWin.InsertItem(mData.EmptyGet(), hashID, item);
                    }
                    catch (Exception exx)
                    {
                        throw exx;
                    }
                    vWin.Unlock();

                    //Increment the necessary counters.
                    Interlocked.Increment(ref mVersion);
                    Interlocked.Increment(ref mCount);

                    //Check whether we need to recalculate the bit size.
                    mData.BitSizeCalculate(mCount);

                    return true;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
#region Profiling
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
#endregion
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
#region Profiling
#if (PROFILING)
            int hopCount = 0;
            int start = Environment.TickCount;
            int endhal = 0;
            int slotLocks1 = 0;
            int slotLocks2 = 0;
            try
            {
#endif 
#endregion
                //Is this a null or default value?
                if (mEqualityComparer.Equals(item, default(T)))
                    return mAllowNullValues ? mDefaultTCount > 0 : false;

                //Ok, we are going to search for the item
                int hashCode = mEqualityComparer.GetHashCode(item);

                int sentIndexID, hashID;
                mData.GetSentinelID(hashCode, false, out sentIndexID, out hashID);

#if (PROFILING)
                endhal = Environment.TickCount - start;
#endif
                //Can we scan without locking?
                if (mContainScanUnlocked)
                {
                    int currVersion = mVersion;

                    //Get the initial sentinel vertex. No need to check locks as sentinels rarely change.
                    int scanPosition = sentIndexID;
                    Vertex<T> scanVertex = mData[scanPosition];

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
                        scanVertex = mData[scanPosition];
                    }
                }

                //Ok, we have a scan miss due to an addition or deletion during the unlocked scan.
                Interlocked.Increment(ref mContainScanUnlockedMiss);

                //Ok, let's add the data from the sentinel position.
                //Lock the start index and initialize the window.
                CombinedVertexArray<T>.VertexWindow<T> vWin = mData.VertexWindowGet(sentIndexID);

                //Ok, find the first instance of the hashID.
#if (PROFILING)
                hopCount = vWin.ScanAndLock(hashID);
#else
                vWin.ScanAndLock(hashID);
#endif
                //Ok, we need to scan for hash collisions and multiple entries.
                while (!vWin.Curr.IsTerminator && vWin.Next.HashID == hashID)
                {
                    if (!vWin.Next.IsSentinel && mEqualityComparer.Equals(item, vWin.Next.Value))
                    {
                        vWin.Unlock();
                        return true;
                    }

                    vWin.MoveUp();
#if (PROFILING)
                    hopCount++;
#endif
                }

                vWin.Unlock();
                return false;
#region Profiling
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
#endregion
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

            int hashCode = mEqualityComparer.GetHashCode(item);

            int sentIndexID, hashID;
            mData.GetSentinelID(hashCode, false, out sentIndexID, out hashID);

            //Lock the start index and initialize the window.
            CombinedVertexArray<T>.VertexWindow<T> vWin = mData.VertexWindowGet(sentIndexID);

            vWin.ScanAndLock(hashID);

            //Ok, we need to scan for hash collisions and multiple entries.
            while (!vWin.Curr.IsTerminator && vWin.Next.HashID == hashID)
            {
                if (!vWin.Next.IsSentinel && mEqualityComparer.Equals(item, vWin.Next.Value))
                {
                    //Remove the item from the linked list.
                    vWin.RemoveItemAndUnlock();
                    
                    //Update the version and reduce the item count.
                    Interlocked.Increment(ref mVersion);
                    Interlocked.Decrement(ref mCount);
                    return true;
                }

                vWin.MoveUp();
            }

            vWin.Unlock();
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

            //Get the window from the root sentinel.
            CombinedVertexArray<T>.VertexWindow<T> vWin = mData.VertexWindowGet();

            //Ok, we need to scan for hash collisions and multiple entries.
            while (!vWin.Curr.IsTerminator)
            {
                if (vWin.Next.IsSentinel)
                    vWin.MoveUp();
                else
                {
                    //Remove the item from the linked list and lock and move up the next item.
                    int emptyPos = vWin.Snip();

                    //Add the empty item for re-allocation.
                    mData.EmptyAdd(emptyPos);

                    //Update the version and reduce the item count.
                    Interlocked.Decrement(ref mCount);
                    Interlocked.Increment(ref mVersion);
                }
            }

            vWin.Unlock();
        }
        #endregion // ClearInternal()

        #region CountInternal
        /// <summary>
        /// This is the count of the number of items currently in the collection.
        /// </summary>
        protected internal virtual int CountInternal
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

        #region InternalScan(bool changeException)
        /// <summary>
        /// This method enumerates through the collection.
        /// </summary>
        /// <param name="changeException">Set this to true if you want the method to throw an exception if the collection changes.</param>
        /// <returns>Returns a enumeration of the collection.</returns>
        /// <exception cref="System.InvalidOperationException">This exception will be thrown when the collection 
        /// changes during the scan and the changeException parameter is set to true.</exception>
        protected internal virtual IEnumerable<KeyValuePair<int, Vertex<T>>> InternalScan(bool changeException)
        {
            if (mCount == 0)
                yield break;

            int currentVersion = mVersion;

            foreach (var item in mData)
            {
                if (changeException && currentVersion != mVersion)
                    throw new InvalidOperationException("The version has changed");

                yield return item;
            }
        }
        #endregion // InternalScan(bool changeException)
        #region GetEnumerator()
        /// <summary>
        /// This method returns an enumeration of the collection values.
        /// </summary>
        /// <returns>Returns a enumeration of the collection.</returns>
        /// <exception cref="System.InvalidOperationException">This exception will be thrown when if collection 
        /// changes during the enumeration.</exception>
        public IEnumerator<T> GetEnumerator()
        {
            //Enumerate the default(T) values.
            for (int i = mDefaultTCount; i > 0; i--)
                yield return default(T);
            //Enumerate the data.
            foreach (var item in InternalScan(true))
                if (!item.Value.IsSentinel)
                    yield return item.Value.Value;
        }
        #endregion // GetEnumerator()
        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator)GetEnumerator();
        }

        #endregion

        #region CopyToInternal(T[] array, int arrayIndex)
        /// <summary>
        /// This method copies the collection to the array specified.
        /// </summary>
        /// <param name="array">The array to copy to.</param>
        /// <param name="arrayIndex">The array index where the class should start copying to.</param>
        protected virtual void CopyToInternal(T[] array, int arrayIndex)
        {
            this.ForIndex((i, d) => array[i + arrayIndex] = d);
        }
        /// <summary>
        /// This method copies the collection to the array specified.
        /// </summary>
        /// <param name="array">The array to copy to.</param>
        /// <param name="arrayIndex">The array index where the class should start copying to.</param>
        protected virtual void CopyToInternal(Array array, int arrayIndex)
        {
            this.ForIndex((i, d) => array.SetValue(d, i));
        }

        #endregion // CopyTo(T[] array, int arrayIndex)
        #region ToArrayInternal()
        /// <summary>
        /// This method copies the internal data to an array.
        /// </summary>
        /// <returns>Returns an array containing the internal data.</returns>
        protected virtual T[] ToArrayInternal()
        {
            T[] array = new T[CountInternal];
            CopyToInternal(array, 0);
            return array;
        }
        #endregion // ToArrayInternal()
    }
}
