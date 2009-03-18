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
using System.Threading;

using Ximura;
#endregion // using
namespace Ximura
{
    /// <summary>
    /// This interface is used by Ximura Service based components.
    /// </summary>
    public interface IXimuraService
    {
        /// <summary>
        /// This method starts the service
        /// </summary>
        void Start();
        /// <summary>
        /// This method pauses the service
        /// </summary>
        void Pause();
        /// <summary>
        /// This method continues a paused service
        /// </summary>
        void Continue();
        /// <summary>
        /// This method stops a running service
        /// </summary>
        void Stop();
        /// <summary>
        /// This method will return the current service status
        /// </summary>
        XimuraServiceStatus ServiceStatus {get;}
        /// <summary>
        /// This property determines whether the component is enable and can start.
        /// </summary>
        bool ServiceEnabled {get;set;}
    }
}
