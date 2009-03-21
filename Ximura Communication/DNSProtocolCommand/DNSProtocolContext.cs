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
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Reflection;
using System.Drawing;
using System.Diagnostics;

using XIMS;
using XIMS.Helper;

using XIMS.Applications;
using XIMS.Applications.Command;
#endregion // using
namespace XIMS.Communication
{
    /// <summary>
    /// This is the context for the DNS protocol.
    /// </summary>
    public class DNSProtocolContext : ProtocolContext
    {
        #region Constructors
        /// <summary>
        /// Empty constructor for use by the component model.
        /// </summary>
        public DNSProtocolContext() : base() { }
        #endregion
    }
}
