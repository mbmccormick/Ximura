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
    /// Summary description for SecurityException.
    /// </summary>
    [Serializable()]
    public class DispatcherSecurityException : XimuraException
    {
        public DispatcherSecurityException() : base() { }
        public DispatcherSecurityException(string message) : base(message) { }
        public DispatcherSecurityException(string message, Exception ex) : base(message, ex) { }
        /// <summary>Initializes the exception with serialized information.</summary>
        /// <param name="info">Serialization information.</param>
        /// <param name="context">Streaming context.</param>
        protected DispatcherSecurityException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
