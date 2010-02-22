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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Security;
using System.Security.Cryptography;
using System.Globalization;
using System.Runtime.Serialization;

using Ximura;
using Ximura.Persistence;
using Ximura.Helper;
using CH = Ximura.Helper.Common;
using RH = Ximura.Helper.Reflection;
using AH = Ximura.Helper.AttributeHelper;
using Ximura.Data;
using Ximura.Server;
using Ximura.Command;
#endregion // using
namespace Ximura.Persistence
{
    /// <summary>
    /// The CDSData class is used by the CDSHelper to hold the information for a request.
    /// </summary>
    public partial class CDSData
    {
        #region Reset()
        /// <summary>
        /// This reset method resets the object to it's default values.
        /// </summary>
        public void Reset()
        {
            mByReference = false;
            mIDVersion = null;
            mIDContent = null;
            mRefValue = null;
            mRefType = null;
            mAction = null;
            mRequestID = Guid.NewGuid();
            mPriority = JobPriority.Normal;
        }
        #endregion // Reset()

        #region Priority
        /// <summary>
        /// This is the priority for the request.
        /// </summary>
        public JobPriority Priority
        {
            get { return mPriority; }
        }
        #endregion
        #region RequestID
        /// <summary>
        /// This is the unique request ID.
        /// </summary>
        public Guid RequestID
        {
            get { return mRequestID; }
        }
        #endregion

        #region Action
        /// <summary>
        /// This is the specific action for the CDS to process.
        /// </summary>
        public CDSStateAction Action
        {
            get { return mAction.Value; }
        }
        #endregion

        #region ByReference
        /// <summary>
        /// This boolean property indicates whether the request is by reference.
        /// </summary>
        public bool ByReference
        {
            get { return mByReference; }
        }
        #endregion

        #region RefType
        /// <summary>
        /// This is the reference type for the entity.
        /// </summary>
        public string RefType
        {
            get { return mRefType; }
        }
        #endregion
        #region RefValue
        /// <summary>
        /// The is the reference key value for the entity.
        /// </summary>
        public string RefValue
        {
            get { return mRefValue; }
        }
        #endregion

        #region IDType
        /// <summary>
        /// This is the optional type ID for the entity.
        /// </summary>
        public Guid? IDType
        {
            get { return mIDContent; }
        }
        #endregion
        #region IDContent
        /// <summary>
        /// This is the content ID for the entity.
        /// </summary>
        public Guid? IDContent
        {
            get { return mIDContent; }
        }
        #endregion
        #region IDVersion
        /// <summary>
        /// THis is the version ID for the entity. If this version is null the latest version will be returned for
        /// a read operation.
        /// </summary>
        public Guid? IDVersion
        {
            get { return mIDVersion; }
        }
        #endregion 
    }
}
