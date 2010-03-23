#region Copyright
//*******************************************************************************
//Copyright (c) 2000-2009 Paul Stancer.
//All rights reserved. This program and the accompanying materials
//are made available under the terms of the Eclipse Public License v1.0
//which accompanies this distribution, and is available at
//http://www.eclipse.org/legal/epl-v10.html

//Contributors:
//    Paul Stancer - initial implementation
//*******************************************************************************
#endregion
#region using
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Threading;
using System.Reflection;
using System.Security.Cryptography;
using System.Linq;

using Ximura;
using Ximura.Data;
using Ximura.Helper;
using CH = Ximura.Helper.Common;
using Ximura.Server;

using Ximura.Command;

#endregion // using
namespace Ximura.Server
{
    /// <summary>
    /// This is the base class for performance agents.
    /// </summary>
    public class PerformanceAgentBase<CONF> : 
        AppServerAgentBase<CONF, PerformanceCounterCollection>, IXimuraPerformanceAgent
         where CONF : ConfigurationBase, new()
    {
        #region Constructor
		/// <summary>
		/// This is the default constructor
		/// </summary>
		public PerformanceAgentBase():this(null){}
		/// <summary>
		/// This is the base constructor for a Ximura command
		/// </summary>
		/// <param name="container">The container to be added to</param>
        public PerformanceAgentBase(System.ComponentModel.IContainer container)
            : base(container) 
        {
        }
        #endregion
    }
}
