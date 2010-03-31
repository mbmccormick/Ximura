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
#endregion // using
namespace Ximura
{
	/// <summary>
	/// This interface is used to provide security access information for
	/// specific access points around the system.
	/// </summary>
	public interface IXimuraSecurityDescriptor
	{
        /// <summary>
        /// This is the permissions bitmap for the module that implements this interface.
        /// </summary>
        long PermissionsBitmap { get;}
        /// <summary>
        /// This method returns the permission attribute collection for the class.
        /// </summary>
        XimuraComponentPermissionAttribute[] Permissions { get;}
	}
}
