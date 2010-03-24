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
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.Remoting;
using System.Runtime.Serialization;

using Ximura;
using Ximura.Data;
using Ximura.Helper;
using CH = Ximura.Helper.Common;
using Ximura.Framework;


#endregion // using
namespace Ximura.Data
{
    /// <summary>
    /// ContentDataStoreException is used to signal an exception in the CDS or its related components.
    /// </summary>
    [Serializable()]
    public class CDSStateException : ContentDataStoreException
    {
        /// <summary>
        /// Initializes a new instance of the XimuraException class.
        /// </summary>
        public CDSStateException() : base() { }
        /// <summary>
        /// Initializes a new instance of the XimuraException class.
        /// </summary>
        /// <param name="message">The error message.</param>
        public CDSStateException(string message) : base(message) { }
        /// <summary>
        /// Initializes a new instance of the XimuraException class.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="ex">The base exception.</param>
        public CDSStateException(string message, Exception ex) : base(message, ex) { }
        /// <summary>
        /// This exception is used for deserialization.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The serialization context.</param>
        protected CDSStateException(SerializationInfo info, StreamingContext context) : base(info, context) { }

        #region ResponseCode
        /// <summary>
        /// This is the HTTP Response code.
        /// </summary>
        public override int ResponseCode { get { return 500; } }
        #endregion // ResponseCode
    }

}