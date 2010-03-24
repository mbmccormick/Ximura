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
using System.Threading;
using System.Timers;
using System.Data;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Net;
using System.Net.Sockets;
using System.Diagnostics;
using System.Runtime.Serialization;

using Ximura;
using Ximura.Data;
using Ximura.Helper;
using CH = Ximura.Helper.Common;
using Ximura.Framework;
using Ximura.Framework;

#endregion // using
namespace Ximura.Communication
{
    public class CloseTCPIPTransportState : TransportState
    {
        #region Constructors
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        public CloseTCPIPTransportState() : this(null) { }
        /// <summary>
        /// This is the component model constructor.
        /// </summary>
        /// <param name="container">The container</param>
        public CloseTCPIPTransportState(IContainer container)
        {
            if (container != null)
                container.Add(this);
        }
        #endregion // Constructors

        #region Close
        /// <summary>
        /// This close method is initiated by the remote server.
        /// </summary>
        /// <param name="context">The current context.</param>
        /// <param name="job">The job.</param>
        /// <param name="Data">The request/response data.</param>
        /// <returns></returns>
        public override bool Close(TransportContext context, SecurityManagerJob job, RQRSContract<TransportCommandRequest, TransportCommandResponse> Data)
        {
            TransportContextTCPIP tcpContext = context as TransportContextTCPIP;

            tcpContext.ClosePending = true;
            tcpContext.SocketClose();

            return true;
        }
        /// <summary>
        /// This close method is initiated by the transport context.
        /// </summary>
        /// <param name="context">The current context.</param>
        public override void Close(TransportContext context)
        {
            TransportContextTCPIP tcpContext = context as TransportContextTCPIP;

            if (tcpContext.ClosePending)
                return;

            tcpContext.ClosePending = true;

            IXimuraRQRSEnvelope Env = null;
            try
            {
                Env = context.EnvelopeHelper.GetCallback(context.ServerCommandID.Value);
                context.SenderIdentitySet(Env);
                Env.DestinationAddress = new EnvelopeAddress(context.ServerCommandID.Value, "Close");
                IXimuraProtocolConnectionRequest Request = Env.Request as IXimuraProtocolConnectionRequest;
                IXimuraProtocolConnectionResponse Response = Env.Response as IXimuraProtocolConnectionResponse;

                Request.ProtocolContextID = context.SignatureID;
                Request.RemoteUri = context.UriRemote;
                Request.LocalUri = context.UriLocal;
                Request.ConnectionType = TransportConnectionType.Connectionful;
                //We want a new server context, so we will not send an ID.
                Request.ServerContextID = context.ServerContextID;

                context.ContextSession.ProcessRequest(Env);
            }
            finally
            {
                if (Env != null && Env.ObjectPoolCanReturn)
                    Env.ObjectPoolReturn();
            }


            tcpContext.SocketClose();
        }
        #endregion // Close

    }
}
