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
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Reflection;
using System.Drawing;
using System.Diagnostics;
using System.Collections.Generic;
using System.Runtime.Remoting;
using System.Runtime.Serialization;

using Ximura;

using Ximura.Framework;
using Ximura.Framework;

#endregion // using
namespace Ximura.Communication
{
    /// <summary>
    /// This base exception class allows exceptions to be thrown with an additional Status and Substatus message.
    /// </summary>
    public class ResponseStatusException: XimuraException
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="status"></param>
        /// <param name="subStatus"></param>
        public ResponseStatusException(string message, string status, string subStatus)
            :base(message)
        {
            Status = status;
            SubStatus = subStatus;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="status"></param>
        /// <param name="subStatus"></param>
        /// <param name="ex"></param>
        public ResponseStatusException(string message, string status, string subStatus, Exception ex):
            base(message, ex)
        {
            Status = status;
            SubStatus = subStatus;
        }

		/// <summary>
		/// This exception is used for deserialization.
		/// </summary>
		/// <param name="info">The serialization info.</param>
		/// <param name="context">The serialization context.</param>
        protected ResponseStatusException(SerializationInfo info, StreamingContext context) : base(info, context) { }

        /// <summary>
        /// This is the error status.
        /// </summary>
        public string Status{get; private set;}
        /// <summary>
        /// This is the error substatus.
        /// </summary>
        public string SubStatus{get; private set;}
    }
}
