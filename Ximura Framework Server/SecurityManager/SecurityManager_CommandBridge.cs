//#region Copyright
//// *******************************************************************************
//// Copyright (c) 2000-2009 Paul Stancer.
//// All rights reserved. This program and the accompanying materials
//// are made available under the terms of the Eclipse Public License v1.0
//// which accompanies this distribution, and is available at
//// http://www.eclipse.org/legal/epl-v10.html
////
//// Contributors:
////     Paul Stancer - initial implementation
//// *******************************************************************************
//#endregion
//﻿#region using
//using System;
//using System.ComponentModel;
//using System.Linq;
//using System.Collections;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.Drawing;
//using System.Threading;
//using System.Security;
//using System.Security.Cryptography;
//using System.Security.Principal;
//using System.Security.Permissions;

//using Ximura;
//
//using Ximura.Framework;

//using Ximura.Framework;
//#endregion // using
//namespace Ximura.Framework
//{
//    public partial class SecurityManager
//    {
//        #region CommandBridge
//        /// <summary>
//        /// The is the command bridge which links the security manager with the command collection.
//        /// </summary>
//        internal DispatcherCommandBridge CommandBridge
//        {
//            get { return mCommandBridge; }
//        }
//        #endregion // CmdBridge
//        #region CommandBridgeServiceAdd
//        /// <summary>
//        /// This method adds a service to the command bridge
//        /// </summary>
//        /// <param name="service">The service type</param>
//        /// <param name="serviceobj">The service object.</param>
//        /// <returns>Returns true is the service was added successfully.</returns>
//        public bool CommandBridgeServiceAdd(Type service, object serviceobj)
//        {
//            return CommandBridge.AddContainerService(service, serviceobj);
//        }

//        public bool CommandBridgeServiceAdd<T>(T serviceobj)
//        {
//            return CommandBridge.AddContainerService(typeof(T), serviceobj);
//        }

//        #endregion // AddCommandBridgeService
//        #region CommandBridgeServiceGet
//        /// <summary>
//        /// This method returns a command bridge service.
//        /// </summary>
//        /// <param name="service">The service type.</param>
//        /// <returns>The service object, or null is the service is not present.</returns>
//        public object CommandBridgeServiceGet(Type service)
//        {
//            // TODO:  Add SecurityManager.GetCommandBridgeService implementation
//            return CommandBridge.GetContainerService(service);
//        }

//        public T CommandBridgeServiceGet<T>()
//        {
//            // TODO:  Add SecurityManager.GetCommandBridgeService implementation
//            return (T)CommandBridge.GetContainerService(typeof(T));
//        }
//        #endregion // GetCommandBridgeService
//        #region CommandBridgeServiceRemove
//        /// <summary>
//        /// This method removes a service from the command bridge.
//        /// </summary>
//        /// <param name="service">The service type to remove.</param>
//        /// <returns>Returns true if the service was removed successfully.</returns>
//        public bool CommandBridgeServiceRemove(Type service)
//        {
//            return CommandBridge.RemoveContainerService(service);
//        }

//        public bool CommandBridgeServiceRemove<T>()
//        {
//            return CommandBridge.RemoveContainerService(typeof(T));
//        }
//        #endregion // RemoveCommandBridgeService
//    }
//}
