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
    /// This interface implements additional initialization method for the objects in the pool.
    /// </summary>
    /// <typeparam name="T">The pool object type.</typeparam>
    public interface IPoolInitialization<T> : IPool<T>
    {
#if (!SILVERLIGHT)
        /// <summary>
        /// The Get() method takes an object from the pool, if 
        /// there are no objects available the pool will create a new object.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The serialization context.</param>
        /// <returns>An object of the type defined in the pool definition, with the serialization data.</returns>
        T Get(SerializationInfo info, StreamingContext context);
        /// <summary>
        /// The Get() method takes an object from the pool, if 
        /// there are no objects available the pool will create a new object.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The serialization context.</param>
        /// <param name="value">An object of the type defined in the pool definition, with the serialization data.</param>
        /// <returns>Returns true if an item has been returned from the pool.</returns>
        bool TryGet(SerializationInfo info, StreamingContext context, out T value);
#endif
        /// <summary>
        /// The Get() method takes an object from the pool, if 
        /// there are no objects available the pool will create a new object.
        /// </summary>
        /// <param name="initializer">This action will be performed on the object after the data has been deserialized.</param>
        /// <returns>An object of the type defined in the pool definition.</returns>
        T Get(Action<T> initializer);
        /// <summary>
        /// The Get() method takes an object from the pool, if 
        /// there are no objects available the pool will create a new object.
        /// </summary>
        /// <param name="initializer">This action will be performed on the object after the data has been deserialized.</param>
        /// <param name="value">An object of the type defined in the pool definition, with the serialization data.</param>
        /// <returns>Returns true if an item has been returned from the pool.</returns>
        bool TryGet(Action<T> initializer, out T value);

#if (!SILVERLIGHT)
        /// <summary>
        /// The Get() method takes an object from the pool, if 
        /// there are no objects available the pool will create a new object.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The serialization context.</param>
        /// <param name="initializer">This action will be performed on the object after the data has been deserialized.</param>
        /// <returns>An object of the type defined in the pool definition, with the serialization data.</returns>
        T Get(SerializationInfo info, StreamingContext context, Action<T> initializer);
        /// <summary>
        /// The Get() method takes an object from the pool, if 
        /// there are no objects available the pool will create a new object.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The serialization context.</param>
        /// <param name="initializer">This action will be performed on the object after the data has been deserialized.</param>
        /// <param name="value">An object of the type defined in the pool definition, with the serialization data.</param>
        /// <returns>Returns true if an item has been returned from the pool.</returns>
        bool TryGet(SerializationInfo info, StreamingContext context, Action<T> initializer, out T value);
#endif
    }
}
