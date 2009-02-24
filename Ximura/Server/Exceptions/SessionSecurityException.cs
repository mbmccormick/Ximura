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
    /// Summary description for SessionExceptions.
    /// </summary>
    public class SessionSecurityException : XimuraException
    {
        public SessionSecurityException() : base() { }
        public SessionSecurityException(string message) : base(message) { }
        public SessionSecurityException(string message, Exception ex) : base(message, ex) { }
        protected SessionSecurityException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
