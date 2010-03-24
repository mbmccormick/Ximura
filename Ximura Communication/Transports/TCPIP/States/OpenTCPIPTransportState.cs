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
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Reflection;
using System.Drawing;
using System.Diagnostics;
using System.Runtime.Serialization;

using Ximura;
using Ximura.Data;

using CH = Ximura.Common;
using Ximura.Framework;
using Ximura.Framework;

#endregion // using
namespace Ximura.Communication
{
    public class OpenTCPIPTransportState : TransportState
    {
        #region Constructors
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        public OpenTCPIPTransportState() : this(null) { }
        /// <summary>
        /// This is the component model constructor.
        /// </summary>
        /// <param name="container">The container</param>
        public OpenTCPIPTransportState(IContainer container)
        {
            if (container != null)
                container.Add(this);
        }
        #endregion // Constructors

        public override bool ConnectionRequest(TransportContext context)
        {
            TransportContextTCPIP tcpContext = context as TransportContextTCPIP;
            IXimuraRQRSEnvelope Env = null;
            try
            {
                Env = context.EnvelopeHelper.GetCallback(context.ServerCommandID.Value);
                context.SenderIdentitySet(Env);
                Env.DestinationAddress = new EnvelopeAddress(context.ServerCommandID.Value, "ConnRQ");

                IXimuraProtocolConnectionRequest Request = Env.Request as IXimuraProtocolConnectionRequest;
                IXimuraProtocolConnectionResponse Response = Env.Response as IXimuraProtocolConnectionResponse;

                Request.ProtocolContextID = context.SignatureID;
                Request.RemoteUri = context.UriRemote;
                Request.LocalUri = context.UriLocal;
                Request.ConnectionType = TransportConnectionType.Connectionful;
                //We want a new server context, so we will not send an ID.
                Request.ServerContextID = context.ServerContextID;

                context.ContextSession.ProcessRequest(Env);

                if (Env.Response.Status != CH.HTTPCodes.OK_200)
                {
                    return false;
                }

                context.ServerContextID = Response.ServerContextID;
                context.MessageTypeIn = Response.MessageRequestType;
                //context.FlowControl = Response.FlowControl;

                if (Response.MessageResponse != null)
                {
                    context.MessageTransmit(Response.MessageResponse);
                }

                context.ClosePending = Response.CloseNotify;
                context.ConnectionStart();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
            finally
            {
                if (Env != null && Env.ObjectPoolCanReturn)
                    Env.ObjectPoolReturn();
            }

            return false;
        }

        public override bool OpenRequest(TransportContext context, SecurityManagerJob job, RQRSContract<TransportCommandRequest, TransportCommandResponse> Data)
        {
            return base.OpenRequest(context, job, Data);
        }

        public override bool OpenConfirm(TransportContext context, SecurityManagerJob job, RQRSContract<TransportCommandRequest, TransportCommandResponse> Data)
        {
            return base.OpenConfirm(context, job, Data);
        }

        public override bool Transmit(TransportContext context, SecurityManagerJob job, RQRSContract<TransportCommandRequest, TransportCommandResponse> Data)
        {
            return base.Transmit(context, job, Data);
        }

    }
}
