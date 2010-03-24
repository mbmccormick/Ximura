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
    /// This is the request folder for the transport command.
    /// </summary>
    public class TransportCommandRequest: RQRSFolder, IXimuraTransportRequest
    {
		#region Constructors
		/// <summary>
		/// This is the default constuctor.
		/// </summary>
		public TransportCommandRequest():base()
		{
		}
		/// <summary>
		/// This is the component model constructor.
		/// </summary>
		/// <param name="container">The base container.</param>
		public TransportCommandRequest(System.ComponentModel.IContainer container)//: base(container)
		{
		}

		/// <summary>
		/// This is the deserialization constructor
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
        public TransportCommandRequest(SerializationInfo info, StreamingContext context)//:base(info,context)
		{
		}
		#endregion
        #region Reset()
        /// <summary>
        /// This override resets the class parameters
        /// </summary>
        public override void Reset()
        {
            TransportContextID = null;
            ServerContextID = null;
            TransportUri = null;
            TransportUriConnectionLimit = null;
            ServerAddress = null;
            MessageType = null;
            Message = null;
            SignalClose = false;
            base.Reset();
        }
        #endregion // Reset()

        #region TransportContextID
        /// <summary>
        /// This property specifies the context on the Transport.
        /// </summary>
        public Guid? TransportContextID
        {
            get;
            set;
        }
        #endregion
        #region ServerContextID
        /// <summary>
        /// This property specifies the context on the Server.
        /// </summary>
        public Guid? ServerContextID
        {
            get;
            set;
        }
        #endregion

        #region TransportUri
        /// <summary>
        /// This property specifies the uri for the transport
        /// </summary>
        public Uri TransportUri
        {
            get;
            set;
        }
        #endregion // TransportUri
        #region TransportUriConnectionLimit
        /// <summary>
        /// This property specifies the maximum number of connections permitted while in listening mode.
        /// </summary>
        public int? TransportUriConnectionLimit
        {
            get;
            set;
        }
        #endregion // TransportUriConnectionLimit

        #region ServerAddress
        /// <summary>
        /// This property specifies the address of the server command responsible for the transport connection.
        /// </summary>
        public EnvelopeAddress? ServerAddress
        {
            get;
            set;
        }
        #endregion // ServerAddress

        #region MessageType
        /// <summary>
        /// This property specifies the message type for new data.
        /// </summary>
        public Type MessageType
        {
            get;
            set;
        }
        #endregion // MessageType

        #region Message
        /// <summary>
        /// This property contains the message to be transmitted to the remote party.
        /// </summary>
        public IXimuraMessageStream Message
        {
            get;
            set;
        }
        #endregion // Message
        #region SignalClose
        /// <summary>
        /// This property signals that the connection should be closed.
        /// </summary>
        public bool SignalClose
        {
            get;
            set;
        }
        #endregion // SignalClose


        #region IXimuraTransportRequest Members


        public Guid? ProtocolContextID
        {
            get
            {
                return TransportContextID;
            }
            set
            {
                TransportContextID = value;
            }
        }

        public Uri ProtocolUri
        {
            get
            {
                return TransportUri;
            }
            set
            {
                TransportUri = value;
            }
        }


        public int? ProtocolUriConnectionLimit
        {
            get
            {
                return TransportUriConnectionLimit;
            }
            set
            {
                TransportUriConnectionLimit = value;
            }
        }


        #endregion
    }
}
