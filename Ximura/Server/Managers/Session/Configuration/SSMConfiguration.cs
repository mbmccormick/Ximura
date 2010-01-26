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
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Security;
using System.Security.Cryptography;
using System.Runtime.Serialization;

using Ximura;
using Ximura.Helper;
using Ximura.Server;

using Ximura.Command;
#endregion // using
namespace Ximura.Server
{
    /// <summary>
    /// This configuration class holds the settings for the system session manager.
    /// </summary>
    public class SSMConfiguration: CommandConfiguration
    {
        #region Constructor
        /// <summary>
        /// The default constructor
        /// </summary>
        public SSMConfiguration()  { }

        /// <summary>
        /// This is the deserialization constructor. 
        /// </summary>
        /// <param name="info">The Serialization info object that contains all the relevant data.</param>
        /// <param name="context">The serialization context.</param>
        public SSMConfiguration(SerializationInfo info, StreamingContext context)
            :
            base(info, context) { }
        #endregion

    }
}
