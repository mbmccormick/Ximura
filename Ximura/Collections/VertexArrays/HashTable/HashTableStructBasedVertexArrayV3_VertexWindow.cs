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
#if (!SILVERLIGHT)
        [Serializable]
#endif
        [StructLayout(LayoutKind.Sequential)]
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
            /// This is the next slot ID plus 1.
            /// </summary>
            private int NextSlotIDPlus1;
            /// <summary>
            /// This is the ID of the last sentinel in the scan plus 1.
            /// </summary>
            private int LastSentinelIDPlus1;
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
                LastSentinelIDPlus1 = indexID + 1;
                mData = data;
                mHashID = hashID;
                mItem = item;
                mEqualityComparer = eqComparer;
#if (LOCKDEBUG)
                Console.WriteLine("Window created for {0:x} {1:x} on {2}"
                    , indexID, mHashID, Thread.CurrentThread.ManagedThreadId);
#endif
                mData.ItemLock(indexID);
                CurrSlotIDPlus1 = LastSentinelIDPlus1;
                NextSlotIDPlus1 = 0;

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
                LastSentinelIDPlus1 = indexID + 1;
                NextSlotIDPlus1 = 0;
#if (LOCKDEBUG)
                Console.WriteLine("S Window created for {0:x} {1:x} on {2}"
                    , indexID, mHashID, Thread.CurrentThread.ManagedThreadId);
#endif
                mData.ItemLock(indexID);
                CurrSlotIDPlus1 = indexID + 1;

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


                return 0;
            }
            #endregion
            #region ItemLockAndInsert()
            /// <summary>
            /// 
            /// </summary>
            public void ItemLockAndInsert()
            {

            }
            #endregion
            #region ItemLockAndRemove()
            /// <summary>
            /// 
            /// </summary>
            public void ItemLockAndRemove()
            {

            }
            #endregion // ItemLockAndRemove()
            #region ItemLockAndSetNext()
            /// <summary>
            /// This method changes the value of the next item.
            /// </summary>
            public void ItemLockAndSetNext()
            {
            }
            #endregion // SetNextItem(T value)
            #region MoveUpAndRemoveMarked()
            /// <summary>
            /// This method moves up the Next vertex to the current position.
            /// </summary>
            public bool MoveUpAndRemoveMarked()
            {

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

            #region ScanProcess()
            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            public bool ScanProcess()
            {
                return false;
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
                    return false;
                }
            }
            #endregion // ScanItemMatch

            #region NextIsSentinel
            /// <summary>
            /// 
            /// </summary>
            public bool NextIsSentinel
            {
                get { return false; }
            }
            #endregion // NextIsSentinel
            #region CurrIsTerminator
            /// <summary>
            /// 
            /// </summary>
            public bool CurrIsTerminator
            {
                get { return false; }
            }
            #endregion // CurrIsTerminator

            #region ToString()
            /// <summary>
            /// This override provides a debug friendly representation of the structure.
            /// </summary>
            /// <returns>Returns the structure value.</returns>
            public override string ToString()
            {
                return "";
            }
            #endregion // ToString()
        }
    }
}
