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
using System.Collections;
using System.Configuration;
using System.Collections.Generic;
using System.IO;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;

using Ximura;
using Ximura.Data;
using Ximura.Helper;
using CH = Ximura.Helper.Common;
using RH = Ximura.Helper.Reflection;

using Ximura.Server;


#endregion // using
namespace Ximura.Command
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="RQ"></typeparam>
    /// <typeparam name="RS"></typeparam>
    /// <typeparam name="CBRQ"></typeparam>
    /// <typeparam name="CBRS"></typeparam>
    /// <typeparam name="CONF"></typeparam>
    /// <typeparam name="PERF"></typeparam>
    public class AppCommandContainer<RQ, RS, CBRQ, CBRS, CONF, PERF> : AppCommandContainer<RQ, RS, CBRQ, CBRS, CONF, PERF, CONF>
        where RQ : RQRSFolder, new() //Request
        where RS : RQRSFolder, new() //Response
        where CBRQ : RQRSFolder, new() //Callback Request
        where CBRS : RQRSFolder, new() //Callback Response
        where CONF : CommandConfiguration, new() //External Configuration
        where PERF : CommandPerformance, new() //Command Performance
    {
        #region Constructor
        /// <summary>
        /// This is the empty constructor
        /// </summary>
        public AppCommandContainer() : this((IContainer)null) { }
        /// <summary>
        /// This is the base constructor for a Ximura command
        /// </summary>
        /// <param name="container">The command container to be added to.</param>
        public AppCommandContainer(IContainer container) : base(container) { }
        /// <summary>
        /// This is the base constructor for a Ximura command
        /// </summary>
        /// <param name="commandID">This is the explicitly set command id, leave this as null if you want to use the default id.</param>
        /// <param name="container">The container to be added to</param>
        public AppCommandContainer(Guid? commandID, IContainer container) : base(commandID, container) { }
        #endregion
    }
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="RQ"></typeparam>
    /// <typeparam name="RS"></typeparam>
    /// <typeparam name="CBRQ"></typeparam>
    /// <typeparam name="CBRS"></typeparam>
    /// <typeparam name="CONF"></typeparam>
    /// <typeparam name="PERF"></typeparam>
    /// <typeparam name="INTCONF"></typeparam>
    public class AppCommandContainer<RQ, RS, CBRQ, CBRS, CONF, PERF, INTCONF> : AppCommandProcess<RQ, RS, CBRQ, CBRS, CONF, PERF>
        where RQ : RQRSFolder, new() //Request
        where RS : RQRSFolder, new() //Response
        where CBRQ : RQRSFolder, new() //Callback Request
        where CBRS : RQRSFolder, new() //Callback Response
        where CONF : CommandConfiguration, new() //External Configuration which contains the properties for the container command.
        where INTCONF : CommandConfiguration, new() //Internal Configuration which contains the settings for the internal commands
        where PERF : CommandPerformance, new() //Command Performance
    {
        #region Constructor
        /// <summary>
        /// This is the empty constructor
        /// </summary>
        public AppCommandContainer() : this((IContainer)null) { }
        /// <summary>
        /// This is the base constructor for a Ximura command
        /// </summary>
        /// <param name="container">The command container to be added to.</param>
        public AppCommandContainer(IContainer container) :
            base(container) { }
        /// <summary>
        /// This is the base constructor for a Ximura command
        /// </summary>
        /// <param name="commandID">This is the explicitly set command id, leave this as null if you want to use the default id.</param>
        /// <param name="container">The container to be added to</param>
        public AppCommandContainer(Guid? commandID, IContainer container) : base(commandID, container) { }
        #endregion

        #region ConfigurationInternal
        /// <summary>
        /// This is the internal configuration that contains the settings for the internal commands.
        /// </summary>
        protected virtual INTCONF ConfigurationInternal
        {
            get;
            private set;
        }
        #endregion // ConfigurationInternal
    }
}
