﻿#region Copyright
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

using CH = Ximura.Common;
using Ximura.Framework;
using Ximura.Framework;

#endregion // using
namespace Ximura.Communication
{
    public class TransportCommandIPBase<CNTX> : TransportCommand<CNTX>
        where CNTX : TransportContextIP, new()
    {
        #region Constructors
        /// <summary>
        /// Empty constructor for use by the component model.
        /// </summary>
        public TransportCommandIPBase() : this(null) { }
        /// <summary>
        /// Default constructor for use by the component model.
        /// </summary>
        /// <param name="container">The container that the transport should be added to.</param>
        public TransportCommandIPBase(System.ComponentModel.IContainer container)
            : base(container)
        {
        }
        #endregion
    }
}
