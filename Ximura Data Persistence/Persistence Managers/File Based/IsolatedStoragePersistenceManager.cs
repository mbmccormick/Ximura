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
using System.Data;
using System.Data.SqlClient;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.IO.IsolatedStorage;

using Ximura;
using Ximura.Data;
using CH = Ximura.Common;
using RH = Ximura.Reflection;
using AH = Ximura.AttributeHelper;
using Ximura.Framework;
#endregion // using
namespace Ximura.Data
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="CONT"></typeparam>
    /// <typeparam name="DCONT"></typeparam>
    /// <typeparam name="CONF"></typeparam>
    [CDSStateActionPermit(CDSAction.Construct)]
    [CDSStateActionPermit(CDSAction.Create)]
    [CDSStateActionPermit(CDSAction.Read)]
    [CDSStateActionPermit(CDSAction.Update)]
    [CDSStateActionPermit(CDSAction.Delete)]
    [CDSStateActionPermit(CDSAction.VersionCheck)]
    [CDSStateActionPermit(CDSAction.ResolveReference)]
    [CDSStateActionPermit(CDSAction.Browse)]
    public class IsolatedStoragePersistenceManager<CONT, DCONT, CONF> : FileSystemPersistenceManager<CONT, DCONT, CONF>
        where CONT : Content, DCONT
        where DCONT : Content
        where CONF : CommandConfiguration, new()
    {
        #region Constructors
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        public IsolatedStoragePersistenceManager() : this(null) { }
        /// <summary>
        /// This is the component model constructor.
        /// </summary>
        /// <param name="container">The container</param>
        public IsolatedStoragePersistenceManager(IContainer container)
            : base(container)
        {

        }
        #endregion // Constructors









    }
}
