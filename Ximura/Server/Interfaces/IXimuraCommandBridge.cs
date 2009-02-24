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
using System.ComponentModel.Design;

using Ximura;
using Ximura.Helper;
#endregion // using
namespace Ximura.Server
{
	/// <summary>
	/// This interface is registered as a service and is used by application 
	/// commands to communicate with the Application Dispatcher
	/// </summary>
	public interface IXimuraCommandBridge
	{
        ///// <summary>
        ///// This method is used to cancel a registered callback with the security manager.
        ///// </summary>
        ///// <param name="callbackID"></param>
        ///// <returns></returns>
        //bool CancelCallback(Guid callbackID);
		/// <summary>
		/// This method is used to register a command in the command collection.
		/// </summary>
		/// <param name="command">The command to register.</param>
		/// <returns>Returns true if the command is registered successfully.</returns>
		bool Register(IXimuraCommandRQ command);
		/// <summary>
		/// This method is used to unregister a command.
		/// </summary>
		/// <param name="command">The commands to remove.</param>
		/// <returns>Returns true if the command is successfully removed
		/// from the collection. Returns false if the command was not a member
		/// of the collection.</returns>
		bool Unregister(IXimuraCommandRQ command);
        ///// <summary>
        ///// This method registers a known service with the command bridge,
        ///// </summary>
        ///// <param name="serviceType">The service type.</param>
        ///// <param name="service">The service object.</param>
        ///// <returns>Returns true if the service was registered successfully.</returns>
        //bool RegisterCommandKnownService(Type serviceType, object service);
        ///// <summary>
        ///// This method unregisters a known service with the command bridge.
        ///// </summary>
        ///// <param name="serviceType">The service type to unregister.</param>
        ///// <returns>Returns true if the service can be unregistered successfully.</returns>
        //bool UnregisterCommandKnownService(Type serviceType);
        ///// <summary>
        ///// This method retrieves a known service from the service container.
        ///// </summary>
        ///// <param name="serviceType"></param>
        ///// <returns></returns>
        //object GetCommandKnownService(Type serviceType);
	}
}
