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

using Ximura.Framework;
using Ximura.Framework;
using AH = Ximura.AttributeHelper;
using RH = Ximura.Reflection;
using CH = Ximura.Common;
#endregion // using
namespace Ximura.Framework
{
    /// <summary>
    /// This is the base class for the application system agents.
    /// </summary>
    /// <typeparam name="CONF">The agent configuration class.</typeparam>
    /// <typeparam name="PERF">The agent performance class.</typeparam>
    public class AppServerAgentBase<CONF, PERF> : AppBase<CONF, PERF>, IXimuraServerAgent
        where CONF : ConfigurationBase, new()
        where PERF : PerformanceCounterCollection, new()
    {
        #region Constructor
		/// <summary>
		/// This is the default constructor
		/// </summary>
		public AppServerAgentBase():this(null){}
		/// <summary>
		/// This is the base constructor for a Ximura command
		/// </summary>
		/// <param name="container">The container to be added to</param>
        public AppServerAgentBase(System.ComponentModel.IContainer container)
            : base(container) 
        {
        }
        #endregion
    }
}
