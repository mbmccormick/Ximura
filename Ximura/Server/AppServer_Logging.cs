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
using Ximura.Helper;
using Ximura.Server;
using Ximura.Command;

using Ximura.Performance;
using AH = Ximura.Helper.AttributeHelper;
using RH = Ximura.Helper.Reflection;
using CH = Ximura.Helper.Common;
#endregion
namespace Ximura.Server
{
    public partial class AppServer<CONFSYS, CONFCOM, PERF>
    {
        #region Logging
        /// <summary>
        /// This is the logging manager for the application.
        /// </summary>
        protected virtual LoggingManager Logging
        {
            get;
            set;
        }
        #endregion // Logging


        #region LoggingStart()
        /// <summary>
        /// This private method checks whether a Logging and Performance manager is active
        /// in the application, and if not creates a default one.
        /// </summary>
        protected virtual void LoggingStart()
        {
            ////Check whether there is a logging manager defined for the application
            //mLoggingMan = ControlServiceContainer.GetService(typeof(IXimuraLogging)) as LoggingManager;
            ////No, then create a default one.
            //if (mLoggingMan == null)
            Logging = new LoggingManager(ControlContainer);

            //Add the loggers.
            AgentsAdd<XimuraAppServerLoggerAttribute>(LoggersDefault, Logging);

            //We wait until here to start the services as they have reference to themselves.
            if (((IXimuraService)Logging).ServiceStatus != XimuraServiceStatus.Started)
                ((IXimuraService)Logging).Start();

            XimuraAppTrace.Start();

            XimuraAppTrace.WriteLine("Start request received: " + this.GetType().AssemblyQualifiedName,
                this.AppServerAttribute.AppServerName, EventLogEntryType.Information);
        }
        #endregion
        #region LoggingStop()
        /// <summary>
        /// This method is used to cleanup and stop logging.
        /// </summary>
        protected virtual void LoggingStop()
        {
            //Remove the loggers.
            AgentsRemove<XimuraAppServerLoggerAttribute>(LoggersDefault, Logging);

            //XimuraAppTrace.Close();
        }
        #endregion

        #region LoggersDefault
        /// <summary>
        /// This property returns default loggers for the application. 
        /// </summary>
        protected virtual IEnumerable<XimuraServerAgentHolder> LoggersDefault
        {
            get
            {
                yield break;
            }
        }
        #endregion
    }
}
