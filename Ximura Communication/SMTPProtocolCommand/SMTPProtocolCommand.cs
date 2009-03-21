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
#region Copyright
// *****************************************************************************
// 
//  Loyalty Points System
// 
//  (c) SLA Ltd 2003-2004 
//
//  All rights reserved. The software and associated documentation 
//  supplied hereunder are the proprietary information of 
//  StancerLee Associates Limited (SLA Ltd), Central, Hong Kong 
//  and are supplied subject to the Non-Disclosure Agreement agreed between 
//  the distributed party and SLA Ltd.
//
//  This code cannot be distributed in part or as a whole to any third party 
//  without the express written permission of SLA Ltd.
//
// *****************************************************************************
#endregion // Copyright
#region using
using System;
using System.Collections;
using System.Diagnostics;
using System.Data;
using System.Reflection;
using System.ComponentModel;
using System.Collections.Generic;

using XIMS;
using XIMS.Data;
using CH = XIMS.Helper.Common;
using XIMS.Helper;
using XIMS.Applications;
using XIMS.Applications.Data;
using XIMS.Applications.Security;
using XIMS.Applications.Command;
#endregion // using
namespace XIMS.Communication
{
    /// <summary>
    /// This command manages the SMTP protocol.
    /// </summary>
    [XIMSAppModule("B9DAC783-3F0E-452a-95D6-BD93452DFA2A", "SMTPProtocolCommand")]
    public class SMTPProtocolCommand : ProtocolCommand
    {
        #region Declarations
        private System.ComponentModel.IContainer components = null;
        /// <summary>
        /// This is the context to hold protocol connections.
        /// </summary>
        protected SMTPProtocolContext smtpContext;
        /// <summary>
        /// This is the tcp transport provider.
        /// </summary>
        protected TCPIPTransport tcpip;
        #endregion
        #region Constructors
        /// <summary>
        /// Empty constructor for use by the component model.
        /// </summary>
        public SMTPProtocolCommand() : this(null) { }
        /// <summary>
        /// Default constructor for use by the component model.
        /// </summary>
        /// <param name="container">The container that the transport should be added to.</param>
        public SMTPProtocolCommand(System.ComponentModel.IContainer container)
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
            this.smtpContext = new XIMS.Communication.SMTPProtocolContext();
            tcpip = new TCPIPTransport(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.mStateExtender)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mContextExtender)).BeginInit();
            // 
            // mContextExtender
            // 
            this.mContextExtender.DefaultContext = this.smtpContext;
            ((System.ComponentModel.ISupportInitialize)(this.mStateExtender)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mContextExtender)).EndInit();
        }
        #endregion
    }
}
