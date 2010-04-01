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
using System.Linq;
using System.Threading;
using System.Timers;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Reflection;
using System.Drawing;
using System.Diagnostics;

using Ximura;
using Ximura.Data;
using CH = Ximura.Common;
using Ximura.Framework;

using Ximura.Framework;

#endregion // using
namespace Ximura.Framework
{
    public class FSMExtenderCommand : FSMExtenderCommand<RQRSFolder, RQRSFolder, RQRSFolder, RQRSFolder, 
        CommandConfiguration, CommandPerformance>
    {
        #region Constructors
        /// <summary>
        /// This is the empty constructor
        /// </summary>
        public FSMExtenderCommand() : this((IContainer)null) { }
        /// <summary>
        /// This is the constrcutor used by the Ximura Application model.
        /// </summary>
        /// <param name="container">The command container.</param>
        public FSMExtenderCommand(System.ComponentModel.IContainer container)
            : base(container)
        {
        }
        /// <summary>
        /// This is the base constructor for a Ximura command
        /// </summary>
        /// <param name="commandID">This is the explicitly set command id, leave this as null if you want to use the default id.</param>
        /// <param name="container">The container to be added to</param>
        public FSMExtenderCommand(Guid? commandID, System.ComponentModel.IContainer container) : base(commandID, container) { }
        #endregion

    }

    /// <summary>
    /// The state extender command allows third parties to extend exisiting finite state machines through a modular architecture.
    /// </summary>
    /// <typeparam name="RQ">The main request RQRSFolder type.</typeparam>
    /// <typeparam name="RS">The main response RQRSFolder type</typeparam>
    /// <typeparam name="CBRQ">The callback request RQRSFolder type</typeparam>
    /// <typeparam name="CBRS">The callback response RQRSFolder type</typeparam>
    /// <typeparam name="CONF">The command configuration object.</typeparam>
    /// <typeparam name="PERF">The command performance monitor object.</typeparam>
    public class FSMExtenderCommand<RQ, RS, CBRQ, CBRS, CONF, PERF> : AppCommandProcess<RQ, RS, CBRQ, CBRS, CONF, PERF>
        where RS : RQRSFolder, new()
        where RQ : RQRSFolder, new()
        where CBRQ : RQRSFolder, new()
        where CBRS : RQRSFolder, new()
        where CONF : CommandConfiguration, new()
        where PERF : CommandPerformance, new()
    {
        #region Decarations
        IXimuraStateExtenderService mStateExtenderService = null;

        /// <summary>
        /// This is the state extender.
        /// </summary>
        protected FSMExtenderStateExtender mStateExtender = null;
        #endregion // Decarations
		#region Constructors
		/// <summary>
		/// This is the empty constructor
		/// </summary>
		public FSMExtenderCommand():this((IContainer)null){}
		/// <summary>
		/// This is the constrcutor used by the Ximura Application model.
		/// </summary>
		/// <param name="container">The command container.</param>
        public FSMExtenderCommand(System.ComponentModel.IContainer container)
            : base(container)
		{
		}
        /// <summary>
        /// This is the base constructor for a Ximura command
        /// </summary>
        /// <param name="commandID">This is the explicitly set command id, leave this as null if you want to use the default id.</param>
        /// <param name="container">The container to be added to</param>
        public FSMExtenderCommand(Guid? commandID, System.ComponentModel.IContainer container) : base(commandID, container) { }
		#endregion

        #region ServicesReference/ServicesDereference
        /// <summary>
        /// This override retrieves a reference to the IXimuraStateExtenderService object.
        /// </summary>
        protected override void ServicesReference()
        {
            base.ServicesReference();
            mStateExtenderService = GetService<IXimuraStateExtenderService>();
        }
        /// <summary>
        /// This override removes the reference to the IXimuraStateExtenderService object.
        /// </summary>
        protected override void ServicesDereference()
        {
            mStateExtenderService = null;
            base.ServicesDereference();
        }
        #endregion // ServicesReference/ServicesDereference
    }
}
