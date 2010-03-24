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
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Threading;
using System.Reflection;

using Ximura;
using Ximura.Data;
using Ximura.Helper;
using CH = Ximura.Helper.Common;
using Ximura.Framework;
using Ximura.Framework;
#endregion // using
namespace Ximura
{
    /// <summary>
    /// This helper provides service based functionality for the components that require it.
    /// </summary>
    public class XimuraComponentBaseHelper : IXimuraServiceContainer
    {
        #region Declarations
        /// <summary>
        /// The service collection.
        /// </summary>
        private Dictionary<Type,object> mServices;
        /// <summary>
        /// currentType is used to identify the parent during debegging time.
        /// </summary>
        private Type currentType;
        private ISite mSite = null;
        #endregion // Declarations

        #region Constructor
        /// <summary>
        /// The default constructor.
        /// </summary>
        public XimuraComponentBaseHelper(Type currentType)
        {
            this.currentType = currentType;
        }
        #endregion // ServiceHelper

        #region ServiceContainer
        private IServiceContainer ServiceContainer
        {
            get
            {
                IServiceContainer parentContainer = null;
                if (this.Site != null)
                {
                    parentContainer = (IServiceContainer)this.Site.GetService(typeof(IServiceContainer));
                }
                return parentContainer;
            }
        }
        #endregion // ServiceContainer

        #region RemoveService
        /// <summary>
        /// Removes the specified service type from the service container.
        /// </summary>
        /// <typeparam name="T">The type of service to remove.</typeparam>
        public void RemoveService<T>()
        {
            RemoveService(typeof(T), false);
        }
        /// <summary>
        /// Removes the specified service type from the service container.
        /// </summary>
        /// <typeparam name="T">The type of service to remove.</typeparam>
        /// <param name="promote">Should be set to true if this service should be removed 
        /// from any parent service containers.</param>
        public void RemoveService<T>(bool promote)
        {
            RemoveService(typeof(T), promote);
        }
        /// <summary>
        /// Removes the specified service type from the service container.
        /// </summary>
        /// <param name="serviceType">The type of service to remove.</param>
        public void RemoveService(Type serviceType)
        {
            RemoveService(serviceType, false);
        }
        /// <summary>
        /// Removes the specified service type from the service container.
        /// </summary>
        /// <param name="serviceType">The type of service to remove.</param>
        /// <param name="promote">Should be set to true if this service should be removed 
        /// from any parent service containers.</param>
        public void RemoveService(Type serviceType, bool promote)
        {
            if (promote)
            {
                IServiceContainer sc = this.ServiceContainer;
                if (sc != null)
                {
                    sc.RemoveService(serviceType, promote);
                    return;
                }
            }
            if (serviceType == null)
            {
                throw new ArgumentNullException("The serviceType cannot be null.");
            }
            this.Services.Remove(serviceType);
        }
        #endregion // RemoveService
        #region AddService
        /// <summary>
        /// Adds the service to the service container.
        /// </summary>
        /// <typeparam name="T">The type of service to add.</typeparam>
        /// <param name="callback">A callback delegate the create the service.</param>
        public void AddService<T>(ServiceCreatorCallback callback)
        {
            AddService(typeof(T), callback, false, -1);
        }
        /// <summary>
        /// Adds the service to the service container.
        /// </summary>
        /// <param name="serviceType">The type of service to add.</param>
        /// <param name="callback">A callback delegate the create the service.</param>
        public void AddService(Type serviceType, ServiceCreatorCallback callback)
        {
            AddService(serviceType, callback, false, -1);
        }

        /// <summary>
        /// Adds the service specified to the service container.
        /// </summary>
        /// <typeparam name="T">The type of service to add.</typeparam>
        /// <param name="serviceInstance">The service.</param>
        public void AddService<T>(T serviceInstance)
        {
            AddService(typeof(T), serviceInstance, false, -1);
        }
        /// <summary>
        /// Adds the service specified to the service container.
        /// </summary>
        /// <param name="serviceType">The type of service to add.</param>
        /// <param name="serviceInstance">The service.</param>
        public void AddService(Type serviceType, object serviceInstance)
        {
            AddService(serviceType, serviceInstance, false, -1);
        }

        /// <summary>
        /// Adds the service specified to the service container.
        /// </summary>
        /// <typeparam name="T">The type of service to add.</typeparam>
        /// <param name="callback">A callback delegate the create the service.</param>
        /// <param name="promote">True promotes this service to the parent service container.</param>
        public void AddService<T>(ServiceCreatorCallback callback, bool promote)
        {
            AddService(typeof(T), callback, promote, -1);
        }
        /// <summary>
        /// Adds the service specified to the service container.
        /// </summary>
        /// <param name="serviceType">The type of service to add.</param>
        /// <param name="callback">A callback delegate the create the service.</param>
        /// <param name="promote">True promotes this service to the parent service container.</param>
        public void AddService(Type serviceType, ServiceCreatorCallback callback, bool promote)
        {
            AddService(serviceType, callback, promote, -1);
        }

        /// <summary>
        /// Adds the service specified to the service container.
        /// </summary>
        /// <typeparam name="T">The type of service to add.</typeparam>
        /// <param name="callback">A callback delegate the create the service.</param>
        /// <param name="promote">True promotes this service to the parent service container.</param>
        /// <param name="depth">The depth to promote the service. If this is set to -1 the depth in unlimited.</param>
        public void AddService<T>(ServiceCreatorCallback callback, bool promote, int depth)
        {
            AddService(typeof(T), callback, promote, depth);
        }
        /// <summary>
        /// Adds the service specified to the service container.
        /// </summary>
        /// <param name="serviceType">The type of service to add.</param>
        /// <param name="callback">A callback delegate the create the service.</param>
        /// <param name="promote">True promotes this service to the parent service container.</param>
        /// <param name="depth">The depth to promote the service. If this is set to -1 the depth in unlimited.</param>
        public void AddService(Type serviceType, ServiceCreatorCallback callback, bool promote, int depth)
        {
            if (promote && depth != 0)
            {
                IServiceContainer sc = ServiceContainer;
                if (sc != null)
                {
                    if (depth != -1 && sc is IXimuraServiceContainer)
                        ((IXimuraServiceContainer)sc).AddService(serviceType, callback, promote, --depth);
                    else
                        sc.AddService(serviceType, callback, promote);
                    return;
                }
            }
            if (serviceType == null)
            {
                throw new ArgumentNullException("The serviceType cannot be null.");
            }
            if (callback == null)
            {
                throw new ArgumentNullException("The callback cannot be null.");
            }
            if (this.Services.ContainsKey(serviceType))
            {
                throw new ArgumentException("The service type already exists: " + serviceType.FullName, "serviceType");
            }
            this.Services[serviceType] = callback;
        }
        /// <summary>
        /// Adds the service specified to the service container.
        /// </summary>
        /// <typeparam name="T">The type of service to add.</param>
        /// <param name="serviceInstance">The service.</param>
        /// <param name="promote">True promotes this service to the parent service container.</param>
        public void AddService<T>(T serviceInstance, bool promote)
        {
            AddService(typeof(T), serviceInstance, promote, -1);
        }
        /// <summary>
        /// Adds the service specified to the service container.
        /// </summary>
        /// <param name="serviceType">The type of service to add.</param>
        /// <param name="serviceInstance">The service.</param>
        /// <param name="promote">True promotes this service to the parent service container.</param>
        public void AddService(Type serviceType, object serviceInstance, bool promote)
        {
            AddService(serviceType, serviceInstance, promote, -1);
        }

        /// <summary>
        /// Adds the service specified to the service container.
        /// </summary>
        /// <typeparam name="T">The type of service to add.</param>
        /// <param name="serviceInstance">The service.</param>
        /// <param name="promote">True promotes this service to the parent service container.</param>
        /// <param name="depth">The depth the service should be promoted. Set this value to -1 if you require it to be unlimited.</param>
        public void AddService<T>(T serviceInstance, bool promote, int depth)
        {
            AddService(typeof(T), serviceInstance, promote, depth);
        }
        /// <summary>
        /// Adds the service specified to the service container.
        /// </summary>
        /// <param name="serviceType">The type of service to add.</param>
        /// <param name="serviceInstance">The service.</param>
        /// <param name="promote">True promotes this service to the parent service container.</param>
        /// <param name="depth">The depth the service should be promoted. Set this value to -1 if you require it to be unlimited.</param>
        public void AddService(Type serviceType, object serviceInstance, bool promote, int depth)
        {
            if (promote && depth != 0)
            {
                IServiceContainer sc = ServiceContainer;
                if (sc != null)
                {
                    if (depth != -1 && sc is IXimuraServiceContainer)
                        ((IXimuraServiceContainer)sc).AddService(serviceType, serviceInstance, promote, --depth);
                    else
                        sc.AddService(serviceType, serviceInstance, promote);
                    return;
                }
            }
            if (serviceType == null)
            {
                throw new ArgumentNullException("The serviceType cannot be null.");
            }
            if (serviceInstance == null)
            {
                throw new ArgumentNullException("The serviceInstance cannot be null.");
            }
            if ((!(serviceInstance is ServiceCreatorCallback) && !serviceInstance.GetType().IsCOMObject)
                && !serviceType.IsAssignableFrom(serviceInstance.GetType()))
            {
                throw new ArgumentException("ErrorInvalidServiceInstance: " + serviceType.FullName);
            }
            if (this.Services.ContainsKey(serviceType))
            {
                throw new ArgumentException("The service type already exists: " + serviceType.FullName, "serviceType");
            }
            this.Services[serviceType] = serviceInstance;
        }
        #endregion // AddService
        #region GetService
        /// <summary>
        /// This method returns the requested service.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetService<T>()
        {
            return GetService<T>(-1);
        }
        /// <summary>
        /// This method returns the requested service.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="depth">The service depth. If this is set to -1 the depth is unlimited.</param>
        /// <returns>Returns the service requested, or null if the service cannot be found.</returns>
        public T GetService<T>(int depth)
        {
            return (T)GetService(typeof(T), depth);
        }
        /// <summary>
        /// This method returns the requested service.
        /// </summary>
        /// <param name="serviceType">The service type.</param>
        /// <returns>Returns the service requested, or null if the service cannot be found.</returns>
        object IServiceProvider.GetService(Type serviceType)
        {
            return GetService(serviceType, -1);
        }
        /// <summary>
        /// This method returns the requested service.
        /// </summary>
        /// <param name="serviceType">The service type.</param>
        /// <param name="depth">The service depth. If this is set to -1 the depth is unlimited.</param>
        /// <returns>Returns the service requested, or null if the service cannot be found.</returns>
        public object GetService(Type serviceType, int depth)
        {
            if (serviceType == typeof(IServiceContainer))
            {
                return this;
            }

            //BUGFIX: Changes to default behaviour. Services would throw an exception if ContainsKay
            //was not called prior to accessing the service.
            object serviceObj = null;
            if (Services.ContainsKey(serviceType))
                serviceObj = Services[serviceType];

            if (serviceObj is ServiceCreatorCallback)
            {
                serviceObj = ((ServiceCreatorCallback)serviceObj)(this, serviceType);
                if (((serviceObj != null) &&
                    !serviceObj.GetType().IsCOMObject) && !serviceType.IsAssignableFrom(serviceObj.GetType()))
                {
                    serviceObj = null;
                }
                Services[serviceType] = serviceObj;
            }
            if ((serviceObj == null) && (ServiceContainer != null))
            {
                if (depth == -1 || !(ServiceContainer is IXimuraServiceContainer))
                    serviceObj = ServiceContainer.GetService(serviceType);
                else
                {
                    if (depth > 0)
                        serviceObj = ((IXimuraServiceContainer)ServiceContainer).GetService(serviceType, --depth);
                }
            }
            return serviceObj;
        }
        #endregion

        #region Services
        /// <summary>
        /// This hashtable contains the locally registered services.
        /// </summary>
        protected Dictionary<Type, object> Services
        {
            get
            {
                if (mServices == null)
                {
                    mServices = new Dictionary<Type,object>();
                }

                return mServices;
            }
        }
        #endregion // Services

        #region Site
        /// <summary>
        /// This override property calls the SiteChanged method when the site changes
        /// for the component.
        /// </summary>
        public virtual ISite Site
        {
            get
            {
                return mSite;
            }
            set
            {
                mSite = value;
            }
        }
        #endregion // Site

        #region ServiceScope structure
        /// <summary>
        /// The service scope structure holds a record of all the services
        /// registered to the component.
        /// </summary>
        private struct ServiceScope
        {
            public bool promote;
            public int depth;
            public Type serviceType;
            public object service;

            public ServiceScope(Type serviceType, object service, bool promote)
            {
                depth = -1;
                this.promote = promote;
                this.serviceType = serviceType;
                this.service = service;
            }

            public ServiceScope(Type serviceType, object service, bool promote, int depth)
                : this(serviceType, service, promote)
            {
                this.depth = depth;
            }
        }
        #endregion // ServiceScope

    }
}
