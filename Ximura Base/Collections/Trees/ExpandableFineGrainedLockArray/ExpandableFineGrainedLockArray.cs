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
        LockFreeRedBlackTreeBase<int, FineGrainedLockArray<T>, RedBlackTreeLockableVertex<int, FineGrainedLockArray<T>>>, ILockableMarkableArray<T>
    {
        #region Declarations
        private RedBlackTreeLockableVertex<int, FineGrainedLockArray<T>> initial;
        private int mCapacity;
        private Func<int, int, int> fnCalculateAutogrow;
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
        public ExpandableFineGrainedLockArray(int capacity) : this(capacity, (c, d) => Prime.Get(c * 2)) { }
        /// <summary>
        /// This constructor sets the initial capacity and also provides a calculation function to calculate the growth of the array.
        /// </summary>
        /// <param name="capacity">The initial capacity of the array.</param>
        /// <param name="fnCalculateAutogrow">The growth function, if this is not set, the array will be of a fixed size.</param>
        public ExpandableFineGrainedLockArray(int capacity, Func<int, int, int> fnCalculateAutogrow)
        {
            IncreaseCapacity(capacity);
            //We set this after the IncreaseCapacity call to ensure that the capacity is set specifically to the value passed.
            this.fnCalculateAutogrow = fnCalculateAutogrow;
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
        public int Count
        {
            get { return mCapacity; }
        }
        #endregion // Length

        #region ItemLockWait(int index)
        /// <summary>
        /// This method waits for the lock to become available on the index.
        /// </summary>
        /// <param name="index">The index to wait for.</param>
        /// <returns>Returns the number of lock spins. If there is no wait it returns 0.</returns>
        public void ItemLockWait(int index)
        {
            ResolveArray(index).ItemLockWait(index);
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
        public void ItemLock(int index)
        {
            ResolveArray(index).ItemLock(index);
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

            //OK, we a shorcut test on the last item added to the collection. Usually this is the largest item so we will check
            //it first. 
            RedBlackTreeLockableVertex<int, FineGrainedLockArray<T>> vertex = initial;
            if (vertex != null && Compare(index, vertex) == 0)
                return vertex.Value;

            TreeTraversalWindow<int, FineGrainedLockArray<T>> window;

            //Let's get the current version ID, we will need to check whis later.
            int currentVersionID = mVersion;

            //Ok, we are going to loop until we have capacity.
            while (!FindInternal(index, false, out window))
            {
                if (currentVersionID == mVersion)
                    IncreaseCapacity(index + 1);
                else
                    currentVersionID = mVersion;
            }

            return window.Current.Value;
        }
        #endregion // ResolveArray(int index) 

        #region IncreaseCapacity(int newCapacity)
        /// <summary>
        /// This method increases the capacity to the amount specified.
        /// </summary>
        /// <param name="newCapacity">The new capacity.</param>
        public void IncreaseCapacity(int newCapacity)
        {
            //Ok, check whether we have an autogrow calculator, and if so, adjust the growth amount.
            if (fnCalculateAutogrow != null)
                newCapacity = fnCalculateAutogrow(newCapacity, mCapacity);

            int additional = newCapacity - mCapacity;

            if (additional <= 0)
                throw new ArgumentOutOfRangeException("Array cannot be grown.");

            throw new NotSupportedException();
            //RedBlackTreeLockableVertex<int, FineGrainedLockArray<T>> vertex = new RedBlackTreeLockableVertex<int, FineGrainedLockArray<T>>();

            //vertex = new RedBlackTreeLockableVertex<int, FineGrainedLockArray<T>>();
            //vertex.Key = mCapacity;
            //vertex.Value = new FineGrainedLockArray<T>(additional, mCapacity);


            //if (AddInternal(vertex))
            //{
            //    Interlocked.Add(ref mCapacity, additional);
            //    Interlocked.Exchange<RedBlackTreeLockableVertex<int, FineGrainedLockArray<T>>>(ref initial, vertex);
            //}
        }
        #endregion // IncreaseCapacity(int newCapacity)

        #region Comparer(LockFreeRedBlackVertex<int, FineGrainedLockArray<T>> vertex, int key)
        /// <summary>
        /// This is the specific comparer for the Expandable array.
        /// </summary>
        /// <param name="vertex">The lock array vertex.</param>
        /// <param name="key">The position key.</param>
        /// <returns>
        /// Returns 0 if the key is equal to the vertex. 
        /// Returns -1 if the key is less than the vertex, and returns 1 if the key is greater than the vertex.
        /// </returns>
        protected override int Compare(int key, RedBlackTreeLockableVertex<int, FineGrainedLockArray<T>> vertex)
        {
            //Ok check if the key is lower than this key.
            if (key < vertex.Key)
                return -1;
            //Is the key within the range contained within this collection, yes then this is a match.
            if (key < vertex.Key + vertex.Value.Count)
                return 0;
            //Key must be greater.
            return 1;
        }
        #endregion // Comparer(LockFreeRedBlackVertex<int, FineGrainedLockArray<T>> vertex, int key)

        #region IFineGrainedLockArray<T> Members

        public LockableWrapper<T> LockableData(int index)
        {
            return ResolveArray(index).LockableData(index); 
        }

        #endregion
    }
}