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
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Drawing;
using System.Threading;
using System.Reflection;
using System.Linq;
using System.IO;
using System.IO.Compression;
using System.Xml;
using System.Security.Cryptography;

using Ximura;
using Ximura.Data;

using Ximura.Framework;
using Ximura.Framework;
using AH = Ximura.AttributeHelper;
using RH = Ximura.Reflection;
using CH = Ximura.Common;
#endregion
namespace Ximura.Framework
{
    public partial class AppServer<CONFSYS, CONFCOM, PERF>
    {
        #region InternalStart()
        /// <summary>
        /// This method is called by the ServiceProcess parent object.
        /// </summary>
        /// <exception cref="Ximura.Framework.AppServerException">An exception is thrown detailing 
        /// the error encountered when starting the application.</exception>
        protected override void InternalStart()
        {
            try
            {
                //Load and verify the config files - if no settings can be found throw an error and exit
                if (!ConfigurationStart())
                    throw new AppServerException("Configuration error - the configuration cannot be loaded.");

                //This method creates the default pool manager for the application.
                PoolManagerStart();

                //Start the logging manager.
                LoggingStart();

                //Start the Performance managers.
                PerformanceStart();

                //This method starts the agent services such as the storage and session managers.
                AgentServicesStart();

                //Create the session manager
                SessionManagerStart();

                //Start the Security components
                JobProcessStart();

                ThreadPoolStart();

                //This method register the Security Manager Command bridge object with the dispatcher.
                CommandBridgeStart();

                //Connect the components to the messaging architecture.
                ConnectComponents();

                //Start the commands and provide a thread for them to run on.
                CommandsStart();

                CommandExtender.CommandsNotify(typeof(XimuraServiceStatus), XimuraServiceStatus.Started);
            }
            catch (AuthenticationException aex)
            {
                throw aex;
            }
            catch (Exception ex)
            {
                XimuraAppTrace.WriteLine("Start request failed. Application will now stop: "
                    + Environment.NewLine + Environment.NewLine + ex.Message,
                    this.AppServerAttribute.AppServerName, EventLogEntryType.Error);
                //Catch the exception and stop app services that have already started
                InternalStop();
                //Throw the original exception so that it can be caught lower down.
                throw ex;
            }
            XimuraAppTrace.WriteLine("Start request completed successfully.",
                this.AppServerAttribute.AppServerName, EventLogEntryType.Information);
        }
        #endregion
        #region InternalStop()
        /// <summary>
        /// This method stops the AppServer
        /// </summary>
        protected override void InternalStop()
        {
            XimuraAppTrace.WriteLine("Stop request received.",
                this.AppServerAttribute.AppServerName);

            CommandExtender.CommandsNotify(typeof(XimuraServiceStatus), XimuraServiceStatus.Stopping);

            Exception aex = null;

            try
            {
                //Close all the commands
                CommandsStop();
                //ComponentsStatusChange(XimuraServiceStatusAction.Stop, ServiceComponents);		
            }
            catch (Exception ex)
            {
                XimuraAppTrace.WriteLine("Stop request failed - component collection: " + Environment.NewLine + ex.Message,
                    this.AppServerAttribute.AppServerName);
                aex = ex;
            }

            try
            {
                //Stop the rest of the control collection
                ComponentsStatusChange(XimuraServiceStatusAction.Stop, ControlContainer.Components);
            }
            catch (Exception ex)
            {
                XimuraAppTrace.WriteLine("Stop request failed - control collection: " + Environment.NewLine + ex.Message,
                    this.AppServerAttribute.AppServerName);
                if (aex == null)
                    aex = ex;
            }

            try
            {
                //This method removes the Security Manager Command bridge object with the dispatcher.
                CommandBridgeStop();

                ThreadPoolStop();
                JobProcessStop();

                //Create the session manager
                SessionManagerStop();

                AgentServicesStop();

                PerformanceStop();

                LoggingStop();

                PoolManagerStop();
            }
            catch (Exception ex)
            {
                XimuraAppTrace.WriteLine("Stop request failed - environment cleanup: " + Environment.NewLine + ex.Message,
                    this.AppServerAttribute.AppServerName);
                if (aex == null)
                    aex = ex;
            }

            if (aex != null)
                throw aex;
        }
        #endregion

        #region InternalPause()
        /// <summary>
        /// This method pauses the AppServer
        /// </summary>
        protected override void InternalPause()
        {
            CommandExtender.CommandsNotify(typeof(XimuraServiceStatus), XimuraServiceStatus.Paused);
            XimuraAppTrace.WriteLine("Pause request received but not implemented. No action was taken.",
                this.AppServerAttribute.AppServerName);
        }
        #endregion
        #region InternalContinue()
        /// <summary>
        /// This method continues the AppServer after it has been paused
        /// </summary>
        protected override void InternalContinue()
        {
            CommandExtender.CommandsNotify(typeof(XimuraServiceStatus), XimuraServiceStatus.Resumed);
            XimuraAppTrace.WriteLine("Continue request received but not implemented. No action was taken.",
                this.AppServerAttribute.AppServerName);
        }
        #endregion
    }
}
