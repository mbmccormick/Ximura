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
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Reflection;
using System.Drawing;
using System.Diagnostics;

using System.Net;
using System.Net.Sockets;
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;

using Ximura;
using Ximura.Data;
using Ximura.Helper;
using Ximura.Framework;
using Ximura.Framework;

#endregion // using
namespace Ximura.Communication
{
    /// <summary>
    /// This enumeration defines the type of transport, namely connection orientated or
    /// connectionless.
    /// </summary>
    public enum TransportConnectionType
    {
        /// <summary>
        /// The transport type is not defined.
        /// </summary>
        Undefined,
        /// <summary>
        /// The transport type is connectionless.
        /// </summary>
        Connectionless,
        /// <summary>
        /// The transport type is connection full.
        /// </summary>
        Connectionful
    }
}
