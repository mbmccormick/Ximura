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

        #region GatewayCreate()
        /// <summary>
        /// This method creates the Gateway service and creates and adds the Gateway agents to the service.
        /// </summary>
        public virtual void GatewayCreate()
        {
            GatewayService = new GatewayManager(ControlContainer);

            //Add the storage agents.
            AgentsAdd<XimuraAppServerGatewayAttribute>(GatewayDefault, GatewayService);
        }
        #endregion // GatewayCreate()
        #region GatewayDispose()
        /// <summary>
        /// This method disposes of the Gateway service.
        /// </summary>
        public virtual void GatewayDispose()
        {
            StorageService.Dispose();
        }
        #endregion // GatewayDispose()

        #region GatewayStart()
        /// <summary>
        /// This method starts the Gateway Manager
        /// </summary>
        protected virtual void GatewayStart()
        {
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
            ((IXimuraService)StorageService).Stop();
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
