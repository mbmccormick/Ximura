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
using System.Collections.Specialized;
using System.Runtime.Serialization;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Threading;
using System.Reflection;

using Ximura;

#endregion // using
namespace Ximura
{
    /// <summary>
    /// This is the base interface supported by components that support the Ximura component architecture.
    /// </summary>
    public interface IXimuraComponentBase : IXimuraServiceContainer, IComponent, IDisposable
    {

    }
    /// <summary>
    /// This interface provides extended functionality for the service based architecture.
    /// </summary>
    public interface IXimuraServiceContainer : IServiceContainer
    {
        /// <summary>
        /// Adds the service specified to the service container.
        /// </summary>
        /// <param name="serviceType">The type of service to add.</param>
        /// <param name="serviceInstance">The service.</param>
        /// <param name="promote">True promotes this service to the parent service container.</param>
        /// <param name="depth">The depth the service should be promoted. Set this value to -1 if you require it to be unlimited.</param>
        void AddService(Type serviceType, object serviceInstance, bool promote, int depth);

        /// <summary>
        /// This method returns the requested service.
        /// </summary>
        /// <param name="serviceType">The service type.</param>
        /// <param name="depth">The service depth. If this is set to -1 the depth is unlimited.</param>
        /// <returns>Returns the service requested, or null if the service cannot be found.</returns>
        object GetService(Type serviceType, int depth);
    }
}
