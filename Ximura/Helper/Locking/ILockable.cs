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

#endregion // using
namespace Ximura
{
    /// <summary>
    /// This interface is implemented by items that are individually lockable.
    /// </summary>
    public interface ILockable
    {
        /// <summary>
        /// Returns true if the item is locked.
        /// </summary>
        /// <returns>Returns true if the item is locked.</returns>
        bool IsLocked { get; }
        /// <summary>
        /// This method locks the particular item.
        /// </summary>
        void Lock();
        /// <summary>
        /// This method halts any threads if the item is locked.
        /// </summary>
        void LockWait();
        /// <summary>
        /// This method attempts to lock the item.
        /// </summary>
        /// <returns>Returns true if the item is successfully locked.</returns>
        bool TryLock();
        /// <summary>
        /// This method unlocks the item.
        /// </summary>
        void Unlock();
    }
}
