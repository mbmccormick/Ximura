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
using System.Configuration;
#endregion // using
namespace Ximura.Server
{
    public interface IXimuraSessionManagerAccess
    {
        /// <summary>
        /// This method registers a session with the security manager.
        /// </summary>
        /// <param name="theSession">The session object.</param>
        void SessionRegister(IXimuraSessionSCM theSession);
        /// <summary>
        /// This method releases a session.
        /// </summary>
        /// <param name="theSession">The session object.</param>
        void SessionRelease(IXimuraSessionSCM theSession);
        /// <summary>
        /// This method informs the security manager that the session security profile
        /// has changed.
        /// </summary>
        /// <param name="theSession">The session object.</param>
        /// <param name="theLevel">The session profile level.</param>
        void SessionProfileChanged(IXimuraSessionSCM theSession, SessionProfileLevel theLevel);
    }
}
