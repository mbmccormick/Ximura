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
using System.Threading;
using System.Runtime.Serialization;
using System.Runtime.InteropServices;

using Ximura;
using Ximura.Helper;
#endregion // using
namespace Ximura.Collections
{
    public abstract class CollectionHelperBase<T> : DisposableBase, IEnumerable<T>
    {
        #region Struct -> CollectionState
        /// <summary>
        /// This structure holds the internal state information for the class.
        /// </summary>
        protected struct CollectionState
        {
            /// <summary>
            /// This constructor sets the default value.
            /// </summary>
            public CollectionState(bool isFixedSize, bool allowNullValues, bool allowMultipleEntries, int initialCapacity)
            {
                Version = int.MinValue;
                Count = 0;
                DefaultTCount = 0;

                InitialCapacity = initialCapacity;

                IsFixedSize = isFixedSize;
                AllowNullValues = allowNullValues;
                AllowMultipleEntries = allowMultipleEntries;

                ContainScanUnlocked = true;
                ContainScanUnlockedMiss = 0;
            }

            /// <summary>
            /// The version value.
            /// </summary>
            public volatile int Version;
            /// <summary>
            /// This is the current item count.
            /// </summary>
            public volatile int Count;
            /// <summary>
            /// This is the current default(T) item capacity. 
            /// </summary>
            public volatile int DefaultTCount;

            /// <summary>
            /// This is the initial capacity of the collection.
            /// </summary>
            public int InitialCapacity;
            /// <summary>
            /// This property determines whether the collection is a fixed size. Fixed size collections will reject new records
            /// when the capacity has been reached.
            /// </summary>
            public bool IsFixedSize;
            /// <summary>
            /// This property determines whether the collection will allow null or default(T) values.
            /// </summary>
            public bool AllowNullValues;
            /// <summary>
            /// This property specifies whether the collection accepts multiple entries of the same object.
            /// </summary>
            public bool AllowMultipleEntries;

            /// <summary>
            /// This property specifies whether the contains operation should attempt to scan without locking.
            /// </summary>
            public volatile bool ContainScanUnlocked;
            /// <summary>
            /// This variable contains the number of scan misses.
            /// </summary>
            public volatile int ContainScanUnlockedMiss;
        }
        #endregion // Struct -> CollectionState

        #region Declarations
        /// <summary>
        /// This is the equality comparer for the collection.
        /// </summary>
        protected IEqualityComparer<T> mEqualityComparer;
        /// <summary>
        /// 
        /// </summary>
        protected CollectionState mState;
        #endregion

        #region CollectionAllowMultipleEntries
        /// <summary>
        /// This setting determines whether the collection allows multiple entries of the same object in the collection.
        /// The default setting is true.
        /// </summary>
        protected virtual bool CollectionAllowMultipleEntries { get { return true; } }
        #endregion
        #region CollectionAllowNullValues
        /// <summary>
        /// This property determines whether the collection will accept null values. The default setting is true.
        /// </summary>
        /// <remarks>This property is ignored if the collection is for a value type such as int.</remarks>
        protected virtual bool CollectionAllowNullValues { get { return true; } }
        #endregion
        #region IsFixedSize
        /// <summary>
        /// This property determines whether the collection will dynamically expand when new values are added. 
        /// This property can only be set from the constructor, although this property can be overriden in derived classes to ensure
        /// a particular value.
        /// </summary>
        public virtual bool IsFixedSize { get { return mState.IsFixedSize; } }
        #endregion

        #region Initialize(int capacity, IEnumerable<T> collection)
        /// <summary>
        /// This method initializes the collection.
        /// </summary>
        /// <param name="comparer">The comparer for the collection items.</param>
        /// <param name="capacity">The initial capacity.</param>
        /// <param name="collection">The initial data to load in to the array.</param>
        /// <param name="isFixedSize">This property determines whether the collection is a fixed size.</param>
        protected virtual void Initialize(IEqualityComparer<T> comparer, int capacity, IEnumerable<T> collection, bool isFixedSize)
        {
            if (capacity < 0)
                throw new ArgumentOutOfRangeException("The capacity cannot be less than 0.");

            mEqualityComparer = (comparer == null) ? EqualityComparer<T>.Default : comparer;

            mState = new CollectionState(isFixedSize
                , typeof(T).IsValueType || CollectionAllowNullValues
                , CollectionAllowMultipleEntries
                , capacity);

            InitializeData();

            if (collection != null)
                AddIncomingData(collection);
        }
        #endregion // Initialize(int capacity)
        #region InitializeData()
        /// <summary>
        /// This abstract method initializes the data collection.
        /// </summary>
        protected abstract void InitializeData();
        #endregion // InitializeData()

        #region AddIncomingData(IEnumerable<T> collection)
        /// <summary>
        /// This method adds items to the collection that were passed in the constructor.
        /// You should override this method to implement any specific logic for your collection.
        /// </summary>
        /// <param name="collection">The data to add to the collection.</param>
        protected virtual void AddIncomingData(IEnumerable<T> collection)
        {
            collection.ForEach(i => Insert(i, true));
        }
        #endregion // InitializeCollection(IEnumerable<T> collection)

        #region Insert(T item)
        /// <summary>
        /// This method adds an item to the collection.
        /// </summary>
        /// <param name="item">The item to add.</param>
        /// <param name="add">The property specifies whether the item is overwritten or a new item is added. 
        /// If multiple entries are not allowed an exception is thrown.</param>
        /// <returns>Returns true if the addition is successful.</returns>
        protected abstract bool Insert(T item, bool add);
        #endregion // AddInternal(T item)
        #region ContainsInternal(T item)
        /// <summary>
        /// This method checks whether the item exists in the collection.
        /// </summary>
        /// <param name="item">The item to check.</param>
        /// <returns>Returns true if the item is in the collection.</returns>
        protected abstract bool ContainsInternal(T item);
        #endregion // ContainsInternal(T item)
        #region TryGetValueInternal(IEqualityComparer<T> comparer, T key, out T value)
        /// <summary>
        /// This method attempts to retrieve an item from the collection.
        /// </summary>
        /// <param name="comparer">The specific comparer.</param>
        /// <param name="key">The key.</param>
        /// <param name="value">The value parameter.</param>
        /// <returns>Returns true if the item can be found in the collection.</returns>
        protected abstract bool TryGetValueInternal(IEqualityComparer<T> comparer, T key, out T value);
        #endregion // TryGetValue(TKey key, out TValue value)
        #region RemoveInternal(T item)
        /// <summary>
        /// The method removes an item from the collection.
        /// </summary>
        /// <param name="item">The item to remove.</param>
        /// <returns>Returns true if the removal is successful.</returns>
        protected abstract bool RemoveInternal(T item);
        #endregion // RemoveInternal(T item)
        #region ClearInternal()
        /// <summary>
        /// This method clears the collection.
        /// </summary>
        /// <remarks>This method scans and locks all the items in the collection, but leaves the sentinel data intact.</remarks>
        protected abstract void ClearInternal();
        #endregion // ClearInternal()

        #region CountInternal
        /// <summary>
        /// This is the count of the number of items currently in the collection.
        /// </summary>
        protected internal virtual int CountInternal
        {
            get { return mState.Count; }
        }
        #endregion
        #region VersionInternal
        /// <summary>
        /// This is the current collection version.
        /// </summary>
        protected internal virtual int VersionInternal
        {
            get { return mState.Version; }
        }
        #endregion
        #region DefaultTCountInternal
        /// <summary>
        /// This is the current default(T) item count. 
        /// </summary>
        protected internal virtual int DefaultTCountInternal
        {
            get { return mState.DefaultTCount; }
        }
        #endregion // DefaultTCountInternal

        #region Count
        /// <summary>
        /// This property returns the number of elements in the collection.
        /// </summary>
        public virtual int Count
        {
            get
            {
                DisposedCheck();
                return CountInternal;
            }
        }
        #endregion // Count
        #region Version
        /// <summary>
        /// This is the current collection version.
        /// </summary>
        public virtual int Version
        {
            get
            {
                DisposedCheck();
                return VersionInternal;
            }
        }
        #endregion // Version
        #region IsReadOnly
        /// <summary>
        /// This property always returns false.
        /// </summary>
        public virtual bool IsReadOnly
        {
            get
            {
                DisposedCheck();
                return false;
            }
        }
        #endregion // IsReadOnly

        #region GetEnumerator()
        /// <summary>
        /// This method returns an enumeration of the collection values.
        /// </summary>
        /// <returns>Returns a enumeration of the collection.</returns>
        /// <exception cref="System.InvalidOperationException">This exception will be thrown when if collection 
        /// changes during the enumeration.</exception>
        public abstract IEnumerator<T> GetEnumerator();
        #endregion // GetEnumerator()
        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator)GetEnumerator();
        }

        #endregion

        #region CopyToInternal(T[] array, int arrayIndex)
        /// <summary>
        /// This method copies the collection to the array specified.
        /// </summary>
        /// <param name="array">The array to copy to.</param>
        /// <param name="arrayIndex">The array index where the class should start copying to.</param>
        protected virtual void CopyToInternal(T[] array, int arrayIndex)
        {
            this.ForIndex((i, d) => array[i + arrayIndex] = d);
        }
        /// <summary>
        /// This method copies the collection to the array specified.
        /// </summary>
        /// <param name="array">The array to copy to.</param>
        /// <param name="arrayIndex">The array index where the class should start copying to.</param>
        protected virtual void CopyToInternal(Array array, int arrayIndex)
        {
            this.ForIndex((i, d) => array.SetValue(d, i));
        }

        #endregion // CopyTo(T[] array, int arrayIndex)
        #region ToArrayInternal()
        /// <summary>
        /// This method copies the internal data to an array.
        /// </summary>
        /// <returns>Returns an array containing the internal data.</returns>
        protected virtual T[] ToArrayInternal()
        {
            T[] array = new T[CountInternal];
            CopyToInternal(array, 0);
            return array;
        }
        #endregion // ToArrayInternal()
    }
}
