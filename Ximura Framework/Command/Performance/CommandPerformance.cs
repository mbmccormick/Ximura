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
using System.Data;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Runtime.Serialization;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.XPath;
using System.Xml.Xsl;
using System.Reflection;
using System.Threading;

using Ximura;
using Ximura.Data;

using CH = Ximura.Common;
using RH = Ximura.Reflection;

using Ximura.Framework;


#endregion // using
namespace Ximura.Framework
{
    /// <summary>
    /// This method holds the standard performance indicators the a command.
    /// </summary>
    public class CommandPerformance : PerformanceCounterCollection
    {
        #region Declarations
        private long mRequestCallbacks;
        private long mRequestCallbacksCurrent;
        private long mRequest;
        private long mRequestCurrent;
        #endregion // Declarations
		#region Constructors
		/// <summary>
		/// This is the default constructor for the Content object.
		/// </summary>
		public CommandPerformance()
        {
        }
		#endregion

        public virtual void RequestCallbackStart(Guid ID)
        {
            Interlocked.Increment(ref mRequestCallbacks);
            Interlocked.Increment(ref mRequestCallbacksCurrent);
        }

        public virtual void RequestCallbackEnd(Guid ID)
        {
            Interlocked.Decrement(ref mRequestCallbacksCurrent);
        }

        public virtual void RequestStart(Guid ID)
        {
            Interlocked.Increment(ref mRequest);
            Interlocked.Increment(ref mRequestCurrent);
        }

        public virtual void RequestEnd(Guid ID)
        {
            Interlocked.Decrement(ref mRequestCurrent);
        }

    }
}
