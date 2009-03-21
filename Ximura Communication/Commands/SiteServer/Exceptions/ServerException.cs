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
using System.Threading;
using System.Timers;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Reflection;
using System.Drawing;
using System.Diagnostics;
using System.Collections.Generic;
using System.Data;
using System.Security;
using System.Security.Cryptography;
using System.Globalization;
using System.Runtime.Serialization;

using Ximura;
using Ximura.Data;
using Ximura.Helper;
using CH = Ximura.Helper.Common;
using Ximura.Server;

using Ximura.Command;

#endregion // using
namespace Ximura.Communication
{
    /// <summary>
    /// The Protocol Exception is thrown by a protocol should an error occur.
    /// </summary>
    public class ServerException : XimuraException
    {
        /// <summary>
        /// Initializes a new instance of the ProtocolException class.
        /// </summary>
        public ServerException() : base() { }
        /// <summary>
        /// Initializes a new instance of the ProtocolException class.
        /// </summary>
        /// <param name="message">The error message.</param>
        public ServerException(string message) : base(message) { }
        /// <summary>
        /// Initializes a new instance of the ProtocolException class.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="ex">The base exception</param>
        public ServerException(string message, Exception ex) : base(message, ex) { }
    }
}
