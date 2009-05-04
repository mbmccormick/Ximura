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
using System.Security.Permissions;
using System.Threading;

using Ximura;
using Ximura.Helper;
#endregion // using
namespace Ximura.Collections
{
    [DebuggerDisplay("Count = {Count}"), HostProtection(SecurityAction.LinkDemand, MayLeakOnAbort = true)]
    public class LockFreeList<T> : LockFreeCollection<T>, ICollection<T>
    {
        #region Constructors
        /// <summary>
        /// This is the default constructor. The collection will be constructed with a base capacity of 1000.
        /// </summary>
        public LockFreeList() : base(null, 1000, null) { }
        /// <summary>
        /// Initializes a new instance of the LockFreeList<(Of <(T>)>) class
        /// </summary>
        /// <param name="collection">The values in this enumeration will be loaded in to the collection.</param>
        /// <param name="comparer">This is the comparer used to detect equality between items in the collection. 
        /// If this is set to null the default comparer for the type will be used instead./</param>
        public LockFreeList(IEnumerable<T> collection, EqualityComparer<T> comparer) : base(collection, 1000, comparer) { }
        /// <summary>
        /// Initializes a new instance of the LockFreeList<(Of <(T>)>) class
        /// </summary>
        /// <param name="collection">The values in this enumeration will be loaded in to the collection.</param>
        /// <param name="capacity">The collection initial capacity.</param>
        /// <param name="comparer">This is the comparer used to detect equality between items in the collection. 
        /// If this is set to null the default comparer for the type will be used instead./</param>
        public LockFreeList(IEnumerable<T> collection, int capacity, EqualityComparer<T> comparer) : base(collection, capacity, comparer) { }
        /// <summary>
        /// Initializes a new instance of the LockFreeList<(Of <(T>)>) class
        /// </summary>
        /// <param name="collection">The values in this enumeration will be loaded in to the collection.</param>
        public LockFreeList(IEnumerable<T> collection) : base(collection, 1000, null) { }
        /// <summary>
        /// Initializes a new instance of the LockFreeList<(Of <(T>)>) class
        /// </summary>
        /// <param name="capacity">The collection initial capacity.</param>
        public LockFreeList(int capacity) : base(null, capacity, null) { }
        /// <summary>
        /// Initializes a new instance of the LockFreeList<(Of <(T>)>) class
        /// </summary>
        /// <param name="capacity">The collection initial capacity.</param>
        /// <param name="comparer">This is the comparer used to detect equality between items in the collection. 
        /// If this is set to null the default comparer for the type will be used instead./</param>
        public LockFreeList(int capacity, EqualityComparer<T> comparer) : base(null, capacity, comparer) { }
        #endregion // Constructors

        #region AllowMultipleEntries
        /// <summary>
        /// The list allows multiple entries.
        /// </summary>
        protected override bool AllowMultipleEntries
        {
            get
            {
                return true;
            }
        }
        #endregion // AllowMultipleEntries

    }
}
