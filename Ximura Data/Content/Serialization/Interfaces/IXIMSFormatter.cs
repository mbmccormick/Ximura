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
﻿#region using
using System;
using System.IO;
using System.Runtime.Serialization;

using Ximura;
using Ximura.Data;
using Ximura.Helper;
using CH = Ximura.Helper.Common;
#endregion
namespace Ximura.Data
{
    /// <summary>
    /// This interface extends the standard formatter interface and adds the support for object pools.
    /// </summary>
    public interface IXimuraFormatter: IFormatter
    {
        /// <summary>
        /// This method deserializes the object in the stream and returns the appropriate content object from the pool
        /// provided. If the pool manager is null, then a new object is created.
        /// </summary>
        /// <param name="serializationStream">The stream to deserialize from.</param>
        /// <param name="pMan">The pool manager to retrieve the new object. This can be null.</param>
        /// <returns>Returns an object specified in the stream.</returns>
        object Deserialize(Stream serializationStream, IXimuraPoolManager pMan);


    }
}
