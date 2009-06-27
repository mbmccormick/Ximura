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
    public struct LockableWrapper<T> : ILockable
    {
        #region Declarations
        /// <summary>
        /// The private value that indicates whether the class is locked.
        /// </summary>
        private int mLocked;
        /// <summary>
        /// The boolean value which specifies whether the structure contains a valid value.
        /// </summary>
        private bool mHasValue;
        /// <summary>
        /// The value.
        /// </summary>
        private T mValue;
#if (DEBUG)
        /// <summary>
        /// The managed ID of the locking thread.
        /// </summary>
        private int? mLockingThread;      
#endif
        #endregion // Declarations
        #region Constructor
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        /// <param name="value"></param>
        public LockableWrapper(T value)
        {
            mLocked = 0;
            mValue = value;

            mHasValue = !EqualityComparer<T>.Default.Equals(value, default(T));
#if (DEBUG)
		    mLockingThread = null;
#endif
        }
        #endregion // Constructor

        #region ILockable Members
        /// <summary>
        /// Returns true if the item is locked.
        /// </summary>
        /// <returns>Returns true if the item is locked.</returns>
        public bool IsLocked
        {
            get
            {
                return mLocked == 1;
            }
        }
        /// <summary>
        /// This method locks the particular item.
        /// </summary>
        public void Lock()
        {
            while (!TryLock())
                Threading.ThreadWait();
        }
        /// <summary>
        /// This method halts any threads if the item is locked.
        /// </summary>
        public void LockWait()
        {
            while (IsLocked)
                Threading.ThreadWait();
        }
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
        #endregion

        #region Value
        /// <summary>
        /// This is the value locked by the collection.
        /// </summary>
        public T Value
        {
            get { return mValue; }
            set 
            { 
                mValue = value;
                mHasValue = !EqualityComparer<T>.Default.Equals(value, default(T));
            }
        }
        #endregion // Value
        #region HasValue
        /// <summary>
        /// Specifies whether the wrapper contains a value.
        /// </summary>
        public bool HasValue
        {
            get
            {
                return mHasValue;
            }
        }
        #endregion // HasValue

        #region ToString()
        /// <summary>
        /// This method provides a string value of the enclosed data.
        /// </summary>
        /// <returns>Returns the internal data as a string.</returns>
        public override string ToString()
        {
            if (!this.HasValue)
            {
                return "";
            }
            return mValue.ToString();
        }
        #endregion // ToString()
        #region GetHashCode()
        /// <summary>
        /// This method returns the hashcode of the enclosed data.
        /// </summary>
        /// <returns>The internal hashcode.</returns>
        public override int GetHashCode()
        {
            if (!this.HasValue)
            {
                return 0;
            }
            return mValue.GetHashCode();
        }
        #endregion // GetHashCode()
        #region Equals(object other)
        /// <summary>
        /// This method compares the other value to the enclosed data.
        /// </summary>
        /// <param name="other">The data to compare.</param>
        /// <returns>Returns true if the data is the same.</returns>
        public override bool Equals(object other)
        {
            if (!this.HasValue)
            {
                return (other == null);
            }

            if (other == null)
            {
                return false;
            }

            return mValue.Equals(other);
        }
        #endregion
    }
}
