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
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Reflection;
using System.Diagnostics;

using Ximura;
using Ximura.Helper;
using Ximura.Data;
using Ximura.Framework;
using Ximura.Framework;
using Ximura.Data;
using Ximura.Communication;
#endregion // using
namespace Ximura.Templates
{
    /// <summary>
    /// This template provides the base functionality for an internet based application server.
    /// </summary>
    public class InternetAppServer : CommAppServer
    {
        #region Declarations
        private System.ComponentModel.IContainer components;
        #endregion
        #region Constructors
        /// <summary>
        /// This is the default constructor for the service.
        /// </summary>
        public InternetAppServer()
            : this((IContainer)null)
        {
        }
        /// <summary>
        /// This constructor is called by the .Net component model when adding it to a container
        /// </summary>
        /// <param name="container">The container to add the component to.</param>
        public InternetAppServer(IContainer container)
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
