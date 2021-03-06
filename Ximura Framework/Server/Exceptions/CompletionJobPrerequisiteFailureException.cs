﻿#region Copyright
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
namespace Ximura.Server
{
    /// <summary>
    /// This exception is thrown by the completion job when an earlier dependency ID has thrown as exception.
    /// </summary>
    public class CompletionJobPrerequisiteFailureException : JobException
    {
        /// <summary>
		/// Initializes a new instance of the LogonException class.
		/// </summary>
		public CompletionJobPrerequisiteFailureException():base(){}
		/// <summary>
		/// Initializes a new instance of the LogonException class.
		/// </summary>
		/// <param name="message">The message that should be passed with the exception</param>
		public CompletionJobPrerequisiteFailureException(string message):base(message){}
		/// <summary>
		/// Initializes a new instance of the LogonException class.
		/// </summary>
		/// <param name="message">The message that should be passed with the exception</param>
		/// <param name="ex">The base exception that should be passed through.</param>
		public CompletionJobPrerequisiteFailureException(string message,Exception ex):base(message,ex){}
		/// <summary>
		/// This exception is used for deserialization.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
        protected CompletionJobPrerequisiteFailureException(SerializationInfo info, StreamingContext context) : base(info, context) { }

		/// <summary>
		/// The 401 HTTP response code.
		/// </summary>
		public override string ResponseCode
		{
			get
			{
                return CH.HTTPCodes.PreconditionFailed_412;
			}
		}
    }

    /// <summary>
    /// This class is used by a parent module to signal to a completion job that the job returned has failed
    /// and that the completion job should cancel all current and future child jobs, depending on the settings
    /// passed in this exception class.
    /// </summary>
    public class CompletionJobDependencyException : XimuraException
    {
        /// <summary>
		/// Initializes a new instance of the LogonException class.
		/// </summary>
		public CompletionJobDependencyException():base(){}
		/// <summary>
		/// Initializes a new instance of the LogonException class.
		/// </summary>
		/// <param name="message">The message that should be passed with the exception</param>
		public CompletionJobDependencyException(string message):base(message){}
		/// <summary>
		/// Initializes a new instance of the LogonException class.
		/// </summary>
		/// <param name="message">The message that should be passed with the exception</param>
		/// <param name="ex">The base exception that should be passed through.</param>
		public CompletionJobDependencyException(string message,Exception ex):base(message,ex){}
		/// <summary>
		/// This exception is used for deserialization.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
        protected CompletionJobDependencyException(SerializationInfo info, StreamingContext context) : base(info, context) { }

    }
}
