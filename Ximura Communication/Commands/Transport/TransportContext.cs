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
    /// <summary>
    /// This is the base context for the transport command.
    /// </summary>
    public class TransportContext : JobContext
        <
            TransportState,
            TransportSettings,
            TransportCommandRequest, TransportCommandResponse,
            TransportConfiguration, TransportPerformance
        >
    {
        #region Declarations
        private object syncTransmit;
        private object syncReceive;
        private int mFlagServerCall;

        private Queue<IXimuraMessageStream> mMessageTransmitQueue = null;
        private Queue<IXimuraMessageStream> mMessageReceiveQueue = null;
        #endregion // Declarations
        #region Constructor
        /// <summary>
        /// This is the default context for the PPC.
        /// </summary>
        public TransportContext()
            : base()
        {
        }
        #endregion

        #region Reset()
        /// <summary>
        /// This override resets the context to its original state.
        /// </summary>
        public override void Reset()
        {
            syncTransmit = new object();
            syncReceive = new object();

            if (mMessageTransmitQueue == null)
                mMessageTransmitQueue = new Queue<IXimuraMessageStream>();
            else
                PurgeQueue(mMessageTransmitQueue);

            if (mMessageReceiveQueue == null)
                mMessageReceiveQueue = new Queue<IXimuraMessageStream>();
            else
                PurgeQueue(mMessageReceiveQueue);

            mFlagServerCall = 0;

            UriLocal = null;
            UriRemote = null;
            ListenerConnectionsLimit = null;
            Location = null;
            ClosePending = false;
            ServerContextID = null;
            ServerCommandID = null;
            MessageInMaxLength = null;

            MessageReceiveCurrent = null;
            MessageTransmitCurrent = null;

            MessageTypeIn = null;
            base.Reset();
        }
        #endregion // Reset()

        #region PurgeQueue(Queue<IXimuraMessageStream> queue)
        /// <summary>
        /// This method removes any items still in the message queue and returns them to the object pool.
        /// </summary>
        /// <param name="queue">The queue to purge.</param>
        protected virtual void PurgeQueue(Queue<IXimuraMessageStream> queue)
        {
            while (queue.Count > 0)
            {
                IXimuraMessageStream item = queue.Dequeue();
                if (item.ObjectPoolCanReturn)
                    item.ObjectPoolReturn();
            }
        }
        #endregion // PurgeQueue(Queue<IXimuraMessageStream> queue)

        #region IN --> Initialize
        /// <summary>
        /// This method initializes the context.
        /// </summary>
        /// <param name="job">The current job.</param>
        /// <param name="Data">The request data.</param>
        public virtual void Initialize()
        {
            ChangeState();
            CurrentState.Initialize(this);
        }
        #endregion // IN --> Initialize
        #region IN --> ConnectionRequest
        /// <summary>
        /// This method handles a connection request to the server.
        /// </summary>
        /// <returns></returns>
        public bool ConnectionRequest()
        {
            return CurrentState.ConnectionRequest(this);
        }
        #endregion // ConnectionRequest

        #region IN --> ListenRequest
        /// <summary>
        /// The method initializes a listen request.
        /// </summary>
        /// <param name="job">The current job.</param>
        /// <param name="Data">The request data.</param>
        /// <returns>Returns true if the context should be reset and returned to the pool.</returns>
        public virtual bool ListenRequest(SecurityManagerJob job, RQRSContract<TransportCommandRequest, TransportCommandResponse> Data)
        {
            return CurrentState.ListenRequest(this, job, Data);
        }
        #endregion // IN --> ListenRequest
        #region IN --> ListenConfirm
        /// <summary>
        /// The method confirms the listening request and starts listening.
        /// </summary>
        /// <param name="job">The current job.</param>
        /// <param name="Data">The request data.</param>
        /// <returns>Returns true if the context should be reset and returned to the pool.</returns>
        public virtual bool ListenConfirm(SecurityManagerJob job, RQRSContract<TransportCommandRequest, TransportCommandResponse> Data)
        {
            return CurrentState.ListenConfirm(this, job, Data);
        }
        #endregion // IN --> ListenConfirm
        #region IN --> OpenRequest
        /// <summary>
        /// This method starts an open request.
        /// </summary>
        /// <param name="job">The current job.</param>
        /// <param name="Data">The request data.</param>
        /// <returns>Returns true if the context should be reset and returned to the pool.</returns>
        public virtual bool OpenRequest(SecurityManagerJob job, RQRSContract<TransportCommandRequest, TransportCommandResponse> Data)
        {
            return CurrentState.OpenRequest(this, job, Data);
        }
        #endregion // IN --> OpenRequest
        #region IN --> OpenConfirm
        /// <summary>
        /// This method start the connection and confirms the open request.
        /// </summary>
        /// <param name="job">The current job.</param>
        /// <param name="Data">The request data.</param>
        /// <returns>Returns true if the context should be reset and returned to the pool.</returns>
        public virtual bool OpenConfirm(SecurityManagerJob job, RQRSContract<TransportCommandRequest, TransportCommandResponse> Data)
        {
            return CurrentState.OpenConfirm(this, job, Data);
        }
        #endregion // IN --> OpenConfirm
        #region IN --> Transmit
        /// <summary>
        /// This method transits a message to the remote party.
        /// </summary>
        /// <param name="job">The current job.</param>
        /// <param name="Data">The request data.</param>
        /// <returns>Returns true if the context should be reset and returned to the pool.</returns>
        public virtual bool Transmit(SecurityManagerJob job, RQRSContract<TransportCommandRequest, TransportCommandResponse> Data)
        {
            return CurrentState.Transmit(this, job, Data);
        }
        #endregion // IN --> Transmit
        #region IN --> Close
        /// <summary>
        /// This method closes a context.
        /// </summary>
        /// <param name="job">The current job.</param>
        /// <param name="Data">The request data.</param>
        /// <returns>Returns true if the context should be reset and returned to the pool.</returns>
        public virtual bool Close(SecurityManagerJob job, RQRSContract<TransportCommandRequest, TransportCommandResponse> Data)
        {
            return CurrentState.Close(this,job,Data);
        }
        #endregion // Close

        #region Close()
        /// <summary>
        /// This method is a hard close initiated by the Transport command.
        /// </summary>
        public virtual void Close()
        {
            try
            {

                if (ClosePending)
                    return;

                if (CurrentState!=null)
                    CurrentState.Close(this);
            }
            catch { }
        }
        #endregion // Close()

        #region Properties

        public Guid? ServerCommandID
        {
            get;
            set;
        }
        public Guid? ServerContextID
        {
            get;
            set;
        }

        public bool ClosePending
        {
            get;
            set;
        }

        public Uri Location
        {
            get;
            set;
        }

        //public Guid? TransportID
        //{
        //    get;
        //    set;
        //}
        #endregion

        #region FlowControl
        /// <summary>
        /// This is the flow control for the connection.
        /// </summary>
        public TransportFlowControl FlowControl
        {
            get { return TransportFlowControl.FullDuplex; }
        }
        #endregion // FlowControl

        #region UriLocal
        /// <summary>
        /// This is the Uri of the local party.
        /// </summary>
        public virtual Uri UriLocal
        {
            get;
            protected set;
        }
        #endregion // UriLocal
        #region UriRemote
        /// <summary>
        /// This is the Uri of the remote party.
        /// </summary>
        public virtual Uri UriRemote
        {
            get;
            protected set;
        }
        #endregion // UriRemote

        #region DefaultScheme
        /// <summary>
        /// This method returns the scheme used to build the uri remote and local schemes.
        /// </summary>
        protected virtual string DefaultScheme
        {
            get { throw new NotImplementedException("DefaultScheme is not implemented."); }
        }
        #endregion // DefaultScheme

        #region MessageTypeIn
        /// <summary>
        /// This is the message object type used for the next incoming message.
        /// </summary>
        public Type MessageTypeIn
        {
            get;
            set;
        }
        #endregion // MessageType
        #region MessageTypeInGetObject()
        /// <summary>
        /// This protected method retrieves a new message in object base on the specified type.
        /// </summary>
        /// <returns>Return null if the type cannot be resolved, or the pool is empty.</returns>
        public virtual IXimuraMessageStream MessageTypeInGetObject()
        {
            //ClosedCheck();
            IXimuraPool pool = GetObjectPool(MessageTypeIn);

            if (pool == null)
                return null;

            IXimuraMessageStream obj = pool.Get() as IXimuraMessageStream;
            if (MessageInMaxLength.HasValue)
                obj.Load(MessageInMaxLength.Value);
            else
                obj.Load();

            return obj;
        }
        #endregion // MessageTypeInGetObject()
        #region MessageInMaxLength
        /// <summary>
        /// This is the maximum permitted length for the incoming message. If this is set to null, then no maximum size is defined.
        /// </summary>
        public long? MessageInMaxLength
        {
            get;
            set;
        }
        #endregion // MessageInMaxLength

        #region ListenerConnectionsLimit
        /// <summary>
        /// This property specifies the number of connections that can be accepted before the listening socket must close.
        /// </summary>
        public int? ListenerConnectionsLimit
        {
            get;
            set;
        }
        #endregion // ListenerConnectionsLimit

        #region MessageTransmit(IXimuraMessageStream message)
        /// <summary>
        /// This method adds a message in to the outgoing send queue.
        /// </summary>
        /// <param name="message">The message to add to the send queue.</param>
        public virtual void MessageTransmit(IXimuraMessageStream message)
        {
            try
            {
                lock (syncTransmit)
                {
                    MessageTransmitQueue.Enqueue(message);
                }

                ConnectionTransmit();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion // Write(IXimuraMessageStream message)

        #region MessageTransmitQueue
        /// <summary>
        /// This property contains the message out queue and will be used by protocols
        /// that process multiple messages concurrently.
        /// </summary>
        private Queue<IXimuraMessageStream> MessageTransmitQueue
        {
            get{return mMessageTransmitQueue;}
        }
        #endregion // MessageOutQueue
        #region MessageTransmitCurrent
        /// <summary>
        /// This is the current message being transmitted to the endpoint.
        /// </summary>
        private IXimuraMessageStream MessageTransmitCurrent
        {
            get;
            set;
        }
        #endregion // MessageTransmitCurrent
        #region Read(byte[] data, int offset, int length)
        /// <summary>
        /// This method writes bytes from the current message to send to the remote endpoint.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="offset">The byte offset.</param>
        /// <param name="length">The length.</param>
        /// <returns>Returns the number of bytes read.</returns>
        protected virtual int Read(byte[] data, int offset, int length)
        {
            lock (syncTransmit)
            {
                try
                {
                    if (MessageTransmitCurrent == null && MessageTransmitQueue.Count == 0)
                        return 0;
                    else if (MessageTransmitCurrent == null)
                        MessageTransmitCurrent = MessageTransmitQueue.Dequeue();

                    int len = MessageTransmitCurrent.Read(data, offset, length);

                    if (!MessageTransmitCurrent.CanRead)
                        MessageTransmitCurrent = null;

                    return len;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
        #endregion // Read(byte[] data, int offset, int length)
        #region CanTransmit
        /// <summary>
        /// This property identifies whether the context can transmit data to the remote endpoint.
        /// </summary>
        protected virtual bool CanTransmit
        {
            get
            {
                lock (syncTransmit)
                {
                    if (ClosePending)
                        return false;

                    return (MessageTransmitCurrent != null && MessageTransmitCurrent.CanRead)
                        || (MessageTransmitCurrent == null && MessageTransmitQueue.Count > 0);
                }
            }
        }
        #endregion // CanTransmit

        #region ServerCall(IAsyncResult ar)
        /// <summary>
        /// This method transmit the receive queue to the remote server on a seperate thread.
        /// </summary>
        /// <param name="ar"></param>
        private void ServerCall(IAsyncResult ar)
        {
            try
            {
                Interlocked.Increment(ref mFlagServerCall);

                if (mFlagServerCall > 1)
                    return;

                //OK, we are going to loop until there are no more messages in the queue, or the connection is closing.
                while (!ClosePending)
                {
                    IXimuraMessageStream message = null;

                    lock (syncReceive)
                    {
                        if (MessageReceiveQueue.Count == 0)
                            return;

                        message = MessageReceiveQueue.Dequeue();
                    }

                    if (message == null)
                        return;

                    IXimuraRQRSEnvelope Env = EnvelopeHelper.GetCallback(ServerCommandID.Value);
                    SenderIdentitySet(Env);
                    Env.DestinationAddress = new EnvelopeAddress(ServerCommandID.Value, "Receive");
                    IXimuraProtocolMessageReceived Request = Env.Request as IXimuraProtocolMessageReceived;
                    IXimuraProtocolMessageResponse Response = Env.Response as IXimuraProtocolMessageResponse; 

                    Request.ServerContextID = ServerContextID;
                    Request.ProtocolContextID = SignatureID;
                    Request.Message = message;

                    ContextSession.ProcessRequest(Env);

                    if (Env.Response.Status != CH.HTTPCodes.OK_200)
                    {
                        Close();
                        return;
                    }

                    //Check whether we have been notified to close, if we have then we should not accept ant more incoming messages.
                    if (!Response.CloseNotify)
                    {
                        MessageTypeIn = Response.MessageRequestType;
                        MessageInMaxLength = Response.MaxLength;
                    }
                    else
                    {
                        MessageTypeIn = null;
                        MessageInMaxLength = null;
                    }

                    //Transmit any outgoing messages.
                    if (Response.MessageResponse != null)
                        MessageTransmit(Response.MessageResponse);

                    if (Response.CloseNotify)
                        Close();
                }
            }
            catch (Exception ex)
            {
                //Any exceptions are caught and ignored.
            }
            finally
            {
                Interlocked.Decrement(ref mFlagServerCall);
            }
        }
        #endregion // ServerCall(IAsyncResult ar)

        #region CanReceive
        /// <summary>
        /// This boolean property identifies whether the context can receive data from the remote endpoint.
        /// </summary>
        protected virtual bool CanReceive
        {
            get
            {
                lock (syncReceive)
                {
                    if (ClosePending)
                        return false;

                    return (MessageReceiveCurrent != null && MessageReceiveCurrent.CanWrite)
                        || (MessageReceiveCurrent == null && MessageTypeIn != null);
                }
            }
        }
        #endregion // CanReceive
        #region MessageReceiveCurrent
        /// <summary>
        /// This is the current message being received from the remote endpoint.
        /// </summary>
        private IXimuraMessageStream MessageReceiveCurrent
        {
            get;
            set;
        }
        #endregion // MessageReceiveCurrent
        #region MessageReceiveQueue
        /// <summary>
        /// This property contains the incoming message queue and will be used
        /// by protocols that process multiple messages concurrently.
        /// </summary>
        private Queue<IXimuraMessageStream> MessageReceiveQueue
        {
            get { return mMessageReceiveQueue; }
        }
        #endregion // MessageInQueue
        #region Write(byte[] blob, int offset, int length)
        /// <summary>
        /// This method writes the transmitted bytes to the message object.
        /// </summary>
        /// <param name="blob">The transmitted blob.</param>
        /// <param name="start">The offset.</param>
        /// <param name="length">The length.</param>
        protected virtual void Write(byte[] blob, int offset, int length)
        {
            lock (syncReceive)
            {
                WriteInternal(blob, offset, length);
            }
        }

        private void WriteInternal(byte[] blob, int offset, int length)
        {
            if (MessageReceiveCurrent == null)
                if (MessageTypeIn != null)
                    MessageReceiveCurrent = MessageTypeInGetObject();
                else
                    return;

            int bytesRead = MessageReceiveCurrent.Write(blob, offset, length);

            if (!MessageReceiveCurrent.CanWrite)
            {
                MessageReceiveQueue.Enqueue(MessageReceiveCurrent);
                MessageReceiveCurrent = null;

                if (MessageReceiveQueue.Count>0 && mFlagServerCall == 0)
                {
                    AsyncCallback cb = new AsyncCallback(ServerCall);
                    cb.BeginInvoke(null, null, null);
                }
            }

            if (bytesRead > 0 && bytesRead < length)
                WriteInternal(blob, offset + bytesRead, length - bytesRead);
        }
        #endregion // Write(byte[] blob, int offset, int length)

        #region ConnectionStart()
        /// <summary>
        /// This method starts the connection.
        /// </summary>
        public virtual void ConnectionStart()
        {
            ConnectionInitialize();

            ConnectionTransmit();

            ConnectionReceive();
        }
        #endregion // StartConnection()

        #region ConnectionInitialize()
        /// <summary>
        /// This method initializes any transport specific properties such as a byte buffer.
        /// </summary>
        protected virtual void ConnectionInitialize()
        {
            
        }
        #endregion // ConnectionInitialize()
        #region ConnectionTransmit()
        /// <summary>
        /// This method triggers a transmit if one is not active already.
        /// </summary>
        protected virtual void ConnectionTransmit()
        {
            throw new NotImplementedException("Transmit is not implemented.");
        }
        #endregion // ConnectionTransmit()
        #region ConnectionReceive()
        /// <summary>
        /// This method triggers a receive if one is not active already.
        /// </summary>
        protected virtual void ConnectionReceive()
        {
            throw new NotImplementedException("Receive is not implemented.");
        }
        #endregion // ConnectionReceive()
    }
}
