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
	[XimuraAppModule("EA63F664-0AE1-4345-A1F3-CD01A1805739","ExtensibleLoggingManager")]
    public class LoggingManager : 
        AppServerAgentManager<LoggerAgentBase, LoggingManagerConfiguration, LoggingManagerPerformance>, IXimuraAppServerAgentService
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

        #region InternalStart()
        /// <summary>
        /// This override adds the loggers to the collection.
        /// </summary>
        protected override void InternalStart()
        {
            base.InternalStart();

        }
        #endregion // InternalStart()
        #region InternalStop()
        /// <summary>
        /// This override removes the loggers.
        /// </summary>
        protected override void InternalStop()
        {
            //LoggingProvidersRemove();

            base.InternalStop();
        }
        #endregion // InternalStop()

		#region Provider Handling

        //public void LoggerRemove(XimuraAgentHolder agent)
        //{
        //    try
        //    {
        //        IXimuraLoggerConfigSH loggerSettings = null;// this.Settings.getLoggerSettings(loggerID);

        //        LoggerAdd(loggerType, loggerSettings);
        //    }
        //    catch (Exception ex)
        //    {

        //    }
        //}

        //private void LoggerAdd(XimuraAgentHolder agent)
        //{
        //    IXimuraLoggingProvider logprov = RH.CreateObjectFromType(loggerType) as IXimuraLoggingProvider;
        //    //If this is null then skip to the next logger or exit
        //    if (logprov != null)
        //    {
        //        //If there are settings then initialize them
        //        if (loggerSettings != null)
        //            logprov.Initialize(loggerSettings);

        //        //Add the logger to the trace collection
        //        XimuraAppTrace.LoggerAdd(logprov);

        //        //Add the logger to the internal collection. This will be used
        //        //to remove the logger when the logging manager closes
        //        loggers.Add(logprov);
        //    }
        //}

        //private void LoggingProvidersRemove()
        //{
        //    loggers.ForEach(logger =>
        //        {
        //            XimuraAppTrace.LoggerRemove(logger);
        //            logger.Deinitialize();
        //        });

        //    loggers.Clear();
        //}
		#endregion

        protected override LoggerAgentBase AgentCreate(XimuraServerAgentHolder holder)
        {
            throw new NotImplementedException();
        }
    }
}