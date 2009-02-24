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

using Ximura;
using Ximura.Data;
using Ximura.Helper;
using CH = Ximura.Helper.Common;
using RH = Ximura.Helper.Reflection;
using Ximura.Server;


using Ximura.Command;
#endregion // using
namespace Ximura.Persistence
{
    public class BaseCDSState : CDSState<CommandConfiguration, CDSStatePerformance>
    {
        #region Constructors
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        public BaseCDSState() : this(null) { }
        /// <summary>
        /// This is the component model constructor.
        /// </summary>
        /// <param name="container">The container</param>
        public BaseCDSState(IContainer container)
            : base(container)
        {
        }
        #endregion // Constructors
    }
}
