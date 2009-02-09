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

using Ximura;
using Ximura.Data;
using Ximura.Helper;
#endregion // using
namespace Ximura.Data
{
    /// <summary>
    /// IXimuraElement is the base interface for the Element object.
    /// </summary>
    public interface IXimuraElement : IXimuraElementLeaf
    { }//, IContainer{}

    /// <summary>
    /// This interface is used as the default intreface of the Data Element Composite pattern.
    /// </summary>
    public interface IXimuraElementLeaf : IXimuraPoolableObject
    {
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
