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
using CH=Ximura.Helper.Common;
#endregion // using
namespace Ximura.Server
{
	/// <summary>
	/// This is the abstract Security Manager Base Exception.
	/// </summary>
	public abstract class SCMBaseException : XimuraException
	{
		#region Constructors
		/// <summary>
		/// Initializes a new instance of the LogonException class.
		/// </summary>
		public SCMBaseException():base(){}
		/// <summary>
		/// Initializes a new instance of the LogonException class.
		/// </summary>
		/// <param name="message">The message that should be passed with the exception</param>
		public SCMBaseException(string message):base(message){}
		/// <summary>
		/// Initializes a new instance of the LogonException class.
		/// </summary>
		/// <param name="message">The message that should be passed with the exception</param>
		/// <param name="ex">The base exception that should be passed through.</param>
		public SCMBaseException(string message,Exception ex):base(message,ex){}
		/// <summary>
		/// This exception is used for deserialization.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		protected SCMBaseException(SerializationInfo info, StreamingContext context) : base(info, context) {}
		#endregion // Constructors

		#region ResponseCode
		/// <summary>
		/// This is the HTTP Response code.
		/// </summary>
		public abstract string ResponseCode{get;}
		#endregion // ResponseCode
	}
}
