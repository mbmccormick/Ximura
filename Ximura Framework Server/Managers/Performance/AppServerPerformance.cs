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
using System.Collections;
using System.Configuration;
using System.Collections.Generic;
using System.IO;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;

using Ximura;
using Ximura.Data;
using Ximura.Command;
using Ximura.Helper;
using Ximura.Server;

using CH = Ximura.Helper.Common;
using RH = Ximura.Helper.Reflection;

#endregion // using
namespace Ximura.Server
{
    public class AppServerPerformance : PerformanceCounterCollection
    {
        #region Declarations
        private Dictionary<Guid, IXimuraPerformanceCounterCollection> mCounters;
        #endregion
		#region Constructors
		/// <summary>
		/// This is the default constructor for the Content object.
		/// </summary>
        public AppServerPerformance()
        {
            mCounters = new Dictionary<Guid, IXimuraPerformanceCounterCollection>();
        }
		#endregion

 
    }
}
