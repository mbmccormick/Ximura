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
    public abstract class StructBasedVertexArray<T> : VertexArray<T>
    {
        #region Declarations
        /// <summary>
        /// This collection holds the data.
        /// </summary>
        protected IFineGrainedLockArray<CollectionVertexStruct<T>> mSlots;
        /// <summary>
        /// This is the vertex that holds the previously used vertexes.
        /// </summary>
        protected LockableWrapper<CollectionVertexStruct<T>> mEmptyVertex;
        /// <summary>
        /// This is the free data queue item count.
        /// </summary>
        private volatile int mFreeListCount;
        /// <summary>
        /// This is the free data queue tail position.
        /// </summary>
        private LockableWrapper<int> mFreeListTail;
        /// <summary>
        /// This is the current next free position in the data collection.
        /// </summary>
        private volatile int mLastIndex;
        /// <summary>
        /// This is the initial data capacity of the collection.
        /// </summary>
        private volatile int mCapacity;
        #endregion // Declarations

        #region InitializeData()
        /// <summary>
        /// This method initializes the data collection.
        /// </summary>
        protected override void InitializeData()
        {
            mFreeListTail.Value = -1;
            mFreeListCount = 0;
            mLastIndex = 0;

            if (IsFixedSize)
                mSlots = new FineGrainedLockArray<CollectionVertexStruct<T>>(InitialCapacity, 0);
            else
                mSlots = new ExpandableFineGrainedLockArray<CollectionVertexStruct<T>>(InitialCapacity, SlotExpander);

            mEmptyVertex = new LockableWrapper<CollectionVertexStruct<T>>(CollectionVertexStruct<T>.Sentinel(0, 0));
        }
        #endregion

        #region EmptyGet()
        /// <summary>
        /// This method returns the next free item, either from empty space, or from a free item in the collection.
        /// </summary>
        /// <returns>Returns the index for the next free item.</returns>
        public virtual int EmptyGet()
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
                    mSlots.ItemLock(pos);

                    //OK get the item.
                    CollectionVertexStruct<T> item = mSlots[pos];

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
                    mSlots.ItemUnlock(pos);

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

            int nextItem = Interlocked.Increment(ref mLastIndex);

            if (nextItem == 0 || (IsFixedSize && nextItem >= mCapacity))
                throw new ArgumentOutOfRangeException("The list has exceeeded the capacity of the maximum integer value.");

            return nextItem - 1;
        }
        #endregion // EmptyGet()
        #region EmptyAdd(int index)
        /// <summary>
        /// This method adds an empty item to the free list.
        /// </summary>
        /// <param name="index">The index of the item to add to the sentinel.</param>
        public virtual void EmptyAdd(int index)
        {
            mFreeListTail.Lock();
            if (mFreeListTail.Value == -1)
            {
                mEmptyVertex.Lock();

                int next = mEmptyVertex.Value.NextSlotIDPlus1;
                mSlots[index] = new CollectionVertexStruct<T>(0, default(T), next);
                mEmptyVertex.Value = new CollectionVertexStruct<T>(0, default(T), index + 1);

                mFreeListTail.Value = index;

                mEmptyVertex.Unlock();
            }
            else
            {
                mSlots[index] = CollectionVertexStruct<T>.Empty;
                mSlots[mFreeListTail.Value] = new CollectionVertexStruct<T>(0, default(T), index + 1);
                mFreeListTail.Value = index;
            }
            Interlocked.Increment(ref mFreeListCount);
            mFreeListTail.Unlock();
        }
        #endregion // EmptyAdd(int index)

        #region ItemIsLocked(int index)
        /// <summary>
        /// This method checks whether an item in the collection is locked.
        /// </summary>
        /// <param name="index">The index of the item to check.</param>
        /// <returns>Returns true if the item is locked.</returns>
        public virtual bool ItemIsLocked(int index)
        {
            return mSlots.ItemIsLocked(index);
        }
        #endregion // ItemIsLocked(int index)
        #region ItemLock(int index)
        /// <summary>
        /// This method locks the specific item.
        /// </summary>
        /// <param name="index">The item index.</param>
        /// <returns>Returns the number of lock cycles the thread entered.</returns>
        public virtual int ItemLock(int index)
        {
            return mSlots.ItemLock(index);
        }
        #endregion // ItemLock(int index)
        #region ItemLockWait(int index)
        /// <summary>
        /// This method waits for a locked item to become available.
        /// </summary>
        /// <param name="index">The index of the item to wait for.</param>
        /// <returns>Returns the number of lock cycles during the wait.</returns>
        public virtual int ItemLockWait(int index)
        {
            return mSlots.ItemLockWait(index);
        }
        #endregion // ItemLockWait(int index)
        #region ItemTryLock(int index)
        /// <summary>
        /// This method attempts to lock the item specified.
        /// </summary>
        /// <param name="index">The index of the item you wish to lock..</param>
        /// <returns>Returns true if the item was successfully locked.</returns>
        public virtual bool ItemTryLock(int index)
        {
            return mSlots.ItemTryLock(index);
        }
        #endregion // ItemTryLock(int index)
        #region ItemUnlock(int index)
        /// <summary>
        /// The method unlocks the item.
        /// </summary>
        /// <param name="index">The index of the item you wish to unlock.</param>
        public virtual void ItemUnlock(int index)
        {
            mSlots.ItemUnlock(index);
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
            get { return mSlots[index]; }
            set { mSlots[index] = value; }
        }
        #endregion // this[int index]

        #region GetEnumerator()
        /// <summary>
        /// This method returns an enumeration through the sentinels and data in the collection.
        /// </summary>
        /// <returns>Returns an enumeration containing the collection data.</returns>
        public override IEnumerator<KeyValuePair<int, ICollectionVertex<T>>> GetEnumerator()
        {
            CollectionVertexStruct<T> item = mSlots[0];

            yield return new KeyValuePair<int, ICollectionVertex<T>>(0, item);

            while (!item.IsTerminator)
            {
                int id = item.NextSlotIDPlus1 - 1;
                ItemLockWait(id);
                item = this[id];

                yield return new KeyValuePair<int, ICollectionVertex<T>>(id, item);
            }
        }
        #endregion

        #region SlotExpander(int requiredSize, int currentSize)
        /// <summary>
        /// This expander grows the buckets by the specified amount.
        /// </summary>
        /// <param name="requiredSize">The index specifying the new capacity.</param>
        /// <param name="currentSize">The current capacity.</param>
        /// <returns>Returns the new capacity.</returns>
        protected virtual int SlotExpander(int requiredSize, int currentSize)
        {
            return Prime.Get(requiredSize * 2);
        }
        #endregion // SlotExpander(int requiredSize, int currentSize)

        #region RootIndexID
        /// <summary>
        /// This is the index ID of the the first item.
        /// </summary>
        protected virtual int RootIndexID { get { return 0; } }
        #endregion // RootIndexID

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

        #region VertexWindowGet(int index)
        /// <summary>
        /// This method returns a vertex window for the index specified.
        /// </summary>
        /// <returns>Returns a vertex window.</returns>
        public override IVertexWindow<T> VertexWindowGet()
        {
            return new StructBasedVertexWindow<T>(this, mEqComparer, RootIndexID, 0, default(T));
        }
        #endregion // VertexWindowGet(int index)
        #region VertexWindowGet(int hashCode, bool createSentinel)
        /// <summary>
        /// This method returns a vertex window for the hashCode specified.
        /// </summary>
        /// <param name="hashCode">The hashcode.</param>
        /// <param name="createSentinel">A boolean value that specifies whether the sentinels should be created.</param>
        /// <returns>Returns a vertex window.</returns>
        public override IVertexWindow<T> VertexWindowGet(T item, bool createSentinel)
        {
            int hashCode = mEqComparer.GetHashCode(item);
            int hashID;
            int sentIndexID = GetSentinelID(mEqComparer.GetHashCode(item), createSentinel, out hashID);

            //Ok, set the MSB to indicate the value is a sentinel.
            return new StructBasedVertexWindow<T>(this, mEqComparer, sentIndexID, hashID, item);
        }
        #endregion // VertexWindowGet(int hashCode, bool createSentinel)
    }
}
