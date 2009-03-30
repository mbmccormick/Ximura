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
namespace Ximura.Server
{
    public partial class AppServer<CONFSYS, CONFCOM, PERF>
    {
        #region PoolManagerCreate()
        /// <summary>
        /// This method creates the pool manager for the system.
        /// </summary>
        protected virtual void PoolManagerCreate()
        {
            PoolManager = new PoolManager(true);
        }
        #endregion // PoolManagerCreate()
        #region PoolManagerDispose()
        /// <summary>
        /// This method disposes of the pool manager for the system.
        /// </summary>
        protected virtual void PoolManagerDispose()
        {
            PoolManager.Dispose();
            PoolManager = null;
        }
        #endregion // PoolManagerDestroy()
        #region PoolManagerStart()
        /// <summary>
        /// This protected method creates the default pool manager for the application.
        /// </summary>
        protected override void PoolManagerStart()
        {

        }
        #endregion // PoolManagerStart()
        #region PoolManagerStop()
        /// <summary>
        /// This protected method disposes of the default pool manager for the application.
        /// </summary>
        protected override void PoolManagerStop()
        {
        }
        #endregion // PoolManagerStop()
    }
}
