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
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.Remoting;
using System.Runtime.Serialization;

using Ximura;
using Ximura.Data;
using Ximura.Helper;
using CH=Ximura.Helper.Common;
using Ximura.Server;


#endregion // using
namespace Ximura.Persistence
{
	/// <summary>
	/// ContentDataStoreException is used to signal an exception in the CDS or its related components.
	/// </summary>
	[Serializable()]
	public class ContentDataStoreException: XimuraException
	{
		/// <summary>
		/// Initializes a new instance of the XimuraException class.
		/// </summary>
		public ContentDataStoreException():base(){}
		/// <summary>
		/// Initializes a new instance of the XimuraException class.
		/// </summary>
		/// <param name="message">The error message.</param>
		public ContentDataStoreException(string message):base(message){}
		/// <summary>
		/// Initializes a new instance of the XimuraException class.
		/// </summary>
		/// <param name="message">The error message.</param>
		/// <param name="ex">The base exception.</param>
		public ContentDataStoreException(string message,Exception ex):base(message,ex){}
		/// <summary>
		/// This exception is used for deserialization.
		/// </summary>
		/// <param name="info">The serialization info.</param>
		/// <param name="context">The serialization context.</param>
		protected ContentDataStoreException(SerializationInfo info, StreamingContext context) : base(info, context){}

        #region ResponseCode
        /// <summary>
        /// This is the HTTP Response code.
        /// </summary>
        public virtual int ResponseCode { get { return 400; } }
        #endregion // ResponseCode
	}
}