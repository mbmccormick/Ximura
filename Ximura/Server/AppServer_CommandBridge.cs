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
        #region Declarations
        private CommandBridge mCommandBridge;
        #endregion // Declarations

        #region CommandBridgeStart()
        /// <summary>
        /// This method register the command bridge with the command collection.
        /// </summary>
        protected virtual void CommandBridgeStart()
        {
            mCommandBridge = new CommandBridge(SessionCreateRemote, 
                Configuration, PerformanceService.Manager, PoolManager);

            AddService<IXimuraConfigurationManager>(CommandBridge);
            AddService<IXimuraPerformanceManager>(CommandBridge);
            AddService<IXimuraPoolManager>(CommandBridge);
            AddService<IXimuraCommandBridge>(CommandBridge);
            AddService<IXimuraSessionManager>(CommandBridge);
        }
        #endregion
        #region CommandBridgeStop()
        /// <summary>
        /// This method removes the command bridge services
        /// </summary>
        protected virtual void CommandBridgeStop()
        {
            RemoveService<IXimuraSessionManager>();
            RemoveService<IXimuraCommandBridge>();
            RemoveService<IXimuraPoolManager>();
            RemoveService<IXimuraPerformanceManagerService>();
            RemoveService<IXimuraConfigurationManager>();
        }
        #endregion

        #region CommandBridge
        /// <summary>
        /// The is the command bridge which links the security manager with the command collection.
        /// </summary>
        protected virtual CommandBridge CommandBridge
        {
            get { return mCommandBridge; }
        }
        #endregion // CmdBridge
        #region CommandBridgeServiceAdd
        /// <summary>
        /// This method adds a service to the command bridge
        /// </summary>
        /// <param name="service">The service type</param>
        /// <param name="serviceobj">The service object.</param>
        /// <returns>Returns true is the service was added successfully.</returns>
        protected bool CommandBridgeServiceAdd(Type service, object serviceobj)
        {
            return CommandBridge.AddContainerService(service, serviceobj);
        }

        protected bool CommandBridgeServiceAdd<T>(T serviceobj)
        {
            return CommandBridge.AddContainerService(typeof(T), serviceobj);
        }

        #endregion // AddCommandBridgeService
        #region CommandBridgeServiceGet
        /// <summary>
        /// This method returns a command bridge service.
        /// </summary>
        /// <param name="service">The service type.</param>
        /// <returns>The service object, or null is the service is not present.</returns>
        protected object CommandBridgeServiceGet(Type service)
        {
            // TODO:  Add SecurityManager.GetCommandBridgeService implementation
            return CommandBridge.GetContainerService(service);
        }

        protected T CommandBridgeServiceGet<T>()
        {
            // TODO:  Add SecurityManager.GetCommandBridgeService implementation
            return (T)CommandBridge.GetContainerService(typeof(T));
        }
        #endregion // GetCommandBridgeService
        #region CommandBridgeServiceRemove
        /// <summary>
        /// This method removes a service from the command bridge.
        /// </summary>
        /// <param name="service">The service type to remove.</param>
        /// <returns>Returns true if the service was removed successfully.</returns>
        public bool CommandBridgeServiceRemove(Type service)
        {
            return CommandBridge.RemoveContainerService(service);
        }

        public bool CommandBridgeServiceRemove<T>()
        {
            return CommandBridge.RemoveContainerService(typeof(T));
        }
        #endregion // RemoveCommandBridgeService
    }
}
