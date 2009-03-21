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

using Ximura;
using Ximura.Helper;
using Ximura.Server;
using Ximura.Command;

#endregion // using
namespace Ximura.Communication
{
    /// <summary>
    /// This enumeration defines the standard interaction from the protocol to the server.
    /// </summary>
    public enum ServerAction
    {
        /// <summary>
        /// This action type is sent when a new connection request is received by the protocol.
        /// </summary>
        ConnectionRequest,
        /// <summary>
        /// This action type is used when a connection close event is initiated by the protocol.
        /// </summary>
        ConnectionClosed,
        /// <summary>
        /// This action type is used when a message is received for the particular connection.
        /// </summary>
        MessageReceived,
        /// <summary>
        /// This action type is reserved for custom messages between the protocol and the server.
        /// </summary>
        Custom
    }

    /// <summary>
    /// This enumeration defines the type of action that server can request from the protocol.
    /// </summary>
    public enum ProtocolAction
    {
        /// <summary>
        /// This action type is used by the server when attempting to 
        /// initiate a listening connection.
        /// </summary>
        Listen,
        /// <summary>
        /// This action type is used by the server when attemption to 
        /// initiate a remote connection in passive mode.
        /// </summary>
        OpenPassive,
        /// <summary>
        /// This action type is used by the server when attemption to 
        /// initiate a remote connection in active mode.
        /// </summary>
        OpenActive,
        /// <summary>
        /// This action type is used by the server to attempt to close an active connection.
        /// </summary>
        CloseConnection,
        /// <summary>
        /// This action type is used by the server when transmitting a message to an active connection.
        /// </summary>
        Transmit,
        /// <summary>
        /// This action type is reserved for custom messages between the server and the protocol.
        /// </summary>
        Custom
    }
}
