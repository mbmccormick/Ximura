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
#endregion // using
namespace Ximura.Collections
{
    /// <summary>
    /// The vertex array class provides basic linked list expandable array support.
    /// </summary>
    /// <typeparam name="T">The collection type.</typeparam>
    public abstract class VertexArray<T> 
        : IFineGrainedLockArray<CollectionVertex<T>>, IEnumerable<KeyValuePair<int, ICollectionVertex<T>>>
    {
        #region Struct -> VertexWindow<T>
        /// <summary>
        /// The vertex window structure holds the search results from a scan.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        [StructLayout(LayoutKind.Sequential)]
        public struct VertexWindow<T>
        {
            #region Declarations
            VertexArray<T> mData;
            #endregion // Declarations
            #region Constructor
            /// <summary>
            /// This is the default constructor for the window.
            /// </summary>
            /// <param name="data">The data collection.</param>
            /// <param name="indexID">The index of the position to set the window.</param>
            public VertexWindow(VertexArray<T> data, int indexID)
            {
                mData = data;

                mData.ItemLock(indexID);
                CurrSlotIDPlus1 = indexID + 1;
                Curr = mData[indexID];

                if (!Curr.IsTerminator)
                {
                    mData.ItemLock(Curr.NextSlotIDPlus1 - 1);
                    Next = mData[Curr.NextSlotIDPlus1 - 1];
                }
                else
                    Next = new CollectionVertex<T>();
            }
            #endregion // Constructor

            #region Public properties
            /// <summary>
            /// The current slot ID plus 1.
            /// </summary>
            public int CurrSlotIDPlus1;
            /// <summary>
            /// THe current vertex.
            /// </summary>
            public CollectionVertex<T> Curr;
            /// <summary>
            /// The next vertex.
            /// </summary>
            public CollectionVertex<T> Next;
            #endregion // Public properties

            #region ToString()
            /// <summary>
            /// This override provides a debug friendly representation of the structure.
            /// </summary>
            /// <returns>Returns the structure value.</returns>
            public override string ToString()
            {
                return string.Format("{0}[{1}] -> {2}[{3}]", CurrSlotIDPlus1 - 1, Curr, Curr.NextSlotIDPlus1 - 1, Next);
            }
            #endregion // ToString()

            #region SetCurrentAndLock
            /// <summary>
            /// This method sets the current slot and locks the position.
            /// </summary>
            /// <param name="slotID">The slot ID.</param>
            public void SetCurrentAndLock(int slotID)
            {
                mData.ItemLock(slotID);
                CurrSlotIDPlus1 = slotID + 1;
                Curr = mData[slotID];

                if (!Curr.IsTerminator)
                {
                    mData.ItemLock(Curr.NextSlotIDPlus1 - 1);
                    Next = mData[Curr.NextSlotIDPlus1 - 1];
                }
                else
                    Next = new CollectionVertex<T>();
            }
            #endregion

            #region InsertSentinel(int bucketID, int hashID)
            /// <summary>
            /// This method inserts a sentinel in to the data collection.
            /// </summary>
            /// <param name="indexID">The new sentinel index id.</param>
            /// <param name="hashID">The sentinel hash id.</param>
            public void InsertSentinel(int indexID, int hashID)
            {
                if (!Curr.IsTerminator)
                    mData.ItemUnlock(Curr.NextSlotIDPlus1 - 1);

                Next = CollectionVertex<T>.Sentinel(hashID, Curr.NextSlotIDPlus1);

                Curr.NextSlotIDPlus1 = indexID + 1;

                mData[indexID] = Next;
                mData[CurrSlotIDPlus1 - 1] = Curr;
            }
            #endregion // InsertSentinel(ExpandableFineGrainedLockArray<Vertex<T>> slots, int newSlot, int hashID)

            #region InsertItem(int newSlot, int hashID, T value)
            /// <summary>
            /// 
            /// </summary>
            /// <param name="newSlot"></param>
            /// <param name="hashID">The hashID to search for and lock.</param>
            /// <param name="value"></param>
            public void InsertItem(int newSlot, int hashID, T value)
            {
                mData.ItemLock(newSlot);

                if (!Curr.IsTerminator)
                    mData.ItemUnlock(Curr.NextSlotIDPlus1 - 1);

                Next = new CollectionVertex<T>(hashID, value, Curr.NextSlotIDPlus1);

                Curr.NextSlotIDPlus1 = newSlot + 1;

                mData[newSlot] = Next;
                mData[CurrSlotIDPlus1 - 1] = Curr;

                mData.NotifyAdd(hashID, value);
            }
            #endregion

            #region Unlock()
            /// <summary>
            /// This method provides common functionality to unlock a VertexWindow.
            /// </summary>
            public void Unlock()
            {
                if (Curr.NextSlotIDPlus1 != 0) mData.ItemUnlock(Curr.NextSlotIDPlus1 - 1);
                if (CurrSlotIDPlus1 != 0) mData.ItemUnlock(CurrSlotIDPlus1 - 1);
            }
            #endregion
            #region ScanAndLock(int hashID)
            /// <summary>
            /// This method scans through the slot data until is reaches the end of the data, or the position 
            /// where the hashID meets a slot with a hashID that is greater than itself.
            /// </summary>
            /// <param name="hashID">The hashID to search for and lock.</param>
            public int ScanAndLock(int hashID)
            {
                //If the current is the last item in the linked list then exit.
                if (Curr.IsTerminator)
                    return 0;

                int hopCount = 0;

                while (Next.HashID < hashID)
                {
                    hopCount++;

                    //Unlock the old current item.
                    mData.ItemUnlock(CurrSlotIDPlus1 - 1);

                    CurrSlotIDPlus1 = Curr.NextSlotIDPlus1;

                    //If this is the last item in the list then move up and exit.
                    if (Next.IsTerminator)
                    {
                        Curr = Next;
                        Next = new CollectionVertex<T>();
                        break;
                    }

                    //OK, lock the next item and move up.
                    mData.ItemLock(Next.NextSlotIDPlus1 - 1);
                    Curr = Next;
                    Next = mData[Curr.NextSlotIDPlus1 - 1];
                }

                return hopCount;
            }
            #endregion
            #region MoveUp()
            /// <summary>
            /// This method moves up the Next vertex to the current position.
            /// </summary>
            public bool MoveUp()
            {
                if (Curr.IsTerminator)
                    return false;

                mData.ItemUnlock(CurrSlotIDPlus1 - 1);
                CurrSlotIDPlus1 = Curr.NextSlotIDPlus1;
                Curr = Next;

                if (!Curr.IsTerminator)
                {
                    mData.ItemLock(Curr.NextSlotIDPlus1 - 1);
                    Next = mData[Curr.NextSlotIDPlus1 - 1];
                }
                else
                    Next = new CollectionVertex<T>();

                return true;
            }
            #endregion // MoveUp()

            #region RemoveItemAndUnlock()
            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            public int RemoveItemAndUnlock()
            {
                int removedItem = Curr.NextSlotIDPlus1 - 1;

                Curr.NextSlotIDPlus1 = Next.NextSlotIDPlus1;

                mData[CurrSlotIDPlus1 - 1] = Curr;
                mData.ItemUnlock(removedItem);
                mData.ItemUnlock(CurrSlotIDPlus1 - 1);

                //Add the empty item for re-allocation.
                mData.EmptyAdd(removedItem);

                //Notify the collection of a deletion.
                mData.NotifyDelete(Next.HashID, Next.Value);

                return removedItem;
            }
            #endregion // SlotsRemoveItem

            #region Snip()
            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            public int Snip()
            {
                int removedItem = Curr.NextSlotIDPlus1 - 1;

                Curr.NextSlotIDPlus1 = Next.NextSlotIDPlus1;

                mData[CurrSlotIDPlus1 - 1] = Curr;
                mData.ItemUnlock(removedItem);

                //Notify the collection of a deletion.
                mData.NotifyDelete(Next.HashID, Next.Value);

                if (!Curr.IsTerminator)
                {
                    mData.ItemLock(Curr.NextSlotIDPlus1 - 1);
                    Next = mData[Curr.NextSlotIDPlus1 - 1];
                }
                else
                    Next = new CollectionVertex<T>();

                return removedItem;
            }
            #endregion // SlotsRemoveItem
        }
        #endregion // Struct -> Vertex<T>
        #region Constants
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
        /// This property specifies whether the collection is a fixed size.
        /// </summary>
        private bool mIsFixedSize;
        /// <summary>
        /// This collection holds the data.
        /// </summary>
        protected IFineGrainedLockArray<CollectionVertex<T>> mSlots;
        /// <summary>
        /// This is the vertex that holds the previously used vertexes.
        /// </summary>
        private LockableWrapper<CollectionVertex<T>> mEmptyVertex;
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

        #region IsFixedSize
        /// <summary>
        /// This property specifies whether the collection is a fixed size.
        /// </summary>
        public bool IsFixedSize { get { return mIsFixedSize; } }
        #endregion // IsFixedSize

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

        #region Initialize(bool isFixedSize, int capacity)
        /// <summary>
        /// This method initializes the class.
        /// </summary>
        /// <param name="isFixedSize">A boolean value indicating whether the data collection is fixed size.</param>
        /// <param name="capacity">The capacity of the data array.</param>
        protected virtual void Initialize(bool isFixedSize, int capacity)
        {
            mIsFixedSize = isFixedSize;
            mCapacity = capacity;

            mFreeListTail.Value = -1;
            mFreeListCount = 0;
            mLastIndex = 0;

            if (isFixedSize)
                mSlots = new FineGrainedLockArray<CollectionVertex<T>>(capacity, 0);
            else
                mSlots = new ExpandableFineGrainedLockArray<CollectionVertex<T>>(capacity, SlotExpander);

            mEmptyVertex = new LockableWrapper<CollectionVertex<T>>(CollectionVertex<T>.Sentinel(0, 0));
        }
        #endregion // Initialize(bool isFixedSize, int capacity)

        #region GetSentinelID(int hashCode, bool create, out int sentSlotID, out int hashID)
        /// <summary>
        /// This method returns the sentinel ID and the hashID for the hashcode passed.
        /// </summary>
        /// <param name="hashCode">The hashcode to search for the sentinel position.</param>
        /// <param name="create">This property determine whether any missing sentinels will be created.</param>
        /// <param name="sentIndexID">The largest sentinel index ID.</param>
        /// <param name="hashID">The hashID for the hashCode that passed.</param>
        public abstract void GetSentinelID(int hashCode, bool create, out int sentIndexID, out int hashID);
        #endregion // GetSentinel(int hashCode, bool create, out int sentSlotID, out int hashID)

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
                    CollectionVertex<T> item = mSlots[pos];

                    //OK, remove the free item from the list and set the sentinel to the next item.
                    mEmptyVertex.Value = new CollectionVertex<T>(0, default(T), item.NextSlotIDPlus1);

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

            if (nextItem == 0 || (mIsFixedSize && nextItem >= mCapacity))
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
                mSlots[index] = new CollectionVertex<T>(0, default(T), next);
                mEmptyVertex.Value = new CollectionVertex<T>(0, default(T), index + 1);

                mFreeListTail.Value = index;

                mEmptyVertex.Unlock();
            }
            else
            {
                mSlots[index] = CollectionVertex<T>.Empty;
                mSlots[mFreeListTail.Value] = new CollectionVertex<T>(0, default(T), index + 1);
                mFreeListTail.Value = index;
            }
            Interlocked.Increment(ref mFreeListCount);
            mFreeListTail.Unlock();
        }
        #endregion // EmptyAdd(int index)

        #region Length
        /// <summary>
        /// The data capacity.
        /// </summary>
        public virtual int Length
        {
            get { return mSlots.Length; }
        }
        #endregion // Length
        #region LengthSlots
        /// <summary>
        /// The data capacity.
        /// </summary>
        public virtual int LengthSlots
        {
            get { return mSlots.Length; }
        }
        #endregion // LengthSlots

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
        public virtual CollectionVertex<T> this[int index]
        {
            get
            {
                return mSlots[index];
            }
            set
            {
                mSlots[index] = value;
            }
        }
        #endregion // this[int index]

        #region IEnumerable<KeyValuePair<int,Vertex<T>>> Members
        /// <summary>
        /// This method returns an enumeration through the sentinels and data in the collection.
        /// </summary>
        /// <returns>Returns an enumeration containing the collection data.</returns>
        public virtual IEnumerator<KeyValuePair<int, ICollectionVertex<T>>> GetEnumerator()
        {
            KeyValuePair<int, ICollectionVertex<T>> item = new KeyValuePair<int, ICollectionVertex<T>>(0, mSlots[0]);
            yield return item;

            while (!item.Value.IsTerminator)
            {
                int id = item.Value.DataNextSlotID - 1;
                ItemLockWait(id);
                item = new KeyValuePair<int, ICollectionVertex<T>>(id, this[id]);

                yield return item;
            }
        }
        #endregion
        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator)GetEnumerator();
        }

        #endregion

        #region VertexWindowGet(int index)
        /// <summary>
        /// This method returns a vertex window for the index specified.
        /// </summary>
        public virtual VertexWindow<T> VertexWindowGet()
        {
            return new VertexWindow<T>(this, 0);
        }
        /// <summary>
        /// This method returns a vertex window for the index specified.
        /// </summary>
        /// <param name="index">The index position.</param>
        /// <returns>Returns the vertex window.</returns>
        public virtual VertexWindow<T> VertexWindowGet(int index)
        {
            return new VertexWindow<T>(this, index);
        }
        #endregion // VertexWindowGet(int index)

        #region SizeRecalculate(int total)
        /// <summary>
        /// This method calculates the current number of bits needed to support the current data.
        /// </summary>
        public abstract void SizeRecalculate(int total);
        #endregion

        #region NotifyAdd(int hashID, T value)
        /// <summary>
        /// This method is used to notify a collection of an addition.
        /// </summary>
        /// <param name="hashID"></param>
        /// <param name="value"></param>
        public virtual void NotifyAdd(int hashID, T value)
        {

        }
        #endregion // NotifyAdd(int hashID, T value)
        #region NotifyDelete(int hashID, T value)
        /// <summary>
        /// This method is used to notify a collection of an deletion.
        /// </summary>
        /// <param name="hashID"></param>
        /// <param name="value"></param>
        public virtual void NotifyDelete(int hashID, T value)
        {

        }
        #endregion // NotifyDelete(int hashID, T value)

#if (DEBUG)
        #region DebugEmpty
        /// <summary>
        /// This is the debug data.
        /// </summary>
        public virtual string DebugEmpty
        {
            get
            {
                StringBuilder sb = new StringBuilder();

                int count = 0;

                if (!mEmptyVertex.Value.IsTerminator)
                {
                    count++;
                    int index = mEmptyVertex.Value.NextSlotIDPlus1 - 1;
                    CollectionVertex<T> item = mSlots[index];

                    while (!item.IsTerminator)
                    {
                        index = item.NextSlotIDPlus1 - 1;

                        if ((index & 0x80000000) > 0)
                        {
                            sb.AppendFormat("Error: {0:X}", index);
                            break;
                        }

                        item = mSlots[index];
                        count++;
                    }
                }

                sb.AppendFormat("{0} empty slots", count);
                sb.AppendLine();
                return sb.ToString();
            }
        }
        #endregion // DebugEmpty
#endif
    }
}
