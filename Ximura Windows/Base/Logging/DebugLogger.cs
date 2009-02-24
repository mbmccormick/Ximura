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

using Ximura;
using Ximura.Server;

#endregion // using
namespace Ximura.Windows
{
    #region DebugLogger
    /// <summary>
    /// The DebugLogger uses the windows Debug object.
    /// </summary>
    public class DebugLogger : LoggerAgentBase
    {

        #region IXimuraLogging Members

        /// <summary>
        /// This method closes the logger.
        /// </summary>
        public override void Close()
        {
            Debug.Close();
        }

        /// <summary>
        /// This method flushes the logger
        /// </summary>
        public override void Flush()
        {
            Debug.Flush();
        }



        /// <summary>
        /// This message writes a string.
        /// </summary>
        /// <param name="message"></param>
        public override void Write(string message)
        {
            Debug.Write(message);
        }

        /// <summary>
        /// This message writes a string with a line end
        /// </summary>
        /// <param name="message">The message to write.</param>
        public override void WriteLine(string message)
        {
            Debug.WriteLine(message);
        }


        //		public TraceListenerCollection Listeners
        //		{
        //			get
        //			{
        //				return Debug.Listeners;
        //			}
        //		}


        #endregion
    }

    #endregion
    #region TraceLoggingProvider
    /// <summary>
    /// The TraceLogger uses the windows Trace method.
    /// </summary>
    public class TraceLogger : LoggerAgentBase
    {
        #region Declarations

        #endregion


        #region IXimuraLogging Members

        /// <summary>
        /// This method closes the logger.
        /// </summary>
        public override void Close()
        {
            Trace.Close();
        }

        /// <summary>
        /// This method flushes the logger.
        /// </summary>
        public override void Flush()
        {
            Trace.Flush();
        }

        /// <summary>
        /// This method writes an object to the logger
        /// </summary>
        /// <param name="value">The message.</param>
        public override void Write(string message)
        {
            Trace.Write(message);
        }

        /// <summary>
        /// This method writes a string to the logger.
        /// </summary>
        /// <param name="message"></param>
        public override void WriteLine(string message)
        {
            Trace.WriteLine(message);
        }

        #endregion
    }
    #endregion
}
