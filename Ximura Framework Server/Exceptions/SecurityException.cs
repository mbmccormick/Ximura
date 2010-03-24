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
﻿#region using
using System;
using System.Runtime.Remoting;
using System.Runtime.Serialization;
#endregion // using
namespace Ximura.Framework
{
    /// <summary>
    /// Summary description for Exceptions.
    /// </summary>
    public class SecurityException : XimuraException
    {
        /// <summary>
        /// Initializes a new instance of the SecurityException class.
        /// </summary>
        public SecurityException() : base() { }
        /// <summary>
        /// Initializes a new instance of the SecurityException class.
        /// </summary>
        /// <param name="message">The message that should be passed with the exception</param>
        public SecurityException(string message) : base(message) { }
        /// <summary>
        /// Initializes a new instance of the SecurityException class.
        /// </summary>
        /// <param name="message">The message that should be passed with the exception</param>
        /// <param name="ex">The base exception that should be passed through.</param>
        public SecurityException(string message, Exception ex) : base(message, ex) { }
        /// <summary>
        /// This exception is used for deserialization.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected SecurityException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
