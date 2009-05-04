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
    /// This class implements a red-black tree network of array parts. This ensures that there is an even depth to reach all of the 
    /// parts as the tree grows
    /// </summary>
    /// <typeparam name="T">The array type.</typeparam>
    public class ExpandableFineGrainedLockArray<T> : 
        LockFreeRedBlackTreeBase<int, FineGrainedLockArray<T>, ExpandableFineGrainedLockArrayVertex<T>>
    {
        #region Declarations
        private int mCapacity;
        private Func<int, int> fnCalculateAutogrow;
        #endregion // Declarations

        #region Constructor
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        public ExpandableFineGrainedLockArray() : this(1000) { }
        /// <summary>
        /// This is the default constructor that sets the initial capacity of the array.
        /// </summary>
        /// <param name="capacity">The initial capacity of the array.</param>
        public ExpandableFineGrainedLockArray(int capacity) : this(capacity, (c) => Prime.Get(c * 2)) { }
        /// <summary>
        /// This constructor sets the initial capacity and also provides a calculation function to calculate the growth of the array.
        /// </summary>
        /// <param name="capacity">The initial capacity of the array.</param>
        /// <param name="fnCalculateAutogrow">The growth function, if this is not set, the array will be of a fixed size.</param>
        public ExpandableFineGrainedLockArray(int capacity, Func<int, int> fnCalculateAutogrow)
        {
            this.fnCalculateAutogrow = fnCalculateAutogrow;

            IncreaseCapacity(capacity);
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
                return ResolveArray(index)[index];
            }
            set
            {
                ResolveArray(index)[index] = value;
            }
        }
        #endregion // this[int index]

        #region Length
        /// <summary>
        /// This is the current capacity of the array.
        /// </summary>
        public int Length
        {
            get { return mCapacity; }
        }
        #endregion // Length

        #region Item methods
        #region ItemLockWait(int index)
        /// <summary>
        /// This method waits for the lock to become available on the index.
        /// </summary>
        /// <param name="index">The index to wait for.</param>
        /// <returns>Returns the number of lock spins. If there is no wait it returns 0.</returns>
        public int ItemLockWait(int index)
        {
            return ResolveArray(index).ItemLockWait(index);
        }
        #endregion // ItemLockWait(int index)
        #region ItemTryLock
        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public bool ItemTryLock(int index)
        {
            return ResolveArray(index).ItemTryLock(index);
        }
        #endregion // ItemTryLock

        #region ItemIsLocked(int index)
        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public bool ItemIsLocked(int index)
        {
            return ResolveArray(index).ItemIsLocked(index);
        }
        #endregion // ItemIsLocked(int index)
        #region ItemLock(int index)
        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        public int ItemLock(int index)
        {
            return ResolveArray(index).ItemLock(index);
        }
        #endregion // ItemLock(int index)
        #region ItemUnlock(int index)
        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        public void ItemUnlock(int index)
        {
            ResolveArray(index).ItemUnlock(index);
        }
        #endregion // ItemUnlock(int index)
        #endregion // Item methods

        #region ResolveArray(int index)
        /// <summary>
        /// The method resolves the specific array within the tree structure. If the array does not exist, then one is created 
        /// adding the required capacity to the collection.
        /// </summary>
        /// <param name="index">The index of the array item.</param>
        /// <returns>Returns the array containing the index item.</returns>
        protected FineGrainedLockArray<T> ResolveArray(int index)
        {
            if (index < 0)
                throw new IndexOutOfRangeException("The index cannot be less that zero.");

            LockFreeRedBlackVertex<int, FineGrainedLockArray<T>> vertex = null;

            //Let's get the current version ID, we will need to check whis later.
            int currentVersionID = mVersionID;

            //Ok, we are going to loop until we have capacity.
            while (!mRoot.Search(index, out vertex))
            {
                if (currentVersionID == mVersionID)
                    IncreaseCapacity(index + 1);
                else
                    currentVersionID = mVersionID;
            }

            return vertex.Value;
        }
        #endregion // ResolveArray(int index)

        #region IncreaseCapacity()
        /// <summary>
        /// This method increases the capacity by the default amount.
        /// </summary>
        public void IncreaseCapacity()
        {
            IncreaseCapacity(mCapacity + fnCalculateAutogrow(mCapacity));
        }
        #endregion // IncreaseCapacity()
        #region IncreaseCapacity(int newCapacity)
        /// <summary>
        /// This method increases the capacity to the amount specified.
        /// </summary>
        /// <param name="newCapacity">The new capacity.</param>
        public void IncreaseCapacity(int newCapacity)
        {
            //Ok, check whether we have an autogrow calculator, and if so, adjust the growth amount.
            if (fnCalculateAutogrow != null)
                newCapacity = fnCalculateAutogrow(newCapacity);

            int additional = newCapacity - mCapacity;

            if (AddInternal(mCapacity, new FineGrainedLockArray<T>(additional, mCapacity)))
                Interlocked.Add(ref mCapacity, additional);
        }
        #endregion // IncreaseCapacity(int newCapacity)
    }
}