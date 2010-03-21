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
    /// This method allows control services to add services to the command bridge.
    /// </summary>
    public interface IXimuraCommandBridgeService
    {
        /// <summary>
        /// This method registers a service with the command container.
        /// </summary>
        /// <param name="service">The service type to add.</param>
        /// <param name="serviceobj">The service object.</param>
        /// <returns>Returns true is the service was successfully inserted.</returns>
        bool CommandBridgeServiceAdd(Type service, object serviceobj);
        /// <summary>
        /// This method registers a service with the command container.
        /// </summary>
        /// <typeparam name="T">The service type.</typeparam>
        /// <param name="serviceobj">The service object.</param>
        /// <returns>Returns true is the service was successfully inserted.</returns>
        bool CommandBridgeServiceAdd<T>(T serviceobj);
        /// <summary>
        /// This method returns a registered service from the command container.
        /// </summary>
        /// <param name="service">The service</param>
        /// <returns>Returns the service object or null if the service does not exist.</returns>
        object CommandBridgeServiceGet(Type service);
        /// <summary>
        /// This method returns a registered service from the command container.
        /// </summary>
        /// <typeparam name="T">The service type.</typeparam>
        /// <returns>Returns the service object or null if the service does not exist.</returns>
        T CommandBridgeServiceGet<T>();
        /// <summary>
        /// This method will remove a registered service from the command container.
        /// </summary>
        /// <param name="service">The service to remove.</param>
        /// <returns>Returns true is the service was successfully removed.</returns>
        bool CommandBridgeServiceRemove(Type service);
        /// <summary>
        /// This method will remove a registered service from the command container.
        /// </summary>
        /// <typeparam name="T">The service type.</typeparam>
        /// <returns>Returns true is the service was successfully removed.</returns>
        bool CommandBridgeServiceRemove<T>();
    }
}
