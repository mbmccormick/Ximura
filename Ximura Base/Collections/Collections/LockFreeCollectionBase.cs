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
using System.Threading;
using System.Runtime.Serialization;
using System.Runtime.InteropServices;

using Ximura;
using Ximura.Helper;
#endregion // using
namespace Ximura.Collections
{
    /// <summary>
    /// The LockFreeCollectionBase class provides generic multi-threaded collection based functionality. The class is designed
    /// to maximize the throughput of the collection in high speed multi-threaded scenarios.
    /// </summary>
    /// <typeparam name="T">The collection class or structure type.</typeparam>
    public abstract partial class LockFreeCollectionBase<T> : IEnumerable<T>, IDisposable
    {
        #region Declarations
        /// <summary>
        /// This variables determines whether the collection has been disposed.
        /// </summary>
        private bool mDisposed = false;
        /// <summary>
        /// This is the equality comparer for the collection.
        /// </summary>
        protected EqualityComparer<T> mEqualityComparer;
#if (x64)
        //This is the 64 bit version. Interlocked works better in a 664 bit environment when working with 64 bit values.
        private long mVersion = int.MinValue;
#else
        /// <summary>
        /// The version value is set for integer for 32bit systems.
        /// </summary>
        protected int mVersion = int.MinValue;
#endif
        /// <summary>
        /// This is the current item count.
        /// </summary>
        protected int mCount = 0;
        /// <summary>
        /// This property determines whether the collection will allow null or default(T) values.
        /// </summary>
        private bool mAllowNullValues;
        private bool mAllowMultipleEntries;
        #endregion

        #region Constructor
        /// <summary>
        /// This is constructor for the abstract list class.
        /// </summary>
        /// <param name="comparer">The comparer for the collection items.</param>
        /// <param name="capacity">The initial capacity for the collection.</param>
        /// <param name="collection">The initial data to load to the collection.</param>
        protected LockFreeCollectionBase(EqualityComparer<T> comparer, int capacity, IEnumerable<T> collection)
        {
#if (PROFILING)
            ProfilingSetup();
#endif
            mEqualityComparer = (comparer == null) ? EqualityComparer<T>.Default : comparer;

            Initialize(capacity, collection);
        }
        #endregion // Constructor
        #region IDisposable Members
        /// <summary>
        /// This is the finalizer for the collection.
        /// </summary>
        ~LockFreeCollectionBase()
        {
            this.Dispose(false);
        }

        #region DisposedCheck()
        /// <summary>
        /// This method identifies when the collection has been disposed and throws an ObjectDisposedException.
        /// </summary>
        /// <exception cref="System.ObjectDisposedException">This exception is thrown when the collection has been disposed.</exception>
        protected virtual void DisposedCheck()
        {
            if (mDisposed)
                throw new ObjectDisposedException(GetType().ToString(), "Collection has been disposed.");
        }
        #endregion // DisposedCheck()

        #region Dispose()
        /// <summary>
        /// This method disposes of the collection.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion // Dispose()

        /// <summary>
        /// This method disposes of the data in the collection. You should override this method if you need to add
        /// custom dispose logic to your collection.
        /// </summary>
        /// <param name="disposing">The class is disposing, i.e. this is called by Dispose and not the finalizer.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                mDisposed = true;
            }
        }

        #endregion

        #region Initialize(int capacity, IEnumerable<T> collection)
        /// <summary>
        /// This method initializes the collection.
        /// </summary>
        /// <param name="capacity">The initial capacity.</param>
        protected virtual void Initialize(int capacity, IEnumerable<T> collection)
        {
            if (capacity < 0)
                throw new ArgumentOutOfRangeException("The capacity cannot be less than 0.");

            mAllowNullValues = typeof(T).IsValueType || AllowNullValues;
            mAllowMultipleEntries = AllowMultipleEntries;

            InitializeData(capacity);
            InitializeBuckets(capacity);

            if (collection != null)
                collection.ForEach(i => AddInternal(i));
        }
        #endregion // Initialize(int capacity)

        #region AllowMultipleEntries
        /// <summary>
        /// This setting determines whether the collection allows multiple entries of the same object in the collection.
        /// The default setting is true.
        /// </summary>
        protected virtual bool AllowMultipleEntries{get{return false;}}
        #endregion // AllowMultiple
        #region AllowNullValues
        /// <summary>
        /// This property determines whether the collection will accept null values. The default setting is true.
        /// </summary>
        /// <remarks>This property is ignored if the collection is for a value type such as int.</remarks>
        protected virtual bool AllowNullValues { get { return true; } }
        #endregion // AllowNullValues

        #region AddInternal(T item)
        /// <summary>
        /// This method adds an item to the collection.
        /// </summary>
        /// <param name="item">The item to add.</param>
        /// <returns>Returns true if the addition is successful.</returns>
        protected virtual bool AddInternal(T item)
        {
            int hashCode, sentinelID, position;
#if (PROFILING)
            int start = Environment.TickCount;
            int has = 0;
            try
            {
#endif
                try
                {
                    //WorldEnter();

                    //Get the hash code and sentinel ID for the item.
                    HashAndSentinel(item, true, out hashCode, out sentinelID);
#if (PROFILING)
                    has = Environment.TickCount;
#endif
                    //Is this a null or default value?
                    if (hashCode == -1)
                    {
                        if (!mAllowNullValues)
                            throw new ArgumentNullException("Null values are not accepted in this collection.");

                        if (!mAllowMultipleEntries)
                        {
                            if (mDefaultTCount > 0)
                                return false;

                            if (Interlocked.CompareExchange(ref mDefaultTCount, 1, 0) != 0)
                                return false;
                        }
                        else
                            Interlocked.Increment(ref mDefaultTCount);

                        Interlocked.Increment(ref mCount);
                        Interlocked.Increment(ref mVersion);
                        return true;
                    }

                    //Ok, add the item to the collection.
                    position = AddInternalWithHashAndSentinel(item, hashCode, sentinelID, mAllowMultipleEntries);

                    //Have we added successfully?
                    if (position > -1)
                    {
                        Interlocked.Increment(ref mCount);
                        Interlocked.Increment(ref mVersion);
                        BitSizeCalculate();
                    }

                    return position > -1;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    //WorldLeave();
                }
#if (PROFILING)
            }
            finally
            {
                Profile(ProfileAction.Time_AddInternal, Environment.TickCount - start);
                Profile(ProfileAction.Time_AddInternalHAS, Environment.TickCount - has);
            }
#endif
        }
        #endregion // AddInternal(T item)
        #region AddInternalWithHashAndSentinel(T item, int hashCode, int index, bool allowMultiple)
        /// <summary>
        /// This method is used internally, specifically for entering sentinel nodes.
        /// </summary>
        /// <param name="item">The item to add.</param>
        /// <param name="hashCode">The item hasCode.</param>
        /// <param name="index">The sentinel ID to start the scan.</param>
        /// <param name="allowMultiple">This property specifies whether multiple entries are allowed.</param>
        /// <returns>Returns the position of the inserted vertex, or -1 if the insertion fails.</returns>
        protected virtual int AddInternalWithHashAndSentinel(T item, int hashCode, int index, bool allowMultiple)
        {
            int insert;
#if (PROFILING)
            int prf_start = Environment.TickCount;
            int prf_endfal=0;
            int prf_endinsert = 0;
            try
            {
#endif
                VertexWindow<T> vWin = new VertexWindow<T>();

                try
                {
                    if (FindAndLock(item, hashCode, index, out vWin) && !allowMultiple)
                        return -1;
#if (PROFILING)
                    prf_endfal = Environment.TickCount;
#endif
                    insert = EmptyGet();

                    mSlots[insert] = new Vertex<T>(hashCode, item, vWin.Curr.NextIDPlus1);
                    mSlots[vWin.CurrIndexPlus1 - 1] = vWin.Insert(insert + 1);
#if (PROFILING)
                    prf_endinsert = Environment.TickCount;
#endif
                    return insert;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    VertexWindowUnlock(vWin);
                }
#if (PROFILING)
            }
            finally
            {
                Profile(ProfileAction.Time_AddIntHAS, Environment.TickCount - prf_start);
                Profile(ProfileAction.Time_AddIntHAS_FindAndLock, prf_endfal - prf_start);
                Profile(ProfileAction.Time_AddIntHAS_Insert, prf_endinsert - prf_endfal);
            }
#endif
        }
        #endregion // AddInternalWithHashAndSentinel(T item, int hashCode, int index, bool allowMultiple)

        #region RemoveInternal(T item)
        /// <summary>
        /// The method removes an item from the collection.
        /// </summary>
        /// <param name="item">The item to remove.</param>
        /// <returns>Returns true if the removal is successful.</returns>
        protected virtual bool RemoveInternal(T item)
        {
            try
            {
                //WorldEnter();

                int hashCode, sentinelID;
                HashAndSentinel(item, false, out hashCode, out sentinelID);

                //Is this a null or default value?
                if (hashCode == -1)
                {
                    if (!mAllowNullValues)
                        return false;

                    int currentCount = mDefaultTCount;
                    if (currentCount == 0)
                        return false;

                    //We check whether another thread has changes the value before us.
                    while (Interlocked.CompareExchange(ref mDefaultTCount, currentCount - 1, currentCount) != currentCount)
                    {
                        currentCount = mDefaultTCount;

                        if (currentCount == 0)
                            return false;
                    }

                    Interlocked.Decrement(ref mCount);
                    Interlocked.Increment(ref mVersion);
                    return true;
                }

                VertexWindow<T> vWin = new VertexWindow<T>();

                //Get the window for the item in the collection.
                if (!FindAndLock(item, hashCode, sentinelID, out vWin))
                {
                    //OK, item cannot be found. Release the lock on the current item and leave.
                    VertexWindowUnlock(vWin);
                    return false;
                }

                //Snip out the item.
                mSlots[vWin.CurrIndexPlus1 - 1] = vWin.Snip();
                //Update the version and reduce the item count.
                Interlocked.Decrement(ref mCount);
                Interlocked.Increment(ref mVersion);

                //Release the parent lock so scans on other threads can continue.
                VertexWindowUnlock(vWin);

                //Add the index to the empty item for re-allocation.
                EmptyAdd(vWin.Curr.NextIDPlus1 - 1);

                return true;
            }
            finally
            {
                //WorldLeave();
            }
        }
        #endregion // RemoveInternal(T item)

        #region ContainsInternal(T item)
        /// <summary>
        /// This method checks whether the item exists in the collection.
        /// </summary>
        /// <param name="item">The item to check.</param>
        /// <returns>Returns true if the item is in the collection.</returns>
        protected virtual bool ContainsInternal(T item)
        {
#if (PROFILING)
            int start = Environment.TickCount;
            int endhal = 0;
            try
            {
#endif
                try
                {
                    //WorldEnter();

                    int hashCode, index;
                    HashAndSentinel(item, false, out hashCode, out index);

#if (PROFILING)
                    endhal = Environment.TickCount;
#endif
                    //Is this a null or default value?
                    if (hashCode == -1)
                        return mAllowNullValues ? mDefaultTCount > 0 : false;

                    VertexWindow<T> vWin = new VertexWindow<T>();
                    bool success = false;
                    try
                    {
                        success = FindAndLock(item, hashCode, index, out vWin);
                    }
                    finally
                    {
                        VertexWindowUnlock(vWin);
                    }

                    return success;
                }
                finally
                {
                    //WorldLeave();
                }
#if (PROFILING)
            }
            finally
            {
                Profile(ProfileAction.Time_ContainsTot, Environment.TickCount - start);
                Profile(ProfileAction.Time_ContainsHAL, endhal - start);
            }
#endif
        }
        #endregion // ContainsInternal(T item)

        #region VertexWindowUnlock(VertexWindow<T> vWin)
        /// <summary>
        /// This method provides common functionality to unlock a VertexWindow.
        /// </summary>
        /// <param name="vWin">The VertexWindow containing the lock data.</param>
        protected void VertexWindowUnlock(VertexWindow<T> vWin)
        {
            if (vWin.Curr.NextIDPlus1 > 0) mSlots.ItemUnlock(vWin.Curr.NextIDPlus1 - 1);
            if (vWin.CurrIndexPlus1 > 0) mSlots.ItemUnlock(vWin.CurrIndexPlus1 - 1);
        }
        #endregion // VertexWindowUnlock(VertexWindow<T> vWin)

        #region ClearInternal()
        /// <summary>
        /// This method clears the collection.
        /// </summary>
        protected virtual void ClearInternal()
        {
            try
            {
                //WorldStop();

                throw new NotImplementedException();

            }
            finally
            {
                //WorldRelease();
            }
        }
        #endregion // ClearInternal()

        #region CountInternal
        /// <summary>
        /// This is the count of the number of items currently in the collection.
        /// </summary>
        protected virtual int CountInternal
        {
            get { return mCount; }
        }
        #endregion

        #region Version
        /// <summary>
        /// This is the current collection version.
        /// </summary>
        public long Version
        {
            get
            {
                DisposedCheck();
                return mVersion;
            }
        }
        #endregion // Version
        #region VersionCheck(long version)
        private bool VersionCheck(long version)
        {
            return mVersion == version;
        }
        #endregion // VersionCheck(long version)
    }
}
