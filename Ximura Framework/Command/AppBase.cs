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
using Ximura.Command;
using Ximura.Helper;
using Ximura.Server;

using CH = Ximura.Helper.Common;
using RH = Ximura.Helper.Reflection;

#endregion // using
namespace Ximura
{
    /// <summary>
    /// This is the base class for both command and application objects.
    /// </summary>
    /// <typeparam name="CONF">The configuration class.</typeparam>
    /// <typeparam name="PERF">The performance class.</typeparam>
    public class AppBase<CONF, PERF> : XimuraComponentService
        where CONF : ConfigurationBase, new()
        where PERF : PerformanceBase, new()
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
        /// <summary>
        /// This is the AppServerCommandDataExtender used to set the command start priority.
        /// </summary>
        protected CommandMetadataExtender CommandExtender = null;
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
        #region InitializeCommandExtender()
        /// <summary>
        /// This method initializes the 
        /// </summary>
        protected virtual void CommandExtenderInitialize()
        {
            CommandExtender = new CommandMetadataExtender(components);
        }
        #endregion // InitializeCommandMetaDataExtender()

        #region Configuration
        /// <summary>
        /// This is the command configuration.
        /// </summary>
        protected virtual CONF Configuration
        {
            get;
            set;
        }
        #endregion // CommandConfig
        #region ConfigurationStart()
        /// <summary>
        /// This method creates and registers the configuration data object.
        /// </summary>
        protected virtual bool ConfigurationStart()
        {
            Configuration = new CONF();

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
        protected virtual PERF Performance
        {
            get;
            set;
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
        #region PerformanceStart()
        /// <summary>
        /// This method starts the performance counters.
        /// </summary>
        protected virtual void PerformanceStart()
        {
            Performance = new PERF();
        }
        #endregion // PerformanceStart()
        #region PerformanceStop()
        /// <summary>
        /// This method stops the performance counters.
        /// </summary>
        protected virtual void PerformanceStop()
        {
            Performance = null;
        }
        #endregion // PerformanceStop()

        #region PoolManager
        /// <summary>
        /// This is the pool manager.
        /// </summary>
        protected virtual IXimuraPoolManager PoolManager
        {
            get;
            set;
        }
        #endregion // PerformanceCounter
        #region PoolManagerStart()
        /// <summary>
        /// This protected method creates the default pool manager for the application.
        /// </summary>
        protected virtual void PoolManagerStart()
        {
            PoolManager = null;
        }
        #endregion // PoolManagerStart()
        #region PoolManagerStop()
        /// <summary>
        /// This protected method disposes of the default pool manager for the application.
        /// </summary>
        protected virtual void PoolManagerStop()
        {
            PoolManager = null;
        }
        #endregion // PoolManagerStop()

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

        #region CommandsStart()
        /// <summary>
        /// This protected method initializes the commands
        /// </summary>
        protected virtual void CommandsStart()
        {
            //Start any components or commands that do not have a priority set.
            ComponentsStatusChange(XimuraServiceStatusAction.Start, ServiceComponents);
            //OK, start the remaining commands based on their priority settings.
            CommandExtender.StartCommandsInOrder();
        }
        #endregion
        #region CommandsStop()
        /// <summary>
        /// This protected method initializes the commands
        /// </summary>
        protected virtual void CommandsStop()
        {
            //Stop the commands based on their reverse priority.
            CommandExtender.StopCommandsInReverseOrder();
            //Ok, stop any remaining commands or service components.
            ComponentsStatusChange(XimuraServiceStatusAction.Stop, ServiceComponents);
        }
        #endregion
        #region ComponentsStatusBeforeChange()
        /// <summary>
        /// This overriden method checks on start whether the command has a priority set, 
        /// if it does has a priority set, the command is not started.
        /// </summary>
        /// <param name="action">The service action.</param>
        /// <param name="service">The service.</param>
        /// <returns>Returns true if the service can start.</returns>
        protected override bool ComponentsStatusBeforeChange(XimuraServiceStatusAction action, IXimuraService service)
        {
            //We only want to start commands that do not have a priority.
            if (action == XimuraServiceStatusAction.Start &&
                CommandExtender.CommandHasPriority(service))
                return false;

            return base.ComponentsStatusBeforeChange(action, service);
        }
        #endregion
    }
}
