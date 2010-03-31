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
using System.Security;
using System.Security.Principal;
using System.Security.Cryptography;

using Ximura;
using Ximura.Data;
using Ximura.Framework;

#endregion // using
namespace Ximura
{
    /// <summary>
    /// This interface is used to identify a session.
    /// </summary>
    public interface IXimuraSessionClient: IIdentity
    {
        /// <summary>
        /// This is the session ID
        /// </summary>
        Guid SessionID { get;}
        /// <summary>
        /// The session culture.
        /// </summary>
        CultureInfo SessionCulture { get;set;}
        /// <summary>
        /// this is the current session state
        /// </summary>
        SessionState State { get;}
    }
}
