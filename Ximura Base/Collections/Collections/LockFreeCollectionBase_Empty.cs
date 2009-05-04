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
                        int freeItem = freeSent.NextIDPlus1 - 1;
                        if (!mSlots.ItemTryLock(freeItem))
                            break;

                        //OK get the item.
                        Vertex<T> item = mSlots[freeSent.NextIDPlus1 - 1];

                        //OK, remove the free item from the list and set the sentinel to the next item.
                        mSlots[cnIndexEmptyQueue] = Vertex<T>.Sentinel(0, item.NextIDPlus1);

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
