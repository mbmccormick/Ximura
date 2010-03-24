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
using System.Configuration;
using System.Collections.Specialized;

using Ximura;
using Ximura.Data;
#endregion // using
namespace Ximura.Framework
{
    /// <summary>
    /// IXimuraSessionManagerAuth
    /// </summary>
    public interface IXimuraSessionManagerAuth
    {
        /// <summary>
        /// This method logs out a session and resets its permission to an
        /// unathorized user.
        /// </summary>
        /// <param name="theSession">The session to logout.</param>
        void Logout(IXimuraSessionSCM theSession);
        /// <summary>
        /// This method closes a session and removes it from the session collection.
        /// </summary>
        /// <param name="theSession">The session object to close.</param>
        void Close(IXimuraSessionSCM theSession);
        /// <summary>
        /// This method gets the seed for the user.
        /// </summary>
        /// <param name="theSession">The session.</param>
        /// <returns>Returns a byte array containing the seed.</returns>
        byte[] GetSeed(IXimuraSessionSCM theSession);
        /// <summary>
        /// This is the one time parameter used to prevent replay attacks. This method
        /// is not currently implemented.
        /// </summary>
        /// <param name="theSession">The session to generate the nonce for.</param>
        /// <returns>A byte array containing the nonce.</returns>
        byte[] GetNonce(IXimuraSessionSCM theSession);
        /// <summary>
        /// This method will authenticate the user.
        /// </summary>
        /// <param name="theSession">The session.</param>
        /// <param name="hash">The hash.</param>
        /// <returns>The session state after the authentication request.</returns>
        SessionState Authenticate(IXimuraSessionSCM theSession, byte[] hash);
    }
}
