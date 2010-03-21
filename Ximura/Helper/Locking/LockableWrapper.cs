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
namespace Ximura.Helper
{
    /// <summary>
    /// The lockable base class is for objects that require fine-grained locking.
    /// </summary>
    [Serializable, StructLayout(LayoutKind.Sequential)]
    public struct LockableWrapper<T>
    {
        #region Declarations
        /// <summary>
        /// The private value that indicates whether the class is locked.
        /// </summary>
        private int mFlags;
#if (DEBUG)
        int mDebugThreadID;
#endif
        #endregion // Declarations
        #region Constructor
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        /// <param name="value"></param>
        public LockableWrapper(T value)
        {
            mFlags = 0;
            Value = value;
#if (DEBUG)
            mDebugThreadID=0;
#endif
        }
        #endregion // Constructor

        #region IsLocked
        /// <summary>
        /// Returns true if the item is locked.
        /// </summary>
        /// <returns>Returns true if the item is locked.</returns>
        public bool IsLocked
        {
            get
            {
                return mFlags == 1;
            }
        }
        #endregion // IsLocked
        #region Lock()
        /// <summary>
        /// This method locks the particular item.
        /// </summary>
        public void Lock()
        {
            while (!TryLock())
                ThreadingHelper.ThreadWait();
        }
        #endregion // Lock()
        #region LockWait()
        /// <summary>
        /// This method halts any threads if the item is locked.
        /// </summary>
        public void LockWait()
        {
            while (IsLocked)
                ThreadingHelper.ThreadWait();
        }
        #endregion // LockWait()
        #region TryLock()
        /// <summary>
        /// This method attempts to lock the item.
        /// </summary>
        /// <returns>Returns true if the item is successfully locked.</returns>
        public bool TryLock()
        {
            bool success = (Interlocked.CompareExchange(ref mFlags, 1, 0) == 0);
#if (DEBUG)
            if (success)
                mDebugThreadID = Thread.CurrentThread.ManagedThreadId;
#endif
            return success;
        }
        #endregion // TryLock()
        #region Unlock()
        /// <summary>
        /// This method unlocks the item.
        /// </summary>
        public void Unlock()
        {
            mFlags = 0;
#if (DEBUG)
            mDebugThreadID = 0;
#endif
        }
        #endregion // Unlock()

        #region Value
        /// <summary>
        /// This is the value locked by the collection.
        /// </summary>
        public T Value;
        #endregion // Value
    }
}
