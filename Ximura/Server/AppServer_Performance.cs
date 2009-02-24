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
        #region PerformanceStart()
        /// <summary>
        /// This protected method checks whether a Logging and Performance manager is active
        /// in the application, and if not creates a default one.
        /// </summary>
        protected override void PerformanceStart()
        {
            base.PerformanceStart();

            AgentsAdd<XimuraAppServerPerformanceAgentAttribute>(PerformanceAgentsDefault, (IXimuraPerformanceService)PerformanceManager);

        }
        #endregion
        #region PerformanceStop()
        /// <summary>
        /// This method stops and performance related logging.
        /// </summary>
        protected override void PerformanceStop()
        {
            AgentsRemove<XimuraAppServerPerformanceAgentAttribute>(PerformanceAgentsDefault, (IXimuraPerformanceService)PerformanceManager);

            base.PerformanceStop();
        }
        #endregion
        #region PerformanceManager
        /// <summary>
        /// This is the performance manager for the application.
        /// </summary>
        protected override IXimuraPerformanceManager PerformanceManager
        {
            get
            {
                return Performance;
            }
            set
            {
                throw new NotSupportedException("PerformanceManager cannot be set in the base application server.");
            }
        }
        #endregion // PerformanceManager

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
