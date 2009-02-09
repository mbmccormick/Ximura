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
    /// The logging provider interface is used by implemented by loggers and is used 
    /// by the logging manager to initialize logger in preperation for being inserted
    /// in the application logging collection.
    /// </summary>
    public interface IXimuraLoggingProvider : IXimuraLogging
    {
        /// <summary>
        /// This method initializes a logger
        /// </summary>
        /// <param name="settings">The logger settings.</param>
        void Initialize(IXimuraLoggerSettings settings);

        /// <summary>
        /// This method should be used to free up any resources used by the logger.
        /// </summary>
        void Deinitialize();
    }
}
