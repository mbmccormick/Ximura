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
using System.Runtime.Serialization;
using System.Runtime.InteropServices;
using System.Threading;

using Ximura;

#endregion // using
namespace Ximura.Collections
{
    /// <summary>
    /// This interface is implemented by lockable arrays.
    /// </summary>
    /// <typeparam name="T">The array item type.</typeparam>
    public interface ILockableMarkableArray<T>
    {
        /// <summary>
        /// This method checks whether an item in the collection is locked.
        /// </summary>
        /// <param name="index">The index of the item to check.</param>
        /// <returns>Returns true if the item is locked.</returns>
        bool ItemIsLocked(int index);
        /// <summary>
        /// This method locks the specific item.
        /// </summary>
        /// <param name="index">The item index.</param>
        void ItemLock(int index);
        /// <summary>
        /// This method waits for a locked item to become available.
        /// </summary>
        /// <param name="index">The index of the item to wait for.</param>
        void ItemLockWait(int index);
        /// <summary>
        /// This method attempts to lock the item specified.
        /// </summary>
        /// <param name="index">The index of the item you wish to lock.</param>
        /// <returns>Returns true if the item was successfully locked.</returns>
        bool ItemTryLock(int index);
        /// <summary>
        /// The method unlocks the item.
        /// </summary>
        /// <param name="index">The index of the item you wish to unlock.</param>
        void ItemUnlock(int index);
        /// <summary>
        /// This is the capacity of the array.
        /// </summary>
        int Count { get; }
        /// <summary>
        /// This is the indexer for the array.
        /// </summary>
        /// <param name="index">The index position.</param>
        /// <returns>Returns the object corresponding to the index position.</returns>
        T this[int index] { get; set; }

        LockableWrapper<T> LockableData(int index);
    }
}
