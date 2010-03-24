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
    /// <summary>
    /// This is the base state for the transport command.
    /// </summary>
    public class TransportState : State
    {
        #region Constructors
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        public TransportState() : this(null) { }
        /// <summary>
        /// This is the component model constructor.
        /// </summary>
        /// <param name="container">The container</param>
        public TransportState(IContainer container)
        {
            if (container != null)
                container.Add(this);
        }
        #endregion // Constructors

        public virtual void Initialize(TransportContext context)
        {
            throw new NotImplementedException("Initialize is not implemented.");
        }

        public virtual bool ListenRequest(TransportContext context, SecurityManagerJob job, RQRSContract<TransportCommandRequest, TransportCommandResponse> Data)
        {
            throw new NotImplementedException("ListenRequest is not implemented.");
        }

        public virtual bool ListenConfirm(TransportContext context, SecurityManagerJob job, RQRSContract<TransportCommandRequest, TransportCommandResponse> Data)
        {
            throw new NotImplementedException("ListenConfirm is not implemented.");
        }

        public virtual bool ConnectionRequest(TransportContext context)
        {
            throw new NotImplementedException("ConnectionRequest is not implemented.");
        }

        public virtual bool OpenRequest(TransportContext context, SecurityManagerJob job, RQRSContract<TransportCommandRequest, TransportCommandResponse> Data)
        {
            throw new NotImplementedException("OpenRequest is not implemented.");
        }

        public virtual bool OpenConfirm(TransportContext context, SecurityManagerJob job, RQRSContract<TransportCommandRequest, TransportCommandResponse> Data)
        {
            throw new NotImplementedException("OpenConfirm is not implemented.");
        }

        public virtual bool Transmit(TransportContext context, SecurityManagerJob job, RQRSContract<TransportCommandRequest, TransportCommandResponse> Data)
        {
            throw new NotImplementedException("Transmit is not implemented.");
        }

        #region Close
        /// <summary>
        /// This base method switches any close request to the Close State.
        /// </summary>
        /// <param name="context">The current context.</param>
        /// <param name="job">The job.</param>
        /// <param name="Data">The request/response data.</param>
        /// <returns></returns>
        public virtual bool Close(TransportContext context, SecurityManagerJob job, RQRSContract<TransportCommandRequest, TransportCommandResponse> Data)
        {
            context.ChangeState("Close");
            return context.Close(job, Data);
        }
        /// <summary>
        /// This base method switches any close request to the Close State.
        /// </summary>
        /// <param name="context">The current context.</param>
        public virtual void Close(TransportContext context)
        {
            context.ChangeState("Close");
            context.Close();
        }
        #endregion // Close

    }
}
