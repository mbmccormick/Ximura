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
using Ximura.Server;
#endregion // using
namespace Ximura
{
    /// <summary>
    /// The IXimuraLogging interface provides the necessary methods to enable logging.
    /// </summary>
    public interface IXimuraLogging : IDisposable
    {
        /// <summary>
        /// This method inform the Logging Manager whether it will accept the 
        /// category for logging.
        /// </summary>
        /// <param name="category">The logging category.</param>
        /// <returns>A boolean value. True indicated the category is accepted.</returns>
        bool AcceptCategory(string category, EventLogEntryType type);
        /// <summary>
        /// Gets or sets a name for this LoggingProvider.
        /// </summary>
        string Name { get;set;}


        /// <summary>
        /// Flushes the output buffer and then closes the Listeners.
        /// </summary>
        void Close();
        /// <summary>
        /// Flushes the output buffer and causes buffered data to write to the Listeners
        /// collection.
        /// </summary>
        void Flush();

        #region Fail
        /// <summary>
        /// Emits an error message and a detailed error message.
        /// </summary>
        /// <param name="message">A message to emit.</param>
        /// <param name="detailMessage">A detailed message to emit.</param>
        void Fail(System.String message, System.String detailMessage);
        /// <summary>
        /// Emits the specified error message.
        /// </summary>
        /// <param name="message">A message to emit.</param>
        void Fail(System.String message);
        #endregion

        /// <summary>
        /// Increases the current IndentLevel by one.
        /// </summary>
        void Indent();
        /// <summary>
        /// Decreases the current IndentLevel by one.
        /// </summary>
        void Unindent();

        #region Write
        /// <summary>
        /// Writes a category name and the value of the object's ToString method to 
        /// the trace listeners in the Listeners collection.
        /// </summary>
        /// <param name="value">An object whose name is sent to the Listeners.</param>
        /// <param name="category">A category name used to organize the output.</param>
        void Write(System.Object value, System.String category);
        /// <summary>
        /// Writes a category name and message to the trace listeners in the Listeners
        ///  collection.
        /// </summary>
        /// <param name="message">A message to write. </param>
        void Write(System.String message);
        /// <summary>
        /// Writes a category name and message to the trace listeners in the Listeners
        ///  collection.
        /// </summary>
        /// <param name="message">A message to write. </param>
        /// <param name="category">A category name used to organize the output.</param>
        void Write(System.String message, System.String category);
        /// <summary>
        /// Writes the value of the object's ToString method to the trace listeners
        /// in the Listeners collection.
        /// </summary>
        /// <param name="value">An object whose name is sent to the Listeners.</param>
        void Write(System.Object value);
        #endregion
        #region WriteLine
        /// <summary>
        /// Writes a category name and the value of the object's ToString method to the trace 
        /// listeners in the Listeners collection.
        /// </summary>
        /// <param name="value">An object whose name is sent to the Listeners.</param>
        /// <param name="category">A category name used to organize the output.</param>
        void WriteLine(System.Object value, System.String category);
        /// <summary>
        /// Writes a category name and message to the trace listeners in the Listeners collection.
        /// </summary>
        /// <param name="message">A message to write.</param>
        /// <param name="category">A category name used to organize the output.</param>
        void WriteLine(System.String message, System.String category);
        /// <summary>
        /// Writes the value of the object's ToString method to the trace listeners
        /// in the Listeners collection.
        /// </summary>
        /// <param name="value">An object whose name is sent to the Listeners. </param>
        void WriteLine(System.Object value);
        /// <summary>
        /// Writes a category name and message to the trace listeners in the
        /// Listeners collection.
        /// </summary>
        /// <param name="message">A message to write.</param>
        void WriteLine(System.String message);
        #endregion

        /// <summary>
        /// Gets or sets a value indicating whether Flush should be called on the 
        /// Listeners after every write.
        /// </summary>
        bool AutoFlush { get; set;}
        /// <summary>
        /// Gets or sets the indent level.
        /// </summary>
        int IndentLevel { get;set;}
        /// <summary>
        /// Gets or sets the number of spaces in an indent.
        /// </summary>
        int IndentSize { get;set;}
    }
}
