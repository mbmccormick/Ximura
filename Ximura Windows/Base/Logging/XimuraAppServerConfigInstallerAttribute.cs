﻿#region Copyright
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
using System.Diagnostics;
using System.Security.Permissions;

using Ximura;
using Ximura.Helper;
using RH = Ximura.Helper.Reflection;
using Ximura.Server;

#endregion // using
namespace Ximura.Windows
{
    /// <summary>
    /// This attribute indicates to the installer which config file to install for which appserver class.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class XimuraAppServerConfigInstallerAttribute : System.Attribute
    {
        public XimuraAppServerConfigInstallerAttribute(string appServerTypeName):this(RH.CreateTypeFromString(appServerTypeName)){}

        public XimuraAppServerConfigInstallerAttribute(Type appServerType)
        {
            AppServerType = appServerType;
        }

        /// <summary>
        /// This is the app server type to process.
        /// </summary>
        public Type AppServerType
        {
            get;
            private set;
        }
    }
}