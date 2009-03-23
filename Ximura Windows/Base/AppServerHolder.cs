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
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using System.ComponentModel;
using System.Windows.Threading;
using System.Threading;

using Ximura;
using Ximura.Helper;
using Ximura.Server;
using AH = Ximura.Helper.AttributeHelper;
#endregion // using
namespace Ximura.Windows
{
    #region Delegates
    /// <summary>
    /// This delegate is used to raise external dialog messages.
    /// </summary>
    /// <param name="sender">The specific AppServerHolder instance.</param>
    /// <param name="message">The message.</param>
    /// <param name="title">The message title.</param>
    /// <param name="options">The button options.</param>
    /// <returns>The message response.</returns>
    public delegate MessageBoxResult RaiseDialogMessage(AppServerHolder sender, string message, string title, MessageBoxButton options);
    #endregion // Delegates

    /// <summary>
    /// This class holds the AppServer and the AppDomain. In addition this class provides the metadata to allow the AppServerControl
    /// to display the current status of the appserver.
    /// </summary>
    public class AppServerHolder : INotifyPropertyChanged, IXimuraService
    {
        #region Events
        /// <summary>
        /// This event is used to attach a message dialog provider for dialog prompts.
        /// </summary>
        public event RaiseDialogMessage RaiseMessage;
        #endregion // Events
        #region Declarations
        protected bool mServiceEnabled;
        protected AppDomain dom = null;
        protected IXimuraService server = null;
        #endregion // Declarations

        #region Constructor
        /// <summary>
        /// This constructor takes the attribute data and creates the holder.
        /// </summary>
        /// <param name="attr">The HostAppServerAttribute attribute data.</param>
        public AppServerHolder(AppServerAttribute attr)
        {
            Priority = attr.Priority;
            Name = attr.Name;
            ServerType = attr.ServerType;
            mServiceEnabled = attr.Enabled;
            CanEdit = true;
            ErrorDescription = "";
            ErrorInStart = false;

            ServiceEnabled = true;

            ServiceStatus = XimuraServiceStatus.NotStarted;
        }
        #endregion // Constructor

        #region Properties
        /// <summary>
        /// This is the current status of the application server.
        /// </summary>
        public XimuraServiceStatus ServiceStatus
        {
            get;
            protected set;
        }

        /// <summary>
        /// This property determines whether the service can be started.
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
                NotifyPropertyChanged("ServiceEnabled");
            }
        }

        /// <summary>
        /// The service priority. Services with a lower priority will be started first.
        /// </summary>
        public int Priority { get; set; }
        /// <summary>
        /// The service friendly name.
        /// </summary>
        public string Name { get; protected set; }
        /// <summary>
        /// The server type.
        /// </summary>
        public Type ServerType { get; protected set; }
        /// <summary>
        /// The server tooltip. This is displayed when the cursor is held over the service.
        /// </summary>
        public virtual string ToolTip { get { return ServerType.AssemblyQualifiedName; } }

        /// <summary>
        /// This property determines whether the service values can be changed.
        /// </summary>
        public bool CanEdit { get; protected set; }

        /// <summary>
        /// This property is the error message generated when the service starts.
        /// </summary>
        public string ErrorDescription {get; protected set; }
        /// <summary>
        /// This boolean property identifies whether the service had errors during start.
        /// </summary>
        public bool ErrorInStart  {get; protected set; }
        #endregion // Properties

        #region Start()
        /// <summary>
        /// This method starts the service.
        /// </summary>
        /// <exception cref="Ximura.Windows.ServiceStartException">This exception is thrown if the service generates an unhandled exception.</exception>
        public void Start()
        {
            bool throwError = false;

            CanEdit = false;
            ErrorInStart = false;
            NotifyPropertyChanged("CanEdit");
            NotifyPropertyChanged("ErrorInStart");

            if (!ServiceEnabled)
            {
                ErrorDescription = "Service Disabled";
                NotifyPropertyChanged("ErrorDescription");
                ServiceStatus = XimuraServiceStatus.Disabled;
                NotifyPropertyChanged("ServiceStatus");
                return;
            }

            ErrorDescription = "Starting";
            NotifyPropertyChanged("ErrorDescription");

            ServiceStatus = XimuraServiceStatus.Starting;
            NotifyPropertyChanged("ServiceStatus");

            try
            {
                dom = AppDomain.CreateDomain(Name);

                object obj = dom.CreateInstanceAndUnwrap(ServerType.Assembly.FullName, ServerType.FullName);
                server = obj as IXimuraService;

                server.Start();

                ServiceStatus = XimuraServiceStatus.Started;
                NotifyPropertyChanged("ServiceStatus");
                ErrorDescription = "";
                NotifyPropertyChanged("ErrorDescription");
            }
            catch (Exception ex)
            {
                ErrorDescription = string.Format("Error: {0}",ex.Message);
                ErrorInStart = true;
                ServiceStatus = XimuraServiceStatus.Failed;

                if (dom != null)
                {
                    AppDomain.Unload(dom);
                    dom = null;
                }

                NotifyPropertyChanged("ErrorInStart");
                NotifyPropertyChanged("ServiceStatus");
                NotifyPropertyChanged("ErrorDescription");

                if (RaiseMessage != null)
                {
                    throwError = RaiseMessage(this,
                        string.Format("Service '{0}' cannot start. Do you wish to continue?", Name),
                        "Start error.", MessageBoxButton.YesNoCancel) != MessageBoxResult.Yes;
                }
                else
                    throwError = true;
            }


            if (throwError)
                throw new ServiceStartException(ErrorDescription);
        }

        #endregion // Start()
        #region Pause()
        /// <summary>
        /// This method pauses the service.
        /// </summary>
        public void Pause()
        {
            if (ServiceStatus != XimuraServiceStatus.Started)
                return;

            ErrorDescription = "";

            try
            {
                server.Pause();
            }
            catch (Exception ex)
            {
                ErrorDescription = ex.Message;
            }

            ServiceStatus = server.ServiceStatus;
            NotifyPropertyChanged("ServiceStatus");
            NotifyPropertyChanged("ErrorDescription");
        }
        #endregion // Pause()
        #region Continue()
        /// <summary>
        /// This method continues the service after it has been paused.
        /// </summary>
        public void Continue()
        {
            if (ServiceStatus != XimuraServiceStatus.Paused)
                return;

            ErrorDescription = "";

            try
            {
                server.Continue();
            }
            catch (Exception ex)
            {
                ErrorDescription = ex.Message;
            }

            ServiceStatus = server.ServiceStatus;
            NotifyPropertyChanged("ServiceStatus");
            NotifyPropertyChanged("ErrorDescription");
        }
        #endregion // Continue()
        #region Stop()
        /// <summary>
        /// This method stops the service. If the service is not running, no action is taken.
        /// </summary>
        public void Stop()
        {
            switch (ServiceStatus)
            {
                case XimuraServiceStatus.Started:
                case XimuraServiceStatus.Paused:
                case XimuraServiceStatus.Pausing:
                case XimuraServiceStatus.Resuming:
                    try
                    {
                        server.Stop();
                    }
                    catch (Exception ex)
                    {

                    }
                    server = null;

                    AppDomain.Unload(dom);
                    dom = null;
                    break;
            }

            ServiceStatus = XimuraServiceStatus.NotStarted;
            NotifyPropertyChanged("ServiceStatus");

            ErrorDescription = "";
            NotifyPropertyChanged("ErrorDescription");

            CanEdit = true;
            NotifyPropertyChanged("CanEdit");
        }
        #endregion // Stop()

        #region INotifyPropertyChanged
        /// <summary>
        /// This event is used to notify containers of data changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// NotifyPropertyChanged will raise the PropertyChanged event, passing the source property that is being updated.
        /// </summary>
        /// <param name="propertyName">The property name that has changed.</param>
        public void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion
    }
}

