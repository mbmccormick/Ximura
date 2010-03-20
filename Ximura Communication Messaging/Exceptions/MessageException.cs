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
using System.IO;
using System.Runtime.Serialization;
using System.ComponentModel;
using System.Collections;
using System.Text;

using Ximura;
using Ximura.Helper;
using Ximura.Data;
using CH = Ximura.Helper.Common;
#endregion
namespace Ximura.Data
{
    /// <summary>
    /// This is the base exception for the Message class.
    /// </summary>
    public class MessageException : XimuraException
    {
        /// <summary>
        /// Initializes a new instance of the XimuraException class.
        /// </summary>
        public MessageException() : base() { }
        /// <summary>
        /// Initializes a new instance of the XimuraException class.
        /// </summary>
        /// <param name="message">The error message.</param>
        public MessageException(string message) : base(message) { }
        /// <summary>
        /// Initializes a new instance of the XimuraException class.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="ex">The base exception.</param>
        public MessageException(string message, Exception ex) : base(message, ex) { }
        /// <summary>
        /// This exception is used for deserialization.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The serialization context.</param>
        protected MessageException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
