﻿#region Copyright
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
    /// This class contains the combined buckets and slots in a single class.
    /// </summary>
    /// <typeparam name="T">The collection type.</typeparam>
    public class HashTableStructBasedVertexArrayV3<T> :
        MultiLevelBucketStructBasedVertexArray<T, CollectionVertexStruct<T>>
    {
        #region StructBasedVertexWindowV3
        /// <summary>
        /// The vertex window structure holds the search results from a scan.
        /// </summary>
        /// <typeparam name="T">The collection type.</typeparam>
        [Serializable, StructLayout(LayoutKind.Sequential)]
        private struct StructBasedVertexWindowV3<T> : IVertexWindow<T>
        {
            #region Declarations
            HashTableStructBasedVertexArrayV3<T> mData;
            int mHashID;
            T mItem;
            IEqualityComparer<T> mEqualityComparer;
            /// <summary>
            /// The current slot ID plus 1.
            /// </summary>
            private int CurrSlotIDPlus1;
            /// <summary>
            /// THe current vertex.
            /// </summary>
            private CollectionVertexStruct<T> Curr;
            /// <summary>
            /// The next vertex.
            /// </summary>
            private CollectionVertexStruct<T> Next;
            #endregion // Declarations
            #region Constructor
            /// <summary>
            /// This is the default constructor for the window.
            /// </summary>
            /// <param name="data">The data collection.</param>
            /// <param name="indexID">The index of the position to set the window.</param>
            public StructBasedVertexWindowV3(HashTableStructBasedVertexArrayV3<T> data,
                IEqualityComparer<T> eqComparer, int indexID, int hashID, T item)
            {
#if (LOCKDEBUG)
                Console.WriteLine("Window created for {0:x} {1:x} on {2}"
                    , indexID, hashID, Thread.CurrentThread.ManagedThreadId);
#endif
                mData = data;
                mHashID = hashID;
                mItem = item;
                mEqualityComparer = eqComparer;


                mData.ItemLock(indexID);
                CurrSlotIDPlus1 = indexID + 1;
                Curr = mData[indexID];

                if (!Curr.IsTerminator)
                {
                    mData.ItemLock(Curr.NextSlotIDPlus1 - 1);
                    Next = mData[Curr.NextSlotIDPlus1 - 1];
                }
                else
                    Next = new CollectionVertexStruct<T>();
            }
            #endregion // Constructor
            #region Sentinel Constructor
            /// <summary>
            /// This is the default constructor for the window.
            /// </summary>
            /// <param name="data">The data collection.</param>
            /// <param name="indexID">The index of the position to set the window.</param>
            public StructBasedVertexWindowV3(HashTableStructBasedVertexArrayV3<T> data,
                IEqualityComparer<T> eqComparer, int indexID, int hashID)
            {
                mData = data;
                mHashID = hashID;
                mItem = default(T);
                mEqualityComparer = eqComparer;
#if (LOCKDEBUG)
                Console.WriteLine("S Window created for {0:x} {1:x} on {2}"
                    , indexID, mHashID, Thread.CurrentThread.ManagedThreadId);
#endif
                mData.ItemLock(indexID);
                CurrSlotIDPlus1 = indexID + 1;
                Curr = mData[indexID];

                if (!Curr.IsTerminator)
                {
                    mData.ItemLock(Curr.NextSlotIDPlus1 - 1);
                    Next = mData[Curr.NextSlotIDPlus1 - 1];
                }
                else
                    Next = new CollectionVertexStruct<T>();
            }
            #endregion // Constructor

            #region HashID
            /// <summary>
            /// This is the hash ID of the item currently being searched.
            /// </summary>
            public int HashID
            {
                get { return mHashID; }
            }
            #endregion
            #region Value
            /// <summary>
            /// This is the current value being handled by the window.
            /// </summary>
            public T Value
            {
                get { return mItem; }
            }
            #endregion

            #region ScanAndRemoveMarked()
            /// <summary>
            /// This method scans through the slot data until is reaches the end of the data, or the position 
            /// where the hashID meets a slot with a hashID that is greater than itself.
            /// </summary>
            /// <param name="hashID">The hashID to search for and lock.</param>
            public int ScanAndRemoveMarked()
            {
                //If the current is the last item in the linked list then exit.
                if (Curr.IsTerminator)
                    return 0;

                int hopCount = 0;

                while (Next.HashID < mHashID)
                {
                    hopCount++;

                    //Unlock the old current item.
                    mData.ItemUnlock(CurrSlotIDPlus1 - 1);

                    CurrSlotIDPlus1 = Curr.NextSlotIDPlus1;

                    //If this is the last item in the list then move up and exit.
                    if (Next.IsTerminator)
                    {
                        Curr = Next;
                        Next = new CollectionVertexStruct<T>();
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
            #region ItemLockAndInsert()
            /// <summary>
            /// 
            /// </summary>
            /// <param name="hashID">The hashID to search for and lock.</param>
            public void ItemLockAndInsert()
            {
                int newSlot = mData.EmptyGet();

                mData.ItemLock(newSlot);

                if (!Curr.IsTerminator)
                    mData.ItemUnlock(Curr.NextSlotIDPlus1 - 1);

                Next = new CollectionVertexStruct<T>(mHashID, mItem, Curr.NextSlotIDPlus1);

                Curr.NextSlotIDPlus1 = newSlot + 1;

                mData[newSlot] = Next;
                mData[CurrSlotIDPlus1 - 1] = Curr;

                //Increment the necessary counters, and
                //check whether we need to recalculate the bit size.
                mData.BucketSizeRecalculate(mData.CountIncrement(), false);
            }
            #endregion
            #region ItemLockAndRemove()
            /// <summary>
            /// 
            /// </summary>
            public void ItemLockAndRemove()
            {
                int removedItem = Curr.NextSlotIDPlus1 - 1;

                Curr.NextSlotIDPlus1 = Next.NextSlotIDPlus1;

                mData[CurrSlotIDPlus1 - 1] = Curr;
                mData.ItemUnlock(removedItem);
                mData.ItemUnlock(CurrSlotIDPlus1 - 1);

                //Add the empty item for re-allocation.
                mData.EmptyAdd(removedItem);

                //Update the version and reduce the item count.
                mData.CountDecrement();
#if (LOCKDEBUG)
                Console.WriteLine("Window remove and unlock for {0:x} {1:x} on {2}"
                    , CurrSlotIDPlus1 - 1, mHashID, Thread.CurrentThread.ManagedThreadId);
#endif
            }
            #endregion // ItemLockAndRemove()
            #region ItemLockAndSetNext()
            /// <summary>
            /// This method changes the value of the next item.
            /// </summary>
            /// <param name="value">The new value.</param>
            public void ItemLockAndSetNext()
            {
                //This code is to accomodate dictionary type collections where the item is a keyvalue pair.
                Next.Value = mItem;
                mData[Curr.NextSlotIDPlus1 - 1] = Next;
            }
            #endregion // SetNextItem(T value)
            #region MoveUpAndRemoveMarked()
            /// <summary>
            /// This method moves up the Next vertex to the current position.
            /// </summary>
            public bool MoveUpAndRemoveMarked()
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
                    Next = new CollectionVertexStruct<T>();

                return true;
            }
            #endregion
            #region SentinelLockAndInsert(int indexID)
            /// <summary>
            /// This method inserts a sentinel in to the data collection.
            /// </summary>
            /// <param name="indexID">The new sentinel index id.</param>
            /// <param name="hashID">The sentinel hash id.</param>
            public void SentinelLockAndInsert(int indexID)
            {
                ScanAndLock();

                //If the current item is part of a list unlock the next item.
                if (!Curr.IsTerminator)
                    mData.ItemUnlock(Curr.NextSlotIDPlus1 - 1);

                //Set the new sentinel and unlock.
                mData[indexID] = CollectionVertexStruct<T>.Sentinel(mHashID, Curr.NextSlotIDPlus1);
                mData.ItemUnlock(indexID);

                //Set the current item and unlock.
                Curr.NextSlotIDPlus1 = indexID + 1;
                mData[CurrSlotIDPlus1 - 1] = Curr;
                mData.ItemUnlock(CurrSlotIDPlus1 - 1);

#if (LOCKDEBUG)
                Console.WriteLine("Window sentinel unlocked for {0:x} {1:x} on {2}"
                    , CurrSlotIDPlus1 - 1, mHashID, Thread.CurrentThread.ManagedThreadId);
#endif
            }
            #endregion

            #region ItemSetNext()
            /// <summary>
            /// This method changes the value of the next item.
            /// </summary>
            /// <param name="value">The new value.</param>
            public void ItemSetNext()
            {
                //This code is to accomodate dictionary type collections where the item is a keyvalue pair.
                Next.Value = mItem;
                mData[Curr.NextSlotIDPlus1 - 1] = Next;
            }
            #endregion // SetNextItem(T value)
            #region ItemInsert(T value)
            /// <summary>
            /// 
            /// </summary>
            /// <param name="hashID">The hashID to search for and lock.</param>
            public void ItemInsert()
            {
                int newSlot = mData.EmptyGet();

                mData.ItemLock(newSlot);

                if (!Curr.IsTerminator)
                    mData.ItemUnlock(Curr.NextSlotIDPlus1 - 1);

                Next = new CollectionVertexStruct<T>(mHashID, mItem, Curr.NextSlotIDPlus1);

                Curr.NextSlotIDPlus1 = newSlot + 1;

                mData[newSlot] = Next;
                mData[CurrSlotIDPlus1 - 1] = Curr;

                //Increment the necessary counters, and
                //check whether we need to recalculate the bit size.
                mData.BucketSizeRecalculate(mData.CountIncrement(), false);
            }
            #endregion

            #region InsertSentinelAndUnlock(int indexID)
            /// <summary>
            /// This method inserts a sentinel in to the data collection.
            /// </summary>
            /// <param name="indexID">The new sentinel index id.</param>
            /// <param name="hashID">The sentinel hash id.</param>
            public void InsertSentinelAndUnlock(int indexID)
            {
                ScanAndLock();

                //If the current item is part of a list unlock the next item.
                if (!Curr.IsTerminator)
                    mData.ItemUnlock(Curr.NextSlotIDPlus1 - 1);

                //Set the new sentinel and unlock.
                mData[indexID] = CollectionVertexStruct<T>.Sentinel(mHashID, Curr.NextSlotIDPlus1);
                mData.ItemUnlock(indexID);

                //Set the current item and unlock.
                Curr.NextSlotIDPlus1 = indexID + 1;
                mData[CurrSlotIDPlus1 - 1] = Curr;
                mData.ItemUnlock(CurrSlotIDPlus1 - 1);

#if (LOCKDEBUG)
                Console.WriteLine("Window sentinel unlocked for {0:x} {1:x} on {2}"
                    , CurrSlotIDPlus1 - 1, mHashID, Thread.CurrentThread.ManagedThreadId);
#endif
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

#if (LOCKDEBUG)
                Console.WriteLine("Window unlocked for {0:x} {1:x} on {2}"
                    , CurrSlotIDPlus1-1, mHashID, Thread.CurrentThread.ManagedThreadId);
#endif
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
                    Next = new CollectionVertexStruct<T>();

                return true;
            }
            #endregion
            #region ScanAndLock()
            /// <summary>
            /// This method scans through the slot data until is reaches the end of the data, or the position 
            /// where the hashID meets a slot with a hashID that is greater than itself.
            /// </summary>
            /// <param name="hashID">The hashID to search for and lock.</param>
            public int ScanAndLock()
            {
                //If the current is the last item in the linked list then exit.
                if (Curr.IsTerminator)
                    return 0;

                int hopCount = 0;

                while (Next.HashID < mHashID)
                {
                    hopCount++;

                    //Unlock the old current item.
                    mData.ItemUnlock(CurrSlotIDPlus1 - 1);

                    CurrSlotIDPlus1 = Curr.NextSlotIDPlus1;

                    //If this is the last item in the list then move up and exit.
                    if (Next.IsTerminator)
                    {
                        Curr = Next;
                        Next = new CollectionVertexStruct<T>();
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
            #region RemoveItemAndUnlock()
            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            public void ItemRemoveAndUnlock()
            {
                int removedItem = Curr.NextSlotIDPlus1 - 1;

                Curr.NextSlotIDPlus1 = Next.NextSlotIDPlus1;

                mData[CurrSlotIDPlus1 - 1] = Curr;
                mData.ItemUnlock(removedItem);
                mData.ItemUnlock(CurrSlotIDPlus1 - 1);

                //Add the empty item for re-allocation.
                mData.EmptyAdd(removedItem);

                //Update the version and reduce the item count.
                mData.CountDecrement();
#if (LOCKDEBUG)
                Console.WriteLine("Window remove and unlock for {0:x} {1:x} on {2}"
                    , CurrSlotIDPlus1 - 1, mHashID, Thread.CurrentThread.ManagedThreadId);
#endif
            }
            #endregion // SlotsRemoveItem

            #region Snip()
            /// <summary>
            /// 
            /// </summary>
            public void Snip()
            {
                int removedItem = Curr.NextSlotIDPlus1 - 1;

                Curr.NextSlotIDPlus1 = Next.NextSlotIDPlus1;

                mData[CurrSlotIDPlus1 - 1] = Curr;
                mData.ItemUnlock(removedItem);

                if (!Curr.IsTerminator)
                {
                    mData.ItemLock(Curr.NextSlotIDPlus1 - 1);
                    Next = mData[Curr.NextSlotIDPlus1 - 1];
                }
                else
                    Next = new CollectionVertexStruct<T>();

                //Add the empty item for re-allocation.
                mData.EmptyAdd(removedItem);

                mData.CountDecrement();
            }
            #endregion // SlotsRemoveItem

            #region ScanProcess()
            public bool ScanProcess()
            {
                return !Curr.IsTerminator && Next.HashID == mHashID;
            }
            #endregion
            #region ScanItemMatch
            /// <summary>
            /// This property specifies whether the next item is a match for the data.
            /// </summary>
            public bool ScanItemMatch
            {
                get
                {
                    return !Next.IsSentinel && mEqualityComparer.Equals(mItem, Next.Data);
                }
            }
            #endregion // ScanItemMatch

            #region NextIsSentinel
            public bool NextIsSentinel
            {
                get { return Next.IsSentinel; }
            }
            #endregion // NextIsSentinel
            #region CurrIsTerminator
            public bool CurrIsTerminator
            {
                get { return Curr.IsTerminator; }
            }
            #endregion // CurrIsTerminator
            #region NextData

            public T NextData
            {
                get { return Next.Data; }
            }

            #endregion

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
        }
        #endregion
        #region Constructor
        /// <summary>
        /// This is the default constructor for the array.
        /// </summary>
        /// <param name="isFixedSize">A boolean value indicating whether the data collection is fixed size.</param>
        /// <param name="capacity">The array capacity.</param>
        public HashTableStructBasedVertexArrayV3()
        {
        }
        #endregion // Constructor

        #region RootIndexID
        /// <summary>
        /// This is the index ID of the the first item.
        /// </summary>
        protected override int RootIndexID { get { return unchecked((int)0x80000000); } }
        #endregion // RootIndexID

        #region InitializeBucketArray(int initialCapacity)
        /// <summary>
        /// This method initializes the data array.
        /// </summary>
        protected override void InitializeBucketArray(int initialCapacity)
        {
            base.InitializeBucketArray(initialCapacity);
            //Set the initial sentinel.
            this[RootIndexID] = CollectionVertexStruct<T>.Sentinel(RootIndexID, 0);
        }
        #endregion // InitializeDataArray()

        #region ItemIsLocked(int index)
        /// <summary>
        /// This method checks whether an item in the collection is locked.
        /// </summary>
        /// <param name="index">The index of the item to check.</param>
        /// <returns>Returns true if the item is locked.</returns>
        public override bool ItemIsLocked(int index)
        {
            int noMaskIndex = index & 0x7FFFFFFF;
            if (noMaskIndex == index)
                return base.ItemIsLocked(index);

            int level, levelPosition;
            BucketCalculatePosition(noMaskIndex, out level, out levelPosition);
            return mBuckets[level][levelPosition].IsLocked;
        }
        #endregion // ItemIsLocked(int index)
        #region ItemLock(int index)
        /// <summary>
        /// This method locks the specific item.
        /// </summary>
        /// <param name="index">The item index.</param>
        /// <returns>Returns the number of lock cycles the thread entered.</returns>
        public override void ItemLock(int index)
        {
            int noMaskIndex = index & 0x7FFFFFFF;
            if (noMaskIndex == index)
                base.ItemLock(index);
            else
                BucketLock(noMaskIndex);
        }
        #endregion // ItemLock(int index)
        #region ItemLockWait(int index)
        /// <summary>
        /// This method waits for a locked item to become available.
        /// </summary>
        /// <param name="index">The index of the item to wait for.</param>
        /// <returns>Returns the number of lock cycles during the wait.</returns>
        public override void ItemLockWait(int index)
        {
            int noMaskIndex = index & 0x7FFFFFFF;
            if (noMaskIndex == index)
            {
                base.ItemLockWait(index);
                return;
            }

            int level, levelPosition;
            BucketCalculatePosition(noMaskIndex, out level, out levelPosition);
            mBuckets[level][levelPosition].LockWait();
        }
        #endregion // ItemLockWait(int index)
        #region ItemTryLock(int index)
        /// <summary>
        /// This method attempts to lock the item specified.
        /// </summary>
        /// <param name="index">The index of the item you wish to lock..</param>
        /// <returns>Returns true if the item was successfully locked.</returns>
        public override bool ItemTryLock(int index)
        {
            int noMaskIndex = index & 0x7FFFFFFF;
            if (noMaskIndex == index)
                return base.ItemTryLock(index);

            int level, levelPosition;
            BucketCalculatePosition(noMaskIndex, out level, out levelPosition);
            return mBuckets[level][levelPosition].TryLock();
        }
        #endregion // ItemTryLock(int index)
        #region ItemUnlock(int index)
        /// <summary>
        /// The method unlocks the item.
        /// </summary>
        /// <param name="index">The index of the item you wish to unlock.</param>
        public override void ItemUnlock(int index)
        {
            int noMaskIndex = index & 0x7FFFFFFF;
            if (noMaskIndex == index)
            {
                base.ItemUnlock(index);
                return;
            }

            BucketUnlock(noMaskIndex);
        }
        #endregion
        #region this[int index]
        /// <summary>
        /// This is the indexer for the array.
        /// </summary>
        /// <param name="index">The index position.</param>
        /// <returns>Returns the vertex corresponding to the index position.</returns>
        public override CollectionVertexStruct<T> this[int index]
        {
            get
            {
                int noMaskIndex = index & 0x7FFFFFFF;
                if (noMaskIndex == index)
                    return base[index];

                return Bucket(noMaskIndex);
            }
            set
            {
                int noMaskIndex = index & 0x7FFFFFFF;
                if (noMaskIndex == index)
                {
                    base[index] = value;
                    return;
                }

                int level, levelPosition;
                BucketCalculatePosition(noMaskIndex, out level, out levelPosition);
                mBuckets[level][levelPosition].Value = value;
            }
        }
        #endregion // this[int index]
        #region LockableData(int index, out bool isLocked)
        /// <summary>
        /// This method reads the vertex from the base collection, as well as the vertex's lock status,
        /// </summary>
        /// <param name="index">The item index.</param>
        /// <param name="isLocked">A boolean value indicating whether the vertex is locked.</param>
        /// <returns>Returns the vertex.</returns>
        protected override CollectionVertexStruct<T> LockableData(int index, out bool isLocked)
        {
            int noMaskIndex = index & 0x7FFFFFFF;
            if (noMaskIndex == index)
                return base.LockableData(index, out isLocked);

            int level, levelPosition;
            BucketCalculatePosition(noMaskIndex, out level, out levelPosition);
            LockableWrapper<CollectionVertexStruct<T>> data = mBuckets[level][levelPosition];
            isLocked = data.IsLocked;
            return data.Value;
        }
        #endregion // LockableData(int index, out bool isLocked)

        #region VertexWindowGet(int index)
        /// <summary>
        /// This method returns a vertex window for the index specified.
        /// </summary>
        /// <returns>Returns a vertex window.</returns>
        public override IVertexWindow<T> VertexWindowGet()
        {
            return new StructBasedVertexWindowV3<T>(this, mEqComparer, RootIndexID, 0, default(T));
        }
        #endregion // VertexWindowGet(int index)
        #region VertexWindowGet(int hashCode, bool createSentinel)
        /// <summary>
        /// This method returns a vertex window for the hashCode specified.
        /// </summary>
        /// <param name="item">The item to get the window for.</param>
        /// <param name="createSentinel">A boolean value that specifies whether the sentinels should be created.</param>
        /// <returns>Returns a vertex window.</returns>
        public override IVertexWindow<T> VertexWindowGet(T item, bool createSentinel)
        {
            int hashCode = mEqComparer.GetHashCode(item);
            int hashID;
            int sentIndexID = GetSentinelID(mEqComparer.GetHashCode(item), createSentinel, out hashID);

            //Ok, set the MSB to indicate the value is a sentinel.
            return new StructBasedVertexWindowV3<T>(this, mEqComparer, sentIndexID, hashID, item);
        }
        #endregion // VertexWindowGet(int hashCode, bool createSentinel)

        #region BucketCalculatePosition(int indexID, out int level, out int levelPosition)
        /// <summary>
        /// This method calculates the specific bucket level and the position within that bucket.
        /// </summary>
        /// <param name="indexID">The bucket index.</param>
        /// <param name="level">The bucket level.</param>
        /// <param name="levelPosition">The bucket position.</param>
        protected override void BucketCalculatePosition(int indexID, out int level, out int levelPosition)
        {
            if (indexID <= 0)
            {
                level = indexID;
                levelPosition = 0;
                return;
            }

            int levelCurrent = BucketsLevelCurrent;
            int mask = (int.MaxValue >> (31 - levelCurrent));

            mask = (mask + 1) >> 1;

            for (level = levelCurrent; level > 0; level--)
            {
                if ((indexID & mask) > 0)
                    break;
                mask >>= 1;
            }

            if (level == 0)
                levelPosition = indexID;
            else
                levelPosition = indexID & (mask - 1);
        }
        #endregion
        #region BucketLevelCapacityCalculate(int level)
        /// <summary>
        /// This method calculates the size of the bucket array.
        /// </summary>
        /// <param name="level">The level.</param>
        /// <returns>Returns 2n+1 as the size of the array where n is the level.</returns>
        protected override int BucketLevelCapacityCalculate(int level)
        {
            if (level == 0)
                return 1;

            return (1 << level - 1);
        }
        #endregion // BucketLevelSize(int level)

        #region ConvertBucketIDToIndexID(int bucketID)
        /// <summary>
        /// This method converts a bucketID to an index ID by setting the MSB.
        /// </summary>
        /// <param name="bucketID">The bucket ID to convert.</param>
        /// <returns>The index ID.</returns>
        int ConvertBucketIDToIndexID(int bucketID)
        {
            return unchecked((int)(bucketID | 0x80000000));
        }
        #endregion // ConvertBucketIDToIndexID(int bucketID)

        #region GetSentinelID(int hashCode, bool createSentinel, out int sentIndexID, out int hashID)
        /// <summary>
        /// This method returns the sentinel ID and the hashID for the hashcode passed.
        /// </summary>
        /// <param name="hashCode">The hashcode to search for the sentinel position.</param>
        /// <param name="createSentinel">This property determine whether any missing sentinels will be created.</param>
        /// <param name="hashID">The hashID for the hashCode that passed.</param>
        /// <returns>The largest sentinel index ID.</returns>
        protected override int GetSentinelID(int hashCode, bool createSentinel, out int hashID)
        {
            hashCode &= cnLowerBitMask;
            hashID = BitReverse(hashCode);

            int bitsStart = BucketsLevelCurrent;
            int bitsCurrent = bitsStart;
            int bucketIDParent = -1;

            int bucketID = hashCode & (int.MaxValue >> (31 - bitsCurrent));

            while (bitsCurrent >= 0)
            {
                if (bucketID != bucketIDParent)
                {
                    bucketIDParent = bucketID;

                    //OK, check until we have reached a bucket with data, or we have reached the base bucket.
                    if (bucketID == 0 || Bucket(bucketID).IsSentinel)
                        break;
                }

                bitsCurrent--;

                bucketID = hashCode & (int.MaxValue >> (31 - bitsCurrent));
            }

            //OK, if we can create the missing sentinels then do so.
            if (createSentinel)
            {
                //OK, loop and create any required sentinels.
                while (bitsCurrent < bitsStart)
                {
                    bitsCurrent++;

                    bucketID = hashCode & (int.MaxValue >> (31 - bitsCurrent));

                    if (bucketID == bucketIDParent)
                        continue;

                    BucketLock(bucketID);
                    //Check whether the bucket has been created, shouldn't happen but best to be safe.
                    if (Bucket(bucketID).HashID > 0)
                    {
                        BucketUnlock(bucketID);
                        continue;
                    }

                    //Ok, get the hash ID for the bucket.
                    int bucketHashID = BitReverse(bucketID);

                    //Ok, insert the sentinel.
                    StructBasedVertexWindowV3<T> vWin = new StructBasedVertexWindowV3<T>(
                        this, mEqComparer, ConvertBucketIDToIndexID(bucketIDParent), bucketHashID);

                    //Insert the new sentinel and unlock.
                    vWin.SentinelLockAndInsert(ConvertBucketIDToIndexID(bucketID));

                    //OK set the parent bucket and move to the next sentinel.
                    bucketIDParent = bucketID;
                }
            }

            //Ok, set the MSB to indicate the value is a sentinel.
            return ConvertBucketIDToIndexID(bucketID);
        }
        #endregion // GetSentinel(int hashCode, bool create, out int sentSlotID, out int hashID)

        #region FastContains
        /// <summary>
        /// Identifies whether this array supports a fast search algorithm
        /// </summary>
        public override bool SupportsFastContain { get { return true; } }
        /// <summary>
        /// This method implements a fast search algoritm.
        /// </summary>
        /// <param name="item">The item to search for.</param>
        /// <returns>Returns true if found, false if not found, and null if the search encountered modified data.</returns>
        public override bool? FastContains(T item)
        {
            //Is this a null or default value?
            if (mEqComparer.Equals(item, default(T)))
                return DefaultTContains();

            //Get the initial sentinel vertex. 
            int hashID;
            int sentIndexID = GetSentinelID(mEqComparer.GetHashCode(item), false, out hashID);
            CollectionVertexStruct<T> scanVertex = this[sentIndexID];

            //First we will attempt to search without locking. However, should the version ID change 
            //during the search we will need to complete a locked search to ensure consistency.
            while (true)
            {
                //Do we have a match?
                if (!scanVertex.IsSentinel && 
                    !scanVertex.IsMarked &&
                    scanVertex.HashID == hashID &&
                    mEqComparer.Equals(item, scanVertex.Value))
                    return true;

                //Is this the end of the line
                if (scanVertex.IsTerminator || scanVertex.HashID > hashID)
                    return false;

                sentIndexID = scanVertex.NextSlotIDPlus1 - 1;
                //Ok, wait if the current vertex is locked.
                ItemLockWait(sentIndexID);
                scanVertex = this[sentIndexID];
            }
        }
        /// <summary>
        /// This method implements a fast search algoritm.
        /// </summary>
        /// <param name="eqComparer">The equality comparer to use for the comparison.</param>
        /// <param name="item">The item to search for.</param>
        /// <param name="value">The output value. This is used for KeyValue pair arrangement for Dictionary type collection.</param>
        /// <returns>Returns a trinary boolean value, true for success, false for fail, and null to continue using a lockable search.</returns>
        public override bool? FastContains(IEqualityComparer<T> eqComparer, T item, out T value)
        {
            //Is this a null or default value, as that is not supported by key/value pair based arrays.?
            if (eqComparer.Equals(item, default(T)))
            {
                value = default(T);
                return false;
            }

            //Get the initial sentinel vertex. 
            int hashID;
            int sentIndexID = GetSentinelID(mEqComparer.GetHashCode(item), false, out hashID);
            CollectionVertexStruct<T> scanVertex = this[sentIndexID];

            //First we will attempt to search without locking. However, should the version ID change 
            //during the search we will need to complete a locked search to ensure consistency.
            while (true)
            {
                //Do we have a match?
                if (!scanVertex.IsSentinel &&
                    !scanVertex.IsMarked &&
                    scanVertex.HashID == hashID &&
                    mEqComparer.Equals(item, scanVertex.Value))
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

                sentIndexID = scanVertex.NextSlotIDPlus1 - 1;
                //Ok, wait if the current vertex is locked.
                ItemLockWait(sentIndexID);
                scanVertex = this[sentIndexID];
            }
        }

        #endregion // FastContain
        #region FastAdd
        /// <summary>
        /// This property specifies that the array supports a fast add algorithm.
        /// </summary>
        public override bool SupportsFastAdd { get { return true; } }
        /// <summary>
        /// This is a fast add implementation of the add algorithm.
        /// </summary>
        /// <param name="item">The item to add.</param>
        /// <param name="add">Specifies whether a new item should be added, or an existing item replaced. 
        /// This is useful for KeyValue pair implementations.</param>
        /// <returns>Returns true if the item was added successfully.</returns>
        public override bool FastAdd(T item, bool add)
        {
            try
            {
                //Is this a null or default value?
                if (mEqComparer.Equals(item, default(T)))
                    return DefaultTAdd();

                int hashID;
                int sentIndexID = GetSentinelID(mEqComparer.GetHashCode(item), true, out hashID);
                StructBasedVertexWindowV3<T> vWin = new StructBasedVertexWindowV3<T>(this, mEqComparer, sentIndexID, hashID, item);

                //Ok, let's add the data from the sentinel position.
                //This method will scan unlocked until it reaches the hashID
                vWin.ScanAndRemoveMarked();

                //Ok, we need to scan for hash collisions and multiple entries.
                while (vWin.ScanProcess())
                {
                    if (vWin.ScanItemMatch)
                    {
                        //Ok, we have a match.
                        if (!add)
                        {
                            //This code is to accomodate dictionary type collections where the item is a keyvalue pair.
                            vWin.ItemLockAndSetNext();
                            return true;
                        }

                        if (AllowMultipleEntries)
                            break;
                        else
                            return false;
                    }

                    vWin.MoveUpAndRemoveMarked();
                }

                //Ok, add the data in the collection.
                try
                {
                    vWin.ItemLockAndInsert();
                }
                catch (Exception exx)
                {
                    throw exx;
                }

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion // FastAdd
        #region FastRemove
        /// <summary>
        /// This property specifies that the array supports a fast remove algorithm.
        /// </summary>
        public override bool SupportsFastRemove { get { return true; } }
        /// <summary>
        /// This method is a fast implementation of the remove algorithm.
        /// </summary>
        /// <param name="item">The item to remove.</param>
        /// <returns>Returns true if the item was successfully removed.</returns>
        public override bool FastRemove(T item)
        {
            //Is this a null or default value?
            if (mEqComparer.Equals(item, default(T)))
                return DefaultTDelete();

            int hashID;
            int sentIndexID = GetSentinelID(mEqComparer.GetHashCode(item), false, out hashID);
            StructBasedVertexWindowV3<T> vWin = new StructBasedVertexWindowV3<T>(this, mEqComparer, sentIndexID, hashID, item);
            vWin.ScanAndRemoveMarked();

            //Ok, we need to scan for hash collisions and multiple entries.
            while (vWin.ScanProcess())
            {
                if (vWin.ScanItemMatch)
                {
                    //Remove the item from the linked list.
                    vWin.ItemLockAndRemove();
                    return true;
                }

                vWin.MoveUpAndRemoveMarked();
            }

            //Ok, the item cannot be found.
            return false;
        }
        #endregion // FastRemove
        #region FastClear
        /// <summary>
        /// This property specifies that the array supports a fast clear algorithm.
        /// </summary>
        public override bool SupportsFastClear { get { return false; } }
        /// <summary>
        /// This method clears the array of data.
        /// </summary>
        public override void FastClear()
        {
            this.Where(s => !s.Value.IsSentinel && !s.Value.IsMarked)
                .ForEach(v =>
                {
                    while (!ItemIsMarked(v.Key) && !ItemTryMark(v.Key))
                        ThreadingHelper.ThreadWait();
                });
        }
        #endregion // FastClear

    }
}
