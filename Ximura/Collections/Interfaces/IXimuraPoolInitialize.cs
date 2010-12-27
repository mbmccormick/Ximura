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

using Ximura;

#endregion // using
namespace Ximura
{

    public interface IXimuraPoolInitialize<T> : IXimuraPool<T>
        where T : IXimuraPoolableObject
    {
        /// <summary>
        /// The Get() method takes an object from the pool, if 
        /// there are no objects available the pool will create a new object.
        /// </summary>
        /// <returns>An object of the type defined in the pool definition.</returns>
        T Get(Action<T> initializer);
#if (!SILVERLIGHT)
        /// <summary>
        /// The Get() method takes an object from the pool, if 
        /// there are no objects available the pool will create a new object.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The serialization context.</param>
        /// <returns>An object of the type defined in the pool definition, with the serialization data.</returns>
        T Get(SerializationInfo info, StreamingContext context, Action<T> initializer);
#endif
    }

}
