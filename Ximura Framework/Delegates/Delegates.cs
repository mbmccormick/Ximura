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
using System.ComponentModel;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Security;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Security.Permissions;

using Ximura;
using Ximura.Helper;
using Ximura.Framework;

using Ximura.Framework;
#endregion // using
namespace Ximura.Framework
{
    public delegate void SMJobCancel(Guid jobID);

    public delegate Guid SMJobProcess(JobBase job, bool async);

    public delegate void SMJobComplete(SecurityManagerJob jobRS, bool signal);


    public delegate void SessionJobReturn(SessionJob job);

    public delegate J JobGet<J>(Guid sessionID, Guid jobID, IXimuraRQRSEnvelope data,
        CommandRSCallback RSCallback, CommandProgressCallback ProgressCallback,
            JobSignature? signature, JobPriority? priority);

    internal delegate void SessionAction(SessionActionType type, IXimuraSessionManager sessionManager, 
        IXimuraSessionSCM session, object data);

}
