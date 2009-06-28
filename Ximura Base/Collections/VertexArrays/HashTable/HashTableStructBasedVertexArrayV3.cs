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
//    public class HashTableStructBasedVertexArrayV3<T> : MultiLevelStructBasedVertexArray<T, BucketArray2>
//    {
//        #region StructBasedVertexWindowV3
//        /// <summary>
//        /// The vertex window structure holds the search results from a scan.
//        /// </summary>
//        /// <typeparam name="T">The collection type.</typeparam>
//        [StructLayout(LayoutKind.Sequential)]
//        internal struct StructBasedVertexWindowV3<T> : IVertexWindow<T>
//        {
//            #region Declarations
//            HashTableStructBasedVertexArrayV3<T> mData;
//            int mHashID;
//            T mItem;
//            IEqualityComparer<T> mEqualityComparer;

//            /// <summary>
//            /// The current slot ID plus 1.
//            /// </summary>
//            private int CurrSlotIDPlus1;
//            /// <summary>
//            /// THe current vertex.
//            /// </summary>
//            private CollectionVertexStruct<T> Curr;
//            /// <summary>
//            /// The next vertex.
//            /// </summary>
//            private CollectionVertexStruct<T> Next;
//            #endregion // Declarations
//            #region Constructor
//            /// <summary>
//            /// This is the default constructor for the window.
//            /// </summary>
//            /// <param name="data">The data collection.</param>
//            /// <param name="indexID">The index of the position to set the window.</param>
//            public StructBasedVertexWindowV3(HashTableStructBasedVertexArrayV3<T> data,
//                IEqualityComparer<T> eqComparer, int indexID, int hashID, T item)
//            {
//                mData = data;
//                mHashID = hashID;
//                mItem = item;
//                mEqualityComparer = eqComparer;

//                mData.ItemLock(indexID);
//                CurrSlotIDPlus1 = indexID + 1;
//                Curr = mData[indexID];

//                if (!Curr.IsTerminator)
//                {
//                    mData.ItemLock(Curr.NextSlotIDPlus1 - 1);
//                    Next = mData[Curr.NextSlotIDPlus1 - 1];
//                }
//                else
//                    Next = new CollectionVertexStruct<T>();
//            }
//            #endregion // Constructor
//            #region Sentinel Constructor
//            /// <summary>
//            /// This is the default constructor for the window.
//            /// </summary>
//            /// <param name="data">The data collection.</param>
//            /// <param name="indexID">The index of the position to set the window.</param>
//            public StructBasedVertexWindowV3(HashTableStructBasedVertexArrayV3<T> data,
//                IEqualityComparer<T> eqComparer, int parentBucketID, int bucketID)
//            {
//                mData = data;
//                mItem = default(T);
//                mEqualityComparer = eqComparer;

//                //This is the target.
//                mHashID = BitReverse(bucketID);

//                mData.BucketLock(parentBucketID);
//                int indexID = ConvertBucketIDToIndexID(parentBucketID);
//                CurrSlotIDPlus1 = indexID + 1;
//                Curr = mData.BucketGet(parentBucketID);

//                if (!Curr.IsTerminator)
//                {
//                    mData.ItemLock(Curr.NextSlotIDPlus1 - 1);
//                    Next = mData[Curr.NextSlotIDPlus1 - 1];
//                }
//                else
//                    Next = new CollectionVertexStruct<T>();
//            }
//            #endregion // Constructor

//            #region HashID
//            /// <summary>
//            /// This is the hash ID of the item currently being searched.
//            /// </summary>
//            public int HashID
//            {
//                get { return mHashID; }
//            }
//            #endregion
//            #region Value
//            /// <summary>
//            /// This is the current value being handled by the window.
//            /// </summary>
//            public T Value
//            {
//                get { return mItem; }
//            }
//            #endregion

//            #region ItemSetNext()
//            /// <summary>
//            /// This method changes the value of the next item.
//            /// </summary>
//            /// <param name="value">The new value.</param>
//            public void ItemSetNext()
//            {
//                //This code is to accomodate dictionary type collections where the item is a keyvalue pair.
//                Next.Value = mItem;
//                mData[Curr.NextSlotIDPlus1 - 1] = Next;
//            }
//            #endregion // SetNextItem(T value)
//            #region ItemInsert(T value)
//            /// <summary>
//            /// 
//            /// </summary>
//            /// <param name="hashID">The hashID to search for and lock.</param>
//            public void ItemInsert()
//            {
//                int newSlot = mData.EmptyGet();

//                mData.ItemLock(newSlot);

//                if (!Curr.IsTerminator)
//                    mData.ItemUnlock(Curr.NextSlotIDPlus1 - 1);

//                Next = new CollectionVertexStruct<T>(mHashID, mItem, Curr.NextSlotIDPlus1);

//                Curr.NextSlotIDPlus1 = newSlot + 1;

//                mData[newSlot] = Next;
//                mData[CurrSlotIDPlus1 - 1] = Curr;

//                //Increment the necessary counters, and
//                //check whether we need to recalculate the bit size.
//                mData.SizeRecalculate(mData.CountIncrement());
//            }
//            #endregion

//            #region InsertSentinelAndUnlock(int bucketID)
//            /// <summary>
//            /// This method inserts a sentinel in to the data collection.
//            /// </summary>
//            /// <param name="bucketID">The new sentinel index id.</param>
//            public void InsertSentinelAndUnlock(int bucketID)
//            {
//                //Scan for the correct position to insert the sentinel.
//                ScanAndLock();

//                mData.BucketSetRaw(bucketID, Curr.NextSlotIDPlus1);
//                if (!Curr.IsTerminator) mData.ItemUnlock(Curr.NextSlotIDPlus1 - 1);
//                mData.BucketUnlock(bucketID);

//                Curr.NextSlotIDPlus1 = ConvertBucketIDToIndexID(bucketID) + 1;
//                mData[CurrSlotIDPlus1 - 1] = Curr;
//                mData.ItemUnlock(CurrSlotIDPlus1 - 1);
//            }
//            #endregion

//            #region Unlock()
//            /// <summary>
//            /// This method provides common functionality to unlock a VertexWindow.
//            /// </summary>
//            public void Unlock()
//            {
//                if (Curr.NextSlotIDPlus1 != 0) mData.ItemUnlock(Curr.NextSlotIDPlus1 - 1);
//                if (CurrSlotIDPlus1 != 0) mData.ItemUnlock(CurrSlotIDPlus1 - 1);
//            }
//            #endregion
//            #region ScanAndLock()
//            /// <summary>
//            /// This method scans through the slot data until is reaches the end of the data, or the position 
//            /// where the hashID meets a slot with a hashID that is greater than itself.
//            /// </summary>
//            public int ScanAndLock()
//            {
//                //If the current is the last item in the linked list then exit.
//                if (Curr.IsTerminator)
//                    return 0;

//                int hopCount = 0;

//                while (Next.HashID < mHashID)
//                {
//                    hopCount++;

//                    //Unlock the old current item.
//                    mData.ItemUnlock(CurrSlotIDPlus1 - 1);

//                    CurrSlotIDPlus1 = Curr.NextSlotIDPlus1;

//                    //If this is the last item in the list then move up and exit.
//                    if (Next.IsTerminator)
//                    {
//                        Curr = Next;
//                        Next = new CollectionVertexStruct<T>();
//                        break;
//                    }

//                    //OK, lock the next item and move up.
//                    mData.ItemLock(Next.NextSlotIDPlus1 - 1);
//                    Curr = Next;
//                    Next = mData[Curr.NextSlotIDPlus1 - 1];
//                }

//                return hopCount;
//            }
//            #endregion
//            #region MoveUp()
//            /// <summary>
//            /// This method moves up the Next vertex to the current position.
//            /// </summary>
//            public bool MoveUp()
//            {
//                if (Curr.IsTerminator)
//                    return false;

//                mData.ItemUnlock(CurrSlotIDPlus1 - 1);
//                CurrSlotIDPlus1 = Curr.NextSlotIDPlus1;
//                Curr = Next;

//                if (!Curr.IsTerminator)
//                {
//                    mData.ItemLock(Curr.NextSlotIDPlus1 - 1);
//                    Next = mData[Curr.NextSlotIDPlus1 - 1];
//                }
//                else
//                    Next = new CollectionVertexStruct<T>();

//                return true;
//            }
//            #endregion

//            #region RemoveItemAndUnlock()
//            /// <summary>
//            /// 
//            /// </summary>
//            /// <returns></returns>
//            public void ItemRemoveAndUnlock()
//            {
//                int removedItem = Curr.NextSlotIDPlus1 - 1;

//                Curr.NextSlotIDPlus1 = Next.NextSlotIDPlus1;

//                mData[CurrSlotIDPlus1 - 1] = Curr;
//                mData.ItemUnlock(removedItem);
//                mData.ItemUnlock(CurrSlotIDPlus1 - 1);

//                //Add the empty item for re-allocation.
//                mData.EmptyAdd(removedItem);

//                //Update the version and reduce the item count.
//                mData.CountDecrement();
//            }
//            #endregion // SlotsRemoveItem

//            #region Snip()
//            /// <summary>
//            /// 
//            /// </summary>
//            public void Snip()
//            {
//                int removedItem = Curr.NextSlotIDPlus1 - 1;

//                Curr.NextSlotIDPlus1 = Next.NextSlotIDPlus1;

//                mData[CurrSlotIDPlus1 - 1] = Curr;
//                mData.ItemUnlock(removedItem);

//                if (!Curr.IsTerminator)
//                {
//                    mData.ItemLock(Curr.NextSlotIDPlus1 - 1);
//                    Next = mData[Curr.NextSlotIDPlus1 - 1];
//                }
//                else
//                    Next = new CollectionVertexStruct<T>();

//                //Add the empty item for re-allocation.
//                mData.EmptyAdd(removedItem);

//                mData.CountDecrement();
//            }
//            #endregion // SlotsRemoveItem

//            #region ScanProcess()
//            public bool ScanProcess()
//            {
//                return !Curr.IsTerminator && Next.HashID == mHashID;
//            }
//            #endregion
//            #region ScanItemMatch
//            /// <summary>
//            /// This property specifies whether the next item is a match for the data.
//            /// </summary>
//            public bool ScanItemMatch
//            {
//                get
//                {
//                    return !Next.IsSentinel && mEqualityComparer.Equals(mItem, Next.Data);
//                }
//            }
//            #endregion // ScanItemMatch

//            #region NextIsSentinel
//            public bool NextIsSentinel
//            {
//                get { return Next.IsSentinel; }
//            }
//            #endregion // NextIsSentinel
//            #region CurrIsTerminator
//            public bool CurrIsTerminator
//            {
//                get { return Curr.IsTerminator; }
//            }
//            #endregion // CurrIsTerminator
//            #region NextData

//            public T NextData
//            {
//                get { return Next.Data; }
//            }

//            #endregion

//            #region ToString()
//            /// <summary>
//            /// This override provides a debug friendly representation of the structure.
//            /// </summary>
//            /// <returns>Returns the structure value.</returns>
//            public override string ToString()
//            {
//                return string.Format("{0}[{1}] -> {2}[{3}]", CurrSlotIDPlus1 - 1, Curr, Curr.NextSlotIDPlus1 - 1, Next);
//            }
//            #endregion // ToString()
//        }
//        #endregion
//        #region Declarations
//#if (DEBUG)
//        [ThreadStatic]
//        static int sLockIndex;

//        static int[] sLockThreadID = new int[30];
//        static int[] sLockArray = new int[30];
//        static int sLockCounter = 0;

//        private static int ThreadLockIndex
//        {
//            get
//            {
//                if (sLockIndex == 0)
//                {
//                    sLockIndex = Interlocked.Increment(ref sLockCounter);
//                    sLockThreadID[sLockIndex - 1] = Thread.CurrentThread.ManagedThreadId;
//                }
//                return sLockIndex - 1;
//            }
//        }
//#endif
//        private object syncBucketExpand = new object();
//        #endregion // Declarations
//        #region Constructor
//        /// <summary>
//        /// This is the default constructor for the array.
//        /// </summary>
//        /// <param name="isFixedSize">A boolean value indicating whether the data collection is fixed size.</param>
//        /// <param name="capacity">The array capacity.</param>
//        public HashTableStructBasedVertexArrayV3()
//        {
//        }
//        #endregion // Constructor
//        #region ConvertBucketIDToIndexID(int bucketID)
//        /// <summary>
//        /// This method converts a bucketID to an index ID by setting the MSB.
//        /// </summary>
//        /// <param name="bucketID">The bucket ID to convert.</param>
//        /// <returns>The index ID.</returns>
//        public static int ConvertBucketIDToIndexID(int bucketID)
//        {
//            return unchecked((int)(bucketID | 0x80000000));
//        }
//        #endregion // ConvertBucketIDToIndexID(int bucketID)
//        #region InitializeBucketArray()
//        /// <summary>
//        /// This method initializes the bucket array.
//        /// </summary>
//        protected override void InitializeBucketArray()
//        {
//            mBuckets = new BucketArray2[LevelMax];
//        }
//        #endregion // InitializeDataArray()
//        #region RootIndexID
//        /// <summary>
//        /// This is the index ID of the the first item.
//        /// </summary>
//        protected override int RootIndexID { get { return unchecked((int)0x80000000); } }
//        #endregion // RootIndexID

//        #region ItemIsLocked(int index)
//        /// <summary>
//        /// This method checks whether an item in the collection is locked.
//        /// </summary>
//        /// <param name="index">The index of the item to check.</param>
//        /// <returns>Returns true if the item is locked.</returns>
//        public override bool ItemIsLocked(int index)
//        {
//            int noMaskIndex = index & 0x7FFFFFFF;
//            if (noMaskIndex == index)
//                return mSlots.ItemIsLocked(index);

//            int level, levelPosition;
//            BucketCalculatePosition(noMaskIndex, out level, out levelPosition);
//            return mBuckets[level].ItemIsLocked(levelPosition);
//        }
//        #endregion // ItemIsLocked(int index)
//        #region ItemLock(int index)
//        /// <summary>
//        /// This method locks the specific item.
//        /// </summary>
//        /// <param name="index">The item index.</param>
//        /// <returns>Returns the number of lock cycles the thread entered.</returns>
//        public override int ItemLock(int index)
//        {
//            int noMaskIndex = index & 0x7FFFFFFF;
//            if (noMaskIndex == index)
//                return mSlots.ItemLock(index);

//            return BucketLock(noMaskIndex);
//        }
//        #endregion // ItemLock(int index)
//        #region ItemLockWait(int index)
//        /// <summary>
//        /// This method waits for a locked item to become available.
//        /// </summary>
//        /// <param name="index">The index of the item to wait for.</param>
//        /// <returns>Returns the number of lock cycles during the wait.</returns>
//        public override int ItemLockWait(int index)
//        {
//            int noMaskIndex = index & 0x7FFFFFFF;
//            if (noMaskIndex == index)
//                return mSlots.ItemLockWait(index);

//            int level, levelPosition;
//            BucketCalculatePosition(noMaskIndex, out level, out levelPosition);
//            return mBuckets[level].ItemLockWait(levelPosition);
//        }
//        #endregion // ItemLockWait(int index)
//        #region ItemTryLock(int index)
//        /// <summary>
//        /// This method attempts to lock the item specified.
//        /// </summary>
//        /// <param name="index">The index of the item you wish to lock..</param>
//        /// <returns>Returns true if the item was successfully locked.</returns>
//        public override bool ItemTryLock(int index)
//        {
//            int noMaskIndex = index & 0x7FFFFFFF;
//            if (noMaskIndex == index)
//                return mSlots.ItemTryLock(index);

//            return BucketTryLock(noMaskIndex);
//        }
//        #endregion // ItemTryLock(int index)
//        #region ItemUnlock(int index)
//        /// <summary>
//        /// The method unlocks the item.
//        /// </summary>
//        /// <param name="index">The index of the item you wish to unlock.</param>
//        public override void ItemUnlock(int index)
//        {
//            int noMaskIndex = index & 0x7FFFFFFF;
//            if (noMaskIndex == index)
//            {
//                mSlots.ItemUnlock(index);
//                return;
//            }

//            BucketUnlock(noMaskIndex);
//        }
//        #endregion

//        #region this[int index]
//        /// <summary>
//        /// This is the indexer for the array.
//        /// </summary>
//        /// <param name="index">The index position.</param>
//        /// <returns>Returns the vertex corresponding to the index position.</returns>
//        public override CollectionVertexStruct<T> this[int index]
//        {
//            get
//            {
//                int noMaskIndex = index & 0x7FFFFFFF;
//                if (noMaskIndex == index)
//                    return mSlots[index];

//                return BucketGet(noMaskIndex);
//            }
//            set
//            {
//                int noMaskIndex = index & 0x7FFFFFFF;
//                if (noMaskIndex == index)
//                {
//                    mSlots[index] = value;
//                    return;
//                }

//                BucketSet(noMaskIndex, value);
//            }
//        }
//        #endregion // this[int index]

//        #region BucketGet(int index)
//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="index"></param>
//        /// <returns>Returns the bucket.</returns>
//        internal CollectionVertexStruct<T> BucketGet(int index)
//        {
//            return CollectionVertexStruct<T>.Sentinel(BitReverse(index), BucketGetRaw(index));
//        }
//        #endregion // BucketGet(int index)
//        #region BucketGetRaw(int index)
//        internal int BucketGetRaw(int index)
//        {
//            int level, levelPosition;
//            BucketCalculatePosition(index, out level, out levelPosition);
//            return mBuckets[level][levelPosition];
//        }
//        #endregion // BucketGetRaw(int index)
//        #region BucketSet(int index, CollectionVertexStruct<T> value)
//        internal void BucketSet(int index, CollectionVertexStruct<T> value)
//        {
//            BucketSetRaw(index, value.NextSlotIDPlus1);
//        }
//        #endregion // BucketSet(int index, CollectionVertexStruct<T> value)
//        #region BucketSetRaw(int index, int value)
//        internal void BucketSetRaw(int index, int value)
//        {
//            int level, levelPosition;
//            BucketCalculatePosition(index, out level, out levelPosition);

//            mBuckets[level][levelPosition] = value;
//        }
//        #endregion // BucketSetRaw(int index, int value)

//        #region BucketLock(int index)
//        /// <summary>
//        /// This method locks a bucket.
//        /// </summary>
//        /// <param name="index">The index of the bucket to lock.</param>
//        /// <returns>Returns the number of lock loops before the call was able to lock and get access.</returns>
//        internal int BucketLock(int index)
//        {
//            int level, levelPosition;
//            BucketCalculatePosition(index, out level, out levelPosition);
//#if (DEBUG)
//            int val = mBuckets[level].ItemLock(levelPosition);
//            sLockArray[ThreadLockIndex] = index + 1;
//            return val;
//#else
//            return mBuckets[level].ItemLock(levelPosition);
//#endif
//        }
//        #endregion // BucketLock(int index)
//        #region BucketTryLock(int index)
//        /// <summary>
//        /// This method locks a bucket.
//        /// </summary>
//        /// <param name="index">The index of the bucket to lock.</param>
//        /// <returns>Returns the number of lock loops before the call was able to lock and get access.</returns>
//        internal bool BucketTryLock(int index)
//        {
//            int level, levelPosition;
//            BucketCalculatePosition(index, out level, out levelPosition);
//#if (DEBUG)
//            bool result = mBuckets[level].ItemTryLock(levelPosition);

//            if (result)
//            {
//                sLockArray[ThreadLockIndex] = index + 1;
//            }

//            return result;
//#else
//            return mBuckets[level].ItemTryLock(levelPosition);
//#endif
//        }
//        #endregion // BucketLock(int index)
//        #region BucketUnlock(int index)
//        /// <summary>
//        /// This method unlocks the bucket.
//        /// </summary>
//        /// <param name="index">The index of the bucket to unlock.</param>
//        internal void BucketUnlock(int index)
//        {
//            int level, levelPosition;
//            BucketCalculatePosition(index, out level, out levelPosition);
//            mBuckets[level].ItemUnlock(levelPosition);
//#if (DEBUG)
//            sLockArray[ThreadLockIndex] = 0;
//#endif
//        }
//        #endregion // BucketUnlock(int index)

//        #region VertexWindowGet(int index)
//        /// <summary>
//        /// This method returns a vertex window for the index specified.
//        /// </summary>
//        /// <returns>Returns a vertex window.</returns>
//        public override IVertexWindow<T> VertexWindowGet()
//        {
//            return new StructBasedVertexWindowV3<T>(this, mEqComparer, RootIndexID, 0, default(T));
//        }
//        #endregion // VertexWindowGet(int index)
//        #region VertexWindowGet(int hashCode, bool createSentinel)
//        /// <summary>
//        /// This method returns a vertex window for the hashCode specified.
//        /// </summary>
//        /// <param name="hashCode">The hashcode.</param>
//        /// <param name="createSentinel">A boolean value that specifies whether the sentinels should be created.</param>
//        /// <returns>Returns a vertex window.</returns>
//        public override IVertexWindow<T> VertexWindowGet(T item, bool createSentinel)
//        {
//            int hashCode = mEqComparer.GetHashCode(item);
//            int hashID;
//            int sentIndexID = GetSentinelID(mEqComparer.GetHashCode(item), createSentinel, out hashID);

//            //Ok, set the MSB to indicate the value is a sentinel.
//            return new StructBasedVertexWindowV3<T>(this, mEqComparer, sentIndexID, hashID, item);
//        }
//        #endregion // VertexWindowGet(int hashCode, bool createSentinel)

//        #region BucketCalculatePosition(int indexID, out int level, out int levelPosition)
//        /// <summary>
//        /// This method calculates the specific bucket level and the position within that bucket.
//        /// </summary>
//        /// <param name="indexID">The bucket index.</param>
//        /// <param name="level">The bucket level.</param>
//        /// <param name="levelPosition">The bucket position.</param>
//        private void BucketCalculatePosition(int indexID, out int level, out int levelPosition)
//        {
//            if (indexID <= 0)
//            {
//                level = indexID;
//                levelPosition = 0;
//                return;
//            }

//            int levelCurrent = LevelCurrent;
//            int mask = (int.MaxValue >> (31 - levelCurrent));

//            if (indexID > (indexID & mask))
//                throw new ArgumentOutOfRangeException("The bucket level has not been initialized.");

//            mask = (mask + 1) >> 1;

//            for (level = levelCurrent; level > 0; level--)
//            {
//                if ((indexID & mask) > 0)
//                    break;
//                mask >>= 1;
//            }

//            if (level == 0)
//                levelPosition = indexID;
//            else
//                levelPosition = indexID & (mask - 1);
//        }
//        #endregion

//        #region BucketLevelExpand(int currentLevel, int newLevel)
//        /// <summary>
//        /// This method expands the bucket arrays.
//        /// </summary>
//        /// <param name="currentLevel">The current level.</param>
//        /// <param name="newLevel">The new level required.</param>
//        protected override void BucketLevelExpand(int currentLevel, int newLevel)
//        {
//            lock (syncBucketExpand)
//            {
//                if (currentLevel != LevelCurrent)
//                    return;

//                for (int level = currentLevel; level <= newLevel; level++)
//                {
//                    mBuckets[level] = new BucketArray2(BucketLevelCapacityCalculate(level));
//                }

//                base.BucketLevelExpand(currentLevel, newLevel);
//            }
//        }
//        #endregion // BucketLevelExpand(int currentLevel, int newLevel)
//        #region BucketLevelCapacityCalculate(int level)
//        /// <summary>
//        /// This method calculates the size of the bucket array.
//        /// </summary>
//        /// <param name="level">The level.</param>
//        /// <returns>Returns 2n+1 as the size of the array where n is the level.</returns>
//        private int BucketLevelCapacityCalculate(int level)
//        {
//            if (level == 0)
//                return 1;

//            return (1 << level - 1);
//        }
//        #endregion // BucketLevelSize(int level)

//        #region GetEnumerator()
//        /// <summary>
//        /// This method returns an enumeration through the sentinels and data in the collection.
//        /// </summary>
//        /// <returns>Returns an enumeration containing the collection data.</returns>
//        public override IEnumerator<KeyValuePair<int, ICollectionVertex<T>>> GetEnumerator()
//        {
//            CollectionVertexStruct<T> item = this[RootIndexID];
//            yield return new KeyValuePair<int, ICollectionVertex<T>>(unchecked((int)0x80000000), item); ;

//            while (!item.IsTerminator)
//            {
//                int id = item.NextSlotIDPlus1 - 1;
//                ItemLockWait(id);
//                item = this[id];

//                yield return new KeyValuePair<int, ICollectionVertex<T>>(id, item); ;
//            }
//        }
//        #endregion

//        #region GetSentinelID(int hashCode, bool createSentinel, out int sentIndexID, out int hashID)
//        /// <summary>
//        /// This method returns the sentinel ID and the hashID for the hashcode passed.
//        /// </summary>
//        /// <param name="hashCode">The hashcode to search for the sentinel position.</param>
//        /// <param name="createSentinel">This property determine whether any missing sentinels will be created.</param>
//        /// <param name="hashID">The hashID for the hashCode that passed.</param>
//        /// <returns>The largest sentinel index ID.</returns>
//        protected override int GetSentinelID(int hashCode, bool createSentinel, out int hashID)
//        {
//            hashCode &= cnLowerBitMask;
//            hashID = BitReverse(hashCode);

//            int bitsStart = LevelCurrent;
//            int bitsCurrent = bitsStart;
//            int bucketIDParent = -1;

//            int bucketID = hashCode & (int.MaxValue >> (31 - bitsCurrent));

//            while (bitsCurrent >= 0)
//            {
//                if (bucketID != bucketIDParent)
//                {
//                    bucketIDParent = bucketID;

//                    //OK, check until we have reached a bucket with data, or we have reached the base bucket.
//                    if (bucketID == 0 || BucketGetRaw(bucketID) != 0)
//                        break;
//                }

//                bitsCurrent--;

//                bucketID = hashCode & (int.MaxValue >> (31 - bitsCurrent));
//            }

//            //OK, if we can create the missing sentinels then do so.
//            if (createSentinel)
//            {
//                //OK, loop and create any required sentinels.
//                while (bitsCurrent < bitsStart)
//                {
//                    bitsCurrent++;

//                    bucketID = hashCode & (int.MaxValue >> (31 - bitsCurrent));

//                    if (bucketID == bucketIDParent)
//                        continue;

//                    BucketLock(bucketID);
//                    //Check whether the bucket has been created, shouldn't happen but best to be safe.
//                    if (BucketGetRaw(bucketID) != 0)
//                    {
//                        BucketUnlock(bucketID);
//                        continue;
//                    }

//                    //Ok, insert the sentinel.
//                    StructBasedVertexWindowV3<T> vWin = new StructBasedVertexWindowV3<T>(
//                        this, mEqComparer, bucketIDParent, bucketID);

//                    //Insert the new sentinel.
//                    vWin.InsertSentinelAndUnlock(bucketID);

//                    if (bucketID == hashCode)
//                        break;

//                    //OK set the parent bucket and move to the next sentinel.
//                    bucketIDParent = bucketID;
//                }
//            }

//            //Ok, set the MSB to indicate the value is a sentinel.
//            return ConvertBucketIDToIndexID(bucketID);
//        }
//        #endregion // GetSentinel(int hashCode, bool create, out int sentSlotID, out int hashID)

//        #region FastContains
//        /// <summary>
//        /// This class supports a fast search algorithm
//        /// </summary>
//        public override bool SupportsFastContain { get { return true; } }
//        /// <summary>
//        /// This method implements a fast search algoritm.
//        /// </summary>
//        /// <param name="item">The item to search for.</param>
//        /// <returns>Returns true if found, false if not found, and null if the search encountered modified data.</returns>
//        public override bool? FastContains(T item)
//        {
//            //Is this a null or default value?
//            if (mEqComparer.Equals(item, default(T)))
//                return DefaultTContains();

//            int currVersion = mVersion;
//            int hashID;
//            int sentIndexID = GetSentinelID(mEqComparer.GetHashCode(item), false, out hashID);

//            //Get the initial sentinel vertex. No need to check locks as sentinels rarely change.
//            int scanPosition = sentIndexID;
//            CollectionVertexStruct<T> scanVertex = this[scanPosition];

//            //First we will attempt to search without locking. However, should the version ID change 
//            //during the search we will need to complete a locked search to ensure consistency.
//            while (mVersion == currVersion)
//            {
//                //Do we have a match?
//                if (!scanVertex.IsSentinel &&
//                    scanVertex.HashID == hashID &&
//                    mEqComparer.Equals(item, scanVertex.Value))
//                    return true;

//                //Is this the end of the line
//                if (scanVertex.IsTerminator || scanVertex.HashID > hashID)
//                    return false;

//                scanPosition = scanVertex.NextSlotIDPlus1 - 1;

//                scanVertex = this[scanPosition];
//            }

//            ContainScanUnlockedMiss();
//            return null;
//        }

//        public override bool? FastContains(IEqualityComparer<T> eqComparer, T item, out T value)
//        {
//            //Is this a null or default value?
//            if (!eqComparer.Equals(item, default(T)))
//            {
//                int currVersion = Version;
//                int hashID;
//                int sentIndexID = GetSentinelID(eqComparer.GetHashCode(item), false, out hashID);

//                //Get the initial sentinel vertex. No need to check locks as sentinels rarely change.
//                int scanPosition = sentIndexID;
//                CollectionVertexStruct<T> scanVertex = this[scanPosition];

//                //First we will attempt to search without locking. However, should the version ID change 
//                //during the search we will need to complete a locked search to ensure consistency.
//                while (VersionCompare(currVersion))
//                {
//                    //Do we have a match?
//                    if (!scanVertex.IsSentinel &&
//                        scanVertex.HashID == hashID &&
//                        eqComparer.Equals(item, scanVertex.Value))
//                    {
//                        value = scanVertex.Value;
//                        return true;
//                    }

//                    //Is this the end of the line
//                    if (scanVertex.IsTerminator || scanVertex.HashID > hashID)
//                    {
//                        value = default(T);
//                        return false;
//                    }

//                    scanPosition = scanVertex.NextSlotIDPlus1 - 1;

//                    scanVertex = this[scanPosition];
//                }
//            }

//            ContainScanUnlockedMiss();
//            value = default(T);
//            return null;
//        }

//        #endregion // FastContain

//#if (DEBUG)
//        #region DebugEmpty
//        /// <summary>
//        /// This is the debug data.
//        /// </summary>
//        public virtual string DebugEmpty
//        {
//            get
//            {
//                StringBuilder sb = new StringBuilder();

//                int count = 0;

//                if (!mEmptyVertex.Value.IsTerminator)
//                {
//                    count++;
//                    int index = mEmptyVertex.Value.NextSlotIDPlus1 - 1;
//                    CollectionVertexStruct<T> item = mSlots[index];

//                    while (!item.IsTerminator)
//                    {
//                        index = item.NextSlotIDPlus1 - 1;

//                        if ((index & 0x80000000) > 0)
//                        {
//                            sb.AppendFormat("Error: {0:X}", index);
//                            break;
//                        }

//                        item = mSlots[index];
//                        count++;
//                    }
//                }

//                sb.AppendFormat("{0} empty slots", count);
//                sb.AppendLine();
//                return sb.ToString();
//            }
//        }
//        #endregion // DebugEmpty
//#endif
//    }
}
