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

using Ximura;
using Ximura.Helper;

using CH = Ximura.Helper.Common;
using RH = Ximura.Helper.Reflection;
#endregion // using
namespace Ximura.Framework
{
    /// <summary>
    /// This enumerations determines where the AppServer looks to load the config
    /// file.
    /// </summary>
    public enum AppServerConfigOptions
    {
        /// <summary>
        /// This application does not require a configuration file
        /// </summary>
        NoConfig,
        /// <summary>
        /// The config file is hardcoded in the application and a resource
        /// </summary>
        ResourceStream,
        /// <summary>
        /// The config file is an unencrypted file in the application directory.
        /// </summary>
        File,
        /// <summary>
        /// The config file is digitally signed and is in the application 
        /// directory.
        /// </summary>
        FileDigitallySigned,
        /// <summary>
        /// The file is encrypted and is in the application directory.
        /// </summary>
        FileEncrypted
    }
}
