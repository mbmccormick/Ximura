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

#endregion // using
namespace Ximura
{
    /// <summary>
    /// This interface is implemented by objects that are poolable, and in addition 
    /// can return themselves to the pool without the need for child objects to have a reference
    /// to the pool.
    /// </summary>
    public interface IXimuraPoolReturnable : IXimuraPoolableObject
    {
        /// <summary>
        /// This property sets or gets the object pool for the object.
        /// </summary>
        IXimuraPool ObjectPool { get;set;}

        /// <summary>
        /// This boolean property determines whether the object can be returned to the pool
        /// for reuse.
        /// </summary>
        bool ObjectPoolCanReturn { get;}

        /// <summary>
        /// This method returns the object to the pool
        /// </summary>
        /// <returns>Returns true if the object has been successfully returned to the pool.</returns>
        void ObjectPoolReturn();
    }
}
