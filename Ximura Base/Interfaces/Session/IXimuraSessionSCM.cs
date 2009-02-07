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

using Ximura;
using Ximura.Data;
using Ximura.Server;

#endregion // using
namespace Ximura
{
    /// <summary>
    /// The IXimuraSessionSM interface is the primary interface implemented
    /// by the session object, and is refernced by the Security Manager
    /// </summary>
    public interface IXimuraSessionSCM : IXimuraSession
    {
        /// <summary>
        /// This property allows the Security Manager to pass the session it's
        /// public key.
        /// </summary>
        RSAParameters SetSCMPublicKey { set; }
        /// <summary>
        /// This method allows the Session Token to get the session's
        /// public key.
        /// </summary>
        RSAParameters GetSessionPublicKey { get; }
    }
}
