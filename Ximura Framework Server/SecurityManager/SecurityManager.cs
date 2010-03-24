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
//#region using
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
//    /// <summary>
//    /// The SecurityManager class is the central class within the Ximura/Ximura processing system. 
//    /// The security manager is responsible for scheduling jobs, and routing requests to the specific
//    /// command. The security manager is also responsible for enforcing security checks on incoming 
//    /// requests based on a user's session profile.
//    /// </summary>
//    [XimuraAppModule("2009A5AD-9F9A-4a13-97F1-86D1D301B63D", "SecurityManager")]
//    [ToolboxBitmap(typeof(XimuraResourcePlaceholder),"Ximura.Resources.SecurityManager.bmp")]
//    //[SecurityCritical(SecurityCriticalScope.Everything)]
//    public partial class SecurityManager : AppServerProcessBase<SCMConfiguration, SCMPerformance>
//    {
//        #region Declarations
//        private bool mDisposed = false;

//        private DispatcherCommandBridge mCommandBridge;

//        private IXimuraPoolManager mPoolManager = null;


//        #endregion
//        #region Constructors
//        /// <summary>
//        /// The constructor used by the Ximura Application model.
//        /// </summary>
//        /// <param name="controls">The control container.</param>
//        /// <param name="commandContainer">The command container.</param>
//        /// <param name="poolManager">This is the system pool manager.</param>
//        public SecurityManager(IContainer controls, IContainer commandContainer, IXimuraPoolManager poolManager)
//            : base(controls)
//        {
//            mPoolManager = poolManager;
//        }
//        #endregion
//        #region Dispose / Destructor
//        /// <summary>
//        /// This is the override Dispose method.
//        /// </summary>
//        /// <param name="disposing"></param>
//        protected override void Dispose(bool disposing)
//        {
//            if (disposing)
//            {
//                lock (this)
//                {
//                    mDisposed = true;
//                    ThreadPoolShutdown();
//                }
//            }

//            base.Dispose(disposing);
//        }
//        #endregion // Dispose

//        #region ServicesProvide()
//        /// <summary>
//        /// This overrides adds the IXimuraSecurityManager service to the control container
//        /// </summary>
//        protected override void ServicesProvide()
//        {
//            base.ServicesProvide ();
//            //Add the IXimuraSecurityManager service
//        }
//        #endregion // ServicesProvide()
//        #region ServicesRemove()
//        /// <summary>
//        /// This overriden method removes the IXimuraSecurityManager service
//        /// </summary>
//        protected override void ServicesRemove()
//        {
//            base.ServicesRemove ();
//        }
//        #endregion // ServicesRemove()

//        #region InternalStart()
//        /// <summary>
//        /// This internal method starts the dispatcher.
//        /// </summary>
//        protected override void InternalStart()
//        {
//            base.InternalStart ();

//            //IXimuraPoolManager poolManager = new PoolManager() as IXimuraPoolManager;
//            JobProcessStart();

//            mCommandBridge =
//                new DispatcherCommandBridge(SessionCreateRemote, ConfigurationManager, PerformanceManager, mPoolManager);

//            //mDispatcher.Start();
//            ThreadPoolInitiate();

//            SessionManagersStart();
//        }
//        #endregion // InternalStart()
//        #region InternalStop()
//        /// <summary>
//        /// This internal method stops the service
//        /// </summary>
//        protected override void InternalStop()
//        {
//            SessionManagersStop();

//            //mDispatcher.Stop();
//            ThreadPoolShutdown();

//            JobProcessStop();

//            base.InternalStop ();
//        }
//        #endregion // InternalStop()

//    }
//}