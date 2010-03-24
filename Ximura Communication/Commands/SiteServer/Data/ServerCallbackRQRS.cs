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
    /// <summary>
    /// This is the protocol callback request folder.
    /// </summary>
    public class RQCallbackServer : RQRSFolder,
        IXimuraProtocolConnectionRequest, IXimuraProtocolMessageReceived, IXimuraProtocolCloseNotificationRequest
    {
        #region Declarations
        private Guid? mProtocolContextID;
        private Guid? mServerContextID;
        private Uri mRemoteUri;
        private Uri mLocalUri;
        private IXimuraMessageStream mMessage;
        private long mMaxLength;
        private TransportConnectionType mConnectionType;
        private TransportCloseType mCloseType;

        #endregion // Declarations
        #region Constructors
        /// <summary>
        /// This is the default constuctor.
        /// </summary>
        public RQCallbackServer()
            : base()
        {
        }
        /// <summary>
        /// This is the component model constructor.
        /// </summary>
        /// <param name="container">The base container.</param>
        public RQCallbackServer(System.ComponentModel.IContainer container)
            : base(container)
        {
        }
        /// <summary>
        /// This is the deserialization constructor
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public RQCallbackServer(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
        #endregion

        #region Reset()
        /// <summary>
        /// This override resets the class parameters
        /// </summary>
        public override void Reset()
        {
            mProtocolContextID = null;
            mRemoteUri = null;
            mLocalUri = null;
            mMessage = null;
            mConnectionType = TransportConnectionType.Undefined;
            mCloseType = TransportCloseType.Undefined;
            base.Reset();
        }
        #endregion // Reset()

        #region ProtocolContextID
        /// <summary>
        /// This property is used to specify a specific context.
        /// </summary>
        public Guid? ProtocolContextID
        {
            get
            {
                return mProtocolContextID;
            }
            set
            {
                mProtocolContextID = value;
            }
        }
        #endregion // ContextID
        #region ServerContextID
        /// <summary>
        /// This property is used to specify a specific context.
        /// </summary>
        public Guid? ServerContextID
        {
            get
            {
                return mServerContextID;
            }
            set
            {
                mServerContextID = value;
            }
        }
        #endregion // ContextID
        #region Message
        public IXimuraMessageStream Message
        {
            get
            {
                return mMessage;
            }
            set
            {
                mMessage = value;
            }
        }
        #endregion

        #region RemoteUri
        public Uri RemoteUri
        {
            get
            {
                return mRemoteUri;
            }
            set
            {
                mRemoteUri = value;
            }
        }
        #endregion // Uri

        #region LocalUri
        public Uri LocalUri
        {
            get
            {
                return mLocalUri;
            }
            set
            {
                mLocalUri = value;
            }
        }
        #endregion

        #region ConnectionType
        public TransportConnectionType ConnectionType
        {
            get
            {
                return mConnectionType;
            }
            set
            {
                mConnectionType = value;
            }
        }
        #endregion // ConnectionType

        #region CloseType
        public TransportCloseType CloseType
        {
            get { return mCloseType; }
            set { mCloseType = value; }
        }
        #endregion // CloseType

    }

    public enum TransportCloseType
    {
        Undefined,
        SoftClose,
        HardClose
    }

    /// <summary>
    /// This is the protocol callback response folder.
    /// </summary>
    public class RSCallbackServer : RQRSFolder, IXimuraProtocolConnectionResponse,
        IXimuraProtocolMessageResponse, IXimuraProtocolCloseNotificationResponse
    {
        #region Declarations
        private Guid? mServerContextID;
        private Guid? mProtocolContextID;
        private IXimuraMessageStream mMessageResponse;
        private IXimuraMessageStream mMessageRequest;
        private Type mMessageRequestType;
        private long mMaxLength;
        private bool mCloseNotify;
        #endregion // Declarations

        #region Constructors
        /// <summary>
        /// This is the default constuctor.
        /// </summary>
        public RSCallbackServer()
            : base()
        {
        }
        /// <summary>
        /// This is the component model constructor.
        /// </summary>
        /// <param name="container">The base container.</param>
        public RSCallbackServer(System.ComponentModel.IContainer container)
            : base(container)
        {
        }
        /// <summary>
        /// This is the deserialization constructor
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public RSCallbackServer(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
        #endregion

        #region Reset()
        /// <summary>
        /// This override resets the various properties.
        /// </summary>
        public override void Reset()
        {
            mCloseNotify = false;
            mServerContextID = null;
            mProtocolContextID = null;
            mMessageResponse = null;
            mMessageRequest = null;
            mMessageRequestType = null;
            mMaxLength = -1;
            base.Reset();
        }
        #endregion // Reset()

        #region ProtocolContextID
        /// <summary>
        /// This property is used to specify a specific context.
        /// </summary>
        public Guid? ProtocolContextID
        {
            get
            {
                return mProtocolContextID;
            }
            set
            {
                mProtocolContextID = value;
            }
        }
        #endregion // ContextID
        #region ServerContextID
        /// <summary>
        /// This is the server context ID.
        /// </summary>
        public Guid? ServerContextID
        {
            get
            {
                return mServerContextID;
            }
            set
            {
                mServerContextID = value;
            }
        }
        #endregion

        #region MessageResponse
        public IXimuraMessageStream MessageResponse
        {
            get
            {
                return mMessageResponse;
            }
            set
            {
                mMessageResponse = value;
            }
        }
        #endregion
        #region MessageRequest
        public IXimuraMessageStream MessageRequest
        {
            get
            {
                return mMessageRequest;
            }
            set
            {
                mMessageRequest = value;
            }
        }
        #endregion
        #region MessageRequestType
        public Type MessageRequestType
        {
            get
            {
                return mMessageRequestType;
            }
            set
            {
                mMessageRequestType = value;
            }
        }
        #endregion

        #region MaxLength
        /// <summary>
        /// This property is the maximum number of bytes allowed for the response message.
        /// A value of -1 specifies no maximum limit.
        /// </summary>
        public long MaxLength
        {
            get
            {
                return mMaxLength;
            }
            set
            {
                mMaxLength = value;
            }
        }
        #endregion

        #region CloseAccept
        public bool CloseNotify
        {
            get { return mCloseNotify; }
            set { mCloseNotify = value; }
        }
        #endregion // CloseResponse

    }
}
