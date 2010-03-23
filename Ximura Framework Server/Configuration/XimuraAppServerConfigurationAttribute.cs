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

using Ximura;
using Ximura.Helper;

using CH = Ximura.Helper.Common;
using RH = Ximura.Helper.Reflection;
#endregion // using
namespace Ximura.Server
{
    #region XimuraAppServerConfigSystemAttribute
    /// <summary>
    /// This attribute contains the system configuration settings.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public sealed class XimuraAppServerConfigSystemAttribute : XimuraAppServerConfigurationAttribute
    {
        #region Constructors
        /// <summary>
        /// This constructor contains the necessary settings to set the configuration.
        /// </summary>
        /// <param name="ConfigOptions">The configuration options.</param>
        /// <param name="ConfigName">The configuration name.</param>
        public XimuraAppServerConfigSystemAttribute(AppServerConfigOptions ConfigOptions, string ConfigName) : base(ConfigOptions, ConfigName) { }
        #endregion
    }
    #endregion // XimuraAppServerConfigurationSystemAttribute

    #region XimuraAppServerConfigCommandAttribute
    /// <summary>
    /// This attribute contains the command configuration settings.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public sealed class XimuraAppServerConfigCommandAttribute : XimuraAppServerConfigurationAttribute
    {
        #region Constructors
        /// <summary>
        /// This constructor contains the necessary settings to set the configuration.
        /// </summary>
        /// <param name="ConfigOptions">The configuration options.</param>
        /// <param name="ConfigName">The configuration name.</param>
        public XimuraAppServerConfigCommandAttribute(AppServerConfigOptions ConfigOptions, string ConfigName) : base(ConfigOptions, ConfigName) { }
        #endregion
    }
    #endregion // XimuraAppServerConfigurationCommandAttribute

    #region XimuraAppServerConfigurationAttribute
    /// <summary>
    /// This is the abstract base attribute used to hold the configuration settings.
    /// </summary>
    public abstract class XimuraAppServerConfigurationAttribute : Attribute
    {
        #region Declarations
        private AppServerConfigOptions m_ConfigOpt = AppServerConfigOptions.NoConfig;
        private string m_ConfigName;
        #endregion

        #region Constructors
        /// <summary>
        /// This constructor contains the necessary settings to set the configuration.
        /// </summary>
        /// <param name="ConfigOptions">The configuration options.</param>
        /// <param name="ConfigName">The configuration name.</param>
        public XimuraAppServerConfigurationAttribute(AppServerConfigOptions ConfigOptions, string ConfigName)
        {
            m_ConfigOpt = ConfigOptions;
            m_ConfigName = ConfigName;
        }

        #endregion

        #region Properties
        /// <summary>
        /// This property returns the Application configuration options.
        /// </summary>
        public AppServerConfigOptions ConfigOptions { get { return m_ConfigOpt; } }

        /// <summary>
        /// This is the location of the config file.
        /// </summary>
        public string ConfigLocation { get { return m_ConfigName; } }
        #endregion
    }
    #endregion // XimuraAppServerConfigurationAttribute
}
