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
namespace Ximura.Server
{
    /// <summary>
    /// Summary description for IXimuraAppServer.
    /// </summary>
    public interface IXimuraAppServer : IXimuraApplication
    {
        ///// <summary>
        ///// This method is used to register a session manager.
        ///// </summary>
        //bool RegisterSessionManager { get;set;}
        ///// <summary>
        ///// This property informs the container when to start the 
        ///// application in a seperate domain.
        ///// </summary>
        //bool RequiresSeperateDomain { get;set;}
    }
}