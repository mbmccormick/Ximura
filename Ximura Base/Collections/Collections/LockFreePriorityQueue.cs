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
    public class LockFreePriorityQueue<T> : LockFreeCollectionBase<T>, IPriorityQueue<T>
    {
        #region Constructor
        /// <summary>
        /// This is the default constructor. The collection will be constructed with a base capacity of 1000.
        /// </summary>
        public LockFreePriorityQueue() 
            : base(null, 1000, null, false) { }

        /// <summary>
        /// Initializes a new instance of the LockFreeCollection<(Of <(T>)>) class
        /// </summary>
        /// <param name="collection">The values in this enumeration will be loaded in to the collection.</param>
        public LockFreePriorityQueue(IEnumerable<T> collection) : base(null, 1000, collection, false) { }

        /// <summary>
        /// Initializes a new instance of the LockFreeCollection<(Of <(T>)>) class
        /// </summary>
        /// <param name="collection">The values in this enumeration will be loaded in to the collection.</param>
        /// <param name="isFixedSize">The collection is fixed to the size passed in the capacity parameter.</param>
        public LockFreePriorityQueue(IEnumerable<T> collection, bool isFixedSize) : base(null, isFixedSize ? -1 : 1000, collection, isFixedSize) { }

        /// <summary>
        /// Initializes a new instance of the LockFreeCollection<(Of <(T>)>) class
        /// </summary>
        /// <param name="collection">The values in this enumeration will be loaded in to the collection.</param>
        /// <param name="comparer">This is the comparer used to detect equality between items in the collection. 
        /// If this is set to null the default comparer for the type will be used instead./</param>
        public LockFreePriorityQueue(IEqualityComparer<T> comparer, IEnumerable<T> collection) 
            : base(comparer, 1000, collection, false) { }

        /// <summary>
        /// Initializes a new instance of the LockFreeCollection<(Of <(T>)>) class
        /// </summary>
        /// <param name="capacity">The collection initial capacity.</param>
        public LockFreePriorityQueue(int capacity)
            : base(null, capacity, null, false) { }
        /// <summary>
        /// Initializes a new instance of the LockFreeCollection<(Of <(T>)>) class
        /// </summary>
        /// <param name="capacity">The collection initial capacity.</param>
        /// <param name="isFixedSize">The collection is fixed to the size passed in the capacity parameter.</param>
        public LockFreePriorityQueue(int capacity, bool isFixedSize)
            : base(null, capacity, null, isFixedSize) { }

        /// <summary>
        /// Initializes a new instance of the LockFreeCollection<(Of <(T>)>) class
        /// </summary>
        /// <param name="comparer">This is the comparer used to detect equality between items in the collection. 
        /// If this is set to null the default comparer for the type will be used instead./</param>
        /// <param name="capacity">The collection initial capacity.</param>
        public LockFreePriorityQueue(IEqualityComparer<T> comparer, int capacity)
            : base(comparer, capacity, null, false) { }
        /// <summary>
        /// Initializes a new instance of the LockFreeCollection<(Of <(T>)>) class
        /// </summary>
        /// <param name="comparer">This is the comparer used to detect equality between items in the collection. 
        /// If this is set to null the default comparer for the type will be used instead./</param>
        /// <param name="capacity">The collection initial capacity.</param>
        /// <param name="isFixedSize">The collection is fixed to the size passed in the capacity parameter.</param>
        public LockFreePriorityQueue(IEqualityComparer<T> comparer, int capacity, bool isFixedSize)
            : base(comparer, capacity, null, isFixedSize) { }

        /// <summary>
        /// Initializes a new instance of the LockFreeCollection<(Of <(T>)>) class
        /// </summary>
        /// <param name="comparer">This is the comparer used to detect equality between items in the collection. 
        /// If this is set to null the default comparer for the type will be used instead./</param>
        /// <param name="capacity">The collection initial capacity.</param>
        /// <param name="collection">The values in this enumeration will be loaded in to the collection.</param>
        public LockFreePriorityQueue(IEqualityComparer<T> comparer, int capacity, IEnumerable<T> collection)
            : base(comparer, capacity, collection, false) { }
        /// <summary>
        /// Initializes a new instance of the LockFreeCollection<(Of <(T>)>) class
        /// </summary>
        /// <param name="comparer">This is the comparer used to detect equality between items in the collection. 
        /// If this is set to null the default comparer for the type will be used instead./</param>
        /// <param name="capacity">The collection initial capacity.</param>
        /// <param name="collection">The values in this enumeration will be loaded in to the collection.</param>
        /// <param name="isFixedSize">The collection is fixed to the size passed in the capacity parameter.</param>
        public LockFreePriorityQueue(IEqualityComparer<T> comparer, int capacity, IEnumerable<T> collection, bool isFixedSize)
            : base(comparer, capacity, collection, isFixedSize) { }
        #endregion // Constructor

        #region Enqueue(T item)
        /// <summary>
        /// This method enqueues an item in to the collection with the highest priority.
        /// </summary>
        /// <param name="item">The item to enqueue.</param>
        public void Enqueue(T item)
        {
            Enqueue(item, 0F);
        }
        #endregion // Enqueue(T item)
        #region Enqueue(T item, float priority)
        /// <summary>
        /// This method enqueues an item in to the collection with the priority specified.
        /// </summary>
        /// <param name="item">The item to enqueue.</param>
        /// <param name="priority">The priority of the item to enqueue.</param>
        public void Enqueue(T item, float priority)
        {
            if (priority < 0)
                throw new ArgumentOutOfRangeException("The priority cannot be less than zero.");

            throw new NotImplementedException();
        }
        #endregion // Enqueue(T item, float priority)

        #region Dequeue()
        /// <summary>
        /// This method dequeues the item from the queue with the highest priority.
        /// </summary>
        /// <returns>Returns an item from the queue.</returns>
        /// <exception cref="System.InvalidOperationException">This exception is thrown if the queue is empty.</exception>
        public T Dequeue()
        {
            T item;
            if (!TryDequeue(out item))
                throw new InvalidOperationException("The count is zero.");

            return item;
        }
        #endregion // TryDequeue(out T item)
        #region TryDequeue(out T item)
        /// <summary>
        /// This method attempt to remove an item from the queue with the highest priority.
        /// </summary>
        /// <param name="item">The item returned from the queue.</param>
        /// <returns>Returns true if an item is returned.</returns>
        public bool TryDequeue(out T item)
        {
            throw new NotImplementedException();
        }
        #endregion // TryDequeue(out T item)

        #region Peek()
        /// <summary>
        /// This method returns an item from the head of the queue without removing the item.
        /// </summary>
        /// <returns>Returns an item from the queue.</returns>
        public T Peek()
        {
            T item;
            if (!TryPeek(out item))
                throw new InvalidOperationException("The count is zero.");

            return item;
        }
        #endregion
        #region TryPeek(out T item)
        /// <summary>
        /// This method attempts to return an item from the head of the queue without removing the item.
        /// </summary>
        /// <param name="item">The item returned from the queue.</param>
        /// <returns>Returns true if an item is returned.</returns>
        public bool TryPeek(out T item)
        {
            throw new NotImplementedException();
        }
        #endregion // TryPeek(out T item)

        #region Contains(T item)
        /// <summary>
        /// This method checks whether an item exists in the collection.
        /// </summary>
        /// <param name="item">The item to check.</param>
        /// <returns>Returns true if the item exists in the collection.</returns>
        public bool Contains(T item)
        {
            DisposedCheck();
            return ContainsInternal(item);
        }
        #endregion // Contains(T item)

        #region Clear()
        /// <summary>
        /// This method clears the collection.
        /// </summary>
        public void Clear()
        {
            DisposedCheck();
            ClearInternal();
        }
        #endregion // Clear()
        #region Count
        /// <summary>
        /// The collection count.
        /// </summary>
        public int Count
        {
            get { return CountInternal; }
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
            CopyToInternal(array, arrayIndex);
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
            CopyToInternal(array, arrayIndex);
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
            return ToArrayInternal();
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

        #region AddIncomingData(IEnumerable<T> collection)
        /// <summary>
        /// This method enqueues the incoming data to the collection that was passed in the constructor.
        /// </summary>
        /// <param name="collection">The data to enqueue.</param>
        protected override void AddIncomingData(IEnumerable<T> collection)
        {
            collection.ForEach(i => Enqueue(i));
        }
        #endregion // AddIncomingData(IEnumerable<T> collection)
    }
}
