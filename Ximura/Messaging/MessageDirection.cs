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
    /// This enumeration defines the messaging charteristics.
    /// </summary>
    [Flags]
    public enum MessageDirection
    {
        /// <summary>
        /// This is the default value that the message is set to 
        /// </summary>
        Undefined = 0,
        /// <summary>
        /// Message can be read from.
        /// </summary>
        Read = 1,
        /// <summary>
        /// Message can be written to.
        /// </summary>
        Write = 2,
        /// <summary>
        /// Message supports both reading and writing.
        /// </summary>
        Bidirectional = 3
    }
}
