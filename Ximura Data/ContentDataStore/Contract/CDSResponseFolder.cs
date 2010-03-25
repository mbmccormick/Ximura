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
    public class CDSResponseFolder : CDSRQRSFolderBase
    {
		#region Declarations
        private Guid? mNewVersionID;
        private Guid? mCurrentVersionID;
        private Guid? mCurrentContentID;
		#endregion
		#region Constructors
		/// <summary>
		/// This is the default constuctor.
		/// </summary>
		public CDSResponseFolder():base()
		{
		}
		/// <summary>
		/// This is the component model constructor.
		/// </summary>
		/// <param name="container">The base container.</param>
		public CDSResponseFolder(System.ComponentModel.IContainer container): base(container)
		{
		}

		/// <summary>
		/// This is the deserialization constructor
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
        public CDSResponseFolder(SerializationInfo info, StreamingContext context)
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
            mCurrentVersionID = null;
            mCurrentContentID = null;
            mNewVersionID = null;
            base.Reset();
        }
        #endregion // Reset()

        #region NewVersionID
        /// <summary>
        /// This is the new version ID property.
        /// </summary>
        public Guid? NewVersionID
        {
            get { return mNewVersionID; }
            set { mNewVersionID = value; }
        }
        #endregion // NewVersionID
        #region CurrentVersionID
        /// <summary>
        /// This property is used by the CDS Version check operation.
        /// </summary>
        public Guid? CurrentVersionID
        {
            get { return mCurrentVersionID; }
            set { mCurrentVersionID = value; }
        }
        #endregion // CurrentVersionID
        #region CurrentContentID
        /// <summary>
        /// This property is used by the CDS Version check operation.
        /// </summary>
        public Guid? CurrentContentID
        {
            get { return mCurrentContentID; }
            set { mCurrentContentID = value; }
        }
        #endregion // CurrentVersionID
    }
}
