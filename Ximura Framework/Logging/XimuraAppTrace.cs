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
using System.Diagnostics;
using System.Threading;

using Ximura;
using Ximura.Server;

#endregion // using
namespace Ximura
{
	/// <summary>
	/// XimuraAppTrace is the static trace object for the Ximura Application framework.
	/// </summary>
	public class XimuraAppTrace
	{
		#region Declarations
        private static int threadCount = 0;
        private static ThreadingHelper.Semaphore jobLogger;
		private static Queue<QueueItem> logQueue = new Queue<QueueItem>();
		private static IXimuraLogging defaultLogger = NullLoggerAgent.NoLog();
		private static IXimuraLogging[] defaultLoggerCol = new IXimuraLogging[]{defaultLogger};
		/// <summary>
		/// categoryLogger contains the collection of active loggers
		/// </summary>
        private static List<IXimuraLogging> categoryLogger = new List<IXimuraLogging>();
		/// <summary>
		/// CategoryLog contains a lookup table based on category with an array list stored
		/// against the value for each logger that accepts 
		/// </summary>
        private static Dictionary<string, List<IXimuraLogging>> CategoryLog = 
            new Dictionary<string, List<IXimuraLogging>>();
        private static List<string> CategoryNoLog = new List<string>();

		private static int mIndentLevel = 0;
		private static int mIndentSize = 0;
		private static bool mAutoFlush = false;
		private static Thread logThread1;
		private static Thread logThread2;

		private static bool mActive = false;

        private static object syncTrace = new object();
        private static object syncLogger = new object();

		#endregion
		#region Static Constructor
		static XimuraAppTrace()
		{
            Start();
		}
		#endregion // Static Constructor

        #region Control methods - Start/Close/Active etc
        #region Start
        /// <summary>
        /// This method starts the logger. If the logger is active this method has no effect.
        /// </summary>
        public static void Start()
        {
            lock (syncTrace)
            {
                if (mActive)
                    return;

                mActive = true;

                jobLogger = new ThreadingHelper.Semaphore(0);

                logThread1 = new Thread(new ThreadStart(ThreadLoop));
                logThread2 = new Thread(new ThreadStart(ThreadLoop));

                logThread1.Name = "Logging Thread 1";
                logThread2.Name = "Logging Thread 2";

                logThread1.Start();
                logThread2.Start();
            }
        }
        #endregion // Start
		#region Close
		/// <summary>
		/// This method closes the logger and flushes any messages from the buffer. If the logger is 
        /// not active this method has no effect.
		/// </summary>
		public static void Close()
		{
            if (!Active)
                return;
				//throw new ObjectDisposedException("XimuraAppTrace");

            lock (syncTrace)
			{
				mActive = false;

			    FlushInternal();

                //Signal the threads so that they will exit.
                for (int pulse = threadCount; pulse >= 0; pulse--)
                {
                    jobLogger.AddOne();
                }

                IXimuraLogging[] currentLoggers = categoryLogger.ToArray();

			    foreach(IXimuraLogging logger in currentLoggers)
			    {
				    try
				    {
					    logger.Close();
					    LoggerRemove(logger);
				    }
				    catch {}
			    }

                //If threads are still hanging about then foreably abort them.
                if (threadCount>0)
                {
                    try
                    {
                        if (logThread1!=null || logThread1.IsAlive)
                            logThread1.Abort();
                    }
                    catch { }

                    try
                    {
                        if (logThread2 != null || logThread2.IsAlive)
                            logThread2.Abort();
                    }
                    catch { }
                }
			}
		}
        #endregion
		#region Active
		/// <summary>
		/// This method identifies whether the logger is active.
		/// </summary>
		public static bool Active
		{
			get
			
            {
				return mActive;
			}
		}
		#endregion // Active
        #endregion // Control methods
        #region Flush methods
        /// <summary>
		/// This method determines whether the logger will flush automatically
		/// </summary>
		public static bool AutoFlush
		{
			get
			{
				return mAutoFlush;
			}
			set
			{
				mAutoFlush = value;
			}
		}
		/// <summary>
		/// This method flushes all the current loggers.
		/// </summary>
		private static void FlushInternal()
		{
            lock (syncTrace)
            {
                foreach (IXimuraLogging logger in categoryLogger)
                    logger.Flush();
            }
		}
		/// <summary>
		/// This method flushes all the current loggers.
		/// </summary>
		public static void Flush()
		{
			if (!Active)
				return;
			
			FlushInternal();
		}
		#endregion

		#region QueueItem structure
        /// <summary>
        /// The QueueItem class holds a log item.
        /// </summary>
		private class QueueItem
		{
			#region Declarations
            public Guid userID;
            public string userName;
            public string machine;
            public string source;

			public bool logAsString,writeline;
			public string category,DebugSwitch; 
			public EventLogEntryType type;
			public object message;
			#endregion // Declarations
			#region Constructors
			public QueueItem(object message, string category, EventLogEntryType type, string DebugSwitch):
				this(message, category, type, DebugSwitch, false,true){}

			public QueueItem(object message, string category, EventLogEntryType type, string DebugSwitch, 
                bool logAsString, bool writeline)
			{
				this.message=message;
				this.category=category;
				this.type=type;
				this.DebugSwitch=DebugSwitch;
				this.logAsString=logAsString;
				this.writeline=writeline;
			}
			#endregion // Constructors

			public string Switch
			{
				get
				{
					return DebugSwitch==null?"_" + type.ToString():DebugSwitch + "_" + type.ToString();
				}
			}
		}
		#endregion // QueueItem structure

        #region ThreadLoop
        private static void ThreadLoop()
		{
            Interlocked.Increment(ref threadCount);
			while (true)
			{
				QueueItem item = GetQueueItem();

				// If we can't get one, go to sleep.
				if (item == null) 
					jobLogger.WaitOne();
				else
					LogItem(item);
                //If we are closed then just let the threads exit.
                if (!mActive)
                    break;
			}
            Interlocked.Decrement(ref threadCount);
        }
		#endregion // LogItem

        #region LogItem(QueueItem item)
        /// <summary>
		/// This method logs the specific item.
		/// </summary>
		/// <param name="item">The item to log.</param>
		private static void LogItem(QueueItem item)
		{
			try
			{			
                //if (item.DebugSwitch==null || item.DebugSwitch=="" ||!CategoryToLog(item.DebugSwitch))
                //{
                //    WriteItem(defaultLogger, item);
                //    if (mAutoFlush) 
                //        defaultLogger.Flush();
                //}
                //else
                //{
					foreach(IXimuraLogging logger in Resolve(item))
					{
						WriteItem(logger, item);
						if (mAutoFlush) 
							logger.Flush();
					}
                //}
			}
            catch (ThreadAbortException)
            {
                //Do nothing
            }
			catch (Exception ex)
			{
				Debug.WriteLine(ex.Message);
			}
		}
		#endregion // LogItem

        #region GetQueueItem
        private static QueueItem GetQueueItem()
		{
            lock (syncLogger)
			{
				try 
				{ 					
					if (logQueue.Count > 0)
						return logQueue.Dequeue();
				} 
				catch (Exception ex)
				{
					Debug.WriteLine(ex.Message);
				} // We should not fail here.
			}

			return null;
		}
		#endregion // GetLogItem
        #region AddQueueItem
        private static void AddQueueItem(QueueItem item)
        {
            if (item == null)
                return;

            lock (syncLogger)
            {
                logQueue.Enqueue(item);
            }

            jobLogger.AddOne();
        }
        #endregion // AddQueueItem

		#region WriteItem
		private static void WriteItem(IXimuraLogging logger, QueueItem item)
		{
			IXimuraLoggingExtended exLog = logger as IXimuraLoggingExtended;
			
			if (item.writeline)
				if (exLog!=null)
				{
					if (item.logAsString)
						exLog.WriteLine(item.message as string,item.category,item.type);
					else
						exLog.WriteLine(item.message.ToString(),item.category,item.type);

				}
				else
				{
					if (item.logAsString)
						logger.WriteLine(item.message,item.category);
					else
						logger.WriteLine(item.message.ToString(),item.category);
				}
			else
			{
				if (exLog!=null)
				{
					if (item.logAsString)
						exLog.Write(item.message as string,item.category,item.type);
					else
						exLog.Write(item.message.ToString(),item.category,item.type);
				}
				else
				{
					if (!item.logAsString)
						logger.Write(item.message,item.category);
					else
						logger.Write(item.message.ToString(),item.category);
				}
			}
		}
		#endregion // WriteItem

		#region DefaultLogger
		/// <summary>
		/// This property gets the default logger
		/// </summary>
		public static IXimuraLogging DefaultLogger
		{
			get{return defaultLogger;}
			set
			{
				if (value==null)
					defaultLogger =  NullLoggerAgent.NoLog();
				else
					defaultLogger=value;
			}
		}
		#endregion
		#region Logger Management - LoggerAdd/LoggerRemove
		/// <summary>
		/// This method adds a logger to the logging collection.
		/// </summary>
		/// <param name="logger">The logger to add.</param>
		public static void LoggerAdd(IXimuraLogging logger)
		{
            lock (syncTrace)
			{
				if (!categoryLogger.Contains(logger))
				{
					categoryLogger.Add(logger);
				}
				if (defaultLogger == NullLoggerAgent.NoLog())
					defaultLogger = logger;
			}
		}
		/// <summary>
		/// This method removes a logger from the logging collection.
		/// </summary>
		/// <param name="logger">The logger to remove.</param>
		public static void LoggerRemove(IXimuraLogging logger)
		{
            lock (syncTrace)
			{
				if (!categoryLogger.Contains(logger))
					return;

				categoryLogger.Remove(logger);

				if (categoryLogger.Count == 0)
					defaultLogger = NullLoggerAgent.NoLog();
				else if (defaultLogger == logger)
					defaultLogger = categoryLogger[0];

				//Safety check
				if (defaultLogger == null)
					defaultLogger = NullLoggerAgent.NoLog();
			}
		}

		#endregion

		#region Resolve
        /// <summary>
        /// This method returns true if there is a category to log.
        /// </summary>
        /// <param name="category">The category to check.</param>
        /// <returns>Returns true if the category is found.</returns>
		private static bool CategoryToLog(string category)
		{
			return !CategoryNoLog.Contains(category);
		}
        /// <summary>
        /// This method is used to resolve the application loggers for a particular logging
        /// event.
        /// </summary>
        /// <param name="item">The logging QueueItem.</param>
        /// <returns>Returns an array list containing the logger collection.</returns>
        private static List<IXimuraLogging> Resolve(QueueItem item)
		{
            //Do a null check for the default logger.
            string strSwitch = item.Switch;
            if (strSwitch == null) 
                strSwitch = "";

            if (CategoryLog.ContainsKey(strSwitch))
                return CategoryLog[strSwitch];

            lock (syncTrace)
			{
                List<IXimuraLogging> newList = new List<IXimuraLogging>();

				foreach(IXimuraLogging logger in categoryLogger)
				{
                    if (logger.AcceptCategory(item.DebugSwitch, item.type))
						newList.Add(logger);
				}

                if (!CategoryLog.ContainsKey(strSwitch))
                {
				    if (newList.Count>0)
                        CategoryLog.Add(strSwitch, newList);
				    else
                        CategoryNoLog.Add(strSwitch);
                }
				return newList;
			}
		}
		#endregion

        #region Logging methods
        #region Assert and Fail
        public static void Assert(Boolean condition, String message, String detailMessage)
		{
			if (!condition)
				defaultLogger.Fail(message,detailMessage);
		}

		public static void Assert(Boolean condition, String message)
		{
			if (!condition)
				defaultLogger.Fail(message);
		}

		public static void Assert(Boolean condition)
		{
			if (!condition)
				defaultLogger.Fail("");
		}

		public static void Fail(String message, String detailMessage)
		{
			defaultLogger.Fail(message, detailMessage);
		}

		public static void Fail(String message)
		{
			defaultLogger.Fail(message);
		}
		#endregion

		#region Write
        /// <summary>
        /// This method writes to the selected loggers with the message object specified.
        /// </summary>
        /// <param name="value">The message object.</param>
        /// <param name="category">The message category.</param>
        /// <param name="type">The entry type.</param>
        /// <param name="DebugSwitch">The applicable debug switch.</param>
		public static void Write(object value, string category, EventLogEntryType type, string DebugSwitch)
		{
			if (!Active)
				return;

            AddQueueItem(new QueueItem(value, category, type, DebugSwitch, false, false));
		}
        /// <summary>
        /// This method writes to the selected loggers with the message specified.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="category">The message category.</param>
        /// <param name="type">The entry type.</param>
        /// <param name="DebugSwitch">The applicable debug switch.</param>
		public static void Write(string message, string category, EventLogEntryType type, string DebugSwitch)
		{
			if (!Active)
				return;

            AddQueueItem(new QueueItem(message, category, type, DebugSwitch, true, false));
		}
		#endregion
		#region WriteLine
        /// <summary>
        /// This method writes to the selected loggers with the message object specified.
        /// For loggers that support multiline entry, this method will append a new line to the message.
        /// </summary>
        /// <param name="value">The message object.</param>
        /// <param name="category">The message category.</param>
        /// <param name="type">The entry type.</param>
        /// <param name="DebugSwitch">The applicable debug switch.</param>
        public static void WriteLine(object value, string category, EventLogEntryType type, string DebugSwitch)
		{
			if (!Active)
				return;

            AddQueueItem(new QueueItem(value, category, type, DebugSwitch, false, true));
		}
        /// <summary>
        /// This method writes to the selected loggers with the message specified. 
        /// For loggers that support multiline entry, this method will append a new line to the message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="category">The message category.</param>
        /// <param name="type">The entry type.</param>
        /// <param name="DebugSwitch">The applicable debug switch.</param>
		public static void WriteLine(string message, string category, EventLogEntryType type, string DebugSwitch)
		{
			if (!Active)
				return;

            AddQueueItem(new QueueItem(message, category, type, DebugSwitch, true, true));
		}
		#endregion

		#region Write helper methods
		public static void Write(object value, string category, EventLogEntryType type)
		{
			Write(value, category, EventLogEntryType.Information, null);
		}

		public static void Write(object value, string category)
		{
			Write(value, category, EventLogEntryType.Information, null);
		}

		public static void Write(object value)
		{
			Write(value, null, EventLogEntryType.Information, null);
		}

		public static void Write(string message, string category, EventLogEntryType type)
		{
			Write(message, category, type, null);
		}
		public static void Write(string message, string category)
		{
			Write(message, category, EventLogEntryType.Information, null);
		}

		public static void Write(string message)
		{
			Write(message, null, EventLogEntryType.Information, null);
		}
		#endregion
		#region WriteLine helper methods
		public static void WriteLine(object value, string category, EventLogEntryType type)
		{
			WriteLine(value, category, type, null);
		}
		public static void WriteLine(object value, string category)
		{
			WriteLine(value, category, EventLogEntryType.Information, null);
		}
		public static void WriteLine(object value)
		{
			WriteLine(value, null, EventLogEntryType.Information, null);
		}

		public static void WriteLine(string message, string category, EventLogEntryType type)
		{
			WriteLine(message, category, type, null);
		}
		public static void WriteLine(string message, string category)
		{
			WriteLine(message, category, EventLogEntryType.Information, null);
		}

		public static void WriteLine(string message)
		{
			WriteLine(message, null, EventLogEntryType.Information, null);
		}
		#endregion

		#region WriteIf
		public static void WriteIf(Boolean condition, Object value, String category)
		{
			if (condition)
				Write(value, category);
		}

		public static void WriteIf(Boolean condition, String message, String category)
		{
			if (condition)
				Write(message, category);
		}

		public static void WriteIf(Boolean condition, Object value)
		{
			if (condition)
				Write(value);
		}

		public static void WriteIf(Boolean condition, String message)
		{
			if (condition)
				Write(message);
		}
		#endregion
		#region WriteLineIf
		public static void WriteLineIf(Boolean condition, Object value, String category)
		{
			if (condition)
				WriteLine(value, category);
		}

		public static void WriteLineIf(Boolean condition, String message, String category)
		{
			if (condition)
				WriteLine(message, category);
		}

		public static void WriteLineIf(Boolean condition, Object value)
		{
			if (condition)
				WriteLine(value);
		}

		public static void WriteLineIf(Boolean condition, String message)
		{
			if (condition)
				WriteLine(message);
		}
		#endregion

		#region Indent Method and Properties
		/// <summary>
		/// This method increase the indent
		/// </summary>
		public static void Indent()
		{
			if (!Active)
				return;

			mIndentLevel++;
		}
		/// <summary>
		/// This method decreases the indent
		/// </summary>
		public static void Unindent()
		{
			if (!Active)
				return;

			if (mIndentLevel >0 ) mIndentLevel--;
		}
		/// <summary>
		/// This property returns the current indent level
		/// </summary>
		public static int IndentLevel
		{
			get
			{
				return mIndentLevel;
			}
			set
			{
				mIndentLevel=value;
			}
		}
		/// <summary>
		/// This property detemines the indent size in characters
		/// </summary>
		public static int IndentSize
		{
			get
			{
				return mIndentSize;
			}
			set
			{
				mIndentSize=value;
			}
		}

		#endregion

		#region Listeners
		/// <summary>
		/// This method is not implemented.
		/// </summary>
		public static TraceListenerCollection Listeners
		{
			get
			{
				return null;//Resolve().Listeners;
			}
		}
		#endregion
        #endregion
    }
}