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

using AH=Ximura.AttributeHelper;
using Ximura.Framework;
using RH = Ximura.Reflection;
#endregion // using
namespace Ximura.Windows
{
    /// <summary>
    /// This is the base installer used to install the service.
    /// </summary>
    public class ServiceAppInstaller : System.Configuration.Install.Installer
    {
        #region Constructor
        /// <summary>
        /// The default constructor.
        /// </summary>
        public ServiceAppInstaller()
        {
            ServiceInstallerAttribute attr = AH.GetAttribute<ServiceInstallerAttribute>(GetType());

            if (attr == null)
                throw new InstallException("The ServiceInstallerAttribute cannot be found.");

            //Create the installer class for each app server
            InitializeAppServers(attr.ServiceType);
            //Create the service and service process installer.
            InitializeService(attr);
        }
        #endregion // Constructor

        protected virtual void InitializeAppServers(Type serviceType)
        {
            AH.GetAttributes<AppServerAttribute>(serviceType)
                .ForEach(a => this.Installers.Add(new AppServerInstaller(a)));
        }

        #region InitializeService(ServiceInstallerAttribute servAttr)
        /// <summary>
        /// This method creates the service and service process installer based on the information passed in the ServiceInstallerAttribute class.
        /// </summary>
        protected virtual void InitializeService(ServiceInstallerAttribute servAttr)
        {
            ServiceProcessInstaller serviceProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            ServiceInstaller serviceInstaller = new System.ServiceProcess.ServiceInstaller();

            // 
            // serviceInstaller
            // 
            serviceInstaller.DisplayName = servAttr.DisplayName;
            serviceInstaller.ServiceName = servAttr.ServiceName;
            serviceInstaller.Description = servAttr.Description;
            serviceInstaller.StartType = servAttr.StartMode;

            //this.serviceInstaller.DisplayName = "Budubu Analysis Engine";
            //this.serviceInstaller.ServiceName = "BudubuAnalysisEngine";
            //this.serviceInstaller.Description = "This is the Snagsta.com analysis service.";
            //this.serviceInstaller.StartType = System.ServiceProcess.ServiceStartMode.Automatic;

            // 
            // serviceProcessInstaller
            // 
            serviceProcessInstaller.Account = servAttr.Account;
            serviceProcessInstaller.Username = servAttr.Username;
            serviceProcessInstaller.Password = servAttr.Password;

            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(
                new System.Configuration.Install.Installer[] { serviceProcessInstaller, serviceInstaller }
                );

        }
        #endregion
    }
}
