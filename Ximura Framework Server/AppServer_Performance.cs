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
        #region PerformanceService
        /// <summary>
        /// This is the performance manager for the application.
        /// </summary>
        protected virtual PerformanceManager PerformanceService
        {
            get;
            set;
        }
        #endregion // Logging

        #region PerformanceManager
        /// <summary>
        /// This is the performance manager for the application which commands and services can register their performance counters with,
        /// </summary>
        protected override IXimuraPerformanceManager PerformanceManager
        {
            get
            {
                return PerformanceService.Manager;
            }
            set
            {
                throw new NotSupportedException("PerformanceManager cannot be set in the base application server.");
            }
        }
        #endregion // PerformanceManager

        #region PerformanceCreate()
        /// <summary>
        /// This method creates the performance manager and add the performance agents.
        /// </summary>
        protected virtual void PerformanceCreate()
        {
            PerformanceService = new PerformanceManager(ControlContainer);

            AgentsAdd<XimuraAppServerPerformanceAgentAttribute>(PerformanceAgentsDefault, PerformanceService);
        }
        #endregion // PerformanceCreate()
        #region PerformanceDispose()
        /// <summary>
        /// This method removes the performance agents and disposes of the performance manager.
        /// </summary>
        protected virtual void PerformanceDispose()
        {
            PerformanceService = null;
        }
        #endregion // PerformanceDispose()
        
        #region PerformanceStart()
        /// <summary>
        /// This protected method checks whether a Logging and Performance manager is active
        /// in the application, and if not creates a default one.
        /// </summary>
        protected override void PerformanceStart()
        {
            base.PerformanceStart();

            PerformanceService.Start();

            //Performance.CommandID = this.ApplicationID;
            //Performance.PCID = this.ApplicationID;
            //Performance.Name = this.ApplicationIDAttribute.;
            //Performance.Category = "Command";

            PerformanceManager.PerformanceCounterCollectionRegister(Performance);
        }
        #endregion
        #region PerformanceStop()
        /// <summary>
        /// This method stops and performance related logging.
        /// </summary>
        protected override void PerformanceStop()
        {
            PerformanceService.Stop();

            base.PerformanceStop();
        }
        #endregion

        #region PerformanceAgentsDefault
        /// <summary>
        /// This property returns default performance agents for the application.
        /// </summary>
        protected virtual IEnumerable<XimuraServerAgentHolder> PerformanceAgentsDefault
        {
            get
            {
                yield break;
            }
        }
        #endregion
    }
}
