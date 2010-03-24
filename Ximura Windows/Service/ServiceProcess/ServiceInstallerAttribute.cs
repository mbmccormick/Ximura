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

using Ximura.Framework;
using RH = Ximura.Reflection;
#endregion // using
namespace Ximura.Windows
{
    /// <summary>
    /// This attribute specifies the service installer properties.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class ServiceInstallerAttribute : System.Attribute
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ServiceType"></param>
        /// <param name="StartMode"></param>
        /// <param name="ServiceName"></param>
        /// <param name="DisplayName"></param>
        /// <param name="Description"></param>
        public ServiceInstallerAttribute(Type ServiceType, 
            ServiceStartMode StartMode, string ServiceName, string DisplayName, string Description)
            : this(ServiceType, StartMode, ServiceName, DisplayName, Description, ServiceAccount.User, null, null)
        {

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ServiceType"></param>
        /// <param name="StartMode"></param>
        /// <param name="ServiceName"></param>
        /// <param name="DisplayName"></param>
        /// <param name="Description"></param>
        /// <param name="Account"></param>
        /// <param name="Username"></param>
        /// <param name="Password"></param>
        public ServiceInstallerAttribute(Type ServiceType, 
            ServiceStartMode StartMode, string ServiceName, string DisplayName, string Description,
            ServiceAccount Account, string Username, string Password)
        {
            this.ServiceType = ServiceType;

            this.StartMode = StartMode;
            this.ServiceName = ServiceName;
            this.DisplayName = DisplayName;
            this.Description = Description;

            this.Account = Account;
            this.Username = Username;
            this.Password = Password;
        }

        public Type ServiceType { get; private set; }

        public ServiceStartMode StartMode { get; private set; }
        public string ServiceName { get; private set; }
        public string DisplayName { get; private set; }
        public string Description { get; private set; }

        public ServiceAccount Account { get; private set; }
        public string Username { get; private set; }
        public string Password { get; private set; }
    }
}
