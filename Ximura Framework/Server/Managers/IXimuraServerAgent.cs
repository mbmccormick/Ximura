﻿#region Copyright
//*******************************************************************************
//Copyright (c) 2000-2009 Paul Stancer.
//All rights reserved. This program and the accompanying materials
//are made available under the terms of the Eclipse Public License v1.0
//which accompanies this distribution, and is available at
//http://www.eclipse.org/legal/epl-v10.html

//Contributors:
//    Paul Stancer - initial implementation
//*******************************************************************************
#endregion
#region using
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Threading;
using System.Reflection;
using System.Security.Cryptography;
using System.Linq;

using Ximura;
using Ximura.Data;
using Ximura.Helper;
using Ximura.Server;
using Ximura.Command;
using AH = Ximura.Helper.AttributeHelper;
using RH = Ximura.Helper.Reflection;
using CH = Ximura.Helper.Common;
#endregion // using
namespace Ximura.Server
{
    /// <summary>
    /// This is the base interface implemented by all server agents.
    /// </summary>
    public interface IXimuraServerAgent
    {
    }
}
