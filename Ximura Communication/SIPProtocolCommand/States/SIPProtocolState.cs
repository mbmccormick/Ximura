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
using System.Diagnostics;
using System.Data;
using System.Reflection;
using System.ComponentModel;
using System.Collections.Generic;

using XIMS;
using XIMS.Data;
using CH = XIMS.Helper.Common;
using XIMS.Helper;
using XIMS.Applications;
using XIMS.Applications.Data;
using XIMS.Applications.Security;
using XIMS.Applications.Command;

#endregion // using
namespace XIMS.Communication
{
    /// <summary>
    /// This is the base state for the SIP protocol.
    /// </summary>
    public class SIPProtocolState : ProtocolState
    {
		#region Constructors
		/// <summary>
		/// This is the default constructor.
		/// </summary>
		public SIPProtocolState():this(null){}
		/// <summary>
		/// This is the component model constructor.
		/// </summary>
		/// <param name="container">The container</param>
        public SIPProtocolState(IContainer container)
            : base(container)
		{
		}
		#endregion // Constructors
    }
}
