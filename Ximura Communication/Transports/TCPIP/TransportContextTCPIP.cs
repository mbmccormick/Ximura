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
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Drawing;
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
    public class TransportContextTCPIP : TransportContextIP
    {
        #region Declarations
        private ManualResetEvent allDone;
        /// <summary>
        /// This is the buffer used to read bytes from the socket.
        /// </summary>
        private byte[] TransmitBuffer;
        /// <summary>
        /// This is the buffer used to send bytes to the remote socket.
        /// </summary>
        private byte[] ReceiveBuffer;

        private int flagTransmit;
        private int flagReceive;
        #endregion // Declarations
        #region Constructor
        /// <summary>
        /// This is the default context for the PPC.
        /// </summary>
        public TransportContextTCPIP()
            : base()
        {
        }
        #endregion

        #region Reset()
        /// <summary>
        /// This method resets the context to it's default values.
        /// </summary>
        public override void Reset()
        {
            TransmitBuffer = null;
            ReceiveBuffer = null;

            flagTransmit = 0;
            flagReceive = 0;

            allDone = null;
            ListenerQueueLength = 10;
            ListenerConnectionsLimit = null;

            base.Reset();
        }
        #endregion // Reset()

        #region DefaultScheme
        /// <summary>
        /// This override sets 'tcp' as the default scheme 
        /// </summary>
        protected override string DefaultScheme
        {
            get { return "tcp"; }
        }
        #endregion // DefaultScheme

        #region ListenerQueueLength
        /// <summary>
        /// This is the listener queue length. This property specifies the number of pending connection the listening socket will allow.
        /// </summary>
        public virtual int ListenerQueueLength
        {
            get;
            protected set;
        }
        #endregion // ListenerQueueLength
        #region ListenerStart
        /// <summary>
        /// This method processes connection requests  by 
        /// sending a async call to the AcceptCallback method
        /// </summary>
        /// <param name="listener">The listening socket</param>
        public virtual void ListenerStart(IAsyncResult ar)
        {

#if DEBUG
            Debug.WriteLine(
                String.Format("TCPIPTransportBuffer --> Listener Started: {0} <{1}>"
                , EndPointLocal.ToString()
                , SignatureID.ToString()
                ));
#endif
            try
            {
                //Set the manual reset event. This will be used to process the incoming connections sequeentially.
                allDone = new ManualResetEvent(false);

                try
                {
                    //Let's get listening for connections ... and loop until we are closing
                    while (!ListenerConnectionsLimit.HasValue || ListenerConnectionsLimit > 0)
                    {
                        // Set the event to nonsignaled state.
                        allDone.Reset();

                        // Start an asynchronous socket to listen for connections.
                        ActiveSocket.BeginAccept(ListenerConnectionRequest, SignatureID.Value);

                        // Wait until a connection is made before continuing.
                        // This may also proceed when the listener is closing down.
                        allDone.WaitOne();//1000,true

                        if (ListenerConnectionsLimit.HasValue)
                            ListenerConnectionsLimit--;
                    }
                }
                catch (Exception ex)
                {
                    XimuraAppTrace.WriteLine("StartSocketAccept -> Close error: " + ex.ToString(), "TransportContextTCPIP/StartSocketAccept", EventLogEntryType.Warning);
                }

                //Close the socket as we have finished with it and inform the context that we are no longer listening.
                Close();

#if DEBUG
                //Debug.WriteLine("TCPIPTransportBuffer --> Listener Closed:" + ID.ToString() + "<" + (string)((Context==null)?"No context":Context.TrackID.ToString())  + ">" + TrackID.ToString());
                //Debug.WriteLine(
                //    String.Format("TCPIPTransportBuffer --> Listener Closed: {0} <{1}> {2} [{3}]"
                //    , listener.LocalEndPoint.ToString()
                //    , ID.ToString()
                //    , (string)((Context == null) ? "No context" : Context.TrackID.ToString())
                //    , TrackID.ToString()
                //    ));
#endif
            }
            catch (Exception ex)
            {
                //It's important that we do not allow unhandled exception to leave here
                //as we are not operating under a Ximura thread and any unhandled exception
                //will crash the service.
                //XimuraAppTrace.WriteLine("StartSocketAccept Unhandled error: " + ex.ToString(), BufferName, EventLogEntryType.Error);
            }
            //BufferAccess.BufferReturn(this);
        }
        #endregion // StartSocketAccept
        #region ListenerConnectionRequest
        /// <summary>
        /// This method receives async connection requests on the listening socket
        /// and processes them.
        /// </summary>
        /// <param name="ar">The async call back object containing the connection request</param>
        private void ListenerConnectionRequest(IAsyncResult ar)
        {
            TransportContextTCPIP connContext = null;
            //Do the declarations for the try-catch block
            try
            {
                //Get the listening buffer
                Guid trackID = (Guid)ar.AsyncState;

                //Check whether the context has been recycled, and if so ignore the request.
                if (trackID != SignatureID.Value)
                    return;

                // Get the new socket reference.
                Socket handler = ActiveSocket.EndAccept(ar);


                if (handler == null)
                {
                    XimuraAppTrace.WriteLine("AcceptCallback handler is null, closing listener.",
                        "TransportContextTCPIP/AcceptCallback", EventLogEntryType.Warning);
                    Close();
                    return;
                }

#if DEBUG
                //Debug.WriteLine(
                //    String.Format("TCPIPTransportBuffer --> AcceptCallback: {0} <{1}> {2} [{3}]"
                //    , handler.RemoteEndPoint.ToString()
                //    , ID.ToString()
                //    , (string)((Context == null) ? "No context" : Context.TrackID.ToString())
                //    , TrackID.ToString()
                //    ));
#endif

                //Check that we can access the context pool to get a new context to handle the connection?
                if (!ContextPoolAccessGranted)
                {
                    //No, then we have to decline the connection as we cannot retrieve a context to process the connection.
                    SocketClose(handler);
                    XimuraAppTrace.WriteLine("Child contexts cannot be accessed. Closing incoming connections.",
                        "TransportContextTCPIP/AcceptCallback", EventLogEntryType.Warning);
                    return;
                }

                connContext = ContextPoolAccess.ContextGetGeneric() as TransportContextTCPIP;
                if (connContext == null)
                {
                    SocketClose(handler);
                    XimuraAppTrace.WriteLine("No child contexts available. Closing incoming connections.",
                        "TransportContextTCPIP/AcceptCallback", EventLogEntryType.Error);
                    return;
                }
                //Ok, set the socket and copy the remote server command ID
                connContext.Initialize();
                connContext.ActiveSocket = handler;
                connContext.ServerCommandID = ServerCommandID;

                //Initiate the connection request with the server.
                if (!connContext.ConnectionRequest())
                {
                    //If the connection request is not successful, then close the context and return it to the pool.
                    connContext.Close();
                    ContextPoolAccess.ContextReturnGeneric(connContext);
                    connContext = null;
                    return;
                }

            }
            catch (Exception ex)
            {
                if (connContext != null)
                {
                    connContext.Close();
                    ContextPoolAccess.ContextReturnGeneric(connContext);
                }
            }
            finally
            {
                // Signal the main thread to continue, if it has not been disposed.
                // This will allow new connections to be processed.
                if (allDone != null)
                    allDone.Set();
            }
        }
        #endregion // AcceptCallback

        #region ConnectionInitialize()
        /// <summary>
        /// This override initializes the byte buffers for the connection.
        /// </summary>
        protected override void ConnectionInitialize()
        {
            TransmitBuffer = new byte[0x1000];
            ReceiveBuffer = new byte[0x1000];
        }
        #endregion // ConnectionInitialize()

        #region ConnectionTransmit()
        /// <summary>
        /// This method initiates transmission if it is not already active.
        /// </summary>
        protected override void ConnectionTransmit()
        {
            if (flagTransmit > 0)
                return;

            try
            {
                Interlocked.Increment(ref flagTransmit);

                if (flagTransmit == 1 && CanTransmit)
                {
                    int len = Read(TransmitBuffer, 0, TransmitBuffer.Length);

                    if (len > 0)
                        ActiveSocket.BeginSend(TransmitBuffer, 0, len, SocketFlags.None, TransmitCallback, SignatureID.Value);
                    else
                        Interlocked.Decrement(ref flagTransmit);

                }
                else
                    Interlocked.Decrement(ref flagTransmit);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion // ConnectionTransmit()
        #region TransmitCallback(IAsyncResult ar)
        private void TransmitCallback(IAsyncResult ar)
        {
            Guid trackID;
            try
            {
                //Get the listening buffer
                trackID = (Guid)ar.AsyncState;
                //Check whether the context has been recycled.
                if (!SignatureID.HasValue || trackID != SignatureID)
                    return;

                int len = ActiveSocket.EndSend(ar);

                if (CanTransmit)
                {
                    len = Read(TransmitBuffer, 0, TransmitBuffer.Length);

                    if (len > 0)
                    {
                        ActiveSocket.BeginSend(TransmitBuffer, 0, len, SocketFlags.None, TransmitCallback, SignatureID);
                        return;
                    }
                }

                Interlocked.Decrement(ref flagTransmit);

            }
            catch (Exception ex)
            {
                Close();
            }
        }
        #endregion // SendCallback(IAsyncResult ar)

        #region ConnectionReceive()
        /// <summary>
        /// This method initiates the socket receive.
        /// </summary>
        protected override void ConnectionReceive()
        {
            ActiveSocket.BeginReceive(this.ReceiveBuffer, 0, this.ReceiveBuffer.Length, SocketFlags.None, ReceiveCallback, SignatureID.Value);
        }
        #endregion // ConnectionReceive()
        #region ReceiveCallback(IAsyncResult ar)
        private void ReceiveCallback(IAsyncResult ar)
        {
            Guid tracker;
            int bytesRead = 0;
            try
            {
                //Get the listening buffer
                tracker = (Guid)ar.AsyncState;
                //Check whether the context has been recycled.
                if (!SignatureID.HasValue || tracker != SignatureID)
                    return;

                bytesRead = ActiveSocket.EndReceive(ar);

                if (bytesRead == 0)
                {
                    if (tracker == SignatureID)
                        Close();
                    return;
                }
            }
            catch (SocketException sex)
            {
                Close();
                return;
            }
            catch (ObjectDisposedException odex)
            {
                return;
            }
            catch (InvalidOperationException iopex)
            {
                Close();
                return;
            }
            catch (Exception ex)
            {
                Close();
                return;
            }

            try
            {
                if (CanReceive)
                    Write(ReceiveBuffer, 0, bytesRead);

                ActiveSocket.BeginReceive(this.ReceiveBuffer, 0, this.ReceiveBuffer.Length,
                    SocketFlags.None, ReceiveCallback, SignatureID.Value);

            }
            catch (SocketException sex)
            {
                if (tracker == SignatureID)
                    Close();
            }
            catch (ObjectDisposedException odex)
            {
                if (tracker == SignatureID)
                    Close();
            }
            catch (InvalidOperationException iopex)
            {
                if (tracker == SignatureID)
                    Close();
            }
            catch (Exception ex)
            {
                if (tracker == SignatureID)
                    Close();
            }
        }
        #endregion // ReceiveCallback(IAsyncResult ar)

    }
}
