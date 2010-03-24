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
using CH = Ximura.Helper.Common;
using Ximura.Helper;
using Ximura.Framework;


using Ximura.Framework;
#endregion // using
namespace Ximura.Communication
{
    public interface IXimuraServerContext<RQ, RS, CBRQ, CBRS, CONF, PERF> : IXimuraFSMContext
        where RQ : RQServer, new()
        where RS : RSServer, new()
        where CBRQ : RQCallbackServer, new()
        where CBRS : RSCallbackServer, new()
        where CONF : FSMCommandConfiguration, new()
        where PERF : FSMCommandPerformance, new()
    {
        void Initialize();
        bool ConnectionRequest(SecurityManagerJob job, RQRSContract<CBRQ, CBRS> Data);
        bool Receive(SecurityManagerJob job, RQRSContract<CBRQ, CBRS> Data);
        bool Close(SecurityManagerJob job, RQRSContract<CBRQ, CBRS> Data);
        bool Close();

        ProtocolConnectionIdentifiers ProtocolConnectionDefault { get;}

        void Transmit(IXimuraMessageStream messageOut, bool SignalClose);
        void Transmit(ProtocolConnectionIdentifiers identifier, IXimuraMessageStream messageOut, bool SignalClose);

        bool VerifyIncomingUri(Uri location);

        void SetResponse(RQRSContract<CBRQ, CBRS> Data, int messageID);
        void SetResponse(RQRSContract<CBRQ, CBRS> Data, int messageID, Type requestType);
        void SetResponse(RQRSContract<CBRQ, CBRS> Data, int messageID, params object[] list);
        void SetResponse(RQRSContract<CBRQ, CBRS> Data, string[] multiLine, int messageID);
        void SetResponse(RQRSContract<CBRQ, CBRS> Data, string overrideLine, int messageID, params object[] list);
        void SetResponse(RQRSContract<CBRQ, CBRS> Data, string overrideLine, int messageID, Type requestType, params object[] list);
        void SetResponse(string response, RQRSContract<CBRQ, CBRS> Data, Type requestType, int requestMaxLength);

        IXimuraSessionRQ ProcessSession { get;}
    }
}
