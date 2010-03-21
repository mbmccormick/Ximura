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
using System.Collections;
using System.ComponentModel;
using System.Configuration;
#endregion // using
namespace Ximura
{
    /// <summary>
    /// This is the default interface for an application config section handler
    /// </summary>
    public interface IXimuraCommandProcessConfigSH : IXimuraConfigSH
    {
        /// <summary>
        /// The username of the process.
        /// </summary>
        string UserName { get;}
        /// <summary>
        /// This is the realm for the user to connect to.
        /// </summary>
        string UserRealm { get;}
        /// <summary>
        /// This is the realm for the user to connect to.
        /// </summary>
        string UserSessionRealm { get;}
        /// <summary>
        /// Returns the hash of the password based on the session seed value
        /// </summary>
        /// <param name="seed">The session seed.</param>
        /// <returns>The hash of the password.</returns>
        byte[] getPasswordHash(byte[] seed);
        /// <summary>
        /// This method determines whether the process command requires a process session.
        /// </summary>
        bool RequiresProcessSession { get;}
        /// <summary>
        /// Returns the security data.
        /// </summary>
        string SecurityData { get; }
    }
}
