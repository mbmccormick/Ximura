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
using System.Runtime.Serialization;
using System.ComponentModel;
using System.Collections.Generic;
using System.Text;

using Ximura;
using Ximura.Data;
#endregion
namespace Ximura.Communication
{
    /// <summary>
    /// This interface is used to provide a consistent interface for initializing an entity from a byte stream or array.
    /// </summary>
    public interface IXimuraMessageLoadData
    {
        int Load(string data);

        int Load(string data, Encoding encoding);

        int Load(byte[] buffer, int offset, int count);

        int Load(Stream data);
   }

    public interface IXimuraMessageLoadData<TERM>
        where TERM : IXimuraMessageTermination
    {
        int Load(TERM terminator, byte[] buffer, int offset, int count);

        int Load(TERM terminator, Stream data);

        int Load(TERM terminator, string data);

        int Load(TERM terminator, string data, Encoding encoding);
    }
    
}
