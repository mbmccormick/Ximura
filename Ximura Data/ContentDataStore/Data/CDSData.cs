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
using Ximura.Data;
using CH = Ximura.Common;
using RH = Ximura.Reflection;
using AH = Ximura.AttributeHelper;
#endregion // using
namespace Ximura.Data
{
    /// <summary>
    /// The CDSData class is used by the CDSHelper to hold the information for a request.
    /// </summary>
    public struct CDSData
    {
        #region Get() - Factory method
        /// <summary>
        /// This factory method can be used to Get a request object and set its 
        /// parameters
        /// </summary>
        /// <returns>Returns an object from the pool.</returns>
        public static CDSData Get(CDSAction action, Guid? CID, Guid? VID)
        {
            return Get(action, CID, VID, JobPriority.Normal);
        }

        /// <summary>
        /// This factory method can be used to Get a request object and set its 
        /// parameters
        /// </summary>
        /// <returns>Returns an object from the pool.</returns>
        public static CDSData Get(CDSAction action, Guid? CID, Guid? VID, JobPriority priority)
        {
            CDSData rq = new CDSData();
            rq.ByReference = false;

            if (CID.HasValue)
                rq.IDContent = CID.Value;

            if (VID.HasValue)
                rq.IDVersion = VID.Value;

            rq.Action = action;
            rq.Priority = priority;
            return rq;
        }

        /// <summary>
        /// This factory method can be used to Get a request object and set its 
        /// parameters
        /// </summary>
        /// <returns>Returns an object from the pool.</returns>
        public static CDSData Get(CDSAction action, string refType, string refValue)
        {
            return Get(action, refType, refValue, JobPriority.Normal);
        }

        /// <summary>
        /// This factory method can be used to Get a request object and set its 
        /// parameters
        /// </summary>
        /// <returns>Returns an object from the pool.</returns>
        public static CDSData Get(CDSAction action, string refType, string refValue, JobPriority priority)
        {
            CDSData rq = new CDSData();

            rq.ByReference = true;
            rq.RefType = refType;
            rq.RefValue = refValue;
            rq.Action = action;
            rq.Priority = priority;

            return rq;
        }

        /// <summary>
        /// This factory method can be used to Get a request object and set its 
        /// parameters
        /// </summary>
        /// <returns>Returns an object from the pool.</returns>
        public static CDSData Get(CDSAction action)
        {
            return Get(action, JobPriority.Normal);
        }

        /// <summary>
        /// This factory method can be used to Get a request object and set its 
        /// parameters
        /// </summary>
        /// <returns>Returns an object from the pool.</returns>
        public static CDSData Get(CDSAction action, JobPriority priority)
        {
            CDSData rq = new CDSData();
            rq.Action = action;
            rq.Priority = priority;
            return rq;
        }
        #endregion

        #region Priority
        /// <summary>
        /// This is the priority for the request.
        /// </summary>
        public JobPriority Priority
        {
            get;
            private set;
        }
        #endregion
        #region RequestID
        /// <summary>
        /// This is the unique request ID.
        /// </summary>
        public Guid RequestID
        {
            get;
            private set;
        }
        #endregion

        #region Action
        /// <summary>
        /// This is the specific action for the CDS to process.
        /// </summary>
        public CDSAction Action
        {
            get;
            private set;
        }
        #endregion

        #region ByReference
        /// <summary>
        /// This boolean property indicates whether the request is by reference.
        /// </summary>
        public bool ByReference
        {
            get;
            private set;
        }
        #endregion
        #region RefType
        /// <summary>
        /// This is the reference type for the entity.
        /// </summary>
        public string RefType
        {
            get;
            private set;
        }
        #endregion
        #region RefValue
        /// <summary>
        /// The is the reference key value for the entity.
        /// </summary>
        public string RefValue
        {
            get;
            private set;
        }
        #endregion

        #region IDType
        /// <summary>
        /// This is the optional type ID for the entity.
        /// </summary>
        public Guid? IDType
        {
            get;
            private set;
        }
        #endregion
        #region IDContent
        /// <summary>
        /// This is the content ID for the entity.
        /// </summary>
        public Guid? IDContent
        {
            get;
            private set;
        }
        #endregion
        #region IDVersion
        /// <summary>
        /// THis is the version ID for the entity. If this version is null the latest version will be returned for
        /// a read operation.
        /// </summary>
        public Guid? IDVersion
        {
            get;
            private set;
        }
        #endregion
    }
}
