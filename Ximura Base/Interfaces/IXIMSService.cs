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

using XIMS;
#endregion // using
namespace XIMS.Helper
{
	/// <summary>
	/// This delegate is used by the Services to report back progress
	/// </summary>
	public delegate void ServiceEvent(object sender, ServiceEventArgs e);
	/// <summary>
	/// This interface is used by XIMS Service based component that wish to fire events for 
	/// service actions
	/// </summary>
	public interface IXIMSServiceWithEvent:IXIMSService
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
	/// <summary>
	/// This interface is used by XIMS Service based components.
	/// </summary>
	public interface IXIMSService
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
		XIMSServiceStatus ServiceStatus{get;}
        /// <summary>
        /// This property determines whether the component is enable and can start.
        /// </summary>
        bool ServiceEnabled {get;set;}
	}
    /// <summary>
    /// This interface is implemented by commands that can be nested in other commands.
    /// </summary>
    public interface IXIMSServiceParentSettings
    {
        /// <summary>
        /// The parent command service name.
        /// </summary>
        string ParentCommandName { get;set;}
    }
}