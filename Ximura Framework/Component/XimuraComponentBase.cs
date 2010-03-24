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
using Ximura.Data;
using Ximura.Helper;
using CH=Ximura.Helper.Common;

using Ximura.Framework;
using Ximura.Framework;
#endregion // using
namespace Ximura
{
	/// <summary>
	/// XimuraComponentBase is the base component for all components in the Ximura system.
	/// </summary>
    public class XimuraComponentBase : Component, IXimuraComponentBase
	{
		#region Declarations
		private Hashtable mContainers;
		private ArrayList mConnectedComponents = null;

        private XimuraComponentBaseHelper xcbaseHelp;

        private object syncComponents = new object();
		/// <summary>
		/// This is the parent container that the component was added to during the constructor.
		/// </summary>
		private IContainer mParentContainer = null;
		#endregion
		#region Constructors
		/// <summary>
		/// This is the default constructor for the XimuraComponentService.
		/// </summary>
		public XimuraComponentBase():this((IContainer)null){}
		/// <summary>
		/// This constructor is called by the .Net component model when adding it to a container
		/// </summary>
		/// <param name="container">The container to add the component to.</param>
		public XimuraComponentBase(IContainer container)
		{
            xcbaseHelp = new XimuraComponentBaseHelper(GetType());
			if (container != null)
			{
				container.Add(this);
				//Remember a reference to the parent as we may add services to it later.
				mParentContainer = container;
			}
		}
		#endregion

		#region IDisposable
		/// <summary> 
		/// This is an override of the IDisposable method which cleans up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if (disposing)
			{
				if (mConnectedComponents != null)
					mConnectedComponents.Clear();

				if (mContainers != null)
				{
					foreach(IContainer components in mContainers.Keys)
					{
						components.Dispose();
					}
					mContainers.Clear();
				}
			}
			base.Dispose( disposing );
		}
		#endregion

		#region Attribute Helper methods
		/// <summary>
		/// This protected method will return the relevant attribute for the 
		/// type specified.
		/// </summary>
		protected Attribute GetAttributeForTypeID(Type theTypeAttribute)
		{
			Attribute attrToReturn = null;

			object[] attrs = this.GetType().GetCustomAttributes(theTypeAttribute,true);
			if (attrs.Length>0)
			{
				attrToReturn = attrs[0] as Attribute;
			}

			return attrToReturn;
		}
		#endregion

		#region AppSite extensions
        /// <summary>
        /// This property returns true when the component is active in execution mode.
        /// </summary>
        protected bool ActiveMode
        {
            get
            {
                return SiteExtended == null ? false : SiteExtended.DesignMode;
            }
        }
		/// <summary>
		/// This is an extended site which allows easy access to the extended app site methods.
		/// </summary>
        protected IXimuraAppSite SiteExtended
		{
			get
			{
                return this.Site as IXimuraAppSite;
			}
		}
		#endregion
		#region Site
		/// <summary>
		/// This override property calls the SiteChanged method when the site changes
		/// for the component.
		/// </summary>
		public override ISite Site
		{
			get
			{
				return base.Site;
			}
			set
			{
				ISite oldSite = this.Site;
				SiteBeforeChange(oldSite, value);
				base.Site = value;
                InternalHelperConnectSite(value);
				SiteChanged(oldSite, value);
			}
		}
		#endregion // Site
        #region InternalHelperConnectSite
        /// <summary>
        /// This protected method is used to connect internal helper objects to the component site.
        /// </summary>
        /// <param name="value">The new site.</param>
        protected virtual void InternalHelperConnectSite(ISite value)
        {
            xcbaseHelp.Site = value;
        }
        #endregion // InternalHelperConnectSite

		#region SiteBeforeChange
		/// <summary>
		/// This method is called before the site changes.
		/// </summary>
		/// <param name="newSite">The new site value.</param>
		/// <param name="oldSite">The old site value.</param>
		protected virtual void SiteBeforeChange(ISite oldSite, ISite newSite)
		{
            if (oldSite is IXimuraAppSite)
				ServicesRemove();
		}
		#endregion // SiteBeforeChange
		#region SiteChanged
		/// <summary>
		/// You should override this method if you require specific actions to occur
		/// if the component site changes.
		/// </summary>
		/// <param name="oldSite">The old site.</param>
		/// <param name="newSite">The new site.</param>
		protected virtual void SiteChanged(ISite oldSite, ISite newSite)
		{
            if (newSite is IXimuraAppSite)
			{
				ServicesProvide();
				ConnectComponents();
			}
		}
		#endregion // SiteChanged

		#region RemoveService
		/// <summary>
		/// Removes the specified service type from the service container.
		/// </summary>
		/// <param name="serviceType">The type of service to remove.</param>
		public void RemoveService(Type serviceType)
		{
            xcbaseHelp.RemoveService(serviceType, false);
		}
        /// <summary>
        /// Removes the specified service type from the service container.
        /// </summary>
        /// <typeparam name="T">The service type.</typeparam>
        public void RemoveService<T>()
        {
            xcbaseHelp.RemoveService(typeof(T), false);
        }
        /// <summary>
        /// Removes the specified service type from the service container.
        /// </summary>
        /// <typeparam name="T">The service type.</typeparam>
        /// <param name="promote">Should be set to true if this service should be removed 
        /// from any parent service containers.</param>
        public void RemoveService<T>(bool promote)
        {
            xcbaseHelp.RemoveService(typeof(T), promote);
        }
		/// <summary>
		/// Removes the specified service type from the service container.
		/// </summary>
		/// <param name="serviceType">The type of service to remove.</param>
		/// <param name="promote">Should be set to true if this service should be removed 
		/// from any parent service containers.</param>
		public void RemoveService(Type serviceType, bool promote)
		{
            xcbaseHelp.RemoveService(serviceType, promote);
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
            xcbaseHelp.AddService(serviceType, callback, false);
		}
		/// <summary>
		/// Adds the service specified to the service container.
		/// </summary>
		/// <param name="serviceType">The type of service to add.</param>
		/// <param name="serviceInstance">The service.</param>
		public void AddService(Type serviceType, object serviceInstance)
		{
            xcbaseHelp.AddService(serviceType, serviceInstance, false);
		}
        /// <summary>
        /// Adds the service specified to the service container.
        /// </summary>
        /// <typeparam name="T">The service type.</typeparam>
        /// <param name="serviceInstance">The service instance.</param>
        public void AddService<T>(T serviceInstance)
        {
            xcbaseHelp.AddService(typeof(T), serviceInstance, false);
        }
        /// <summary>
        /// Adds the service specified to the service container.
        /// </summary>
        /// <typeparam name="T">The service type.</typeparam>
        /// <param name="serviceInstance">The service instance.</param>
        /// <param name="promote">True promotes this service to the parent service container.</param>
        public void AddService<T>(T serviceInstance, bool promote)
        {
            xcbaseHelp.AddService(typeof(T), serviceInstance, promote);
        }
        /// <summary>
        /// Adds the service specified to the service container.
        /// </summary>
        /// <typeparam name="T">The service type.</typeparam>
        /// <param name="serviceInstance">The service instance.</param>
        /// <param name="promote">True promotes this service to the parent service container.</param>
        /// <param name="depth">The depth the service should be promoted. Set this value to -1 if you require it to be unlimited.</param>
        public void AddService<T>(T serviceInstance, bool promote, int depth)
        {
            xcbaseHelp.AddService(typeof(T), serviceInstance, promote, depth);
        }

		/// <summary>
		/// Adds the service specified to the service container.
		/// </summary>
		/// <param name="serviceType">The type of service to add.</param>
		/// <param name="callback">A callback delegate the create the service.</param>
		/// <param name="promote">True promotes this service to the parent service container.</param>
		public void AddService(Type serviceType, ServiceCreatorCallback callback, bool promote)
		{
            xcbaseHelp.AddService(serviceType, callback, promote);
		}
        /// <summary>
        /// Adds the service specified to the service container.
        /// </summary>
        /// <param name="serviceType">The type of service to add.</param>
        /// <param name="serviceInstance">The service.</param>
        /// <param name="promote">True promotes this service to the parent service container.</param>
        public void AddService(Type serviceType, object serviceInstance, bool promote)
        {
            xcbaseHelp.AddService(serviceType, serviceInstance, promote, -1);
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
            xcbaseHelp.AddService(serviceType, serviceInstance, promote, depth);
		}
		#endregion // AddService
		#region GetService
		/// <summary>
		/// This method returns the requested service.
		/// </summary>
		/// <param name="serviceType">The service type.</param>
		/// <returns>Returns the service requested, or null if the service cannot be found.</returns>
		object IServiceProvider.GetService(Type serviceType)
		{
            return xcbaseHelp.GetService(serviceType, -1);
        }
        /// <summary>
        /// This method returns the requested service.
        /// </summary>
        /// <param name="serviceType">The service type.</param>
        /// <param name="depth">The service depth. If this is set to -1 the depth is unlimited.</param>
        /// <returns>Returns the service requested, or null if the service cannot be found.</returns>
        public object GetService(Type serviceType, int depth)
        {
			return xcbaseHelp.GetService(serviceType, depth);		
		}
        #endregion
        #region GetService intercept
        /// <summary>
		/// This overriden method ensures that any services pass through
		/// the component and are not bypassed to the component site.
		/// </summary>
		/// <param name="service">The service.</param>
		/// <returns>An object that provides the service, 
		/// or null if the service type cannot be resolved.</returns>
		protected override object GetService(Type service)
		{
			return ((IServiceProvider)this).GetService (service);
		}
        /// <summary>
        /// This overriden method ensures that any services pass through
        /// the component and are not bypassed to the component site.
        /// </summary>
        /// <typeparam name="T">The service.</typeparam>
        /// <returns>An object that provides the service, 
        /// or null if the service type cannot be resolved.</returns>
        protected virtual T GetService<T>()
        {
            return (T)((IServiceProvider)this).GetService(typeof(T));
        }
		#endregion

        #region ServicesProvide()/ServicesRemove()
        /// <summary>
		/// This protected method should register any services with the component
		/// container.
		/// </summary>
		protected virtual void ServicesProvide()
		{
		}

		/// <summary>
		/// This protected method should remove any services that the component
		/// has registered with the component containers.
		/// </summary>
		protected virtual void ServicesRemove()
		{
		}
		#endregion // Services References

		#region ServiceComponents
		/// <summary>
		/// This is the list containing the service components. This list will be only be
		/// populated after ConnectComponents has been called. 
		/// </summary>
		protected IList ServiceComponents
		{
			get
			{
				lock (syncComponents)
				{
					if (mConnectedComponents==null)
					{
						mConnectedComponents = new ArrayList();
					}
				}
				return mConnectedComponents;
			}
		}
		#endregion // ServiceComponents

		#region ConnectComponents
		/// <summary>
		/// This method will connect the components held in the 
		/// containers in to the Ximura messaging architecture.
		/// </summary>
		protected virtual void ConnectComponents()
		{
			if (mContainers == null)
				return;

			if (ServiceComponents.Count>0)
				ServiceComponents.Clear();

			foreach(IContainer components in mContainers.Keys)
			{
                try
                {
                    ProcessContainer(components.Components);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
			}
		}
		#endregion
		#region ProcessContainer
		/// <summary>
		/// This method sets each Ximura based component site property to point to this component
		/// and adds the component to the ServiceComponents list.
		/// </summary>
		/// <param name="componentList">The components to add.</param>
		protected virtual void ProcessContainer(ICollection componentList)
		{
			ProcessContainer(componentList, ServiceComponents);
		}
		/// <summary>
		/// This method sets each Ximura based component site property to point to this component
		/// and adds the component to the new list passed.
		/// </summary>
		/// <param name="componentList">The components to add.</param>
		/// <param name="newList">The new list to which the parameter will be added to.</param>
		protected virtual void ProcessContainer(ICollection componentList, IList newList)
		{
			//This shouldn't happen, but no harm to check.
			if (componentList == null || componentList.Count==0)
				return;

			if (newList == null)
				throw new ArgumentNullException("The new list cannot be null.", "newList");

			foreach(object component in componentList)
			{
                IXimuraComponentBase xCom = component as IXimuraComponentBase;

				if (xCom!=null)
                    AttachComponent(xCom, newList);
			}
		}
		#endregion // ProcessContainer
        #region AttachComponent(IComponent xCom, IList newList)
        /// <summary>
        /// This method attaches a component to the system service messaging architecture.
        /// </summary>
        /// <param name="xCom"></param>
        /// <param name="newList"></param>
        protected virtual void AttachComponent(IComponent xCom, IList newList)
        {
            try
            {
                IXimuraAppSite site = new ElementSite((IContainer)null, xCom, this, null, false, this);
                xCom.Site = site;
                newList.Add(xCom);
                ProcessComponent(xCom);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion // AttachComponent(IComponent xCom, IList newList)
        #region ProcessComponent(IComponent xCom)
        /// <summary>
        /// This method can be overriden to provide specific functionality after a component is connected
        /// to the Ximura messaging architecture.
        /// </summary>
        /// <param name="xCom">The component.</param>
        protected virtual void ProcessComponent(IComponent xCom)
        {

        }
        #endregion // ProcessComponent(IComponent xCom)

		#region RegisterContainer
		/// <summary>
		/// This method is used to register the containers for the inherited classes that wish to participate
		/// is the Ximura messaging infrastructure.
		/// </summary>
		/// <param name="components">The component collection to add.</param>
		protected virtual void RegisterContainer(IContainer components)
		{
            if (components != null && !Containers.Contains(components))
                Containers.Add(components, null);
            else
                XimuraAppTrace.WriteLine("Duplicate");
		}
		#endregion // RegisterContainer
		#region Containers
		private Hashtable Containers
		{
			get
			{
				if (mContainers == null)
					mContainers = new Hashtable();

				return mContainers;
			}
		}
		#endregion // Services
	}
}