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


        private int mSlotsLevelCurrent;
        private int mSlotsLevelMax;
        private int mSlotsLevelOffset;

        private int mSlotInitialCapacity;

        private int mSlotsCapacity;

        private int mSlotLast;

        private int mSlotLastCapacity;

        private int mSlotsBlock31;
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
            get { return mSlotInitialCapacity; }
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
            get { return mSlotsCapacity; }
        }
        #endregion // Capacity

        #region SlotsLevelMax
        /// <summary>
        /// The maximum number of levels. You should override this value if you wish to change it.
        /// </summary>
        protected virtual int SlotsLevelMax
        {
            get { return 30; }
        }
        #endregion
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
            mSlotInitialCapacity = initialCapacity == 0 ? InitialCapacityDefault : initialCapacity;
            //Ok, find the next bit up from the value, i.e. if the initial capacity is 1000, the next whole 
            //2^n number would be 1024, so n will be 10.
            mSlotsLevelOffset = FindMaxBit(mSlotInitialCapacity, 31);
            //OK, find the last slot offset. This will be the last slot to make the capacity
            //of the collection add up to int.MaxValue.
            mSlotsBlock31 = (1 << (mSlotsLevelOffset)) - mSlotInitialCapacity - 1;
            //Ok, find the total number of slots needed for this collection after in initial capacity
            //has been taken in to account
            mSlotsLevelMax = mSlotsBlock31 == 0 ? 32 - mSlotsLevelOffset : 33 - mSlotsLevelOffset;
            mSlotsLevelCurrent = mSlotsLevelOffset;

            mSlots = new LockableWrapper<CollectionVertexStruct<T>>[mIsFixedSize?1:mSlotsLevelMax][];
            //Set the initial capacity.
            mSlots[0] = new LockableWrapper<CollectionVertexStruct<T>>[mSlotInitialCapacity];
            mSlotsCapacity = mSlotInitialCapacity;

            //int a, b;
            //SlotsCalculateLevelPosition(mSlotInitialCapacity + 536870912, out a, out b);
            //SlotsCalculateLevelPosition(4071, out a, out b);
            //SlotsCalculateLevelPosition(int.MaxValue - 10, out a, out b);
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
            if (index < mSlotInitialCapacity)
            {
                level = 0;
                levelPosition = index;
                return;
            }

            //Base line the binary progression by removing the initial capacity.
            index -= mSlotInitialCapacity;

            //Check bottom bounds, are we within the first bit block.
            if (index < (1 << mSlotsLevelOffset))
            {
                level = 1;
                levelPosition = index;
                return;
            }

            //OK, are we within the final block?
            if (index >= (1 << 30))
            {
                level = 30 - mSlotsLevelOffset;
                levelPosition = index - (1 << 30);
                return;
            }

            //Ok, find the most significant bit for the number, starting at the mSlotsLevelCurrent+1
            int mask;
            level = FindMaxBit(index, mSlotsLevelCurrent + 1, out mask);

            //Ok, calculate the level. 0 is the initial capacity, mSlotsLevelOffset is the next slot 1, 
            //so the actual slot ID is the difference plus 1.
            level = level - mSlotsLevelOffset + 1;
            //Set the position within that level.
            levelPosition = (level == 1) ? index : index & mask;


        }
        #endregion

        #region FindMaxBit(int index, int startLevel)
        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="startLevel"></param>
        /// <returns></returns>
        protected int FindMaxBit(int index, int startLevel)
        {
            int mask = ((int.MaxValue >> (31 - startLevel)) + 1) >> 1;
            while (startLevel > 0 && ((index & mask) <= 0))
            {
                mask >>= 1;
                startLevel--;
            }

            return startLevel;
        }
        #endregion // FindMaxBit(int index, int startLevel)
        #region FindMaxBit(int index, int startLevel, out int mask)
        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="startLevel"></param>
        /// <returns></returns>
        protected int FindMaxBit(int index, int startLevel, out int mask)
        {
            mask = (1 << startLevel);
            while (startLevel > 0 && ((index & mask) == 0))
            {
                mask >>= 1;
                startLevel--;
            }

            mask--;
            return startLevel;
        }
        #endregion // FindMaxBit(int index, int startLevel)

        #region SlotsCalculateLevelCapacity(int level)
        /// <summary>
        /// This method calculates the size of the bucket array.
        /// </summary>
        /// <param name="level">The level.</param>
        /// <returns>Returns 2n+1 as the size of the array where n is the level.</returns>
        protected virtual int SlotsCalculateLevelCapacity(int level)
        {
            if (level == 30)
                return (1 << 30) + mSlotsBlock31;

            return (1 << level);
        }
        #endregion // BucketLevelSize(int level)
        #region SlotsLevelExpand
        /// <summary>
        /// This method expands the data slots for the array.
        /// </summary>
        /// <param name="index">The new index.</param>
        protected virtual void SlotsLevelExpand(int index)
        {
            int level, levelPosition;
            SlotsCalculateLevelPosition(index, out level, out levelPosition);
            SlotsLevelExpand(mSlotsLevelCurrent, level);
        }
        #endregion
        #region SlotsLevelExpand(int currentLevel, int newLevel)
        /// <summary>
        /// This method expands the bucket arrays.
        /// </summary>
        /// <param name="currentLevel">The current level.</param>
        /// <param name="newLevel">The new level required.</param>
        private void SlotsLevelExpand(int currentLevel, int newLevel)
        {
            if (currentLevel != mSlotsLevelCurrent)
                return;

            for (int level = currentLevel - 1; level <= newLevel; level++)
            {
                int additionalCapacity = SlotsCalculateLevelCapacity(level);
                mSlots[level] = new LockableWrapper<CollectionVertexStruct<T>>[additionalCapacity];
                Interlocked.Add(ref mSlotsCapacity, additionalCapacity);
            }

#if (LOCKDEBUG)
                Console.WriteLine("{0} Bucket Expand: {1} -> {2} on {3}", Interlocked.Increment(ref mDebugCounter), currentLevel, newLevel, Thread.CurrentThread.ManagedThreadId);
#endif
        }
        #endregion // BucketLevelExpand(int currentLevel, int newLevel)

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

            if (nextItem >= mSlotsCapacity)
            {
                if (mIsFixedSize)
                    throw new InvalidOperationException("The array is a fixed size and the capacity has been exceeded.");

                lock (syncLockExpandSlots)
                {
                    //Have we already expanded the array?
                    if (nextItem < mSlotsCapacity)
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
            if (mIsFixedSize || index < mSlotInitialCapacity)
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
            if (mIsFixedSize || index < mSlotInitialCapacity)
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
            if (mIsFixedSize || index < mSlotInitialCapacity)
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
            if (mIsFixedSize || index < mSlotInitialCapacity)
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
            if (mIsFixedSize || index < mSlotInitialCapacity)
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
                if (mIsFixedSize || index < mSlotInitialCapacity)
                    return mSlots[0][index].Value;
                int level, levelPosition;
                SlotsCalculateLevelPosition(index, out level, out levelPosition);
                return mSlots[level][levelPosition].Value;
            }
            set
            {
                if (mIsFixedSize || index < mSlotInitialCapacity)
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
            if (mIsFixedSize || index < mSlotInitialCapacity)
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
