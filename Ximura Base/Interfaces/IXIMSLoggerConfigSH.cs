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
	/// This interface is used by the logging config section handlers
	/// </summary>
	public interface IXIMSLoggerConfigSH: IXIMSConfigSH
	{
		/// <summary>
		/// This method returns a specific switch value for the type of message.
		/// </summary>
		/// <param name="Type">The switch type.</param>
		/// <returns>An integer value with the switch value.</returns>
		int GetSwitchValue(string Type);
	}
}
