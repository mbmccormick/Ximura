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
using System.Collections;
using System.Diagnostics;
using System.Data;
using System.Reflection;
using System.ComponentModel;
using System.Collections.Generic;

using Ximura;
using Ximura.Data;
using CH = Ximura.Common;

using Ximura.Framework;


using Ximura.Framework;
#endregion // using
namespace Ximura.Communication
{
    public interface IXimuraServerState<RQ, RS, CBRQ, CBRS, CONF, PERF> : IXimuraFSMState
        where RQ : RQServer, new()
        where RS : RSServer, new()
        where CBRQ : RQCallbackServer, new()
        where CBRS : RSCallbackServer, new()
        where CONF : FSMCommandConfiguration, new()
        where PERF : FSMCommandPerformance, new()
    {
        void Initialize(IXimuraServerContext<RQ, RS, CBRQ, CBRS, CONF, PERF> context);
        bool ConnectionRequest(IXimuraServerContext<RQ, RS, CBRQ, CBRS, CONF, PERF> context, SecurityManagerJob job, RQRSContract<CBRQ, CBRS> Data);
        bool Receive(IXimuraServerContext<RQ, RS, CBRQ, CBRS, CONF, PERF> context, SecurityManagerJob job, RQRSContract<CBRQ, CBRS> Data);
        bool Close(IXimuraServerContext<RQ, RS, CBRQ, CBRS, CONF, PERF> context, SecurityManagerJob job, RQRSContract<CBRQ, CBRS> Data);
        bool Close(IXimuraServerContext<RQ, RS, CBRQ, CBRS, CONF, PERF> context);

        bool Transmit(IXimuraServerContext<RQ, RS, CBRQ, CBRS, CONF, PERF> context, 
            ProtocolConnectionIdentifiers identifier, IXimuraMessageStream messageOut, bool SignalClose);
    }
}
