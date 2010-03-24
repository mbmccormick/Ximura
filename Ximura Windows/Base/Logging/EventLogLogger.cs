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
﻿#region using
using System;
using System.Diagnostics;
using System.Security.Permissions;

using Ximura;
using Ximura.Framework;

#endregion // using
namespace Ximura.Windows
{
    /// <summary>
    /// The EventLogLogger class is used to log specific occurences in to the 
    /// Windows Event Log.
    /// </summary>
    public class EventLogLogger : LoggerAgentBase, IXimuraLoggingExtended
    {
        #region Declarations
        private string mSource = null;
        private EventLog myLog = null;
        #endregion // Declarations
        #region Initialize(IXimuraLoggerConfigSH settings)
        /// <summary>
        /// This method initializes the logger.
        /// </summary>
        /// <param name="ApplicationDefinition">The application definition.</param>
        /// <param name="settings">The logger settings.</param>
        public override void Initialize(IXimuraLoggerSettings settings)
        {
            base.Initialize(settings);

#if (DEBUG)
            bool exists = false;

            try
            {
                exists = EventLog.SourceExists(settings.LoggerID);
            }
            catch (Exception ex)
            {}

            if (!exists)
            {
                //An event log source should not be created and immediately used.
                //There is a latency time to enable the source, it should be created
                //prior to executing the application that uses the source.
                //Execute this sample a second time to use the new source.

                //EventSourceCreationData data = new EventSourceCreationData();
                //data.
                EventLog.CreateEventSource(settings.LoggerID, settings.LoggerName);
            }
#endif

            mSource = settings.LoggerID;
        }
        #endregion // Initialize(IXimuraApplicationDefinition ApplicationDefinition, IXimuraLoggerConfigSH settings)

        #region AcceptCategory(string category, EventLogEntryType type)
        /// <summary>
        /// This method inform the Logging Manager whether it will accept the 
        /// category for logging.
        /// </summary>
        /// <param name="category">The logging category.</param>
        /// <param name="type">The envent log entry type category.</param>
        /// <returns>A boolean value. True indicates the category is accepted.</returns>
        public override bool AcceptCategory(string category, EventLogEntryType type)
        {
            if (category == "EventLog")
                return true;

            return base.AcceptCategory(category, type);
        }
        #endregion // AcceptCategory(string category, EventLogEntryType type)

        #region WriteLine(string message)
        /// <summary>
        /// This method does nothing in the null logger.
        /// </summary>
        /// <param name="message">The message</param>
        public override void WriteLine(string message)
        {
            EventLogWriteInternal("", message, EventLogEntryType.Information);
        }
        #endregion // WriteLine(string message)
        #region Write(string message)
        /// <summary>
        /// This method does nothing in the null logger.
        /// </summary>
        /// <param name="message">The message</param>
        public override void Write(string message)
        {
            EventLogWriteInternal("", message, EventLogEntryType.Information);
        }
        #endregion // Write(string message)
        #region IXimuraLogging - Write
        /// <summary>
        /// Writes a category name and message to the trace listeners in the Listeners
        ///  collection.
        /// </summary>
        /// <param name="message">A message to write. </param>
        /// <param name="category">A category name used to organize the output.</param>
        public override void Write(string message, String category)
        {
            EventLogWriteInternal(category, message, EventLogEntryType.Information);
        }
        #endregion
        #region IXimuraLoggingExtended Members
        /// <summary>
        /// This base method is used to write to the event log.
        /// </summary>
        /// <param name="message">The message</param>
        /// <param name="category">The category</param>
        /// <param name="type">The event type</param>
        public void WriteLine(string message, string category, System.Diagnostics.EventLogEntryType type)
        {
            EventLogWriteInternal(category, message, type);
        }
        /// <summary>
        /// This base method is used to write to the event log.
        /// </summary>
        /// <param name="message">The message</param>
        /// <param name="category">The category</param>
        /// <param name="type">The event type</param>
        public void Write(string message, string category, System.Diagnostics.EventLogEntryType type)
        {
            EventLogWriteInternal(category, message, type);
        }
        #endregion

        private void EventLogWriteInternal(string category, string message, System.Diagnostics.EventLogEntryType type)
        {
            try
            {
                if (message == null)
                    message = "";

                if (myLog == null)
                {
                    lock (this)
                    {
                        // Create an EventLog instance and assign its source.
                        EventLog newLog = new EventLog();
                        newLog.Source = mSource;
                        myLog = newLog;
                    }
                }

                myLog.WriteEntry(message, type);
            }
            catch (Exception ex)
            {
                //We don't really want to throw an exception here.
            }
        }
    }

}
