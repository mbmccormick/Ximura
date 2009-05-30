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
    public class LockFreeStack<T> : LockFreeCollectionBase<T>, IStack<T>
    {
        #region Constructor
        /// <summary>
        /// This is the default constructor. The collection will be constructed with a base capacity of 1000.
        /// </summary>
        public LockFreeStack() 
            : base(null, 1000, null, false) { }

        /// <summary>
        /// Initializes a new instance of the LockFreeCollection<(Of <(T>)>) class
        /// </summary>
        /// <param name="collection">The values in this enumeration will be loaded in to the collection.</param>
        public LockFreeStack(IEnumerable<T> collection) : base(null, 1000, collection, false) { }

        /// <summary>
        /// Initializes a new instance of the LockFreeCollection<(Of <(T>)>) class
        /// </summary>
        /// <param name="collection">The values in this enumeration will be loaded in to the collection.</param>
        /// <param name="isFixedSize">The collection is fixed to the size passed in the capacity parameter.</param>
        public LockFreeStack(IEnumerable<T> collection, bool isFixedSize) : base(null, isFixedSize ? -1 : 1000, collection, isFixedSize) { }

        /// <summary>
        /// Initializes a new instance of the LockFreeCollection<(Of <(T>)>) class
        /// </summary>
        /// <param name="collection">The values in this enumeration will be loaded in to the collection.</param>
        /// <param name="comparer">This is the comparer used to detect equality between items in the collection. 
        /// If this is set to null the default comparer for the type will be used instead./</param>
        public LockFreeStack(IEqualityComparer<T> comparer, IEnumerable<T> collection) 
            : base(comparer, 1000, collection, false) { }

        /// <summary>
        /// Initializes a new instance of the LockFreeCollection<(Of <(T>)>) class
        /// </summary>
        /// <param name="capacity">The collection initial capacity.</param>
        public LockFreeStack(int capacity)
            : base(null, capacity, null, false) { }
        /// <summary>
        /// Initializes a new instance of the LockFreeCollection<(Of <(T>)>) class
        /// </summary>
        /// <param name="capacity">The collection initial capacity.</param>
        /// <param name="isFixedSize">The collection is fixed to the size passed in the capacity parameter.</param>
        public LockFreeStack(int capacity, bool isFixedSize)
            : base(null, capacity, null, isFixedSize) { }

        /// <summary>
        /// Initializes a new instance of the LockFreeCollection<(Of <(T>)>) class
        /// </summary>
        /// <param name="comparer">This is the comparer used to detect equality between items in the collection. 
        /// If this is set to null the default comparer for the type will be used instead./</param>
        /// <param name="capacity">The collection initial capacity.</param>
        public LockFreeStack(IEqualityComparer<T> comparer, int capacity)
            : base(comparer, capacity, null, false) { }
        /// <summary>
        /// Initializes a new instance of the LockFreeCollection<(Of <(T>)>) class
        /// </summary>
        /// <param name="comparer">This is the comparer used to detect equality between items in the collection. 
        /// If this is set to null the default comparer for the type will be used instead./</param>
        /// <param name="capacity">The collection initial capacity.</param>
        /// <param name="isFixedSize">The collection is fixed to the size passed in the capacity parameter.</param>
        public LockFreeStack(IEqualityComparer<T> comparer, int capacity, bool isFixedSize)
            : base(comparer, capacity, null, isFixedSize) { }

        /// <summary>
        /// Initializes a new instance of the LockFreeCollection<(Of <(T>)>) class
        /// </summary>
        /// <param name="comparer">This is the comparer used to detect equality between items in the collection. 
        /// If this is set to null the default comparer for the type will be used instead./</param>
        /// <param name="capacity">The collection initial capacity.</param>
        /// <param name="collection">The values in this enumeration will be loaded in to the collection.</param>
        public LockFreeStack(IEqualityComparer<T> comparer, int capacity, IEnumerable<T> collection)
            : base(comparer, capacity, collection, false) { }
        /// <summary>
        /// Initializes a new instance of the LockFreeCollection<(Of <(T>)>) class
        /// </summary>
        /// <param name="comparer">This is the comparer used to detect equality between items in the collection. 
        /// If this is set to null the default comparer for the type will be used instead./</param>
        /// <param name="capacity">The collection initial capacity.</param>
        /// <param name="collection">The values in this enumeration will be loaded in to the collection.</param>
        /// <param name="isFixedSize">The collection is fixed to the size passed in the capacity parameter.</param>
        public LockFreeStack(IEqualityComparer<T> comparer, int capacity, IEnumerable<T> collection, bool isFixedSize)
            : base(comparer, capacity, collection, isFixedSize) { }
        #endregion // Constructor

        #region IStack<T> Members

        public void Push(T item)
        {
            throw new NotImplementedException();
        }

        public T Pop()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region ICollectionBase<T> Members

        public int Count
        {
            get { throw new NotImplementedException(); }
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(T item)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public T[] ToArray()
        {
            throw new NotImplementedException();
        }

        public void TrimExcess()
        {
            throw new NotImplementedException();
        }

        public T Peek()
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        #endregion

        #region ICollection Members

        public void CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }

        public bool IsSynchronized
        {
            get { throw new NotImplementedException(); }
        }

        public object SyncRoot
        {
            get { throw new NotImplementedException(); }
        }

        #endregion
    }
}
