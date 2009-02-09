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
    // Summary:
    //     Provides custom implementation for user-defined type (UDT) and user-defined
    //     aggregate serialization and deserialization.
    public interface IXimuraBinarySerialize
    {
        // Summary:
        //     Generates a user-defined type (UDT) or user-defined aggregate from its binary
        //     form.
        //
        // Parameters:
        //   r:
        //     The System.IO.BinaryReader stream from which the object is deserialized.
        void Read(BinaryReader r);
        //
        // Summary:
        //     Converts a user-defined type (UDT) or user-defined aggregate into its binary
        //     format.
        //
        // Parameters:
        //   w:
        //     The System.IO.BinaryWriter stream to which the UDT or user-defined aggregate
        //     is serialized.
        void Write(BinaryWriter w);
    }
}

