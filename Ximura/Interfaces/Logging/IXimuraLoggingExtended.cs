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
using System.Diagnostics;
using System.Configuration;

using Ximura;
using Ximura.Framework;
#endregion // using
namespace Ximura
{
    /// <summary>
    /// This interface provides extended functionality to enable the EventLogEntryType parameter
    /// to be passed to the write methods
    /// </summary>
    public interface IXimuraLoggingExtended
    {
        #region Write
        /// <summary>
        /// Writes a category name and message to the trace listeners in the Listeners
        ///  collection.
        /// </summary>
        /// <param name="message">A message to write. </param>
        /// <param name="category">A category name used to organize the output.</param>
        /// <param name="type">The event log type.</param>
        void Write(string message, string category, EventLogEntryType type);

        #endregion
        #region WriteLine
        /// <summary>
        /// Writes a category name and message to the trace listeners in the Listeners collection.
        /// </summary>
        /// <param name="message">A message to write.</param>
        /// <param name="category">A category name used to organize the output.</param>
        /// <param name="type">The event log type.</param>
        void WriteLine(string message, string category, EventLogEntryType type);
        #endregion
    }
}
