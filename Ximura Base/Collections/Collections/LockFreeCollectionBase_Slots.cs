#region Copyright
// *******************************************************************************
// Copyright (c) 2000-2009 Paul Stancer.
// All rights reserved. This program and the accompanying materials
// are made available under the terms of the Eclipse Public License v1.0
// which accompanies this distribution, and is available at
// http://www.eclipse.org/legal/epl-v10.html
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

using Ximura;
using Ximura.Helper;
#endregion // using
namespace Ximura.Collections
{
    public abstract partial class LockFreeCollectionBase<T>
    {
        #region Constants
        /// <summary>
        /// This is the reserved vertex position for the initial sentinel.
        /// </summary>
        private const int cnIndexData = 0;
        /// <summary>
        /// This is the reserved vertex position for the empty vertex queue.
        /// </summary>
        private const int cnIndexEmptyQueue = 1;
        #endregion // Declarations

        #region Struct -> Vertex<T>
        /// <summary>
        /// This structure is used to hold the item in the collection.
        /// </summary>
        /// <typeparam name="T">The container object.</typeparam>
        [StructLayout(LayoutKind.Sequential)]
        public struct Vertex<T>
        {
            #region Static methods
            /// <summary>
            /// This static method creates a sentinel vertex. Sentinel vertexes are vertexes that do not include data,
            /// but are used by the hash table to mark a shortcut to data sets based on their hashcode.
            /// </summary>
            /// <param name="hashID">The hashID.</param>
            /// <param name="nextSlotIDPlus1">The ID of the next vertex in the chain (plus 1).</param>
            /// <returns>Returns a new sentinel for the specific hash ID.</returns>
            public static Vertex<T> Sentinel(int hashID, int nextSlotIDPlus1)
            {
                return new Vertex<T>(hashID, nextSlotIDPlus1);
            }
            #endregion // Static methods
            #region Constants
            private const int cnSentinelMaskSet = 0x40000000;
            private const int cnSentinelMaskRemove = 0x3FFFFFFF;
            #endregion // Constants

            #region HashID
            /// <summary>
            /// The internal hashid.
            /// </summary>
            private int mHashID;
            /// <summary>
            /// The item hashid.
            /// </summary>
            public int HashID { get { return mHashID & cnSentinelMaskRemove; } }
            #endregion
            #region NextSlotIDPlus1
            /// <summary>
            /// The next item in the list.
            /// </summary>
            public int NextSlotIDPlus1;
            #endregion
            #region Value
            /// <summary>
            /// The slot value.
            /// </summary>
            public T Value;
            #endregion // Value

            #region Constructor
            /// <summary>
            /// This constructor creates a slot as a sentinel, with only the next parameter set.
            /// </summary>
            /// <param name="hashID">The item hashcode.</param>
            /// <param name="nextSlotIDPlus1">The next item in the list.</param>
            public Vertex(int hashID, int nextSlotIDPlus1)
            {
                mHashID = hashID | cnSentinelMaskSet;
                Value = default(T);
                NextSlotIDPlus1 = nextSlotIDPlus1;
            }
            /// <summary>
            /// This constructor sets the value for the slot.
            /// </summary>
            /// <param name="hashID">The item hashcode.</param>
            /// <param name="value">The slot value.</param>
            /// <param name="nextSlotIDPlus1">The next item in the list.</param>
            public Vertex(int hashID, T value, int nextSlotIDPlus1)
            {
                mHashID = hashID;
                Value = value;
                NextSlotIDPlus1 = nextSlotIDPlus1;
            }
            #endregion // Constructor

            #region IsTerminator
            /// <summary>
            /// This property identifies whether the vertex is the last item in the data chain.
            /// </summary>
            public bool IsTerminator { get { return NextSlotIDPlus1 == 0; } }
            #endregion // IsTerminator

            #region IsSentinel
            /// <summary>
            /// This property identifies whether the vertex is a sentinel vertex.
            /// </summary>
            public bool IsSentinel { get { return (mHashID & cnSentinelMaskSet) > 0; } }
            #endregion // IsSentinel

            #region ToString()
            /// <summary>
            /// This override provides quick and easy debugging support.
            /// </summary>
            /// <returns>Returns a string representation of the vertex.</returns>
            public override string ToString()
            {
                if (IsSentinel)
                    return string.Format("V->{0}  H{1:X} SNTL", IsTerminator ? "END" : (NextSlotIDPlus1 - 1).ToString(), HashID);
                else
                    return string.Format("V->{0}  H{1:X} = {2}", IsTerminator ? "END" : (NextSlotIDPlus1 - 1).ToString(), HashID, Value.ToString());
            }
            #endregion // ToString()
        }
        #endregion // struct LockFreeSlot<T>
        #region Struct -> VertexWindow<T>
        /// <summary>
        /// The vertex window structure holds the search results from a scan.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        [StructLayout(LayoutKind.Sequential)]
        protected struct VertexWindow<T>
        {
            #region Public properties
            /// <summary>
            /// The current slot ID plus 1.
            /// </summary>
            public int CurrSlotIDPlus1;
            /// <summary>
            /// THe current vertex.
            /// </summary>
            public Vertex<T> Curr;
            /// <summary>
            /// The next vertex.
            /// </summary>
            public Vertex<T> Next;
            #endregion // Public properties

            #region Constructor
            /// <summary>
            /// This constructor sets all the values for the window.
            /// </summary>
            /// <param name="slotIDPlus1">The current slotID plus 1.</param>
            /// <param name="curr">The current vertex.</param>
            /// <param name="next">The next vertex.</param>
            public VertexWindow(int slotIDPlus1, Vertex<T> curr, Vertex<T> next)
            {
                CurrSlotIDPlus1 = slotIDPlus1;
                Curr = curr;
                Next = next;
            }
            #endregion // Constructor

            #region SetCurrent(int indexPlus1, Vertex<T> curr)
            /// <summary>
            /// This method sets the current values.
            /// </summary>
            /// <param name="slotIDPlus1">The slotID index plus 1.</param>
            /// <param name="curr">The vertex.</param>
            public void SetCurrent(int slotIDPlus1, Vertex<T> curr)
            {
                CurrSlotIDPlus1 = slotIDPlus1;
                Curr = curr;
            }
            #endregion // SetCurrent(int indexPlus1, Vertex<T> curr)

            #region MoveUp()
            /// <summary>
            /// This method moves up the Next vertex to the current position.
            /// </summary>
            public void MoveUp()
            {
                CurrSlotIDPlus1 = Curr.NextSlotIDPlus1;
                Curr = Next;
                Next = new Vertex<T>();
            }
            #endregion // MoveUp()
            #region MoveUp(Vertex<T> next)
            /// <summary>
            /// This method moves up the next vertex to current and sets a new next vertex.
            /// </summary>
            /// <param name="next">The next vertex.</param>
            public void MoveUp(Vertex<T> next)
            {
                CurrSlotIDPlus1 = Curr.NextSlotIDPlus1;
                Curr = Next;
                Next = next;
            }
            #endregion // MoveUp(Vertex<T> next)

            #region Snip()
            /// <summary>
            /// This method returns a new vertex that removes the Next vertex in the list.
            /// </summary>
            /// <returns>The new Curr vertex that skips the Next vertex.</returns>
            public Vertex<T> Snip()
            {
                if (Curr.IsSentinel)
                    return Vertex<T>.Sentinel(Curr.HashID, Next.NextSlotIDPlus1);

                return new Vertex<T>(Curr.HashID, Curr.Value, Next.NextSlotIDPlus1);
            }
            #endregion // Snip()
            #region Insert()
            /// <summary>
            /// This method returns a new vertex that removes the Next vertex in the list.
            /// </summary>
            /// <param name="nextSlotIDPlus1">The next slotID plus 1.</param>
            /// <returns>The new Curr vertex that skips the Next vertex.</returns>
            public Vertex<T> Insert(int nextSlotIDPlus1)
            {
                if (Curr.IsSentinel)
                    return Vertex<T>.Sentinel(Curr.HashID, nextSlotIDPlus1);

                return new Vertex<T>(Curr.HashID, Curr.Value, nextSlotIDPlus1);
            }
            #endregion // Snip()
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

            #region SlotsInsertItem
            public void SlotsInsertItem(IFineGrainedLockArray<Vertex<T>> slots, int newSlot, int hashID, T value)
            {
                slots.ItemLock(newSlot);

                if (!Curr.IsTerminator)
                    slots.ItemUnlock(Curr.NextSlotIDPlus1 - 1);

                Next = new Vertex<T>(hashID, value, Curr.NextSlotIDPlus1);

                Curr.NextSlotIDPlus1 = newSlot + 1;

                slots[newSlot] = Next;
                slots[CurrSlotIDPlus1 - 1] = Curr;
            }
            #endregion
            #region SlotsInsertSentinel
            public void SlotsInsertSentinel(IFineGrainedLockArray<Vertex<T>> slots, int newSlot, int hashID)
            {
                slots.ItemLock(newSlot);

                if (!Curr.IsTerminator)
                    slots.ItemUnlock(Curr.NextSlotIDPlus1 - 1);

                Next = Vertex<T>.Sentinel(hashID, Curr.NextSlotIDPlus1);

                Curr.NextSlotIDPlus1 = newSlot + 1;

                slots[newSlot] = Next;
                slots[CurrSlotIDPlus1 - 1] = Curr;
            }
            #endregion // InsertSentinel(ExpandableFineGrainedLockArray<Vertex<T>> slots, int newSlot, int hashID)
            #region SlotsUnlock
            /// <summary>
            /// This method provides common functionality to unlock a VertexWindow.
            /// </summary>
            /// <param name="mSlots">The slot data.</param>
            public void SlotsUnlock(IFineGrainedLockArray<Vertex<T>> slots)
            {
                if (Curr.NextSlotIDPlus1 > 0) slots.ItemUnlock(Curr.NextSlotIDPlus1 - 1);
                if (CurrSlotIDPlus1 > 0) slots.ItemUnlock(CurrSlotIDPlus1 - 1);
            }
            #endregion
            #region SlotsScanAndLock
            /// <summary>
            /// This method scans through the slot data until is reaches the end of the data, or the position 
            /// where the hashID meets a slot with a hashID that is greater than itself.
            /// </summary>
            /// <param name="slots">The slot collection</param>
            /// <param name="hashID">The hashID to search for and lock.</param>
            public int SlotsScanAndLock(IFineGrainedLockArray<Vertex<T>> slots, int hashID)
            {
                //If the current is the last item in the linked list then exit.
                if (Curr.IsTerminator)
                    return 0;

                int hopCount = 0;

                while (Next.HashID < hashID)
                {
                    hopCount++;

                    //Unlock the old current item.
                    slots.ItemUnlock(CurrSlotIDPlus1 - 1);

                    CurrSlotIDPlus1 = Curr.NextSlotIDPlus1;

                    //If this is the last item in the list then move up and exit.
                    if (Next.IsTerminator)
                    {
                        Curr = Next;
                        Next = new Vertex<T>();
                        break;
                    }

                    //OK, lock the next item and move up.
                    slots.ItemLock(Next.NextSlotIDPlus1 - 1);
                    Curr = Next;
                    Next = slots[Curr.NextSlotIDPlus1 - 1];
                }

                return hopCount;
            }
            #endregion
            #region SlotsSetCurrentAndLock
            /// <summary>
            /// This method sets the current slot and locks the position.
            /// </summary>
            /// <param name="slots">The slot collection.</param>
            /// <param name="slotID">The slot ID.</param>
            public void SlotsSetCurrentAndLock(IFineGrainedLockArray<Vertex<T>> slots, int slotID)
            {
                slots.ItemLock(slotID);
                CurrSlotIDPlus1 = slotID + 1;
                Curr = slots[slotID];

                if (!Curr.IsTerminator)
                {
                    slots.ItemLock(Curr.NextSlotIDPlus1 - 1);
                    Next = slots[Curr.NextSlotIDPlus1 - 1];
                }
                else
                    Next = new Vertex<T>();
            }
            #endregion
            #region SlotsMoveUp
            /// <summary>
            /// This method moves up the Next vertex to the current position.
            /// </summary>
            public bool SlotsMoveUp(IFineGrainedLockArray<Vertex<T>> slots)
            {
                if (Curr.IsTerminator)
                    return false;

                slots.ItemUnlock(CurrSlotIDPlus1 - 1);
                CurrSlotIDPlus1 = Curr.NextSlotIDPlus1;
                Curr = Next;

                if (!Curr.IsTerminator)
                {
                    slots.ItemLock(Curr.NextSlotIDPlus1 - 1);
                    Next = slots[Curr.NextSlotIDPlus1 - 1];
                }
                else
                    Next = new Vertex<T>();

                return true;
            }
            #endregion // MoveUp()

            #region SlotsRemoveItem
            public int SlotsRemoveItem(IFineGrainedLockArray<Vertex<T>> slots)
            {
                int removedItem = Curr.NextSlotIDPlus1 - 1;

                Curr.NextSlotIDPlus1 = Next.NextSlotIDPlus1;

                slots[CurrSlotIDPlus1 - 1] = Curr;
                slots.ItemUnlock(removedItem);

                return removedItem;
            }
            #endregion // SlotsRemoveItem

            #region SlotsSnip
            public int SlotsSnip(IFineGrainedLockArray<Vertex<T>> slots)
            {
                int removedItem = Curr.NextSlotIDPlus1 - 1;

                Curr.NextSlotIDPlus1 = Next.NextSlotIDPlus1;

                slots[CurrSlotIDPlus1 - 1] = Curr;
                slots.ItemUnlock(removedItem);

                if (!Curr.IsTerminator)
                {
                    slots.ItemLock(Curr.NextSlotIDPlus1 - 1);
                    Next = slots[Curr.NextSlotIDPlus1 - 1];
                }
                else
                    Next = new Vertex<T>();

                return removedItem;
            }
            #endregion // SlotsRemoveItem

        }
        #endregion // Struct -> Vertex<T>

        #region Declarations
        /// <summary>
        /// This collection holds the data.
        /// </summary>
        private IFineGrainedLockArray<Vertex<T>> mSlots;

        /// <summary>
        /// This is the free data queue tail position.
        /// </summary>
        private int mFreeListTail;
        /// <summary>
        /// This is the free data queue item count.
        /// </summary>
        private int mFreeListCount;
        /// <summary>
        /// This is the current next free position in the data collection.
        /// </summary>
        private int mLastIndex;

        #endregion 

        #region InitializeData(int capacity, bool isFixedSize)
        /// <summary>
        /// This method initializes the data allocation.
        /// </summary>
        protected virtual void InitializeData(int capacity, bool isFixedSize)
        {
            mSlots = new ExpandableFineGrainedLockArray<Vertex<T>>(capacity);

            mSlots[cnIndexData] = Vertex<T>.Sentinel(0, 0);
            mSlots[cnIndexEmptyQueue] = Vertex<T>.Sentinel(0, 0);

            mFreeListTail = cnIndexEmptyQueue;
            mFreeListCount = 0;

            mLastIndex = cnIndexEmptyQueue;
        }
        #endregion // InitializeAllocation(int capacity)

        #region EmptyGet()
        /// <summary>
        /// This method returns the next free item, either from empty space, or from a free item in the collection.
        /// </summary>
        /// <returns>Returns the index for the next free item.</returns>
        protected int EmptyGet()
        {
#if (PROFILING)
            int start = Environment.TickCount;
            try
            {
#endif
                //If there are free items, try and lock the empty sentinel, 
                //but if already locked, just take a new item from the end of the collection.
                while (mFreeListCount > 0 && mSlots.ItemTryLock(cnIndexEmptyQueue))
                {
                    try
                    {
                        //Get the free sentinel pointer.
                        Vertex<T> freeSent = mSlots[cnIndexEmptyQueue];

                        //If there are no items in the list then get a new item.
                        if (freeSent.IsTerminator)
                            break;

                        //See if we can lock the free item, if not then just get a new item.
                        int freeItem = freeSent.NextSlotIDPlus1 - 1;
                        if (!mSlots.ItemTryLock(freeItem))
                            break;

                        //OK get the item.
                        Vertex<T> item = mSlots[freeSent.NextSlotIDPlus1 - 1];

                        //OK, remove the free item from the list and set the sentinel to the next item.
                        mSlots[cnIndexEmptyQueue] = Vertex<T>.Sentinel(0, item.NextSlotIDPlus1);

                        if (freeItem == mFreeListTail)
                            mFreeListTail = cnIndexEmptyQueue;

                        //Unlock the free item.
                        mSlots.ItemUnlock(freeItem);

                        Interlocked.Decrement(ref mFreeListCount);

                        //Returns the index of the free item.
                        return freeItem;
                    }
                    finally
                    {
                        //Unlock the empty sentinel pointer.
                        mSlots.ItemUnlock(cnIndexEmptyQueue);
                    }
                }

                int nextItem = Interlocked.Increment(ref mLastIndex);

                if (nextItem == 0)
                    throw new ArgumentOutOfRangeException("The list has exceeeded the capacity of the maximum integer value.");

                return nextItem;
#if (PROFILING)
            }
            finally
            {
                Profile(ProfileAction.Time_EmptyGet, Environment.TickCount - start);
            }
#endif
        }
        #endregion // EmptyGet()
        #region EmptyAdd(int index)
        /// <summary>
        /// This method adds an empty item to the free list.
        /// </summary>
        /// <param name="index">The index of the item to add to the sentinel.</param>
        protected void EmptyAdd(int index)
        {
#if (PROFILING)
            int profilestart = Environment.TickCount;
            try
            {
#endif
                int pos = mFreeListTail;
                while (mSlots.ItemTryLock(pos))
                {
                    if (pos != mFreeListTail)
                    {
                        mSlots.ItemUnlock(pos);
                        pos = mFreeListTail;
                        Threading.ThreadWait();
                        continue;
                    }
                }

                try
                {
                    mSlots[index] = Vertex<T>.Sentinel(0, 0);
                    mSlots[pos] = Vertex<T>.Sentinel(0, index + 1);
                    mFreeListTail = index;
                    Interlocked.Increment(ref mFreeListCount);
                }
                finally
                {
                    mSlots.ItemUnlock(pos);
                }
#if (PROFILING)
            }
            finally
            {
                Profile(ProfileAction.Time_EmptyAdd, Environment.TickCount - profilestart);
            }
#endif
        }
        #endregion // EmptyAdd(int index)
    }
}
