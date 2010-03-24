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
using System.Xml;
using System.Configuration;

using Ximura.Framework;
using Ximura.Framework;
using CH = Ximura.Helper.Common;
#endregion // using
namespace Ximura.Data
{
    /// <summary>
    /// This interface is used to share CDS settings for data connections
    /// between other commands in the command container.
    /// </summary>
    public interface ICDSSettingsService
    {
        /// <summary>
        /// This method is used to retrieve the connection string for specific catalog
        /// </summary>
        /// <param name="ConnectionMapping">Mapping Name</param>
        /// <returns>Connection string</returns>
        string ResolveConnectionString(string ConnectionMapping);
        /// <summary>
        /// Get All Connection Strings
        /// </summary>
        /// <returns>setting value</returns>
        Dictionary<string, string> GetAllConnectionStrings();
    }
}