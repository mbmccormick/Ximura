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
using System.Globalization;
using System.Security.Cryptography;
using System.Collections.Generic;

using Ximura;
using Ximura.Data;
using Ximura.Server;
#endregion // using
namespace Ximura
{
    /// <summary>
    /// This interface is used to negotiate an authenticated session.
    /// </summary>
    public interface IXimuraSessionNegotiate
    {
        /// <summary>
        /// This method specifies whether the user is able to change their password.
        /// </summary>
        /// <returns>Returns true is the user is able to change their password.</returns>
        bool PasswordChangeSupported();
        /// <summary>
        /// The session realm.
        /// </summary>
        string Realm { get;}

        void Logout();
    }
}
