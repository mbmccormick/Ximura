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
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Runtime.Serialization;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Threading;
using System.Reflection;

using Ximura;
using Ximura.Data;
using Ximura.Helper;
using CH = Ximura.Helper.Common;
using Ximura.Server;
using Ximura.Command;
#endregion // using
namespace Ximura
{
    /// <summary>
    /// This exception is thrown when an error is encountered during the component status change event.
    /// </summary>
    public class XimuraComponentServiceException: XimuraException
    {
		/// <summary>
		/// Initializes a new instance of the XimuraException class.
		/// </summary>
		public XimuraComponentServiceException():base(){}
		/// <summary>
		/// Initializes a new instance of the XimuraException class.
		/// </summary>
		/// <param name="message">The error message.</param>
		public XimuraComponentServiceException(string message):base(message){}
		/// <summary>
		/// Initializes a new instance of the XimuraException class.
		/// </summary>
		/// <param name="message">The error message.</param>
		/// <param name="ex">The base exception.</param>
		public XimuraComponentServiceException(string message,Exception ex):base(message,ex){}

		/// <summary>
		/// This exception is used for deserialization.
		/// </summary>
		/// <param name="info">The serialization info.</param>
		/// <param name="context">The serialization context.</param>
        protected XimuraComponentServiceException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
