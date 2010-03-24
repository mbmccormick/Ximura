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
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading;

using Ximura;

using AH = Ximura.AttributeHelper;
#endregion // using
namespace Ximura.Windows
{
    /// <summary>
    /// This wondow is responsible for starting the services registerd through the HostAppServerAttributes.
    /// </summary>
    public partial class WindowAppServers : Window
    {
        #region Declarations
        AppServerCollection appServerHolders = null;
        #endregion // Declarations
        #region Constructors
        /// <summary>
        /// This constructor takes the appserver attributes collection as a parameter.
        /// </summary>
        /// <param name="attrs">The attribute collection.</param>
        public WindowAppServers(AppServerAttribute[] attrs)
            : this()
        {
            appServerHolders = new AppServerCollection(attrs);

            this.contAppServers.ItemsSource = appServerHolders;
        }

        /// <summary>
        /// This is the default constructor for the app server window.
        /// </summary>
        public WindowAppServers():base()
        {
            InitializeComponent();

            this.btStart.Click += new RoutedEventHandler(btStart_Click);
            this.btStop.Click += new RoutedEventHandler(btStop_Click);
            this.btExit.Click += new RoutedEventHandler(btExit_Click);

            this.Closing += new System.ComponentModel.CancelEventHandler(WindowAppServers_Closing);
        }
        #endregion // Constructors

        #region UI logic

        MessageBoxResult holder_RaiseMessage(AppServerHolder sender, string message, string title, MessageBoxButton options)
        {
            return MessageBox.Show(message, title, options);
        }

        void WindowAppServers_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.btStop.IsEnabled = false;
            this.btStart.IsEnabled = false;
            this.btExit.IsEnabled = false;

            AppServerStop();
        }

        void btStop_Click(object sender, RoutedEventArgs e)
        {
            this.btStop.IsEnabled = false;

            AppServerStop();

            this.btStart.IsEnabled = true;
        }

        void btStart_Click(object sender, RoutedEventArgs e)
        {
            this.btStart.IsEnabled = false;

            ThreadPool.QueueUserWorkItem(new WaitCallback(AppServerStart));
        }

        void btExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        #endregion // UI logic

        #region AppServerStart(object stateInfo)
        /// <summary>
        /// This method starts the individual services on a seperate thread.
        /// </summary>
        /// <param name="stateInfo">THe state information. This is ignored.</param>
        protected virtual void AppServerStart(object stateInfo)
        {
            try
            {
                //OK, start the services in order.
                appServerHolders.Start();
            }
            catch (ServiceStartException)
            {
                AppServerStop();

                this.btStart.InvokeOrCallDirect(() => btStart.IsEnabled = true );
                this.btStop.InvokeOrCallDirect(() => btStop.IsEnabled = false );

                return;
            }

            this.btStop.InvokeOrCallDirect(() => btStop.IsEnabled = true);

        }
        #endregion // AppServerStart(object stateInfo)
        #region AppServerStop()
        /// <summary>
        /// This method stops the application servers.
        /// </summary>
        protected virtual void AppServerStop()
        {
            appServerHolders.Stop();
        }
        #endregion // AppServerStop()

    }
}
