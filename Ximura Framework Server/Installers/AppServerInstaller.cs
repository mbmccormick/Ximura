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

using Ximura;

using Ximura.Framework;
using RH = Ximura.Reflection;
using AH = Ximura.AttributeHelper;
#endregion
namespace Ximura.Framework
{
    /// <summary>
    /// This installer class creates the necessary installers to properly configure the app server for its environment.
    /// </summary>
    public class AppServerInstaller : ComponentInstaller
    {
        //public override void InitializeFromAppServerType(System.Type appServerType)
        //{
        //    //Ok, add the config installer
        //    InstallerConfiguration config = new InstallerConfiguration();
        //    config.InitializeFromAppServerType(appServerType);
        //    this.Installers.Add(config);


        //}

        public override void CopyFromComponent(IComponent component)
        {
            throw new NotImplementedException();
        }

        public override bool IsEquivalentInstaller(System.Configuration.Install.ComponentInstaller otherInstaller)
        {
            return base.IsEquivalentInstaller(otherInstaller);
        }
    }
}
