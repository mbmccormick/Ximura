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
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Drawing;
using System.Security;
using System.Security.Cryptography;
using System.Security.Principal;

using Ximura;
using Ximura.Helper;
using Ximura.Server;
using Ximura.Command;
#endregion // using
namespace Ximura.Server
{
    /// <summary>
    /// This exception is thrown be the AgentManagers when the type passed through the agent attributes
    /// does not match the required interface or base class requirements.
    /// </summary>
    public class AgentInvalidTypeException: XimuraException
    {
		#region Constructors
		/// <summary>
		/// Initializes a new instance of the exception.
		/// </summary>
		public AgentInvalidTypeException():base(){}
		/// <summary>
        /// Initializes a new instance of the exception.
		/// </summary>
		/// <param name="message">The error message.</param>
		public AgentInvalidTypeException(string message):base(message){}
		/// <summary>
        /// Initializes a new instance of the exception.
		/// </summary>
		/// <param name="message">The error message.</param>
		/// <param name="ex">The base exception.</param>
		public AgentInvalidTypeException(string message,Exception ex):base(message,ex){}

		/// <summary>
		/// This exception is used for deserialization.
		/// </summary>
		/// <param name="info">The serialization info.</param>
		/// <param name="context">The serialization context.</param>
        protected AgentInvalidTypeException(SerializationInfo info, StreamingContext context) : base(info, context) { }
		#endregion // Constructors
    }
}
