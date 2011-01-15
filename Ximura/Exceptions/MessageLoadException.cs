﻿#region Copyright
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

using Ximura.Data;
using CH = Ximura.Common;
#endregion
namespace Ximura
{
    /// <summary>
    /// This exception is thrown should the message exceed the maximum permitted size.
    /// </summary>
    public class MessageLoadException : MessageException
    {
        /// <summary>
        /// Initializes a new instance of the XimuraException class.
        /// </summary>
        public MessageLoadException() : base() { }
        /// <summary>
        /// Initializes a new instance of the XimuraException class.
        /// </summary>
        /// <param name="message">The error message.</param>
        public MessageLoadException(string message) : base(message) { }
        /// <summary>
        /// Initializes a new instance of the XimuraException class.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="ex">The base exception.</param>
        public MessageLoadException(string message, Exception ex) : base(message, ex) { }
#if (!SILVERLIGHT)
        /// <summary>
        /// This exception is used for deserialization.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The serialization context.</param>
        protected MessageLoadException(SerializationInfo info, StreamingContext context) : base(info, context) { }
#endif
    }
}