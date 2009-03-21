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
    /// <summary>
    /// This is the callback request for the transport command. Currently this class is not used, but is defined for completeness.
    /// </summary>
    public class TransportCommandCBRequest : RQRSFolder
    {
        #region Constructors
        /// <summary>
        /// This is the default constuctor.
        /// </summary>
        public TransportCommandCBRequest()
            : base()
        {
        }
        /// <summary>
        /// This is the component model constructor.
        /// </summary>
        /// <param name="container">The base container.</param>
        public TransportCommandCBRequest(System.ComponentModel.IContainer container)
            : base(container)
        {
        }
        /// <summary>
        /// This is the deserialization constructor
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public TransportCommandCBRequest(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
        #endregion
    }
}
