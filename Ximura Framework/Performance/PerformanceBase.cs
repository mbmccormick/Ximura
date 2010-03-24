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
using Ximura.Helper;
using CH = Ximura.Helper.Common;
using RH = Ximura.Helper.Reflection;

using Ximura.Framework;


#endregion // using
namespace Ximura
{
    /// <summary>
    /// This is the base class for the performance counter architecture.
    /// </summary>
    public class PerformanceBase : IXimuraPerformance
    {
		#region Constructors
		/// <summary>
		/// This is the default constructor for the Content object.
		/// </summary>
        public PerformanceBase()
        {
        }
		#endregion

        #region PCID
        /// <summary>
        /// The performance counter id.
        /// </summary>
        public virtual Guid PCID
        {
            get;
            set;
        }
        #endregion // PCID
        #region AppID
        /// <summary>
        /// The command id.
        /// </summary>
        public virtual Guid AppID
        {
            get;
            set;
        }
        #endregion // CommandID
        #region ID
        /// <summary>
        /// The command id.
        /// </summary>
        public virtual Guid ID
        {
            get;
            set;
        }
        #endregion // CommandID

        #region Name
        /// <summary>
        /// The counter name.
        /// </summary>
        public virtual string Name
        {
            get;
            set;
        }
        #endregion // Name
        #region Category
        /// <summary>
        /// The category name.
        /// </summary>
        public virtual string Category
        {
            get;
            set;
        }
        #endregion // Category

    }
}
