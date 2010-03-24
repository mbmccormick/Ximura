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
using System.Runtime.Serialization;
using System.Data;

using Ximura;

#endregion // using
namespace Ximura.Data
{
    /// <summary>
    /// IXimuraContent is the default interface for the content object.
    /// </summary>
    public interface IXimuraContent : IXimuraContentEntityPersistence, 
        ISerializable, IDeserializationCallback, IXimuraPoolableObjectDeserializable, 
        IXimuraPoolReturnable
    {
        /// <summary>
        /// This is the Content Type ID.
        /// </summary>
        Guid IDType { get;}
        /// <summary>
        /// This is the Content ID.
        /// </summary>
        Guid IDContent { get;set;}
        /// <summary>
        /// This is the Content Version ID.
        /// </summary>
        Guid IDVersion { get;set;}

        /// <summary>
        /// This boolean property indicates whether the object has been changed.
        /// </summary>
        bool Dirty { get;set;}
        /// <summary>
        /// This is a public function that indicated whether the object
        /// internal data has been changed since it was created or last saved.
        /// </summary>
        /// <returns>A boolean value - true indicates the object has been changed.</returns>
        bool IsDirty();
    }
}