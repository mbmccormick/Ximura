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
    /// <summary>
    /// This class holds the AppServer and the AppDomain. In addition this class provides the metadata to allow the AppServerControl
    /// to display the current status of the appserver.
    /// </summary>
    public class AppServerHolder : INotifyPropertyChanged
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

        #region Declarations
        private bool mCanStart;
        private AppDomain dom = null;
        private AppServer server = null;
        #endregion // Declarations
        #region Constructor
        /// <summary>
        /// This constructor takes the attribute data and creates the holder.
        /// </summary>
        /// <param name="attr">The HostAppServerAttribute attribute data.</param>
        public AppServerHolder(HostAppServerAttribute attr)
        {
            Priority = attr.Priority;
            Name = attr.Name;
            ServerType = attr.ServerType;
            mCanStart = attr.Enabled;
            CanEdit = true;
            ErrorDescription = "";
            ErrorInStart = false;

            Status = AppServerStatus.NotStarted;
        }
        #endregion // Constructor

        #region Properties
        /// <summary>
        /// The service priority. Services with a lower priority will be started first.
        /// </summary>
        public int Priority { get; set; }
        /// <summary>
        /// The service friendly name.
        /// </summary>
        public string Name { get; private set; }
        /// <summary>
        /// The server type.
        /// </summary>
        public Type ServerType { get; private set; }
        /// <summary>
        /// The server tooltip. This is displayed when the cursor is held over the service.
        /// </summary>
        public virtual string ToolTip { get { return ServerType.AssemblyQualifiedName; } }

        /// <summary>
        /// This is the current status of the application server.
        /// </summary>
        public AppServerStatus Status{ get; private set; }

        /// <summary>
        /// This property determines whether the service can be started.
        /// </summary>
        public bool CanStart 
        {
            get
            {
                return mCanStart;
            }
            set
            {
                mCanStart = value;
                NotifyPropertyChanged("CanStart");
            }
        }
        /// <summary>
        /// This property determines whether the service values can be changed.
        /// </summary>
        public bool CanEdit { get; private set; }

        /// <summary>
        /// This property is the error message generated when the service starts.
        /// </summary>
        public string ErrorDescription {get; private set; }
        /// <summary>
        /// This boolean property identifies whether the service had errors during start.
        /// </summary>
        public bool ErrorInStart  {get; private set; }
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

            if (!CanStart)
            {
                ErrorDescription = "Service Disabled";
                NotifyPropertyChanged("ErrorDescription");
                Status = AppServerStatus.Disabled;
                NotifyPropertyChanged("Status");
                return;
            }

            ErrorDescription = "Starting";
            NotifyPropertyChanged("ErrorDescription");

            Status = AppServerStatus.Starting;
            NotifyPropertyChanged("Status");

            try
            {
                dom = AppDomain.CreateDomain(Name);

                server = (AppServer)dom.CreateInstanceAndUnwrap(ServerType.Assembly.FullName, ServerType.FullName);

                server.Start();

                Status = AppServerStatus.Started;
                NotifyPropertyChanged("Status");
                ErrorDescription = "";
                NotifyPropertyChanged("ErrorDescription");
            }
            catch (Exception ex)
            {
                ErrorDescription = string.Format("Error: {0}",ex.Message);
                ErrorInStart = true;
                Status = AppServerStatus.Failed;

                if (dom != null)
                {
                    AppDomain.Unload(dom);
                    dom = null;
                }

                NotifyPropertyChanged("ErrorInStart");
                NotifyPropertyChanged("Status");
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
        #region Stop()
        /// <summary>
        /// This method stops the service. If the service is not running, no action is taken.
        /// </summary>
        public void Stop()
        {
            if (Status == AppServerStatus.Started)
            {
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
            }

            Status = AppServerStatus.NotStarted;
            NotifyPropertyChanged("Status");

            ErrorDescription = "";
            NotifyPropertyChanged("ErrorDescription");

            CanEdit = true;
            NotifyPropertyChanged("CanEdit");
        }
        #endregion // Stop()


        #region Event management
        /// <summary>
        /// This event is used to notify containers of data changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// This event is used to attach a message dialog provider for dialog prompts.
        /// </summary>
        public event RaiseDialogMessage RaiseMessage;

        // NotifyPropertyChanged will raise the PropertyChanged event, passing 
        // the source property that is being updated.
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
