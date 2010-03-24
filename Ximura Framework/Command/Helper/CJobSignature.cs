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
using System.Threading;
using System.Timers;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Reflection;
using System.Drawing;
using System.Diagnostics;

using Ximura;
using Ximura.Data;
using CH = Ximura.Helper.Common;
using Ximura.Framework;
using Ximura.Framework;
#endregion // using
namespace Ximura.Framework
{
    /// <summary>
    /// The job signature class is used to hold metadata for a JobContext completion job request.
    /// </summary>
    internal class CJobSignature<CNTX, ST, RQ, RS, SET, CONF, PERF>
        where CNTX : IXimuraFSMContext, new()
        where ST : class, IXimuraFSMState
        where RQ : RQRSFolder, new()
        where RS : RQRSFolder, new()
        where SET : class, IXimuraFSMSettings<ST, CONF, PERF>, new()
        where CONF : FSMCommandConfiguration, new()
        where PERF : FSMCommandPerformance, new()
    {
        #region Declarations
        private Guid mSignatureID;
        private CNTX mContext;
        private DateTime mCreateTime = DateTime.Now;
        #endregion // Declarations
        #region Constructor
        /// <summary>
        /// This is the default constructor for the signature.
        /// </summary>
        /// <param name="context"></param>
        public CJobSignature(CNTX context)
        {
            mSignatureID = context.SignatureID.Value;
            mContext = context;
        }
        #endregion // Constructor

        #region SignatureID
        /// <summary>
        /// The job signature.
        /// </summary>
        public Guid SignatureID
        {
            get
            {
                return mSignatureID;
            }
        }
        #endregion // SignatureID

        #region Context
        /// <summary>
        /// The context
        /// </summary>
        public CNTX Context
        {
            get
            {
                return mContext;
            }
        }
        #endregion // Context
        #region CreateTime
        /// <summary>
        /// The job create time.
        /// </summary>
        public DateTime CreateTime
        {
            get
            {
                return mCreateTime;
            }
        }
        #endregion // CreateTime
        #region JobLength()
        /// <summary>
        /// The job execution length.
        /// </summary>
        /// <returns></returns>
        public TimeSpan JobLength()
        {
            return DateTime.Now.Subtract(mCreateTime);
        }
        #endregion // JobLength()

        #region ValidateContext()
        /// <summary>
        /// This method validates that the context has not been returned to the pool and used for 
        /// another job while this has been executing.
        /// </summary>
        /// <returns>Returns true if the signature IDs match.</returns>
        public bool ValidateContext()
        {
            return this.Context.SignatureID == mSignatureID;
        }
        #endregion // ValidateContext()
    }
}
