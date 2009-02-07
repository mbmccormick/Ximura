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
    /// This interface specifically identifies the linkage behaviour of the data content.
    /// </summary>
    public interface IXimuraDataContentLinkage
    {
        /// <summary>
        /// This property defines the DataContent linkage type.
        /// </summary>
        DataContentLinkType LinkType { get;set;}
        /// <summary>
        /// This property identifies the specific object to link to, when the LinkType property
        /// is set to Link.
        /// </summary>
        string LinkTypeIdentifier { get;set;}
    }
}
