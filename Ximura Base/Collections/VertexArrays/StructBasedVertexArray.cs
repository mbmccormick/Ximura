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
    /// This is the base class for struct/array based vertex collection.
    /// </summary>
    /// <typeparam name="T">The colection data type.</typeparam>
    public abstract partial class StructBasedVertexArray<T> : VertexArray<T>
    {
        #region Declarations
#if (DEBUG)
        /// <summary>
        /// This is the debug counter.
        /// </summary>
        protected int mDebugCounter = 0;
#endif
        /// <summary>
        /// This lock grows the collection.
        /// </summary>
        private object syncLockExpandSlots = new object();
        
        /// <summary>
        /// This collection holds the data.
        /// </summary>
        private LockableWrapper<CollectionVertexStruct<T>>[][] mSlots;
        private int[] mSlotCapacityFastLookup;
        private volatile int mSlotsLevelCurrent;
        private volatile int mSlotsCapacityCurrent;
        private int mSlot0Capacity;

        /// <summary>
        /// This is the vertex that holds the previously used vertexes.
        /// </summary>
        private LockableNullableWrapper<CollectionVertexStruct<T>> mEmptyVertex;
        /// <summary>
        /// This is the free data queue tail position.
        /// </summary>
        private LockableNullableWrapper<int> mFreeListTail;

        /// <summary>
        /// This is the free data queue item count.
        /// </summary>
        private volatile int mFreeListCount;
        /// <summary>
        /// This is the current next free position in the data collection.
        /// </summary>
        private int mLastIndex;


        #endregion // Declarations

        #region InitializeData(int initialCapacity)
        /// <summary>
        /// This method initializes the data collection.
        /// </summary>
        protected override void InitializeData(int initialCapacity)
        {
            mFreeListTail.Value = -1;
            mFreeListCount = 0;
            mLastIndex = 0;

            SlotsInitialize(initialCapacity);

            mEmptyVertex = new LockableNullableWrapper<CollectionVertexStruct<T>>(CollectionVertexStruct<T>.Sentinel(0, 0));
        }
        #endregion

        #region InitialCapacity
        /// <summary>
        /// This is the initial capacity of the array.
        /// </summary>
        public override int InitialCapacity
        {
            get { return mSlotCapacityFastLookup[0]; }
        }
        #endregion // InitialCapacity
        #region InitialCapacityDefault
        /// <summary>
        /// This is the default initial capacity value that will be used if the value passed in the
        /// initializer is zero.
        /// </summary>
        protected virtual int InitialCapacityDefault { get { return 1024; } }
        #endregion // InitialCapacityDefault

        #region Capacity
        /// <summary>
        /// This is the current capacity of the array.
        /// </summary>
        public override int Capacity
        {
            get { return mSlotsCapacityCurrent; }
        }
        #endregion // Capacity

        #region SlotsLevelCurrent
        /// <summary>
        /// The maximum number of levels.
        /// </summary>
        protected virtual int SlotsLevelCurrent
        {
            get { return mSlotsLevelCurrent; }
        }
        #endregion

        #region SlotsInitialize()
        /// <summary>
        /// This method initializes the slot array.
        /// </summary>
        protected virtual void SlotsInitialize(int initialCapacity)
        {
            //Calculate the initial capacity of slot 0. If there is no value set, choose the default.
            mSlot0Capacity = initialCapacity == 0 ? InitialCapacityDefault : initialCapacity;

            //Ok, find the next bit up from the value, i.e. if the initial capacity is 1000, the next whole 
            //2^n number would be 1024, so n will be 10.
            int slotsLevelOffset = BitHelper.FindMostSignificantBit(mSlot0Capacity, 31) + 1;

            mSlotCapacityFastLookup = new int[32 - slotsLevelOffset];

            mSlotCapacityFastLookup[0] = mSlot0Capacity;
            mSlotCapacityFastLookup[1] = (1 << slotsLevelOffset) + mSlot0Capacity;

            int i = 2;
            for (; i < (31 - slotsLevelOffset); i++)
            {
                mSlotCapacityFastLookup[i] = mSlotCapacityFastLookup[i - 1] + (1 << (i + slotsLevelOffset - 1));
            }

            mSlotCapacityFastLookup[31 - slotsLevelOffset] = int.MaxValue;

            mSlots = new LockableWrapper<CollectionVertexStruct<T>>[32 - slotsLevelOffset][];
            mSlots[0] = new LockableWrapper<CollectionVertexStruct<T>>[mSlot0Capacity];
            mSlotsCapacityCurrent = mSlot0Capacity;
            mSlotsLevelCurrent = 0;
        }
        #endregion
        #region SlotsCalculateLevelPosition(int indexID, out int level, out int levelPosition)
        /// <summary>
        /// This method calculates the specific bucket level and the position within that bucket.
        /// </summary>
        /// <param name="index">The slot index.</param>
        /// <param name="level">The slot level.</param>
        /// <param name="levelPosition">The slot level position.</param>
        protected virtual void SlotsCalculateLevelPosition(int index, out int level, out int levelPosition)
        {
            for (level = 0; index >= mSlotCapacityFastLookup[level]; level++) { };

            levelPosition = (level == 0) ? index : index - mSlotCapacityFastLookup[level - 1];
        }
        #endregion
        #region SlotsLevelExpand
        /// <summary>
        /// This method expands the data slots for the array.
        /// </summary>
        /// <param name="index">The new index.</param>
        protected virtual void SlotsLevelExpand(int index)
        {
            int level, levelPosition;
            SlotsCalculateLevelPosition(index, out level, out levelPosition);

            if (level <= mSlotsLevelCurrent)
                return;

            int additionalCapacity = mSlotCapacityFastLookup[level] - mSlotCapacityFastLookup[level - 1];
            mSlots[level] = new LockableWrapper<CollectionVertexStruct<T>>[additionalCapacity];
            mSlotsLevelCurrent++;
            //This addition has to be atomic as other threads may bypass the lock.
            Interlocked.Add(ref mSlotsCapacityCurrent, additionalCapacity);

#if (LOCKDEBUG)
            Console.WriteLine("{0} Bucket Expand: {1} -> {2} on {3}", Interlocked.Increment(ref mDebugCounter), mSlotsCapacityCurrent - additionalCapacity, mSlotsCapacityCurrent, Thread.CurrentThread.ManagedThreadId);
#endif

        }
        #endregion

        #region EmptyGetRecycle()
        /// <summary>
        /// This method returns a recycled item from the collection.
        /// </summary>
        /// <returns>Returns the indexID of the item, or null if no items are available.</returns>
        private int? EmptyGetRecycle()
        {
            //If there are free items, try and lock the empty sentinel, 
            //but if already locked, just take a new item from the end of the collection.
            while (mFreeListCount > 0)
            {
                mEmptyVertex.Lock();
                try
                {
                    if (mEmptyVertex.Value.IsTerminator)
                        continue;

                    int pos = mEmptyVertex.Value.NextSlotIDPlus1 - 1;
                    ItemLock(pos);

                    //OK get the item.
                    CollectionVertexStruct<T> item = this[pos];

                    //OK, remove the free item from the list and set the sentinel to the next item.
                    mEmptyVertex.Value = new CollectionVertexStruct<T>(0, default(T), item.NextSlotIDPlus1);

                    if (mEmptyVertex.Value.IsTerminator)
                    {
                        mFreeListTail.Value = -1;
                        mFreeListCount = 0;
                    }
                    else
                    {
                        Interlocked.Decrement(ref mFreeListCount);
                    }

                    //Unlock the free item.
                    ItemUnlock(pos);

                    //Returns the index of the free item.
                    return pos;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    mEmptyVertex.Unlock();
                }
            }

            return null;
        }
        #endregion // EmptyGetRecycle()
        #region EmptyGet()
        /// <summary>
        /// This method returns the next free item, either from empty space, or from a free item in the collection.
        /// </summary>
        /// <returns>Returns the index for the next free item.</returns>
        public int EmptyGet()
        {
            if (mFreeListCount > 0)
            {
                int? recycleID = EmptyGetRecycle();
                if (recycleID.HasValue)
                    return recycleID.Value;
            }

            //Ok, get the next available item.
            int nextItem = Interlocked.Increment(ref mLastIndex);

            if (nextItem >= mSlotsCapacityCurrent)
            {
                if (mIsFixedSize)
                    throw new InvalidOperationException("The array is a fixed size and the capacity has been exceeded.");

                lock (syncLockExpandSlots)
                {
                    //Have we already expanded the array?
                    if (nextItem < mSlotsCapacityCurrent)
                        return nextItem - 1;

                    SlotsLevelExpand(nextItem);
                }
            }

            return nextItem - 1;
        }
        #endregion // EmptyGet()
        #region EmptyAdd(int index)
        /// <summary>
        /// This method adds an empty item to the free list.
        /// </summary>
        /// <param name="index">The index of the item to add to the sentinel.</param>
        public void EmptyAdd(int index)
        {
            mFreeListTail.Lock();
            if (mFreeListTail.Value == -1)
            {
                mEmptyVertex.Lock();

                int next = mEmptyVertex.Value.NextSlotIDPlus1;
                this[index] = new CollectionVertexStruct<T>(0, default(T), next);
                mEmptyVertex.Value = new CollectionVertexStruct<T>(0, default(T), index + 1);

                mFreeListTail.Value = index;

                mEmptyVertex.Unlock();
            }
            else
            {
                this[index] = CollectionVertexStruct<T>.Empty;
                this[mFreeListTail.Value] = new CollectionVertexStruct<T>(0, default(T), index + 1);
                mFreeListTail.Value = index;
            }
            Interlocked.Increment(ref mFreeListCount);
            mFreeListTail.Unlock();
        }
        #endregion // EmptyAdd(int index)

        #region RootIndexID
        /// <summary>
        /// This is the index ID of the the first item.
        /// </summary>
        protected virtual int RootIndexID { get { return 0; } }
        #endregion // RootIndexID

        #region ItemIsLocked(int index)
        /// <summary>
        /// This method checks whether an item in the collection is locked.
        /// </summary>
        /// <param name="index">The index of the item to check.</param>
        /// <returns>Returns true if the item is locked.</returns>
        public virtual bool ItemIsLocked(int index)
        {
            if (mIsFixedSize || index < mSlot0Capacity)
                return mSlots[0][index].IsLocked;
            int level, levelPosition;
            SlotsCalculateLevelPosition(index, out level, out levelPosition);
            return mSlots[level][levelPosition].IsLocked;
        }
        #endregion // ItemIsLocked(int index)
        #region ItemLockWait(int index)
        /// <summary>
        /// This method waits for a locked item to become available.
        /// </summary>
        /// <param name="index">The index of the item to wait for.</param>
        /// <returns>Returns the number of lock cycles during the wait.</returns>
        public virtual void ItemLockWait(int index)
        {
            if (mIsFixedSize || index < mSlot0Capacity)
                mSlots[0][index].LockWait();
            else
            {
                int level, levelPosition;
                SlotsCalculateLevelPosition(index, out level, out levelPosition);
                mSlots[level][levelPosition].LockWait();
            }
        }
        #endregion // ItemLockWait(int index)
        #region ItemLock(int index)
        /// <summary>
        /// This method locks the specific item.
        /// </summary>
        /// <param name="index">The item index.</param>
        /// <returns>Returns the number of lock cycles the thread entered.</returns>
        public virtual void ItemLock(int index)
        {
            if (mIsFixedSize || index < mSlot0Capacity)
                mSlots[0][index].Lock();
            else
            {
                int level, levelPosition;
                SlotsCalculateLevelPosition(index, out level, out levelPosition);
                mSlots[level][levelPosition].Lock();
            }
        }
        #endregion // ItemLock(int index)
        #region ItemTryLock(int index)
        /// <summary>
        /// This method attempts to lock the item specified.
        /// </summary>
        /// <param name="index">The index of the item you wish to lock..</param>
        /// <returns>Returns true if the item was successfully locked.</returns>
        public virtual bool ItemTryLock(int index)
        {
            if (mIsFixedSize || index < mSlot0Capacity)
                return mSlots[0][index].TryLock();
            int level, levelPosition;
            SlotsCalculateLevelPosition(index, out level, out levelPosition);
            return mSlots[level][levelPosition].TryLock();
        }
        #endregion // ItemTryLock(int index)
        #region ItemUnlock(int index)
        /// <summary>
        /// The method unlocks the item.
        /// </summary>
        /// <param name="index">The index of the item you wish to unlock.</param>
        public virtual void ItemUnlock(int index)
        {
            if (mIsFixedSize || index < mSlot0Capacity)
                mSlots[0][index].Unlock();
            else
            {
                int level, levelPosition;
                SlotsCalculateLevelPosition(index, out level, out levelPosition);
                mSlots[level][levelPosition].Unlock();
            }
        }
        #endregion // ItemUnlock(int index)
        #region this[int index]
        /// <summary>
        /// This is the indexer for the array.
        /// </summary>
        /// <param name="index">The index position.</param>
        /// <returns>Returns the vertex corresponding to the index position.</returns>
        public virtual CollectionVertexStruct<T> this[int index]
        {
            get
            {
                if (mIsFixedSize || index < mSlot0Capacity)
                    return mSlots[0][index].Value;
                int level, levelPosition;
                SlotsCalculateLevelPosition(index, out level, out levelPosition);
                return mSlots[level][levelPosition].Value;
            }
            set
            {
                if (mIsFixedSize || index < mSlot0Capacity)
                    mSlots[0][index].Value = value;
                else
                {
                    int level, levelPosition;
                    SlotsCalculateLevelPosition(index, out level, out levelPosition);
                    mSlots[level][levelPosition].Value = value;
                }
            }
        }
        #endregion // this[int index]
        #region LockableData(int index)
        /// <summary>
        /// This method returns the full lockable data from the slot array.
        /// </summary>
        /// <param name="index">The slot index.</param>
        /// <param name="isLocked">Returns a boolean value indicating that the item is locked.</param>
        /// <returns>Returns a lockable wrapper containing the vertex data.</returns>
        protected virtual CollectionVertexStruct<T> LockableData(int index, out bool isLocked)
        {
            LockableWrapper<CollectionVertexStruct<T>> item;
            if (mIsFixedSize || index < mSlot0Capacity)
                item = mSlots[0][index];
            else
            {
                int level, levelPosition;
                SlotsCalculateLevelPosition(index, out level, out levelPosition);
                item = mSlots[level][levelPosition];
            }
            isLocked = item.IsLocked;
            return item.Value;
        }
        #endregion // LockableData(int index)

        #region GetSentinelID(int hashCode, bool createSentinel, out int sentIndexID, out int hashID)
        /// <summary>
        /// This method returns the sentinel ID and the hashID for the hashcode passed.
        /// </summary>
        /// <param name="hashCode">The hashcode to search for the sentinel position.</param>
        /// <param name="createSentinel">This property determine whether any missing sentinels will be created.</param>
        /// <param name="hashID">The hashID for the hashCode that passed.</param>
        /// <returns>The largest sentinel index ID.</returns>
        protected abstract int GetSentinelID(int hashCode, bool createSentinel, out int hashID);
        #endregion

        #region GetEnumerator()
        /// <summary>
        /// This method returns an enumeration through the sentinels and data in the collection.
        /// </summary>
        /// <returns>Returns an enumeration containing the collection data.</returns>
        public override IEnumerator<KeyValuePair<int, ICollectionVertex<T>>> GetEnumerator()
        {
            CollectionVertexStruct<T> item = this[RootIndexID];
            yield return new KeyValuePair<int, ICollectionVertex<T>>(RootIndexID, item); ;

            while (!item.IsTerminator)
            {
                int id = item.NextSlotIDPlus1 - 1;
                ItemLockWait(id);
                item = this[id];

                yield return new KeyValuePair<int, ICollectionVertex<T>>(id, item); ;
            }
        }
        #endregion
    }
}
