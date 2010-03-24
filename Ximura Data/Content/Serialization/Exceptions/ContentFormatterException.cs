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

using Ximura;
using Ximura.Helper;
using CH = Ximura.Helper.Common;
#endregion // using
namespace Ximura.Data
{
    /// <summary>
    /// The ContentFormatterException is used by the Content Formatter when an exception is encountered.
    /// </summary>
    public class ContentFormatterException : XimuraException
    {
        /// <summary>
        /// Initializes a new instance of the ISOMessageException class.
        /// </summary>
        public ContentFormatterException() : base() { }
        /// <summary>
        /// Initializes a new instance of the ISOMessageException class.
        /// </summary>
        /// <param name="message"></param>
        public ContentFormatterException(string message) : base(message) { }
        /// <summary>
        /// Initializes a new instance of the ISOMessageException class.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="ex"></param>
        public ContentFormatterException(string message, Exception ex) : base(message, ex) { }
        /// <summary>
        /// This is the serialized constructor.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The context.</param>
        protected ContentFormatterException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
