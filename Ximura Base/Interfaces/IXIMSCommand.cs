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

using XIMS;
using XIMS.Helper;

#endregion // using
namespace XIMS.Applications
{
	/// <summary>
    /// IXIMSCommand is the base interface for the command object.
	/// </summary>
	public interface IXIMSCommand
	{
		/// <summary>
		/// The command unique identifier.
		/// </summary>
		Guid CommandID{get;}
		/// <summary>
		/// The command name. This is used in to the config file to retrieve the
		/// settings.
		/// </summary>
		string CommandName{get;set;}
		/// <summary>
		/// The command friendly description
		/// </summary>
		string CommandDescription{get;set;}
	}
    /// <summary>
    /// This is the command interface implemented by commands.
    /// </summary>
    public interface IXIMSCommandPerformance
    {
        IXIMSPerformanceManager PerformanceManager { get;}
    }
    /// <summary>
    /// This interface determines the specific permissions that the command presents to the application.
    /// </summary>
    public interface IXIMSCommandPermissions
    {

    }
}
