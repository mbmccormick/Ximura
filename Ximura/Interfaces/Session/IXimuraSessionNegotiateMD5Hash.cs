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
using Ximura.Server;
#endregion // using
namespace Ximura
{
    /// <summary>
    /// This interface is used for MD5 authentication.
    /// </summary>
    public interface IXimuraSessionNegotiateMD5Hash : IXimuraSessionNegotiate
    {
        /// <summary>
        /// This method returns the seed for the current username.
        /// </summary>
        /// <returns>The seed as a byte array</returns>
        byte[] GetSeed();
        /// <summary>
        /// This method returns the seed for the specified username.
        /// </summary>
        /// <param name="Username">The username required</param>
        /// <returns>The seed as a byte array</returns>
        byte[] GetSeed(string Username);
        /// <summary>
        /// This method returns the seed for the specified username.
        /// </summary>
        /// <param name="Username">The username required</param>
        /// <param name="newCulture">The culture for the session.</param>
        /// <returns>The seed as a byte array</returns>
        byte[] GetSeed(string Username, CultureInfo newCulture);
        /// <summary>
        /// This method will attempt to authenticate the session.
        /// </summary>
        /// <param name="hash">The MD5 hash of the username and the seed</param>
        /// <returns>The session state after the authtication attempt</returns>
        SessionState Authenticate(byte[] hash);
        /// <summary>
        /// This method will attempt to authenticate the session.
        /// </summary>
        /// <param name="hash">The MD5 hash of the username and the seed</param>
        /// <param name="sessionCulture">The session culture</param>
        /// <returns>The session state after the authtication attempt</returns>
        SessionState Authenticate(byte[] hash, CultureInfo sessionCulture);

    }
}
