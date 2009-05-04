﻿#region Copyright
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
    public class LockFreeCollection<T> : LockFreeCollectionBase<T>, ICollection<T>
    {
        #region Constructor
        /// <summary>
        /// This is the default constructor. The collection will be constructed with a base capacity of 1000.
        /// </summary>
        public LockFreeCollection() : base(null, 1000, null) { }
        /// <summary>
        /// Initializes a new instance of the LockFreeCollection<(Of <(T>)>) class
        /// </summary>
        /// <param name="collection">The values in this enumeration will be loaded in to the collection.</param>
        /// <param name="comparer">This is the comparer used to detect equality between items in the collection. 
        /// If this is set to null the default comparer for the type will be used instead./</param>
        public LockFreeCollection(IEnumerable<T> collection, EqualityComparer<T> comparer) : base(comparer, 1000, collection) { }
        /// <summary>
        /// Initializes a new instance of the LockFreeCollection<(Of <(T>)>) class
        /// </summary>
        /// <param name="collection">The values in this enumeration will be loaded in to the collection.</param>
        /// <param name="capacity">The collection initial capacity.</param>
        /// <param name="comparer">This is the comparer used to detect equality between items in the collection. 
        /// If this is set to null the default comparer for the type will be used instead./</param>
        public LockFreeCollection(IEnumerable<T> collection, int capacity, EqualityComparer<T> comparer) : base(comparer, capacity, collection) { }
        /// <summary>
        /// Initializes a new instance of the LockFreeCollection<(Of <(T>)>) class
        /// </summary>
        /// <param name="collection">The values in this enumeration will be loaded in to the collection.</param>
        public LockFreeCollection(IEnumerable<T> collection) : base(null, 1000, collection) { }
        /// <summary>
        /// Initializes a new instance of the LockFreeCollection<(Of <(T>)>) class
        /// </summary>
        /// <param name="capacity">The collection initial capacity.</param>
        public LockFreeCollection(int capacity) : base(null, capacity, null) { }
        /// <summary>
        /// Initializes a new instance of the LockFreeCollection<(Of <(T>)>) class
        /// </summary>
        /// <param name="capacity">The collection initial capacity.</param>
        /// <param name="comparer">This is the comparer used to detect equality between items in the collection. 
        /// If this is set to null the default comparer for the type will be used instead./</param>
        public LockFreeCollection(int capacity, EqualityComparer<T> comparer) : base(comparer, capacity, null) { }
        #endregion // Constructor

        #region Add(T item)
        /// <summary>
        /// Adds and item to the collection.
        /// </summary>
        /// <param name="item">The item to add.</param>
        public void Add(T item)
        {
            AddInternal(item);
        }
        #endregion // Add(T item)

        #region Remove(T item)
        /// <summary>
        /// Removes an item from the collection.
        /// </summary>
        /// <param name="item">The item to remove.</param>
        /// <returns>Returns true if the item is successfully removed.</returns>
        public bool Remove(T item)
        {
            return RemoveInternal(item);
        }
        #endregion // Remove(T item)

        #region Clear()
        /// <summary>
        /// This method clears the collection.
        /// </summary>
        public void Clear()
        {
            ClearInternal();
        }
        #endregion // Clear()

        #region Contains(T item)
        /// <summary>
        /// This method checks whether an item exists in the collection.
        /// </summary>
        /// <param name="item">The item to check.</param>
        /// <returns>Returns true if the item exists in the collection.</returns>
        public bool Contains(T item)
        {
            return ContainsInternal(item);
        }
        #endregion // Contains(T item)

        #region CopyTo(T[] array, int arrayIndex)
        /// <summary>
        /// This method copies the collection to the array specified.
        /// </summary>
        /// <param name="array">The array to copy to.</param>
        /// <param name="arrayIndex">The array index where the class should start copying to.</param>
        public void CopyTo(T[] array, int arrayIndex)
        {
            CopyToInternal(array, arrayIndex);
        }
        #endregion // CopyTo(T[] array, int arrayIndex)

        #region Count
        /// <summary>
        /// This property returns the number of elements in the collection.
        /// </summary>
        public int Count
        {
            get
            {
                return CountInternal;
            }
        }
        #endregion // Count

        #region IsReadOnly
        /// <summary>
        /// This property always returns false.
        /// </summary>
        public bool IsReadOnly
        {
            get
            {
                DisposedCheck();
                return false;
            }
        }
        #endregion // IsReadOnly
    }
}