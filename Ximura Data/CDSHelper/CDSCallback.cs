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
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Security;
using System.Security.Cryptography;
using System.Globalization;
using System.Runtime.Serialization;

using Ximura;
using Ximura.Helper;
using CH = Ximura.Helper.Common;
using RH = Ximura.Helper.Reflection;
using AH = Ximura.Helper.AttributeHelper;
using Ximura.Data;
using Ximura.Server;
using Ximura.Command;


#endregion // using
namespace Ximura.Persistence
{
    /// <summary>
    /// This delegate is used by the DCSWrapper when processing an async request.
    /// </summary>
    public delegate void CDSCallback(object state, Guid jobID,
        string response, Content content);

    /// <summary>
    /// This delegate is used by the DCSWrapper when processing an async request.
    /// </summary>
    public delegate void CDSCallback<T>(object state, Guid jobID,
        string response, T content);
}
