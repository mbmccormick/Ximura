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
using System.Data.SqlClient;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.Serialization;

using Ximura;
using Ximura.Data;

using CH = Ximura.Common;
using RH = Ximura.Reflection;
using Ximura.Framework;


using Ximura.Framework;
#endregion // using
namespace Ximura.Data
{
    /// <summary>
    /// This is the folder base for the CDS Request and Response.
    /// </summary>
    public class CDSRQRSFolderBase: RQRSFolder
    {
		#region Declarations
        private Content mData;
        #endregion
		#region Constructors
		/// <summary>
		/// This is the default constuctor.
		/// </summary>
		public CDSRQRSFolderBase():base()
		{
		}
		/// <summary>
		/// This is the component model constructor.
		/// </summary>
		/// <param name="container">The base container.</param>
		public CDSRQRSFolderBase(System.ComponentModel.IContainer container): base(container)
		{
		}

		/// <summary>
		/// This is the deserialization constructor
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
        public CDSRQRSFolderBase(SerializationInfo info, StreamingContext context)
            : base(info, context)
		{
		}
		#endregion
        #region Reset()
        /// <summary>
        /// This method resets the object.
        /// </summary>
        public override void Reset()
        {
            mData = null;
            base.Reset();
        }
        #endregion // Reset()

        #region Data
        /// <summary>
        /// This is the Data for the request.
        /// </summary>
        public virtual Content Data
        {
            get
            {
                return mData;
            }
            set
            {
                mData = value;
            }
        }
        #endregion // Data
    }
}
