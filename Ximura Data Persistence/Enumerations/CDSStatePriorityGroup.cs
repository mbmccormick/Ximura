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
using System.Data;
using System.Data.SqlClient;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;

using Ximura;
using Ximura.Data;
using Ximura.Helper;
using CH = Ximura.Helper.Common;
using RH = Ximura.Helper.Reflection;
using Ximura.Server;


using Ximura.Command;
#endregion // using
namespace Ximura.Persistence
{
    /// <summary>
    /// The state priority group is used to determine the order in which CDSStates are applied to the incoming requests.
    /// CDSStates will be polled for incoming requests in the order of High, Standard and finally Low.
    /// </summary>
    public enum CDSStatePriorityGroup:short
    {
        /// <summary>
        /// This priority should be used by cache managers that require access to the request before it is passed 
        /// to the persistence managers.
        /// </summary>
        High = 1,
        /// <summary>
        /// This is the default state for a standard persistence manager.
        /// </summary>
        Standard = 2,
        /// <summary>
        /// This status should primarily be used by gateway services that access remote CDS services.
        /// </summary>
        Low = 3
    }
}
