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
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using System.ComponentModel;
using System.Windows.Threading;
using System.Threading;
using System.Diagnostics;

using Ximura;
using Ximura.Helper;
using Ximura.Server;
using AH = Ximura.Helper.AttributeHelper;
#endregion // using
namespace Ximura.Windows
{
    /// <summary>
    /// This class holds the collection of AppServers that will be managed.
    /// </summary>
    [Serializable()]
    public class AppServerCollection : IXimuraService, IEnumerable<AppServerHolder>
    {
        #region Declarations
        List<AppServerHolder> mAppServerHolders = null;
        #endregion // Declarations

        #region Constructors
        /// <summary>
        /// This constructor passes the 
        /// </summary>
        /// <param name="baseType"></param>
        public AppServerCollection(Type baseType): this(AH.GetAttributes<AppServerAttribute>(baseType))
        {

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="baseType"></param>
        public AppServerCollection(AppServerAttribute[] attrs)
        {
            mAppServerHolders = new List<AppServerHolder>();

            attrs
                .OrderBy(a => a.Priority)
                .ForEach(a =>
                {
                    AppServerHolder holder = new AppServerHolder(a);
                    holder.RaiseMessage += new AppServerHolder.RaiseDialogMessage(AppServerHolder_RaiseMessage);
                    mAppServerHolders.Add(holder);
                });

            ServiceStatus = XimuraServiceStatus.NotStarted;
        }
        #endregion // Constructors

        MessageBoxResult AppServerHolder_RaiseMessage(AppServerHolder sender, string message, string title, MessageBoxButton options)
        {
            return MessageBoxResult.None;
        }

        #region Start()
        /// <summary>
        /// This method starts the app servers in order.
        /// </summary>
        public void Start()
        {
            ServiceStatus = XimuraServiceStatus.Starting;
            try
            {
                //OK, start the services in order.
                mAppServerHolders
                    .OrderBy(a => a.Priority)
                    .ForEach(a => a.Start());
            }
            catch (ServiceStartException ssex)
            {
                Stop();

                return;
            }

            ServiceStatus = XimuraServiceStatus.Started;
        }
        #endregion // Start()
        #region Pause()
        /// <summary>
        /// This method pauses the app servers in decending order.
        /// </summary>
        public void Pause()
        {
            ServiceStatus = XimuraServiceStatus.Pausing;

            //OK, start the services in order.
            mAppServerHolders
                .OrderByDescending(a => a.Priority)
                .ForEach(a => a.Pause());

            ServiceStatus = XimuraServiceStatus.Paused;
        }
        #endregion // Pause()
        #region Continue()
        /// <summary>
        /// This method continues the services if needed.
        /// </summary>
        public void Continue()
        {
            ServiceStatus = XimuraServiceStatus.Resuming;

            //OK, start the services in order.
            mAppServerHolders
                .OrderBy(a => a.Priority)
                .ForEach(a => a.Continue());

            ServiceStatus = XimuraServiceStatus.Started;
        }
        #endregion // Continue()
        #region Stop()
        /// <summary>
        /// This method stops the services in decending order.
        /// </summary>
        public void Stop()
        {
            ServiceStatus = XimuraServiceStatus.Stopping;

            mAppServerHolders
                .OrderByDescending(a => a.Priority)
                .ForEach(a => a.Stop());

            ServiceStatus = XimuraServiceStatus.Stopped;
        }
        #endregion // Stop()

        #region ServiceStatus
        /// <summary>
        /// This is the service status for the collection.
        /// </summary>
        public XimuraServiceStatus ServiceStatus
        {
            get; private set;
        }
        #endregion // ServiceStatus
        #region ServiceEnabled
        /// <summary>
        /// This method determines whether the service is enabled. For the AppServerCollection this property is set to true.
        /// </summary>
        public bool ServiceEnabled
        {
            get
            {
                return true;
            }
            set
            {
                //throw new NotSupportedException("ServiceEnabled cannot be changed.");
            }
        }
        #endregion // ServiceEnabled

        #region IEnumerable<AppServerHolder> Members

        public IEnumerator<AppServerHolder> GetEnumerator()
        {
            if (mAppServerHolders == null)
                yield break;

            foreach (var holder in mAppServerHolders)
                yield return holder;
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator)GetEnumerator();
        }

        #endregion
    }
}
