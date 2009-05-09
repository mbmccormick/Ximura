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
    /// This attribute is used to define command properties for the external configuration.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class XimuraAppConfigurationExternalAttribute : System.Attribute
    {
        #region Declaration
        private ConfigurationLocation mConfigType;
        private string mConfigLocation;
        #endregion

        #region Class Constructors
        /// <summary>
        /// This attribute is used to specify where a command should get their settings from.
        /// </summary>
        /// <param name="configType">The configuration type.</param>
        /// <param name="configLocation">The configuration location.</param>
        public XimuraAppConfigurationExternalAttribute(ConfigurationLocation configType, string configLocation)
        {
            mConfigType = configType;
            mConfigLocation = configLocation;
        }
        #endregion

        #region ConfigType
        /// <summary>
        /// This is the configuration type.
        /// </summary>
        public ConfigurationLocation ConfigType
        {
            get
            {
                return mConfigType;
            }
        }
        #endregion // ConfigType
        #region ConfigLocation
        /// <summary>
        /// The configuration location.
        /// </summary>
        public string ConfigLocation
        {
            get { return mConfigLocation; }
        }
        #endregion // ConfigLocation
    }
}
