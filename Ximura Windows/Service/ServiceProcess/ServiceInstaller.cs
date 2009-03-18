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
using System.Linq;
using System.Threading;
using System.Timers;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Reflection;
using System.Diagnostics;
using System.Security.Cryptography;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Configuration.Install;
using System.ServiceProcess;

using Ximura;
using Ximura.Helper;
using AH=Ximura.Helper.AttributeHelper;
using Ximura.Server;
using RH = Ximura.Helper.Reflection;
#endregion // using
namespace Ximura.Windows
{
    /// <summary>
    /// This is the base installer used to install the service.
    /// </summary>
    public class ServiceAppInstaller : ApplicationInstaller
    {
        #region Declarations
        private System.ServiceProcess.ServiceProcessInstaller serviceProcessInstaller;
        private System.ServiceProcess.ServiceInstaller serviceInstaller;
        #endregion // Declarations
        #region Constructor
        /// <summary>
        /// The default constructor.
        /// </summary>
        public ServiceAppInstaller()
        {
            // This call is required by the Designer.
            InitializeService(AH.GetAttribute<ServiceInstallerAttribute>(GetType()));
        }
        #endregion // Constructor

        #region InitializeService(ServiceInstallerAttribute servAttr)
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        protected virtual void InitializeService(ServiceInstallerAttribute servAttr)
        {
            this.serviceProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            this.serviceInstaller = new System.ServiceProcess.ServiceInstaller();

            // 
            // serviceInstaller
            // 
            this.serviceInstaller.DisplayName = servAttr.DisplayName;
            this.serviceInstaller.ServiceName = servAttr.ServiceName;
            this.serviceInstaller.Description = servAttr.Description;
            this.serviceInstaller.StartType = servAttr.StartType;

            //this.serviceInstaller.DisplayName = "Budubu Analysis Engine";
            //this.serviceInstaller.ServiceName = "BudubuAnalysisEngine";
            //this.serviceInstaller.Description = "This is the Snagsta.com analysis service.";
            //this.serviceInstaller.StartType = System.ServiceProcess.ServiceStartMode.Automatic;

            // 
            // serviceProcessInstaller
            // 
            this.serviceProcessInstaller.Account = servAttr.Account;
            this.serviceProcessInstaller.Username = servAttr.Username;
            this.serviceProcessInstaller.Password = servAttr.Password;

            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(
                new System.Configuration.Install.Installer[] { this.serviceProcessInstaller, this.serviceInstaller }
                );

        }
        #endregion
    }
}
