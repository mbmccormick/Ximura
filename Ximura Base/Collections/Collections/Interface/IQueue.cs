﻿#region Copyright
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
using System.Runtime.Serialization;
using System.Runtime.InteropServices;
using System.Threading;

using Ximura;
using Ximura.Helper;
#endregion // using
namespace Ximura.Collections
{
    /// <summary>
    /// This interface is implemented by a queue.
    /// </summary>
    /// <typeparam name="T">The collection item type.</typeparam>
    public interface IQueue<T> : ICollectionBase<T>
    {
        /// <summary>
        /// Removes an item from the head of the queue.
        /// </summary>
        /// <returns>Returns the item at the head of the queue.</returns>
        T Dequeue();
        /// <summary>
        /// This method adds an item to the tail of the queue.
        /// </summary>
        /// <param name="item">The item to add to the queue.</param>
        void Enqueue(T item);
    }
}
