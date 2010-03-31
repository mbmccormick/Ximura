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
using System.Globalization;
using System.Security.Cryptography;

using Ximura;
using Ximura.Data;
using Ximura.Framework;

#endregion // using
namespace Ximura
{
	/// <summary>
	/// The XimuraSessionState enumeration identifies the current state of a session.
	/// </summary>
	public enum SessionState
	{
        /// <summary>
        /// The session is undefined.
        /// </summary>
        Undefined,
		/// <summary>
		/// The session is not initialized.
		/// </summary>
		NotInitialized=0,
		/// <summary>
		/// The session is initialized.
		/// </summary>
		Initialized,
		/// <summary>
		/// The session is currently authenticating.
		/// </summary>
		Authenticating,
		/// <summary>
		/// The session is authenticated with the security manager.
		/// </summary>
		Authenticated,
		/// <summary>
		/// The session is closing.
		/// </summary>
		Closing,
		/// <summary>
		/// The session is closed.
		/// </summary>
		Closed,
		/// <summary>
		/// The session is not authorized.
		/// </summary>
		NotAuthorized,
		/// <summary>
		/// The session was rejected by the security manager.
		/// </summary>
		Rejected
	}
}
