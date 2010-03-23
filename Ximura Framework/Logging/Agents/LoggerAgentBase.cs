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
using System.Diagnostics;
#endregion // using
namespace Ximura.Server
{
	/// <summary>
	/// Summary description for BaseLogger.
	/// </summary>
	public abstract class LoggerAgentBase : TraceListener, IXimuraLoggingProvider, IXimuraServerAgent
	{
		#region Declarations
        /// <summary>
        /// The logger settings.
        /// </summary>
        protected IXimuraLoggerSettings mSettings = null;
		/// <summary>
		/// The autoflush parameter
		/// </summary>
		protected bool mAutoFlush = true;
		/// <summary>
		/// The log level
		/// </summary>
		protected int mLoglevel = 0;
		/// <summary>
		/// The log machine name
		/// </summary>
		protected string LogMachine = Environment.MachineName;

		#endregion

        #region Initialize
        /// <summary>
		/// This property initializes the logger.
		/// </summary>
		/// <param name="settings">The logger settings</param>
		/// <exception cref="System.ArgumentNullException">The settings parameter cannot be null.</exception>
        public virtual void Initialize(IXimuraLoggerSettings settings)
		{
			if (settings == null)
				throw new ArgumentNullException("settings","The logger settings cannot be null.");

			this.mSettings=settings;

			try
			{
                mLoglevel = mSettings.LogLevel;
			}
			catch
			{
				mLoglevel = 0;
			}
		
			try 
			{
                Name = mSettings.LoggerID;
			}
			catch
			{
				Name = "";
			}
		}
		#endregion
        #region Deinitialize()
        /// <summary>
        /// This method should be used to release any resources held by the logger.
        /// </summary>
        public virtual void Deinitialize()
        {
            
        }
        #endregion // Deinitialize()

		#region AcceptCategory(string category, EventLogEntryType type)
		/// <summary>
		/// This method inform the Logging Manager whether it will accept the 
		/// category for logging.
		/// </summary>
		/// <param name="category">The logging category.</param>
		/// <param name="type">The envent log entry type category.</param>
		/// <returns>A boolean value. True indicates the category is accepted.</returns>
		public virtual bool AcceptCategory(string category, EventLogEntryType type)
		{
			int level = ConvertType(type);

			return (level + mSettings.GetSwitchValue(category))>=mLoglevel;
		}
		#endregion // AcceptCategory(string category, EventLogEntryType type)

		#region ConvertType(EventLogEntryType type)
		/// <summary>
		/// This is a bit messy, but I require a fine grained resolution on the type and cannot depend on .NET implicit conversions.
		/// </summary>
		/// <param name="type">The type to convert.</param>
		/// <returns>Returns an integer specifying the debug type.</returns>
		protected int ConvertType(EventLogEntryType type)
		{
			switch (type)
			{
				case EventLogEntryType.Information:
					return 0;
				case EventLogEntryType.SuccessAudit:
					return 1;
				case EventLogEntryType.Warning:
					return 2;
				case EventLogEntryType.FailureAudit:
					return 3;
				case EventLogEntryType.Error:
					return 4;
				default: //Not sure what this is, but we would want to log it just in case.
					return 5;
			}
		}
		#endregion // ConvertType(EventLogEntryType type)

		#region AutoFlush
		/// <summary>
		/// The auto-flush property.
		/// </summary>
		public virtual bool AutoFlush
		{
			get{return mAutoFlush;}
			set{mAutoFlush = value;}
		}
		#endregion // AutoFlush
		#region Indent()
		/// <summary>
		/// This method indents the logger input.
		/// </summary>
		public virtual void Indent()
		{
			IndentLevel++;
		}
		#endregion // Indent()
		#region Unindent()
		/// <summary>
		/// This method unindents the logger input.
		/// </summary>
		public virtual void Unindent()
		{
			if (IndentLevel>0) IndentLevel--;
		}
		#endregion // Unindent()
    }
}