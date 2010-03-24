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
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Security;
using System.Security.Cryptography;

using Ximura;
using Ximura.Helper;
using Ximura.Framework;

using Ximura.Framework;
#endregion // using
namespace Ximura.Framework
{
    /// <summary>
    /// This is the performance counter for the logging system command. By default the logging manager does not connect
    /// to the performance architecture due to the fact that this service starts before the performance manager. This class is a stub.
    /// </summary>
    public class LoggingManagerPerformance : PerformanceCounterCollection
    {
		#region Constructors
		/// <summary>
		/// This is the default constructor for the performance object.
		/// </summary>
        public LoggingManagerPerformance()
        {
        }
		#endregion
    }
}
