//#region Copyright
//// *******************************************************************************
//// Copyright (c) 2000-2009 Paul Stancer.
//// All rights reserved. This program and the accompanying materials
//// are made available under the terms of the Eclipse Public License v1.0
//// which accompanies this distribution, and is available at
//// http://www.eclipse.org/legal/epl-v10.html
////
//// Contributors:
////     Paul Stancer - initial implementation
//// *******************************************************************************
//#endregion
//#region using
//using System;

//using Ximura;
//
//using CH = Ximura.Common;
//#endregion // using
//namespace Ximura.Framework
//{
//    /// <summary>
//    /// This class is used by Ximura Commands to specify their config section handlers.
//    /// </summary>
//    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
//    public class AppCommandConfigurationAttribute: System.Attribute
//    {
//        #region Declarations
//        private string mConfigSHType;
//        private string mDefaultResource;
//        #endregion // Declarations
//        #region Constructors
//        /// <summary>
//        /// This constructor defines the config section handler type for the application.
//        /// </summary>
//        /// <param name="configSHType">The config section handler type.</param>
//        public AppCommandConfigurationAttribute(string configSHType):this(configSHType,null)
//        {
//        }
//        /// <summary>
//        /// This constructor defines the config section handler type, and defines a default resource that 
//        /// holds the default settings for the command.
//        /// </summary>
//        /// <param name="configSHType">The config section handler type.</param>
//        /// <param name="defaultResource">The default config section.</param>
//        public AppCommandConfigurationAttribute(string configSHType, string defaultResource)
//        {
//            this.mConfigSHType = configSHType;
//            this.mDefaultResource = defaultResource;
//        }
//        #endregion // Constructors

//        #region ConfigSHType
//        /// <summary>
//        /// The configuration section handler.
//        /// </summary>
//        public string ConfigSHType
//        {
//            get
//            {
//                return mConfigSHType;
//            }
//        }
//        #endregion // ConfigSHType
//        #region DefaultConfigResource
//        /// <summary>
//        /// The default configuration section handler source.
//        /// </summary>
//        public string DefaultConfigResource
//        {
//            get
//            {
//                return mDefaultResource;
//            }
//        }
//        #endregion // DefaultConfigResource
//    }
//}
