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

using Ximura;
using Ximura.Helper;
#endregion // using
namespace Ximura
{
    /// <summary>
    /// The pool manager interface is used to provide an interface that allows multiple
    /// pool objects to be created around a base object type.
    /// </summary>
    public interface IXimuraPoolManager: IDisposable
    {
        /// <summary>
        /// This method retrieve a pool manage object based on
        /// </summary>
        /// <param name="objectType">The object type for the pool.</param>
        /// <returns>Returns a pool manager for the type specified.</returns>
        IXimuraPool GetPoolManager(Type objectType);

        /// <summary>
        /// This method retrieve a pool manage object based on
        /// </summary>
        /// <param name="objectType">The object type for the pool.</param>
        /// <param name="buffered">A boolean value that specifies whether the pool manager should
        /// be buffered. A buffered pool cannot be reset by the receiving party.</param>
        /// <returns>Returns a pool manager for the type specified.</returns>
        IXimuraPool GetPoolManager(Type objectType, bool buffered);

        IXimuraPool<T> GetPoolManager<T>() where T : IXimuraPoolableObject;

        IXimuraPool<T> GetPoolManager<T>(bool buffered) where T : IXimuraPoolableObject;


    }
}
