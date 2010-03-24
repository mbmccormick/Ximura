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
using Ximura.Framework;
using Ximura.Framework;

#endregion // using
namespace Ximura.Communication
{
    public class TransportContextNamedPipes : TransportContext
    {
        #region Constructor
        /// <summary>
        /// This is the default context for the PPC.
        /// </summary>
        public TransportContextNamedPipes()
            : base()
        {
        }
        #endregion

        #region DefaultScheme
        /// <summary>
        /// This override sets 'tcp' as the default scheme 
        /// </summary>
        protected override string DefaultScheme
        {
            get { return "npipe"; }
        }
        #endregion // DefaultScheme
    }
}
