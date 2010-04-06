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
using System.Runtime.Serialization;

using Ximura;

#endregion // using
namespace Ximura
{
    /// <summary>
    /// This interface is used to retrieve an object that support the deserializable interface.
    /// </summary>
    public interface IXimuraPoolDeserializable : IXimuraPool
    {
        /// <summary>
        /// This boolean property returns true if the object type for the pool supports deserialization.
        /// </summary>
        bool SupportsDeserialization { get;}

        /// <summary>
        /// This method returns an object from the pool and deserializes this information in the serialization
        /// objects.
        /// </summary>
        /// <param name="info">The serialization information.</param>
        /// <param name="context">The serialization context.</param>
        /// <returns>Returns the object specified from the pool.</returns>
        object Get(SerializationInfo info, StreamingContext context);
    }
    /// <summary>
    /// This is the generic interface for poolable deserializable objects.
    /// </summary>
    /// <typeparam name="T">The poolable object type.</typeparam>
    public interface IXimuraPoolDeserializable<T> : IXimuraPool<T>, IXimuraPoolDeserializable
        where T : IXimuraPoolableObject
    {
        /// <summary>
        /// This method returns an object of type T from the pool and deserializes this 
        /// information in the serialization objects.
        /// </summary>
        /// <param name="info">The serialization information.</param>
        /// <param name="context">The serialization context.</param>
        /// <returns>Returns the object specified from the pool.</returns>
        T Get(SerializationInfo info, StreamingContext context);
    }
}
