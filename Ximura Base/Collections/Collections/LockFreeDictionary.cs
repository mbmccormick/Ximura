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
    /// <typeparam name="TKey">The key type.</typeparam>
    /// <typeparam name="TValue">The value type.</typeparam>
    [DebuggerDisplay("Count = {Count}"), HostProtection(SecurityAction.LinkDemand, MayLeakOnAbort = true)]
    public class LockFreeDictionary<TKey, TValue> : LockFreeCollectionBase<KeyValuePair<TKey, TValue>>, IDictionary<TKey, TValue>
    {
        #region Class -> DictionaryWrapper
        /// <summary>
        /// This helper class provides the functionality for the Key and Value enumerators.
        /// </summary>
        /// <typeparam name="TColl">The base collection type.</typeparam>
        /// <typeparam name="T">The collection type.</typeparam>
        protected class DictionaryWrapper<TColl, T> : ICollection<T>
        {
            #region Declarations
            LockFreeCollectionBase<TColl> mColl;
            Func<TColl, T> mfnOutput;
            Predicate<T> mfnContains;
            #endregion // Declarations
            #region Constructor
            /// <summary>
            /// This helper class provides additional functionality for the Key and Value enumerators.
            /// </summary>
            /// <param name="coll">The base collection.</param>
            /// <param name="output">The function to output the specific value from the collection.</param>
            /// <param name="contains">The function to check the item exists in the collection.</param>
            public DictionaryWrapper(LockFreeCollectionBase<TColl> coll, Func<TColl, T> output, Predicate<T> contains)
            {
                mColl = coll;
                mfnOutput = output;
                mfnContains = contains;
            }
            #endregion // Constructor

            #region NotSupported Members

            /// <summary>
            /// This method is not supported.
            /// </summary>
            public void Add(T item)
            {
                throw new NotSupportedException();
            }
            /// <summary>
            /// This method is not supported.
            /// </summary>
            public void Clear()
            {
                throw new NotSupportedException();
            }
            /// <summary>
            /// This method is not supported.
            /// </summary>
            public bool Remove(T item)
            {
                throw new NotSupportedException();
            }

            #endregion

            #region Contains(T item)
            /// <summary>
            /// This function checks the item exists in the base collection.
            /// </summary>
            /// <param name="item">The item to check.</param>
            /// <returns>Returns true if the item is in the collection.</returns>
            public bool Contains(T item)
            {
                return mfnContains(item);
            }
            #endregion // Contains(T item)
            #region CopyTo(T[] array, int arrayIndex)
            /// <summary>
            /// This method copies the base collection data to the array specified.
            /// </summary>
            /// <param name="array">The array to copy the data to.</param>
            /// <param name="arrayIndex">The index position to copy the data to.</param>
            public void CopyTo(T[] array, int arrayIndex)
            {
                mColl.DisposedCheck();

                mColl.InternalScan(true).ForIndex((i, item) => array[i + arrayIndex] = mfnOutput(item.Value.Value));
            }
            #endregion // CopyTo(T[] array, int arrayIndex)
            #region Count
            /// <summary>
            /// The collection count.
            /// </summary>
            public int Count
            {
                get 
                {
                    mColl.DisposedCheck();
                    return mColl.CountInternal; 
                }
            }
            #endregion // Count

            #region IsReadOnly
            /// <summary>
            /// This property always returns true.
            /// </summary>
            public bool IsReadOnly
            {
                get { return true; }
            }
            #endregion // IsReadOnly

            #region IEnumerable<T> Members
            /// <summary>
            /// This method returns an enumeration of the specific base data.
            /// </summary>
            /// <returns>Returns an enumerator.</returns>
            public IEnumerator<T> GetEnumerator()
            {
                mColl.DisposedCheck();
                //Enumerate the data.
                foreach (var item in mColl.InternalScan(true))
                    if (!item.Value.IsSentinel)
                        yield return mfnOutput(item.Value.Value);
            }
            #endregion

            #region IEnumerable Members

            IEnumerator IEnumerable.GetEnumerator()
            {
                return (IEnumerator)GetEnumerator();
            }

            #endregion
        }
        #endregion // Class -> DictionaryWrapper
        #region Class -> KeyValueOnlyKeyEqualityComparer
        /// <summary>
        /// This comparer is used to only report the key for the hashcode and equality comparer.
        /// </summary>
        /// <typeparam name="TKey">The key type.</typeparam>
        /// <typeparam name="TValue">The value type.</typeparam>
        public class KeyValueOnlyKeyEqualityComparer<TKey, TValue> : IEqualityComparer<KeyValuePair<TKey, TValue>>
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
        #endregion // Class -> KeyValueOnlyKeyEqualityComparer

        #region Constructor
        /// <summary>
        /// This is the default constructor. The collection will be constructed with a base capacity of 1000.
        /// </summary>
        public LockFreeDictionary()
            : base(new KeyValueOnlyKeyEqualityComparer<TKey, TValue>(), 1000, null, false) { }

        /// <summary>
        /// Initializes a new instance of the LockFreeDictionary<(Of <(T>)>) class
        /// </summary>
        /// <param name="collection">The values in this enumeration will be loaded in to the collection.</param>
        public LockFreeDictionary(IEnumerable<KeyValuePair<TKey,TValue>> collection)
            : base(new KeyValueOnlyKeyEqualityComparer<TKey, TValue>(), 1000, collection, false) { }

        /// <summary>
        /// Initializes a new instance of the LockFreeDictionary<(Of <(T>)>) class
        /// </summary>
        /// <param name="collection">The values in this enumeration will be loaded in to the collection. Set this to null if not required.</param>
        /// <param name="isFixedSize">The collection is fixed to the size passed in the capacity parameter.</param>
        public LockFreeDictionary(IEnumerable<KeyValuePair<TKey, TValue>> collection, bool isFixedSize)
            : base(new KeyValueOnlyKeyEqualityComparer<TKey, TValue>(), isFixedSize ? -1 : 1000, collection, isFixedSize) { }

        /// <summary>
        /// Initializes a new instance of the LockFreeDictionary<(Of <(T>)>) class
        /// </summary>
        /// <param name="collection">The values in this enumeration will be loaded in to the collection. Set this to null if not required.</param>
        /// <param name="comparer">This is the comparer used to detect equality between items in the collection. 
        /// If this is set to null the default comparer for the type will be used instead./</param>
        public LockFreeDictionary(IEqualityComparer<TKey> comparer, IEnumerable<KeyValuePair<TKey, TValue>> collection)
            : base(new KeyValueOnlyKeyEqualityComparer<TKey, TValue>(comparer), 1000, collection, false) { }

        /// <summary>
        /// Initializes a new instance of the LockFreeDictionary<(Of <(T>)>) class
        /// </summary>
        /// <param name="capacity">The collection initial capacity.</param>
        public LockFreeDictionary(int capacity)
            : base(new KeyValueOnlyKeyEqualityComparer<TKey, TValue>(), capacity, null, false) { }
        /// <summary>
        /// Initializes a new instance of the LockFreeDictionary<(Of <(T>)>) class
        /// </summary>
        /// <param name="capacity">The collection initial capacity.</param>
        /// <param name="isFixedSize">The collection is fixed to the size passed in the capacity parameter.</param>
        public LockFreeDictionary(int capacity, bool isFixedSize)
            : base(new KeyValueOnlyKeyEqualityComparer<TKey, TValue>(), capacity, null, isFixedSize) { }

        /// <summary>
        /// Initializes a new instance of the LockFreeDictionary<(Of <(T>)>) class
        /// </summary>
        /// <param name="comparer">This is the comparer used to detect equality between items in the collection. 
        /// If this is set to null the default comparer for the type will be used instead./</param>
        /// <param name="capacity">The collection initial capacity.</param>
        public LockFreeDictionary(IEqualityComparer<TKey> comparer, int capacity)
            : base(new KeyValueOnlyKeyEqualityComparer<TKey, TValue>(comparer), capacity, null, false) { }
        /// <summary>
        /// Initializes a new instance of the LockFreeDictionary<(Of <(T>)>) class
        /// </summary>
        /// <param name="comparer">This is the comparer used to detect equality between items in the collection. 
        /// If this is set to null the default comparer for the type will be used instead./</param>
        /// <param name="capacity">The collection initial capacity.</param>
        /// <param name="isFixedSize">The collection is fixed to the size passed in the capacity parameter.</param>
        public LockFreeDictionary(IEqualityComparer<TKey> comparer, int capacity, bool isFixedSize)
            : base(new KeyValueOnlyKeyEqualityComparer<TKey, TValue>(comparer), capacity, null, isFixedSize) { }

        /// <summary>
        /// Initializes a new instance of the LockFreeDictionary<(Of <(T>)>) class
        /// </summary>
        /// <param name="comparer">This is the comparer used to detect equality between items in the collection. 
        /// If this is set to null the default comparer for the type will be used instead./</param>
        /// <param name="capacity">The collection initial capacity.</param>
        /// <param name="collection">The values in this enumeration will be loaded in to the collection. Set this to null if not required.</param>
        public LockFreeDictionary(IEqualityComparer<TKey> comparer, int capacity, IEnumerable<KeyValuePair<TKey, TValue>> collection)
            : base(new KeyValueOnlyKeyEqualityComparer<TKey, TValue>(comparer), capacity, collection, false) { }
        /// <summary>
        /// Initializes a new instance of the LockFreeDictionary<(Of <(T>)>) class
        /// </summary>
        /// <param name="comparer">This is the comparer used to detect equality between items in the collection. 
        /// If this is set to null the default comparer for the type will be used instead./</param>
        /// <param name="capacity">The collection initial capacity.</param>
        /// <param name="collection">The values in this enumeration will be loaded in to the collection. Set this to null if not required.</param>
        /// <param name="isFixedSize">The collection is fixed to the size passed in the capacity parameter.</param>
        public LockFreeDictionary(IEqualityComparer<TKey> comparer, int capacity, IEnumerable<KeyValuePair<TKey, TValue>> collection, bool isFixedSize)
            : base(new KeyValueOnlyKeyEqualityComparer<TKey, TValue>(comparer), capacity, collection, isFixedSize) { }

        /// <summary>
        /// Initializes a new instance of the LockFreeDictionary<(Of <(T>)>) class
        /// </summary>
        /// <param name="comparer">This is the comparer that allows a custom value comparer to be set in addition to a custom key comparer.</param>
        /// <param name="capacity">The collection initial capacity.</param>
        /// <param name="collection">The values in this enumeration will be loaded in to the collection. Set this to null if not required.</param>
        /// <param name="isFixedSize">The collection is fixed to the size passed in the capacity parameter.</param>
        public LockFreeDictionary(KeyValueOnlyKeyEqualityComparer<TKey, TValue> comparer, int capacity, IEnumerable<KeyValuePair<TKey, TValue>> collection, bool isFixedSize)
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
            DisposedCheck();
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
            DisposedCheck();
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
            DisposedCheck();
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
        #region Count
        /// <summary>
        /// The collection count.
        /// </summary>
        public int Count
        {
            get { return CountInternal; }
        }
        #endregion // Count
        #region IsReadOnly
        /// <summary>
        /// The collection is read only.
        /// </summary>
        public bool IsReadOnly
        {
            get { return false; }
        }
        #endregion // IsReadOnly

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
            #region Profiling
#if (PROFILING)
            int hopCount = 0;
            int start = Environment.TickCount;
            int endhal = 0;
            int slotLocks1 = 0;
            int slotLocks2 = 0;
            try
            {
#endif
            #endregion
                //Get the hash code for the item.
                int hashCode = KeyOnlyEqualityComparer.KeyComparer.GetHashCode(key);

                int sentIndexID, hashID;
                mData.GetSentinelID(hashCode, true, out sentIndexID, out hashID);
#if (PROFILING)
                endhal = Environment.TickCount - start;
#endif
                //Can we scan without locking?
                if (mContainScanUnlocked)
                {
                    int currVersion = mVersion;

                    //Get the initial sentinel vertex. No need to check locks as sentinels rarely change.
                    int scanPosition = sentIndexID;
                    Vertex<KeyValuePair<TKey, TValue>> scanVertex = mData[scanPosition];

                    //First we will attempt to search without locking. However, should the version ID change 
                    //during the search we will need to complete a locked search to ensure consistency.
                    while (mVersion == currVersion)
                    {
                        //Do we have a match?
                        if (!scanVertex.IsSentinel &&
                            scanVertex.HashID == hashID &&
                            KeyOnlyEqualityComparer.KeyComparer.Equals(key, scanVertex.Value.Key))
                        {
                            value = scanVertex.Value.Value;
                            return true;
                        }

                        //Is this the end of the line
                        if (scanVertex.IsTerminator || scanVertex.HashID > hashID)
                        {
                            value = default(TValue);
                            return false;
                        }
#if (PROFILING)
                        hopCount++;
#endif
                        scanPosition = scanVertex.NextSlotIDPlus1 - 1;
                        //slotLocks1 += mSlots.ItemLockWait(scanPosition);
                        scanVertex = mData[scanPosition];
                    }
                }

                //Ok, we have a scan miss.
                Interlocked.Increment(ref mContainScanUnlockedMiss);

                //Ok, let's add the data from the sentinel position.
                //Lock the start index and initialize the window.
                CombinedVertexArray<KeyValuePair<TKey, TValue>>.VertexWindow<KeyValuePair<TKey, TValue>> vWin
                    = mData.VertexWindowGet(sentIndexID);

                //Ok, find the first instance of the hashID.
#if (PROFILING)
                hopCount = vWin.ScanAndLock(hashID);
#else
                vWin.ScanAndLock(hashID);
#endif
                //Ok, we need to scan for hash collisions and multiple entries.
                while (!vWin.Curr.IsTerminator && vWin.Next.HashID == hashID)
                {
                    if (!vWin.Next.IsSentinel && KeyOnlyEqualityComparer.KeyComparer.Equals(key, vWin.Next.Value.Key))
                    {
                        vWin.Unlock();
                        value = vWin.Next.Value.Value;
                        return true;
                    }

                    vWin.MoveUp();
#if (PROFILING)
                    hopCount++;
#endif
                }

                vWin.Unlock();
                value = default(TValue);
                return false;
                #region Profiling
#if (PROFILING)
            }
            finally
            {
                //Profile(ProfileAction.Time_FindAndLock, Environment.TickCount - start);
                Profile(ProfileAction.Count_FindAndLockHopCount, hopCount);
                Profile(ProfileAction.Count_FindAndLockSlotLocks, slotLocks1 + slotLocks2);
                Profile(ProfileAction.Time_ContainsTot, Environment.TickCount - start);
                Profile(ProfileAction.Time_ContainsHAL, endhal);
            }
#endif
                #endregion
        }
        #endregion // TryGetValue(TKey key, out TValue value)
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
                DisposedCheck();
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

        #region Keys
        /// <summary>
        /// The key collection.
        /// </summary>
        public ICollection<TKey> Keys
        {
            get
            {
                DisposedCheck();
                return new DictionaryWrapper<KeyValuePair<TKey, TValue>, TKey>(this, i => i.Key, i => ContainsKey(i));
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
                DisposedCheck();
                return new DictionaryWrapper<KeyValuePair<TKey, TValue>, TValue>(this, i => i.Value, i => ContainsValue(i));
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
