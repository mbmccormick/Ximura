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
using Ximura.Helper;
#endregion // using
namespace Ximura
{
    /// <summary>
    /// This interface is implemented by poolable objects.
    /// </summary>
    public interface IXimuraPoolableObject: IDisposable
    {
        /// <summary>
        /// This property returns true if the object can pool. This allows inherited objects to turn
        /// off the ability to be pooled. By default, objects that implement this interface should
        /// return true if they wish to pool.
        /// </summary>
        bool CanPool { get;}
        /// <summary>
        /// This property is used by the object pool for statistical tracking
        /// of the object through the system. This Guid should be created when the object
        /// is first created and should not change. The ID is independent of any data actually 
        /// stored in the object.
        /// </summary>
        Guid TrackID { get;}
        /// <summary>
        /// This method resets the object in to its default state.
        /// </summary>
        void Reset();
    }

    /// <summary>
    /// This interface is implemented by poolable object that also support deserialization.
    /// </summary>
    public interface IXimuraPoolableObjectDeserializable : IXimuraPoolableObject, ISerializable, IDeserializationCallback
    {
        /// <summary>
        /// This propoerty indicates whether the object support a deserialization reset.
        /// </summary>
        bool CanResetWithDeserialization { get;}
        /// <summary>
        /// This method resets the object in to the deserialized state specified in the info and context.
        /// </summary>
        /// <param name="info">The serialization information.</param>
        /// <param name="context">The serialization context.</param>
        void Reset(SerializationInfo info, StreamingContext context);
    }
}
