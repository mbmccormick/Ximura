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

using Ximura;
using Ximura.Server;
#endregion // using
namespace Ximura
{
    /// <summary>
    /// This interface is used by the logging config section handlers
    /// </summary>
    public interface IXimuraLoggerConfigSH : IXimuraLoggerSettings, IXimuraConfigSH
    {

    }

    public interface IXimuraLoggerSettings
    {
        /// <summary>
        /// This method returns a specific switch value for the type of message.
        /// </summary>
        /// <param name="Type">The switch type.</param>
        /// <returns>An integer value with the switch value.</returns>
        int GetSwitchValue(string Type);
        /// <summary>
        /// This is the logger identifier.
        /// </summary>
        string LoggerID { get; }
        /// <summary>
        /// This is the logger public name.
        /// </summary>
        string LoggerName { get; }
        /// <summary>
        /// This is the default log level
        /// </summary>
        int LogLevel { get; }

        /// <summary>
        /// Get the config setting by specific Type
        /// </summary>
        /// <param name="Type">setting type</param>
        /// <returns>setting value</returns>
        string GetSetting(string Type);
    }
}
