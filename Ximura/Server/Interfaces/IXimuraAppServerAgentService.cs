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
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Drawing;
using System.Threading;
using System.Reflection;
using System.Linq;
using System.IO;
using System.IO.Compression;
using System.Xml;
using System.Security.Cryptography;

using Ximura;
using Ximura.Data;
using Ximura.Helper;
using Ximura.Server;
using Ximura.Command;

using Ximura.Performance;
using AH = Ximura.Helper.AttributeHelper;
using RH = Ximura.Helper.Reflection;
using CH = Ximura.Helper.Common;
#endregion
namespace Ximura.Server
{
    /// <summary>
    /// THis interface is implemented by application server components that need the ability to register specific agents, 
    /// such as logging providers, session managers etc.
    /// </summary>
    public interface IXimuraAppServerAgentService
    {
        /// <summary>
        /// This method adds an agent to the service.
        /// </summary>
        /// <param name="holder">The agent identifying information.</param>
        void AgentAdd(XimuraServerAgentHolder holder);
        /// <summary>
        /// This method removes an agent from the service.
        /// </summary>
        /// <param name="holder">The agent identifying information.</param>
        void AgentRemove(XimuraServerAgentHolder holder);
    }
}
