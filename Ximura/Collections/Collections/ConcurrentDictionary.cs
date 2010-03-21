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
    /// This class is a concurrent lock-free implementation of the IDictionary interface.
    /// </summary>
    /// <typeparam name="TKey">The dictionary key type.</typeparam>
    /// <typeparam name="TValue">The dictionary value type.</typeparam>
    /// <typeparam name="A">The vertex array type.</typeparam>
    [DebuggerDisplay("Count = {Count}"), HostProtection(SecurityAction.LinkDemand, MayLeakOnAbort = true)]
    public class ConcurrentDictionary<TKey, TValue, A> : CollectionHelperBase<KeyValuePair<TKey, TValue>, A>, IDictionary<TKey, TValue>
        where A : VertexArray<KeyValuePair<TKey, TValue>>, new()
    {
        #region Class -> KeyValueOnlyKeyEqualityComparer
        /// <summary>
        /// This comparer is used to only report the key for the hashcode and equality comparer.
        /// </summary>
        /// <typeparam name="TKey">The key type.</typeparam>
        /// <typeparam name="TValue">The value type.</typeparam>
        protected class KeyValueOnlyKeyEqualityComparer<TKey, TValue> : IEqualityComparer<KeyValuePair<TKey, TValue>>
        {
            #region Constructors
            /// <summary>
            /// This is the default constructor.
            /// </summary>
            public KeyValueOnlyKeyEqualityComparer() : this(null, null) { }
            /// <summary>
            /// This constructor allows a custom key and value equality comparer to be passed to the class.
            /// </summary>
            /// <param name="keyComparer">The key comparer. If this is null, the default comparer is used.</param>
            public KeyValueOnlyKeyEqualityComparer(IEqualityComparer<TKey> keyComparer) : this(keyComparer, null) { }
            /// <summary>
            /// This constructor allows a custom key and value equality comparers to be passed to the class.
            /// </summary>
            /// <param name="keyComparer">The key comparer. If this is null, the default comparer is used.</param>
            /// <param name="valueComparer">The value comparer. If this is null, the default comparer is used.</param>
            public KeyValueOnlyKeyEqualityComparer(IEqualityComparer<TKey> keyComparer, IEqualityComparer<TValue> valueComparer)
            {
                if (keyComparer == null)
                    KeyComparer = EqualityComparer<TKey>.Default;
                else
                    KeyComparer = keyComparer;

                if (valueComparer == null)
                    ValueComparer = EqualityComparer<TValue>.Default;
                else
                    ValueComparer = valueComparer;
            }
            #endregion // Constructors
            #region KeyComparer
            /// <summary>
            /// This is the key comparer.
            /// </summary>
            public IEqualityComparer<TKey> KeyComparer { get; private set; }
            #endregion // Declarations
            #region ValueComparer
            /// <summary>
            /// This is the key comparer.
            /// </summary>
            public IEqualityComparer<TValue> ValueComparer { get; private set; }
            #endregion // Declarations

            #region Equals(KeyValuePair<TKey, TValue> x, KeyValuePair<TKey, TValue> y)
            /// <summary>
            /// This method only compares the key of the keyvalue pair for equality.
            /// </summary>
            /// <param name="x">The first keyvalue pair.</param>
            /// <param name="y">The second keyvalue pair.</param>
            /// <returns>Returns true if the keys are equal.</returns>
            public bool Equals(KeyValuePair<TKey, TValue> x, KeyValuePair<TKey, TValue> y)
            {
                return KeyComparer.Equals(x.Key, y.Key);
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
                return KeyComparer.GetHashCode(obj.Key);
            }
            #endregion // GetHashCode(KeyValuePair<TKey, TValue> obj)
        }
        #endregion // KeyValueOnlyKeyEqualityComparer
        #region Constructor
        /// <summary>
        /// This is the default constructor. The collection will be constructed with a base capacity of 1000.
        /// </summary>
        public ConcurrentDictionary()
            : base(new KeyValueOnlyKeyEqualityComparer<TKey, TValue>(), 1000, null, false) { }

        /// <summary>
        /// Initializes a new instance of the LockFreeDictionary<(Of <(T>)>) class
        /// </summary>
        /// <param name="collection">The values in this enumeration will be loaded in to the collection.</param>
        public ConcurrentDictionary(IEnumerable<KeyValuePair<TKey,TValue>> collection)
            : base(new KeyValueOnlyKeyEqualityComparer<TKey, TValue>(), 1000, collection, false) { }

        /// <summary>
        /// Initializes a new instance of the LockFreeDictionary<(Of <(T>)>) class
        /// </summary>
        /// <param name="collection">The values in this enumeration will be loaded in to the collection. Set this to null if not required.</param>
        /// <param name="isFixedSize">The collection is fixed to the size passed in the capacity parameter.</param>
        public ConcurrentDictionary(IEnumerable<KeyValuePair<TKey, TValue>> collection, bool isFixedSize)
            : base(new KeyValueOnlyKeyEqualityComparer<TKey, TValue>(), isFixedSize ? -1 : 1000, collection, isFixedSize) { }

        /// <summary>
        /// Initializes a new instance of the LockFreeDictionary<(Of <(T>)>) class
        /// </summary>
        /// <param name="collection">The values in this enumeration will be loaded in to the collection. Set this to null if not required.</param>
        /// <param name="comparer">This is the comparer used to detect equality between items in the collection. 
        /// If this is set to null the default comparer for the type will be used instead./</param>
        public ConcurrentDictionary(IEqualityComparer<TKey> comparer, IEnumerable<KeyValuePair<TKey, TValue>> collection)
            : base(new KeyValueOnlyKeyEqualityComparer<TKey, TValue>(comparer), 1000, collection, false) { }

        /// <summary>
        /// Initializes a new instance of the LockFreeDictionary<(Of <(T>)>) class
        /// </summary>
        /// <param name="capacity">The collection initial capacity.</param>
        public ConcurrentDictionary(int capacity)
            : base(new KeyValueOnlyKeyEqualityComparer<TKey, TValue>(), capacity, null, false) { }
        /// <summary>
        /// Initializes a new instance of the LockFreeDictionary<(Of <(T>)>) class
        /// </summary>
        /// <param name="capacity">The collection initial capacity.</param>
        /// <param name="isFixedSize">The collection is fixed to the size passed in the capacity parameter.</param>
        public ConcurrentDictionary(int capacity, bool isFixedSize)
            : base(new KeyValueOnlyKeyEqualityComparer<TKey, TValue>(), capacity, null, isFixedSize) { }

        /// <summary>
        /// Initializes a new instance of the LockFreeDictionary<(Of <(T>)>) class
        /// </summary>
        /// <param name="comparer">This is the comparer used to detect equality between items in the collection. 
        /// If this is set to null the default comparer for the type will be used instead./</param>
        /// <param name="capacity">The collection initial capacity.</param>
        public ConcurrentDictionary(IEqualityComparer<TKey> comparer, int capacity)
            : base(new KeyValueOnlyKeyEqualityComparer<TKey, TValue>(comparer), capacity, null, false) { }
        /// <summary>
        /// Initializes a new instance of the LockFreeDictionary<(Of <(T>)>) class
        /// </summary>
        /// <param name="comparer">This is the comparer used to detect equality between items in the collection. 
        /// If this is set to null the default comparer for the type will be used instead./</param>
        /// <param name="capacity">The collection initial capacity.</param>
        /// <param name="isFixedSize">The collection is fixed to the size passed in the capacity parameter.</param>
        public ConcurrentDictionary(IEqualityComparer<TKey> comparer, int capacity, bool isFixedSize)
            : base(new KeyValueOnlyKeyEqualityComparer<TKey, TValue>(comparer), capacity, null, isFixedSize) { }

        /// <summary>
        /// Initializes a new instance of the LockFreeDictionary<(Of <(T>)>) class
        /// </summary>
        /// <param name="comparer">This is the comparer used to detect equality between items in the collection. 
        /// If this is set to null the default comparer for the type will be used instead./</param>
        /// <param name="capacity">The collection initial capacity.</param>
        /// <param name="collection">The values in this enumeration will be loaded in to the collection. Set this to null if not required.</param>
        public ConcurrentDictionary(IEqualityComparer<TKey> comparer, int capacity, IEnumerable<KeyValuePair<TKey, TValue>> collection)
            : base(new KeyValueOnlyKeyEqualityComparer<TKey, TValue>(comparer), capacity, collection, false) { }
        /// <summary>
        /// Initializes a new instance of the LockFreeDictionary<(Of <(T>)>) class
        /// </summary>
        /// <param name="comparer">This is the comparer used to detect equality between items in the collection. 
        /// If this is set to null the default comparer for the type will be used instead./</param>
        /// <param name="capacity">The collection initial capacity.</param>
        /// <param name="collection">The values in this enumeration will be loaded in to the collection. Set this to null if not required.</param>
        /// <param name="isFixedSize">The collection is fixed to the size passed in the capacity parameter.</param>
        public ConcurrentDictionary(IEqualityComparer<TKey> comparer, int capacity, IEnumerable<KeyValuePair<TKey, TValue>> collection, bool isFixedSize)
            : base(new KeyValueOnlyKeyEqualityComparer<TKey, TValue>(comparer), capacity, collection, isFixedSize) { }

        /// <summary>
        /// Initializes a new instance of the LockFreeDictionary<(Of <(T>)>) class
        /// </summary>
        /// <param name="comparer">This is the comparer that allows a custom value comparer to be set in addition to a custom key comparer.</param>
        /// <param name="capacity">The collection initial capacity.</param>
        /// <param name="collection">The values in this enumeration will be loaded in to the collection. Set this to null if not required.</param>
        /// <param name="isFixedSize">The collection is fixed to the size passed in the capacity parameter.</param>
        protected ConcurrentDictionary(KeyValueOnlyKeyEqualityComparer<TKey, TValue> comparer, int capacity, IEnumerable<KeyValuePair<TKey, TValue>> collection, bool isFixedSize)
            : base(comparer, capacity, collection, isFixedSize) { }

        #endregion // Constructor

        #region KeyOnlyEqualityComparer
        /// <summary>
        /// This is the key only equality comparer.
        /// </summary>
        protected KeyValueOnlyKeyEqualityComparer<TKey, TValue> KeyOnlyEqualityComparer
        {
            get{return (KeyValueOnlyKeyEqualityComparer<TKey, TValue>)mEqualityComparer;}
        }
        #endregion // KeyOnlyEqualityComparer
        #region CollectionAllowNullValues
        /// <summary>
        /// This property determines whether the collection will accept null values. The default setting is false for the Dictionary.
        /// </summary>
        protected override bool CollectionAllowNullValues { get { return false; } }
        #endregion

        #region Add
        /// <summary>
        /// This method adds a key and an item to the collection.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value data.</param>
        public void Add(TKey key, TValue value)
        {
            Add(new KeyValuePair<TKey, TValue>(key, value));
        }
        /// <summary>
        /// This method adds a keyvalue pair to the collection.
        /// </summary>
        /// <param name="item">The keyvalue pair item to add.</param>
        public void Add(KeyValuePair<TKey, TValue> item)
        {
            DisposedCheck();
            if (!Insert(item, true))
                throw new ArgumentException("Key already exists in the collection.");
        }
        #endregion // Add

        #region Contains
        /// <summary>
        /// This method returns true if both the key and value are present in the collection.
        /// </summary>
        /// <param name="item">The keyvalue pair item to check.</param>
        /// <returns>Returns true if the item exists in the collection.</returns>
        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            TValue value;
            if (!TryGetValue(item.Key, out value))
                return false;

            return KeyOnlyEqualityComparer.ValueComparer.Equals(value, item.Value);
        }
        #endregion // Contains
        #region ContainsKey(TKey key)
        /// <summary>
        /// This method returns true if the key is present in the collection.
        /// </summary>
        /// <param name="key">The key to check.</param>
        /// <returns>Returns true if the key is present in the collection.</returns>
        public bool ContainsKey(TKey key)
        {
            TValue value;
            return TryGetValue(key, out value);
        }
        #endregion
        #region ContainsValue(TValue value)
        /// <summary>
        /// This method returns true if the value is present in the collection.
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <returns>Returns true if the value is present in the collection.</returns>
        /// <remarks>For large collections this method may take some time as a full scan of the collection is required to identify the value.</remarks>
        public bool ContainsValue(TValue value)
        {
            DisposedCheck();
            return this.Any(i => KeyOnlyEqualityComparer.ValueComparer.Equals(i.Value, value));
        }
        #endregion

        #region Remove
        /// <summary>
        /// Removes the keyvalue pair from the dictionary.
        /// </summary>
        /// <param name="item">The item to remove.</param>
        /// <returns>Returns true if the item is successfully removed.</returns>
        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            TValue value;
            if (!TryGetValue(item.Key, out value))
                return false;

            if (!KeyOnlyEqualityComparer.ValueComparer.Equals(value, item.Value))
                return false;

            return RemoveInternal(item);
        }
        /// <summary>
        /// Removes the item from the collection, that matches the key passed in the parameter.
        /// </summary>
        /// <param name="key">The key of the item to remove.</param>
        /// <returns>Returns true if the item is successfully removed.</returns>
        public bool Remove(TKey key)
        {
            DisposedCheck();
            if (key == null)
            {
                throw new ArgumentNullException();
            }

            return RemoveInternal(new KeyValuePair<TKey, TValue>(key, default(TValue)));
        }
        #endregion // Remove

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

        #region this[TKey key]
        /// <summary>
        /// This indexer is used to access the items in the collection by use of the key value.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>Returns the value associated with the key.</returns>
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
                DisposedCheck();
                Insert(new KeyValuePair<TKey, TValue>(key, value), false);
            }
        }
        #endregion // this[TKey key]

        #region TryGetValue(TKey key, out TValue value)
        /// <summary>
        /// This method attempts to retrieve an item from the collection.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value parameter.</param>
        /// <returns>Returns true if the item can be found in the collection.</returns>
        public bool TryGetValue(TKey key, out TValue value)
        {
            DisposedCheck();

            KeyValuePair<TKey, TValue> kpValue;
            bool success = TryGetValueInternal(mEqualityComparer
                , new KeyValuePair<TKey, TValue>(key, default(TValue))
                , out kpValue);

            if (success)
            {
                value = kpValue.Value;
                return true;
            }

            value = default(TValue);
            return false;
        }
        #endregion // TryGetValue(TKey key, out TValue value)

        #region Keys
        /// <summary>
        /// The key collection.
        /// </summary>
        public ICollection<TKey> Keys
        {
            get
            {
                throw new NotImplementedException();

                //DisposedCheck();
                //return new DictionaryWrapper<KeyValuePair<TKey, TValue>, TKey>(this, i => i.Key, i => ContainsKey(i));
            }
        }
        #endregion
        #region Values
        /// <summary>
        /// The value collection.
        /// </summary>
        public ICollection<TValue> Values
        {
            get
            {
                throw new NotImplementedException();
                //DisposedCheck();
                //return new DictionaryWrapper<KeyValuePair<TKey, TValue>, TValue>(this, i => i.Value, i => ContainsValue(i));
            }
        }
        #endregion

        #region CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        /// <summary>
        /// This method copies the collection to the array specified.
        /// </summary>
        /// <param name="array">The array to copy to.</param>
        /// <param name="arrayIndex">The array index where the class should start copying to.</param>
        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            DisposedCheck();
            CopyToInternal(array, arrayIndex);
        }
        #endregion // CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    }
}
