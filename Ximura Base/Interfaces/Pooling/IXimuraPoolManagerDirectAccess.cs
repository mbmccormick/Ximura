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
    /// This method is for poolable objects that require access to the base pool manager, so that they can retrieve
    /// additional internal objects from the pool.
    /// </summary>
    public interface IXimuraPoolManagerDirectAccess
    {
        /// <summary>
        /// This is the pool manager for the object.
        /// </summary>
        IXimuraPoolManager PoolManager { get;set;}
        /// <summary>
        /// This method returns the specific pool manager for the type.
        /// </summary>
        /// <param name="objectType"></param>
        /// <returns>Returns the pool for the object type.</returns>
        IXimuraPool PoolGet(Type objectType);
        /// <summary>
        /// Returns a new object of the type specified.
        /// </summary>
        /// <param name="objectType">The object type required.</param>
        /// <returns>Returns a poolable object of the type defined.</returns>
        object PoolGetObject(Type objectType);
        /// <summary>
        /// This method returns the object to the appropriate pool.
        /// </summary>
        /// <param name="poolObject">The object to return.</param>
        void PoolGetReturn(object poolObject);
    }
}
