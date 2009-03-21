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
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Reflection;
using System.Drawing;
using System.Diagnostics;

using Ximura;
using Ximura.Data;
using Ximura.Helper;
using Ximura.Server;

using Ximura.Command;

#endregion // using
namespace Ximura.Communication
{
    /// <summary>
    /// This is the base class for the authentication handler.
    /// </summary>
    public class AuthHandler : AppCommandProcess<RQRSFolder, RQRSFolder, RQRSFolder, RQRSFolder, 
        CommandConfiguration, CommandPerformance>, IXimuraAuthHandler
    {
        protected IXimuraSessionManager sessionMan = null;

        #region Constructors
        /// <summary>
        /// Empty constructor for use by the component model.
        /// </summary>
        public AuthHandler() : this(null) { }

        /// <summary>
        /// Default constructor for use by the component model.
        /// </summary>
        /// <param name="container">The container that the transport should be added to.</param>
        public AuthHandler(System.ComponentModel.IContainer container)
            : base(container)
        {
        }
        #endregion

        protected override void ServicesReference()
        {
            base.ServicesReference();
            sessionMan = GetService(typeof(IXimuraSessionManager)) as IXimuraSessionManager;
        }

        protected override void ServicesDereference()
        {
            sessionMan = null;
            base.ServicesDereference();
        }

        protected override void CommandBridgeRegister(bool register)
        {

        }
    }
}
