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
    /// The SIP Protocol is the root command for communication between 
    /// applications within the XIMS framework.
    /// </summary>
    [XIMSAppModule("AE24EC40-C855-4f7f-B501-7802346AE6D0", "SIPProtocolCommand")]
    public class SIPProtocolCommand : InternetProtocolCommand
    {
		#region Declarations
		private System.ComponentModel.IContainer components = null;
        /// <summary>
        /// This is the context to hold protocol connections.
        /// </summary>
        protected SIPProtocolContext sipContext;
        /// <summary>
        /// This is the tcp transport provider.
        /// </summary>
        protected TCPIPTransport tcpip;
        /// <summary>
        /// This is the secure tcp transport provider.
        /// </summary>
        protected TCPIPTransport tcpips;
        /// <summary>
        /// This is the udp transport provider.
        /// </summary>
        protected UDPTransport udp;

        protected AckSIP ACK;
        protected ByeSIP BYE;
        protected CancelSIP CANCEL;
        protected InviteSIP INVITE;
        protected OptionsSIP OPTIONS;
        protected RegisterSIP REGISTER;
        #endregion
        #region Constructors
        /// <summary>
        /// Empty constructor for use by the component model.
        /// </summary>
        public SIPProtocolCommand() : this(null) { }

        /// <summary>
        /// Default constructor for use by the component model.
        /// </summary>
        /// <param name="container">The container that the transport should be added to.</param>
        public SIPProtocolCommand(System.ComponentModel.IContainer container)
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
            this.ACK = new XIMS.Communication.AckSIP(this.components);
            this.BYE = new XIMS.Communication.ByeSIP(this.components);
            this.CANCEL = new XIMS.Communication.CancelSIP(this.components);
            this.INVITE = new XIMS.Communication.InviteSIP(this.components);
            this.OPTIONS = new XIMS.Communication.OptionsSIP(this.components);
            this.REGISTER = new XIMS.Communication.RegisterSIP(this.components);
            this.sipContext = new XIMS.Communication.SIPProtocolContext();
            this.tcpip = new XIMS.Communication.TCPIPTransport(this.components);
            this.tcpips = new XIMS.Communication.TCPIPTransport(this.components);
            this.udp = new XIMS.Communication.UDPTransport(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.transportExtender)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.authHandlerExtender)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.verbExtender)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mStateExtender)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mContextExtender)).BeginInit();
            // 
            // transportExtender
            // 
            this.transportExtender.DefaultTransport = this.udp;
            // 
            // mContextExtender
            // 
            this.mContextExtender.DefaultContext = this.sipContext;
            // 
            // ACK
            // 
            this.ACK.CommandDescription = "";
            this.ACK.CommandName = "";
            this.verbExtender.SetEnabled(this.ACK, true);
            // 
            // BYE
            // 
            this.BYE.CommandDescription = "";
            this.BYE.CommandName = "";
            this.verbExtender.SetEnabled(this.BYE, true);
            // 
            // CANCEL
            // 
            this.CANCEL.CommandDescription = "";
            this.CANCEL.CommandName = "";
            this.verbExtender.SetEnabled(this.CANCEL, true);
            // 
            // INVITE
            // 
            this.INVITE.CommandDescription = "";
            this.INVITE.CommandName = "";
            this.verbExtender.SetEnabled(this.INVITE, true);
            // 
            // OPTIONS
            // 
            this.OPTIONS.CommandDescription = "";
            this.OPTIONS.CommandName = "";
            this.verbExtender.SetEnabled(this.OPTIONS, true);
            // 
            // REGISTER
            // 
            this.REGISTER.CommandDescription = "";
            this.REGISTER.CommandName = "";
            this.verbExtender.SetEnabled(this.REGISTER, true);
            // 
            // sipContext
            // 
            this.mContextExtender.SetEnabled(this.sipContext, true);
            // 
            // tcpip
            // 
            this.tcpip.CommandDescription = "";
            this.tcpip.CommandName = "tcp";
            this.transportExtender.SetEnabled(this.tcpip, true);
            // 
            // tcpips
            // 
            this.tcpips.CommandDescription = "";
            this.tcpips.CommandName = "sips";
            // 
            // udp
            // 
            this.udp.CommandDescription = "";
            this.udp.CommandName = "sip";
            this.transportExtender.SetEnabled(this.udp, true);
            ((System.ComponentModel.ISupportInitialize)(this.transportExtender)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.authHandlerExtender)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.verbExtender)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mStateExtender)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mContextExtender)).EndInit();

        }
        #endregion
    }
}
