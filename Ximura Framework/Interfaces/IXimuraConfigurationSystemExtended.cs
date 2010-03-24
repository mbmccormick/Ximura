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
﻿#region using
using System;
using System.IO;
using System.Xml;
using System.ComponentModel;
using System.Collections.Specialized;
using System.Configuration;

using Ximura.Data;

using RH = Ximura.Reflection;
#endregion // using
namespace Ximura.Framework
{
    /// <summary>
    /// This interface adds addition fucntionality to the configuration system to allow
    /// types to be specified for specific configuration sections.
    /// </summary>
    public interface IXimuraConfigurationSystemExtended : IConfigurationSystem
    {
        /// <summary>
        /// Gets the specified configuration.
        /// </summary>
        /// <param name="configKey">The configuration key.</param>
        /// <param name="settingsType">The settings type. 
        /// The settings object must inherit from this type.</param>
        /// <returns>The object representing the configuration.</returns>
        object GetConfigExtended(string configKey, Type settingsType);
        /// <summary>
        /// Gets the specified configuration.
        /// </summary>
        /// <param name="configKey">The configuration key.</param>
        /// <param name="settingsType">The settings type. 
        /// The settings object must inherit from this type.</param>
        /// <param name="force">When this object is set to true, the settings object type 
        /// specified in the config file is ignored.</param>
        /// <returns>The object representing the configuration.</returns>
        object GetConfigExtended(string configKey, Type settingsType, bool force);
    }
}
