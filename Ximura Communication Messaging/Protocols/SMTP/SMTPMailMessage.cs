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
using System.Text;
using System.Timers;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Reflection;

using System.Diagnostics;

using Ximura;
using Ximura.Helper;
using Ximura.Data;

using Ximura.Server;
using Ximura.Command;
#endregion // using
namespace Ximura.Communication
{
    public class SMTPMailMessage: InternetMessage
    {
    }
}
