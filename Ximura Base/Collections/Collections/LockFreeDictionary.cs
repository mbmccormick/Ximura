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
    /// This class is a lock-free implementation of the IDictionary interface.
    /// </summary>
    /// <typeparam name="TKey">The key type.</typeparam>
    /// <typeparam name="TValue">The value type.</typeparam>
    public partial class LockFreeDictionary<TKey, TValue> : LockFreeCollectionBase<KeyValuePair<TKey, TValue>>, IDictionary<TKey, TValue>
    {
        #region KeyValueOnlyKeyComparer<TKey, TValue>
        /// <summary>
        /// This comparer is used to only report the key for the hashcode and equality comparer.
        /// </summary>
        private class KeyValueOnlyKeyComparer<TKey, TValue> : IEqualityComparer<KeyValuePair<TKey, TValue>>
        {
            #region Declarations
            private IEqualityComparer<TKey> mKeyComparer;
            #endregion // Declarations
            #region Constructors
            /// <summary>
            /// This is the default constructor.
            /// </summary>
            public KeyValueOnlyKeyComparer() : this(null) { }
            /// <summary>
            /// This constructor allows a custom key equality comparer to be passed to the class.
            /// </summary>
            /// <param name="keyComparer">The key comparer. If this is null, the default comparer is used.</param>
            public KeyValueOnlyKeyComparer(IEqualityComparer<TKey> keyComparer)
            {
                if (keyComparer == null)
                    mKeyComparer = EqualityComparer<TKey>.Default;
                else
                    mKeyComparer = keyComparer;
            }
            #endregion // Constructors

            #region Equals(KeyValuePair<TKey, TValue> x, KeyValuePair<TKey, TValue> y)
            /// <summary>
            /// This method only compares the key of the keyvalue pair for equality.
            /// </summary>
            /// <param name="x">The first keyvalue pair.</param>
            /// <param name="y">The second keyvalue pair.</param>
            /// <returns>Returns true if the keys are equal.</returns>
            public bool Equals(KeyValuePair<TKey, TValue> x, KeyValuePair<TKey, TValue> y)
            {
                return mKeyComparer.Equals(x.Key, y.Key);
            }
            #endregion // Equals(KeyValuePair<TKey, TValue> x, KeyValuePair<TKey, TValue> y)
            #region GetHashCode(KeyValuePair<TKey, TValue> obj)
            /// <summary>
            /// The method returns the hashcode for the key of the keyvalue pair.
            /// </summary>
            /// <param name="obj">The object to get the hashcode.</param>
            /// <returns>Returns the key hashcode.</returns>
            public int GetHashCode(KeyValuePair<TKey, TValue> obj)
            {
                return mKeyComparer.GetHashCode(obj.Key);
            }
            #endregion // GetHashCode(KeyValuePair<TKey, TValue> obj)
        }
        #endregion // KeyValueOnlyKeyComparer<TKey, TValue>
        #region Constructor
        /// <summary>
        /// This is the default constructor. The collection will be constructed with a base capacity of 1000.
        /// </summary>
        public LockFreeDictionary() 
            : base(null, 1000, null, false) { }

        /// <summary>
        /// Initializes a new instance of the LockFreeCollection<(Of <(T>)>) class
        /// </summary>
        /// <param name="collection">The values in this enumeration will be loaded in to the collection.</param>
        public LockFreeDictionary(IEnumerable<KeyValuePair<TKey,TValue>> collection) : base(null, 1000, collection, false) { }

        /// <summary>
        /// Initializes a new instance of the LockFreeCollection<(Of <(T>)>) class
        /// </summary>
        /// <param name="collection">The values in this enumeration will be loaded in to the collection.</param>
        /// <param name="isFixedSize">The collection is fixed to the size passed in the capacity parameter.</param>
        public LockFreeDictionary(IEnumerable<KeyValuePair<TKey, TValue>> collection, bool isFixedSize) : base(null, isFixedSize ? -1 : 1000, collection, isFixedSize) { }

        /// <summary>
        /// Initializes a new instance of the LockFreeCollection<(Of <(T>)>) class
        /// </summary>
        /// <param name="collection">The values in this enumeration will be loaded in to the collection.</param>
        /// <param name="comparer">This is the comparer used to detect equality between items in the collection. 
        /// If this is set to null the default comparer for the type will be used instead./</param>
        public LockFreeDictionary(IEqualityComparer<TKey> comparer, IEnumerable<KeyValuePair<TKey, TValue>> collection)
            : base(new KeyValueOnlyKeyComparer<TKey, TValue>(comparer), 1000, collection, false) { }

        /// <summary>
        /// Initializes a new instance of the LockFreeCollection<(Of <(T>)>) class
        /// </summary>
        /// <param name="capacity">The collection initial capacity.</param>
        public LockFreeDictionary(int capacity)
            : base(null, capacity, null, false) { }
        /// <summary>
        /// Initializes a new instance of the LockFreeCollection<(Of <(T>)>) class
        /// </summary>
        /// <param name="capacity">The collection initial capacity.</param>
        /// <param name="isFixedSize">The collection is fixed to the size passed in the capacity parameter.</param>
        public LockFreeDictionary(int capacity, bool isFixedSize)
            : base(null, capacity, null, isFixedSize) { }

        /// <summary>
        /// Initializes a new instance of the LockFreeCollection<(Of <(T>)>) class
        /// </summary>
        /// <param name="comparer">This is the comparer used to detect equality between items in the collection. 
        /// If this is set to null the default comparer for the type will be used instead./</param>
        /// <param name="capacity">The collection initial capacity.</param>
        public LockFreeDictionary(IEqualityComparer<TKey> comparer, int capacity)
            : base(new KeyValueOnlyKeyComparer<TKey, TValue>(comparer), capacity, null, false) { }
        /// <summary>
        /// Initializes a new instance of the LockFreeCollection<(Of <(T>)>) class
        /// </summary>
        /// <param name="comparer">This is the comparer used to detect equality between items in the collection. 
        /// If this is set to null the default comparer for the type will be used instead./</param>
        /// <param name="capacity">The collection initial capacity.</param>
        /// <param name="isFixedSize">The collection is fixed to the size passed in the capacity parameter.</param>
        public LockFreeDictionary(IEqualityComparer<TKey> comparer, int capacity, bool isFixedSize)
            : base(new KeyValueOnlyKeyComparer<TKey, TValue>(comparer), capacity, null, isFixedSize) { }

        /// <summary>
        /// Initializes a new instance of the LockFreeCollection<(Of <(T>)>) class
        /// </summary>
        /// <param name="comparer">This is the comparer used to detect equality between items in the collection. 
        /// If this is set to null the default comparer for the type will be used instead./</param>
        /// <param name="capacity">The collection initial capacity.</param>
        /// <param name="collection">The values in this enumeration will be loaded in to the collection.</param>
        public LockFreeDictionary(IEqualityComparer<TKey> comparer, int capacity, IEnumerable<KeyValuePair<TKey, TValue>> collection)
            : base(new KeyValueOnlyKeyComparer<TKey, TValue>(comparer), capacity, collection, false) { }
        /// <summary>
        /// Initializes a new instance of the LockFreeCollection<(Of <(T>)>) class
        /// </summary>
        /// <param name="comparer">This is the comparer used to detect equality between items in the collection. 
        /// If this is set to null the default comparer for the type will be used instead./</param>
        /// <param name="capacity">The collection initial capacity.</param>
        /// <param name="collection">The values in this enumeration will be loaded in to the collection.</param>
        /// <param name="isFixedSize">The collection is fixed to the size passed in the capacity parameter.</param>
        public LockFreeDictionary(IEqualityComparer<TKey> comparer, int capacity, IEnumerable<KeyValuePair<TKey, TValue>> collection, bool isFixedSize)
            : base(new KeyValueOnlyKeyComparer<TKey, TValue>(comparer), capacity, collection, isFixedSize) { }
        #endregion // Constructor

        #region Add
        public void Add(TKey key, TValue value)
        {
            AddInternal(new KeyValuePair<TKey, TValue>(key, value));
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            AddInternal(item);
        }
        #endregion // Add
        #region Contains
        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return ContainsInternal(item);
        }

        public bool ContainsKey(TKey key)
        {
            return ContainsInternal(new KeyValuePair<TKey, TValue>(key, default(TValue)));
        }
        #endregion // Contains
        #region Remove
        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            if (item.Key == null)
            {
                throw new ArgumentNullException();
            }

            return false;
        }

        public bool Remove(TKey key)
        {
            if (key == null)
            {
                throw new ArgumentNullException();
            }


            return false;
        }
        #endregion // Remove
        #region Clear()
        public void Clear()
        {
            DisposedCheck();
            ClearInternal();
        }
        #endregion // Clear()
        #region Count
        public int Count
        {
            get { return CountInternal; }
        }
        #endregion // Count
        #region IsReadOnly
        public bool IsReadOnly
        {
            get { return false; }
        }
        #endregion // IsReadOnly

        #region TryGetValue(TKey key, out TValue value)
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryGetValue(TKey key, out TValue value)
        {
            throw new NotImplementedException();
        }
        #endregion // TryGetValue(TKey key, out TValue value)
        #region this[TKey key]
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public TValue this[TKey key]
        {
            get
            {
                TValue value;
                if (!TryGetValue(key, out value))
                    throw new KeyNotFoundException();

                return value;
            }
            set
            {
                if (!AddInternal(new KeyValuePair<TKey, TValue>(key, value)))
                    throw new Exception();
            }
        }
        #endregion // this[TKey key]
    }
}
