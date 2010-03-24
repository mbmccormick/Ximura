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
    public partial class AppServer<CONFSYS, CONFCOM, PERF>
    {
        #region StorageService
        /// <summary>
        /// This is the storage manager for the application.
        /// </summary>
        protected virtual StorageManager StorageService
        {
            get;
            set;
        }
        #endregion // Storage

        #region StorageCreate()
        /// <summary>
        /// This method creates the storage managers and the storage agents.
        /// </summary>
        protected virtual void StorageCreate()
        {
            StorageService = new StorageManager(ControlContainer);
            //Add the storage agents.
            AgentsAdd<XimuraAppServerStorageAttribute>(StorageDefault, StorageService);
        }
        #endregion // StorageStart()
        #region StorageDispose()
        /// <summary>
        /// This method removes the storage services and disposes of the storage manager.
        /// </summary>
        protected virtual void StorageDispose()
        {
            StorageService.Dispose();
            StorageService = null;
        }
        #endregion // StorageDispose()

        #region StorageStart()
        /// <summary>
        /// This private method checks whether a Logging and Performance manager is active
        /// in the application, and if not creates a default one.
        /// </summary>
        protected virtual void StorageStart()
        {
            //We wait until here to start the services as they have reference to themselves.
            if (((IXimuraService)StorageService).ServiceStatus != XimuraServiceStatus.Started)
                ((IXimuraService)StorageService).Start();
        }
        #endregion
        #region StorageStop()
        /// <summary>
        /// This method is used to cleanup and stop logging.
        /// </summary>
        protected virtual void StorageStop()
        {
            if (((IXimuraService)StorageService).ServiceStatus != XimuraServiceStatus.Stopped)
                ((IXimuraService)StorageService).Stop();
        }
        #endregion

        #region StorageDefault
        /// <summary>
        /// This property returns default loggers for the application. 
        /// </summary>
        protected virtual IEnumerable<XimuraServerAgentHolder> StorageDefault
        {
            get
            {
                yield break;
            }
        }
        #endregion
    }
}
