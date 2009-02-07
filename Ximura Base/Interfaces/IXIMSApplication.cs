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
using System.Configuration;
#endregion // using
namespace XIMS.Applications
{
	/// <summary>
	/// Summary description for IXIMSApplication.
	/// </summary>
	public interface IXIMSApplication
	{
		/// <summary>
		/// This is the application ID.
		/// </summary>
		Guid ApplicationID {get;}
	}

    /// <summary>
    /// This interface is used to provide application based configuration settings.
    /// </summary>
    public interface IXIMSApplicationDefinition : IXIMSApplication
    {
        /// <summary>
        /// This is the application name.
        /// </summary>
        string ApplicationName { get;}


    }
}
