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
using System.Collections.Specialized;
using System.Runtime.Serialization;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Threading;
using System.Reflection;
#endregion // using
namespace Ximura.Helper
{
	/// <summary>
	/// XimuraException is the root exception object for the Ximura system.
	/// </summary>
	[Serializable()]
	public class XimuraServiceException: XimuraException 
	{
		/// <summary>
		/// Initializes a new instance of the XimuraException class.
		/// </summary>
		public XimuraServiceException():base(){}
		/// <summary>
		/// Initializes a new instance of the XimuraException class.
		/// </summary>
		/// <param name="message"></param>
		public XimuraServiceException(string message):base(message){}
		/// <summary>
		/// Initializes a new instance of the XimuraException class.
		/// </summary>
		/// <param name="message"></param>
		/// <param name="ex"></param>
		public XimuraServiceException(string message,Exception ex):base(message,ex){}
        /// <summary>
        /// Initializes a protected instance of the XimuraException class for remoting.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The serialization context.</param>
		protected XimuraServiceException(SerializationInfo info, StreamingContext context) : base(info, context) {}
	}
}
