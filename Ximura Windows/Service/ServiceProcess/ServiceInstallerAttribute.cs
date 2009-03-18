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
using Ximura.Server;
using RH = Ximura.Helper.Reflection;
#endregion // using
namespace Ximura.Windows
{
    /// <summary>
    /// This attribute specifies the service installer properties.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class ServiceInstallerAttribute : System.Attribute
    {
        public ServiceInstallerAttribute(ServiceStartMode StartType, string ServiceName, string DisplayName, string Description)
            :this(StartType, ServiceName, DisplayName, Description, ServiceAccount.User, null, null)
        {

        }

        public ServiceInstallerAttribute(
            ServiceStartMode StartType, string ServiceName, string DisplayName, string Description,
            ServiceAccount Account, string Username, string Password)
        {
            this.DisplayName = DisplayName;
            this.ServiceName = ServiceName;
            this.Description = Description;
            this.StartType = StartType;

            this.Account = Account;
            this.Username = Username;
            this.Password = Password;
        }

        public string DisplayName { get; private set; }
        public string ServiceName { get; private set; }
        public string Description { get; private set; }
        public ServiceStartMode StartType { get; private set; }

        public ServiceAccount Account { get; private set; }
        public string Username { get; private set; }
        public string Password { get; private set; }
    }
}
