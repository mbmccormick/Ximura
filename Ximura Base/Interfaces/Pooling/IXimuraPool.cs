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
using Ximura.Helper;
#endregion // using
namespace Ximura
{
    /// <summary>
    /// This interface is implemented by the object pool
    /// </summary>
    public interface IXimuraPool: IDisposable
    {
        /// <summary>
        /// This method returns an object of the specified type.
        /// </summary>
        /// <returns>An object of the pool type.</returns>
        object Get();
        /// <summary>
        /// This method returns an object of the specified type, with the deserialized data.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The serialization context.</param>
        /// <returns>An object of the pool type.</returns>
        object Get(SerializationInfo info, StreamingContext context);

        /// <summary>
        /// This method returns an object to the pool.
        /// </summary>
        /// <param name="value">The object to return to the pool.</param>
        void Return(object value);

        /// <summary>
        /// This method returns true if there are objects available in the pool.
        /// </summary>
        bool Available { get;}
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
        int Count { get;}
        /// <summary>
        /// This method is used to clear the pool of all objects.
        /// </summary>
        void Clear();
        /// <summary>
        /// This property indicates whether the pool is buffered. Buffered pools are shared amongst multiple 
        /// clients and do not implement the clear method.
        /// </summary>
        bool IsBuffered { get;}
        /// <summary>
        /// This property is used to set the pool manager for the specific pool.
        /// </summary>
        IXimuraPoolManager PoolManager { get;set;}
    }

    /// <summary>
    /// This interface is implemented by the generic object pool
    /// </summary>
    public interface IXimuraPool<T> : IXimuraPool
        where T : IXimuraPoolableObject
    {
        /// <summary>
        /// The Get() method takes an object from the pool, if 
        /// there are no objects available the pool will create a new object.
        /// </summary>
        /// <returns>An object of the type defined in the pool definition.</returns>
        T Get();
        /// <summary>
        /// The Get() method takes an object from the pool, if 
        /// there are no objects available the pool will create a new object.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The serialization context.</param>
        /// <returns>An object of the type defined in the pool definition, with the serialization data.</returns>
        T Get(SerializationInfo info, StreamingContext context);
        /// <summary>
        /// This method returns an object to the pool.
        /// </summary>
        /// <param name="value">The object to return to the pool.</param>
        void Return(T value);

        string Stats { get;}
    }

    public interface IXimuraPoolInitialize<T> : IXimuraPool<T>
        where T : IXimuraPoolableObject
    {
        /// <summary>
        /// The Get() method takes an object from the pool, if 
        /// there are no objects available the pool will create a new object.
        /// </summary>
        /// <returns>An object of the type defined in the pool definition.</returns>
        T Get(Action<T> initializer);
        /// <summary>
        /// The Get() method takes an object from the pool, if 
        /// there are no objects available the pool will create a new object.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The serialization context.</param>
        /// <returns>An object of the type defined in the pool definition, with the serialization data.</returns>
        T Get(SerializationInfo info, StreamingContext context, Action<T> initializer);
    }

    /// <summary>
    /// This is the pool buffer interface. This interface is used by the pool managers 
    /// to reset the references when closing down.
    /// </summary>
    public interface IXimuraPoolBuffer
    {
        /// <summary>
        /// This method resets the buffer and it's connection to the underlying object pool.
        /// </summary>
        void ResetBuffer();

    }
}
