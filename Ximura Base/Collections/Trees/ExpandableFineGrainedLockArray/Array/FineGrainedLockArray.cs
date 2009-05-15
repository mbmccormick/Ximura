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
using System.Runtime.Serialization;
using System.Threading;

using Ximura;
using Ximura.Helper;
#endregion // using
namespace Ximura.Collections
{
    /// <summary>
    /// This fine grained lock array is an array with the added ability to mark individual items as locked.
    /// </summary>
    /// <typeparam name="T">The array type.</typeparam>
    public class FineGrainedLockArray<T>
    {
        #region Declarations
        private T[] mArray;
        private int[] mArrayLocks;

        private int mCapacity;
        private int mOffset;
        #endregion // Declarations

        #region Constructor
        /// <summary>
        /// This constructor sets the array capacity and the array offset integer.
        /// </summary>
        /// <param name="capacity">The array capacity.</param>
        /// <param name="offset">The array offset, this is the initial position of the array, the default should be 0.</param>
        public FineGrainedLockArray(int capacity, int offset)
        {
            if (offset < 0)
                throw new ArgumentOutOfRangeException("offset", "Offset cannot be less than zero.");

            if (capacity < 0)
                throw new ArgumentOutOfRangeException("capacity", "Capacity cannot be less than zero.");

            mOffset = offset;
            mCapacity = capacity;
            mArray = new T[capacity];
            mArrayLocks = new int[capacity];
        }
        #endregion // Constructor

        #region this[int index]
        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public T this[int index]
        {
            get
            {
                return mArray[index - mOffset];
            }
            set
            {
                mArray[index - mOffset] = value;
            }
        }
        #endregion // this[int index]

        #region Length
        /// <summary>
        /// This is the capacity of the array.
        /// </summary>
        public int Length
        {
            get { return mCapacity; }
        }
        #endregion // Length
        #region Offset
        /// <summary>
        /// This is the offset used in calculation when retrieving items from the array.
        /// </summary>
        public int Offset
        {
            get { return mOffset; }
        }
        #endregion // Offset

        #region ItemIsLocked(int index)
        /// <summary>
        /// This method checks whether an item in the collection is locked.
        /// </summary>
        /// <param name="index">The index of the item to check.</param>
        /// <returns>Returns true if the item is locked.</returns>
        public bool ItemIsLocked(int index)
        {
            return mArrayLocks[index - mOffset] == 1;
        }
        #endregion // ItemIsLocked(int index)
        #region ItemLockWait(int index)
        /// <summary>
        /// This method waits for a locked item to become available.
        /// </summary>
        /// <param name="index">The index of the item to wait for.</param>
        /// <returns>Returns the number of lock cycles during the wait.</returns>
        public int ItemLockWait(int index)
        {
            int lockLoops = 0;
            while (ItemIsLocked(index))
            {
                lockLoops++;
                Threading.ThreadWait();
            }

            return lockLoops;
        }
        #endregion
        #region ItemLock(int index)
        /// <summary>
        /// This method locks the specific item.
        /// </summary>
        /// <param name="index">The item index.</param>
        /// <returns>Returns the number of lock cycles the thread entered.</returns>
        public int ItemLock(int index)
        {
            int lockLoops = 0;

            while (!ItemTryLock(index))
            {
                lockLoops++;
                Threading.ThreadWait();
            }

            return lockLoops;
        }
        #endregion // ItemLock(int index)
        #region ItemTryLock
        /// <summary>
        /// This method attempts to lock the item specified.
        /// </summary>
        /// <param name="index">The index of the item you wish to lock..</param>
        /// <returns>Returns true if the item was successfully locked.</returns>
        public bool ItemTryLock(int index)
        {
            int id = index - mOffset;
            return Interlocked.CompareExchange(ref mArrayLocks[id], Thread.CurrentThread.ManagedThreadId + 1, 0) == 0;
        }
        #endregion // ItemTryLock
        #region ItemUnlock(int index)
        /// <summary>
        /// The method unlocks the item.
        /// </summary>
        /// <param name="index">The index of the item you wish to unlock.</param>
        public void ItemUnlock(int index)
        {
            int id = index - mOffset;
            mArrayLocks[id] = 0;
        }
        #endregion // ItemUnlock(int index)

        #region ToString()
        /// <summary>
        /// This is the debug data.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("FGLA={0}<{1}", mOffset, mOffset + mCapacity);
        }
        #endregion
    }
}
