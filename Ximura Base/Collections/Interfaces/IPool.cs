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
using Ximura.Helper;
#endregion // using
namespace Ximura.Collections
{
    /// <summary>
    /// This interface is implemented by collection classes the provide object pooling.
    /// </summary>
    /// <typeparam name="T">The pool object type.</typeparam>
    public interface IPool<T> : ICollectionBase<T>
    {
        /// <summary>
        /// This method returns true if there are objects available in the pool.
        /// </summary>
        bool Available { get; }
        /// <summary>
        /// This is the maximum pool size, a value of -1 specifies no maximum size.
        /// </summary>
        int Max { get; }
        /// <summary>
        /// This is the minimum pool size.
        /// </summary>
        int Min { get; }
        /// <summary>
        /// This is prefered pool size.
        /// </summary>
        int Prefered { get; }
        /// <summary>
        /// This property returns the number of active objects.
        /// </summary>
        int Count { get; }
        /// <summary>
        /// This property specifies whether the pool can grow when more items are requested than are currently available.
        /// </summary>
        bool IsFixedSize { get; }

        /// <summary>
        /// This method returns an object to the pool.
        /// </summary>
        /// <param name="value">The object to return to the pool.</param>
        void Return(T value);
        /// <summary>
        /// This method attemtps to return an object to the pool.
        /// </summary>
        /// <param name="value">The item to return.</param>
        /// <returns>Returns true if the item was successfully returned.</returns>
        bool TryReturn(T value);

        /// <summary>
        /// The Get() method takes an object from the pool, if 
        /// there are no objects available the pool will create a new object.
        /// </summary>
        /// <returns>An object of the type defined in the pool definition.</returns>
        T Get();
        /// <summary>
        /// This method attempts to take an item from the pool.
        /// </summary>
        /// <param name="value">The item from the pool.</param>
        /// <returns>Returns true if an item has been returned from the pool.</returns>
        bool TryGet(out T value);
    }
}
