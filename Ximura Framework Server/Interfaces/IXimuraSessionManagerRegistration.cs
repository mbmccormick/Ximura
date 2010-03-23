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
    /// <summary>
	/// IXimuraSecurityManager is the public interface that the security manager exposes
	/// to any session managers, and is used to negotiate sessions.
	/// </summary>
    public interface IXimuraSessionManagerRegistration : IXimuraAppServerAgentService, IXimuraService
	{
	}
}