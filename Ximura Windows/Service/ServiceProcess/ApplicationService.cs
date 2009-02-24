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
using System.ComponentModel.Design.Serialization;
using System.Data;
using System.Diagnostics;
using System.ServiceProcess;

using Ximura;
using Ximura.Server;
using Ximura.Helper;
#endregion // using
namespace Ximura.Server
{
	/// <summary>
	/// ApplicationService is the base service container for the Ximura
	/// Application model.
	/// </summary>
	public class ApplicationService : System.ServiceProcess.ServiceBase
	{
		#region Declarations
        private UnhandledExceptionEventHandler uhexHandler;
        private Hashtable mContainers = null;
        private ArrayList mConnectedComponents = null;
		/// <summary>
		/// The service container contains all the components for the container.
		/// </summary>
		protected ServiceContainer m_ServiceContainer = null;
		#endregion // Declarations
		#region Constructor
		/// <summary>
		/// This is the default constructor for the application
		/// </summary>
		public ApplicationService()
		{
			InitializeContainer();
            uhexHandler = new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
		}
		#endregion // Constructor

		#region InitializeContainer()
		protected virtual void InitializeContainer()
		{
			//Set up the control container
			if (m_ServiceContainer == null)
				m_ServiceContainer = new ServiceContainer();
		}
		#endregion
        #region IDisposable
        /// <summary> 
        /// This is an override of the IDisposable method which cleans up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (mConnectedComponents != null)
                    mConnectedComponents.Clear();

                if (mContainers != null)
                {
                    foreach (IContainer components in mContainers.Keys)
                    {
                        components.Dispose();
                    }
                    mContainers.Clear();
                }
            }
            base.Dispose(disposing);
        }
        #endregion

        #region CurrentDomain_UnhandledException
        /// <summary>
        /// This method is used to trap unhanded exceptions in the service and log them before the service closes.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The exception arguments.</param>
        void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e == null)
            {
                EventLog.WriteEntry(this.ServiceName, "SERVICE CRASH - no data was passed.", EventLogEntryType.Error);
                return;
            }
            
                      
            Exception ex = e.ExceptionObject as Exception;
            if (ex == null)
            {
                EventLog.WriteEntry(this.ServiceName, 
                    "SERVICE CRASH THE FOLLOWING ERROR CAUSED THE SERVICE TO FAIL: " 
                    + e.ToString(), EventLogEntryType.Error);
                return;
            }

            EventLog.WriteEntry(this.ServiceName, 
                "SERVICE CRASH THE FOLLOWING ERROR CAUSED THE SERVICE TO FAIL: " + ex.Message
                + Environment.NewLine + Environment.NewLine + ex.ToString(), EventLogEntryType.Error);
        }
        #endregion // CurrentDomain_UnhandledException

		#region OnStart
		/// <summary>
		/// Set things in motion so your service can do its work.
		/// </summary>
		protected override void OnStart(string[] args)
		{
            //Add the unhandled exception watcher.
            AppDomain.CurrentDomain.UnhandledException += uhexHandler;
            
            //Attach the components and start the applications/components.
            try
			{
                ConnectComponents();
				ComponentsStatusChange(XimuraServiceStatusAction.Start,
                    (ICollection)ServiceComponents);
			}
			catch (Exception ex)
			{
				EventLog.WriteEntry(this.ServiceName, ex.Message, EventLogEntryType.Error);
				throw ex;
			}
		}
		#endregion // OnStart
		#region OnStop
		/// <summary>
		/// Stop this service and all applications within this container
		/// </summary>
		protected override void OnStop()
		{
            //Stop all the applications.
			ComponentsStatusChange(XimuraServiceStatusAction.Stop,
                (ICollection)ServiceComponents);

            //Remove the unhandled exception watcher.
            AppDomain.CurrentDomain.UnhandledException -= uhexHandler;
        }
		#endregion // OnStop
		#region OnPause
		/// <summary>
		/// Pauses this service and all contained applications
		/// </summary>
		protected override void OnPause()
		{
			ComponentsStatusChange(XimuraServiceStatusAction.Pause,
                (ICollection)ServiceComponents);
			base.OnPause ();
		}
		#endregion // OnPause
		#region OnContinue
		/// <summary>
		/// Continues this application after it has been paused
		/// </summary>
		protected override void OnContinue()
		{
			base.OnContinue ();
			ComponentsStatusChange(XimuraServiceStatusAction.Continue,
                (ICollection)ServiceComponents);
		}
		#endregion // OnContinue

		#region ComponentsStatusChange - Static Helper method
		/// <summary>
		/// This method can start, stop, resume or pause a group of components.
		/// </summary>
		/// <param name="action">The action required</param>
		/// <param name="components">The components to which the action should be provided</param>
		public static void ComponentsStatusChange(XimuraServiceStatusAction action, 
			ICollection components)
		{
			if (components == null)
				return;

			//AppDomain hello = System.AppDomain.CreateDomain("hello");

			foreach(object objService in components)
			{
				IXimuraService service = objService as IXimuraService;
				if (service != null)
					try
					{
						IXimuraAppServer appServer = objService as IXimuraAppServer;
					
						switch (action)
						{
							case XimuraServiceStatusAction.Start:
								service.Start();
								break;
							case XimuraServiceStatusAction.Stop:
								service.Stop();
								break;
							case XimuraServiceStatusAction.Pause:
								service.Pause();
								break;
							case XimuraServiceStatusAction.Continue:
								service.Continue();
								break;
						}
					}
					catch (Exception ex)
					{
						throw ex;
					}
			}
		}
		#endregion // ComponentsStatusChange - Static Helper method

        #region ServiceComponents
        /// <summary>
        /// This is the list containing the service components. This list will be only be
        /// populated after ConnectComponents has been called. 
        /// </summary>
        protected IList ServiceComponents
        {
            get
            {
                lock (this)
                {
                    if (mConnectedComponents == null)
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

            if (ServiceComponents.Count > 0)
                ServiceComponents.Clear();

            foreach (IContainer components in mContainers.Keys)
            {
                ProcessContainer(components.Components);
            }
        }

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
            if (componentList == null || componentList.Count == 0)
                return;

            if (newList == null)
                throw new ArgumentNullException("The new list cannot be null.", "newList");

            foreach (object component in componentList)
            {
                XimuraComponentBase xCom = component as XimuraComponentBase;

                if (xCom != null)
                {
                    IXimuraAppSite site =
                        new ElementSite((IContainer)null, (IComponent)component, m_ServiceContainer, null, false, this);
                    xCom.Site = site;
                    newList.Add(xCom);
                }
            }
        }
        #endregion
        #region RegisterContainer
        /// <summary>
        /// This method is used to register the containers for the inherited classes that wish to participate
        /// is the Ximura messaging infrastructure.
        /// </summary>
        /// <param name="components">The component collection to add.</param>
        protected virtual void RegisterContainer(IContainer components)
        {
            if (components != null)
                Containers.Add(components, null);
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
