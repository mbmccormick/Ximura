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

using Ximura.Communication;
#endregion // using
namespace Ximura.Communication
{
    public class StartTransportState: TransportState
    {
        #region Constructors
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        public StartTransportState() : this(null) { }
        /// <summary>
        /// This is the component model constructor.
        /// </summary>
        /// <param name="container">The container</param>
        public StartTransportState(IContainer container)
        {
            if (container != null)
                container.Add(this);
        }
        #endregion // Constructors

        #region Initialize
        /// <summary>
        /// This method currently does nothing.
        /// </summary>
        /// <param name="context">The current transport context.</param>
        /// <param name="job">The current job.</param>
        /// <param name="Data">The data.</param>
        public override void Initialize(TransportContext context)
        {
        }
        #endregion // Initialize

        #region ConnectionRequest
        /// <summary>
        /// This method swaps to the open state and calls the corresponding method.
        /// </summary>
        /// <param name="context">The current context.</param>
        /// <returns>Returns true if the server accepted the connection request.</returns>
        public override bool ConnectionRequest(TransportContext context)
        {
            context.ChangeState("Open");
            return context.ConnectionRequest();
        }
        #endregion // ConnectionRequest
        #region OpenRequest
        /// <summary>
        /// This method swaps to the open state and calls the corresponding method.
        /// </summary>
        /// <param name="context">The current context.</param>
        /// <param name="job">The job.</param>
        /// <param name="Data">The request/response data.</param>
        /// <returns></returns>
        public override bool OpenRequest(TransportContext context, SecurityManagerJob job, RQRSContract<TransportCommandRequest, TransportCommandResponse> Data)
        {
            context.ChangeState("Open");
            return context.OpenRequest(job, Data);
        }
        #endregion // OpenRequest
        #region ListenRequest
        /// <summary>
        /// This method changes the context to the listen state.
        /// </summary>
        /// <param name="context">The current context.</param>
        /// <param name="job">The job.</param>
        /// <param name="Data">The request/response data.</param>
        /// <returns></returns>
        public override bool ListenRequest(TransportContext context, SecurityManagerJob job, RQRSContract<TransportCommandRequest, TransportCommandResponse> Data)
        {
            context.ChangeState("Listen");
            return context.ListenRequest(job, Data);
        }
        #endregion // ListenRequest
    }
}
