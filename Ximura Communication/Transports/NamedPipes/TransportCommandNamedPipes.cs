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
using System.IO.Pipes;
using System.Threading;
using System.Timers;
using System.Data;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Reflection;
using System.Drawing;
using System.Diagnostics;
using System.Runtime.Serialization;

using Ximura;
using Ximura.Data;
using Ximura.Helper;
using CH = Ximura.Helper.Common;
using Ximura.Server;
using Ximura.Command;

#endregion // using
namespace Ximura.Communication
{
#if (DEBUG)
    [XimuraAppConfiguration(ConfigurationLocation.Resource,
        "xmrres://XimuraComm/Ximura.Communication.TransportConfiguration/Ximura.Communication.TransportCommand.Configuration.TransportConfiguration_Default.xml")]
#else
    [XimuraAppConfiguration(ConfigurationLocation.Hybrid, 
        "xmrres://XimuraComm/Ximura.Communication.TransportConfiguration/Ximura.Communication.TransportCommand.Configuration.TransportConfiguration_Default.xml")]
#endif
    [XimuraAppModule("50B2C014-AFD7-4a97-AFC4-D7B759170A76", "TransportCommandNamedPipes")]
    public class TransportCommandNamedPipes : TransportCommand<TransportContextNamedPipes>
    {
        #region Constructor
        /// <summary>
        /// This is the default context for the PPC.
        /// </summary>
        public TransportCommandNamedPipes()
            : base()
        {
        }
        #endregion
    }
}
