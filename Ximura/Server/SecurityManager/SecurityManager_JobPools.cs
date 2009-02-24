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
//using Ximura.Helper;
//using Ximura.Server;

//using Ximura.Command;
//#endregion // using
//namespace Ximura.Server
//{
//    public partial class SecurityManager
//    {
//        #region Declarations
//        /// <summary>
//        /// The active job collection.
//        /// </summary>
//        private JobCollection mJobActiveCollection = null;

//        /// <summary>
//        /// The job pool.
//        /// </summary>
//        protected PoolInvocator<Job> mPoolJob = null;

//        protected PoolInvocator<SecurityManagerJob> mPoolSecurityManagerJob = null;
//        protected PoolInvocator<CompletionJob> mPoolCompletionJob = null;

//        #endregion // Declarations

//        #region JobProcessStart()/JobProcessStop()

//        /// <summary>
//        /// This method creates the job pools for the security manager.
//        /// </summary>
//        protected virtual void JobProcessStart()
//        {
//            mJobActiveCollection =
//                new JobCollection(Configuration.JobMax, Configuration.JobCapacityOverrideLevel, false);

//            mPoolJob = new PoolInvocator<Job>(delegate() { return new Job(); });
//        }

//        /// <summary>
//        /// This method clears and disposes of the job pools for the security manager.
//        /// </summary>
//        protected virtual void JobProcessStop()
//        {
//            mPoolJob.Dispose();
//            mPoolJob = null;
//            mJobActiveCollection = null;
//        }

//        #endregion // JobProcessStart()

//    }
//}
