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
using System.Collections;
using System.Configuration;
using System.Collections.Generic;
using System.IO;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;

using Ximura;
using Ximura.Data;
using Ximura.Framework;

using Ximura.Framework;

using CH = Ximura.Common;
using RH = Ximura.Reflection;
#endregion // using
namespace Ximura.Framework
{
    #region AppBase<CONF, PERF>
    /// <summary>
    /// This is the base class for both command and application objects.
    /// </summary>
    /// <typeparam name="CONF">The configuration class.</typeparam>
    /// <typeparam name="PERF">The performance class.</typeparam>
    public class AppBase<CONF, PERF> : AppBase<CONF, PERF, CONF>
        where CONF : ConfigurationBase, new()
        where PERF : PerformanceCounterCollection, new()
    {
        #region Constructor
        /// <summary>
        /// This is the default constructor
        /// </summary>
        public AppBase() : this(null) { }
        /// <summary>
        /// This is the base constructor for a Ximura command
        /// </summary>
        /// <param name="container">The container to be added to</param>
        public AppBase(System.ComponentModel.IContainer container)
            : base(container)
        {
        }
        #endregion
    }
    #endregion



    #region AppBase<CONF, PERF, EXTCONF>
    /// <summary>
    /// This is the base class for both command and application objects.
    /// </summary>
    /// <typeparam name="CONF">The configuration class.</typeparam>
    /// <typeparam name="PERF">The performance class.</typeparam>
    /// <typeparam name="EXTCONF">The external configuration class.</typeparam>
    public class AppBase<CONF, PERF, EXTCONF> : FrameworkComponentBase
        where CONF : ConfigurationBase, new()
        where PERF : PerformanceCounterCollection, new()
        where EXTCONF : ConfigurationBase, new() //External Configuration which contains the settings for the internal commands
    {
        #region Declarations
        private IContainer components = null;
        /// <summary>
        /// This is the performance manager.
        /// </summary>
        private IXimuraPerformanceManager mPerformanceManager = null;
        /// <summary>
        /// This is the configuration manager.
        /// </summary>
        private IXimuraConfigurationManager mConfigurationManager = null;
        /// <summary>
        /// This private method contains the unique application identifiers.
        /// </summary>
        protected IXimuraApplicationDefinition mApplicationDefinition = null;
        #endregion
		#region Constructor
		/// <summary>
		/// This is the default constructor
		/// </summary>
		public AppBase():this(null){}
		/// <summary>
		/// This is the base constructor for a Ximura command
		/// </summary>
		/// <param name="container">The container to be added to</param>
        public AppBase(System.ComponentModel.IContainer container) : base(container) 
        {
            PerformanceCreate();
            InitializeComponents();
            CommandExtenderInitialize();
            RegisterContainer(components);
        }
        #endregion

        #region InitializeComponents()
        private void InitializeComponents()
        {
            components = new System.ComponentModel.Container();
        }
        #endregion // InitializeComponents()

        #region InternalStart()/InternalStop()
        /// <summary>
        /// This method starts the AppServerProcess and registers any services
        /// </summary>
        protected override void InternalStart()
        {
            ServicesReference();
        }
        /// <summary>
        /// The method stops the AppServerProcess and unregisters and services
        /// </summary>
        protected override void InternalStop()
        {
            ServicesDereference();
        }
        #endregion

        #region Configuration
        /// <summary>
        /// This is the command configuration.
        /// </summary>
        protected virtual CONF Configuration
        {
            get;
            set;
        }
        #endregion
        #region ConfigurationExternal
        /// <summary>
        /// This is the external configuration that contains the settings for the main configuration.
        /// </summary>
        protected virtual EXTCONF ConfigurationExternal
        {
            get;
            set;
        }
        #endregion

        #region ConfigurationStart()
        /// <summary>
        /// This method creates and registers the configuration data object.
        /// </summary>
        protected virtual bool ConfigurationStart()
        {
            Configuration = new CONF();
            ConfigurationExternal = null;

            return ConfigurationLoad(Configuration);
        }
        #endregion // ConfigurationGet()
        #region ConfigurationLoad
        /// <summary>
        /// This method loads the command configuration.
        /// </summary>
        /// <param name="commandConfiguration"></param>
        protected virtual bool ConfigurationLoad(CONF commandConfiguration)
        {
            return true;
        }
        #endregion // commandConfiguration
        #region ConfigurationStop()
        /// <summary>
        /// This method unregisters and destpys the configuration data object.
        /// </summary>
        protected virtual void ConfigurationStop()
        {
            Configuration = null;
            ConfigurationExternal = null;
        }
        #endregion
        #region ConfigurationManager
        /// <summary>
        /// This is the application configuration manager for the command.
        /// </summary>
        protected virtual IXimuraConfigurationManager ConfigurationManager
        {
            get
            {
                return mConfigurationManager;
            }
            set
            {
                mConfigurationManager = value;
            }
        }
        #endregion

        #region Performance
        /// <summary>
        /// This is the performance counter for the command.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual PERF Performance
        {
            get;
            protected set;
        }
        #endregion // PerformanceCounter
        #region PerformanceManager
        /// <summary>
        /// This is the application performance manager for the command.
        /// </summary>
        protected virtual IXimuraPerformanceManager PerformanceManager
        {
            get
            {
                return mPerformanceManager;
            }
            set
            {
                mPerformanceManager = value;
            }
        }
        #endregion

        #region PerformanceCreate()/PerformanceInitialize(PERF perf)
        /// <summary>
        /// This method creates the performance object and calls the extended PerformanceInitialize method before setting 
        /// the component performance property object.
        /// </summary>
        protected virtual void PerformanceCreate()
        {
            PERF perf = new PERF();
            PerformanceInitialize(perf);
            Performance = perf;
        }
        /// <summary>
        /// This method should be used to set the specific performance properties before the 
        /// performance object is registered with the component.
        /// </summary>
        /// <param name="perf">The performance object.</param>
        protected virtual void PerformanceInitialize(PERF perf)
        {

        }
        #endregion // PerformanceInitialize
        #region PerformanceDispose()
        /// <summary>
        /// This method removes all references to the performance object.
        /// </summary>
        protected virtual void PerformanceDispose()
        {
            PERF perf = Performance;
            Performance = null;
        }
        #endregion // PerformanceDispose()

        #region PerformanceStart()
        /// <summary>
        /// This method starts the performance counters.
        /// </summary>
        protected virtual void PerformanceStart()
        {
        }
        #endregion // PerformanceStart()
        #region PerformanceStop()
        /// <summary>
        /// This method stops the performance counters.
        /// </summary>
        protected virtual void PerformanceStop()
        {
        }
        #endregion // PerformanceStop()

        #region ApplicationDefinition
        /// <summary>
        /// This private method contains the unique application identifiers.
        /// </summary>
        protected virtual IXimuraApplicationDefinition ApplicationDefinition
        {
            get
            {
                return mApplicationDefinition;
            }
        }
        #endregion // ApplicationDefinition
        #region EnvelopeHelper
        /// <summary>
        /// This property comtains the envelope helper for the framework object.
        /// </summary>
        protected virtual IXimuraEnvelopeHelper EnvelopeHelper
        {
            get;
            set;
        }
        #endregion // EnvelopeHelper

    }
    #endregion
}
