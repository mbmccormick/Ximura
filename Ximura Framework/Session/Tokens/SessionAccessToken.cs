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
using System.Net.Mail;
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

using Ximura.Framework;

using Ximura.Framework;
#endregion // using
namespace Ximura.Framework
{
    public class SessionAccessToken : IIdentity
    {
        #region Declarations
        private JobGet<SessionJob> delGetSessionJob = null;
        private SessionJobReturn delSessionJobReturn = null;

        private Guid mSessionID;

        private SMJobProcess delJobProcess = null;

        private string mAuthenticationType = null;
        private string mName = null;
        private bool mIsAuthenticated = false;

        private SessionState mState = SessionState.Undefined;
        #endregion // Declarations

        internal SessionAccessToken(Guid sessionID)
        {
            mSessionID = sessionID;
        }


        public SessionState State
        {
            get
            {
                return mState;
            }
        }

        public Guid ID
        {
            get
            {
                return mSessionID;
            }
        }

        public SessionJob JobGet(Guid jobID, IXimuraRQRSEnvelope data, 
            CommandRSCallback RSCallback, CommandProgressCallback ProgressCallback, 
                JobSignature? signature, JobPriority priority)
        {
            if (delSessionJobReturn == null)
                throw new SecurityException("Not authenticated");

            return delGetSessionJob(ID, jobID, data, RSCallback, ProgressCallback, signature, priority);
        }

        public void JobReturn(SessionJob job)
        {
            if (delSessionJobReturn == null)
                throw new SecurityException("Not authenticated");

            delSessionJobReturn(job);
        }

        public Guid JobProcess(JobBase job, bool async)
        {
            if (delJobProcess == null)
                throw new SecurityException("Not authenticated");

            return delJobProcess(job, async);
        }

        #region IIdentity Members

        public string AuthenticationType
        {
            get { return mAuthenticationType; }
        }

        public bool IsAuthenticated
        {
            get { return mIsAuthenticated; }
        }

        public string Name
        {
            get 
            { 
                return mName; 
            }
        }

        #endregion
    }
}
