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
    /// This is the linkage option for the DataContent
    /// </summary>
    public enum DataContentLinkType
    {
        /// <summary>
        /// This object is not linked to any other object.
        /// </summary>
        NoLinkage,
        /// <summary>
        /// The data content is copied from another object in the model folder. Changes made to the
        /// data in this object will not be passed on to the linked object.
        /// </summary>
        MergeIfPresent,
        /// <summary>
        /// The entity is directly linked to another object in the model folder. Changes made to this
        /// object will be directly syncronised with the parent object.
        /// </summary>
        Link
    }
}
