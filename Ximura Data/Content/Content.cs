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
using System.IO;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Diagnostics;
using System.Drawing;
using System.Text;

using Ximura;
using Ximura.Data;
using Ximura.Data.Serialization;
using Ximura.Helper;
using CH=Ximura.Helper.Common;
#endregion // using
namespace Ximura.Data
{
	/// <summary>
	/// This is the base abstract Ximura Content object. 
	/// </summary>
	[XimuraContentSerialization("Ximura.Data.Serialization.ContentFormatter, XimuraData")]
    [XimuraContentCachePolicy(ContentCacheOptions.VersionCheck)]
    public abstract partial class Content : ISupportInitializeNotification, IDisposable 
	{
		#region Constructors / Destructors
		/// <summary>
		/// This is the default constructor for the Content object.
		/// </summary>
        public Content() 
        {
            SetAttributes();

            Reset();        
        }
		#endregion

        #region IDisposable Members
        /// <summary>
        /// This is the dispose method.
        /// </summary>
        public virtual void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}