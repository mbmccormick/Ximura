﻿#region Copyright
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
using System.Threading;
using System.Timers;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Reflection;
using System.Drawing;
using System.Diagnostics;
using System.Collections.Generic;

using Ximura;

using CH = Ximura.Common;
using Ximura.Framework;
using Ximura.Framework;

#endregion // using
namespace Ximura.Communication
{
    /// <summary>
    /// The MessageProcessCommand is a base class for commands that process incoming messages and 
    /// send outgoing message responses.
    /// </summary>
    /// <typeparam name="RQ"></typeparam>
    /// <typeparam name="RS"></typeparam>
    /// <typeparam name="CBRQ"></typeparam>
    /// <typeparam name="CBRS"></typeparam>
    /// <typeparam name="CNTX"></typeparam>
    /// <typeparam name="ST"></typeparam>
    /// <typeparam name="SET"></typeparam>
    public class MessageProcessCommand<RQ, RS, CBRQ, CBRS, CNTX, ST, SET, CONF, PERF> :
        ConnectionFSMBase<RQ, RS, CBRQ, CBRS, CNTX, ST, SET, CONF, PERF>
        where RQ : RQServer, new()
        where RS : RSServer, new()
        where CBRQ : RQCallbackServer, new()
        where CBRS : RSCallbackServer, new()
        where CNTX : class, IXimuraServerContext<RQ, RS, CBRQ, CBRS, CONF, PERF>, new()
        where ST : class,IXimuraServerState<RQ, RS, CBRQ, CBRS, CONF, PERF>
        where SET : class, IXimuraServerSettings<ST, RQ, RS, CBRQ, CBRS, CONF, PERF>, new()
        where CONF : FSMCommandConfiguration, new()
        where PERF : FSMCommandPerformance, new()
    {
        #region Declarations
        private System.ComponentModel.IContainer components = null;
        #endregion
        #region Constructors
        /// <summary>
        /// Empty constructor for use by the component model.
        /// </summary>
        public MessageProcessCommand() : this(null) { }
        /// <summary>
        /// Default constructor for use by the component model.
        /// </summary>
        /// <param name="container">The container that the transport should be added to.</param>
        public MessageProcessCommand(System.ComponentModel.IContainer container)
            : base(container)
        {
            InitializeComponent();
            RegisterContainer(components);

        }
        #endregion
        #region InitializeComponent()
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
        }
        #endregion

    }
}
