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
using System.Runtime.Remoting;
using System.Runtime.Serialization;
#endregion // using
namespace Ximura.Server
{
    /// <summary>
    /// This exception is used by the Dispatcher process.
    /// </summary>
    [Serializable()]
    public class DispatcherException : XimuraException
    {
        /// <summary>
        /// The default constructor
        /// </summary>
        public DispatcherException() : base() { }
        /// <summary>
        /// The default constructor.
        /// </summary>
        /// <param name="message">The error message</param>
        public DispatcherException(string message) : base(message) { }
        /// <summary>
        /// The DispatcherException.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="ex">The base exception.</param>
        public DispatcherException(string message, Exception ex) : base(message, ex) { }
        /// <summary>Initializes the exception with serialized information.</summary>
        /// <param name="info">Serialization information.</param>
        /// <param name="context">Streaming context.</param>
        protected DispatcherException(SerializationInfo info, StreamingContext context) : base(info, context) { }

    }
}
