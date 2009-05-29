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
    /// This interface is implemented by a stack.
    /// </summary>
    /// <typeparam name="T">The collection item type.</typeparam>
    public interface IStack<T> : ICollectionBase<T>
    {
        /// <summary>
        /// This method pushes an item on to the stack.
        /// </summary>
        /// <param name="item">The item to add to the stack.</param>
        void Push(T item);
        /// <summary>
        /// This method removes the top item from the stack.
        /// </summary>
        /// <returns>Returns the top item on the stack.</returns>
        T Pop();
    }
}
