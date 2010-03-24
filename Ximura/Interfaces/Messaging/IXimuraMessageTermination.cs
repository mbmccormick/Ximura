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
    public interface IXimuraMessageTermination : IXimuraPoolReturnable
    {
        /// <summary>
        /// This property specifies whether the fragment byte array has reached the termination requirements
        /// </summary>
        bool IsTerminator { get; }

        int CarryOver { get; }

        bool Match(byte[] buffer, int offset, int count, out int length, out long? bodyLength);

    }
}
