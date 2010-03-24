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
using System.Globalization;
using System.Security;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Security.Policy;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Diagnostics;

using Ximura;
using Ximura.Framework;
using Ximura.Framework;

using CH = Ximura.Common;
#endregion // using
namespace Ximura.Framework
{
    public class SessionStandard: SessionBase
    {
        internal SessionStandard(SessionAccessToken sat) : base(sat) { }


    }
}
