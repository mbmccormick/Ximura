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
﻿#region using
using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;

using Ximura;
using Ximura.Data;

using CH = Ximura.Common;
using RH = Ximura.Reflection;
using AH = Ximura.AttributeHelper;
using Ximura.Framework;


using Ximura.Framework;
#endregion // using
namespace Ximura.Data
{
    /// <summary>
    /// This class is th base class for persistence managers that use the file system as a store.
    /// </summary>
    /// <typeparam name="CONT"></typeparam>
    /// <typeparam name="DCONT"></typeparam>
    public class FileSystemBasePMCDSState<CONT, DCONT, CONF> : CollectionBasePersistenceManager<CONT, DCONT, CONF>
        where CONT : Content, DCONT
        where DCONT : Content
        where CONF : CommandConfiguration, new()
    {
        #region Declarations
        #endregion // Declarations
        #region Constructors
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        public FileSystemBasePMCDSState() : this(null) { }
        /// <summary>
        /// This is the component model constructor.
        /// </summary>
        /// <param name="container">The container</param>
        public FileSystemBasePMCDSState(IContainer container)
            : base(container)
        {
        }
        #endregion // Constructors
    }
}
