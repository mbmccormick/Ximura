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
	/// XimuraException is the root exception object for the Ximura system.
	/// </summary>
	[Serializable()]
	public class XimuraException : System.Exception
	{
		#region Constructors
		/// <summary>
		/// Initializes a new instance of the XimuraException class.
		/// </summary>
		public XimuraException():base(){}
		/// <summary>
		/// Initializes a new instance of the XimuraException class.
		/// </summary>
		/// <param name="message">The error message.</param>
		public XimuraException(string message):base(message){}
		/// <summary>
		/// Initializes a new instance of the XimuraException class.
		/// </summary>
		/// <param name="message">The error message.</param>
		/// <param name="ex">The base exception.</param>
		public XimuraException(string message,Exception ex):base(message,ex){}

		/// <summary>
		/// This exception is used for deserialization.
		/// </summary>
		/// <param name="info">The serialization info.</param>
		/// <param name="context">The serialization context.</param>
		protected XimuraException(SerializationInfo info, StreamingContext context) : base(info, context) {}
		#endregion // Constructors

        #region GetObjectData
        /// <summary>Provides serialization functionality.</summary>
        /// <param name="info">Serialization information.</param>
        /// <param name="context">Streaming context.</param>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
        } 
        #endregion

        #region LogException
        /// <summary>
        /// This is a shortcut to log the exception message to the Ximura Logging provider. 
        /// This may be overriden in derived classes to provide a more detailed breakdown.
        /// </summary>
        /// <param name="provider">The Ximura logging provider to log to.</param>
        /// <returns>A boolean value. True indicated that the message was successfully logged.</returns>
        public virtual bool LogException(IXimuraLogging provider)
        {
            if (provider == null) return false;

            try
            {
                provider.WriteLine(this.Message);
            }
            catch
            {
                return false;
            }

            return true;
        } 
        #endregion
	}
}
