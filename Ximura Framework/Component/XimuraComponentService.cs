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

using CH=Ximura.Common;
using Ximura.Framework;
using Ximura.Framework;
#endregion // using
namespace Ximura.Framework
{
	/// <summary>
	/// XimuraComponentService is the base object for all Ximura components 
	/// that can be started and stopped.
	/// </summary>
    public class XimuraComponentService : 
        XimuraComponentBase, IXimuraComponentService, IXimuraSecurityDescriptor
	{
		#region Declarations
        private long? mPermissionBitmap = null;
        /// <summary>
        /// This collection holds the specific attribute that determines the permissions for 
        /// the particular bit.
        /// </summary>
        private Dictionary<int, XimuraComponentPermissionAttribute> permissionAttrs = null;

        private XimuraComponentServiceHelper xcServiceh;

        #endregion
		#region Constructors
		/// <summary>
		/// This is the default constructor for the XimuraComponentService.
		/// </summary>
		public XimuraComponentService():this((IContainer)null){}
		/// <summary>
		/// This constructor is called by the .Net component model when adding it to a container
		/// </summary>
		/// <param name="container">The container to add the component to.</param>
		public XimuraComponentService(IContainer container):base(container)
		{
            xcServiceh = new XimuraComponentServiceHelper(this,
                new XimuraComponentServiceHelper.ServiceCallBack(InternalStart),
                new XimuraComponentServiceHelper.ServiceCallBack(InternalStop),
                new XimuraComponentServiceHelper.ServiceCallBack(InternalPause),
                new XimuraComponentServiceHelper.ServiceCallBack(InternalContinue));

            xcServiceh.Site = this.Site;
		}
		#endregion

		#region Virtual Service Control methods - InternalStart/InternalStop etc.
		#region InternalStart()
		/// <summary>
		/// This is the method to override which should start your service.
		/// </summary>
		protected virtual void InternalStart()
		{
			throw new NotImplementedException("InternalStart is not implemented. This must be overriden in the derived class");
		}
		#endregion // InternalStart()
		#region InternalStop()
		/// <summary>
		/// This is the method to override which should stop your service
		/// </summary>
		protected virtual void InternalStop()
		{
			throw new NotImplementedException("InternalStop is not implemented. This must be overriden in the derived class");
		}
		#endregion // InternalStop()
		#region InternalPause()
		/// <summary>
		/// This is the method to override which should pause your service
		/// </summary>
		protected virtual void InternalPause()
		{
			throw new NotImplementedException("InternalPause is not implemented. This must be overriden in the derived class");
		}
		#endregion // InternalPause()
		#region InternalContinue()
		/// <summary>
		/// This is the method to override which should continue your service 
		/// if it has been paused
		/// </summary>
		protected virtual void InternalContinue()
		{
			throw new NotImplementedException("InternalContinue is not implemented. This must be overriden in the derived class");
		}
		#endregion // InternalContinue()
		#endregion

		#region IXimuraService Members - Start/Stop/Pause/Continue
		#region Start()
		/// <summary>
		/// This method starts the service based on the default async settings
		/// </summary>
		public virtual void Start()
		{
            xcServiceh.Start();
		}
		#endregion // Start()
		#region Pause()
		/// <summary>
		/// This method pauses the service
		/// </summary>
		public virtual void Pause()
		{
            xcServiceh.Pause();
		}
		#endregion // Pause()
		#region Continue()
		/// <summary>
		/// This method continues a paused service
		/// </summary>
		public virtual void Continue()
		{
            xcServiceh.Continue();
		}
		#endregion // Continue()
		#region Stop()
		/// <summary>
		/// This method stops a running service
		/// </summary>
		public virtual void Stop()
		{
            xcServiceh.Stop();
		}
		#endregion // Stop()
		#region ServiceStatus
		/// <summary>
		/// This method will return the current service status
		/// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public XimuraServiceStatus ServiceStatus
		{
			get
			{
                return xcServiceh.ServiceStatus;
			}
		}
		#endregion // ServiceStatus
        #region ServiceEnabled
        /// <summary>
        /// This property determines whether the service component can be started.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool ServiceEnabled
        {
            get
            {
                return xcServiceh.ServiceEnabled;
            }
            set
            {
                xcServiceh.ServiceEnabled = value;
            }
        } 
        #endregion
		#endregion
		#region IXimuraServiceWithEvent Members
		/// <summary>
		/// This event will be fired when the service starts
		/// </summary>
        public event ServiceEvent ServiceStarted
        {
            add
            {
                xcServiceh.ServiceEventAdd(XimuraServiceStatus.Started, value);
            }
            remove
            {
                xcServiceh.ServiceEventRemove(XimuraServiceStatus.Started, value);
            }
        }
		/// <summary>
		/// This event will be fired when the service pauses
		/// </summary>
		public event ServiceEvent ServicePaused
        {
            add
            {
                xcServiceh.ServiceEventAdd(XimuraServiceStatus.Paused, value);
            }
            remove
            {
                xcServiceh.ServiceEventRemove(XimuraServiceStatus.Paused, value);
            }
        }
		/// <summary>
		/// This event will be fired when the service is resumed from a paused state
		/// </summary>
		public event ServiceEvent ServiceResumed
        {
            add
            {
                xcServiceh.ServiceEventAdd(XimuraServiceStatus.Resumed, value);
            }
            remove
            {
                xcServiceh.ServiceEventRemove(XimuraServiceStatus.Resumed, value);
            }
        }
		/// <summary>
		/// This event will be fired when the service is stopped
		/// </summary>
		public event ServiceEvent ServiceStopped
        {
            add
            {
                xcServiceh.ServiceEventAdd(XimuraServiceStatus.Stopped, value);
            }
            remove
            {
                xcServiceh.ServiceEventRemove(XimuraServiceStatus.Stopped, value);
            }
        }
		#endregion
        #region ProcessEvent
        /// <summary>
		/// This method fires an event to all parties that have registered with the ServiceEvent
		/// </summary>
		/// <param name="theEvent">The ServiceEvent Type to fire</param>
		protected virtual void ProcessEvent(ServiceEvent theEvent)
		{
			ProcessEvent(theEvent,this,null);
		}
		/// <summary>
		/// This method fires an event to all parties that have registered with the ServiceEvent
		/// </summary>
		/// <param name="theEvent">The ServiceEvent Type to fire</param>
		/// <param name="e">The ServiceEventArgs object to pass</param>
		protected virtual void ProcessEvent(ServiceEvent theEvent, ServiceEventArgs e)
		{
			ProcessEvent(theEvent,this,e);
		}
		/// <summary>
		/// This method fires an event to all parties that have registered with the ServiceEvent
		/// </summary>
		/// <param name="theEvent">The ServiceEvent Type to fire</param>
		/// <param name="sender">The sended object to pass</param>
		/// <param name="e">The ServiceEventArgs object to pass</param>
		protected virtual void ProcessEvent(ServiceEvent theEvent, object sender, ServiceEventArgs e)
		{
			//Check whether anyone is attached to the event
			if (theEvent == null)
				return;

			foreach(ServiceEvent te in theEvent.GetInvocationList())
			{
				te(sender,e);
			}
		}
		#endregion

        #region InternalHelperConnectSite
        /// <summary>
        /// This protected method is used to connect internal helper objects to the component site.
        /// </summary>
        /// <param name="value">The new site.</param>
        protected override void InternalHelperConnectSite(ISite value)
        {
            base.InternalHelperConnectSite(value);
            if (xcServiceh != null)
                xcServiceh.Site = value;
        }
        #endregion // InternalHelperConnectSite

		#region ComponentsStatusChange()
		/// <summary>
		/// This method can start, stop, resume or pause a group of components.
		/// </summary>
		/// <param name="action">The action required</param>
		/// <param name="components">The components to which the action should be provided</param>
		protected virtual void ComponentsStatusChange(XimuraServiceStatusAction action, ICollection components)
		{
			ComponentsStatusChange(action,components,null);
		}
		/// <summary>
		/// This method can start, stop, resume or pause a group of components of the specified type.
		/// </summary>
		/// <param name="action">The action required</param>
		/// <param name="components">The components to which the action should be provided</param>
		/// <param name="componentType">The component type.</param>
		protected virtual void ComponentsStatusChange(XimuraServiceStatusAction action, 
			ICollection components, Type componentType)
		{
			if (components == null)
				return;

			foreach(object objService in components)
			{
				IXimuraService service = objService as IXimuraService;
				if (service != null && 
					(componentType==null || (componentType.IsInstanceOfType(service))))
				{
					try
					{
						switch (action)
						{
							case XimuraServiceStatusAction.Start:
								if (service.ServiceEnabled && 
                                    (service.ServiceStatus == XimuraServiceStatus.Stopped ||
									service.ServiceStatus == XimuraServiceStatus.NotStarted))
								{
									if (ComponentsStatusBeforeChange(action, service))
									{
                                        if (service is IXimuraServiceParentSettings)
                                            ServiceParentSettingsSet((IXimuraServiceParentSettings)service);
										service.Start();
										ComponentsStatusAfterChange(action, service);
									}
								}
								break;
							case XimuraServiceStatusAction.Stop:
								if (service.ServiceStatus != XimuraServiceStatus.Stopped ||
									service.ServiceStatus != XimuraServiceStatus.Stopping ||
									service.ServiceStatus != XimuraServiceStatus.NotStarted ||
									service.ServiceStatus != XimuraServiceStatus.Undefined)
								{
									if (ComponentsStatusBeforeChange(action, service))
									{
										service.Stop();
										ComponentsStatusAfterChange(action, service);
									}				
								}
								break;
							case XimuraServiceStatusAction.Pause:
								if (service.ServiceStatus == XimuraServiceStatus.Started)
								{
									if (ComponentsStatusBeforeChange(action, service))
									{
										service.Pause();
										ComponentsStatusAfterChange(action, service);
									}				
								}								
								break;
							case XimuraServiceStatusAction.Continue:
								if (service.ServiceStatus == XimuraServiceStatus.Paused)
									if (service.ServiceStatus == XimuraServiceStatus.Started)
									{
										if (ComponentsStatusBeforeChange(action, service))
										{
											service.Continue();
											ComponentsStatusAfterChange(action, service);
										}				
									}									
								break;
						}
					}
					catch (Exception ex)
					{
                        string message = "ComponentsStatusChange: " + objService.ToString() + " -> " 
                            + action.ToString() + " -> " + ex.Message;

                        XimuraAppTrace.WriteLine(message, action.ToString() + " error.", EventLogEntryType.Error);
                        throw new XimuraComponentServiceException(message,ex);
					}
				}
			}
		}
		#endregion
		#region ComponentsStatusBeforeChange()
		/// <summary>
		/// This method is called before the status of a component is changed. You may override 
		/// this method to make additional checks before the component status is changed.
		/// </summary>
		/// <param name="action">The action, i.e. start, stop, etc.</param>
		/// <param name="service">The component service to change.</param>
		/// <returns>This method should return true if you want the status to change. 
		/// If this method returns false the status of the service will not change.</returns>
		protected virtual bool ComponentsStatusBeforeChange(
			XimuraServiceStatusAction action,IXimuraService service)
		{
			return true;
		}
		#endregion // ComponentsStatusBeforeChange()
		#region ComponentsStatusAfterChange()
		/// <summary>
		/// This method is called after the status of the service has been changed.
		/// </summary>
		/// <param name="action">The action, i.e. start, stop, etc.</param>
		/// <param name="service">The component service to change.</param>
		protected virtual void ComponentsStatusAfterChange(
			XimuraServiceStatusAction action,IXimuraService service)
		{

		}
		#endregion // ComponentsStatusAfterChange()

		#region ServicesReference/Dereference
		/// <summary>
		/// This protected method retrieves the base services for the container. 
		/// You should override this method if you wish to set references to
		/// base services from the container.
		/// </summary>
		protected virtual void ServicesReference()
		{
		}
		/// <summary>
		/// This protected method should remove any references
		/// to other services in the conatiner. 
		/// </summary>
		protected virtual void ServicesDereference()
		{
		}
		#endregion

        #region IXimuraSecurityDescriptor Members
        /// <summary>
        /// This is the permission bitmap.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual long PermissionsBitmap
        {
            get
            {
                lock (this)
                {
                    if (!mPermissionBitmap.HasValue)
                        BuildPermissions();

                    return mPermissionBitmap.Value;
                }
            }
        }
        /// <summary>
        /// This method returns the permission attribute collection for the class.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual XimuraComponentPermissionAttribute[] Permissions
        {
            get
            {
                lock (this)
                {
                    if (!mPermissionBitmap.HasValue)
                        BuildPermissions();

                    XimuraComponentPermissionAttribute[] attr = new
                        XimuraComponentPermissionAttribute[permissionAttrs.Values.Count];

                    permissionAttrs.Values.CopyTo(attr,0);

                    return attr;
                }
            }
        }
        /// <summary>
        /// This method builds the permission bitmap from the permission attributes.
        /// </summary>
        protected virtual void BuildPermissions()
        {
            mPermissionBitmap = null;

            if (permissionAttrs == null)
                permissionAttrs = new Dictionary<int, XimuraComponentPermissionAttribute>();
            else
                permissionAttrs.Clear();

            object[] permissionTypeAttrs;

            try
            {
                permissionTypeAttrs = PermissionsType.GetCustomAttributes(
                    typeof(XimuraComponentPermissionAttribute), true);
            }
            catch (Exception ex)
            {
                permissionTypeAttrs = null;
            }

            mPermissionBitmap = 0;

            if (permissionTypeAttrs == null)
                return;

            foreach (object attr in permissionTypeAttrs)
            {
                XimuraComponentPermissionAttribute xpAttr = attr as XimuraComponentPermissionAttribute;
                if (xpAttr == null)
                    continue;

                BitArray ba = BitShift(xpAttr.PermissionBitmap);

                if (xpAttr.PermissionIsSupported)
                    //Add the permission.
                    mPermissionBitmap |= xpAttr.PermissionBitmap;
                else
                    //Remove the permission.
                    mPermissionBitmap &= long.MaxValue - xpAttr.PermissionBitmap;
            }
        }

        /// <summary>
        /// This method returns a collection of integers for the bits present in the collection.
        /// </summary>
        /// <param name="bitMap">The 64 bit map.</param>
        /// <returns>An integer collection or and empty collection if the bitmap is 0.</returns>
        BitArray BitShift(long bitMap)
        {
            BitArray ba = 
                new BitArray(new int[]{(int)(bitMap & 0xFFFFFFFF),(int)(bitMap<<32)});

            return ba;
        }

        /// <summary>
        /// This is the type of the object that we require the permission bitmap for.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        protected virtual Type PermissionsType
        {
            get
            {
                return this.GetType();
            }
        }

        #endregion

        #region ServiceParentSettingsSet(IXimuraService service)
        /// <summary>
        /// This method is called when a component inmplements the IXimuraServiceParentSettings interface.
        // You should override it and set any service specific values from the parent component.
        /// </summary>
        /// <param name="service">The service.</param>
        protected virtual void ServiceParentSettingsSet(IXimuraServiceParentSettings service)
        {

        }
        #endregion // ServiceParentSettingsSet(IXimuraService service)
	}
}