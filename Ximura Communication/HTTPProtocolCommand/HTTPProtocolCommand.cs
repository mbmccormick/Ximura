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

using XIMS;
using XIMS.Helper;

using XIMS.Applications;
using XIMS.Applications.Command;
#endregion // using
namespace XIMS.Communication
{
    /// <summary>
    /// This command handles the HTTP Protocol command.
    /// </summary>
    [XIMSAppModule("4F7C8ADF-F85B-4aa6-A983-61BE37F5A1D4", "HTTPProtocolCommand")]
    public class HTTPProtocolCommand : ProtocolCommand
    {
        #region Declarations
        private System.ComponentModel.IContainer components = null;
        /// <summary>
        /// This is the context to hold protocol connections.
        /// </summary>
        protected HTTPProtocolContext httpContext;
        /// <summary>
        /// This is the tcp transport provider.
        /// </summary>
        protected TCPIPTransport tcpip;
        #endregion

        #region Constructors
        /// <summary>
        /// Empty constructor for use by the component model.
        /// </summary>
        public HTTPProtocolCommand() : this(null) { }
        /// <summary>
        /// Default constructor for use by the component model.
        /// </summary>
        /// <param name="container">The container that the transport should be added to.</param>
        public HTTPProtocolCommand(System.ComponentModel.IContainer container)
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
            this.httpContext = new XIMS.Communication.HTTPProtocolContext();
            tcpip = new TCPIPTransport(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.mStateExtender)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mContextExtender)).BeginInit();
            // 
            // mContextExtender
            // 
            this.mContextExtender.DefaultContext = this.httpContext;
            ((System.ComponentModel.ISupportInitialize)(this.mStateExtender)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mContextExtender)).EndInit();
        }
        #endregion
    }
}
