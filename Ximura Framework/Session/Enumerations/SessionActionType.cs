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

using Ximura.Framework;

using Ximura.Framework;
#endregion // using
namespace Ximura.Framework
{
    /// <summary>
    /// The session action type enumeration is used to mediate communication between the session managers and the security manager.
    /// </summary>
    public enum SessionActionType
    {
        /// <summary>
        /// A new session has been created.
        /// </summary>
        Register,
        /// <summary>
        /// A session is being released.
        /// </summary>
        Release,
        /// <summary>
        /// A session profile has changed, i.e. they have logged in or logged out.
        /// </summary>
        ProfileChanged
    }
}
