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
using System.Globalization;
using System.Security;
using System.Security.Cryptography;
using System.Collections;
using System.Threading;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

using Ximura;
using Ximura.Framework;
using Ximura.Framework;

using CH=Ximura.Common;
#endregion // using
namespace Ximura.Framework
{
	/// <summary>
	/// The CompletionJob object is used to run multiple parallel jobs within a 
	/// command and then to signal using a single callback when all the jobs
	/// have completed, or when the job has timed out.
	/// </summary>
	public class CompletionJob : CompletionJobWrapperBase, IXimuraSessionRQAsyncWithDependency
	{
		#region Static JobPool Implementation
//        #region Static declarations
//        private static PoolInvocator<CompletionJob> sJobPool;
//        private static bool sPoolDisposed = false;
//        private static bool sPoolInitiated = false;
//        #endregion // Static declarations
//        #region Initialize and Dispose
//        /// <summary>
//        /// This internal method initializes the pool.
//        /// </summary>
//        internal static void InitializePool()
//        {
//            //sJobPool = new XimuraObjectPool(
//            //    XimuraObjectPoolType.StandardPool,typeof(CompletionJob),
//            //    "CompletionJob Pool","",false);
//            sJobPool = new PoolInvocator<CompletionJob>(internalCreateJobRequest, internalGetPool);
//            sPoolInitiated = true;
//            sPoolDisposed = false;
//        }

//        #region Static -> internalCreateCDSRequest()
//        /// <summary>
//        /// This internal static method is used by the pool manager to create a new object.
//        /// </summary>
//        /// <returns></returns>
//        private static CompletionJob internalCreateJobRequest()
//        {
//            return new CompletionJob();
//        }

//        private static IXimuraPool internalGetPool()
//        {
//            return sJobPool;
//        }
//        #endregion // Static -> internalCreateCDSRequest()

//        /// <summary>
//        /// This internal method disposes of the pool.
//        /// </summary>
//        internal static void DisposePool()
//        {
//            sPoolInitiated = false;
//            sPoolDisposed = true;
//            sJobPool.Dispose();
//            sJobPool = null;
//        }
//        #endregion // Initialize and Dispose

//        #region GetCompletionJob
//        ///// <summary>
//        ///// This method return a CompletionJob object from the object pool.
//        ///// </summary>
//        ///// <param name="job">The security manager job.</param>
//        ///// <param name="callback">This is the job completion callback.</param>
//        ///// <returns>A new CompletionJob object.</returns>
//        //internal static CompletionJob GetCompletionJob(
//        //    SecurityManagerJob job, CompletionJobCallBack callback)
//        //{
//        //    return GetCompletionJob(job, callback, null, false,0);
//        //}
//        ///// <summary>
//        ///// This method return a CompletionJob object from the object pool.
//        ///// </summary>
//        ///// <param name="job">The security manager job.</param>
//        ///// <param name="callback">This is the job completion callback.</param>
//        ///// <param name="AutoComplete">Identifies whether the jobs will be executed immediately.</param>
//        ///// <param name="throttlingThreshold">This parameter specifies the maximum number of jobs that will be processed in parallel. If this is
//        ///// set to 0 or below throttling will not be implemented.</param>
//        ///// <returns>A new CompletionJob object.</returns>
//        //internal static CompletionJob GetCompletionJob(
//        //    SecurityManagerJob job, CompletionJobCallBack callback, bool AutoComplete, int throttlingThreshold)
//        //{
//        //    return GetCompletionJob(job, callback, null, AutoComplete, throttlingThreshold);
//        //}
//        ///// <summary>
//        ///// This method return a CompletionJob object from the object pool.
//        ///// </summary>
//        ///// <param name="job">The security manager job.</param>
//        ///// <param name="callback">This is the job completion callback.</param>
//        ///// <param name="state">This is the job state.</param>
//        ///// <returns>A new CompletionJob object.</returns>
//        //internal static CompletionJob GetCompletionJob(
//        //    SecurityManagerJob job, CompletionJobCallBack callback, object state)
//        //{
//        //    return GetCompletionJob(job, callback, state, false,0);
//        //}
//        ///// <summary>
//        ///// This method return a CompletionJob object from the object pool.
//        ///// </summary>
//        ///// <param name="job">The security manager job.</param>
//        ///// <param name="callback">This is the job completion callback.</param>
//        ///// <param name="state">This is the job state.</param>
//        ///// <param name="AutoExecute">Identifies whether the jobs will be executed immediately.</param>
//        ///// <param name="throttlingThreshold">This parameter specifies the maximum number of jobs that will be processed in parallel. If this is
//        ///// set to 0 or below throttling will not be implemented.</param>
//        ///// <returns>A new CompletionJob object.</returns>
//        //internal static CompletionJob GetCompletionJob(
//        //    SecurityManagerJob job, CompletionJobCallBack callback, object state, 
//        //    bool AutoExecute, int throttlingThreshold)
//        //{
//        //    return GetCompletionJob(job, callback, state, AutoExecute, throttlingThreshold, false);       
//        //}
//        /// <summary>
//        /// This method return a CompletionJob object from the object pool.
//        /// </summary>
//        /// <param name="job">The security manager job.</param>
//        /// <param name="callback">This is the job completion callback.</param>
//        /// <param name="state">This is the job state.</param>
//        /// <param name="AutoExecute">Identifies whether the jobs will be executed immediately.</param>
//        /// <param name="throttlingThreshold">This parameter specifies the maximum number of jobs that will be processed in parallel. If this is
//        /// set to 0 or below throttling will not be implemented.</param>
//        /// <param name="trace">This boolean method determines whether the completion job
//        /// will collate trace information during the processing of the completion job.</param>
//        /// <returns>A new CompletionJob object.</returns>
//        internal static CompletionJob GetCompletionJob(
//            SecurityManagerJob job, CompletionJobCallBack callback, object state,
//            bool AutoExecute, int throttlingThreshold, bool trace)
//        {
//#if (USEJOBPOOL)
//            CompletionJob newJob = sJobPool.ObjectGet() as CompletionJob;
//#else
//            CompletionJob newJob = new CompletionJob();
//#endif
//            try
//            {
//                newJob.Initialize(job, callback, state, AutoExecute, throttlingThreshold, trace);

//                return newJob;
//            }
//            catch (Exception ex)
//            {
//                if (newJob!=null)
//                    CompletionJobReturn(newJob);
//                throw ex;
//            }
//        }
//        #endregion // GetSecurityManagerJob
//        #region CompletionJobReturn
//        /// <summary>
//        /// This method returns a used SecurityManagerJob object to the pool for further reuse.
//        /// </summary>
//        /// <param name="completedJob">The job to return.</param>
//        internal static void CompletionJobReturn(CompletionJob completedJob)
//        {
//            completedJob.Reset();
//#if (USEJOBPOOL)
//            sJobPool.ObjectReturn(completedJob);
//#endif		
//        }
//        #endregion // SecurityManagerJobReturn
		#endregion

		#region Declarations
        private ManualResetEvent mreThrottle = new ManualResetEvent(true);

        private Guid CompletionJobID;
        private StringBuilder mTraceBuilder = null;

        private int jobsSubmitted = 0;
        private int jobsReturned = 0;

        private int errorCount = 0;
        private int throttleThreshold = -1;
        private int executingJobs = 0;

        private Dictionary<string, Guid?> mDependencyList = null;

        private Queue<JobHolder> mThrottlingQueue = null;

        private List<Guid> errorList = new List<Guid>();

		private CommandRSCallback internalCallback;
		
		private CompletionJobStatus mStatus;

		private CompletionJobCallBack CompletionJobComplete;

		private bool autoExecute = false;
        private bool mJobShouldTrace = false;

		private object mState;

        private Action<CompletionJob> delCompletionJobReturn;
		#endregion // Declarations
		#region Constructor
		/// <summary>
		/// This is the default constructor.
		/// </summary>
		internal CompletionJob():base(null)
        {
            CompletionJobID = Guid.NewGuid();
        }
		#endregion // Constructor
		#region InitializeJob()
		/// <summary>
		/// This overriden method is used to initialize any variables before the initial
		/// Reset() is called.
		/// </summary>
		protected override void InitializeJob()
		{
			base.InitializeJob();
			internalCallback = new CommandRSCallback(RQCallback);
		}
		#endregion // InitializeJob()
		#region Reset()
		/// <summary>
		/// This private method reset the Completion job.
		/// </summary>
		public override void Reset()
		{
			base.Reset();
			mState = null;
			mStatus = CompletionJobStatus.Unset;
			CompletionJobComplete = null;
            //internalCallback = null;
            ThrottleSet();
            autoExecute = false;
            mJobShouldTrace = false;

            delCompletionJobReturn = null;
		}
		#endregion // Reset()
		#region Initialize()
		/// <summary>
		/// This private method initializes the completion job.
		/// </summary>
		/// <param name="job">The parent job.</param>
		/// <param name="callback">The completion callback.</param>
		/// <param name="state">The object state.</param>
		protected void Initialize(SecurityManagerJob job, CompletionJobCallBack callback, object state)
		{
			Initialize(job, callback, state, false, 0, false);
		}
		/// <summary>
		/// This private method initializes the completion job.
		/// </summary>
		/// <param name="job">The parent job.</param>
		/// <param name="callback">The completion callback.</param>
		/// <param name="state">The object state.</param>
		/// <param name="AutoExecute">This method identifies whether the jobs will execute automatically and only signal after the complete method has been set.</param>
        /// <param name="throttleThreshold">The throttle threshold is the amount of queued jobs that 
        /// will trigger the throttle flag to be set.</param>
        protected void Initialize(SecurityManagerJob job, CompletionJobCallBack callback, 
            object state, bool AutoExecute, int throttleThreshold)
		{
            Initialize(job, callback, state, AutoExecute, throttleThreshold, false);
        }
        /// <summary>
        /// This private method initializes the completion job.
        /// </summary>
        /// <param name="job">The parent job.</param>
        /// <param name="callback">The completion callback.</param>
        /// <param name="state">The object state.</param>
        /// <param name="AutoExecute">This method identifies whether the jobs will execute automatically and only signal after the complete method has been set.</param>
        /// <param name="throttleThreshold">The throttle threshold is the amount of queued jobs that 
        /// will trigger the throttle flag to be set.</param>
        /// <param name="trace">The trace flag determines whether the completion job should submit trace
        /// data during the job processing.</param>
        protected void Initialize(SecurityManagerJob job, CompletionJobCallBack callback, 
            object state, bool AutoExecute, int throttleThreshold, bool trace)
        {
			mParentJob = job;
			CompletionJobComplete = callback;
			mState = state;
			autoExecute=AutoExecute;
			if (autoExecute)
				mStatus = CompletionJobStatus.Submitting;

            mJobShouldTrace = trace;

            if (mDependencyList != null)
                mDependencyList.Clear();
            errorList.Clear();

            errorCount = 0;
            this.throttleThreshold = throttleThreshold;
            if (throttleThreshold > 0)
            {
                if (mThrottlingQueue == null)
                    mThrottlingQueue = new Queue<JobHolder>();
                else
                    mThrottlingQueue.Clear();
            }

            ThrottleSet();

            executingJobs = 0;
		}
		#endregion // Initialize

		#region Status
		/// <summary>
		/// This is the completion job status.
		/// </summary>
		public CompletionJobStatus Status
		{
			get{return mStatus;}
		}
		#endregion // Status

		#region State
		/// <summary>
		/// This is the job object state.
		/// </summary>
		public object State
		{
			get
			{
				return mState;
			}
			set
			{
				mState = value;
			}
		}
		#endregion // State

		#region Timer functionality
		/// <summary>
		/// This method is called when the job times out.
		/// </summary>
		/// <param name="state"></param>
		protected override void OnTimeOut(object state)
		{
			base.OnTimeOut(state);
			mStatus = CompletionJobStatus.TimeOut;

			CompletionJobComplete(this.ParentJob, CompletionJobStatus.TimeOut, State);
		}
		#endregion // Timer functionality
		#region SubmitComplete()
		/// <summary>
		/// This method can be called to signal that a completion job is complete.
		/// </summary>
		public void SubmitComplete()
		{
			if (!autoExecute)
				throw new JobException("SubmitComplete can only be called to completion jobs that have autocomplete set to true.");

			lock (this)
			{
				mStatus = CompletionJobStatus.Processing;
				CheckCompletionJobComplete();
			}
		}
		#endregion // SubmitComplete()

        #region Abort()
        /// <summary>
        /// This method aborts the completion job.
        /// </summary>
        public void Abort()
        {
            ThrottleSet();
        }
        #endregion // Abort()
		#region Execute
		/// <summary>
		/// This method executes all the containing jobs.
		/// </summary>
		public void Execute()
		{
			Execute(-1);
		}
        /// <summary>
		/// This method executes all the containing jobs.
		/// </summary>
		/// <param name="Timeout">The timeout period in which the job should execute in its entirity.</param>
        public void Execute(int Timeout)
        {
#if (DEBUG)
            Execute(Timeout,true);
#else
            Execute(Timeout,false);
#endif
        }
		/// <summary>
		/// This method executes all the containing jobs.
		/// </summary>
		/// <param name="Timeout">The timeout period in which the job should execute in its entirity.</param>
		public void Execute(int Timeout, bool traceOutput)
		{
            mJobShouldTrace = traceOutput;

            if (autoExecute)
            {
                mStatus = CompletionJobStatus.Processing;
                CheckCompletionJobComplete();
                return;
            }

			if (mStatus != CompletionJobStatus.Unset)
				throw new JobException("The CompletionJob has already started executing. Call Reset().");

			mStatus = CompletionJobStatus.Processing;

			foreach(Guid jobID in WorkTable.JobIDs)
			{
				SubmitJob(jobID);
			}

			if (Timeout>-1)
				SetTimer(Timeout);
		}
		#endregion // Execute
		#region SubmitJob
		/// <summary>
		/// This method submits the job.
		/// </summary>
		/// <param name="jobID"></param>
		protected void SubmitJob(Guid jobID)
		{
            JobHolder currentJob;
            lock (this)
            {
                currentJob = WorkTable[jobID] as JobHolder;
                if (currentJob == null)
                    throw new ArgumentNullException("JobHolder", "Job was null: " + jobID.ToString());

                //This job cannot be executed twice.
                if (currentJob.Executed)
                    throw new JobException("The job can only be executed once: " + jobID.ToString());

                //Check whether this is a dependent job waiting for it's parent to complete.
                //And if it is, do not process.
                if (currentJob.DependencyID != null 
                    && mDependencyList.ContainsKey(currentJob.DependencyID)
                    && mDependencyList[currentJob.DependencyID]!=currentJob.ID)
                    return;
            }

			currentJob.Executed=true;

            try
            {
                //First check whether we have a throttling threshold, and if so we will queue the job.
                lock (this)
                {
                    Interlocked.Increment(ref executingJobs);
                    if (throttleThreshold > 0 && executingJobs > throttleThreshold)
                    {
                        //Queue the job in the throttle
                        mThrottlingQueue.Enqueue(currentJob);

                        //Log that the job is now queued for processing
                        if (JobShouldTrace)
                            JobTrace(">>--? JOB THROTTLED AND QUEUED " + jobID.ToString());
                        //We are now throttling jobs, so we need to throttle any threads that
                        //call the throttle method.
                        if (throttleThreshold >0 && mThrottlingQueue.Count >= throttleThreshold)
                            ThrottleReset();
                        return;
                    }
                }

                int retriesTooBusy = SubmissionRetrySteps;

                do
                {
                    try
                    {
                        //OK, we are going to process the job
                        ParentJob.ProcessRequestAsync(jobID, currentJob.Data,
                            internalCallback, currentJob.ProgressCallback, currentJob.Priority);
                        retriesTooBusy = 0;
                    }
                    catch (SCMCapacityException scex)
                    {
                        if (retriesTooBusy == -1)
                            throw scex;

                        Thread.Sleep(SubmissionRetryWaitInMs(retriesTooBusy));

                        retriesTooBusy--;
                    }
                }
                while (retriesTooBusy > 0);

                Interlocked.Increment(ref jobsSubmitted);

                if (JobShouldTrace)
                    JobTrace("<---> JOB SUBMITTED " + jobID.ToString() 
                        + "Total Jobs Submitted=" + jobsSubmitted.ToString());


            }
            catch (Exception ex)
            {
                if (JobShouldTrace)
                    JobTrace("!! JOB SUBMISSION EXCEPTION " + jobID.ToString() + " - " + ex.Message);

                //We need to remove the job that crashed out; otherwise strange things may happen.
                //We also need to back out of any dependency jobs, and mark them as failed.
                RemoveErrorJob(currentJob);
                throw ex;
            }
		}
		#endregion // SubmitJob

        #region EnvelopeHelper
        /// <summary>
        /// The completion job EnvelopeHelper is copied from the ParentJob helper.
        /// </summary>
        public override IXimuraEnvelopeHelper EnvelopeHelper
        {
            get 
            {
                return ParentJob.EnvelopeHelper;
            }
        }
        #endregion // EnvelopeHelper


        protected virtual int SubmissionRetrySteps
        {
            get
            {
                return 5;
            }
        }

        protected virtual int SubmissionRetryWaitInMs(int step)
        {
            return step * 1000;
        }

        #region HasErrors()
        /// <summary>
        /// This method returns true if the completion job has errors.
        /// </summary>
        /// <returns></returns>
        public bool HasErrors()
        {
            lock (this)
            {
                return errorCount > 0;
            }
        }
        #endregion // HasErrors()

        #region DependencyHasErrors
        /// <summary>
        /// This method returns true if the dependency ID has been set in an error state.
        /// </summary>
        /// <param name="dependencyID"></param>
        /// <returns></returns>
        public bool DependencyIDHasErrors(string dependencyID)
        {
            lock (this)
            {
                if (!mDependencyList.ContainsKey(dependencyID))
                    return false;

                return !mDependencyList[dependencyID].HasValue;
            }
        }
        #endregion
        #region ResetDependency
        /// <summary>
        /// This method will reset the dependency.
        /// </summary>
        /// <param name="dependencyID"></param>
        public void ResetDependencyID(string dependencyID)
        {
            lock (this)
            {
                //If the key exists and does not have a value, then delete it.
                if (mDependencyList.ContainsKey(dependencyID) && 
                    !mDependencyList[dependencyID].HasValue)
                    mDependencyList.Remove(dependencyID);
            }
        }
        #endregion

        #region ThrottleSet
        /// <summary>
        /// This method sets the throttle ManualResetEven and allows any threads held in the Throttle 
        /// method to proceed.
        /// </summary>
        private void ThrottleSet()
        {
            if (JobShouldTrace)
                JobTrace("<--- THROTTLE CLEAR" + executingJobs.ToString());

            mreThrottle.Set();
        }
        #endregion // ThrottleSet
        #region ThrottleReset
        /// <summary>
        /// This method reset the throttle and holds any threads which call the Throttle method.
        /// </summary>
        private void ThrottleReset()
        {
            if (JobShouldTrace)
                JobTrace("THROTTLE TRAFFIC ---> " + executingJobs.ToString());

            mreThrottle.Reset();
        }
        #endregion // ThrottleReset
        #region IsThrottling()
        /// <summary>
        /// This method returns true if 
        /// </summary>
        /// <returns></returns>
        public bool IsThrottling()
        {
            lock (this)
            {
                return ThreadUnsafeThrottleIsSet();
            }
        }

        private bool ThreadUnsafeThrottleIsSet()
        {
            if (mThrottlingQueue == null)
                return false;

            return this.throttleThreshold != 0 && mThrottlingQueue.Count >= this.throttleThreshold;
        }
        #endregion // IsThrottling()
        #region Throttle()
        /// <summary>
        /// This method will suspend the entry thread until the throttling queue has reached 0.
        /// </summary>
        public void Throttle()
        {
            while (IsThrottling())
                if (!Throttle(100))
                    ProcessTimeOuts();
        }

        /// <summary>
        /// This method is used to identify any jobs that have timed out and are blocking the queue.
        /// </summary>
        protected virtual void ProcessTimeOuts()
        {
            lock (this)
            {
                if (!ThreadUnsafeThrottleIsSet())
                    return;

                //OK, the queue maybe blocked by a job which has crashed out or expired.
                //Loop through each job until we find one that has expired. This may free up the queue.
                foreach (JobHolder job in mThrottlingQueue)
                {
                    //If we are no longer throttling then go back to doing work. Note multiple threads
                    //may be released at the same time by the manual reset event and we want to make sure
                    //that we don't block the other threads unnecessarily.
                    if (!ThreadUnsafeThrottleIsSet())
                        return;
                    //If the job has expired then purge it.
                    if (job.Expired)
                    {
                        PurgeJob(job);
                    }
                }
            }
        }
        /// <summary>
        /// This method will suspend the entry thread until 
        /// the completion job is no longer throttling.
        /// </summary>
        /// <param name="timeOut">The time out in milliseconds specifying 
        /// the maximum amount of time that the thread should wait.</param>
        public bool Throttle(int timeOut)
        {
            return mreThrottle.WaitOne(timeOut, false);
        }
        #endregion // Throttle()

		#region ProcessRequestAsync Helper Method
		/// <summary>
		/// Process an asychronous request.
		/// </summary>
		/// <param name="jobID">The job ID. This should be set to a new Guid.</param>
		/// <param name="data">The data</param>
		/// <param name="RSCallback">The call back completion delegate.</param>
		/// <param name="ProgressCallback">The request progress delegate. Set this to null if not needed.</param>
		/// <param name="priority">The request priority.</param>
		/// <returns>The job guid.</returns>
		public override Guid ProcessRequestAsync(Guid jobID, IXimuraRQRSEnvelope data, 
			CommandRSCallback RSCallback, CommandProgressCallback ProgressCallback, JobPriority priority)
		{
			return ProcessRequestAsyncWithDependency(jobID, data, RSCallback, ProgressCallback, priority, null);
		}
		#endregion
        #region IXimuraSessionRQAsyncWithDependency Helper methods
        /// <summary>
		/// Process an asychronous request.
		/// </summary>
		/// <param name="data">The data</param>
		/// <param name="RSCallback">The call back completion delegate.</param>
		/// <param name="dependencyKey">The dependency key, if this is set to null the key is ignored.</param>
		/// <returns>The job guid.</returns>
		public Guid ProcessRequestAsyncWithDependency(IXimuraRQRSEnvelope data, CommandRSCallback RSCallback, string dependencyKey)
		{
			return ProcessRequestAsyncWithDependency(Guid.NewGuid(),data,RSCallback,null, JobPriority.Normal,dependencyKey);
		}
		/// <summary>
		/// Process an asychronous request.
		/// </summary>
		/// <param name="data">The data</param>
		/// <param name="RSCallback">The call back completion delegate.</param>
        /// <param name="ProgressCallback">The request progress delegate. Set this to null if not needed.</param>
		/// <param name="priority">The request priority.</param>
		/// <param name="dependencyKey">The dependency key, if this is set to null the key is ignored.</param>
		/// <returns>The job guid.</returns>
		public Guid ProcessRequestAsyncWithDependency(IXimuraRQRSEnvelope data, CommandRSCallback RSCallback, 
			CommandProgressCallback ProgressCallback, JobPriority priority, string dependencyKey)
		{
			return ProcessRequestAsyncWithDependency(Guid.NewGuid(), data, RSCallback, ProgressCallback, priority, dependencyKey);
		}
		/// <summary>
		/// Process an asychronous request.
		/// </summary>
		/// <param name="jobID">The job ID. This should be set to a new Guid.</param>
		/// <param name="data">The data</param>
		/// <param name="RSCallback">The call back completion delegate.</param>
		/// <param name="dependencyKey">The dependency key, if this is set to null the key is ignored.</param>
		/// <returns>The job guid.</returns>
		public Guid ProcessRequestAsyncWithDependency(Guid jobID, IXimuraRQRSEnvelope data, CommandRSCallback RSCallback, string dependencyKey)
		{
			return ProcessRequestAsyncWithDependency(jobID, data, RSCallback, null, JobPriority.Normal, dependencyKey);
		}
        		/// <summary>
		/// Process an asychronous request.
		/// </summary>
		/// <param name="jobID">The job ID. This should be set to a new Guid.</param>
		/// <param name="data">The data</param>
		/// <param name="RSCallback">The call back completion delegate.</param>
		/// <param name="ProgressCallback">The request progress delegate. Set this to null if not needed.</param>
		/// <param name="priority">The request priority.</param>
		/// <param name="dependencyKey">The dependency key, if this is set to null the key is ignored.</param>
		/// <returns>The job guid.</returns>
        public Guid ProcessRequestAsyncWithDependency(Guid jobID, IXimuraRQRSEnvelope data, CommandRSCallback RSCallback,
            CommandProgressCallback ProgressCallback, JobPriority priority, string dependencyKey)
        {
            return ProcessRequestAsyncWithDependency(jobID, data, RSCallback, ProgressCallback, 
                JobPriority.Normal, dependencyKey, null);
        }
		#endregion // Helper methods
		#region IXimuraSessionRQAsyncWithDependency Members
		/// <summary>
		/// Process an asychronous request.
		/// </summary>
		/// <param name="jobID">The job ID. This should be set to a new Guid.</param>
		/// <param name="data">The data</param>
		/// <param name="RSCallback">The call back completion delegate.</param>
		/// <param name="ProgressCallback">The request progress delegate. Set this to null if not needed.</param>
		/// <param name="priority">The request priority.</param>
		/// <param name="dependencyKey">The dependency key, if this is set to null the key is ignored.</param>
        /// <param name="ValidateRSCallBack">The delegate should contain the code to validate the callback.</param>
		/// <returns>The job guid.</returns>
		public Guid ProcessRequestAsyncWithDependency(Guid jobID, IXimuraRQRSEnvelope data, CommandRSCallback RSCallback, 
			CommandProgressCallback ProgressCallback, JobPriority priority, string dependencyKey,
            DependencyValidateRSCallback ValidateRSCallBack)
		{
			if (mStatus != CompletionJobStatus.Unset && mStatus != CompletionJobStatus.Submitting)
				throw new JobException("The CompletionJob has already started executing. Call Reset().");

            return AddJob(jobID, data, RSCallback, ProgressCallback, priority, dependencyKey, ValidateRSCallBack);
		}

		#endregion

        #region AddJob
        /// <summary>
        /// This overriden method adds a job to the collection. Jobs with a dependency ID will be
        /// queued behind earlier jobs with the same ID.
        /// </summary>
        /// <param name="newJobID">The job identifier.</param>
        /// <param name="data">The data.</param>
        /// <param name="RSCallback">The callback.</param>
        /// <param name="ProgressCallback">The progress callback.</param>
        /// <param name="priority">The job priority.</param>
        /// <param name="dependencyID">The dependency identifier.</param>
        /// <param name="ValidateRSCallBack"></param>
        /// <returns>The job ID.</returns>
        protected virtual Guid AddJob(Guid newJobID, IXimuraRQRSEnvelope data, 
            CommandRSCallback RSCallback, CommandProgressCallback ProgressCallback,
            JobPriority priority, string dependencyID, DependencyValidateRSCallback ValidateRSCallBack)
        {
            bool jobNotLinked = true;

            lock (this)
            {
                //Create the job holder object.
                JobHolder newJob = new JobHolder(newJobID, data, RSCallback, ProgressCallback, priority,
                    null, null, ValidateRSCallBack);
                //OK, let's continue as normal.
                //Add the job.
                WorkTable.Add(newJob);

                if (dependencyID != null)
                {
                    //Register the dependency job in the dependency tree.
                    jobNotLinked = RegisterLinkedJob(dependencyID, newJobID);

                    newJob.DependencyID = dependencyID;
                }

                Interlocked.Increment(ref jobRequests);

                if (JobShouldTrace)
                {
                    string dependencyInfo = "";
                    if (dependencyID != null)
                    {
                        dependencyInfo = " Dependency: " + dependencyID + " " + 
                            (string)(jobNotLinked ? "Not Linked" : "Linked");
                    }
                    JobTrace("-->" + jobRequests.ToString() + " CJ=" + CompletionJobID.ToString()
                        + " Job=" + newJobID.ToString() + dependencyInfo);
                }
            }

            if (autoExecute && jobNotLinked)
                SubmitJob(newJobID);

            return newJobID;
        }
        /// <summary>
        /// This method redirects any job requests without a dependency ID to this code with this callback.
        /// </summary>
        /// <param name="jobID">The job identifier.</param>
        /// <param name="data">The data.</param>
        /// <param name="RSCallback">The callback.</param>
        /// <param name="ProgessCallback">The progress callback.</param>
        /// <param name="priority">The job priority.</param>
        /// <returns>The job ID.</returns>
        protected override Guid AddJob(Guid jobID, IXimuraRQRSEnvelope data,
            CommandRSCallback RSCallback, CommandProgressCallback ProgessCallback,
            JobPriority priority)
        {
            return AddJob(jobID, data, RSCallback, ProgessCallback, priority, null, null);
        }
        #endregion // AddJob

        #region RegisterLinkedJob
        /// <summary>
        /// This method links the dependency job in the dependency tree.
        /// </summary>
        /// <param name="dependencyID">The dependency ID.</param>
        /// <param name="newJobID">The new job.</param>
        private bool RegisterLinkedJob(string dependencyID, Guid newJobID)
        {
            lock (this)
            {
                //Check whether the dependency list has been previously created,
                //and if not create it.
                if (mDependencyList == null)
                    mDependencyList = new Dictionary<string, Guid?>();

                //Has the dependency ID been registered?
                if (mDependencyList.ContainsKey(dependencyID))
                {
                    Guid? dependencyJobID = mDependencyList[dependencyID];
                    if (!dependencyJobID.HasValue)
                        throw new CompletionJobPrerequisiteFailureException(dependencyID);

                    //Yes, so get the first job.
                    JobHolder parentJob = WorkTable[dependencyJobID.Value] as JobHolder;
                    //Does the parent have a next job?
                    if (parentJob.NextJob.HasValue)
                    {
                        //OK, there are a queue of jobs so we have to insert ourselves at the
                        //end of the chain.
                        //Get the last job, and insert ourselves in to the end.
                        JobHolder lastJob = WorkTable[parentJob.LastJob.Value] as JobHolder;
                        lastJob.NextJob = newJobID;
                        parentJob.LastJob = newJobID;
                    }
                    else
                    {
                        //No then this will be the next job after the current job has completed.
                        parentJob.NextJob = newJobID;
                        //Also this will be the last job.
                        parentJob.LastJob = newJobID;
                    }
                    return false;
                }

                //Add the job to the dependency list collection.
                mDependencyList.Add(dependencyID, newJobID);
                return true;
            }
        }
        #endregion // RegisterLinkedJob

        #region ENTRY POINT --> RQCallback / Signal completion
        /// <summary>
        /// This is the internal callback point for the associated jobs.
        /// </summary>
        /// <param name="sender">The sender. This is usually null.</param>
        /// <param name="args">The job arguments.</param>
        private void RQCallback(object sender, CommandRSEventArgs args)
        {
            Guid jobID = args.ID.Value;
            JobHolder currentJob = null;
            Interlocked.Increment(ref jobsReturned);
            Interlocked.Decrement(ref executingJobs);

            //If the job exists then remove the job and reset the time out timer.
            //This method will call the job callback.
            try
            {
                if (RemoveJob(jobID, sender, args))
                {
                    if (CheckCompletionJobComplete())
                    {
                        ThrottleSet();
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            lock (this)
            {
                //Set the throttling status so that jobs will continue.
                if (mThrottlingQueue == null || mThrottlingQueue.Count == 0)
                {
                    ThrottleSet();
                    return;
                }
                //Get the next job out of the throttle queue
                currentJob = mThrottlingQueue.Dequeue();
                //Reset the throttling queue if this is the last job.
                if (throttleThreshold == 0 || mThrottlingQueue.Count < throttleThreshold)
                    ThrottleSet();
            }

            //Process the job from the queue.
            ParentJob.ProcessRequestAsync(currentJob.ID.Value, currentJob.Data,
                internalCallback, currentJob.ProgressCallback, currentJob.Priority);
            Interlocked.Increment(ref jobsSubmitted);
        }
        #endregion // RQCallback

        #region RemoveJob
        /// <summary>
        /// This method removes a job from the completion job.
        /// </summary>
        /// <param name="oldJobID">The old job to remove.</param>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The sender arguments.</param>
        /// <returns>Returns true if the job was successfully removed.</returns>
        protected override bool RemoveJob(Guid oldJobID, object sender, CommandRSEventArgs args)
        {
            JobHolder oldJob;
            Guid? nextJobID = null;

            lock (this)
            {
                if (!WorkTable.Contains(oldJobID))
                {
                    return false;
                }

                oldJob = WorkTable[oldJobID] as JobHolder;
            }

            //Execute the call back - this must be done before the job is removed to ensure 
            //that the completion job does not fire too early.
            try
            {
                if (oldJob.RSCallback != null)
                    oldJob.RSCallback(sender, args);
            }
            catch (CompletionJobDependencyException)
            {
                //This exception will be fired by the parent if the job was not successful.
                //We should mark the dependency ID as erroneous and any dependent jobs should be cancelled.
            }
            catch (Exception ex)
            {
                string error = "Completion job error:" + ex.Message;
                if (JobShouldTrace)
                {
                    JobTrace("!!" + jobRequests.ToString() + " Job="
                        + oldJobID.ToString() + " --> " + error);
                }

                XimuraAppTrace.WriteLine(error, "Completion Job", EventLogEntryType.Warning);
            }

            lock (this)
            {
                //Is this a dependency job? If so truncate and pull out the next job.
                if (oldJob.DependencyID != null && this.mDependencyList.ContainsKey(oldJob.DependencyID))
                {
                    //Is there a next job?
                    if (oldJob.NextJob.HasValue)
                    {
                        nextJobID=oldJob.NextJob;
						JobHolder nextJob = WorkTable[nextJobID.Value] as JobHolder;
						nextJob.LastJob = oldJob.LastJob;
                        //Yes, so re-adjust the dependency list.
                        mDependencyList[oldJob.DependencyID] = nextJobID.Value;
                    }
                    else
                        //No, so remove the dependency reference.
                        mDependencyList.Remove(oldJob.DependencyID);
                }

                Interlocked.Decrement(ref jobRequests);
                WorkTable.Remove(oldJobID);
                //This following statement clears the job of any parameters and delegates
                //and allows Garbage collection to release the resources.
                if (oldJob!=null)
                    oldJob.Reset();

                if (JobShouldTrace)
                    JobTrace("<--" + jobRequests.ToString() + " CJ=" + CompletionJobID.ToString() 
                        + " Job=" + oldJobID.ToString());
            }

            //Stop any time-out timer.
            if (jobRequests == 0)
                ResetTimer();

            if (nextJobID.HasValue)
                SubmitJob(nextJobID.Value);

            return true;
        }
        #endregion // RemoveJob
        #region RemoveErrorJob
        /// <summary>
        /// This method will remove a job from the collection and flag it as an error.
        /// </summary>
        /// <param name="errorJob">THe job to remove.</param>
        protected virtual void RemoveErrorJob(JobHolder errorJob)
        {
            lock (this)
            {
                Guid? errorID = errorJob.ID;

                while (errorID.HasValue && WorkTable.Contains(errorID.Value))
                {
                    //Add the ID to the error list.
                    errorList.Add(errorID.Value);
                    Interlocked.Increment(ref errorCount);

                    try
                    {
                        //Get the error job, this is used to determine the dependency relationships.
                        errorJob = WorkTable[errorID.Value] as JobHolder;

                        //Remove the job from the collection
                        WorkTable.Remove(errorID.Value);
                        //Set the counters
                        Interlocked.Decrement(ref jobRequests);

                        //Does this job have dependencies? If not, then quit.
                        if (errorJob.DependencyID == null)
                            break;
                        //Is there a next job? If not then quit.
                        if (!errorJob.NextJob.HasValue)
                            break;

                        errorID = errorJob.NextJob.Value;
                    }
                    finally
                    {
                        if (errorJob != null)
                            errorJob.Reset();
                    }
                }
            }
        }
        #endregion // RemoveErrorJob

        #region PurgeJob
        /// <summary>
        /// This method will purge the job from the queue.
        /// </summary>
        /// <param name="job"></param>
        protected virtual void PurgeJob(JobHolder job)
        {
            //Do nothing for the moment.
        }
        #endregion // PurgeJob

        #region CheckCompletionJobComplete
        /// <summary>
		/// This method completes the job if it is not already completed.
		/// </summary>
		protected virtual bool CheckCompletionJobComplete()
		{
			lock(this)
			{
                if (jobRequests > 0)
                    return false;

                if (mThrottlingQueue != null && mThrottlingQueue.Count > 0)
                    return false;

				//If there are more jobs to process then exit.
				if (mStatus == CompletionJobStatus.Submitting 
                    || mStatus == CompletionJobStatus.Complete)
					return false;

				//This completion job has completed.
				mStatus = CompletionJobStatus.Complete;
			}

            try
            {
                if (CompletionJobComplete != null)
                    CompletionJobComplete(this.ParentJob, CompletionJobStatus.Complete, State);

                delCompletionJobReturn(this);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                ThrottleSet();
            }

            return true;
		}
		#endregion // CheckJobComplete

        #region Job Tracing
        #region JobTrace
        private void JobTrace(string id)
        {
            lock (this)
            {
                if (mTraceBuilder == null)
                    mTraceBuilder = new StringBuilder();
                mTraceBuilder.AppendLine(id);
#if (DEBUG)
                System.Diagnostics.Debug.WriteLine(id);
#endif
            }
        }
        #endregion // JobTrace
        #region JobBreakdown
        /// <summary>
        /// This method returns a text breakdown of the job process.
        /// </summary>
        public string JobBreakdown
        {
            get
            {
                lock (this)
                {
                    if (mTraceBuilder == null)
                        return "";
                    return mTraceBuilder.ToString();
                }
            }
        }
        #endregion // JobBreakdown
        #region JobShouldTrace
        private bool JobShouldTrace
        {
            get
            {
                return mJobShouldTrace;
            }
        }
        #endregion // JobShouldTrace
        #endregion // Job Tracing
	}
}
