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
using System.Xml;
using System.Configuration;

using Ximura.Framework;
#endregion // using
namespace Ximura.Framework
{
	/// <summary>
	/// This interface is the base interface all security manager config 
	/// section handlers must implement.
	/// </summary>	
	internal interface IXimuraSecurityManagerConfigSH: IXimuraConfigSH
	{
		/// <summary>
		/// Get Permission of specific User with cid/level pairs of permission
		/// </summary>
		/// <param name="UserID">user id</param>
		/// <returns>hashtable of cid/level pairs</returns>
		Hashtable GetPermission(string UserID);
		/// <summary>
		/// Get User Setting of specific User and Type
		/// </summary>
		/// <param name="UserID">user name</param>
		/// <param name="cid">setting type</param>
		/// <returns>setting value</returns>
		string GetPermissionLevel(string UserID, string cid);
		/// <summary>
		/// This method is used to verify the job permission.
		/// </summary>
		/// <param name="scmJob">The job to verify.</param>
		/// <param name="userID">The user id.</param>
		void VerifyJobPermissions(SecurityManagerJob scmJob, string userID);
        ///// <summary>
        ///// This property returns the dispatcher settings.
        ///// </summary>
        //DispatcherConfigSH DispatcherSettings{get;}
	}
}
