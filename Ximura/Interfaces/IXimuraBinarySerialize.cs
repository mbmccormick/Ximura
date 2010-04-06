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
using System.IO;
using System.Collections.Generic;
using System.Text;
#endregion // using
namespace Ximura.Data
{
    /// <summary>
    /// Provides custom implementation for user-defined type (UDT) and user-defined aggregate serialization and deserialization.
    /// </summary>
    public interface IXimuraBinarySerialize
    {
        /// <summary>
        /// Generates a user-defined type (UDT) or user-defined aggregate from its binary form.
        /// </summary>
        /// <param name="r">The System.IO.BinaryReader stream from which the object is deserialized.</param>
        void Read(BinaryReader r);
        /// <summary>
        /// Converts a user-defined type (UDT) or user-defined aggregate into its binary format.
        /// </summary>
        /// <param name="w">The System.IO.BinaryWriter stream to which the UDT or user-defined aggregate is serialized.</param>
        void Write(BinaryWriter w);
    }
}

