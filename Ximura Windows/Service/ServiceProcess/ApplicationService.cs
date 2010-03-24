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
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Data;
using System.Diagnostics;
using System.ServiceProcess;

using Ximura;
using Ximura.Framework;

#endregion // using
namespace Ximura.Windows
{
	/// <summary>
	/// ApplicationService is the base service container for the Ximura
	/// Application model.
	/// </summary>
	public class AppServerService : System.ServiceProcess.ServiceBase
	{
		#region Declarations
        /// <summary>
        /// This collection holds the service components
        /// </summary>
        protected AppServerCollection appServerHolders;
		#endregion // Declarations
		#region Constructor
		/// <summary>
		/// This is the default constructor for the application
		/// </summary>
		public AppServerService()
		{
            appServerHolders = new AppServerCollection(GetType());
            AppDomain.CurrentDomain.UnhandledException+=new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
		}
		#endregion // Constructor

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
            //Attach the components and start the applications/components.
            try
			{
                appServerHolders.Start();
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
            appServerHolders.Stop();
        }
		#endregion // OnStop
		#region OnPause
		/// <summary>
		/// Pauses this service and all contained applications
		/// </summary>
		protected override void OnPause()
		{
            appServerHolders.Pause();
		}
		#endregion // OnPause
		#region OnContinue
		/// <summary>
		/// Continues this application after it has been paused
		/// </summary>
		protected override void OnContinue()
		{
            appServerHolders.Continue();
        }
		#endregion // OnContinue

	}
}
