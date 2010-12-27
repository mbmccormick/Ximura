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
using System.Collections;
using System.Text;

using Ximura;

using Ximura.Data;
#endregion
namespace Ximura
{
    /// <summary>
    /// This interface is used by objects that support the messaging format.
    /// </summary>
    public interface IXimuraMessage : IXimuraPoolReturnable, ISupportInitialize, IXimuraMessageStreamLoad
    {
        bool SupportsInitialization { get; }

        bool CanRead { get; }
        bool CanWrite { get; }

        long Position { get;set; }
        long Length { get; }
        long? BodyLength { get; }

        int Read(byte[] buffer, int offset, int count);
        int ReadByte();

        int Write(byte[] buffer, int offset, int count);
        int WriteByte(byte value);

        /// <summary>
        /// This property indicates the message direction.
        /// </summary>
        MessageDirection Direction { get;}

        /// <summary>
        /// This property indicates whether this section signals the end of the message.
        /// </summary>
        bool IsTerminator { get; }

        string DebugString { get; }

        byte[] ToArray();
        byte[] ToArray(bool copy);
    }
}
