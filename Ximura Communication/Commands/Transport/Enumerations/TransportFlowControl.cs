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
using System.Collections;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Security.Principal;

using Ximura;
using Ximura.Server;
using Ximura.Helper;
#endregion // using
namespace Ximura.Communication
{
    /// <summary>
    /// This setting determines the transport duplex state.
    /// </summary>
    public enum TransportFlowControl
    {
        /// <summary>
        /// Full-duplex protocol connection can receive requests and send responses at 
        /// the same time.
        /// </summary>
        FullDuplex,        
        /// <summary>
        /// A half-duplex protocol connection can receive and transmit information, but
        /// not at the same time. This is used by standard Request/Response
        /// protocols.
        /// </summary>
        HalfDuplex,
        /// <summary>
        /// The protocol connection is a straight broadcast connection and does
        /// not receive any response to the information broadcast.
        /// </summary>
        SimplexOutgoing,
        /// <summary>
        /// The protocol connection is a straight listening connection and does
        /// not require any response to the information broadcast.
        /// </summary>
        SimplexIncoming,
        /// <summary>
        /// No specific format can be defined for this protocol.
        /// </summary>
        Custom
    }
}
