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
using Ximura.Framework;

#endregion // using
namespace Ximura
{
    /// <summary>
    /// The IXimuraSession interface is exposed to the user.
    /// </summary>
    public interface IXimuraSession : IXimuraSessionClient, IXimuraSessionRQ
    {
        /// <summary>
        /// This enumeration returns the supported authentication interfaces.
        /// </summary>
        IEnumerable<Type> AuthenticationTypes { get; }
        /// <summary>
        /// This method closes the session and cancals any pending job requests.
        /// </summary>
        void Close();
    }
}