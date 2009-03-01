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
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using System.ComponentModel;
using System.Windows.Threading;
using System.Threading;

using Ximura;
using Ximura.Helper;
using Ximura.Server;
using AH = Ximura.Helper.AttributeHelper;
#endregion // using
namespace Ximura.Windows
{
    /// <summary>
    /// This enumeration displays the current status of the appserver.
    /// </summary>
    public enum AppServerStatus
    {
        /// <summary>
        /// The server has not started.
        /// </summary>
        NotStarted,
        /// <summary>
        /// The server is disabled.
        /// </summary>
        Disabled,
        /// <summary>
        /// The server is starting.
        /// </summary>
        Starting,
        /// <summary>
        /// The server is running.
        /// </summary>
        Started,
        /// <summary>
        /// The server has failed to start.
        /// </summary>
        Failed
    }
}
