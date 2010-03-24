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
using System.ComponentModel;
using System.Threading;
using System.Diagnostics;

using Ximura;
using Ximura.Framework;
using Ximura.Framework;
using Ximura.Helper;
using CH = Ximura.Helper.Common;
#endregion // using
namespace Ximura.Framework
{
    public class SessionAgentSystem : SessionAgentBase<SessionSystem, SessionAgentSystemConfiguration>
    {
        #region Constructor
		/// <summary>
		/// This is the constructor used by the Ximura Application model.
		/// </summary>
		/// <param name="container">The control container.</param>
        public SessionAgentSystem(IContainer container): base(container)
        {
        }
		#endregion

        public override SessionSystem SessionCreate(string domain, string userID)
        {
            throw new NotImplementedException();
        }
    }
}
