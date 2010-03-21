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
using System.Configuration;
#endregion // using
namespace Ximura.Server
{
	/// <summary>
	/// This enumeration is used by the security manager to identify the authorization
	/// level of a session. 
	/// </summary>
	public enum SessionProfileLevel
	{
		/// <summary>
		/// The session is unathorized and will only have the default security rights
		/// </summary>
		Unauthorized,
		/// <summary>
		/// The session is authorized based on the user ID.
		/// </summary>
		Authorized
	}
}
