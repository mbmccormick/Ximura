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
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Runtime.Serialization;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Drawing;
using System.Security;
using System.Security.Cryptography;
using System.Security.Principal;

using Ximura;

using Ximura.Framework;
using Ximura.Framework;
#endregion // using
namespace Ximura.Framework
{
    /// <summary>
    /// The session manager is the default session manager that is responsible for system session management. 
    /// </summary>
    [XimuraAppModule("FCCD39C8-CFD3-4441-8584-4A569D445914", "SessionManagerStandard")]
    public class SessionAgentStandard : SessionAgentBase<SessionStandard, SessionAgentStandardConfiguration>
    {
        #region Constructor
        /// <summary>
        /// This is the constructor used by the Ximura Application model.
        /// </summary>
        /// <param name="container">The control container.</param>
        public SessionAgentStandard(IContainer container): base(container)
        {
        }
        #endregion


        public override SessionStandard SessionCreate(string domain, string userID)
        {
            throw new NotImplementedException();
        }
    }
}
