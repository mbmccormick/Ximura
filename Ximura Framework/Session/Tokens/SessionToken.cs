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
using System.ComponentModel;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Security;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Security.Permissions;

using Ximura;
using Ximura.Helper;
using Ximura.Server;

using Ximura.Command;
#endregion // using
namespace Ximura.Server
{
    /// <summary>
    /// SessionToken is used to validate session requests.
    /// </summary>
    public class SessionToken : PoolableObjectBase
    {
        #region Delegates
        private SMJobCancel dJobCancel;
        private SMJobProcess dJobProcess;
        private SMJobComplete dJobComplete;
        #endregion // Delegates
        #region Declarations

        private Guid? mSessionID;

        private SessionProfileLevel mSessionProfileLevel = SessionProfileLevel.Unauthorized;

        #endregion
        #region Constructor
        public SessionToken()
        {

        }
        ///// <summary>
        ///// This constructor creates the token for the session.
        ///// </summary>
        ///// <param name="theSession"></param>
        //internal SessionToken(Session theSession,
        //    SMJobProcess dJobProcess, SMJobCancel dJobCancel, SMJobComplete dJobComplete)
        //{
        //    mSession = theSession;
        //    ProvideSecureHandshake(theSession);

        //    dJobComplete = dJobComplete;
        //    dJobCancel = dJobCancel;
        //    dJobProcess = dJobProcess;
        //}
        #endregion

        public override void Reset()
        {
            base.Reset();

            mSessionID = null;
            mSessionProfileLevel = SessionProfileLevel.Unauthorized;
            dJobComplete = null;
            dJobCancel = null;
            dJobProcess = null;
        }

        #region Secure methods
        //public void setAccessProfile(SessionProfileLevel theLevel)
        //{
        //    mSessionProfileLevel = theLevel;

        //}
        //protected void ProvideSecureHandshake(IXimuraSessionSCM theSession)
        //{
        //    RSAParameters sessionPubKey = theSession.GetSessionPublicKey;
        //}
        #endregion

        #region Properties
        //public Guid? theSession { get { return mSession; } }
        //public SessionProfileLevel theLevel { get { return mSessionProfileLevel; } }
        #endregion

        #region VerifyJobSignature
        /// <summary>
        /// This method verifies the job signature.
        /// </summary>
        /// <param name="jobRQ"><The job to verify./param>
        /// <returns>Returns true is the job is verified.</returns>
        public bool VerifyJobSignature(JobBase jobRQ)
        {
            //JobSignature sig = jobRQ.Signature;
            //byte[] buffer = jobRQ.IDBuffer();

            return true;
        }
        #endregion // VerifyJobSignature

        public void JobCancel(Guid jobID)
        {
            dJobCancel(jobID);
        }

        public Guid JobProcess(JobBase job, bool async)
        {
            return dJobProcess(job, async);
        }

        public void JobComplete(SecurityManagerJob job, bool signal)
        {
            dJobComplete(job, signal);
        }
    }
}
