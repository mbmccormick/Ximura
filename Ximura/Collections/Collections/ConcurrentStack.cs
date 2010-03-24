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
using System.Security.Permissions;
using System.Threading;

using Ximura;

#endregion // using
namespace Ximura.Collections
{
    /// <summary>
    /// This class is an implementation of a concurrent scalable stack.
    /// </summary>
    /// <typeparam name="T">The item type for the stack.</typeparam>
    [DebuggerDisplay("Count = {Count}"), HostProtection(SecurityAction.LinkDemand, MayLeakOnAbort = true)]
    public class ConcurrentStack<T> : ConcurrentTreeBase, IStack<T>
    {
        #region Declarations
        /// <summary>
        /// The version value.
        /// </summary>
        private volatile int mVersion;
        /// <summary>
        /// This is the current item count.
        /// </summary>
        private volatile int mCount;
        /// <summary>
        /// This property determines whether the collection is a fixed size. Fixed size collections will reject new records
        /// when the capacity has been reached.
        /// </summary>
        private bool mIsFixedSize;

        /// <summary>
        /// This is the equality comparer for the collection.
        /// </summary>
        private IEqualityComparer<T> mEqualityComparer;
        #endregion // Declarations
        #region Constructor
        /// <summary>
        /// This is the default constructor. The collection will be constructed with a base capacity of 1000.
        /// </summary>
        public ConcurrentStack()
            : this(null, 1000, null, false) { }

        /// <summary>
        /// Initializes a new instance of the LockFreeCollection<(Of <(T>)>) class
        /// </summary>
        /// <param name="collection">The values in this enumeration will be loaded in to the collection.</param>
        public ConcurrentStack(IEnumerable<T> collection) : this(null, 1000, collection, false) { }

        /// <summary>
        /// Initializes a new instance of the LockFreeCollection<(Of <(T>)>) class
        /// </summary>
        /// <param name="collection">The values in this enumeration will be loaded in to the collection.</param>
        /// <param name="isFixedSize">The collection is fixed to the size passed in the capacity parameter.</param>
        public ConcurrentStack(IEnumerable<T> collection, bool isFixedSize) : this(null, isFixedSize ? -1 : 1000, collection, isFixedSize) { }

        /// <summary>
        /// Initializes a new instance of the LockFreeCollection<(Of <(T>)>) class
        /// </summary>
        /// <param name="collection">The values in this enumeration will be loaded in to the collection.</param>
        /// <param name="comparer">This is the comparer used to detect equality between items in the collection. 
        /// If this is set to null the default comparer for the type will be used instead./</param>
        public ConcurrentStack(IEqualityComparer<T> comparer, IEnumerable<T> collection)
            : this(comparer, 1000, collection, false) { }

        /// <summary>
        /// Initializes a new instance of the LockFreeCollection<(Of <(T>)>) class
        /// </summary>
        /// <param name="capacity">The collection initial capacity.</param>
        public ConcurrentStack(int capacity)
            : this(null, capacity, null, false) { }
        /// <summary>
        /// Initializes a new instance of the LockFreeCollection<(Of <(T>)>) class
        /// </summary>
        /// <param name="capacity">The collection initial capacity.</param>
        /// <param name="isFixedSize">The collection is fixed to the size passed in the capacity parameter.</param>
        public ConcurrentStack(int capacity, bool isFixedSize)
            : this(null, capacity, null, isFixedSize) { }

        /// <summary>
        /// Initializes a new instance of the LockFreeCollection<(Of <(T>)>) class
        /// </summary>
        /// <param name="comparer">This is the comparer used to detect equality between items in the collection. 
        /// If this is set to null the default comparer for the type will be used instead./</param>
        /// <param name="capacity">The collection initial capacity.</param>
        public ConcurrentStack(IEqualityComparer<T> comparer, int capacity)
            : this(comparer, capacity, null, false) { }
        /// <summary>
        /// Initializes a new instance of the LockFreeCollection<(Of <(T>)>) class
        /// </summary>
        /// <param name="comparer">This is the comparer used to detect equality between items in the collection. 
        /// If this is set to null the default comparer for the type will be used instead./</param>
        /// <param name="capacity">The collection initial capacity.</param>
        /// <param name="isFixedSize">The collection is fixed to the size passed in the capacity parameter.</param>
        public ConcurrentStack(IEqualityComparer<T> comparer, int capacity, bool isFixedSize)
            : this(comparer, capacity, null, isFixedSize) { }

        /// <summary>
        /// Initializes a new instance of the LockFreeCollection<(Of <(T>)>) class
        /// </summary>
        /// <param name="comparer">This is the comparer used to detect equality between items in the collection. 
        /// If this is set to null the default comparer for the type will be used instead./</param>
        /// <param name="capacity">The collection initial capacity.</param>
        /// <param name="collection">The values in this enumeration will be loaded in to the collection.</param>
        public ConcurrentStack(IEqualityComparer<T> comparer, int capacity, IEnumerable<T> collection)
            : this(comparer, capacity, collection, false) { }
        /// <summary>
        /// Initializes a new instance of the LockFreeCollection<(Of <(T>)>) class
        /// </summary>
        /// <param name="comparer">This is the comparer used to detect equality between items in the collection. 
        /// If this is set to null the default comparer for the type will be used instead./</param>
        /// <param name="capacity">The collection initial capacity.</param>
        /// <param name="collection">The values in this enumeration will be loaded in to the collection.</param>
        /// <param name="isFixedSize">The collection is fixed to the size passed in the capacity parameter.</param>
        public ConcurrentStack(IEqualityComparer<T> comparer, int capacity, IEnumerable<T> collection, bool isFixedSize)
        {
//#if (PROFILING)
//            ProfilingSetup();
//#endif
            mCount = 0;
            mVersion = int.MinValue;

            mEqualityComparer = (comparer == null) ? EqualityComparer<T>.Default : comparer;

            if (collection != null)
                AddIncomingData(collection);
        }
        #endregion // Constructor

        #region AddIncomingData(IEnumerable<T> collection)
        /// <summary>
        /// This override pushes the data passed in the constructor to the stack.
        /// </summary>
        /// <param name="collection">The incoming data.</param>
        protected virtual void AddIncomingData(IEnumerable<T> collection)
        {
            collection.ForEach(i => Push(i));
        }
        #endregion // AddIncomingData(IEnumerable<T> collection)

        protected override void Dispose(bool disposing)
        {

        }

        #region Push(T item)
        /// <summary>
        /// This method pushes an item to the top of the stack.
        /// </summary>
        /// <param name="item">The item to add to the stack.</param>
        public void Push(T item)
        {
            DisposedCheck();
            throw new NotImplementedException();
        }
        #endregion // Push(T item)

        #region TryPop(out T item)
        /// <summary>
        /// This method removes an item from the top of the stack.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool TryPop(out T item)
        {
            DisposedCheck();
            if (mCount == 0)
            {
                item = default(T);
                return false;
            }

            throw new NotImplementedException();
        }
        #endregion // TryPop(out T item)
        #region Pop()
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public T Pop()
        {
            T item;
            if (!TryPop(out item))
                throw new InvalidOperationException("The count is zero.");

            return item;
        }
        #endregion // Pop()

        #region TryPeek(out T item)
        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool TryPeek(out T item)
        {
            DisposedCheck();
            if (mCount == 0)
            {
                item = default(T);
                return false;
            }

            throw new NotImplementedException();
        }
        #endregion // TryPeek(out T item)
        #region Peek()
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public T Peek()
        {
            T item;
            if (!TryPeek(out item))
                throw new InvalidOperationException("The count is zero.");

            return item;
        }
        #endregion // Peek()

        #region Contains(T item)
        /// <summary>
        /// This method checks whether an item exists in the collection.
        /// </summary>
        /// <param name="item">The item to check.</param>
        /// <returns>Returns true if the item exists in the collection.</returns>
        public bool Contains(T item)
        {
            DisposedCheck();
            throw new NotImplementedException();
        }
        #endregion // Contains(T item)

        #region Clear()
        /// <summary>
        /// This method clears the collection.
        /// </summary>
        public void Clear()
        {
            DisposedCheck();
            throw new NotImplementedException();
        }
        #endregion // Clear()
        #region Count
        /// <summary>
        /// The collection count.
        /// </summary>
        public int Count
        {
            get { return mCount; }
        }
        #endregion // Count

        #region CopyTo(T[] array, int arrayIndex)
        /// <summary>
        /// This method copies the collection to the array specified.
        /// </summary>
        /// <param name="array">The array to copy to.</param>
        /// <param name="arrayIndex">The array index where the class should start copying to.</param>
        public void CopyTo(T[] array, int arrayIndex)
        {
            DisposedCheck();
            throw new NotImplementedException();
        }
        #endregion // CopyTo(T[] array, int arrayIndex)
        #region CopyTo(Array array, int arrayIndex)
        /// <summary>
        /// This method copies the collection to the array specified.
        /// </summary>
        /// <param name="array">The array to copy to.</param>
        /// <param name="arrayIndex">The array index where the class should start copying to.</param>
        public void CopyTo(Array array, int arrayIndex)
        {
            DisposedCheck();
            throw new NotImplementedException();
        }
        #endregion // CopyTo(Array array, int arrayIndex)
        #region ToArray()
        /// <summary>
        /// Returns the data in the collection as an array.
        /// </summary>
        /// <returns>Returns an array of data.</returns>
        public T[] ToArray()
        {
            DisposedCheck();
            throw new NotImplementedException();
        }
        #endregion // ToArray()

        #region ICollection not-supported Members
        /// <summary>
        /// This method is not supported.
        /// </summary>
        public void TrimExcess()
        {
            throw new NotSupportedException();
        }
        /// <summary>
        /// This property is not supported.
        /// </summary>
        public bool IsSynchronized
        {
            get { throw new NotSupportedException(); }
        }
        /// <summary>
        /// This property is not supported.
        /// </summary>
        public object SyncRoot
        {
            get { throw new NotSupportedException(); }
        }
        #endregion

        #region IEnumerable<T> Members

        public IEnumerator<T> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        #endregion
        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator)GetEnumerator();
        }

        #endregion
    }
}
