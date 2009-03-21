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
    /// The internet protocol command is the base command for protocols that use
    /// internet based messaging such as HTTP/SIP/SMTP etc.
    /// </summary>
    public class InternetProtocolCommand : ProtocolCommand
    {
        #region Constructors
        /// <summary>
        /// Empty constructor for use by the component model.
        /// </summary>
        public InternetProtocolCommand() : this(null) { }
        /// <summary>
        /// Default constructor for use by the component model.
        /// </summary>
        /// <param name="container">The container that the transport should be added to.</param>
        public InternetProtocolCommand(System.ComponentModel.IContainer container)
            : base(container)
        {
        }
        #endregion
    }
}
