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
    /// This class contains the combined buckets and slots in a single class.
    /// </summary>
    /// <typeparam name="T">The collection type.</typeparam>
    public class CombinedVertexArray<T> : VertexArray<T>
    {
        #region Constants
        private const double elog2 = 0.693147181;
        /// <summary>
        /// This is the root sentinel vertex.
        /// </summary>
        private const int Root = unchecked((int)0x80000000);

        #endregion // Declarations
        #region Declarations
        /// <summary>
        /// This collection holds the data.
        /// </summary>
        private IFineGrainedLockArray<CollectionVertex<T>> mBuckets;
        /// <summary>
        /// The current number of bits being used by the collection.
        /// </summary>
        private volatile int mCurrentBits;
        /// <summary>
        /// This is the slot recalculate threshold.
        /// </summary>
        private volatile int mRecalculateThreshold;
        #endregion // Declarations
        #region Constructor
        /// <summary>
        /// This is the default constructor for the array.
        /// </summary>
        /// <param name="isFixedSize">A boolean value indicating whether the data collection is fixed size.</param>
        /// <param name="capacity">The array capacity.</param>
        public CombinedVertexArray(bool isFixedSize, int capacity)
        {
            Initialize(isFixedSize, capacity);
        }
        #endregion // Constructor

        #region Initialize(bool isFixedSize, int capacity)
        /// <summary>
        /// This method initializes the class.
        /// </summary>
        /// <param name="isFixedSize">A boolean value indicating whether the data collection is fixed size.</param>
        /// <param name="capacity">The capacity of the data array.</param>
        protected override void Initialize(bool isFixedSize, int capacity)
        {
            base.Initialize(isFixedSize, capacity);

            int maxBits = (int)(Math.Log(capacity) / elog2);
            int bucketSize = 1 << maxBits;

            mCurrentBits = maxBits;

            if (isFixedSize)
                mBuckets = new FineGrainedLockArray<CollectionVertex<T>>(bucketSize, 0);
            else
                mBuckets = new ExpandableFineGrainedLockArray<CollectionVertex<T>>(bucketSize, BucketExpander);

            mBuckets[0] = CollectionVertex<T>.Sentinel(0, 0);
        }
        #endregion // Initialize(bool isFixedSize, int capacity)

        #region BucketExpander(int requiredSize, int currentSize)
        /// <summary>
        /// 
        /// </summary>
        /// <param name="requiredSize">The index specifying the new capacity.</param>
        /// <param name="currentSize">The current capacity.</param>
        /// <returns>Returns the new capacity.</returns>
        private int BucketExpander(int requiredSize, int currentSize)
        {
            return requiredSize;
        }
        #endregion // BucketExpander(int requiredSize, int currentSize)
        #region EmptyAdd(int index)
        /// <summary>
        /// This method adds an empty item to the free list.
        /// </summary>
        /// <param name="index">The index of the item to add to the sentinel.</param>
        public override void EmptyAdd(int index)
        {
            //Check whether they are attempting to return a sentinel.
            int noMaskIndex = index & 0x7FFFFFFF;
            if (noMaskIndex != index)
                throw new ArgumentOutOfRangeException("Sentinel vertexes cannot be returned.");

            base.EmptyAdd(index);
        }
        #endregion // EmptyAdd(int index)

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
                return mSlots.ItemIsLocked(index);

            return mBuckets.ItemIsLocked(noMaskIndex);
        }
        #endregion // ItemIsLocked(int index)
        #region ItemLock(int index)
        /// <summary>
        /// This method locks the specific item.
        /// </summary>
        /// <param name="index">The item index.</param>
        /// <returns>Returns the number of lock cycles the thread entered.</returns>
        public override int ItemLock(int index)
        {
            int noMaskIndex = index & 0x7FFFFFFF;
            if (noMaskIndex == index)
                return mSlots.ItemLock(index);

            return mBuckets.ItemLock(noMaskIndex);
        }
        #endregion // ItemLock(int index)
        #region ItemLockWait(int index)
        /// <summary>
        /// This method waits for a locked item to become available.
        /// </summary>
        /// <param name="index">The index of the item to wait for.</param>
        /// <returns>Returns the number of lock cycles during the wait.</returns>
        public override int ItemLockWait(int index)
        {
            int noMaskIndex = index & 0x7FFFFFFF;
            if (noMaskIndex == index)
                return mSlots.ItemLockWait(index);

            return mBuckets.ItemLockWait(noMaskIndex);
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
                return mSlots.ItemTryLock(index);

            return mBuckets.ItemTryLock(noMaskIndex);
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
                mSlots.ItemUnlock(index);
            else
                mBuckets.ItemUnlock(noMaskIndex);
        }
        #endregion // ItemUnlock(int index)

        #region this[int index]
        /// <summary>
        /// This is the indexer for the array.
        /// </summary>
        /// <param name="index">The index position.</param>
        /// <returns>Returns the vertex corresponding to the index position.</returns>
        public override CollectionVertex<T> this[int index]
        {
            get
            {
                int noMaskIndex = index & 0x7FFFFFFF;
                if (noMaskIndex == index)
                    return mSlots[index];

                return mBuckets[noMaskIndex];
            }
            set
            {
                int noMaskIndex = index & 0x7FFFFFFF;
                if (noMaskIndex == index)
                    mSlots[index] = value;
                else
                    mBuckets[noMaskIndex] = value;
            }
        }
        #endregion // this[int index]

        #region IEnumerable<KeyValuePair<int,Vertex<T>>> Members
        /// <summary>
        /// This method returns an enumeration through the sentinels and data in the collection.
        /// </summary>
        /// <returns>Returns an enumeration containing the collection data.</returns>
        public override IEnumerator<KeyValuePair<int, ICollectionVertex<T>>> GetEnumerator()
        {
            KeyValuePair<int, ICollectionVertex<T>> item = new KeyValuePair<int, ICollectionVertex<T>>(unchecked((int)0x80000000), mBuckets[0]);
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

        #region LengthBuckets
        /// <summary>
        /// The bucket capacity.
        /// </summary>
        public int LengthBuckets
        {
            get { return mBuckets.Length; }
        }
        #endregion // LengthBuckets

        #region SizeRecalculate(int total)
        /// <summary>
        /// This method calculates the current number of bits needed to support the current data.
        /// </summary>
        public override void SizeRecalculate(int total)
        {
            int currentBits = mCurrentBits;
            int recalculateThreshold = mRecalculateThreshold;

            if (total < recalculateThreshold)
                return;

            int newBits = (int)(Math.Log(total) / elog2);
            int newThreshold = 2 << newBits;

            if (newBits > currentBits)
                Interlocked.CompareExchange(ref mCurrentBits, newBits, currentBits);

            if (newThreshold > recalculateThreshold)
                Interlocked.CompareExchange(ref mRecalculateThreshold, newThreshold, recalculateThreshold);
        }
        #endregion
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

        #region GetSentinelID(int hashCode, bool create, out int sentSlotID, out int hashID)
        /// <summary>
        /// This method returns the sentinel ID and the hashID for the hashcode passed.
        /// </summary>
        /// <param name="hashCode">The hashcode to search for the sentinel position.</param>
        /// <param name="create">This property determine whether any missing sentinels will be created.</param>
        /// <param name="sentIndexID">The largest sentinel index ID.</param>
        /// <param name="hashID">The hashID for the hashCode that passed.</param>
        public override void GetSentinelID(int hashCode, bool create, out int sentIndexID, out int hashID)
        {
            hashCode &= cnLowerBitMask;
            hashID = BitReverse(hashCode);

            int bitsStart = mCurrentBits;
            int bitsCurrent = bitsStart;
            int bucketIDParent = -1;

            int bucketID = hashCode & (int.MaxValue >> (31 - bitsCurrent));

            while (bitsCurrent >= 0)
            {
                if (bucketID != bucketIDParent)
                {
                    bucketIDParent = bucketID;

                    //OK, check until we have reached a bucket with data, or we have reached the base bucket.
                    if (bucketID == 0 || mBuckets[bucketID].IsSentinel)
                        break;
                }

                bitsCurrent--;

                bucketID = hashCode & (int.MaxValue >> (31 - bitsCurrent));
            }

            //OK, if we can create the missing sentinels then do so.
            if (create)
            {
                //OK, loop and create any required sentinels.
                while (bitsCurrent < bitsStart)
                {
                    bitsCurrent++;

                    bucketID = hashCode & (int.MaxValue >> (31 - bitsCurrent));

                    if (bucketID == bucketIDParent)
                        continue;

                    mBuckets.ItemLock(bucketID);
                    //Check whether the bucket has been created, shouldn't happen but best to be safe.
                    if (mBuckets[bucketID].HashID > 0)
                    {
                        mBuckets.ItemUnlock(bucketID);
                        continue;
                    }

                    //Ok, get the hash ID for the bucket.
                    int bucketHashID = BitReverse(bucketID);

                    //Ok, insert the sentinel.
                    VertexWindow<T> vWin = VertexWindowGet(ConvertBucketIDToIndexID(bucketIDParent));

                    //Scan for the correct position to insert the sentinel.
                    vWin.ScanAndLock(bucketHashID);

                    //Insert the new sentinel.
                    vWin.InsertSentinel(ConvertBucketIDToIndexID(bucketID), bucketHashID);

                    //Unlock the window data.
                    vWin.Unlock();

                    //OK set the parent bucket and move to the next sentinel.
                    bucketIDParent = bucketID;
                } 
            }

            //Ok, set the MSB to indicate the value is a sentinel.
            sentIndexID = ConvertBucketIDToIndexID(bucketID);
        }
        #endregion // GetSentinel(int hashCode, bool create, out int sentSlotID, out int hashID)

        #region VertexWindowGet(int index)
        /// <summary>
        /// This method returns a vertex window for the index specified.
        /// </summary>
        public override VertexWindow<T> VertexWindowGet()
        {
            return new VertexWindow<T>(this, Root);
        }
        #endregion // VertexWindowGet(int index)
    }
}
