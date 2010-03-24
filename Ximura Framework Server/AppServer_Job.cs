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
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Drawing;
using System.Threading;
using System.Reflection;
using System.Linq;
using System.IO;
using System.IO.Compression;
using System.Xml;
using System.Security.Cryptography;

using Ximura;
using Ximura.Data;
using Ximura.Helper;
using Ximura.Framework;
using Ximura.Framework;
using AH = Ximura.Helper.AttributeHelper;
using RH = Ximura.Helper.Reflection;
using CH = Ximura.Helper.Common;
#endregion
namespace Ximura.Framework
{
    public partial class AppServer<CONFSYS, CONFCOM, PERF>
    {
        #region Declarations
        /// <summary>
        /// The active job collection.
        /// </summary>
        private JobCollection mJobActiveCollection = null;

        /// <summary>
        /// The job pool.
        /// </summary>
        protected PoolInvocator<Job> mPoolJob = null;

        protected PoolInvocator<SecurityManagerJob> mPoolSecurityManagerJob = null;
        protected PoolInvocator<CompletionJob> mPoolCompletionJob = null;

        #endregion // Declarations

        #region JobProcessStart()
        /// <summary>
        /// This method creates the job pools for the security manager.
        /// </summary>
        protected virtual void JobProcessStart()
        {
            mJobActiveCollection =
                new JobCollection(ConfigurationSystem.JobMax, ConfigurationSystem.JobCapacityOverrideLevel, false);

            mPoolJob = new PoolInvocator<Job>(delegate() { return new Job(); });
        }
        #endregion
        #region JobProcessStop()
        /// <summary>
        /// This method clears and disposes of the job pools for the security manager.
        /// </summary>
        protected virtual void JobProcessStop()
        {
            mPoolJob.Dispose();
            mPoolJob = null;
            mJobActiveCollection = null;
        }
        #endregion

        #region MAIN ENTRY POINT ->> JobProcess
        /// <summary>
        /// This method is used to submit jobs to the security manager.
        /// </summary>
        /// <param name="jobRQ">The job.</param>
        /// <param name="async">If this is set the true, the job will be processed using
        /// the thread pool; otherwise the job will be processed using the current thread.</param>
        /// <returns>The job Guid.</returns>
        protected virtual Guid JobProcess(JobBase jobRQ, bool async)
        {
            if (this.ServiceStatus != XimuraServiceStatus.Started)
                throw new XimuraServiceException("The Security Manager is not started.");

            SecurityManagerJob scmJob = null;

            try
            {
                SessionToken token = JobValidate(jobRQ);

                //Get the security manager job, or create a new job.
                if (jobRQ is SecurityManagerJob)
                    scmJob = jobRQ as SecurityManagerJob;
                else
                {
                    //scmJob = mPoolSecurityManagerJob.Get();
                    //scmJob = SecurityManagerJob.GetSecurityManagerJob(jobRQ, token);

                    scmJob = mPoolSecurityManagerJob.Get(j => j.Initialize(jobRQ, token, null, null, null));
                }

                //Resolve the command object, this will throw a SCMCommandNotFoundException is the command is not found.
                Guid cmdID = jobRQ.Data.DestinationAddress.command;
                scmJob.Command = CommandBridge.ResolveCommand(cmdID);

                //Verify the permissions.
                JobPermissionsVerify(scmJob, token);

                //Add the job to the collection. 
                //This will throw an SCMCapacityException if the job has exceeded the 
                //current capacity and high priority overflows are not enabled.
                mJobActiveCollection.Add(scmJob);

                if (async)
                    //Send to the dispatcher to await an available thread.
                    return JobProcessDispatcher(scmJob);
                else
                    return JobProcessCommand(scmJob);
            }
            catch (Exception ex)
            {
                if (scmJob != null && mJobActiveCollection.Contains(scmJob))
                    mJobActiveCollection.Remove(scmJob);

                XimuraAppTrace.WriteLine("SCM command process error: " + ex.ToString(),
                    ConfigurationSystem.Name, EventLogEntryType.Error);

                throw ex;
            }
        }
        #endregion // ProcessJob
        #region MAIN ENTRY POINT ->> JobCancel(Guid jobID)
        /// <summary>
        /// This method cancels a pending request.
        /// </summary>
        /// <param name="jobID">The job ID to cancel.</param>
        protected void JobCancel(Guid jobID)
        {
            throw new NotImplementedException("CancelJob is not currently implemented.");
        }
        #endregion

        #region JobProcessCommand
        /// <summary>
        /// This method calls the command directly to process the job.
        /// </summary>
        /// <param name="scmJob">The security manager job.</param>
        /// <returns>Returns the job id.</returns>
        private Guid JobProcessCommand(SecurityManagerJob scmJob)
        {
            if (scmJob == null)
            {
                XimuraAppTrace.WriteLine("Null job passed", "Security Manager", EventLogEntryType.Error);
                return Guid.Empty;
            }

            Guid tempID = scmJob.ID.Value;

            try
            {
                //Execute using the current thread.
                scmJob.Command.ProcessRequestSCM(scmJob);
            }
            catch (Exception ex)
            {
                XimuraAppTrace.WriteLine("ProcessCommandsJob -> Job Process error: " + ex.ToString(),
                    "SecurityManager", EventLogEntryType.Error);
            }

            JobComplete(scmJob);
            //Return the job ID to the caller.
            return tempID;
        }
        #endregion // ProcessCommandsJob
        #region JobProcessDispatcher
        /// <summary>
        /// This method queues the job for processing by the dispatcher
        /// </summary>
        /// <param name="scmJob"></param>
        /// <returns></returns>
        private Guid JobProcessDispatcher(SecurityManagerJob scmJob)
        {
            Guid id = scmJob.ID.Value;
            //Send to the dispatcher to await an available thread.
            //mDispatcher.ProcessJob(scmJob);

            if (this.ServiceStatus != XimuraServiceStatus.Started)
                throw new XimuraServiceException("The dispatcher is not started.");

            //Push the job to the queue
            mWaitingJobs.Push(scmJob, scmJob.Priority);

            //Signal that there is a job waiting to be processed.
            mWorkerThreadNeeded.AddOne();

            // Check whether we can add more threads to the pool.
            if (ThreadNeedNew)
                ThreadAddNew();

            //Update the counters with the new values
            PerformanceUpdateCounters();

            return id;
        }
        #endregion // ProcessDispatcherJob

        #region JobComplete
        /// <summary>
        /// This method signals to the calling session that the job is complete.
        /// </summary>
        /// <param name="jobRS">The dispatcher job that is complete.</param>
        protected virtual void JobComplete(SecurityManagerJob jobRS)
        {
            JobComplete(jobRS, false);
        }
        /// <summary>
        /// This method signals to the calling session that the job is complete.
        /// </summary>
        /// <param name="jobRS">The dispatcher job that is complete.</param>
        /// <param name="signal">Set this to true to manually complete a job.</param>
        protected virtual void JobComplete(SecurityManagerJob jobRS, bool signal)
        {
            if (!signal && jobRS.JobCompletionType != SCMJobCompletionType.OnExit)
                return;

            if (jobRS.IsChildJob)
            {
                //Do no wrong
                jobRS.ParentJob.ChildJobComplete(jobRS);
            }
            else
            {
                //Get the session and tell it that the job has completed.
                Guid jobID = jobRS.ID.Value;
                SessionToken token = mSessionTokens[jobRS.SessionID.Value];

                //token.theSession.OnJobComplete(jobRS.ID);
            }

            mJobActiveCollection.Remove(jobRS);

            try
            {
                if (jobRS != null && jobRS.ObjectPoolCanReturn)
                    jobRS.ObjectPoolReturn();
            }
            catch (Exception ex)
            {
                XimuraAppTrace.WriteLine("Job return error: " + ex.Message, ConfigurationSystem.Name, EventLogEntryType.Warning);
            }
        }
        #endregion
        #region JobException
        /// <summary>
        /// This method is used to signal an unexpected exception on an executing 
        /// thread in the job pool.
        /// </summary>
        /// <param name="job">The job that has caused the errors.</param>
        /// <param name="ex">The exception.</param>
        /// <param name="thread">The thread.</param>
        protected virtual void JobException(SecurityManagerJob job, Exception ex, Thread thread)
        {
            string message = ex.ToString();
            if (job != null && job.Command != null)
                message = @"Command=" + job.Command.CommandName + " -- " + message;

            XimuraAppTrace.WriteLine(message, "Thread_Error: " + thread.Name, EventLogEntryType.Error, "Dispatcher");
        }
        #endregion // JobException

        #region JobValidate
        /// <summary>
        /// This method validate the job request and checks that it is valid.
        /// </summary>
        /// <param name="jobRQ">The job request.</param>
        /// <returns>The SessionToken object for the request.</returns>
        protected virtual SessionToken JobValidate(JobBase jobRQ)
        {
            if (!mSessionTokens.ContainsKey(jobRQ.SessionID.Value))
                throw new SCMAuthenticationException("The session does not exist.");

            if (mJobActiveCollection.Contains(jobRQ))
                throw new SCMValidationException("The job is already being processed.");

            SessionToken token = mSessionTokens[jobRQ.SessionID.Value];

            if (token == null)
                throw new SCMAuthenticationException("The session token does not exist.");

            if (!token.VerifyJobSignature(jobRQ))
                throw new SCMValidationException("The signature is not valid.");

            return token;
        }
        #endregion // ValidateJob
        #region JobPermissionsVerify
        /// <summary>
        /// This method verifies a job's permissions.
        /// </summary>
        /// <param name="scmJob">The job.</param>
        /// <param name="token">The session token.</param>
        private void JobPermissionsVerify(SecurityManagerJob scmJob, SessionToken token)
        {
            //Settings.VerifyJobPermissions(scmJob, token.theSession.UserID);
        }
        #endregion // VerifyJobPermissions

        #region PurgeExistingJobs
        /// <summary>
        /// This method should purge any existing jobs for a session.
        /// </summary>
        /// <param name="token"></param>
        private void PurgeExistingJobs(SessionToken token)
        {

        }
        #endregion // PurgeExistingJobs
    }
}
