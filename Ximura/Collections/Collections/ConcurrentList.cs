﻿#region Copyright
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

#if (!SILVERLIGHT)
using System.Security.Permissions;
#endif

#endregion // using
namespace Ximura.Collections
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T">The collection item type.</typeparam>
    /// <typeparam name="A">The vertex array type.</typeparam>
    [DebuggerDisplay("Count = {Count}")]
#if (!SILVERLIGHT)
    [HostProtection(SecurityAction.LinkDemand, MayLeakOnAbort = true)]
#endif
    public class ConcurrentList<T, A> : ConcurrentCollectionBase<T, A>, ICollection<T>
        where A : VertexArray<T>,new()
    {
        #region Constructor
        /// <summary>
        /// This is the default constructor. The collection will be constructed with a base capacity of 1000.
        /// </summary>
        public ConcurrentList() 
            : base(null, 1000, null, false) { }

        /// <summary>
        /// Initializes a new instance of the LockFreeList<(Of <(T>)>) class
        /// </summary>
        /// <param name="collection">The values in this enumeration will be loaded in to the collection.</param>
        public ConcurrentList(IEnumerable<T> collection) : base(null, 1000, collection, false) { }

        /// <summary>
        /// Initializes a new instance of the LockFreeList<(Of <(T>)>) class
        /// </summary>
        /// <param name="collection">The values in this enumeration will be loaded in to the collection.</param>
        /// <param name="isFixedSize">The collection is fixed to the size passed in the capacity parameter.</param>
        public ConcurrentList(IEnumerable<T> collection, bool isFixedSize) : base(null, isFixedSize ? -1 : 1000, collection, isFixedSize) { }

        /// <summary>
        /// Initializes a new instance of the LockFreeList<(Of <(T>)>) class
        /// </summary>
        /// <param name="collection">The values in this enumeration will be loaded in to the collection.</param>
        /// <param name="comparer">This is the comparer used to detect equality between items in the collection. 
        /// If this is set to null the default comparer for the type will be used instead./</param>
        public ConcurrentList(IEqualityComparer<T> comparer, IEnumerable<T> collection) 
            : base(comparer, 1000, collection, false) { }

        /// <summary>
        /// Initializes a new instance of the LockFreeList<(Of <(T>)>) class
        /// </summary>
        /// <param name="capacity">The collection initial capacity.</param>
        public ConcurrentList(int capacity)
            : base(null, capacity, null, false) { }
        /// <summary>
        /// Initializes a new instance of the LockFreeList<(Of <(T>)>) class
        /// </summary>
        /// <param name="capacity">The collection initial capacity.</param>
        /// <param name="isFixedSize">The collection is fixed to the size passed in the capacity parameter.</param>
        public ConcurrentList(int capacity, bool isFixedSize)
            : base(null, capacity, null, isFixedSize) { }

        /// <summary>
        /// Initializes a new instance of the LockFreeList<(Of <(T>)>) class
        /// </summary>
        /// <param name="comparer">This is the comparer used to detect equality between items in the collection. 
        /// If this is set to null the default comparer for the type will be used instead./</param>
        /// <param name="capacity">The collection initial capacity.</param>
        public ConcurrentList(IEqualityComparer<T> comparer, int capacity)
            : base(comparer, capacity, null, false) { }
        /// <summary>
        /// Initializes a new instance of the LockFreeList<(Of <(T>)>) class
        /// </summary>
        /// <param name="comparer">This is the comparer used to detect equality between items in the collection. 
        /// If this is set to null the default comparer for the type will be used instead./</param>
        /// <param name="capacity">The collection initial capacity.</param>
        /// <param name="isFixedSize">The collection is fixed to the size passed in the capacity parameter.</param>
        public ConcurrentList(IEqualityComparer<T> comparer, int capacity, bool isFixedSize)
            : base(comparer, capacity, null, isFixedSize) { }

        /// <summary>
        /// Initializes a new instance of the LockFreeList<(Of <(T>)>) class
        /// </summary>
        /// <param name="comparer">This is the comparer used to detect equality between items in the collection. 
        /// If this is set to null the default comparer for the type will be used instead./</param>
        /// <param name="capacity">The collection initial capacity.</param>
        /// <param name="collection">The values in this enumeration will be loaded in to the collection.</param>
        public ConcurrentList(IEqualityComparer<T> comparer, int capacity, IEnumerable<T> collection)
            : base(comparer, capacity, collection, false) { }
        /// <summary>
        /// Initializes a new instance of the LockFreeList<(Of <(T>)>) class
        /// </summary>
        /// <param name="comparer">This is the comparer used to detect equality between items in the collection. 
        /// If this is set to null the default comparer for the type will be used instead./</param>
        /// <param name="capacity">The collection initial capacity.</param>
        /// <param name="collection">The values in this enumeration will be loaded in to the collection.</param>
        /// <param name="isFixedSize">The collection is fixed to the size passed in the capacity parameter.</param>
        public ConcurrentList(IEqualityComparer<T> comparer, int capacity, IEnumerable<T> collection, bool isFixedSize)
            : base(comparer, capacity, collection, isFixedSize) { }
        #endregion // Constructor

        #region AllowMultipleEntries
        /// <summary>
        /// The list allows multiple entries.
        /// </summary>
        protected override bool CollectionAllowMultipleEntries { get { return true; } }
        #endregion // AllowMultipleEntries
    }
}
