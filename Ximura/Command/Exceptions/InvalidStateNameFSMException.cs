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
using System.Runtime.Remoting;
using System.Runtime.Serialization;

using Ximura.Server;

#endregion // using
namespace Ximura.Command
{
    /// <summary>
    /// Summary description for FSMException.
    /// </summary>
    public class InvalidStateNameFSMException : FSMException
    {
        /// <summary>
        /// Initializes a new instance of the XimuraException class.
        /// </summary>
        public InvalidStateNameFSMException() : base() { }
        /// <summary>
        /// Initializes a new instance of the XimuraException class.
        /// </summary>
        /// <param name="message">The error message.</param>
        public InvalidStateNameFSMException(string message) : base(message) { }
        /// <summary>
        /// Initializes a new instance of the XimuraException class.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="ex">The base exception.</param>
        public InvalidStateNameFSMException(string message, Exception ex) : base(message, ex) { }

        /// <summary>
        /// This exception is used for deserialization.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The serialization context.</param>
        protected InvalidStateNameFSMException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
