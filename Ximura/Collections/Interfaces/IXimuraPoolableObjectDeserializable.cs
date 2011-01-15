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
#if (!SILVERLIGHT)
    /// <summary>
    /// This interface is implemented by poolable object that also support deserialization.
    /// </summary>
    public interface IXimuraPoolableObjectDeserializable : IXimuraPoolableObject, ISerializable, IDeserializationCallback
    {
        /// <summary>
        /// This propoerty indicates whether the object support a deserialization reset.
        /// </summary>
        bool CanResetWithDeserialization { get; }
        /// <summary>
        /// This method resets the object in to the deserialized state specified in the info and context.
        /// </summary>
        /// <param name="info">The serialization information.</param>
        /// <param name="context">The serialization context.</param>
        void Reset(SerializationInfo info, StreamingContext context);
    }
#endif
}