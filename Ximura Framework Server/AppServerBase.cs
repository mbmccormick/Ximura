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
using Ximura.Framework;
using Ximura.Framework;
using AH = Ximura.Helper.AttributeHelper;
using RH = Ximura.Helper.Reflection;
using CH = Ximura.Helper.Common;
#endregion
namespace Ximura.Framework
{
    /// <summary>
    /// The AppServerBase class is the class that all server applications derive from.
    /// </summary>
    public class AppServer : AppServer<AppServerSystemConfiguration, AppServerCommandConfiguration, AppServerPerformance>
    {
        #region Constructors
        /// <summary>
        /// This is the default constructor for the service.
        /// </summary>
        public AppServer()
            : this((IContainer)null)
        {
        }
        /// <summary>
        /// This constructor is called by the .Net component model when adding it to a container
        /// </summary>
        /// <param name="container">The container to add the component to.</param>
        public AppServer(IContainer container)
            : base(container)
        {
        }
        #endregion
    }
}
