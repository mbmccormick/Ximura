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
using System.Security.Cryptography;
using System.Collections.Generic;

using Ximura;
using Ximura.Data;
using Ximura.Framework;
#endregion // using
namespace Ximura
{
    /// <summary>
    /// This method is used to authenticate a user using plain text.
    /// </summary>
    public interface IXimuraSessionNegotiatePlainText : IXimuraSessionNegotiate
    {
        /// <summary>
        /// This method extends the basic session functionality and
        /// authenticates a user based on their plain text username and
        /// password.
        /// </summary>
        /// <param name="password">The password.</param>
        SessionState Authenticate(string password);

        /// <summary>
        /// This method extends the basic session functionality and
        /// authenticates a user based on their plain text username and
        /// password.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        SessionState Authenticate(string username, string password);
    }
}
