#region using
using System;
using System.Runtime.Serialization;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

using Ximura;
using Ximura.Data;
using Ximura.Helper;
using CH = Ximura.Helper.Common;
using CDS = Ximura.Data.CDSHelper;
using Ximura.Data;
using Ximura.Framework;
using Ximura.Framework;
using Ximura.Communication;
#endregion // using
namespace Ximura.Communication
{
    /// <summary>
    /// The internet server command provides the base functionality for internet based servers.
    /// </summary>
    public class InternetServiceCommand : InternetServiceCommand<InternetServerConfiguration>
    {
        #region Constructors
        /// <summary>
        /// This is the empty constructor
        /// </summary>
        public InternetServiceCommand()
            : this((IContainer)null) { }
        /// <summary>
        /// This is the constrcutor used by the Ximura Application model.
        /// </summary>
        /// <param name="container">The command container.</param>
        public InternetServiceCommand(System.ComponentModel.IContainer container)
            : base(container)
        {
        }
        /// <summary>
        /// This is the base constructor for a Ximura command
        /// </summary>
        /// <param name="commandID">This is the explicitly set command id, leave this as null if you want to use the default id.</param>
        /// <param name="container">The container to be added to</param>
        public InternetServiceCommand(Guid? commandID, System.ComponentModel.IContainer container)
            : base(commandID, container)
        {
        }
        #endregion
    }

    /// <summary>
    /// The internet server command provides the base functionality for internet based servers.
    /// </summary>
    /// <typeparam name="EXTCONF">This class contains the specific configuration parameters for the InternetServiceCommand</typeparam>
    public class InternetServiceCommand<EXTCONF>
        : AppCommandProcess<RQRSFolder, RQRSFolder, RQRSFolder, RQRSFolder, InternetServerConfiguration, CommandPerformance, EXTCONF>
         where EXTCONF : InternetServerConfiguration, new() 
    {
        #region Declarations
        //HashSet<int> hello;
        private System.ComponentModel.IContainer components = null;
        /// <summary>
        /// TCP/IP Protocol
        /// </summary>
        protected TransportCommandTCPIP transportTCPIP;
        /// <summary>
        /// UDP Protocol
        /// </summary>
        protected TransportCommandUDP transportUDP;
        /// <summary>
        /// Site Server
        /// </summary>
        protected SiteServerCommand siteServer;
        /// <summary>
        /// The Site Logger command
        /// </summary>
        protected SiteControllerLoggerCommand siteControllerLogger;
        /// <summary>
        /// Content Compiler
        /// </summary>
        protected ContentCompilerCommand contentCompiler;
        /// <summary>
        /// Site Manager
        /// </summary>
        protected SiteControllerCommand siteController;
        /// <summary>
        /// Resource Manager
        /// </summary>
        protected ResourceManagerCommand resourceManager;        
        #endregion
		#region Constructors
		/// <summary>
		/// This is the empty constructor
		/// </summary>
		public InternetServiceCommand()
            : this((IContainer)null) { }
		/// <summary>
		/// This is the constrcutor used by the Ximura Application model.
		/// </summary>
		/// <param name="container">The command container.</param>
        public InternetServiceCommand(System.ComponentModel.IContainer container)
            : base(container) 
        {
            InitializeComponents();
            RegisterContainer(components);
        }
        /// <summary>
        /// This is the base constructor for a Ximura command
        /// </summary>
        /// <param name="commandID">This is the explicitly set command id, leave this as null if you want to use the default id.</param>
        /// <param name="container">The container to be added to</param>
        public InternetServiceCommand(Guid? commandID, System.ComponentModel.IContainer container) 
            : base(commandID, container) 
        {
            InitializeComponents();
            RegisterContainer(components);
        }
        #endregion
        #region InitializeComponents()
        private void InitializeComponents()
        {
            components = new System.ComponentModel.Container();

            this.transportTCPIP = new TransportCommandTCPIP(components);
            this.transportUDP = new TransportCommandUDP(components);
            this.siteServer = new SiteServerCommand(components);
            this.siteControllerLogger = new SiteControllerLoggerCommand(components);
            this.contentCompiler = new ContentCompilerCommand(components);
            this.siteController = new SiteControllerCommand(components);
            this.resourceManager = new ResourceManagerCommand(components);

            ((System.ComponentModel.ISupportInitialize)(CommandExtender)).BeginInit();

            // 
            // resourceManager
            // 
            this.resourceManager.CommandName = "ResourceManagerCommand";
            CommandExtender.SetPriority(this.resourceManager, 9);
            // 
            // contentCompiler
            // 
            this.contentCompiler.CommandName = "ContentCompilerCommand";
            CommandExtender.SetPriority(this.contentCompiler, 8);
            // 
            // transportTCPIP
            // 
            this.transportTCPIP.CommandName = "TransportCommandTCPIP";
            CommandExtender.SetPriority(this.transportTCPIP, 6);
            // 
            // transportUDP
            // 
            this.transportUDP.CommandName = "TransportCommandUDP";
            CommandExtender.SetPriority(this.transportUDP, 6);
            // 
            // siteServer
            // 
            this.siteServer.CommandName = "SiteServerCommand";
            CommandExtender.SetPriority(this.siteServer, 4);
            //
            // siteControllerLogger
            //
            this.siteControllerLogger.CommandName = "SiteControllerLogger";
            CommandExtender.SetPriority(this.siteControllerLogger, 4);

            // 
            // siteController
            // 
            this.siteController.CommandName = "SiteControllerCommand";
            CommandExtender.SetPriority(this.siteController, 3);

            ((System.ComponentModel.ISupportInitialize)(CommandExtender)).EndInit();
        }
        #endregion // InitializeComponents()
    }
}
