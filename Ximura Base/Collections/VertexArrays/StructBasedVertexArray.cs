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
        #region Static declarations
        /// <summary>
        /// This value holds an individual incremental ID for each thread.
        /// </summary>
        [ThreadStatic]
        static int sLockIndex;
        /// <summary>
        /// This value holds the maximum number of thread registered with the collection.
        /// </summary>
        static int sLockCounter = 0;
        #endregion // Static declarations

        #region Declarations
        /// <summary>
        /// This array holds the write thread status.
        /// </summary>
        private int[] mThreadWrite;
        /// <summary>
        /// This property specifies whether threads can modify data in the collection.
        /// </summary>
        private volatile int mWriteLock;
        /// <summary>
        /// This collection holds the data.
        /// </summary>
        private LockableWrapper<CollectionVertexStruct<T>>[] mSlots;
        /// <summary>
        /// This is the vertex that holds the previously used vertexes.
        /// </summary>
        private LockableNullableWrapper<CollectionVertexStruct<T>> mEmptyVertex;
        /// <summary>
        /// This is the free data queue item count.
        /// </summary>
        private volatile int mFreeListCount;
        /// <summary>
        /// This is the free data queue tail position.
        /// </summary>
        private LockableNullableWrapper<int> mFreeListTail;
        /// <summary>
        /// This is the current next free position in the data collection.
        /// </summary>
        private volatile int mLastIndex;
        /// <summary>
        /// This lock grows the collection.
        /// </summary>
        private object syncGrowLock = new object();
        #endregion // Declarations
        #region Constructor
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        protected StructBasedVertexArray()
        {
            WriteLockInitialize();
        }
        #endregion // Constructor

        #region MaxConcurrentThreads
        /// <summary>
        /// This protected property returns the maximum supported number of threads. You should override this
        /// method if you require the collection to support more threads.
        /// </summary>
        protected virtual int MaxConcurrentThreads { get { return Environment.ProcessorCount * 50; } }
        #endregion // MaxConcurrentThreads

        #region InitializeData()
        /// <summary>
        /// This method initializes the data collection.
        /// </summary>
        protected override void InitializeData()
        {
            mFreeListTail.Value = -1;
            mFreeListCount = 0;
            mLastIndex = 0;

            mSlots = new LockableWrapper<CollectionVertexStruct<T>>[InitialCapacity];

            mEmptyVertex = new LockableNullableWrapper<CollectionVertexStruct<T>>(CollectionVertexStruct<T>.Sentinel(0, 0));
        }
        #endregion

        #region EmptyGet()
        /// <summary>
        /// This method returns the next free item, either from empty space, or from a free item in the collection.
        /// </summary>
        /// <returns>Returns the index for the next free item.</returns>
        public int EmptyGet()
        {
            if (sLockIndex == 0)
                sLockIndex = Interlocked.Increment(ref sLockCounter);

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
                    mSlots[pos].Lock();

                    //OK get the item.
                    CollectionVertexStruct<T> item = mSlots[pos].Value;

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
                    mSlots[pos].Unlock();

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

            //Ok, get the next available item.
            int nextItem = Interlocked.Increment(ref mLastIndex);

            if (nextItem >= mSlots.Length)
            {
                if (mIsFixedSize)
                    throw new InvalidOperationException("The array is a fixed size and the capacity has been exceeded");

                lock (syncGrowLock)
                {
                    if (nextItem < mSlots.Length)
                        return nextItem - 1;

                    WriteLockAcquire();
                    try
                    {


                    }
                    finally
                    {
                        WriteLockRelease();
                    }
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
                mSlots[index].Value = new CollectionVertexStruct<T>(0, default(T), next);
                mEmptyVertex.Value = new CollectionVertexStruct<T>(0, default(T), index + 1);

                mFreeListTail.Value = index;

                mEmptyVertex.Unlock();
            }
            else
            {
                mSlots[index].Value = CollectionVertexStruct<T>.Empty;
                mSlots[mFreeListTail.Value].Value = new CollectionVertexStruct<T>(0, default(T), index + 1);
                mFreeListTail.Value = index;
            }
            Interlocked.Increment(ref mFreeListCount);
            mFreeListTail.Unlock();
        }
        #endregion // EmptyAdd(int index)


        #region Resize(int newCapacity)
        /// <summary>
        /// This method changes the size of the array.
        /// </summary>
        protected virtual void Resize()
        {
            int newCapacity = SlotExpander(mSlots.Length);

            LockableWrapper<CollectionVertexStruct<T>>[] newArray =
                new LockableWrapper<CollectionVertexStruct<T>>[newCapacity];

            int copyCapacity = newCapacity < mSlots.Length ? newCapacity : mSlots.Length;

            Array.Copy(mSlots, 0, newArray, 0, copyCapacity);

            Interlocked.Exchange(ref mSlots, newArray);
        }
        #endregion


        #region SlotExpander(int requiredSize, int currentSize)
        /// <summary>
        /// This expander grows the buckets by the specified amount.
        /// </summary>
        /// <param name="currentSize">The current capacity.</param>
        /// <returns>Returns the new capacity.</returns>
        protected virtual int SlotExpander(int currentSize)
        {
            return Prime.Get(currentSize * 2);
        }
        #endregion // SlotExpander(int requiredSize, int currentSize)


        #region LockableData(int index)
        /// <summary>
        /// This method returns the full lockable data from the slot array.
        /// </summary>
        /// <param name="index">The slot index.</param>
        /// <returns>Returns a lockable wrapper containing the vertex data.</returns>
        protected virtual LockableWrapper<CollectionVertexStruct<T>> LockableData(int index)
        {
            return mSlots[index];
        }
        #endregion // LockableData(int index)

        #region ItemIsLocked(int index)
        /// <summary>
        /// This method checks whether an item in the collection is locked.
        /// </summary>
        /// <param name="index">The index of the item to check.</param>
        /// <returns>Returns true if the item is locked.</returns>
        public virtual bool ItemIsLocked(int index)
        {
            return mSlots[index].IsLocked;
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
            while (mSlots[index].IsLocked)
                ThreadingHelper.ThreadWait();
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
            while (!ItemTryLock(index))
                ThreadingHelper.ThreadWait();
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
            if (mIsFixedSize)
                return mSlots[index].TryLock();

            WriteEnter();
            try
            {
                return mSlots[index].TryLock();
            }
            finally
            {
                WriteExit();
            }
        }
        #endregion // ItemTryLock(int index)
        #region ItemUnlock(int index)
        /// <summary>
        /// The method unlocks the item.
        /// </summary>
        /// <param name="index">The index of the item you wish to unlock.</param>
        public virtual void ItemUnlock(int index)
        {
            if (mIsFixedSize)
            {
                mSlots[index].Unlock();
                return;
            }

            WriteEnter();
            try
            {
                mSlots[index].Unlock();
            }
            finally
            {
                WriteExit();
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
                return mSlots[index].Value;
            }
            set
            {
                if (mIsFixedSize)
                {
                    mSlots[index].Value = value;
                    return;
                }

                WriteEnter();
                try
                {
                    mSlots[index].Value = value;
                }
                finally
                {
                    WriteExit();
                }
            }
        }
        #endregion // this[int index]

        #region WriteLockInitialize()
        /// <summary>
        /// This method initializes the write lock logic.
        /// </summary>
        private void WriteLockInitialize()
        {
            mThreadWrite = new int[MaxConcurrentThreads];
            mWriteLock = 0;
        }
        #endregion // WriteLockInitialize()
        #region WriteEnter()
        /// <summary>
        /// This method is called when a thread enters a critical section that can modify the slot data.
        /// </summary>
        private void WriteEnter()
        {
            //Wait for lock release before entering the critical section.
            while (mWriteLock == 1)
                ThreadingHelper.ThreadWait();

            if (sLockIndex == 0)
                sLockIndex = Interlocked.Increment(ref sLockCounter);

            mThreadWrite[sLockIndex] = 1;
        }
        #endregion // WriteEnter()
        #region WriteExit()
        /// <summary>
        /// This method is called when a thread leaves a critical section that can modify the slot data.
        /// </summary>
        private void WriteExit()
        {
            mThreadWrite[sLockIndex] = 0;
        }
        #endregion // WriteExit()


        private void WriteLockAcquire()
        {
            //Set the lock to stop new threads from entering.
            mWriteLock = 1;

            //Wait for all threads to leave the critical section.
            int id = sLockCounter;
            for (; --id >= 0;)
            {
                if (mThreadWrite[id] == 1)
                {
                    ThreadingHelper.ThreadWait();
                    id = sLockCounter;
                    continue;
                }
            }
        }

        private void WriteLockRelease()
        {
            //Unset the lock. Threads can now enter write sections.
            mWriteLock = 0;

        }




        #region GetEnumerator()
        /// <summary>
        /// This method returns an enumeration through the sentinels and data in the collection.
        /// </summary>
        /// <returns>Returns an enumeration containing the collection data.</returns>
        public override IEnumerator<KeyValuePair<int, ICollectionVertex<T>>> GetEnumerator()
        {
            CollectionVertexStruct<T> item = mSlots[0].Value;

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
