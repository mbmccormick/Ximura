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
namespace Ximura
{
    /// <summary>
    /// This is the default interface for an application config section handler
    /// </summary>
    public interface IXimuraConfigSH : IConfigurationSectionHandler
    {
        /// <summary>
        /// Get the config setting by specific Type
        /// </summary>
        /// <param name="Type">setting type</param>
        /// <returns>setting value</returns>
        string GetSetting(string Type);
        /// <summary>
        /// This method returns the extended setting object or null if the type 
        /// specified does not exist.
        /// </summary>
        /// <param name="Type">The name of the type.</param>
        /// <param name="subType">The name of the subtype.</param>
        /// <returns>The settings object containing the specific settings or null if the object can not be found.</returns>
        object GetSettingExtended(string Type, string subType);
        /// <summary>
        /// This property determines whether the command is enabled.
        /// </summary>
        bool Enabled { get; }
    }
}
