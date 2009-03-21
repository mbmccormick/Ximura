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
using Ximura.Server;
using Ximura.Command;

#endregion // using
namespace Ximura.Communication
{
    public class ListenTCPIPTransportState : TransportState
    {
        #region Constructors
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        public ListenTCPIPTransportState() : this(null) { }
        /// <summary>
        /// This is the component model constructor.
        /// </summary>
        /// <param name="container">The container</param>
        public ListenTCPIPTransportState(IContainer container)
        {
            if (container != null)
                container.Add(this);
        }
        #endregion // Constructors

        #region ListenRequest
        /// <summary>
        /// This method sets the transport context server connection parameters.
        /// </summary>
        /// <param name="context">The transport context.</param>
        /// <param name="job">The current job.</param>
        /// <param name="Data">The data.</param>
        /// <returns>Returns false as this context should be accepted.</returns>
        public override bool ListenRequest(TransportContext context, SecurityManagerJob job, RQRSContract<TransportCommandRequest, TransportCommandResponse> Data)
        {
            context.Location = Data.ContractRequest.TransportUri;
            context.ServerContextID = Data.ContractRequest.ServerContextID;
            Data.ContractResponse.ConnectionType = TransportConnectionType.Connectionful;
            Data.Response.Status = CH.HTTPCodes.OK_200;
            return false;
        }
        #endregion // ListenRequest
        #region ListenConfirm
        /// <summary>
        /// This method is called after the transport-server handshake is confirmed. This method starts the listening process.
        /// </summary>
        /// <param name="context">The transport context.</param>
        /// <param name="job">The job.</param>
        /// <param name="Data">The request/response data.</param>
        /// <returns>Returns false is the listening process has started successfully.</returns>
        public override bool ListenConfirm(TransportContext context, SecurityManagerJob job, RQRSContract<TransportCommandRequest, TransportCommandResponse> Data)
        {
            TransportContextTCPIP tcpContext = context as TransportContextTCPIP;
            if (tcpContext == null)
            {
                Data.Response.Status = CH.HTTPCodes.ServiceUnavailable_503;
                return true;
            }

            Uri location = tcpContext.Location;
            //Ok, validate the IP address
            IPAddress address;
            if (!IPAddress.TryParse(location.Host, out address))
            {
                if (location.Host.ToLowerInvariant() != "localhost")
                {
                    Data.Response.Status = CH.HTTPCodes.BadRequest_400;
                    Data.Response.Substatus = string.Format("Location '{0}' is not a valid listening address.", location.Host);
                    return true;
                }
                address = IPAddress.Loopback;
            }

            IPEndPoint EndPoint = new IPEndPoint(address, location.Port);

            try
            {
                Socket listeningSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                //We need to start listening on the appropriate ports and IP addresses
                listeningSocket.Bind(EndPoint);

                //Start the socket listening, the cnLISTENQUEUE is the number of items we allow
                //in the connection queue
                listeningSocket.Listen(tcpContext.ListenerQueueLength);

                //tcpContext.LocalEndPoint = EndPoint;
                tcpContext.ActiveSocket = listeningSocket;

                //Ok, we need to shunt off the listening duties to another thread so that we can returns to
                //the server on this thread.


                AsyncCallback startListening = new AsyncCallback(tcpContext.ListenerStart);

                startListening.BeginInvoke(null, null, null);

                Data.Response.Status = CH.HTTPCodes.OK_200;
                return false;

            }
            catch (SocketException socex)
            {
                Data.Response.Status = CH.HTTPCodes.BadRequest_400;
                Data.Response.Substatus = string.Format("The socket threw an exception {0}:{1} ({2})", socex.ErrorCode, socex.SocketErrorCode, socex.Message);
                return true;
            }
            catch (ObjectDisposedException obex)
            {
                Data.Response.Status = CH.HTTPCodes.InternalServerError_500;
                Data.Response.Substatus = string.Format("The socket threw an object disposed exception {0}", obex.Message);
                return true;
            }
            catch (Exception ex)
            {
                Data.Response.Status = CH.HTTPCodes.InternalServerError_500;
                Data.Response.Substatus = string.Format("An unhandeled exception was caught {0}/{1}", ex.GetType().ToString(), ex.Message);
                return true;
            }

        }
        #endregion // ListenConfirm

        public override void Close(TransportContext context)
        {
            base.Close(context);
        }

        public override bool Close(TransportContext context, SecurityManagerJob job, RQRSContract<TransportCommandRequest, TransportCommandResponse> Data)
        {
            return base.Close(context, job, Data);
        }
    }
}
