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
    /// This is the base tree vertex class.
    /// </summary>
    /// <typeparam name="TKey">The key type.</typeparam>
    /// <typeparam name="TVal">The value type.</typeparam>
    public class LockFreeVertexBase<TKey, TVal>
    {
        #region Declarations
        /// <summary>
        /// The private value that indicates whether the class is locked.
        /// </summary>
        private int mLocked;
#if (DEBUG)
        /// <summary>
        /// The managed ID of the locking thread.
        /// </summary>
        private int? mLockingThread = null;
#endif
        #endregion // Declarations
        #region Constructor
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        public LockFreeVertexBase()
        {
            Key = default(TKey);
            Value = default(TVal);
        }
        #endregion // BalancedBinaryVertex

        #region Key
        /// <summary>
        /// This is the vertex key used for partitioning.
        /// </summary>
        public TKey Key { get; set; }
        #endregion // Key
        #region Value
        /// <summary>
        /// This is the value encapsulated by the vertex.
        /// </summary>
        public TVal Value { get; set; }
        #endregion // Value

        #region IsLocked()
        /// <summary>
        /// Returns true if the item is locked.
        /// </summary>
        /// <returns>Returns true if the item is locked.</returns>
        public bool IsLocked()
        {
            return mLocked == 1;
        }
        #endregion // IsLocked()
        #region Lock()
        /// <summary>
        /// This method locks the particular item.
        /// </summary>
        public void Lock()
        {
            while (!TryLock())
                Threading.ThreadWait();
        }
        #endregion // Lock()
        #region LockWait()
        /// <summary>
        /// This method halts any threads if the item is locked.
        /// </summary>
        public void LockWait()
        {
            while (IsLocked())
                Threading.ThreadWait();
        }
        #endregion // LockWait()
        #region TryLock()
        /// <summary>
        /// This method attempts to lock the item.
        /// </summary>
        /// <returns>Returns true if the item is successfully locked.</returns>
        public bool TryLock()
        {
            bool result = (Interlocked.CompareExchange(ref mLocked, 1, 0) == 0);

#if (DEBUG)
            if (result)
                mLockingThread = Thread.CurrentThread.ManagedThreadId;
#endif
            return result;
        }
        #endregion // TryLock()
        #region Unlock()
        /// <summary>
        /// This method unlocks the item.
        /// </summary>
        public void Unlock()
        {
            mLocked = 0;
#if (DEBUG)
            mLockingThread = null;
#endif
        }
        #endregion // Unlock()
    }
}
