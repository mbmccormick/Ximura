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
namespace Ximura.Collections.Data
{
    /// <summary>
    /// The vertex window structure holds the search results from a scan.
    /// </summary>
    /// <typeparam name="T">The collection type.</typeparam>
    [StructLayout(LayoutKind.Sequential)]
    public struct ClassBasedVertexWindow<T> : IVertexWindow<T>
    {
        #region Declarations
        ClassBasedVertexArray<T> mData;
        int mHashID;
        T mItem;
        IEqualityComparer<T> mEqualityComparer;
        /// <summary>
        /// The current vertex.
        /// </summary>
        private CollectionVertexClass<T> Curr;
        /// <summary>
        /// The next vertex.
        /// </summary>
        private CollectionVertexClass<T> Next;
        #endregion // Declarations
        #region Constructor
        /// <summary>
        /// This is the default constructor for the window.
        /// </summary>
        /// <param name="coll">The data collection.</param>
        /// <param name="vertex">The start vertex.</param>
        /// <param name="eqComparer">The equality comparer for the collection.</param>
        /// <param name="hashID">The hashID of the item.</param>
        /// <param name="item">The data item.</param>
        public ClassBasedVertexWindow(ClassBasedVertexArray<T> coll, CollectionVertexClass<T> vertex, IEqualityComparer<T> eqComparer, int hashID, T item)
        {
            mData = coll;
            Curr = vertex;
            mHashID = hashID;
            mItem = item;
            mEqualityComparer = eqComparer;

            Curr.Lock();

            if (!Curr.IsTerminator)
            {
                Curr.Next.Lock();
                Next = Curr.Next;
            }
            else
                Next = null;
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
        }
        #endregion // SetNextItem(T value)
        #region ItemInsert(T value)
        /// <summary>
        /// 
        /// </summary>
        /// <param name="hashID">The hashID to search for and lock.</param>
        public void ItemInsert()
        {
            CollectionVertexClassData<T> newItem = new CollectionVertexClassData<T>(mItem, mHashID);
            newItem.Lock();

            if (!Curr.IsTerminator)
            {
                newItem.Next = Next;
                Next.Unlock();
            }

            Next = newItem;
            Curr.Next = newItem;

            //Increment the necessary counters, and
            //check whether we need to recalculate the bit size.
            mData.CountIncrement();
        }
        #endregion

        #region InsertDataSentinel()
        /// <summary>
        /// This method inserts a data sentinel in to the data collection.
        /// </summary>
        public CollectionVertexClassDataSentinel<T> InsertDataSentinel()
        {
            CollectionVertexClassDataSentinel<T> newItem = new CollectionVertexClassDataSentinel<T>(mHashID);
            newItem.Lock();

            if (!Curr.IsTerminator)
            {
                newItem.Next = Next;
                Next.Unlock();
            }

            Next = newItem;
            Curr.Next = newItem;

            return newItem;
        }
        #endregion
        #region InsertSentinel(VertexClassBase<T> down)
        /// <summary>
        /// This method inserts a data sentinel in to the data collection.
        /// </summary>
        public CollectionVertexClassSentinel<T> InsertSentinel(CollectionVertexClass<T> down)
        {
            CollectionVertexClassSentinel<T> newItem = new CollectionVertexClassSentinel<T>(mHashID, down);
            newItem.Lock();

            if (!Curr.IsTerminator)
            {
                newItem.Next = Next;
                Next.Unlock();
            }

            Next = newItem;
            Curr.Next = newItem;

            return newItem;
        }
        #endregion

        #region Unlock()
        /// <summary>
        /// This method provides common functionality to unlock a VertexWindow.
        /// </summary>
        public void Unlock()
        {
            if (Next != null) Next.Unlock();
            if (Curr != null) Curr.Unlock();
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
                Curr.Unlock();
                Curr = Next;

                //If this is the last item in the list then move up and exit.
                if (Curr.IsTerminator)
                {
                    Next = null;
                    break;
                }

                //OK, lock the next item and move up.
                Next = Curr.Next;
                Next.Lock();
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

            Curr.Unlock();
            Curr = Next;

            if (Curr.IsTerminator)
                Next = null;
            else
            {
                Next = Curr.Next;
                Next.Lock();
            }

            return true;
        }
        #endregion

        #region RemoveItemAndUnlock()
        /// <summary>
        /// This method removes the next item for the list and discards it.
        /// </summary>
        /// <returns></returns>
        public void ItemRemoveAndUnlock()
        {
            Curr.Next = Next.Next;
            //Update the version and reduce the item count.
            mData.CountDecrement();
            //Unlock the current vertex so other searches can continue.
            Curr.Unlock();

            //Dispose of the old data, this will also remove any locks.
            Next.Dispose();
        }
        #endregion // SlotsRemoveItem

        #region Snip()
        /// <summary>
        /// This method snips out the next vertex.
        /// </summary>
        public void Snip()
        {
            CollectionVertexClass<T> temp = Next;

            if (Next.IsTerminator)
            {
                Curr.Next = null;
                Next = null;
            }
            else
            {
                temp.Next.Lock();
                Next = temp.Next;
                Curr.Next = Next;
            }

            temp.Unlock();
            temp.Dispose();

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
                return !Next.IsSentinel && mEqualityComparer.Equals(mItem, Next.Value);
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
            get { return Next.Value; }
        }

        #endregion
    }
}
