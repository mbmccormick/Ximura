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
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.Serialization;

using Ximura;
using Ximura.Helper;
#endregion // using
namespace Ximura
{
    /// <summary>
    /// This exception is thrown when the pool is not able to return an object.
    /// </summary>
    public class PoolObjectUnavailableException : XimuraException
    {
        /// <summary>
        /// Initializes a new instance of the exception class.
        /// </summary>
        public PoolObjectUnavailableException() : base() { }
        /// <summary>
        /// Initializes a new instance of the exception class.
        /// </summary>
        /// <param name="message">The error message.</param>
        public PoolObjectUnavailableException(string message) : base(message) { }
        /// <summary>
        /// Initializes a new instance of the exception class.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="ex">The base exception.</param>
        public PoolObjectUnavailableException(string message, Exception ex) : base(message, ex) { }
        /// <summary>
        /// This exception is used for deserialization.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The serialization context.</param>
        protected PoolObjectUnavailableException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
