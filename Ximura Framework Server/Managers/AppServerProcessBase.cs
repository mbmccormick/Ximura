#region Copyright
 //*******************************************************************************
 //Copyright (c) 2000-2009 Paul Stancer.
 //All rights reserved. This program and the accompanying materials
 //are made available under the terms of the Eclipse Public License v1.0
 //which accompanies this distribution, and is available at
 //http://www.eclipse.org/legal/epl-v10.html

 //Contributors:
 //    Paul Stancer - initial implementation
 //*******************************************************************************
#endregion
#region using
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Threading;
using System.Reflection;
using System.Security.Cryptography;
using System.Linq;

using Ximura;
using Ximura.Data;

using CH=Ximura.Common;
using Ximura.Framework;

using Ximura.Framework;

#endregion // using
namespace Ximura.Framework
{
    /// <summary>
    /// AppServerProcess is the base class for AppServer processes such as Session managers and security managers.
    /// </summary>
    /// <typeparam name="CONF">The configuration class.</typeparam>
    /// <typeparam name="PERF">The performance class.</typeparam>
    public class AppServerProcessBase<CONF, PERF> : AppBase<CONF, PERF>
        where CONF : ConfigurationBase, new()
        where PERF : PerformanceCounterCollection, new()
    {
        #region Declarations
        /// <summary>
        /// This is the server process key pair
        /// </summary>
        protected RSACryptoServiceProvider SCMRSAProvider;
        /// <summary>
        /// This is the public key
        /// </summary>
        protected RSAParameters SCMRSAPublicKey;
        #endregion
        #region Constructors
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        public AppServerProcessBase() : this((IContainer)null) { }
        /// <summary>
        /// The Ximura Application component model constructor
        /// </summary>
        /// <param name="container">The container that the services should be added to.</param>
        public AppServerProcessBase(IContainer container)
            : base(container)
        {
            CreateSecurityKeys();
        }
        #endregion

        //#region Start()
        ///// <summary>
        ///// This method starts the service based on the default async settings
        ///// </summary>
        //public override void Start()
        //{
        //    //if (this.CommandSettings == null)
        //    if (!ConfigurationStart())
        //    {
        //        XimuraAppTrace.WriteLine(CommandName + " - there are no settings for the command so it cannot start.",
        //            CommandName, EventLogEntryType.Error);
        //        return;
        //    }

        //    PerformanceStart();

        //    base.Start();
        //}
        //#endregion // Start()
        //#region Stop()
        ///// <summary>
        ///// This override removes the performance and configuration.
        ///// </summary>
        //public override void Stop()
        //{
        //    PerformanceStop();
        //    ConfigurationStop();

        //    base.Stop();
        //}
        //#endregion // Stop()


        #region CreateSecurityKeys()
        /// <summary>
        /// This method will contain the security keys for the component lifetime.
        /// </summary>
        protected virtual void CreateSecurityKeys()
        {
            SCMRSAProvider = new RSACryptoServiceProvider();
            SCMRSAPublicKey = SCMRSAProvider.ExportParameters(false);
        }
        #endregion // CreateSecurityKeys()

        #region ConfigurationManager
        /// <summary>
        /// This override gets the ConfigurationManager from the IXimuraConfigurationManager service.
        /// </summary>
        protected override IXimuraConfigurationManager ConfigurationManager
        {
            get
            {
                if (base.ConfigurationManager == null)
                    base.ConfigurationManager = GetService<IXimuraConfigurationManager>();

                return base.ConfigurationManager;
            }
            set
            {
                throw new NotSupportedException("ConfigurationManager cannot be set in the command object.");
            }
        }
        #endregion
    }
}