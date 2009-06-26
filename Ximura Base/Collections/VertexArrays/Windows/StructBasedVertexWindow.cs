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
    /// The vertex window structure holds the search results from a scan.
    /// </summary>
    /// <typeparam name="T">The collection type.</typeparam>
    //[StructLayout(LayoutKind.Sequential)]
    public struct StructBasedVertexWindow<T> : IVertexWindow<T>
    {
        #region Declarations
        StructBasedVertexArray<T> mData;
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
        private VertexStruct<T> Curr;
        /// <summary>
        /// The next vertex.
        /// </summary>
        private VertexStruct<T> Next;
        #endregion // Declarations
        #region Constructor
        /// <summary>
        /// This is the default constructor for the window.
        /// </summary>
        /// <param name="data">The data collection.</param>
        /// <param name="indexID">The index of the position to set the window.</param>
        public StructBasedVertexWindow(StructBasedVertexArray<T> data, IEqualityComparer<T> eqComparer, int indexID, int hashID, T item)
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
                Next = new VertexStruct<T>();
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

            Next = new VertexStruct<T>(mHashID, mItem, Curr.NextSlotIDPlus1);

            Curr.NextSlotIDPlus1 = newSlot + 1;

            mData[newSlot] = Next;
            mData[CurrSlotIDPlus1 - 1] = Curr;

            //Increment the necessary counters, and
            //check whether we need to recalculate the bit size.
            mData.SizeRecalculate(mData.CountIncrement());
        }
        #endregion

        #region InsertSentinel(int bucketID, int hashID)
        /// <summary>
        /// This method inserts a sentinel in to the data collection.
        /// </summary>
        /// <param name="indexID">The new sentinel index id.</param>
        /// <param name="hashID">The sentinel hash id.</param>
        public void InsertSentinel(int indexID)
        {
            if (!Curr.IsTerminator)
                mData.ItemUnlock(Curr.NextSlotIDPlus1 - 1);

            Next = VertexStruct<T>.Sentinel(mHashID, Curr.NextSlotIDPlus1);

            Curr.NextSlotIDPlus1 = indexID + 1;

            mData[indexID] = Next;
            mData[CurrSlotIDPlus1 - 1] = Curr;
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
                    Next = new VertexStruct<T>();
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
                Next = new VertexStruct<T>();

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
                Next = new VertexStruct<T>();

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
}
