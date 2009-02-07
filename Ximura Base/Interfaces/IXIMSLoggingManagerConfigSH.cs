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
using System.Diagnostics;
using System.Configuration;

using XIMS;
using XIMS.Applications;
#endregion // using
namespace XIMS.Applications.Logging
{
	/// <summary>
	/// This interface is used by the logging config section handler.
	/// </summary>
	public interface IXIMSLoggingManagerConfigSH : IXIMSConfigSH
	{
		/// <summary>
		/// This method returns an ArrayList containing a collection of strings with the name of the 
		/// logging providers.
		/// </summary>
		/// <returns>An ArrayList containing a collection of strings.</returns>
		ArrayList getLoggers();
		/// <summary>
		/// This method returns the specific logger type.
		/// </summary>
		/// <param name="provider">The provider.</param>
		/// <returns>An ArrayList containing the list of loggers.</returns>
		string getLoggerType(string provider);
		/// <summary>
		/// This method returns the specific logger settings object.
		/// </summary>
		/// <param name="provider">The provider to return the settings for.</param>
		/// <returns>The logging object.</returns>
		IXIMSLoggerConfigSH getLoggerSettings(string provider);
	}
}
