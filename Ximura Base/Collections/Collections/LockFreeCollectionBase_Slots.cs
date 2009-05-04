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
        #region Static Declarations
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
            /// This is the empty vertex.
            /// </summary>
            public static readonly Vertex<T> Empty;
            /// <summary>
            /// The static constructor creates the empty vertex.
            /// </summary>
            static Vertex()
            {
                Empty = new Vertex<T>(0, default(T), 0);
            }
            /// <summary>
            /// This static method creates a sentinel vertex. Sentinel vertexes are vertexes that do not include data,
            /// but are used by the hash table to mark a shortcut to data sets based on their hashcode.
            /// </summary>
            /// <param name="hashCode">The hashcode.</param>
            /// <param name="next">The ID of the next vertex in the chain (plus 1).</param>
            /// <returns></returns>
            public static Vertex<T> Sentinel(int hashCode, int nextIDPlus1)
            {
                return new Vertex<T>(hashCode, default(T), nextIDPlus1);
            }
            #endregion // Static methods

            /// <summary>
            /// The item hashcode.
            /// </summary>
            public int HashCode;
            /// <summary>
            /// The next item in the list.
            /// </summary>
            public int NextIDPlus1;
            /// <summary>
            /// The slot value.
            /// </summary>
            public T Value;

            #region Constructor
            /// <summary>
            /// This constructor creates a slot as a sentinel, with only the next parameter set.
            /// </summary>
            /// <param name="next"></param>
            public Vertex(int hashcode, int nextIDPlus1)
            {
                HashCode = hashcode;
                Value = default(T);
                NextIDPlus1 = nextIDPlus1;
            }
            /// <summary>
            /// This constructor sets the value for the slot.
            /// </summary>
            /// <param name="hashCode">The item hashcode.</param>
            /// <param name="value">The slot value.</param>
            /// <param name="next">The next item in the list.</param>
            public Vertex(int hashCode, T value, int nextIDPlus1)
            {
                HashCode = hashCode;
                Value = value;
                NextIDPlus1 = nextIDPlus1;
            }
            #endregion // Constructor

            #region IsTerminator
            /// <summary>
            /// This property identifies whether the vertex is the last item in the data chain.
            /// </summary>
            public bool IsTerminator { get { return NextIDPlus1 == 0; } }
            #endregion // IsTerminator

            #region IsSentinel
            /// <summary>
            /// This property identifies whether the vertex is a sentinel vertex.
            /// </summary>
            public bool IsSentinel { get { return Value.Equals(default(T)); } }
            #endregion // IsSentinel

            #region ToString()
            /// <summary>
            /// This override provides quick and easy debugging support.
            /// </summary>
            /// <returns></returns>
            public override string ToString()
            {
                if (IsSentinel)
                    return string.Format("V->{0}  H{1:X} SNTL", IsTerminator ? "END" : (NextIDPlus1 - 1).ToString(), HashCode);
                else
                    return string.Format("V->{0}  H{1:X} = {2}", IsTerminator ? "END" : (NextIDPlus1 - 1).ToString(), HashCode, Value.ToString());
            }
            #endregion // ToString()

            public override bool Equals(object obj)
            {
                return base.Equals(obj);
            }

            public override int GetHashCode()
            {
                return base.GetHashCode();
            }
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
            /// The current index plus 1.
            /// </summary>
            public int CurrIndexPlus1;
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
            /// THis constructor sets all the values for the window.
            /// </summary>
            /// <param name="index">The current index.</param>
            /// <param name="curr">The current vertex.</param>
            /// <param name="next">The next vertex.</param>
            public VertexWindow(int index, Vertex<T> curr, Vertex<T> next)
            {
                CurrIndexPlus1 = index;
                Curr = curr;
                Next = next;
            }
            #endregion // Constructor

            #region SetCurrent(int indexPlus1, Vertex<T> curr)
            /// <summary>
            /// This method sets the current values.
            /// </summary>
            /// <param name="indexPlus1">The vertex index plus 1.</param>
            /// <param name="curr">The vertex.</param>
            public void SetCurrent(int indexPlus1, Vertex<T> curr)
            {
                CurrIndexPlus1 = indexPlus1;
                Curr = curr;
            }
            #endregion // SetCurrent(int indexPlus1, Vertex<T> curr)

            #region MoveUp()
            /// <summary>
            /// This method moves up the Next vertex to the current position.
            /// </summary>
            public void MoveUp()
            {
                CurrIndexPlus1 = Curr.NextIDPlus1; ;
                Curr = Next;
                Next = Vertex<T>.Empty;
            }
            #endregion // MoveUp()

            #region MoveUp(Vertex<T> next)
            /// <summary>
            /// This method moves up the next vertex to current and sets a new next vertex.
            /// </summary>
            /// <param name="next">The next vertex.</param>
            public void MoveUp(Vertex<T> next)
            {
                CurrIndexPlus1 = Curr.NextIDPlus1;
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
                return new Vertex<T>(Curr.HashCode, Curr.Value, Next.NextIDPlus1);
            }
            #endregion // Snip()

            public override bool Equals(object obj)
            {
                return base.Equals(obj);
            }

            public override int GetHashCode()
            {
                return base.GetHashCode();
            }

            #region Insert()
            /// <summary>
            /// This method returns a new vertex that removes the Next vertex in the list.
            /// </summary>
            /// <returns>The new Curr vertex that skips the Next vertex.</returns>
            public Vertex<T> Insert(int nextIndexPlus1)
            {
                return new Vertex<T>(Curr.HashCode, Curr.Value, nextIndexPlus1);
            }
            #endregion // Snip()

            #region ToString()
            /// <summary>
            /// This override provides a debug friendly representation of the structure.
            /// </summary>
            /// <returns>Returns the structure value.</returns>
            public override string ToString()
            {
                return string.Format("{0}[{1}] -> {2}[{3}]", CurrIndexPlus1 - 1, Curr, Curr.NextIDPlus1 - 1, Next);
            }
            #endregion // ToString()
        }
        #endregion // Struct -> Vertex<T>

        #region Declarations
        /// <summary>
        /// This collection holds the data.
        /// </summary>
        private ExpandableFineGrainedLockArray<Vertex<T>> mSlots;

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

        #region InitializeData(int capacity)
        /// <summary>
        /// This method initializes the data allocation.
        /// </summary>
        protected virtual void InitializeData(int capacity)
        {
            mSlots = new ExpandableFineGrainedLockArray<Vertex<T>>(capacity);

            mSlots[cnIndexData] = Vertex<T>.Sentinel(0, 0);
            mSlots[cnIndexEmptyQueue] = Vertex<T>.Sentinel(0, 0);

            mFreeListTail = cnIndexEmptyQueue;
            mFreeListCount = 0;

            mLastIndex = cnIndexEmptyQueue;
        }
        #endregion // InitializeAllocation(int capacity)

        #region FindAndLock(T item, int hashCode, int index, out VertexWindow<T> vWin)
        /// <summary>
        /// This method scans the collection for the item and returns true if the item is found.
        /// </summary>
        /// <param name="item">The item to scan.</param>
        /// <param name="hashCode">The item hash code.</param>
        /// <param name="index">The item scan index.</param>
        /// <param name="vWin">Returns the vertex window containing the data.</param>
        /// <returns>Returns true if the item is found in the collection.</returns>
        protected bool FindAndLock(T item, int hashCode, int index, out VertexWindow<T> vWin)
        {
            int slotLocks1 = 0;
            int slotLocks2 = 0;

#if (PROFILING)
            int start = Environment.TickCount;
            int hopCount = 0;
            try
            {
#endif
                vWin = new VertexWindow<T>();

                //Lock the start index immediately.
                slotLocks1 = mSlots.ItemLock(index);
#if (PROFILING)
                hopCount++;
                if (slotLocks1 > 0)
                    ProfileHotspot(ProfileArrayType.Slots, index);
#endif
                vWin.SetCurrent(index + 1, mSlots[index]);

                while (true)
                {
#if (PROFILING)
                    hopCount++;
#endif
                    //If this is the last item in the list and does not contain data then exit.
                    if (vWin.Curr.IsTerminator)
                        return false;

                    //OK, lock the next item.
                    slotLocks2 = mSlots.ItemLock(vWin.Curr.NextIDPlus1 - 1);
#if (PROFILING)
                    if (slotLocks2 > 0)
                        ProfileHotspot(ProfileArrayType.Slots, vWin.Curr.NextIDPlus1 - 1);
#endif

                    vWin.Next = mSlots[vWin.Curr.NextIDPlus1 - 1];

                    //If the hashCode of the current item is greater than the search hashCode then exit,
                    //as we order by hashCode.
                    if (vWin.Next.HashCode > hashCode)
                    {
                        return false;
                    }

                    if (vWin.Next.IsSentinel ||
                        vWin.Next.HashCode != hashCode || !mEqualityComparer.Equals(item, vWin.Next.Value))
                    {
                        mSlots.ItemUnlock(vWin.CurrIndexPlus1 - 1);
                        vWin.MoveUp();
                        continue;
                    }

                    break;
                }

                return true;
#if (PROFILING)
            }
            finally
            {
                Profile(ProfileAction.Time_FindAndLock, Environment.TickCount - start);
                Profile(ProfileAction.Count_FindAndLockHopCount, hopCount);
                Profile(ProfileAction.Count_FindAndLockSlotLocks, slotLocks1 + slotLocks2);
            }
#endif
        }
        #endregion // ContainsScanInternal(T item, int hashCode, int index)
    }
}
