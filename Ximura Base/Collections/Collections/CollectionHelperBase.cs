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
    /// <summary>
    /// This is the base class that both struct and class based collections inherit from.
    /// </summary>
    /// <typeparam name="T">The collection object type.</typeparam>
    public abstract partial class CollectionHelperBase<T, A> : DisposableBase, IEnumerable<T>
        where A : class, IVertexArray<T>, new()
    {
        #region Declarations
        /// <summary>
        /// This array holds both the data.
        /// </summary>
        protected A mData;
        /// <summary>
        /// This is the equality comparer for the collection.
        /// </summary>
        protected internal IEqualityComparer<T> mEqualityComparer;
        #endregion

        #region Constructor
        /// <summary>
        /// This is constructor for the abstract list class.
        /// </summary>
        /// <param name="comparer">The comparer for the collection items.</param>
        /// <param name="capacity">The initial capacity for the collection.</param>
        /// <param name="collection">The initial data to load to the collection.</param>
        /// <param name="isFixedSize">This property determines whether the collection is a fixed size.
        /// Fixed size collections will reject new records when the capacity has been reached, 
        /// although they may deliver performance improvements as they do not need to use a growable data structure.</param>
        protected CollectionHelperBase(IEqualityComparer<T> comparer, int capacity, IEnumerable<T> collection, bool isFixedSize)
        {
#if (PROFILING)
            ProfilingSetup();
#endif
            Initialize(comparer, capacity, collection, isFixedSize);
        }
        #endregion // Constructor
        #region Dispose(bool disposing)
        /// <summary>
        /// This method disposes of the data in the collection. You should override this method if you need to add
        /// custom dispose logic to your collection.
        /// </summary>
        /// <param name="disposing">The class is disposing, i.e. this is called by Dispose and not the finalizer.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                //Clear the collection. This removes all references to any contained objects.
                ClearInternal();
                mData = null;
            }
        }
        #endregion // Dispose(bool disposing)

        #region CollectionAllowMultipleEntries
        /// <summary>
        /// This setting determines whether the collection allows multiple entries of the same object in the collection.
        /// The default setting is true.
        /// </summary>
        protected virtual bool CollectionAllowMultipleEntries { get { return false; } }
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
        public virtual bool IsFixedSize { get { return mData.IsFixedSize; } }
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
#if (DEBUG)
            DebugReset();
#endif
            if (capacity < 0)
                throw new ArgumentOutOfRangeException("The capacity cannot be less than 0.");

            mEqualityComparer = (comparer == null) ? EqualityComparer<T>.Default : comparer;

            InitializeData(mEqualityComparer, isFixedSize, capacity);

            if (collection != null)
                AddIncomingData(collection);
        }
        #endregion // Initialize(int capacity)
        #region InitializeData()
        /// <summary>
        /// This abstract method initializes the data collection.
        /// </summary>
        protected virtual void InitializeData(IEqualityComparer<T> comparer, bool isFixedSize, int capacity)
        {
            mData = new A();

            mData.Initialize(comparer, isFixedSize
                , capacity
                , typeof(T).IsValueType || CollectionAllowNullValues
                ,CollectionAllowMultipleEntries);
            
        }
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
        protected internal virtual bool Insert(T item, bool add)
        {
            #region Profiling
#if (PROFILING)
            int hopsData = 0;
            int start = Environment.TickCount;
            int has = 0;
            int hopsSentinel = 0;
            int hopsBucketSkip = 0;
            try
            {
#endif
            #endregion

            if (mData.SupportsFastAdd)
                return mData.FastAdd(item, add);

            try
            {
                //Is this a null or default value?
                if (mEqualityComparer.Equals(item, default(T)))
                    return mData.DefaultTAdd();

                //Add any required sentinels and retrieve the nearest sentinel slot ID.
                IVertexWindow<T> vWin = mData.VertexWindowGet(item, true);

                #region Profiling
#if (PROFILING)
                    has = Environment.TickCount;
#endif
                #endregion
                //Ok, let's add the data from the sentinel position.
                //Lock the start index and initialize the window.
#if (PROFILING)
                    hopsData = vWin.ScanAndLock();
#else
                vWin.ScanAndLock();
#endif
                //Ok, we need to scan for hash collisions and multiple entries.
                while (vWin.ScanProcess())
                {
                    if (vWin.ScanItemMatch)
                    {
                        //Ok, we have a match.
                        if (!add)
                        {
                            //This code is to accomodate dictionary type collections where the item is a keyvalue pair.
                            vWin.ItemSetNext();
                            vWin.Unlock();
                            return true;
                        }

                        if (mData.AllowMultipleEntries)
                            break;
                        else
                        {
                            vWin.Unlock();
                            return false;
                        }
                    }
                    vWin.MoveUp();
#if (PROFILING)
                        hopsData++;
#endif
                }

                //Ok, add the data in the collection.
                try
                {
                    vWin.ItemInsert();
                }
                catch (Exception exx)
                {
                    throw exx;
                }

                vWin.Unlock();

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            #region Profiling
#if (PROFILING)
            }
            finally
            {
                Profile(ProfileAction.Count_HopData, hopsData);
                Profile(ProfileAction.Count_HopBucketSkip, hopsBucketSkip);
                Profile(ProfileAction.Count_HopSentinel, hopsSentinel);

                Profile(ProfileAction.Time_AddInternal, Environment.TickCount - start);
                if (has > 0)
                    Profile(ProfileAction.Time_AddInternalHAS, has - start);
            }
#endif
            #endregion
        }
        #endregion // AddInternal(T item)
        #region ContainsInternal(T item)
        /// <summary>
        /// This method checks whether the item exists in the collection.
        /// </summary>
        /// <param name="item">The item to check.</param>
        /// <returns>Returns true if the item is in the collection.</returns>
        protected internal virtual bool ContainsInternal(T item)
        {
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

            if (mData.SupportsFastContain)
            {
                bool? resultFastScan = mData.FastContains(item);
                if (resultFastScan.HasValue)
                    return resultFastScan.Value;
            }

            //Is this a null or default value?
            if (mEqualityComparer.Equals(item, default(T)))
                return mData.DefaultTContains();

#if (PROFILING)
            endhal = Environment.TickCount - start;
#endif
            ////Ok, let's add the data from the sentinel position.
            ////Lock the start index and initialize the window.
            IVertexWindow<T> vWin = mData.VertexWindowGet(item, false);

            //Ok, find the first instance of the hashID.
#if (PROFILING)
            hopCount = vWin.ScanAndLock();
#else
            vWin.ScanAndLock();
#endif
            //Ok, we need to scan for hash collisions and multiple entries.
            while (vWin.ScanProcess())
            {
                if (vWin.ScanItemMatch)
                {
                    vWin.Unlock();
                    return true;
                }

                vWin.MoveUp();
#if (PROFILING)
                hopCount++;
#endif
            }

            vWin.Unlock();
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
        #endregion // ContainsInternal(T item)
        #region TryGetValueInternal(IEqualityComparer<T> comparer, T key, out T value)
        /// <summary>
        /// This method attempts to retrieve an item from the collection.
        /// </summary>
        /// <param name="comparer">The specific comparer.</param>
        /// <param name="key">The key.</param>
        /// <param name="value">The value parameter.</param>
        /// <returns>Returns true if the item can be found in the collection.</returns>
        protected internal virtual bool TryGetValueInternal(IEqualityComparer<T> comparer, T key, out T value)
        {
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

                if (mData.SupportsFastContain)
                {
                    bool? resultFastScan = mData.FastContains(comparer, key, out value);
                    if (resultFastScan.HasValue)
                        return resultFastScan.Value;
                }
                //Ok, we have a scan miss.
                mData.ContainScanUnlockedMiss();

            //Get the hash code for the item.
            int hashCode = comparer.GetHashCode(key);

#if (PROFILING)
                endhal = Environment.TickCount - start;
#endif
            //Ok, let's add the data from the sentinel position.
            //Lock the start index and initialize the window.
            IVertexWindow<T> vWin = mData.VertexWindowGet(key, false);


            //Ok, find the first instance of the hashID.
#if (PROFILING)
                hopCount = vWin.ScanAndLock();
#else
            vWin.ScanAndLock();
#endif
            //Ok, we need to scan for hash collisions and multiple entries.
            while (vWin.ScanProcess())
            {
                value = vWin.NextData;
                if (vWin.ScanItemMatch)
                {
                    vWin.Unlock();
                    return true;
                }

                vWin.MoveUp();
#if (PROFILING)
                    hopCount++;
#endif
            }

            vWin.Unlock();
            value = default(T);
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
        #region RemoveInternal(T item)
        /// <summary>
        /// The method removes an item from the collection.
        /// </summary>
        /// <param name="item">The item to remove.</param>
        /// <returns>Returns true if the removal is successful.</returns>
        protected internal virtual bool RemoveInternal(T item)
        {
            if (mData.SupportsFastRemove)
                return mData.FastRemove(item);

            //Is this a null or default value?
            if (mEqualityComparer.Equals(item, default(T)))
                return mData.DefaultTDelete();

            IVertexWindow<T> vWin = mData.VertexWindowGet(item, false);

            vWin.ScanAndLock();

            //Ok, we need to scan for hash collisions and multiple entries.
            while (vWin.ScanProcess())
            {
                if (vWin.ScanItemMatch)
                {
                    //Remove the item from the linked list.
                    vWin.ItemRemoveAndUnlock();
                    return true;
                }

                vWin.MoveUp();
            }

            vWin.Unlock();
            //Ok, the item cannot be found.
            return false;
        }
        #endregion // RemoveInternal(T item)
        #region ClearInternal()
        /// <summary>
        /// This method clears the collection.
        /// </summary>
        /// <remarks>This method scans and locks all the items in the collection, but leaves the sentinel data intact.</remarks>
        protected internal virtual void ClearInternal()
        {
            if (mData.SupportsFastClear)
            {
                mData.FastClear();
                return;
            }

            //Clear the DefaultT counters.
            mData.DefaultTClear();

            //Get the window from the root sentinel.
            IVertexWindow<T> vWin = mData.VertexWindowGet();

            //Ok, we need to scan for hash collisions and multiple entries.
            while (!vWin.CurrIsTerminator)
            {
                if (vWin.NextIsSentinel)
                    vWin.MoveUp();
                else
                {
                    //Remove the item from the linked list and lock and move up the next item.
                    vWin.Snip();
                }
            }

            vWin.Unlock();
        }
        #endregion // ClearInternal()

        #region Count
        /// <summary>
        /// This property returns the number of elements in the collection.
        /// </summary>
        public virtual int Count
        {
            get
            {
                DisposedCheck();
                return mData.Count;
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
                return mData.Version;
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

        #region InternalScan(bool changeException)
        /// <summary>
        /// This method enumerates through the collection.
        /// </summary>
        /// <param name="changeException">Set this to true if you want the method to throw an exception if the collection changes.</param>
        /// <returns>Returns a enumeration of the collection.</returns>
        /// <exception cref="System.InvalidOperationException">This exception will be thrown when the collection 
        /// changes during the scan and the changeException parameter is set to true.</exception>
        protected internal virtual IEnumerable<KeyValuePair<int, ICollectionVertex<T>>> InternalScan(bool changeException)
        {
            if (mData.Count == 0)
                yield break;

            int currentVersion = mData.Version;

            foreach (var item in mData)
            {
                if (changeException && !mData.VersionCompare(currentVersion))
                    throw new InvalidOperationException("The version has changed");

                yield return item;
            }
        }
        #endregion // InternalScan(bool changeException)
        #region GetEnumerator()
        /// <summary>
        /// This method returns an enumeration of the collection values.
        /// </summary>
        /// <returns>Returns a enumeration of the collection.</returns>
        /// <exception cref="System.InvalidOperationException">This exception will be thrown when if collection 
        /// changes during the enumeration.</exception>
        public virtual IEnumerator<T> GetEnumerator()
        {
            //Enumerate the default(T) values.
            for (int i = mData.DefaultTCount; i > 0; i--)
                yield return default(T);
            //Enumerate the data.
            foreach (var item in InternalScan(true))
                if (!item.Value.IsSentinel)
                    yield return item.Value.Data;
        }
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
        protected internal virtual void CopyToInternal(T[] array, int arrayIndex)
        {
            this.ForIndex((i, d) => array[i + arrayIndex] = d);
        }
        /// <summary>
        /// This method copies the collection to the array specified.
        /// </summary>
        /// <param name="array">The array to copy to.</param>
        /// <param name="arrayIndex">The array index where the class should start copying to.</param>
        protected internal virtual void CopyToInternal(Array array, int arrayIndex)
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
            T[] array = new T[mData.Count];
            CopyToInternal(array, 0);
            return array;
        }
        #endregion // ToArrayInternal()
    }
}
