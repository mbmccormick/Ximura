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
    /// This is the response folder for the transport command.
    /// </summary>
    public class TransportCommandResponse : RQRSFolder, IXimuraTransportResponse
    {
		#region Constructors
		/// <summary>
		/// This is the default constuctor.
		/// </summary>
		public TransportCommandResponse():base()
		{
		}
		/// <summary>
		/// This is the component model constructor.
		/// </summary>
		/// <param name="container">The base container.</param>
		public TransportCommandResponse(System.ComponentModel.IContainer container)//: base(container)
		{
		}

		/// <summary>
		/// This is the deserialization constructor
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
        public TransportCommandResponse(SerializationInfo info, StreamingContext context)//:base(info,context)
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
            ConnectionType = TransportConnectionType.Undefined;
            TransportUri = null;
            base.Reset();
        }
        #endregion // Reset()

        #region TransportContextID
        /// <summary>
        /// This property is used to specify a specific context.
        /// </summary>
        public Guid? TransportContextID
        {
            get;
            set;
        }
        #endregion // ContextID
        #region ConnectionType
        /// <summary>
        /// This property is used to specify a specific context.
        /// </summary>
        public TransportConnectionType ConnectionType
        {
            get;
            set;
        }
        #endregion // ContextID
        #region TransportUri
        /// <summary>
        /// This is the Uri that the command will listen on.
        /// </summary>
        public Uri TransportUri
        {
            get;
            set;
        }
        #endregion // ProtocolUri

        #region IXimuraTransportResponse Members

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

        #endregion
    }
}
