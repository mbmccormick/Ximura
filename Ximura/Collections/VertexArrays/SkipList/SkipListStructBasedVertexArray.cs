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
    /// This vertex array implements the data as a skip list array.
    /// </summary>
    /// <typeparam name="T">The collection type.</typeparam>
    public class SkipListStructBasedVertexArray<T> : MultiLevelBucketStructBasedVertexArray<T, SkipListSentinelVertexStruct>
    {
        #region SkipListStructBasedVertexWindow<T>
        /// <summary>
        /// The vertex window structure holds the search results from a scan.
        /// </summary>
        /// <typeparam name="T">The collection type.</typeparam>
        [StructLayout(LayoutKind.Sequential)]
        internal struct SkipListStructBasedVertexWindow<T> : IVertexWindow<T>
        {
            #region Declarations
            SkipListStructBasedVertexArray<T> mData;
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
            public SkipListStructBasedVertexWindow(SkipListStructBasedVertexArray<T> data,
                IEqualityComparer<T> eqComparer, int indexID, int hashID, T item)
            {
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
            #region Constructor
            /// <summary>
            /// This is the default constructor for the window.
            /// </summary>
            /// <param name="data">The data collection.</param>
            /// <param name="indexID">The index of the position to set the window.</param>
            public SkipListStructBasedVertexWindow(SkipListStructBasedVertexArray<T> data,
                IEqualityComparer<T> eqComparer, int indexID, int hashID)
            {
                mData = data;
                mHashID = hashID;
                mItem = default(T);
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

            #region InsertSentinel(int indexID)
            /// <summary>
            /// This method inserts a sentinel in to the data collection.
            /// </summary>
            /// <param name="indexID">The new sentinel index id.</param>
            /// <param name="hashID">The sentinel hash id.</param>
            public void InsertSentinel(int indexID)
            {
                if (!Curr.IsTerminator)
                    mData.ItemUnlock(Curr.NextSlotIDPlus1 - 1);

                Next = CollectionVertexStruct<T>.Sentinel(mHashID, Curr.NextSlotIDPlus1);

                Curr.NextSlotIDPlus1 = indexID + 1;

                mData[indexID] = Next;
                mData[CurrSlotIDPlus1 - 1] = Curr;
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

        #region Static Declarations
        /// <summary>
        /// This is a thread specific value. This is to ensure that each thread gets a specific
        /// random class as Random is not thread safe.
        /// </summary>
        [ThreadStatic()]
        static Random sRand;
        #endregion // Static Declarations
        #region Declarations
        /// <summary>
        /// The probability of an item being propagated to the next level.
        /// </summary>
        private double mProbability;
        #endregion // Declarations
        #region Constructor
        /// <summary>
        /// This is the default constructor for the array.
        /// </summary>
        public SkipListStructBasedVertexArray()
        {
        }
        #endregion // Constructor

        #region ConvertProbabilityToBool()
        /// <summary>
        /// This method converts the output from the thread specific random function to a simple boolean value.
        /// </summary>
        /// <returns>Returns a boolean value based on the probability factor.</returns>
        private bool ConvertProbabilityToBool()
        {
            //Create the random function for the specific thread.
            if (sRand == null)
                sRand = new Random();

            return (sRand.NextDouble() < mProbability);
        }
        #endregion
        #region Probability
        /// <summary>
        /// The probability of an item being propagated to the next level.
        /// </summary>
        public virtual double Probability
        {
            get { return mProbability; }
            set
            {
                if (value <= 0 || value > 1)
                    throw new ArgumentOutOfRangeException("The probability must be greater than 0 and less than or equal to 1.");

                //We have to use interlock for setting this value as it is read by multiple threads at the same time.
                Interlocked.Exchange(ref mProbability, value);
            }
        }
        #endregion // Probability
        #region LevelMax
        /// <summary>
        /// This is the maximum levels implemented by the skip list.
        /// </summary>
        public override int BucketsLevelMax { get { return 16; } }
        #endregion // LevelMax



        protected override void BucketCalculatePosition(int indexID, out int level, out int levelPosition)
        {
            throw new NotImplementedException();
        }

        protected override int BucketLevelCapacityCalculate(int level)
        {
            throw new NotImplementedException();
        }

        protected override int GetSentinelID(int hashCode, bool createSentinel, out int hashID)
        {
            throw new NotImplementedException();
        }

        public override IVertexWindow<T> VertexWindowGet()
        {
            throw new NotImplementedException();
        }

        public override IVertexWindow<T> VertexWindowGet(T item, bool createSentinel)
        {
            throw new NotImplementedException();
        }
    }
}
