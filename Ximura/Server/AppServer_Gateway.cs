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
using AH = Ximura.Helper.AttributeHelper;
using RH = Ximura.Helper.Reflection;
using CH = Ximura.Helper.Common;
#endregion
namespace Ximura.Server
{
    public partial class AppServer<CONFSYS, CONFCOM, PERF>
    {
        #region GatewayService
        /// <summary>
        /// This is the gateway manager for the application.
        /// </summary>
        protected virtual GatewayManager GatewayService
        {
            get;
            set;
        }
        #endregion // Storage

        #region GatewayStart()
        /// <summary>
        /// This method starts the Gateway Manager
        /// </summary>
        protected virtual void GatewayStart()
        {
            StorageService = new StorageManager(ControlContainer);

            //Add the storage agents.
            AgentsAdd<XimuraAppServerGatewayAttribute>(StorageDefault, StorageService);

            //We wait until here to start the services as they have reference to themselves.
            if (((IXimuraService)StorageService).ServiceStatus != XimuraServiceStatus.Started)
                ((IXimuraService)StorageService).Start();
        }
        #endregion
        #region GatewayStop()
        /// <summary>
        /// This method stops the Gateway Manager.
        /// </summary>
        protected virtual void GatewayStop()
        {
            //Remove the loggers.
            AgentsRemove<XimuraAppServerGatewayAttribute>(GatewayDefault, GatewayService);

        }
        #endregion

        #region GatewayDefault
        /// <summary>
        /// This property returns default agents for the gateway manager. 
        /// </summary>
        protected virtual IEnumerable<XimuraServerAgentHolder> GatewayDefault
        {
            get
            {
                yield break;
            }
        }
        #endregion
    }
}
