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
using System.IO;
using System.Xml;
using System.Collections.Specialized;
using System.Configuration;

using Ximura;
using Ximura.Helper;
#endregion // using
namespace Ximura.Server
{
    /// <summary>
    /// This enumeration determines which collection the AppSettings exposes through its collection.
    /// </summary>
    public enum AppServerConfigSettingsType
    {
        /// <summary>
        /// This should only expose the private control settings
        /// </summary>
        ControlCollection,
        /// <summary>
        /// This should only expose the command settings
        /// </summary>
        CommandCollection
    }
}
