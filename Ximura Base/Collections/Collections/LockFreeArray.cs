//#region Copyright
//// *******************************************************************************
//// Copyright (c) 2000-2009 Paul Stancer.
//// All rights reserved. This program and the accompanying materials
//// are made available under the terms of the Eclipse Public License v1.0
//// which accompanies this distribution, and is available at
//// http://www.eclipse.org/legal/epl-v10.html
////
//// Contributors:
////     Paul Stancer - initial implementation
//// *******************************************************************************
//#endregion
//#region using
//using System;
//using System.Linq;
//using System.ComponentModel;
//using System.Collections;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.Reflection;
//using System.Runtime.Serialization;
//using System.Threading;

//using Ximura;
//using Ximura.Helper;
//#endregion // using
//namespace Ximura.Collections
//{
//    public partial class LockFreeArray<T> : IEnumerable<T>
//    {
//        #region Declarations
//        protected LockFreeArrayState mState;

//        private int mLength = 0;
//        private int mCapacity = 0;

//        private T[] mArray;

//        private LockFreeArray<T> mNext = null;

//        private ReaderWriterLockSlim mCapacityLock = null;
//        #endregion // Declarations

//        #region Constructor
//        public LockFreeArray():this (0){}

//        public LockFreeArray(int capacity):this (new LockFreeArrayState(), false, capacity)
//        {
//        }

//        protected LockFreeArray(LockFreeArrayState state, bool child, int capacity)
//        {
//            mState = state;

//            SetCapacity(capacity);

//            if (!child)
//                mCapacityLock = new ReaderWriterLockSlim();
//        }
//        #endregion // Constructor

//        #region Expand(int length)
//        /// <summary>
//        /// This method expands the array be the specified amount.
//        /// </summary>
//        /// <param name="length">The length to grow the array.</param>
//        public void Expand(int length)
//        {
//            try
//            {
//                WriteLockAcquire();
//                SetCapacity(mLength + length);
//            }
//            finally
//            {
//                WriteLockRemove();
//            }

//        }
//        #endregion // Expand(int length)
//        #region SetCapacity(int length)
//        /// <summary>
//        /// This method sets the array collection capacity to the specified amount.
//        /// </summary>
//        /// <param name="length">The total length of the array.</param>
//        protected virtual void SetCapacity(int length)
//        {

//            if (length < 0)
//                throw new ArgumentOutOfRangeException("The new length is less than the current length.");

//            if (length == 0)
//                return;

//            if (mArray == null)
//            {
//                mArray = new T[length];
//                mLength = length;
//                mCapacity = length;
//                return;
//            }

//            int increase = length - mLength;

//            if (increase>0)
//            {
//                if (mNext != null)
//                    mNext.Expand(increase);
//                else
//                    mNext = new LockFreeArray<T>(mState, true, increase);

//                mLength += increase;
//            }
//        }
//        #endregion // SetCapacity(int length)

//        #region Locks
//        protected virtual void WriteLockAcquire()
//        {
//            if (mCapacityLock != null)
//                mCapacityLock.EnterWriteLock();
//        }

//        protected virtual void WriteLockRemove()
//        {
//            if (mCapacityLock != null)
//                mCapacityLock.ExitWriteLock();
//        }

//        protected virtual void ReadLockAcquire()
//        {
//            if (mCapacityLock != null)
//                mCapacityLock.EnterReadLock();
//        }

//        protected virtual void ReadLockRemove()
//        {
//            if (mCapacityLock != null)
//                mCapacityLock.ExitReadLock();
//        }
//        #endregion // Locks

//        #region this[int key]
//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="key"></param>
//        /// <returns></returns>
//        public T this[int key]
//        {
//            get
//            {
//                try
//                {
//                    ReadLockAcquire();

//                    if (key < mCapacity)
//                        return mArray[key];

//                    if (mNext == null)
//                        throw new ArgumentOutOfRangeException("The index is out of range.");

//                    return mNext[key - mCapacity];
//                }
//                finally
//                {
//                    ReadLockRemove();
//                }
//            }
//            set
//            {
//                try
//                {
//                    ReadLockAcquire();

//                    if (key < mCapacity)
//                    {
//                        mArray[key] = value;
//                        mState.VersionChange();
//                        return;
//                    }

//                    if (mNext == null)
//                        throw new ArgumentOutOfRangeException("The index is out of range.");

//                    mNext[key - mCapacity] = value;
//                }
//                finally
//                {
//                    ReadLockRemove();
//                }
//            }
//        }
//        #endregion // this[int key]

//        #region Version
//        /// <summary>
//        /// This is the current version ID of the collection.
//        /// </summary>
//        public long Version { get { return mState.Version; } }
//        #endregion // Version

//        public void Clear()
//        {
//            try
//            {
//                WriteLockAcquire();

//                throw new NotImplementedException();
//            }
//            finally
//            {
//                WriteLockRemove();
//            }
//        }

//        public void CopyTo(T[] array, int arrayIndex)
//        {
//            try
//            {
//                WriteLockAcquire();
//                throw new NotImplementedException();
//            }
//            finally
//            {
//                WriteLockRemove();
//            }
//        }

//        #region Length
//        /// <summary>
//        /// This is the current length of the array.
//        /// </summary>
//        public int Length
//        {
//            get 
//            {
//                try
//                {
//                    ReadLockAcquire();
//                    return Interlocked.CompareExchange(ref mLength, mLength, mLength);
//                }
//                finally
//                {
//                    ReadLockRemove();
//                }
//            }
//        }
//        #endregion // Length

//        #region Struct -> LockFreeArrayEnumerator<T>
//        /// <summary>
//        /// This struct provide the enumeration functionality for the array.
//        /// </summary>
//        /// <typeparam name="T">The array type.</typeparam>
//        protected internal struct LockFreeArrayEnumerator<T> : IEnumerator<T>
//        {
//            #region Declarations
//            long mVersion;
//            LockFreeArray<T> mArray;
//            int mPosition;
//            #endregion // Declarations
//            #region Constructor
//            /// <summary>
//            /// This constructor creates the enumerator for the array.
//            /// </summary>
//            /// <param name="array">The array.</param>
//            public LockFreeArrayEnumerator(LockFreeArray<T> array)
//            {
//                mVersion = array.Version;
//                mArray = array;
//                mPosition = -1;
//            }
//            #endregion // Constructor.

//            private void VersionCheck()
//            {
//                if (mVersion != mArray.Version)
//                {
//                    throw new InvalidOperationException("The array has been modified.");
//                }
//            }

//            #region Current
//            /// <summary>
//            /// Returns the current item.
//            /// </summary>
//            public T Current
//            {
//                get
//                {
//                    return mArray[mPosition];
//                }
//            }
//            #endregion // Current
//            #region MoveNext()
//            /// <summary>
//            /// Move to the next position.
//            /// </summary>
//            /// <returns>Returns true if method has moved to the next position.</returns>
//            public bool MoveNext()
//            {
//                VersionCheck();
//                mPosition++;
//                return mPosition < mArray.Length;
//            }
//            #endregion // MoveNext()
//            #region Reset()
//            /// <summary>
//            /// Reset the collection to the beginning.
//            /// </summary>
//            public void Reset()
//            {
//                VersionCheck();
//                mPosition = -1;
//            }
//            #endregion // Reset()

//            #region IDisposable Members
//            /// <summary>
//            /// This method removes the reference to the array.
//            /// </summary>
//            public void Dispose()
//            {
//                mArray = null;
//            }
//            #endregion

//            #region IEnumerator Members

//            object IEnumerator.Current
//            {
//                get { return (object)Current; }
//            }


//            #endregion
//        }
//        #endregion // Struct -> LockFreeArrayEnumerator<T>

//        #region GetEnumerator()
//        /// <summary>
//        /// This method returns an enumerator for the collection.
//        /// </summary>
//        /// <returns>An enumeration class.</returns>
//        public IEnumerator<T> GetEnumerator()
//        {
//            return new LockFreeArrayEnumerator<T>(this);
//        }
//        #endregion // GetEnumerator()

//        #region IEnumerator
//        IEnumerator IEnumerable.GetEnumerator()
//        {
//            return (IEnumerator)GetEnumerator();
//        }
//        #endregion // IEnumerator
//    }
//}
