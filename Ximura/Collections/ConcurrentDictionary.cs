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
#if (!SILVERLIGHT)
using System.Security.Permissions;
#endif
using System.Threading;

using Ximura;

#endregion // using
namespace Ximura.Collections
{
    #region ConcurrentDictionary
    /// <summary>
    /// This class is a concurrent lock-free implementation of the IDictionary interface using a hash-table based array.
    /// </summary>
    /// <typeparam name="TKey">The dictionary key type.</typeparam>
    /// <typeparam name="TValue">The dictionary value type.</typeparam>
    [DebuggerDisplay("Count = {Count}")]
#if (!SILVERLIGHT)
    [HostProtection(SecurityAction.LinkDemand, MayLeakOnAbort = true)]
#endif
    public sealed class ConcurrentDictionary<TKey, TValue> : ConcurrentDictionary<TKey, TValue, HashTableStructBasedVertexArrayV2<KeyValuePair<TKey, TValue>>>
    {
        #region Constructor
        /// <summary>
        /// This is the default constructor. The collection will be constructed with a base capacity of 1000.
        /// </summary>
        public ConcurrentDictionary()
            : base(new KeyValueOnlyKeyEqualityComparer<TKey, TValue>(), 1000, null, false) { }

        /// <summary>
        /// Initializes a new instance of the class
        /// </summary>
        /// <param name="collection">The values in this enumeration will be loaded in to the collection.</param>
        public ConcurrentDictionary(IEnumerable<KeyValuePair<TKey,TValue>> collection)
            : base(new KeyValueOnlyKeyEqualityComparer<TKey, TValue>(), 1000, collection, false) { }

        /// <summary>
        /// Initializes a new instance of the class
        /// </summary>
        /// <param name="collection">The values in this enumeration will be loaded in to the collection. Set this to null if not required.</param>
        /// <param name="isFixedSize">The collection is fixed to the size passed in the capacity parameter.</param>
        public ConcurrentDictionary(IEnumerable<KeyValuePair<TKey, TValue>> collection, bool isFixedSize)
            : base(new KeyValueOnlyKeyEqualityComparer<TKey, TValue>(), isFixedSize ? -1 : 1000, collection, isFixedSize) { }

        /// <summary>
        /// Initializes a new instance of the class
        /// </summary>
        /// <param name="collection">The values in this enumeration will be loaded in to the collection. Set this to null if not required.</param>
        /// <param name="comparer">This is the comparer used to detect equality between items in the collection. 
        /// If this is set to null the default comparer for the type will be used instead./</param>
        public ConcurrentDictionary(IEqualityComparer<TKey> comparer, IEnumerable<KeyValuePair<TKey, TValue>> collection)
            : base(new KeyValueOnlyKeyEqualityComparer<TKey, TValue>(comparer), 1000, collection, false) { }

        /// <summary>
        /// Initializes a new instance of the class
        /// </summary>
        /// <param name="capacity">The collection initial capacity.</param>
        public ConcurrentDictionary(int capacity)
            : base(new KeyValueOnlyKeyEqualityComparer<TKey, TValue>(), capacity, null, false) { }
        /// <summary>
        /// Initializes a new instance of the class
        /// </summary>
        /// <param name="capacity">The collection initial capacity.</param>
        /// <param name="isFixedSize">The collection is fixed to the size passed in the capacity parameter.</param>
        public ConcurrentDictionary(int capacity, bool isFixedSize)
            : base(new KeyValueOnlyKeyEqualityComparer<TKey, TValue>(), capacity, null, isFixedSize) { }

        /// <summary>
        /// Initializes a new instance of the class
        /// </summary>
        /// <param name="comparer">This is the comparer used to detect equality between items in the collection. 
        /// If this is set to null the default comparer for the type will be used instead./</param>
        /// <param name="capacity">The collection initial capacity.</param>
        public ConcurrentDictionary(IEqualityComparer<TKey> comparer, int capacity)
            : base(new KeyValueOnlyKeyEqualityComparer<TKey, TValue>(comparer), capacity, null, false) { }
        /// <summary>
        /// Initializes a new instance of the class
        /// </summary>
        /// <param name="comparer">This is the comparer used to detect equality between items in the collection. 
        /// If this is set to null the default comparer for the type will be used instead./</param>
        /// <param name="capacity">The collection initial capacity.</param>
        /// <param name="isFixedSize">The collection is fixed to the size passed in the capacity parameter.</param>
        public ConcurrentDictionary(IEqualityComparer<TKey> comparer, int capacity, bool isFixedSize)
            : base(new KeyValueOnlyKeyEqualityComparer<TKey, TValue>(comparer), capacity, null, isFixedSize) { }

        /// <summary>
        /// Initializes a new instance of the class
        /// </summary>
        /// <param name="comparer">This is the comparer used to detect equality between items in the collection. 
        /// If this is set to null the default comparer for the type will be used instead./</param>
        /// <param name="capacity">The collection initial capacity.</param>
        /// <param name="collection">The values in this enumeration will be loaded in to the collection. Set this to null if not required.</param>
        public ConcurrentDictionary(IEqualityComparer<TKey> comparer, int capacity, IEnumerable<KeyValuePair<TKey, TValue>> collection)
            : base(new KeyValueOnlyKeyEqualityComparer<TKey, TValue>(comparer), capacity, collection, false) { }
        /// <summary>
        /// Initializes a new instance of the class
        /// </summary>
        /// <param name="comparer">This is the comparer used to detect equality between items in the collection. 
        /// If this is set to null the default comparer for the type will be used instead./</param>
        /// <param name="capacity">The collection initial capacity.</param>
        /// <param name="collection">The values in this enumeration will be loaded in to the collection. Set this to null if not required.</param>
        /// <param name="isFixedSize">The collection is fixed to the size passed in the capacity parameter.</param>
        public ConcurrentDictionary(IEqualityComparer<TKey> comparer, int capacity, IEnumerable<KeyValuePair<TKey, TValue>> collection, bool isFixedSize)
            : base(new KeyValueOnlyKeyEqualityComparer<TKey, TValue>(comparer), capacity, collection, isFixedSize) { }

        /// <summary>
        /// Initializes a new instance of the class
        /// </summary>
        /// <param name="comparer">This is the comparer that allows a custom value comparer to be set in addition to a custom key comparer.</param>
        /// <param name="capacity">The collection initial capacity.</param>
        /// <param name="collection">The values in this enumeration will be loaded in to the collection. Set this to null if not required.</param>
        /// <param name="isFixedSize">The collection is fixed to the size passed in the capacity parameter.</param>
        protected ConcurrentDictionary(KeyValueOnlyKeyEqualityComparer<TKey, TValue> comparer, int capacity, IEnumerable<KeyValuePair<TKey, TValue>> collection, bool isFixedSize)
            : base(comparer, capacity, collection, isFixedSize) { }

        #endregion // Constructor
    }
    #endregion // ConcurrentDictionary

#if (!SILVERLIGHT)
    #region ConcurrentDictionarySLC
    /// <summary>
    /// This class is a concurrent lock-free implementation of the IDictionary interface using a skip-list class based array.
    /// </summary>
    /// <typeparam name="TKey">The dictionary key type.</typeparam>
    /// <typeparam name="TValue">The dictionary value type.</typeparam>
    [DebuggerDisplay("Count = {Count}")]
    [HostProtection(SecurityAction.LinkDemand, MayLeakOnAbort = true)]
    public sealed class ConcurrentDictionarySLC<TKey, TValue> : ConcurrentDictionary<TKey, TValue, SkipListClassBasedVertexArray<KeyValuePair<TKey, TValue>>>
    {
        #region Constructor
        /// <summary>
        /// This is the default constructor. The collection will be constructed with a base capacity of 1000.
        /// </summary>
        public ConcurrentDictionarySLC()
            : base(new KeyValueOnlyKeyEqualityComparer<TKey, TValue>(), 1000, null, false) { }

        /// <summary>
        /// Initializes a new instance of the ConcurrentDictionarySLC<(Of <(T>)>) class
        /// </summary>
        /// <param name="collection">The values in this enumeration will be loaded in to the collection.</param>
        public ConcurrentDictionarySLC(IEnumerable<KeyValuePair<TKey, TValue>> collection)
            : base(new KeyValueOnlyKeyEqualityComparer<TKey, TValue>(), 1000, collection, false) { }

        /// <summary>
        /// Initializes a new instance of the ConcurrentDictionarySLC<(Of <(T>)>) class
        /// </summary>
        /// <param name="collection">The values in this enumeration will be loaded in to the collection. Set this to null if not required.</param>
        /// <param name="isFixedSize">The collection is fixed to the size passed in the capacity parameter.</param>
        public ConcurrentDictionarySLC(IEnumerable<KeyValuePair<TKey, TValue>> collection, bool isFixedSize)
            : base(new KeyValueOnlyKeyEqualityComparer<TKey, TValue>(), isFixedSize ? -1 : 1000, collection, isFixedSize) { }

        /// <summary>
        /// Initializes a new instance of the ConcurrentDictionarySLC<(Of <(T>)>) class
        /// </summary>
        /// <param name="collection">The values in this enumeration will be loaded in to the collection. Set this to null if not required.</param>
        /// <param name="comparer">This is the comparer used to detect equality between items in the collection. 
        /// If this is set to null the default comparer for the type will be used instead./</param>
        public ConcurrentDictionarySLC(IEqualityComparer<TKey> comparer, IEnumerable<KeyValuePair<TKey, TValue>> collection)
            : base(new KeyValueOnlyKeyEqualityComparer<TKey, TValue>(comparer), 1000, collection, false) { }

        /// <summary>
        /// Initializes a new instance of the ConcurrentDictionarySLC<(Of <(T>)>) class
        /// </summary>
        /// <param name="capacity">The collection initial capacity.</param>
        public ConcurrentDictionarySLC(int capacity)
            : base(new KeyValueOnlyKeyEqualityComparer<TKey, TValue>(), capacity, null, false) { }
        /// <summary>
        /// Initializes a new instance of the ConcurrentDictionarySLC<(Of <(T>)>) class
        /// </summary>
        /// <param name="capacity">The collection initial capacity.</param>
        /// <param name="isFixedSize">The collection is fixed to the size passed in the capacity parameter.</param>
        public ConcurrentDictionarySLC(int capacity, bool isFixedSize)
            : base(new KeyValueOnlyKeyEqualityComparer<TKey, TValue>(), capacity, null, isFixedSize) { }

        /// <summary>
        /// Initializes a new instance of the ConcurrentDictionarySLC<(Of <(T>)>) class
        /// </summary>
        /// <param name="comparer">This is the comparer used to detect equality between items in the collection. 
        /// If this is set to null the default comparer for the type will be used instead./</param>
        /// <param name="capacity">The collection initial capacity.</param>
        public ConcurrentDictionarySLC(IEqualityComparer<TKey> comparer, int capacity)
            : base(new KeyValueOnlyKeyEqualityComparer<TKey, TValue>(comparer), capacity, null, false) { }
        /// <summary>
        /// Initializes a new instance of the ConcurrentDictionarySLC<(Of <(T>)>) class
        /// </summary>
        /// <param name="comparer">This is the comparer used to detect equality between items in the collection. 
        /// If this is set to null the default comparer for the type will be used instead./</param>
        /// <param name="capacity">The collection initial capacity.</param>
        /// <param name="isFixedSize">The collection is fixed to the size passed in the capacity parameter.</param>
        public ConcurrentDictionarySLC(IEqualityComparer<TKey> comparer, int capacity, bool isFixedSize)
            : base(new KeyValueOnlyKeyEqualityComparer<TKey, TValue>(comparer), capacity, null, isFixedSize) { }

        /// <summary>
        /// Initializes a new instance of the ConcurrentDictionarySLC<(Of <(T>)>) class
        /// </summary>
        /// <param name="comparer">This is the comparer used to detect equality between items in the collection. 
        /// If this is set to null the default comparer for the type will be used instead./</param>
        /// <param name="capacity">The collection initial capacity.</param>
        /// <param name="collection">The values in this enumeration will be loaded in to the collection. Set this to null if not required.</param>
        public ConcurrentDictionarySLC(IEqualityComparer<TKey> comparer, int capacity, IEnumerable<KeyValuePair<TKey, TValue>> collection)
            : base(new KeyValueOnlyKeyEqualityComparer<TKey, TValue>(comparer), capacity, collection, false) { }
        /// <summary>
        /// Initializes a new instance of the ConcurrentDictionarySLC<(Of <(T>)>) class
        /// </summary>
        /// <param name="comparer">This is the comparer used to detect equality between items in the collection. 
        /// If this is set to null the default comparer for the type will be used instead./</param>
        /// <param name="capacity">The collection initial capacity.</param>
        /// <param name="collection">The values in this enumeration will be loaded in to the collection. Set this to null if not required.</param>
        /// <param name="isFixedSize">The collection is fixed to the size passed in the capacity parameter.</param>
        public ConcurrentDictionarySLC(IEqualityComparer<TKey> comparer, int capacity, IEnumerable<KeyValuePair<TKey, TValue>> collection, bool isFixedSize)
            : base(new KeyValueOnlyKeyEqualityComparer<TKey, TValue>(comparer), capacity, collection, isFixedSize) { }

        /// <summary>
        /// Initializes a new instance of the ConcurrentDictionarySLC<(Of <(T>)>) class
        /// </summary>
        /// <param name="comparer">This is the comparer that allows a custom value comparer to be set in addition to a custom key comparer.</param>
        /// <param name="capacity">The collection initial capacity.</param>
        /// <param name="collection">The values in this enumeration will be loaded in to the collection. Set this to null if not required.</param>
        /// <param name="isFixedSize">The collection is fixed to the size passed in the capacity parameter.</param>
        protected ConcurrentDictionarySLC(KeyValueOnlyKeyEqualityComparer<TKey, TValue> comparer, int capacity, IEnumerable<KeyValuePair<TKey, TValue>> collection, bool isFixedSize)
            : base(comparer, capacity, collection, isFixedSize) { }

        #endregion // Constructor
    }
    #endregion // ConcurrentDictionarySLC
#endif

}
