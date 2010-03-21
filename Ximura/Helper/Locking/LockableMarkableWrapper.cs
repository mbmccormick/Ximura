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
    public struct LockableMarkableWrapper<T>
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
        public LockableMarkableWrapper(T value)
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
                return (mFlags & 1) > 0;
            }
        }
        #endregion // IsLocked
        #region IsMarked
        /// <summary>
        /// Returns true if the item is marked.
        /// </summary>
        /// <returns>Returns true if the item is locked.</returns>
        public bool IsMarked
        {
            get
            {
                return (mFlags & 2) > 0;
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
        #region Mark()
        /// <summary>
        /// This method locks the particular item.
        /// </summary>
        public void Mark()
        {
            while (!TryMark())
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
            while ((mFlags & 1) == 0)
            {
                int flags = mFlags;
                bool success = Interlocked.CompareExchange(ref mFlags, flags | 1, flags) == flags;
#if (DEBUG)
                if (success)
                    mDebugThreadID = Thread.CurrentThread.ManagedThreadId;
#endif
                return success;
            }

            return false;
        }
        #endregion // TryLock()
        #region TryMark()
        /// <summary>
        /// This method attempts to lock the item.
        /// </summary>
        /// <returns>Returns true if the item is successfully locked.</returns>
        public bool TryMark()
        {
            while ((mFlags & 2) == 0)
            {
                int flags = mFlags;
                bool success = Interlocked.CompareExchange(ref mFlags, flags | 2, flags) == flags;

                return success;
            }

            return false;
        }
        #endregion // TryLock()

        #region Unlock()
        /// <summary>
        /// This method unlocks the item.
        /// </summary>
        public void Unlock()
        {
            while ((mFlags & 1) == 1)
            {
                int flags = mFlags;
                bool success = Interlocked.CompareExchange(ref mFlags, flags & (int.MaxValue - 1), flags) == flags;
                if (success) break;
            }
#if (DEBUG)
            mDebugThreadID = 0;
#endif
        }
        #endregion
        #region Unmark()
        /// <summary>
        /// This method unmarks the item.
        /// </summary>
        public void Unmark()
        {
            while ((mFlags & 2) == 1)
            {
                int flags = mFlags;
                bool success = Interlocked.CompareExchange(ref mFlags, flags & (int.MaxValue-2), flags) == flags;
                break;
            }
        }
        #endregion

        #region Clear()
        /// <summary>
        /// This method clears all the flags for the wrapper.
        /// </summary>
        public void Clear()
        {
            mFlags = 0;

#if (DEBUG)
            mDebugThreadID = 0;
#endif
        }
        #endregion // Clear()

        #region Value
        /// <summary>
        /// This is the value locked by the collection.
        /// </summary>
        public T Value;
        #endregion // Value
    }
}
