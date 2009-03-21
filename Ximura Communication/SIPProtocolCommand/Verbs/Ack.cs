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
using System.Threading;
using System.Timers;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Reflection;
using System.Drawing;
using System.Diagnostics;

using XIMS;
using XIMS.Data;
using XIMS.Helper;
using XIMS.Applications;
using XIMS.Applications.Command;
using XIMS.Applications.Logging;
#endregion // using
namespace XIMS.Communication
{
    /// <summary>
    /// This is the Ack verb for the SIP Protocol.
    /// </summary>
    public class AckSIP: Verb
    {
        #region Constructors
        /// <summary>
        /// Empty constructor for use by the component model.
        /// </summary>
        public AckSIP() : this(null) { }

        /// <summary>
        /// Default constructor for use by the component model.
        /// </summary>
        /// <param name="container">The container that the transport should be added to.</param>
        public AckSIP(System.ComponentModel.IContainer container)
            : base(container)
        {
        }
        #endregion
    }
}
