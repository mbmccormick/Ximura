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

using CH = Ximura.Common;
using Ximura.Framework;
using Ximura.Framework;
#endregion // using
namespace Ximura.Framework
{
    /// <summary>
    /// The XimuraComponentServiceHelper object is used to provide common functionality for 
    /// modules that implement the IXimuraComponentService interface.
    /// </summary>
    public class XimuraComponentServiceHelper : IXimuraService
    {
        #region Declarations
        /// <summary>
        /// This delegate is used to notify for changes to a child service.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="service">Thse service notification.</param>
        public delegate bool ComponentStatusChangeNotify(XimuraServiceStatusAction action,IXimuraService service);

        private Dictionary<XimuraServiceStatus, ServiceEvent> EventsCollection = null;
        
        private XimuraServiceStatus m_ServiceStatus = XimuraServiceStatus.NotStarted;
        private ISite mSite = null;
        /// <summary>
        /// This is the delegate used for service callbacks.
        /// </summary>
        public delegate void ServiceCallBack();

        private ServiceCallBack delInternalStart;
        private ServiceCallBack delInternalStop;
        private ServiceCallBack delInternalPause;
        private ServiceCallBack delInternalContinue;

        private ComponentStatusChangeNotify mNotifyAfter;
        private ComponentStatusChangeNotify mNotifyBefore;

        private object rootClass;

        private bool mServiceEnabled = true;
        #endregion // Declarations

        #region ServiceEventAdd
        /// <summary>
        /// This method adds a service event to the collection.
        /// </summary>
        /// <param name="status">The event status</param>
        /// <param name="theEvent">The event</param>
        public void ServiceEventAdd(XimuraServiceStatus status, ServiceEvent value)
        {
            lock (this)
            {
                if (EventsCollection == null)
                    EventsCollection = new Dictionary<XimuraServiceStatus, ServiceEvent>();

                if (EventsCollection.ContainsKey(status))
                    EventsCollection[status] += value;
                else
                    EventsCollection.Add(status, value);
            }
        }
        #endregion // ServiceEventAdd
        #region ServiceEventRemove
        /// <summary>
        /// This method handles service event removal.
        /// </summary>
        /// <param name="status">The service status.</param>
        /// <param name="value">The event.</param>
        public void ServiceEventRemove(XimuraServiceStatus status, ServiceEvent value)
        {
            lock (this)
            {
                if (EventsCollection == null)
                    EventsCollection = new Dictionary<XimuraServiceStatus, ServiceEvent>();

                if (!EventsCollection.ContainsKey(status))
                    return;

                ServiceEvent current = EventsCollection[status] - value;

                if (current == null)
                    EventsCollection.Remove(status);
                else
                    EventsCollection[status] = current;
            }
        }
        #endregion // ServiceEventRemove

        #region Constructor
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        /// <param name="delInternalStart"></param>
        /// <param name="delInternalStop"></param>
        /// <param name="delInternalPause"></param>
        /// <param name="delInternalContinue"></param>
        public XimuraComponentServiceHelper(object rootClass, ServiceCallBack delInternalStart, ServiceCallBack delInternalStop, 
            ServiceCallBack delInternalPause, ServiceCallBack delInternalContinue)
        {
            this.rootClass = rootClass;
            this.delInternalStart = delInternalStart;
            this.delInternalStop = delInternalStop;
            this.delInternalPause = delInternalPause;
            this.delInternalContinue = delInternalContinue;

            mNotifyAfter = null;
            mNotifyBefore = null;

        }
        #endregion // Constructor

        #region IXimuraService Members

        #region IXimuraService Members - Start/Stop/Pause/Continue
        #region Start()
        /// <summary>
        /// This method starts the service based on the default async settings
        /// </summary>
        public virtual void Start()
        {
            //Check whether we are in design mode. If so, do not proceed.
            if (this.DesignMode || !mServiceEnabled)
                return;

            //First check whether we are running, if not return
            if (!(m_ServiceStatus == XimuraServiceStatus.Stopped ||
                m_ServiceStatus == XimuraServiceStatus.NotStarted))
                throw new XimuraServiceException("Cannot start the service.");

            try
            {
                m_ServiceStatus = XimuraServiceStatus.Starting;

                //This method is overriden by the specific service.
                InternalStart();

            }
            catch (Exception ex)
            {
                m_ServiceStatus = XimuraServiceStatus.NotStarted;
                throw ex;
            }

            m_ServiceStatus = XimuraServiceStatus.Started;
            ProcessEvent(m_ServiceStatus);
        }
        #endregion // Start()
        #region Pause()
        /// <summary>
        /// This method pauses the service
        /// </summary>
        public virtual void Pause()
        {
            //Check whether we are in design mode. If so, do not proceed.
            if (this.DesignMode)
                return;

            //First check whether we are running, if not return
            if (m_ServiceStatus != XimuraServiceStatus.Started)
                return;

            try
            {
                m_ServiceStatus = XimuraServiceStatus.Pausing;
                //This method is overriden by the specific service.
                InternalPause();
            }
            catch (NotImplementedException)
            {
                m_ServiceStatus = XimuraServiceStatus.Started;
            }
            catch (Exception ex)
            {
                m_ServiceStatus = XimuraServiceStatus.Started;
                throw ex;
            }

            m_ServiceStatus = XimuraServiceStatus.Paused;
            ProcessEvent(m_ServiceStatus);
        }
        #endregion // Pause()
        #region Continue()
        /// <summary>
        /// This method continues a paused service
        /// </summary>
        public virtual void Continue()
        {
            //Check whether we are in design mode. If so, do not proceed.
            if (this.DesignMode)
                return;

            //First check whether we are paused, if not return
            if (m_ServiceStatus != XimuraServiceStatus.Paused)
                return;

            try
            {
                m_ServiceStatus = XimuraServiceStatus.Resuming;
                //This method is overriden by the specific service.
                InternalContinue();
            }
            catch (NotImplementedException)
            {
                //Do nothing as we shouldn't really get here
                return;
            }
            catch (Exception ex)
            {
                m_ServiceStatus = XimuraServiceStatus.Paused;
                throw ex;
            }

            m_ServiceStatus = XimuraServiceStatus.Started;
            ProcessEvent(XimuraServiceStatus.Resumed);
        }
        #endregion // Continue()
        #region Stop()
        /// <summary>
        /// This method stops a running service
        /// </summary>
        public virtual void Stop()
        {
            //Check whether we are in design mode. If so, do not proceed.
            if (this.DesignMode)
                return;

            //First check whether we are not already stopped or stopping, if not return
            if (m_ServiceStatus == XimuraServiceStatus.Stopped ||
                m_ServiceStatus == XimuraServiceStatus.Stopping)
                //|| m_ServiceStatus == XimuraServiceStatus.NotStarted
                return;

            XimuraServiceStatus oldStatus = m_ServiceStatus;
            m_ServiceStatus = XimuraServiceStatus.Stopping;
            try
            {
                //This method is overriden by the specific service.
                InternalStop();
            }
            catch (Exception ex)
            {
                m_ServiceStatus = oldStatus;
                throw ex;
            }
            m_ServiceStatus = XimuraServiceStatus.Stopped;
            ProcessEvent(m_ServiceStatus);
        }
        #endregion // Stop()
        #region ServiceStatus
        /// <summary>
        /// This method will return the current service status
        /// </summary>
        public XimuraServiceStatus ServiceStatus
        {
            get
            {
                return m_ServiceStatus;
            }
        }
        #endregion // ServiceStatus

        #region ServiceEnabled
        /// <summary>
        /// This property determines whether the service component can be started.
        /// </summary>
        public bool ServiceEnabled
        {
            get
            {
                return mServiceEnabled;
            }
            set
            {
                mServiceEnabled = value;
            }
        }
        #endregion
        #endregion

        #region Virtual Service Control methods - InternalStart/InternalStop etc.
        #region InternalStart()
        /// <summary>
        /// This is the method to override which should start your service.
        /// </summary>
        protected virtual void InternalStart()
        {
            if (delInternalStart == null)
                throw new NotImplementedException("InternalStart is not implemented. This must be overriden in the derived class");

            delInternalStart();
        }
        #endregion // InternalStart()
        #region InternalStop()
        /// <summary>
        /// This is the method to override which should stop your service
        /// </summary>
        protected virtual void InternalStop()
        {
            if (delInternalStop == null)
                throw new NotImplementedException("InternalStop is not implemented. This must be overriden in the derived class");

            delInternalStop();
        }
        #endregion // InternalStop()
        #region InternalPause()
        /// <summary>
        /// This is the method to override which should pause your service
        /// </summary>
        protected virtual void InternalPause()
        {
            if (delInternalPause == null)
                throw new NotImplementedException("InternalPause is not implemented. This must be overriden in the derived class");
            delInternalPause();
        }
        #endregion // InternalPause()
        #region InternalContinue()
        /// <summary>
        /// This is the method to override which should continue your service 
        /// if it has been paused
        /// </summary>
        protected virtual void InternalContinue()
        {
            if (delInternalContinue == null)
                throw new NotImplementedException("InternalContinue is not implemented. This must be overriden in the derived class");
            delInternalContinue();
        }
        #endregion // InternalContinue()
        #endregion

        #region Static Event Helper code
        private void ProcessEvent(XimuraServiceStatus status)
        {
            if (EventsCollection == null || !EventsCollection.ContainsKey(status))
                return;

            ProcessEvent(EventsCollection[status]);
        }
        /// <summary>
        /// This method fires an event to all parties that have registered with the ServiceEvent
        /// </summary>
        /// <param name="theEvent">The ServiceEvent Type to fire</param>
        public virtual void ProcessEvent(ServiceEvent theEvent)
        {
            ProcessEvent(theEvent, rootClass, null);
        }
        /// <summary>
        /// This method fires an event to all parties that have registered with the ServiceEvent
        /// </summary>
        /// <param name="theEvent">The ServiceEvent Type to fire</param>
        /// <param name="e">The ServiceEventArgs object to pass</param>
        public virtual void ProcessEvent(ServiceEvent theEvent, ServiceEventArgs e)
        {
            ProcessEvent(theEvent, rootClass, e);
        }
        /// <summary>
        /// This method fires an event to all parties that have registered with the ServiceEvent
        /// </summary>
        /// <param name="theEvent">The ServiceEvent Type to fire</param>
        /// <param name="sender">The sended object to pass</param>
        /// <param name="e">The ServiceEventArgs object to pass</param>
        public virtual void ProcessEvent(ServiceEvent theEvent, object sender, ServiceEventArgs e)
        {
            //Check whether anyone is attached to the event
            if (theEvent == null)
                return;

            foreach (ServiceEvent te in theEvent.GetInvocationList())
            {
                te(sender, e);
            }
        }
        #endregion
        #endregion

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
        /// <summary>
        /// This method identifies whether it is design mode.
        /// </summary>
        public virtual bool DesignMode
        {
            get
            {
                return mSite == null ? false : mSite.DesignMode;
            }
        }
        #endregion // Site

        #region ComponentsStatusChange
        /// <summary>
		/// This method can start, stop, resume or pause a group of components of the specified type.
		/// </summary>
		/// <param name="action">The action required</param>
		/// <param name="components">The components to which the action should be provided</param>
        public virtual void ComponentsStatusChange(XimuraServiceStatusAction action, 
			ICollection components)
		{
            ComponentsStatusChange(action, components, null, mNotifyAfter, mNotifyBefore);
        }
        /// <summary>
		/// This method can start, stop, resume or pause a group of components of the specified type.
		/// </summary>
		/// <param name="action">The action required</param>
		/// <param name="components">The components to which the action should be provided</param>
		/// <param name="componentType">The component type.</param>
        public virtual void ComponentsStatusChange(XimuraServiceStatusAction action, 
			ICollection components, Type componentType)
		{
            ComponentsStatusChange(action, components, componentType, mNotifyAfter, mNotifyBefore);
        }
        /// <summary>
		/// This method can start, stop, resume or pause a group of components of the specified type.
		/// </summary>
		/// <param name="action">The action required</param>
		/// <param name="components">The components to which the action should be provided</param>
		/// <param name="componentType">The component type.</param>
		public virtual void ComponentsStatusChange(XimuraServiceStatusAction action, 
			ICollection components, Type componentType, 
            ComponentStatusChangeNotify NotifyAfter, ComponentStatusChangeNotify NotifyBefore)
		{
			if (components == null)
				return;

            bool donotNotifyBefore = NotifyBefore==null;
            bool doNotifyAfter = NotifyAfter!=null;

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
								if (service.ServiceStatus == XimuraServiceStatus.Stopped ||
									service.ServiceStatus == XimuraServiceStatus.NotStarted)
								{
									if (donotNotifyBefore || NotifyBefore(action, service))
									{
										service.Start();
										if (doNotifyAfter) NotifyAfter(action, service);
									}
								}
								break;
							case XimuraServiceStatusAction.Stop:
								if (service.ServiceStatus != XimuraServiceStatus.Stopped ||
									service.ServiceStatus != XimuraServiceStatus.Stopping ||
									service.ServiceStatus != XimuraServiceStatus.NotStarted ||
									service.ServiceStatus != XimuraServiceStatus.Undefined)
								{
									if (donotNotifyBefore || NotifyBefore(action, service))
									{
										service.Stop();
										if (doNotifyAfter) NotifyAfter(action, service);
									}				
								}
								break;
							case XimuraServiceStatusAction.Pause:
								if (service.ServiceStatus == XimuraServiceStatus.Started)
								{
									if (donotNotifyBefore || NotifyBefore(action, service))
									{
										service.Pause();
										if (doNotifyAfter) NotifyAfter(action, service);
									}				
								}								
								break;
							case XimuraServiceStatusAction.Continue:
								if (service.ServiceStatus == XimuraServiceStatus.Paused)
									if (service.ServiceStatus == XimuraServiceStatus.Started)
									{
										if (donotNotifyBefore || NotifyBefore(action, service))
										{
											service.Continue();
											if (doNotifyAfter) NotifyAfter(action, service);
										}				
									}									
								break;
						}
					}
					catch (Exception ex)
					{
                        XimuraAppTrace.WriteLine("ComponentsStatusChange -> " + action.ToString() + 
                            " -> " + ex.Message, "Start error.", EventLogEntryType.Error);
                        throw ex;
					}
				}
			}
		}
        #endregion // ComponentsStatusChange
    }
}
