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
namespace Ximura.Communication
{
    /// <summary>
    /// The CommAppServer class is a base server class that provides communication server functionality.
    /// </summary>
    public class CommAppServer : CommAppServer<AppServerSystemConfiguration, AppServerCommandConfiguration, AppServerPerformance>
    {
        #region Constructors
        /// <summary>
        /// This is the default constructor for the service.
        /// </summary>
        public CommAppServer()
            : this((IContainer)null)
        {
        }
        /// <summary>
        /// This constructor is called by the .Net component model when adding it to a container
        /// </summary>
        /// <param name="container">The container to add the component to.</param>
        public CommAppServer(IContainer container)
            : base(container)
        {
        }
        #endregion
    }

    /// <summary>
    /// The CommAppServer class is a base server class that provides communication server functionality.
    /// </summary>
    /// <typeparam name="CONFSYS">The server system configuration.</typeparam>
    /// <typeparam name="CONFCOM">The server command configuration.</typeparam>
    /// <typeparam name="PERF">The performance settings for the server.</typeparam>
    public partial class CommAppServer<CONFSYS, CONFCOM, PERF> : AppServer<CONFSYS, CONFCOM, PERF>
        where CONFSYS : AppServerSystemConfiguration, new()
        where CONFCOM : AppServerCommandConfiguration, new()
        where PERF : AppServerPerformance, new()
    {
        #region Containers/Service containers
        private System.ComponentModel.IContainer components = null;
        /// <summary>
        /// This is the internet server.
        /// </summary>
        protected InternetServiceCommand internetServer;
        /// <summary>
        /// This is the content data store for the application.
        /// </summary>
        protected InternetServerCDSCommand internetServerCDS;
        #endregion // Containers
        #region Constructors
        /// <summary>
        /// This is the default constructor for the service.
        /// </summary>
        public CommAppServer() : this((IContainer)null) { }
        /// <summary>
        /// This constructor is called by the .Net component model when adding it to a container
        /// </summary>
        /// <param name="container">The container to add the component to.</param>
        public CommAppServer(IContainer container)
            : base(container)
        {
            //Set the default value for the domain status.
            //mSeperateDomain = AppServerAttribute.DomainRequired;

            InitializeComponents();
            RegisterContainer(components);
        }
        #endregion
        #region InitializeComponents()
        private void InitializeComponents()
        {
            components = new System.ComponentModel.Container();

            internetServer = new InternetServiceCommand(components);
            internetServerCDS = new InternetServerCDSCommand(components);

            ((System.ComponentModel.ISupportInitialize)(CommandExtender)).BeginInit();

            //
            // internetServer
            //
            this.internetServer.CommandName = "InternetServer";
            CommandExtender.SetPriority(this.internetServer, 3);
            // 
            // internetServerCDS
            // 
            this.internetServerCDS.CommandName = "InternetServerCDS";
            CommandExtender.SetPriority(this.internetServerCDS, 10);

            ((System.ComponentModel.ISupportInitialize)(CommandExtender)).EndInit();
        }
        #endregion // InitializeComponents()
    }
}
