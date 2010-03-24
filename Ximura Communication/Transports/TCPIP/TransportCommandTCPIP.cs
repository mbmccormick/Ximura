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
using System.Threading;
using System.Timers;
using System.Data;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Reflection;
using System.Drawing;
using System.Diagnostics;
using System.Runtime.Serialization;

using Ximura;
using Ximura.Data;

using CH = Ximura.Common;
using Ximura.Framework;
using Ximura.Framework;

#endregion // using
namespace Ximura.Communication
{
#if (DEBUG)
    [XimuraAppConfiguration(ConfigurationLocation.Hybrid,
        "xmrres://XimuraComm/Ximura.Communication.TransportConfiguration/Ximura.Communication.TransportCommand.Configuration.TransportConfiguration_Default.xml")]
#else
    [XimuraAppConfiguration(ConfigurationLocation.Hybrid, 
        "xmrres://XimuraComm/Ximura.Communication.TransportConfiguration/Ximura.Communication.TransportCommand.Configuration.TransportConfiguration_Default.xml")]
#endif
    [XimuraAppModule(TransportCommandTCPIP.ID, "TransportCommandTCPIP")]
    public class TransportCommandTCPIP: TransportCommandIPBase<TransportContextTCPIP>
    {
        #region Static Declarations
        public const string ID = "06F5003C-4502-40c8-8767-2966D8E471A2";
        #endregion // Static Declarations
        #region Declarations
        private System.ComponentModel.IContainer components = null;
        /// <summary>
        /// This is the start state.
        /// </summary>
        protected ListenTCPIPTransportState listenState;

        protected OpenTCPIPTransportState openState;

        protected CloseTCPIPTransportState closeState;
        #endregion // Declarations
        #region Constructors
        /// <summary>
        /// Empty constructor for use by the component model.
        /// </summary>
        public TransportCommandTCPIP() : this(null) { }
        /// <summary>
        /// Default constructor for use by the component model.
        /// </summary>
        /// <param name="container">The container that the transport should be added to.</param>
        public TransportCommandTCPIP(System.ComponentModel.IContainer container)
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

            listenState = new ListenTCPIPTransportState(components);
            openState = new OpenTCPIPTransportState(components);
            closeState = new CloseTCPIPTransportState(components);

            ((System.ComponentModel.ISupportInitialize)(this.mStateExtender)).BeginInit();
            // 
            // startState
            // 
            this.mStateExtender.SetEnabled(this.startState, true);
            this.mStateExtender.SetNextStateID(this.startState, null);
            this.mStateExtender.SetStateID(this.startState, "Start");
            // 
            // listenState
            // 
            this.mStateExtender.SetEnabled(this.listenState, true);
            this.mStateExtender.SetNextStateID(this.listenState, null);
            this.mStateExtender.SetStateID(this.listenState, "Listen");
            // 
            // openState
            // 
            this.mStateExtender.SetEnabled(this.openState, true);
            this.mStateExtender.SetNextStateID(this.openState, null);
            this.mStateExtender.SetStateID(this.openState, "Open");
            // 
            // closeState
            // 
            this.mStateExtender.SetEnabled(this.closeState, true);
            this.mStateExtender.SetNextStateID(this.closeState, null);
            this.mStateExtender.SetStateID(this.closeState, "Close");
            // 
            // State_Extender
            // 
            this.mStateExtender.InitialState = "Start";
            ((System.ComponentModel.ISupportInitialize)(this.mStateExtender)).EndInit();
        }
        #endregion
    }

}
