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
    /// This class contains the combined buckets and slots in a single class.
    /// </summary>
    /// <typeparam name="T">The collection type.</typeparam>
    public partial class HashTableStructBasedVertexArrayV3<T> :
        MultiLevelBucketStructBasedVertexArray<T, CollectionVertexStruct<T>>
    {
        /// <summary>
        /// The vertex window structure holds the search results from a scan.
        /// </summary>
        /// <typeparam name="D">The structure data type.</typeparam>
        [Serializable, StructLayout(LayoutKind.Sequential)]
        private struct StructBasedVertexWindowV3<D>
        {
            #region Declarations
            /// <summary>
            /// The internal reference.
            /// </summary>
            private HashTableStructBasedVertexArrayV3<D> mData;
            /// <summary>
            /// The hashID
            /// </summary>
            private int mHashID;
            /// <summary>
            /// The item.
            /// </summary>
            private D mItem;
            /// <summary>
            /// The equality comparer.
            /// </summary>
            private IEqualityComparer<D> mEqualityComparer;
            /// <summary>
            /// The current slot ID plus 1.
            /// </summary>
            private int CurrSlotIDPlus1;
            /// <summary>
            /// THe current vertex.
            /// </summary>
            private CollectionVertexStruct<D> Curr;
            /// <summary>
            /// The next vertex.
            /// </summary>
            private CollectionVertexStruct<D> Next;
            #endregion // Declarations
            #region Constructor
            /// <summary>
            /// This is the default constructor for the window.
            /// </summary>
            /// <param name="data">The data collection.</param>
            /// <param name="indexID">The index of the position to set the window.</param>
            public StructBasedVertexWindowV3(HashTableStructBasedVertexArrayV3<D> data,
                IEqualityComparer<D> eqComparer, int indexID, int hashID, D item)
            {
                mData = data;
                mHashID = hashID;
                mItem = item;
                mEqualityComparer = eqComparer;
#if (LOCKDEBUG)
                Console.WriteLine("Window created for {0:x} {1:x} on {2}"
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
                    Next = new CollectionVertexStruct<D>();
            }
            #endregion // Constructor
            #region Sentinel Constructor
            /// <summary>
            /// This is the default constructor for the window.
            /// </summary>
            /// <param name="data">The data collection.</param>
            /// <param name="indexID">The index of the position to set the window.</param>
            public StructBasedVertexWindowV3(HashTableStructBasedVertexArrayV3<D> data,
                IEqualityComparer<D> eqComparer, int indexID, int hashID)
            {
                mData = data;
                mHashID = hashID;
                mItem = default(D);
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
                    Next = new CollectionVertexStruct<D>();
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
            public D Value
            {
                get { return mItem; }
            }
            #endregion

            #region ScanAndRemoveMarked()
            /// <summary>
            /// This method scans through the slot data until is reaches the end of the data, or the position 
            /// where the hashID meets a slot with a hashID that is greater than itself.
            /// </summary>
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
                        Next = new CollectionVertexStruct<D>();
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
            public void ItemLockAndInsert()
            {
                int newSlot = mData.EmptyGet();

                mData.ItemLock(newSlot);

                if (!Curr.IsTerminator)
                    mData.ItemUnlock(Curr.NextSlotIDPlus1 - 1);

                Next = new CollectionVertexStruct<D>(mHashID, mItem, Curr.NextSlotIDPlus1);

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
                    Next = new CollectionVertexStruct<D>();

                return true;
            }
            #endregion
            #region SentinelLockAndInsert(int indexID)
            /// <summary>
            /// This method inserts a sentinel in to the data collection.
            /// </summary>
            /// <param name="indexID">The new sentinel index id.</param>
            public void SentinelLockAndInsert(int indexID)
            {
//                ScanAndLock();

//                //If the current item is part of a list unlock the next item.
//                if (!Curr.IsTerminator)
//                    mData.ItemUnlock(Curr.NextSlotIDPlus1 - 1);

//                //Set the new sentinel and unlock.
//                mData[indexID] = CollectionVertexStruct<D>.Sentinel(mHashID, Curr.NextSlotIDPlus1);
//                mData.ItemUnlock(indexID);

//                //Set the current item and unlock.
//                Curr.NextSlotIDPlus1 = indexID + 1;
//                mData[CurrSlotIDPlus1 - 1] = Curr;
//                mData.ItemUnlock(CurrSlotIDPlus1 - 1);

//#if (LOCKDEBUG)
//                Console.WriteLine("Window sentinel unlocked for {0:x} {1:x} on {2}"
//                    , CurrSlotIDPlus1 - 1, mHashID, Thread.CurrentThread.ManagedThreadId);
//#endif
            }
            #endregion

            #region ItemInsert(T value)
            ///// <summary>
            ///// 
            ///// </summary>
            ///// <param name="hashID">The hashID to search for and lock.</param>
            //public void ItemInsert()
            //{
            //    int newSlot = mData.EmptyGet();

            //    mData.ItemLock(newSlot);

            //    if (!Curr.IsTerminator)
            //        mData.ItemUnlock(Curr.NextSlotIDPlus1 - 1);

            //    Next = new CollectionVertexStruct<T>(mHashID, mItem, Curr.NextSlotIDPlus1);

            //    Curr.NextSlotIDPlus1 = newSlot + 1;

            //    mData[newSlot] = Next;
            //    mData[CurrSlotIDPlus1 - 1] = Curr;

            //    //Increment the necessary counters, and
            //    //check whether we need to recalculate the bit size.
            //    mData.BucketSizeRecalculate(mData.CountIncrement(), false);
            //}
            #endregion

            #region InsertSentinelAndUnlock(int indexID)
//            /// <summary>
//            /// This method inserts a sentinel in to the data collection.
//            /// </summary>
//            /// <param name="indexID">The new sentinel index id.</param>
//            /// <param name="hashID">The sentinel hash id.</param>
//            public void InsertSentinelAndUnlock(int indexID)
//            {
//                ScanAndLock();

//                //If the current item is part of a list unlock the next item.
//                if (!Curr.IsTerminator)
//                    mData.ItemUnlock(Curr.NextSlotIDPlus1 - 1);

//                //Set the new sentinel and unlock.
//                mData[indexID] = CollectionVertexStruct<D>.Sentinel(mHashID, Curr.NextSlotIDPlus1);
//                mData.ItemUnlock(indexID);

//                //Set the current item and unlock.
//                Curr.NextSlotIDPlus1 = indexID + 1;
//                mData[CurrSlotIDPlus1 - 1] = Curr;
//                mData.ItemUnlock(CurrSlotIDPlus1 - 1);

//#if (LOCKDEBUG)
//                Console.WriteLine("Window sentinel unlocked for {0:x} {1:x} on {2}"
//                    , CurrSlotIDPlus1 - 1, mHashID, Thread.CurrentThread.ManagedThreadId);
//#endif
//            }
            #endregion
            #region Unlock()
//            /// <summary>
//            /// This method provides common functionality to unlock a VertexWindow.
//            /// </summary>
//            public void Unlock()
//            {
//                if (Curr.NextSlotIDPlus1 != 0) mData.ItemUnlock(Curr.NextSlotIDPlus1 - 1);
//                if (CurrSlotIDPlus1 != 0) mData.ItemUnlock(CurrSlotIDPlus1 - 1);

//#if (LOCKDEBUG)
//                Console.WriteLine("Window unlocked for {0:x} {1:x} on {2}"
//                    , CurrSlotIDPlus1 - 1, mHashID, Thread.CurrentThread.ManagedThreadId);
//#endif
//            }
            #endregion
            #region MoveUp()
            ///// <summary>
            ///// This method moves up the Next vertex to the current position.
            ///// </summary>
            //public bool MoveUp()
            //{
            //    if (Curr.IsTerminator)
            //        return false;

            //    mData.ItemUnlock(CurrSlotIDPlus1 - 1);
            //    CurrSlotIDPlus1 = Curr.NextSlotIDPlus1;
            //    Curr = Next;

            //    if (!Curr.IsTerminator)
            //    {
            //        mData.ItemLock(Curr.NextSlotIDPlus1 - 1);
            //        Next = mData[Curr.NextSlotIDPlus1 - 1];
            //    }
            //    else
            //        Next = new CollectionVertexStruct<T>();

            //    return true;
            //}
            #endregion
            #region ScanAndLock()
            ///// <summary>
            ///// This method scans through the slot data until is reaches the end of the data, or the position 
            ///// where the hashID meets a slot with a hashID that is greater than itself.
            ///// </summary>
            ///// <param name="hashID">The hashID to search for and lock.</param>
            //public int ScanAndLock()
            //{
            //    //If the current is the last item in the linked list then exit.
            //    if (Curr.IsTerminator)
            //        return 0;

            //    int hopCount = 0;

            //    while (Next.HashID < mHashID)
            //    {
            //        hopCount++;

            //        //Unlock the old current item.
            //        mData.ItemUnlock(CurrSlotIDPlus1 - 1);

            //        CurrSlotIDPlus1 = Curr.NextSlotIDPlus1;

            //        //If this is the last item in the list then move up and exit.
            //        if (Next.IsTerminator)
            //        {
            //            Curr = Next;
            //            Next = new CollectionVertexStruct<D>();
            //            break;
            //        }

            //        //OK, lock the next item and move up.
            //        mData.ItemLock(Next.NextSlotIDPlus1 - 1);
            //        Curr = Next;
            //        Next = mData[Curr.NextSlotIDPlus1 - 1];
            //    }

            //    return hopCount;
            //}
            #endregion
            #region RemoveItemAndUnlock()
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
//#if (LOCKDEBUG)
//                Console.WriteLine("Window remove and unlock for {0:x} {1:x} on {2}"
//                    , CurrSlotIDPlus1 - 1, mHashID, Thread.CurrentThread.ManagedThreadId);
//#endif
//            }
            #endregion // SlotsRemoveItem

            #region Snip()
            ///// <summary>
            ///// 
            ///// </summary>
            //public void Snip()
            //{
            //    int removedItem = Curr.NextSlotIDPlus1 - 1;

            //    Curr.NextSlotIDPlus1 = Next.NextSlotIDPlus1;

            //    mData[CurrSlotIDPlus1 - 1] = Curr;
            //    mData.ItemUnlock(removedItem);

            //    if (!Curr.IsTerminator)
            //    {
            //        mData.ItemLock(Curr.NextSlotIDPlus1 - 1);
            //        Next = mData[Curr.NextSlotIDPlus1 - 1];
            //    }
            //    else
            //        Next = new CollectionVertexStruct<T>();

            //    //Add the empty item for re-allocation.
            //    mData.EmptyAdd(removedItem);

            //    mData.CountDecrement();
            //}
            #endregion // SlotsRemoveItem

            #region ScanProcess()
            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
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
            /// <summary>
            /// 
            /// </summary>
            public bool NextIsSentinel
            {
                get { return Next.IsSentinel; }
            }
            #endregion // NextIsSentinel
            #region CurrIsTerminator
            /// <summary>
            /// 
            /// </summary>
            public bool CurrIsTerminator
            {
                get { return Curr.IsTerminator; }
            }
            #endregion // CurrIsTerminator

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
    }
}
