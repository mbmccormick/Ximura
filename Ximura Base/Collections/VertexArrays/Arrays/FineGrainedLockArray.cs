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
using System.Runtime.Serialization;
using System.Runtime.InteropServices;
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
    public class FineGrainedLockArray<T> : ILockableMarkableArray<T>
    {
        #region Declarations
        private LockableWrapper<T>[] mArray;
        #endregion // Declarations
        #region Constructor
        /// <summary>
        /// This constructor sets the array capacity and the array offset integer.
        /// </summary>
        /// <param name="capacity">The array capacity.</param>
        public FineGrainedLockArray(int capacity)
        {
            if (capacity < 0)
                throw new ArgumentOutOfRangeException("capacity", "Capacity cannot be less than zero.");

            mArray = new LockableWrapper<T>[capacity];
        }
        #endregion // Constructor

        #region this[int index]
        /// <summary>
        /// This is the indexer for the array.
        /// </summary>
        /// <param name="index">The index position.</param>
        /// <returns>Returns the object corresponding to the index position.</returns>
        public T this[int index]
        {
            get
            {
                return mArray[index].Value;
            }
            set
            {
                mArray[index].Value = value;
            }
        }
        #endregion // this[int index]

        #region Count
        /// <summary>
        /// This is the capacity of the array.
        /// </summary>
        public int Count
        {
            get { return mArray.Length; }
        }
        #endregion // Length

        #region ItemIsLocked(int index)
        /// <summary>
        /// This method checks whether an item in the collection is locked.
        /// </summary>
        /// <param name="index">The index of the item to check.</param>
        /// <returns>Returns true if the item is locked.</returns>
        public bool ItemIsLocked(int index)
        {
            return mArray[index].IsLocked;
        }
        #endregion // ItemIsLocked(int index)
        #region ItemLockWait(int index)
        /// <summary>
        /// This method waits for a locked item to become available.
        /// </summary>
        /// <param name="index">The index of the item to wait for.</param>
        public void ItemLockWait(int index)
        {
            mArray[index].LockWait();
        }
        #endregion
        #region ItemLock(int index)
        /// <summary>
        /// This method locks the specific item.
        /// </summary>
        /// <param name="index">The item index.</param>
        public void ItemLock(int index)
        {
            mArray[index].Lock();
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
            return mArray[index].TryLock();
        }
        #endregion // ItemTryLock
        #region ItemUnlock(int index)
        /// <summary>
        /// The method unlocks the item.
        /// </summary>
        /// <param name="index">The index of the item you wish to unlock.</param>
        public void ItemUnlock(int index)
        {
            mArray[index].Unlock();
        }
        #endregion // ItemUnlock(int index)

        #region ToString()
        /// <summary>
        /// This is the debug data.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("FGLA={0}", mArray.Length);
        }
        #endregion

        #region Resize(int newCapacity)
        /// <summary>
        /// This method changes the size of the array.
        /// </summary>
        /// <param name="newCapacity">The new capacity.</param>
        protected virtual void Resize(int newCapacity)
        {
            if (newCapacity < 1)
                throw new ArgumentOutOfRangeException("newCapacity", "The capacity cannot be less than 1.");

            if (newCapacity == mArray.Length)
                return;

            LockableWrapper<T>[] newArray = new LockableWrapper<T>[newCapacity];

            int copyCapacity = newCapacity < mArray.Length ? newCapacity : mArray.Length;

            Array.Copy(mArray, 0, newArray, 0, copyCapacity);

            mArray = newArray;
        }
        #endregion // Resize(int newCapacity)

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public virtual LockableWrapper<T> LockableData(int index)
        {
            return mArray[index];
        }
    }
}
