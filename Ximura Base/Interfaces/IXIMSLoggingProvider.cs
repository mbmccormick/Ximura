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
	/// The logging provider interface is used by implemented by loggers and is used 
	/// by the logging manager to initialize logger in preperation for being inserted
	/// in the application logging collection.
	/// </summary>
	public interface IXIMSLoggingProvider: IXIMSLogging
	{
		/// <summary>
		/// This method initializes a logger
		/// </summary>
		/// <param name="settings">The logger settings.</param>
		void Initialize(IXIMSLoggerConfigSH settings);
	}
}
