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
using Ximura.Server;


using Ximura.Command;
#endregion
namespace Ximura
{
    public interface IXimuraJobContext<RQ, RS, CONF, PERF> : IXimuraFSMContext
        where RQ : RQRSFolder, new()
        where RS : RQRSFolder, new()
        where CONF : FSMCommandConfiguration, new()
        where PERF : FSMCommandPerformance, new()
    {
        void Reset(IXimuraFSMSettingsBase fsm, SecurityManagerJob job,
            RQRSContract<RQ, RS> data, IXimuraFSMContextPoolAccess contextGet);

        RQRSContract<RQ, RS> Data { get;}
        RQ Request { get;}
        RS Response { get;}
        JobPriority ChildJobPriority { get;}
        EnvelopeAddress Destination { get;}
        SecurityManagerJob Job { get;set;}
    }
}
