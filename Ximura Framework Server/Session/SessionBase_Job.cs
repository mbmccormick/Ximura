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
using System.Globalization;
using System.Security;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Security.Policy;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Diagnostics;

using Ximura;
using Ximura.Framework;
using Ximura.Framework;

using CH = Ximura.Common;
#endregion // using
namespace Ximura.Framework
{
    /// <summary>
    /// This class is the base.
    /// </summary>
    public abstract partial class SessionBase
    {
        #region Declarations
        private object sessionSyncObject = new object();
#if (DEBUG)
        private static Dictionary<Guid, WeakReference> WeakReferenceJobList = new Dictionary<Guid, WeakReference>();
#endif
        private Dictionary<Guid, SessionJob> SessionJobList = new Dictionary<Guid, SessionJob>();
        #endregion // Declarations

        private Action<JobBase, bool> delSessionJobProcess = null;
        private Action<JobBase> delSessionJobReturn = null;
        private Action<Guid> delSessionJobCancel = null;
        private SessionDelegate.SessionJobGet delSessionJobGet = null;
        private Func<IXimuraEnvelopeHelper> delEnvelopeHelperGet = null;

        #region ProcessRequest
        /// <summary>
        /// This method processes a system request synchronously.
        /// </summary>
        /// <param name="Data">The Envelope containing the system request.</param>
        public void ProcessRequest(IXimuraRQRSEnvelope Data)
        {
            ProcessRequest(Data, JobPriority.Normal, null);
        }
        /// <summary>
        /// This method processes a system request synchronously.
        /// </summary>
        /// <param name="Data">The Envelope containing the system request.</param>
        /// <param name="priority">The job priority.</param>
        public void ProcessRequest(IXimuraRQRSEnvelope Data, JobPriority priority)
        {
            ProcessRequest(Data, priority, null);
        }
        /// <summary>
        /// Process a synchronous request.
        /// </summary>
        /// <param name="Data">The data.</param>
        /// <param name="ProgressCallback">The progress calback delegate. 
        /// This can be used to report progress during long running processes.</param>
        public void ProcessRequest(IXimuraRQRSEnvelope Data, CommandProgressCallback ProgressCallback)
        {
            ProcessRequest(Data, JobPriority.Normal, ProgressCallback);
        }
        /// <summary>
        /// Process a synchronous request.
        /// </summary>
        /// <param name="Data">The data.</param>
        /// <param name="priority">The request priority.</param>
        /// <param name="ProgressCallback">The progress calback delegate. 
        /// This can be used to report progress during long running processes.</param>
        public void ProcessRequest(IXimuraRQRSEnvelope Data, JobPriority priority,
            CommandProgressCallback ProgressCallback)
        {
            RQRSFolder rq = Data.Request;

            //if (rq != null && rq.Culture == null && this.SessionCulture != null)
            //    rq.Culture = this.SessionCulture;

            SessionJob job = ProcessRQAsyncInternal(Guid.NewGuid(), Data, null, ProgressCallback, priority);

            job.WaitForCompletion();
        }
        #endregion
        #region ProcessRequestAsync
        /// <summary>
        /// Process an asychronous request.
        /// </summary>
        /// <param name="data">The data</param>
        /// <param name="RSCallback">The call back completion delegate.</param>
        /// <returns>The job guid.</returns>
        public Guid ProcessRequestAsync(IXimuraRQRSEnvelope data, CommandRSCallback RSCallback)
        {
            Guid jobID = Guid.NewGuid();
            ProcessRQAsyncInternal(Guid.NewGuid(), data, RSCallback, null, JobPriority.Normal);
            return jobID;
        }
        /// <summary>
        /// Process an asychronous request.
        /// </summary>
        /// <param name="data">The data</param>
        /// <param name="RSCallback">The call back completion delegate.</param>
        /// <param name="ProgessCallback">The request progress delegate.</param>
        /// <param name="priority">The request priority.</param>
        /// <returns>The job guid.</returns>
        public Guid ProcessRequestAsync(IXimuraRQRSEnvelope data, CommandRSCallback RSCallback,
            CommandProgressCallback ProgessCallback, JobPriority priority)
        {
            Guid jobID = Guid.NewGuid();
            ProcessRQAsyncInternal(jobID, data, RSCallback, ProgessCallback, priority);
            return jobID;
        }
        /// <summary>
        /// Process an asychronous request.
        /// </summary>
        /// <param name="jobID">The job ID.</param>
        /// <param name="data">The data</param>
        /// <param name="RSCallback">The call back completion delegate.</param>
        /// <returns>The job guid.</returns>
        public Guid ProcessRequestAsync(Guid jobID, IXimuraRQRSEnvelope data,
            CommandRSCallback RSCallback)
        {
            ProcessRQAsyncInternal(jobID, data, RSCallback, null, JobPriority.Normal);
            return jobID;
        }
        /// <summary>
        /// Process an asychronous request.
        /// </summary>
        /// <param name="jobid">The job id.</param>
        /// <param name="data">The data</param>
        /// <param name="RSCallback">The call back completion delegate.</param>
        /// <param name="ProgessCallback">The request progress delegate.</param>
        /// <param name="priority">The request priority.</param>
        /// <returns>The job guid.</returns>
        public Guid ProcessRequestAsync(Guid jobid, IXimuraRQRSEnvelope data, CommandRSCallback RSCallback,
            CommandProgressCallback ProgessCallback, JobPriority priority)
        {
            ProcessRQAsyncInternal(jobid, data, RSCallback, ProgessCallback, priority);
            return jobid;
        }
        #endregion

        #region ProcessRQAsyncInternal
        /// <summary>
        /// This private method is used to process an asynchronous request.
        /// </summary>
        /// <param name="jobid">The job ID</param>
        /// <param name="data">The data</param>
        /// <param name="RSCallback">The call back completion delegate.</param>
        /// <param name="ProgressCallback">The request progress delegate.</param>
        /// <returns>The Session job object.</returns>
        /// <exception cref="Ximura.Framework.SCMBaseException">
        /// This method will throw a security exception on the calling thread
        /// you should be prepared to catch such an exception.</exception>
        private SessionJob ProcessRQAsyncInternal(Guid jobid, IXimuraRQRSEnvelope data,
            CommandRSCallback RSCallback, CommandProgressCallback ProgressCallback, JobPriority priority)
        {
            RQRSFolder rq = data.Request;

            //TODO: Place the temporary user details here
            data.JobUserID = Guid.Empty;
            data.JobUserReferenceID = this.UserID;
            data.JobSecurityIdentifier = null;

            //Create a new session job.
            SessionJob newJob = 
                delSessionJobGet(SessionID, jobid, data, RSCallback, ProgressCallback, CalcSig(jobid, data), priority);

            //We lock this critical section to ensure that the response
            //does not get processed before we have added the job to the job list.
            //If an exception is thrown we do not add the job to the collection.
            lock (sessionSyncObject)
            {
                try
                {
                    //Add the job to the session list.
                    JobAdd(newJob);
                    //SessionJobList.Add(jobid, newJob);
                    //Get the Security Manager to process the request.
                    delSessionJobProcess(newJob as JobBase, true);
                }
                catch (SCMBaseException aex)
                {
                    JobRemove(jobid);

                    if (newJob != null)
                        delSessionJobReturn(newJob);

                    throw aex;
                }
                catch (Exception ex)
                {
                    JobRemove(jobid);

                    if (newJob != null)
                        delSessionJobReturn(newJob);

                    throw new SCMServerErrorException("Unexpected Security Verification Exception.", ex);
                }
            }
            return newJob;
        }
        #endregion // processRQAsyncInternal

        #region IXimuraSessionRQAsync / EnvelopeHelper
        /// <summary>
        /// This is the envelope helper that provides access to the Envelope pool.
        /// </summary>
        public IXimuraEnvelopeHelper EnvelopeHelper
        {
            get 
            {
                if (delEnvelopeHelperGet == null)
                    throw new Exception();

                return delEnvelopeHelperGet(); 
            }
        }
        #endregion

        #region OnJobComplete(Guid jobID)
        /// <summary>
        /// This internal method is called when a job has completed.
        /// </summary>
        /// <param name="jobID">The job ID of the completed job.</param>
        internal void OnJobComplete(Guid jobID)
        {
            SessionJob sessionJob = JobRemove(jobID);

            if (sessionJob != null && sessionJob.ID == jobID)
                sessionJob.SignalCompletion();

            delSessionJobReturn(sessionJob);
        }
        #endregion // OnJobComplete(Guid jobID)

        #region CancelRequest(Guid jobID)
        /// <summary>
        /// This method cancels a pending request.
        /// </summary>
        /// <param name="jobID">The job ID to cancel.</param>
        public void CancelRequest(Guid jobID)
        {
            if (SessionJobList.ContainsKey(jobID))
                delSessionJobCancel(jobID);
        }
        #endregion

        #region JobAdd
        private void JobAdd(SessionJob newJob)
        {
            SessionJobList.Add(newJob.ID.Value, newJob);

#if (DEBUG)
            WeakReferenceJobList.Add(newJob.ID.Value, new WeakReference(newJob));
#endif
        }
        #endregion // JobAdd
        #region JobRemove
        private SessionJob JobRemove(Guid jobID)
        {
            if (!SessionJobList.ContainsKey(jobID))
                return null;

            SessionJob sessionJob;
            lock (sessionSyncObject)
            {
                sessionJob = SessionJobList[jobID];
                SessionJobList.Remove(jobID);
            }
            return sessionJob;
        }
        #endregion // JobRemove

        #region OnCommandProgress
        /// <summary>
        /// This method will be called when there is a progress update from 
        /// the command.
        /// </summary>
        /// <param name="jobID">The job id to notify progress update.</param>
        /// <param name="args">The arguments.</param>
        protected virtual void OnCommandProgress(Guid jobID, CommandProgressEventArgs args)
        {
            SessionJob sessionJob = null;

            lock (sessionSyncObject)
            {
                if (!SessionJobList.ContainsKey(jobID))
                    return;
                sessionJob = SessionJobList[jobID];
            }

            if (sessionJob == null)
                return;

            try
            {
                //If the job exists, call the progress method with the arguments passed.
                //We check that the job has not been cycled during this call by comparing the IDs
                if (sessionJob.ID == jobID && sessionJob.RequiresProgressUpdates)
                    sessionJob.ProgressCallback(null, args);
            }
            catch (Exception ex)
            {
                XimuraAppTrace.WriteLine(this.UserID + " - OnCommandProgress error:" + ex.ToString(),
                    "Session", EventLogEntryType.Error);
            }
        }
        #endregion // OnCommandProgress
    }
}
