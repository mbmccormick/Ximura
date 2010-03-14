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
using System.Linq;
using System.Linq.Expressions;
using System.Data;
using System.Data.SqlClient;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.Serialization;

using Ximura;
using Ximura.Data;
using Ximura.Helper;
using CH = Ximura.Helper.Common;
using RH = Ximura.Helper.Reflection;
using Ximura.Server;


using Ximura.Command;
#endregion // using
namespace Ximura.Persistence
{
    /// <summary>
    /// This class contains the information for a CDS request.
    /// </summary>
    public class CDSRequestFolder : CDSRQRSFolderBase
    {
		#region Declarations
        #endregion
		#region Constructors
		/// <summary>
		/// This is the default constuctor.
		/// </summary>
		public CDSRequestFolder():base()
		{
		}
		/// <summary>
		/// This is the component model constructor.
		/// </summary>
		/// <param name="container">The base container.</param>
		public CDSRequestFolder(System.ComponentModel.IContainer container): base(container)
		{
		}

		/// <summary>
		/// This is the deserialization constructor
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
        public CDSRequestFolder(SerializationInfo info, StreamingContext context)
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
            InternalCall = false;
            ByReference = false;
            DataReferenceValue = null;
            DataReferenceType = null;

            DataContentID = null;
            DataVersionID = null;
            DataTypeID = null;

            DataType = null;

            BrowseExpression = null;

            base.Reset();
        }
        #endregion // Reset()

        #region BrowseExpression
        /// <summary>
        /// This is the expression tree passed to the Persistence Manager to evaluate for a browse request.
        /// </summary>
        public Expression BrowseExpression
        {
            get;
            set;
        }
        #endregion // BrowseExpression
        
        #region ByReference
        /// <summary>
        /// This method determines whether the request is by reference.
        /// The default is false.
        /// </summary>
        public bool ByReference
        {
            get;
            set;
        }
        #endregion // ByReference
        #region DataReferenceType
        /// <summary>
        /// This is the reference type field used for reading by reference.
        /// </summary>
        public string DataReferenceType
        {
            get;
            set;
        }
        #endregion // DataReferenceType
        #region DataReferenceValue
        /// <summary>
        /// This is the reference field used for reading by reference.
        /// </summary>
        public string DataReferenceValue
        {
            get;
            set;
        }
        #endregion // DataReference

        #region DataType
        /// <summary>
        /// The data type of the request.
        /// </summary>
        public Type DataType
        {
            get;
            set;
        }
        #endregion // DataType
        #region DataTypeID
        /// <summary>
        /// This is the data request content ID
        /// </summary>
        public Guid? DataTypeID
        {
            get;
            set;
        }
        #endregion // DataContentID
        #region DataContentID
        /// <summary>
        /// This is the data request content ID
        /// </summary>
        public Guid? DataContentID
        {
            get;
            set;
        }
        #endregion // DataContentID
        #region DataVersionID
        /// <summary>
        /// This is the data request version ID
        /// </summary>
        public Guid? DataVersionID
        {
            get;
            set;
        }
        #endregion // DataVersionID

        #region InternalCall
        /// <summary>
        /// This property is used to identify an internal call originating from within the CDS.
        /// </summary>
        public bool InternalCall
        {
            get;
            set;
        }
        #endregion // InternalCall
    }
}
