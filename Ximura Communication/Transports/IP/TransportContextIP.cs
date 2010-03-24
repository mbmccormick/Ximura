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
using System.IO;
using System.Net;
using System.Net.Sockets;

using Ximura;
using Ximura.Data;

using CH = Ximura.Common;
using Ximura.Framework;
using Ximura.Framework;

#endregion // using
namespace Ximura.Communication
{
    /// <summary>
    /// This context is used by IP based protocols.
    /// </summary>
    public class TransportContextIP : TransportContext
    {
        #region Declarations
        /// <summary>
        /// The sync object.
        /// </summary>
        protected object syncObject = null;
        #endregion // Declarations

        #region Constructor
        /// <summary>
        /// This is the default context for the PPC.
        /// </summary>
        public TransportContextIP(): base()
        {
        }
        #endregion

        #region Reset()
        /// <summary>
        /// This override resets the context to its initial value.
        /// </summary>
        public override void Reset()
        {
            ActiveSocket = null;
            base.Reset();
        }
        #endregion // Reset()

        #region SocketClose
        /// <summary>
        /// This method closes the buffer socket.
        /// </summary>
        public virtual void SocketClose()
        {
            SocketClose(ActiveSocket);
        }
        /// <summary>
        /// This method closes a socket.
        /// </summary>
        /// <param name="handler">The socket to close.</param>
        protected virtual void SocketClose(Socket socket)
        {
            //First check whether there is anything to close.
            if (socket == null)
                return;

            //Ok, gracefully shutdown the socket if it is connected.
            try
            {
                if (socket.Connected)
                    socket.Shutdown(SocketShutdown.Both);
            }
            finally
            {
                try
                {
                    //Whatever happens close it anyway even if we couldn't shutdown
                    socket.Close();
                }
                catch { }
            }
        }
        #endregion

        #region DNSResolve
        /// <summary>
        /// This method should be overriden to provide DNS integration with the Ximura.Communication DNS system.
        /// Currently, this method just calls the System.Net Dns class directly.
        /// </summary>
        /// <param name="context">The context that this call is running under.</param>
        /// <param name="Location">The location to resolve.</param>
        /// <returns>Returns a collection of IP addresses for the host, or null if the host cannot be
        /// resolved.</returns>
        protected virtual IPAddress[] DNSResolve(Uri Location)
        {
            return Dns.GetHostAddresses(Location.Host);
            //throw new ArgumentOutOfRangeException("Location", Location.AbsoluteUri + " is not a valid listening address.");

            //return null;
        }
        #endregion // DNSResolve

        #region ActiveSocket
        /// <summary>
        /// This is the socket for the connection.
        /// </summary>
        public virtual Socket ActiveSocket
        {
            get;
            set;
        }
        #endregion // Socket

        #region EndPointLocal
        /// <summary>
        /// This is the local end point.
        /// </summary>
        public virtual IPEndPoint EndPointLocal
        {
            get
            {
                return ActiveSocket == null ? (IPEndPoint)null : (IPEndPoint)ActiveSocket.LocalEndPoint;
            }
        }
        #endregion // ReceiveEndPoint
        #region EndPointRemote
        /// <summary>
        /// This is the remote end point.
        /// </summary>
        public virtual IPEndPoint EndPointRemote
        {
            get
            {
                if (ActiveSocket == null)
                    return null;

                try
                {
                    return (IPEndPoint)ActiveSocket.RemoteEndPoint;
                }
                catch
                {
                    return null;
                }
            }
        }
        #endregion // ReceiveEndPoint

        #region UriLocal
        /// <summary>
        /// This si the uri of the local endpoint
        /// </summary>
        public override Uri UriLocal
        {
            get
            {
                return UriBuild(EndPointLocal);
            }
        }
        #endregion // UriLocal
        #region UriRemote
        /// <summary>
        /// This is the Uri of the remote endpoint.
        /// </summary>
        public override Uri UriRemote
        {
            get
            {
                return UriBuild(EndPointRemote);
            }
        }
        #endregion // UriRemote

        #region UriBuild(IPEndPoint ep)
        /// <summary>
        /// This protected method builds the Uri from the Endpoint.
        /// </summary>
        /// <param name="ep">The endpoint.</param>
        /// <returns>The uri fromed from the endpoint.</returns>
        protected Uri UriBuild(IPEndPoint ep)
        {
            if (ep == null)
                return null;

            UriBuilder ub = new UriBuilder(DefaultScheme, ep.Address.ToString(), ep.Port);

            return ub.Uri;
        }
        #endregion // UriBuild(IPEndPoint ep)
    }
}
