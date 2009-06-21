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
using Ximura.Helper;
#endregion // using
namespace Ximura.Collections
{
    /// <summary>
    /// This is the concurrent skiplist class.
    /// </summary>
    /// <typeparam name="T">The collection object type.</typeparam>
    [DebuggerDisplay("Count = {Count}"), HostProtection(SecurityAction.LinkDemand, MayLeakOnAbort = true)]
    public class LockFreeSkipList<T> : LockFreeSkipListBase<T>, ICollection<T>
    {
        #region Constructor
        /// <summary>
        /// This is the default constructor. The collection will be constructed with a base capacity of 1000.
        /// </summary>
        public LockFreeSkipList()
            : base(null, 1000, null, false) { }

        /// <summary>
        /// Initializes a new instance of the LockFreeCollection<(Of <(T>)>) class
        /// </summary>
        /// <param name="collection">The values in this enumeration will be loaded in to the collection.</param>
        public LockFreeSkipList(IEnumerable<T> collection) : base(null, 1000, collection, false) { }

        /// <summary>
        /// Initializes a new instance of the LockFreeCollection<(Of <(T>)>) class
        /// </summary>
        /// <param name="collection">The values in this enumeration will be loaded in to the collection.</param>
        /// <param name="isFixedSize">The collection is fixed to the size passed in the capacity parameter.</param>
        public LockFreeSkipList(IEnumerable<T> collection, bool isFixedSize) : base(null, isFixedSize ? -1 : 1000, collection, isFixedSize) { }

        /// <summary>
        /// Initializes a new instance of the LockFreeCollection<(Of <(T>)>) class
        /// </summary>
        /// <param name="collection">The values in this enumeration will be loaded in to the collection.</param>
        /// <param name="comparer">This is the comparer used to detect equality between items in the collection. 
        /// If this is set to null the default comparer for the type will be used instead./</param>
        public LockFreeSkipList(IEqualityComparer<T> comparer, IEnumerable<T> collection)
            : base(comparer, 1000, collection, false) { }

        /// <summary>
        /// Initializes a new instance of the LockFreeCollection<(Of <(T>)>) class
        /// </summary>
        /// <param name="capacity">The collection initial capacity.</param>
        public LockFreeSkipList(int capacity)
            : base(null, capacity, null, false) { }
        /// <summary>
        /// Initializes a new instance of the LockFreeCollection<(Of <(T>)>) class
        /// </summary>
        /// <param name="capacity">The collection initial capacity.</param>
        /// <param name="isFixedSize">The collection is fixed to the size passed in the capacity parameter.</param>
        public LockFreeSkipList(int capacity, bool isFixedSize)
            : base(null, capacity, null, isFixedSize) { }

        /// <summary>
        /// Initializes a new instance of the LockFreeCollection<(Of <(T>)>) class
        /// </summary>
        /// <param name="comparer">This is the comparer used to detect equality between items in the collection. 
        /// If this is set to null the default comparer for the type will be used instead./</param>
        /// <param name="capacity">The collection initial capacity.</param>
        public LockFreeSkipList(IEqualityComparer<T> comparer, int capacity)
            : base(comparer, capacity, null, false) { }
        /// <summary>
        /// Initializes a new instance of the LockFreeCollection<(Of <(T>)>) class
        /// </summary>
        /// <param name="comparer">This is the comparer used to detect equality between items in the collection. 
        /// If this is set to null the default comparer for the type will be used instead./</param>
        /// <param name="capacity">The collection initial capacity.</param>
        /// <param name="isFixedSize">The collection is fixed to the size passed in the capacity parameter.</param>
        public LockFreeSkipList(IEqualityComparer<T> comparer, int capacity, bool isFixedSize)
            : base(comparer, capacity, null, isFixedSize) { }

        /// <summary>
        /// Initializes a new instance of the LockFreeCollection<(Of <(T>)>) class
        /// </summary>
        /// <param name="comparer">This is the comparer used to detect equality between items in the collection. 
        /// If this is set to null the default comparer for the type will be used instead./</param>
        /// <param name="capacity">The collection initial capacity.</param>
        /// <param name="collection">The values in this enumeration will be loaded in to the collection.</param>
        public LockFreeSkipList(IEqualityComparer<T> comparer, int capacity, IEnumerable<T> collection)
            : base(comparer, capacity, collection, false) { }
        /// <summary>
        /// Initializes a new instance of the LockFreeCollection<(Of <(T>)>) class
        /// </summary>
        /// <param name="comparer">This is the comparer used to detect equality between items in the collection. 
        /// If this is set to null the default comparer for the type will be used instead./</param>
        /// <param name="capacity">The collection initial capacity.</param>
        /// <param name="collection">The values in this enumeration will be loaded in to the collection.</param>
        /// <param name="isFixedSize">The collection is fixed to the size passed in the capacity parameter.</param>
        public LockFreeSkipList(IEqualityComparer<T> comparer, int capacity, IEnumerable<T> collection, bool isFixedSize)
            : base(comparer, capacity, collection, isFixedSize) { }
        #endregion // Constructor

        #region CollectionAllowMultipleEntries
        /// <summary>
        /// This setting determines whether the collection allows multiple entries of the same object in the collection.
        /// The default setting is true.
        /// </summary>
        protected override bool CollectionAllowMultipleEntries { get { return true; } }
        #endregion

        #region Add(T item)
        /// <summary>
        /// Adds and item to the collection.
        /// </summary>
        /// <param name="item">The item to add.</param>
        public void Add(T item)
        {
            DisposedCheck();
            Insert(item, true);
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
            DisposedCheck();
            return RemoveInternal(item);
        }
        #endregion // Remove(T item)
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
        #endregion // CopyTo(T[] array, int arrayIndex)    }
    }
}
