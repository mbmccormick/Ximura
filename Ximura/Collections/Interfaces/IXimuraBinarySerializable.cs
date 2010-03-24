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
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.Serialization;
using System.IO;

using Ximura;

#endregion // using
namespace Ximura.Collections
{
    /// <summary>
    /// This interface is used to persist or restore an abject from a binary stream.
    /// </summary>
    public interface IXimuraBinarySerializable
    {
        /// <summary>
        /// This method restores the object from the stream.
        /// </summary>
        /// <param name="r">The reader to read from.</param>
        void Read(BinaryReader r);
        /// <summary>
        /// Persists an object to the binary stream.
        /// </summary>
        /// <param name="w">The writer to persist to.</param>
        void Write(BinaryWriter w);
    }
}
