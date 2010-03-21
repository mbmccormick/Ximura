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
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;

using Ximura;
using Ximura.Command;
using Ximura.Server;
using Ximura.Data;
using Ximura.Helper;

using RH=Ximura.Helper.Reflection;
#endregion // using
namespace Ximura.Server
{
	/// <summary>
	/// The LoggingManager object creates the set of loggers for the application
	/// as specified in the configuration file.
	/// </summary>
    [XimuraAppModule("ED2902A1-1773-4b3f-B290-49E3A470AD7E", "LoggingManager")]
    public class LoggingManager :
        AppServerAgentManager<LoggerAgentBase, LoggingManagerConfiguration, LoggingManagerPerformance>, IXimuraLoggingManagerService
	{
		#region Declarations
        private XimuraAppServerLoggerAttribute[] mAppServerLoggers = null;
		private IXimuraLoggingManagerConfigSH mSettings = null;
		private IXimuraLogging internalLogging = NullLoggerAgent.NoLog();
		#endregion
		#region Constructors / Destructor
		/// <summary>
		/// The default Ximura Application constructor
		/// </summary>
		/// <param name="container">The container the services should be added to.</param>
        public LoggingManager(System.ComponentModel.IContainer container) : base(container) 
        {
        }
		#endregion

        #region ServicesProvide/ServicesRemove
        /// <summary>
        /// This override adds the IXimuraLoggingManager service to the control container.
        /// </summary>
        protected override void ServicesProvide()
        {
            base.ServicesProvide();

            AddService<IXimuraLoggingManagerService>(this);
        }
        /// <summary>
        /// This override removes the IXimuraLoggingManager service to the control container.
        /// </summary>
        protected override void ServicesRemove()
        {
            RemoveService<IXimuraLoggingManagerService>();

            base.ServicesRemove();
        }
        #endregion // ServicesProvide/ServicesRemove

        #region AgentCreate(XimuraServerAgentHolder holder)
        /// <summary>
        /// This method creates the logging agent.
        /// </summary>
        /// <param name="holder">The agent holder.</param>
        /// <returns>Returns the logging agent.</returns>
        protected override LoggerAgentBase AgentCreate(XimuraServerAgentHolder holder)
        {
            return (LoggerAgentBase)RH.CreateObjectFromType(holder.AgentType);
        }
        #endregion // AgentCreate(XimuraServerAgentHolder holder)
    }
}