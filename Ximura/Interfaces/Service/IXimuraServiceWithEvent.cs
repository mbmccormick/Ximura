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
namespace Ximura.Helper
{
    /// <summary>
    /// This delegate is used by the Services to report back progress
    /// </summary>
    public delegate void ServiceEvent(object sender, ServiceEventArgs e);
    /// <summary>
    /// This interface is used by Ximura Service based component that wish to fire events for 
    /// service actions
    /// </summary>
    public interface IXimuraServiceWithEvent : IXimuraService
    {
        /// <summary>
        /// This event will be fired when the service starts
        /// </summary>
        event ServiceEvent ServiceStarted;
        /// <summary>
        /// This event will be fired when the service pauses
        /// </summary>
        event ServiceEvent ServicePaused;
        /// <summary>
        /// This event will be fired when the service is resumed from a paused state
        /// </summary>
        event ServiceEvent ServiceResumed;
        /// <summary>
        /// This event will be fired when the service is stopped
        /// </summary>
        event ServiceEvent ServiceStopped;
    }
}
