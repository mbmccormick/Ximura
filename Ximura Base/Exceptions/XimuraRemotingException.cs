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
#endregion // using
namespace Ximura
{
	/// <summary>
	/// This class is used to signal an exception when using Ximura remoting
	/// </summary>
	[Serializable]
	public class XimuraRemotingException : RemotingException, ISerializable
	{
		/// <summary>Initializes a new instance of the SecureRemotingException class with default properties.</summary>
		public XimuraRemotingException(): base(){}
		
		/// <summary>Initializes a new instance of the SecureRemotingException class with the given message.</summary>
		/// <param name="message">The error message that explains why the exception occurred.</param>
		public XimuraRemotingException(string message): base(message){}

		/// <summary>Initializes a new instance of the SecureRemotingException class with the specified properties.</summary>
		/// <param name="message">The error message that explains why the exception occurred.</param>
		/// <param name="innerException">The exception that is the cause of the current exception.</param>
		public XimuraRemotingException(string message, System.Exception innerException): base(message, innerException){}

		/// <summary>Initializes the exception with serialized information.</summary>
		/// <param name="info">Serialization information.</param>
		/// <param name="context">Streaming context.</param>
		protected XimuraRemotingException(SerializationInfo info, StreamingContext context): base(info, context){}

		/// <summary>Provides serialization functionality.</summary>
		/// <param name="info">Serialization information.</param>
		/// <param name="context">Streaming context.</param>
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
