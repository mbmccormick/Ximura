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

using System.Text;

using Ximura;
using Ximura.Data;
using Ximura.Data;

using CH=Ximura.Common;
#endregion // using
namespace Ximura.Data
{
	/// <summary>
	/// This is the base abstract Ximura Content object. 
	/// </summary>
	[XimuraContentSerialization("Ximura.Data.ContentFormatter, XimuraData")]
    public abstract partial class Content : ContentBase, IDisposable  
	{
		#region Constructor
		/// <summary>
		/// This is the default constructor for the Content object.
		/// </summary>
        public Content() 
        {
            SetAttributes();

            Reset();        
        }
		#endregion

        #region Dispose()
        /// <summary>
        /// This is the dispose method.
        /// </summary>
        public virtual void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
        #region Dispose(bool disposing)
        private bool mDisposed = false;
        /// <summary>
        /// This is the dispose override.
        /// </summary>
        /// <param name="disposing">True when this is called by the dispose method.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!mDisposed)
            {
                if (disposing)
                {
                    mOnInitialized = null;
                    mInfo = null;
                    mEntityType = null;
                    mEntitySubType = null;
                    mPoolManager = null;
                    mPool = null;

                    //base.Dispose(disposing);
                }
                mDisposed = true;
            }
        }
        #endregion // Dispose(bool disposing)
    }
}