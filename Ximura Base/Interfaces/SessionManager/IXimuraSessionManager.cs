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
namespace Ximura.Server
{
    /// <summary>
    /// The session create remote delegate.
    /// </summary>
    public delegate IXimuraSession SessionCreateRemote(string domain, string username);

    #region IXimuraSessionManager
    /// <summary>
    /// The IXimuraSessionManager interface is the public face of the session manager and is registered
    /// in the components collection of the Application.
    /// </summary>
    public interface IXimuraSessionManager
    {
        /// <summary>
        /// This method creates a blank session.
        /// </summary>
        /// <returns>A IXimuraSession object is returned.</returns>
        IXimuraSession SessionCreate();
        /// <summary>
        /// This method creates a session object for the named user.
        /// </summary>
        /// <param name="username">The user ID of the user.</param>
        /// <returns>A IXimuraSession object is returned.</returns>
        IXimuraSession SessionCreate(string username);
        /// <summary>
        /// This method creates a session object for the named user.
        /// </summary>
        /// <param name="domain">The domain for the session or null if the default is to be used.</param>
        /// <param name="username">The user ID of the user.</param>
        /// <returns>A IXimuraSession object is returned.</returns>
        IXimuraSession SessionCreate(string domain, string username);
    }
    #endregion // IXimuraSessionManager
}