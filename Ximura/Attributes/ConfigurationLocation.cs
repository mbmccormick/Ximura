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

using Ximura;
using Ximura.Helper;
#endregion // using
namespace Ximura
{
    /// <summary>
    /// This enumeration specifies the action the command should take to load the configuration.
    /// </summary>
    public enum ConfigurationLocation : int
    {
        /// <summary>
        /// The command uses the legacy config setting handler system.
        /// </summary>
        None,
        /// <summary>
        /// The configuration file is a resource embedded in the application.
        /// </summary>
        Resource,
        /// <summary>
        /// The configuration should be retrieved from the configuration manager.
        /// </summary>
        ConfigurationManager,
        /// <summary>
        /// The configuration file is a resource embedded by the system, but should be updated using the settings class.
        /// </summary>
        Hybrid
    }
}
