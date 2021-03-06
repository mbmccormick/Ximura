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
    [XimuraAppConfiguration(ConfigurationLocation.Resource,
        "xmrres://XimuraComm/Ximura.Communication.TransportConfiguration/Ximura.Communication.TransportCommand.Configuration.TransportConfiguration_Default.xml")]
#else
    [XimuraAppConfiguration(ConfigurationLocation.Hybrid, 
        "xmrres://XimuraComm/Ximura.Communication.TransportConfiguration/Ximura.Communication.TransportCommand.Configuration.TransportConfiguration_Default.xml")]
#endif
    [XimuraAppModule(TransportCommandUDP.ID , "TransportCommandUDP")]
    public class TransportCommandUDP : TransportCommand<TransportContextUDP>
    {
        #region Static Declarations
        public const string ID = "A52F7906-C72F-481c-98CE-327BB18E82C9";
        #endregion // Static Declarations
        #region Declarations
        private System.ComponentModel.IContainer components = null;
        #endregion // Declarations
        #region Constructors
        /// <summary>
        /// Empty constructor for use by the component model.
        /// </summary>
        public TransportCommandUDP() : this(null) { }
        /// <summary>
        /// Default constructor for use by the component model.
        /// </summary>
        /// <param name="container">The container that the transport should be added to.</param>
        public TransportCommandUDP(System.ComponentModel.IContainer container)
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
