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
//    /// <summary>
//    /// This class contains the combined buckets and slots in a single class.
//    /// </summary>
//    /// <typeparam name="T">The collection type.</typeparam>
//    public class HashTableStructBasedVertexArray<T> : StructBasedVertexArray<T>
//    {
//        #region Declarations
//        /// <summary>
//        /// This collection holds the data.
//        /// </summary>
//        private ILockableMarkableArray<CollectionVertexStruct<T>> mBuckets;
//        /// <summary>
//        /// The current number of bits being used by the collection.
//        /// </summary>
//        private volatile int mCurrentBits;
//        /// <summary>
//        /// This is the slot recalculate threshold.
//        /// </summary>
//        private volatile int mRecalculateThreshold;
//        #endregion // Declarations
//        #region Constructor
//        /// <summary>
//        /// This is the default constructor for the array.
//        /// </summary>
//        /// <param name="isFixedSize">A boolean value indicating whether the data collection is fixed size.</param>
//        /// <param name="capacity">The array capacity.</param>
//        public HashTableStructBasedVertexArray()
//        {
//        }
//        #endregion // Constructor

//        #region Initialize(IEqualityComparer<T> eqComparer, bool isFixedSize, int capacity, bool allowNullValues, bool allowMultipleEntries)
//        /// <summary>
//        /// This method initializes the data collection.
//        /// </summary>
//        /// <param name="eqComparer">This is the equality comparer for the array.</param>
//        /// <param name="isFixedSize">Specifies whether the collection is a fixed size.</param>
//        /// <param name="capacity">The initial capacity.</param>
//        /// <param name="allowNullValues">This boolean values specifies whether null values are allowed in the collection.</param>
//        /// <param name="allowMultipleEntries">This boolean value specicifies whether the collection allows items to exist 
//        /// more than once in the collection.</param>
//        public override void Initialize(IEqualityComparer<T> eqComparer, 
//            bool isFixedSize, int capacity, bool allowNullValues, bool allowMultipleEntries)
//        {
//            base.Initialize(eqComparer, isFixedSize, capacity, allowNullValues, allowMultipleEntries);

//            int maxBits = (int)(Math.Log(capacity) / elog2);
//            int bucketSize = 1 << maxBits;

//            mCurrentBits = maxBits;

//            if (isFixedSize)
//                mBuckets = new FineGrainedLockArray<CollectionVertexStruct<T>>(bucketSize);
//            else
//                mBuckets = new ExpandableFineGrainedLockArray<CollectionVertexStruct<T>>(bucketSize, BucketExpander);

//            mBuckets[0] = CollectionVertexStruct<T>.Sentinel(0, 0);
//        }
//        #endregion // Initialize(bool isFixedSize, int capacity)

//        #region BucketExpander(int requiredSize, int currentSize)
//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="requiredSize">The index specifying the new capacity.</param>
//        /// <param name="currentSize">The current capacity.</param>
//        /// <returns>Returns the new capacity.</returns>
//        private int BucketExpander(int requiredSize, int currentSize)
//        {
//            return requiredSize;
//        }
//        #endregion // BucketExpander(int requiredSize, int currentSize)
//        #region EmptyAdd(int index)
//        /// <summary>
//        /// This method adds an empty item to the free list.
//        /// </summary>
//        /// <param name="index">The index of the item to add to the sentinel.</param>
//        public override void EmptyAdd(int index)
//        {
//            //Check whether they are attempting to return a sentinel.
//            int noMaskIndex = index & 0x7FFFFFFF;
//            if (noMaskIndex != index)
//                throw new ArgumentOutOfRangeException("Sentinel vertexes cannot be returned.");

//            base.EmptyAdd(index);
//        }
//        #endregion // EmptyAdd(int index)

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

//            return mBuckets.ItemIsLocked(noMaskIndex);
//        }
//        #endregion // ItemIsLocked(int index)
//        #region ItemLock(int index)
//        /// <summary>
//        /// This method locks the specific item.
//        /// </summary>
//        /// <param name="index">The item index.</param>
//        /// <returns>Returns the number of lock cycles the thread entered.</returns>
//        public override void ItemLock(int index)
//        {
//            int noMaskIndex = index & 0x7FFFFFFF;
//            if (noMaskIndex == index)
//                mSlots.ItemLock(index);
//            else
//                mBuckets.ItemLock(noMaskIndex);
//        }
//        #endregion // ItemLock(int index)
//        #region ItemLockWait(int index)
//        /// <summary>
//        /// This method waits for a locked item to become available.
//        /// </summary>
//        /// <param name="index">The index of the item to wait for.</param>
//        /// <returns>Returns the number of lock cycles during the wait.</returns>
//        public override void ItemLockWait(int index)
//        {
//            int noMaskIndex = index & 0x7FFFFFFF;
//            if (noMaskIndex == index)
//                mSlots.ItemLockWait(index);
//            else
//                mBuckets.ItemLockWait(noMaskIndex);
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

//            return mBuckets.ItemTryLock(noMaskIndex);
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
//                mSlots.ItemUnlock(index);
//            else
//                mBuckets.ItemUnlock(noMaskIndex);
//        }
//        #endregion // ItemUnlock(int index)

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

//                return mBuckets[noMaskIndex];
//            }
//            set
//            {
//                int noMaskIndex = index & 0x7FFFFFFF;
//                if (noMaskIndex == index)
//                    mSlots[index] = value;
//                else
//                    mBuckets[noMaskIndex] = value;
//            }
//        }
//        #endregion // this[int index]

//        #region GetEnumerator()
//        /// <summary>
//        /// This method returns an enumeration through the sentinels and data in the collection.
//        /// </summary>
//        /// <returns>Returns an enumeration containing the collection data.</returns>
//        public override IEnumerator<KeyValuePair<int, ICollectionVertex<T>>> GetEnumerator()
//        {
//            CollectionVertexStruct<T> item = mBuckets[0];
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

//        #region SizeRecalculate(int total)
//        /// <summary>
//        /// This method calculates the current number of bits needed to support the current data.
//        /// </summary>
//        public override void SizeRecalculate(int total)
//        {
//            int currentBits = mCurrentBits;
//            int recalculateThreshold = mRecalculateThreshold;

//            if (total < recalculateThreshold)
//                return;

//            int newBits = (int)(Math.Log(total) / elog2);
//            int newThreshold = 2 << newBits;

//            if (newBits > currentBits)
//                Interlocked.CompareExchange(ref mCurrentBits, newBits, currentBits);

//            if (newThreshold > recalculateThreshold)
//                Interlocked.CompareExchange(ref mRecalculateThreshold, newThreshold, recalculateThreshold);
//        }
//        #endregion
//        #region ConvertBucketIDToIndexID(int bucketID)
//        /// <summary>
//        /// This method converts a bucketID to an index ID by setting the MSB.
//        /// </summary>
//        /// <param name="bucketID">The bucket ID to convert.</param>
//        /// <returns>The index ID.</returns>
//        int ConvertBucketIDToIndexID(int bucketID)
//        {
//            return unchecked((int)(bucketID | 0x80000000));
//        }
//        #endregion // ConvertBucketIDToIndexID(int bucketID)

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

//            int bitsStart = mCurrentBits;
//            int bitsCurrent = bitsStart;
//            int bucketIDParent = -1;

//            int bucketID = hashCode & (int.MaxValue >> (31 - bitsCurrent));

//            while (bitsCurrent >= 0)
//            {
//                if (bucketID != bucketIDParent)
//                {
//                    bucketIDParent = bucketID;

//                    //OK, check until we have reached a bucket with data, or we have reached the base bucket.
//                    if (bucketID == 0 || mBuckets[bucketID].IsSentinel)
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

//                    mBuckets.ItemLock(bucketID);
//                    //Check whether the bucket has been created, shouldn't happen but best to be safe.
//                    if (mBuckets[bucketID].HashID > 0)
//                    {
//                        mBuckets.ItemUnlock(bucketID);
//                        continue;
//                    }

//                    //Ok, get the hash ID for the bucket.
//                    int bucketHashID = BitReverse(bucketID);

//                    //Ok, insert the sentinel.
//                    StructBasedVertexWindow<T> vWin = new StructBasedVertexWindow<T>(
//                        this, mEqComparer, ConvertBucketIDToIndexID(bucketIDParent), bucketHashID, default(T));

//                    //Scan for the correct position to insert the sentinel.
//                    vWin.ScanAndLock();

//                    //Insert the new sentinel.
//                    vWin.InsertSentinel(ConvertBucketIDToIndexID(bucketID));

//                    //Unlock the window data.
//                    vWin.Unlock();

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
//            int sentIndexID =GetSentinelID(mEqComparer.GetHashCode(item), false, out hashID);

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

//        #region RootIndexID
//        /// <summary>
//        /// This is the index ID of the the first item.
//        /// </summary>
//        protected override int RootIndexID { get { return unchecked((int)0x80000000); } }
//        #endregion // RootIndexID

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
