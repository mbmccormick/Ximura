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
    public abstract partial class CollectionHelperStructBase<T> : CollectionHelperBase<T>
    {
        #region Declarations
        /// <summary>
        /// This array holds both the slot and vertex data.
        /// </summary>
        protected VertexArray<T> mData;
        #endregion
        #region Dispose(bool disposing)
        /// <summary>
        /// This method disposes of the data in the collection. You should override this method if you need to add
        /// custom dispose logic to your collection.
        /// </summary>
        /// <param name="disposing">The class is disposing, i.e. this is called by Dispose and not the finalizer.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                //Clear the collection. This removes all references to any contained objects.
                ClearInternal();
                mData = null;
            }
        }
        #endregion // Dispose(bool disposing)

        #region Initialize(IEqualityComparer<T> comparer, int capacity, IEnumerable<T> collection, bool isFixedSize)
        /// <summary>
        /// 
        /// </summary>
        /// <param name="comparer"></param>
        /// <param name="capacity"></param>
        /// <param name="collection"></param>
        /// <param name="isFixedSize"></param>
        protected override void Initialize(IEqualityComparer<T> comparer, int capacity, IEnumerable<T> collection, bool isFixedSize)
        {
            base.Initialize(comparer, capacity, collection, isFixedSize);

#if (DEBUG)
            DebugReset();
#endif
        }
        #endregion // Initialize(IEqualityComparer<T> comparer, int capacity, IEnumerable<T> collection, bool isFixedSize)

        #region Insert(T item)
        /// <summary>
        /// This method adds an item to the collection.
        /// </summary>
        /// <param name="item">The item to add.</param>
        /// <param name="add">The property specifies whether the item is overwritten or a new item is added. 
        /// If multiple entries are not allowed an exception is thrown.</param>
        /// <returns>Returns true if the addition is successful.</returns>
        protected override bool Insert(T item, bool add)
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
                        if (!mState.AllowNullValues)
                            throw new ArgumentNullException("Null values are not accepted in this collection.");

                        if (!mState.AllowMultipleEntries)
                        {
                            if (mState.DefaultTCount > 0)
                                return false;

                            if (Interlocked.CompareExchange(ref mState.DefaultTCount, 1, 0) != 0)
                                return false;
                        }
                        else
                            Interlocked.Increment(ref mState.DefaultTCount);

                        Interlocked.Increment(ref mState.Version);
                        Interlocked.Increment(ref mState.Count);
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

                            if (mState.AllowMultipleEntries)
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
                    Interlocked.Increment(ref mState.Version);
                    Interlocked.Increment(ref mState.Count);

                    //Check whether we need to recalculate the bit size.
                    mData.SizeRecalculate(mState.Count);

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
        protected override bool ContainsInternal(T item)
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
                    return mState.AllowNullValues ? mState.DefaultTCount > 0 : false;

                //Ok, we are going to search for the item
                int hashCode = mEqualityComparer.GetHashCode(item);

                int sentIndexID, hashID;
                mData.GetSentinelID(hashCode, false, out sentIndexID, out hashID);

#if (PROFILING)
                endhal = Environment.TickCount - start;
#endif
                //Can we scan without locking?
                if (mState.ContainScanUnlocked)
                {
                    int currVersion = mState.Version;

                    //Get the initial sentinel vertex. No need to check locks as sentinels rarely change.
                    int scanPosition = sentIndexID;
                    CollectionVertex<T> scanVertex = mData[scanPosition];

                    //First we will attempt to search without locking. However, should the version ID change 
                    //during the search we will need to complete a locked search to ensure consistency.
                    while (mState.Version == currVersion)
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
                Interlocked.Increment(ref mState.ContainScanUnlockedMiss);

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
        #region TryGetValueInternal(IEqualityComparer<T> comparer, T key, out T value)
        /// <summary>
        /// This method attempts to retrieve an item from the collection.
        /// </summary>
        /// <param name="comparer">The specific comparer.</param>
        /// <param name="key">The key.</param>
        /// <param name="value">The value parameter.</param>
        /// <returns>Returns true if the item can be found in the collection.</returns>
        protected override bool TryGetValueInternal(IEqualityComparer<T> comparer, T key, out T value)
        {
            DisposedCheck();
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
                //Get the hash code for the item.
                int hashCode = comparer.GetHashCode(key);

                int sentIndexID, hashID;
                mData.GetSentinelID(hashCode, true, out sentIndexID, out hashID);
#if (PROFILING)
                endhal = Environment.TickCount - start;
#endif
                //Can we scan without locking?
                if (mState.ContainScanUnlocked)
                {
                    int currVersion = mState.Version;

                    //Get the initial sentinel vertex. No need to check locks as sentinels rarely change.
                    int scanPosition = sentIndexID;
                    CollectionVertex<T> scanVertex = mData[scanPosition];

                    //First we will attempt to search without locking. However, should the version ID change 
                    //during the search we will need to complete a locked search to ensure consistency.
                    while (mState.Version == currVersion)
                    {
                        //Do we have a match?
                        if (!scanVertex.IsSentinel &&
                            scanVertex.HashID == hashID &&
                            comparer.Equals(key, scanVertex.Value))
                        {
                            value = scanVertex.Value;
                            return true;
                        }

                        //Is this the end of the line
                        if (scanVertex.IsTerminator || scanVertex.HashID > hashID)
                        {
                            value = default(T);
                            return false;
                        }
#if (PROFILING)
                        hopCount++;
#endif
                        scanPosition = scanVertex.NextSlotIDPlus1 - 1;
                        //slotLocks1 += mSlots.ItemLockWait(scanPosition);
                        scanVertex = mData[scanPosition];
                    }
                }

                //Ok, we have a scan miss.
                Interlocked.Increment(ref mState.ContainScanUnlockedMiss);

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
                    if (!vWin.Next.IsSentinel && comparer.Equals(key, vWin.Next.Value))
                    {
                        vWin.Unlock();
                        value = vWin.Next.Value;
                        return true;
                    }

                    vWin.MoveUp();
#if (PROFILING)
                    hopCount++;
#endif
                }

                vWin.Unlock();
                value = default(T);
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
        #endregion // TryGetValue(TKey key, out TValue value)
        #region RemoveInternal(T item)
        /// <summary>
        /// The method removes an item from the collection.
        /// </summary>
        /// <param name="item">The item to remove.</param>
        /// <returns>Returns true if the removal is successful.</returns>
        protected override bool RemoveInternal(T item)
        {
            //Is this a null or default value?
            if (mEqualityComparer.Equals(item, default(T)))
            {
                #region Null Check
                if (!mState.AllowNullValues)
                    return false;

                int currentCount = mState.DefaultTCount;
                if (currentCount == 0)
                    return false;

                //We check whether another thread has changes the value before us.
                while (Interlocked.CompareExchange(ref mState.DefaultTCount, currentCount - 1, currentCount) != currentCount)
                {
                    currentCount = mState.DefaultTCount;

                    if (currentCount == 0)
                        return false;
                }

                Interlocked.Decrement(ref mState.Count);
                Interlocked.Increment(ref mState.Version);
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
                    Interlocked.Increment(ref mState.Version);
                    Interlocked.Decrement(ref mState.Count);
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
        protected override void ClearInternal()
        {
            if (mState.AllowNullValues)
            {
                int currentCount = mState.DefaultTCount;
                //We check whether another thread has changes the value before us.
                while (Interlocked.CompareExchange(ref mState.DefaultTCount, 0, currentCount) != currentCount)
                {
                    currentCount = mState.DefaultTCount;
                }

                if (currentCount > 0)
                {
                    Interlocked.Add(ref mState.Count, currentCount * -1);
                    Interlocked.Increment(ref mState.Version);
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
                    Interlocked.Decrement(ref mState.Count);
                    Interlocked.Increment(ref mState.Version);
                }
            }

            vWin.Unlock();
        }
        #endregion // ClearInternal()

        #region InternalScan(bool changeException)
        /// <summary>
        /// This method enumerates through the collection.
        /// </summary>
        /// <param name="changeException">Set this to true if you want the method to throw an exception if the collection changes.</param>
        /// <returns>Returns a enumeration of the collection.</returns>
        /// <exception cref="System.InvalidOperationException">This exception will be thrown when the collection 
        /// changes during the scan and the changeException parameter is set to true.</exception>
        protected internal virtual IEnumerable<KeyValuePair<int, ICollectionVertex<T>>> InternalScan(bool changeException)
        {
            if (mState.Count == 0)
                yield break;

            int currentVersion = mState.Version;

            foreach (var item in mData)
            {
                if (changeException && currentVersion != mState.Version)
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
        public override IEnumerator<T> GetEnumerator()
        {
            //Enumerate the default(T) values.
            for (int i = mState.DefaultTCount; i > 0; i--)
                yield return default(T);
            //Enumerate the data.
            foreach (var item in InternalScan(true))
                if (!item.Value.IsSentinel)
                    yield return item.Value.DataValue;
        }
        #endregion // GetEnumerator()
    }
}