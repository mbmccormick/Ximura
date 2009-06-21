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
    /// This interface is shared by both the IQueue and IStack interfaces and contains shared functionality.
    /// </summary>
    /// <typeparam name="T">The collection item type.</typeparam>
    public interface ICollectionBase<T> : IEnumerable<T>, ICollection
    {
        /// <summary>
        /// The collection item count.
        /// </summary>
        int Count { get; }

        void Clear();
        bool Contains(T item);
        void CopyTo(T[] array, int arrayIndex);

        T[] ToArray();
        void TrimExcess();

        T Peek();

        /// <summary>
        /// This method tries to peek the next item to leave the collection.
        /// </summary>
        /// <param name="item">The item at the top or default.</param>
        /// <returns>Returns true if an item is available.</returns>
        bool TryPeek(out T item);
    }
}
