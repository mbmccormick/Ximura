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
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Reflection;
using System.Drawing;
using System.Diagnostics;

using Ximura;
using Ximura.Data;
using Ximura.Persistence;
using CH = Ximura.Helper.Common;
using Ximura.Server;

using Ximura.Command;

#endregion // using
namespace Ximura
{
    public interface IXimuraStateExtender
    {
    }

    public interface IXimuraStateExtender<ST> : IXimuraStateExtender
        where ST : class, IXimuraFSMState
    {

    }
}
