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
using System.IO;
using System.Xml;
using System.Collections.Specialized;
using System.Configuration;

using Ximura;
using Ximura.Helper;
#endregion // using
namespace Ximura.Framework
{
    /// <summary>
    /// This interface is used by a control to retrieve his application settings
    /// </summary>
    public interface IXimuraAppSettings : IXimuraConfigurationSystemExtended
    {
        /// <summary>
        /// Name/Value Collection of AppSettings
        /// </summary>
        NameValueCollection AppSettings { get; }
    }
}
