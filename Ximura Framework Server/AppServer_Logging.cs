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
        #region Logging
        /// <summary>
        /// This is the logging manager for the application.
        /// </summary>
        protected virtual LoggingManager LoggingService
        {
            get;
            set;
        }
        #endregion // Logging

        #region LoggingCreate()
        /// <summary>
        /// This method creates the logging manager.
        /// </summary>
        protected virtual void LoggingCreate()
        {
            LoggingService = new LoggingManager(ControlContainer);

            //Add the loggers.
            AgentsAdd<XimuraAppServerLoggerAttribute>(LoggersDefault, LoggingService);
        }
        #endregion // LoggingCreate()
        #region LoggingDispose()
        /// <summary>
        /// This method disposes the logging manager and closes the XimuraAppTrace static object.
        /// </summary>
        protected virtual void LoggingDispose()
        {
            XimuraAppTrace.Close();

            LoggingService.Dispose();
            LoggingService = null;
        }
        #endregion // LoggingDispose()

        #region LoggingStart()
        /// <summary>
        /// This private method checks whether a Logging and Performance manager is active
        /// in the application, and if not creates a default one.
        /// </summary>
        protected virtual void LoggingStart()
        {
            //We wait until here to start the services as they have reference to themselves.
            if (((IXimuraService)LoggingService).ServiceStatus != XimuraServiceStatus.Started)
                ((IXimuraService)LoggingService).Start();

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
            ((IXimuraService)LoggingService).Stop();
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
