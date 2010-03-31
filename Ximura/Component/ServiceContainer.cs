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
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;

using Ximura;
#endregion
namespace Ximura
{
	/// <summary>
	/// Summary description for ServiceContainer.
	/// </summary>
    public class XimuraServiceContainer : IServiceContainer
	{
		#region Declarations
		private IServiceProvider mParentProvider;
		private Dictionary<Type,object> mServices = null;
        private object syncObject = new object();
		#endregion // Declarations
		#region Constructors
		/// <summary>
		/// The default constructor.
		/// </summary>
		public XimuraServiceContainer():this(null)
		{
		}
		/// <summary>
		/// The parent provider constructor.
		/// </summary>
		/// <param name="parentProvider">The parent service provider.</param>
		public XimuraServiceContainer(IServiceProvider parentProvider)
		{
			ParentProvider = parentProvider;
		}
		#endregion // Constructors

		#region ParentProvider
		/// <summary>
		/// This property sets the parent provider.
		/// </summary>
		public virtual IServiceProvider ParentProvider
		{
			get
			{
				return mParentProvider;
			}
			set
			{
				mParentProvider = value;
			}
		}
		#endregion // ParentProvider

		#region Container
		private IServiceContainer Container
		{
			get
			{
				IServiceContainer container1 = null;
				if (mParentProvider != null)
				{
					container1 = (IServiceContainer) mParentProvider.GetService(typeof(IServiceContainer));
				}
				return container1;
			}
		}
		#endregion // Container
		#region Services
        private Dictionary<Type, object> Services
		{
			get
			{
				if (mServices == null)
				{
                    lock (syncObject)
                    {
                        if (mServices == null)
                            mServices = new Dictionary<Type, object>();
                    }
				}
				return mServices;
			}
		}
		#endregion // Services
 
		#region RemoveService
		public void RemoveService(Type serviceType)
		{
			RemoveService(serviceType, false);
		}

		public void RemoveService(Type serviceType, bool promote)
		{
			if (promote)
			{
				IServiceContainer container1 = Container;
				if (container1 != null)
				{
					container1.RemoveService(serviceType, promote);
					return;
				}
			}

			if (serviceType == null)
			{
				throw new ArgumentNullException("serviceType");
			}

			Services.Remove(serviceType);
		}
		#endregion // RemoveService
		#region AddService
		/// <summary>
		/// Adds the service to the service container.
		/// </summary>
		/// <param name="serviceType">The type of service to add.</param>
		/// <param name="callback">A callback delegate the create the service.</param>
		public void AddService(Type serviceType, ServiceCreatorCallback callback)
		{
			AddService(serviceType, callback, false);
		}
		/// <summary>
		/// Adds the service specified to the service container.
		/// </summary>
		/// <param name="serviceType">The type of service to add.</param>
		/// <param name="serviceInstance">The service.</param>
		public void AddService(Type serviceType, object serviceInstance)
		{
			AddService(serviceType, serviceInstance, false);
		}
		/// <summary>
		/// Adds the service specified to the service container.
		/// </summary>
		/// <param name="serviceType">The type of service to add.</param>
		/// <param name="callback">A callback delegate the create the service.</param>
		/// <param name="promote">True promotes this service to the parent service container.</param>
		public void AddService(Type serviceType, ServiceCreatorCallback callback, bool promote)
		{
			if (promote)
			{
				IServiceContainer container1 = Container;
				if (container1 != null)
				{
					container1.AddService(serviceType, callback, promote);
					return;
				}
			}
			if (serviceType == null)
			{
				throw new ArgumentNullException("serviceType");
			}

			if (callback == null)
			{
				throw new ArgumentNullException("callback");
			}

			if (Services.ContainsKey(serviceType))
			{
				throw new ArgumentException("ErrorServiceExists: " + serviceType.FullName, "serviceType");
			}

			Services[serviceType] = callback;
		}
		/// <summary>
		/// Adds the service specified to the service container.
		/// </summary>
		/// <param name="serviceType">The type of service to add.</param>
		/// <param name="serviceInstance">The service.</param>
		/// <param name="promote">True promotes this service to the parent service container.</param>
		public void AddService(Type serviceType, object serviceInstance, bool promote)
		{
			if (promote)
			{
				IServiceContainer container1 = Container;

				if (container1 != null)
				{
					container1.AddService(serviceType, serviceInstance, promote);
					return;
				}
			}

			if (serviceType == null)
			{
				throw new ArgumentNullException("serviceType");
			}

			if (serviceInstance == null)
			{
				throw new ArgumentNullException("serviceInstance");
			}

			if ((!(serviceInstance is ServiceCreatorCallback) && !serviceInstance.GetType().IsCOMObject) && !serviceType.IsAssignableFrom(serviceInstance.GetType()))
			{
				throw new ArgumentException("ErrorInvalidServiceInstance: " + serviceType.FullName);
			}

			if (this.Services.ContainsKey(serviceType))
			{
				throw new ArgumentException("ErrorServiceExists: " +serviceType.FullName, "serviceType");
			}

			this.Services[serviceType] = serviceInstance;
		}
		#endregion // AddService
		#region GetService
		/// <summary>
		/// This method returns the requested service.
		/// </summary>
		/// <param name="serviceType">The service type.</param>
		/// <returns>Returns the service requested, or null if the service cannot be found.</returns>
		public object GetService(Type serviceType)
		{
			if (serviceType == typeof(IServiceContainer))
			{
				return this;
			}

			object serviceObj = this.Services[serviceType];

			if (serviceObj is ServiceCreatorCallback)
			{
				serviceObj = ((ServiceCreatorCallback) serviceObj)(this, serviceType);

				if (((serviceObj != null) 
                    && !serviceObj.GetType().IsCOMObject) 
                    && !serviceType.IsAssignableFrom(serviceObj.GetType()))
				{
					serviceObj = null;
				}
				Services[serviceType] = serviceObj;
			}

			if ((serviceObj == null) && (this.mParentProvider != null))
			{
				serviceObj = mParentProvider.GetService(serviceType);
			}

			return serviceObj;
		}


		#endregion
	}
}
