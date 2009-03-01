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
        #region Class -> AppServerDefinition
        /// <summary>
        /// This class copies application specific data from the application and makes it available 
        /// to any requesting parties without any security risk of having the base object directly accessible.
        /// </summary>
        private class AppServerDefinition : IXimuraApplicationDefinition
        {
            #region Constructor
            /// <summary>
            /// This is the public constructor that copies the required values from the application.
            /// </summary>
            internal AppServerDefinition(XimuraApplicationIDAttribute attrID, XimuraAppServerAttribute attrServ)
            {
                ApplicationID = attrID.ApplicationID;
                ApplicationName = attrServ.AppServerName;
                ApplicationDescription = attrServ.AppDescription;
            }
            #endregion // Constructor

            #region IXimuraApplicationDefinition Members
            /// <summary>
            /// This is the application name.
            /// </summary>
            public string ApplicationName
            {
                get;
                private set;
            }
            /// <summary>
            /// This is the application description.
            /// </summary>
            public string ApplicationDescription
            {
                get;
                private set;
            }

            #endregion

            #region IXimuraApplication Members
            /// <summary>
            /// This is the application ID.
            /// </summary>
            public Guid ApplicationID
            {
                get;
                private set;
            }

            #endregion
        }
        #endregion // AppServerDefinition

        #region ApplicationServicesStart()
        /// <summary>
        /// This method registers specific application information.
        /// </summary>
        protected virtual void ApplicationServicesStart()
        {
            mApplicationDefinition = new AppServerDefinition(this.ApplicationIDAttribute, this.AppServerAttribute);

            //Register the base application definition parameters.
            AddService<IXimuraApplicationDefinition>(mApplicationDefinition, true);

            ControlServiceContainer.AddService(typeof(IXimuraApplicationDefinition),mApplicationDefinition);
        }
        #endregion
        #region ApplicationServicesStop()
        /// <summary>
        /// This method removes specific application information.
        /// </summary>
        protected virtual void ApplicationServicesStop()
        {
            ControlServiceContainer.RemoveService(typeof(IXimuraApplicationDefinition));

            //Remove the base application definition parameters.
            RemoveService<IXimuraApplicationDefinition>();
        }
        #endregion

        #region PoolManagerStart()
        /// <summary>
        /// This protected method creates the default pool manager for the application.
        /// </summary>
        protected override void PoolManagerStart()
        {
            PoolManager = new PoolManager(true);
        }
        #endregion // PoolManagerStart()
        #region PoolManagerStop()
        /// <summary>
        /// This protected method disposes of the default pool manager for the application.
        /// </summary>
        protected override void PoolManagerStop()
        {
            PoolManager.Dispose();
            PoolManager = null;
        }
        #endregion // PoolManagerStop()

        #region AgentServicesStart()
        /// <summary>
        /// This method starts the storage and session managers. You can override this method to add additional service managers.
        /// </summary>
        protected virtual void AgentServicesStart()
        {
            //Start the storage next, as the session managers may depend on the storage engine.
            StorageStart();
        }
        #endregion // AgentServicesStart()
        #region AgentServicesStop()
        /// <summary>
        /// This method stops the storage and session managers. You can override this method to add additional service managers.
        /// </summary>
        protected virtual void AgentServicesStop()
        {
            //Start the storage next, as the session managers may depend on the storage engine.
            StorageStop();
        }
        #endregion // AgentServicesStop()
    }
}
