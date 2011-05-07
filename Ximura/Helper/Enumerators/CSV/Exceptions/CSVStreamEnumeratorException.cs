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
using Ximura.Data;
#endregion
namespace Ximura
{
    /// <summary>
    /// This is the base exception class for the CSV enumerator.
    /// </summary>
    public class CSVStreamEnumeratorException: XimuraException
    {
        /// <summary>
        /// Initializes a new instance of the XimuraException class.
        /// </summary>
        public CSVStreamEnumeratorException() : base() { }
        /// <summary>
        /// Initializes a new instance of the XimuraException class.
        /// </summary>
        /// <param name="message">The error message.</param>
        public CSVStreamEnumeratorException(string message) : base(message) { }
        /// <summary>
        /// Initializes a new instance of the XimuraException class.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="ex">The base exception.</param>
        public CSVStreamEnumeratorException(string message, Exception ex) : base(message, ex) { }

#if (!SILVERLIGHT)
        /// <summary>
        /// This exception is used for deserialization.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The serialization context.</param>
        protected CSVStreamEnumeratorException(SerializationInfo info, StreamingContext context) : base(info, context) { }
#endif
    }
}
