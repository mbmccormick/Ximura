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
using Ximura.Helper;
using CH = Ximura.Helper.Common;
using Ximura.Framework;
using Ximura.Framework;

#endregion // using
namespace Ximura.Communication
{
    public class TransportException: XimuraException
    {
        #region Declarations
        private string mStatus = string.Empty;
        private string mSubStatus = string.Empty;
        #endregion
		#region Constructors
        /// <summary>
		/// Initializes a new instance of the XimuraException class.
		/// </summary>
		/// <param name="message"></param>
		public TransportException(string message, string status, string subStatus):
			this(message)
        {
            this.mStatus = status;
            this.mSubStatus = subStatus;
        }
		/// <summary>
		/// Initializes a new instance of the XimuraException class.
		/// </summary>
		/// <param name="message"></param>
		/// <param name="ex"></param>
        public TransportException(string message, string status, string subStatus, Exception ex) :
			this(message, ex)
        {
            this.mStatus = status;
            this.mSubStatus = subStatus;
        }
		/// <summary>
		/// Initializes a new instance of the XimuraException class.
		/// </summary>
		public TransportException():base(){}
		/// <summary>
		/// Initializes a new instance of the XimuraException class.
		/// </summary>
		/// <param name="message">The error message.</param>
		public TransportException(string message):base(message){}
		/// <summary>
		/// Initializes a new instance of the XimuraException class.
		/// </summary>
		/// <param name="message">The error message.</param>
		/// <param name="ex">The base exception.</param>
		public TransportException(string message,Exception ex):base(message,ex){}

		/// <summary>
		/// This exception is used for deserialization.
		/// </summary>
		/// <param name="info">The serialization info.</param>
		/// <param name="context">The serialization context.</param>
        protected TransportException(SerializationInfo info, StreamingContext context) : base(info, context) { }
		#endregion // Constructors

        /// <summary>
        /// The data session error code.
        /// </summary>
        public string ErrorCode
        {
            get { return CH.HTTPCodes.InternalServerError_500; }
        }
        /// <summary>
        /// Error Status
        /// </summary>
        public string Status
        {
            get { return this.mStatus; }
        }
        /// <summary>
        /// Error SubStatus
        /// </summary>
        public string SubStatus
        {
            get { return this.mSubStatus; }
        }
    }
}
