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

using Ximura;
using Ximura.Framework;

using CH=Ximura.Common;

#endregion // using
namespace Ximura.Framework
{
    public delegate SecurityManagerJob SecurityManagerJobGet(
        JobBase job, SessionToken token, SecurityManagerJob parent, CommandRSCallback RSCallback, CommandProgressCallback ProgressCallback);

    public delegate CompletionJob CompletionJobGet(
        JobBase job, SessionToken token, SecurityManagerJob parent, CommandRSCallback RSCallback, CommandProgressCallback ProgressCallback);

	/// <summary>
	/// This is the Security Manager job object.
	/// </summary>
	public class SecurityManagerJob : CompletionJobWrapperBase, IXimuraSessionRQ
	{
        #region Static JobPool Implementation
//        #region Static declarations
//        private static PoolInvocator<SecurityManagerJob> sJobPool;
//        private static bool sPoolDisposed = false;
//        private static bool sPoolInitiated = false;
//        private static int mSCMJobs = 0;
//        #endregion // Static declarations
//        #region Initialize and Dispose
//        internal static void InitializePool()
//        {
//            //sJobPool = new XimuraObjectPool(
//            //    XimuraObjectPoolType.StandardPool,typeof(SecurityManagerJob),
//            //        "SecurityManagerJob Pool","",false);
//            sJobPool = new PoolInvocator<SecurityManagerJob>(internalCreateJobRequest, internalGetPool);
//            sPoolInitiated = true;
//            sPoolDisposed = false;
//        }

//        internal static void DisposePool()
//        {
//            sPoolInitiated = false;
//            sPoolDisposed = true;
//            sJobPool.Dispose();
//            sJobPool = null;
//        }

//        #region Static -> internalCreateCDSRequest()
//        /// <summary>
//        /// This internal static method is used by the pool manager to create a new object.
//        /// </summary>
//        /// <returns></returns>
//        private static SecurityManagerJob internalCreateJobRequest()
//        {
//            return new SecurityManagerJob();
//        }

//        private static IXimuraPool internalGetPool()
//        {
//            return sJobPool;
//        }
//        #endregion // Static -> internalCreateCDSRequest()

//        #endregion // Initialize and Dispose

//        #region GetSecurityManagerJob
//        /// <summary>
//        /// This method return a SecurityManagerJob object from the oject pool.
//        /// </summary>
//        /// <param name="secMan">The security manager.</param>
//        /// <param name="jobRQ">The base job request.</param>
//        /// <param name="token">The session token of the original caller.</param>
//        /// <returns>A new SecurityManagerJob object.</returns>
//        internal static SecurityManagerJob GetSecurityManagerJob( 
//            JobBase jobRQ, SecurityManager.SessionToken token)
//        {
//            return GetSecurityManagerJob(jobRQ, token, null, null, null);
//        }
//        /// <summary>
//        /// This method return a SecurityManagerJob object from the oject pool.
//        /// </summary>
//        /// <param name="secMan">The security manager.</param>
//        /// <param name="jobRQ">The base job request.</param>
//        /// <param name="token">The session token of the original caller.</param>
//        /// <param name="parentJob">The parent SecurityManagerJob object.</param>
//        /// <returns>A new SecurityManagerJob object.</returns>
//        internal static SecurityManagerJob GetSecurityManagerJob( 
//            JobBase jobRQ, SecurityManager.SessionToken token, SecurityManagerJob parentJob, 
//            CommandRSCallback RSCallback, CommandProgressCallback ProgressCallback)
//        {
//#if (USEJOBPOOL)
//            SecurityManagerJob newJob = sJobPool.ObjectGet() as SecurityManagerJob;
//#else
//            SecurityManagerJob newJob = new SecurityManagerJob();
//#endif 
//            Interlocked.Increment(ref mSCMJobs);

//            try
//            {
//                newJob.Initialize(jobRQ, token, parentJob, RSCallback, ProgressCallback);

//                return newJob;
//            }
//            catch (Exception ex)
//            {
//                if (newJob!=null)
//                    SecurityManagerJobReturn(newJob);
//                else
//                    Interlocked.Decrement(ref mSCMJobs);

//                throw ex;
//            }
//        }
//        #endregion // GetSecurityManagerJob
//        #region SecurityManagerJobReturn
//        /// <summary>
//        /// This method returns a used SecurityManagerJob object to the pool for further reuse.
//        /// </summary>
//        /// <param name="completedJob">The job to return.</param>
//        internal static void SecurityManagerJobReturn(SecurityManagerJob completedJob)
//        {
//            if (completedJob == null)
//                return;

//            Interlocked.Decrement(ref mSCMJobs);

//            completedJob.Reset();
//#if (USEJOBPOOL)
//            sJobPool.ObjectReturn(completedJob);
//#endif
//        }
//        #endregion // SecurityManagerJobReturn
		#endregion

		#region Declarations
        private CommandRequestCallback mJobCallback;
		private IXimuraCommandRQ mCommand;
		private SCMJobCompletionType mJobCompletionType;
		private Thread jobThread;
		private DJobType mDJobType;
		private SessionToken token;
        //private SecurityManager mSecMan;
        //private Hashtable childJobs = null;
		private int mChildJobDepth;
		private int mTotalChildJobCount;
        //private bool mIsCallback;

        private JobGet<Job> delJobGet;
        private SecurityManagerJobGet delSecurityManagerJobGet;
        private CompletionJobGet delCompletionJobGet;

		#endregion // Declarations
		#region Constructors
		/// <summary>
		/// This is the job pool constructor.
		/// </summary>
		public SecurityManagerJob():base(null){}
		#endregion // Constructors

		#region Initialize(...)
		/// <summary>
		/// This method initializes the session.
		/// </summary>
		/// <param name="secMan">The security manager.</param>
		/// <param name="jobRQ">The job request.</param>
		/// <param name="token">The session token.</param>
		/// <param name="parentJob">The parent job. This should be left null if this is not a child job.</param>
		public void Initialize(JobBase jobRQ, 
            SessionToken token, SecurityManagerJob parentJob, 
			CommandRSCallback RSCallback, CommandProgressCallback ProgressCallback)
		{
			try
			{
                //mSecMan = secMan;
				mParentJob=parentJob;
				if (parentJob!=null)
					mChildJobDepth = parentJob.ChildJobDepth+1;
				mRSCallback=RSCallback;
				mProgressCallback=ProgressCallback;
				mBaseJob = jobRQ;
				this.token=token;
			}
			catch(Exception ex)
			{
				Reset();
				throw ex;
			}
		}
		#endregion // Initialize
		#region Reset()
		/// <summary>
		/// This method resets the object to its initial state.
		/// </summary>
		public override void Reset()
		{
			base.Reset();

            //mIsCallback = false;
			mCommand = null;
			mJobCallback = null;
			jobThread = null;
			mDJobType = DJobType.Command;
			token = null;
            //mSecMan = null;
			mChildJobDepth = 0;
			mTotalChildJobCount = 0;
			mJobCompletionType = SCMJobCompletionType.OnExit;

            delJobGet = null;
            delSecurityManagerJobGet = null;
            delCompletionJobGet = null;

            //if (childJobs != null)
            //    childJobs.Clear();
		}
		#endregion

		#region ParentID
		/// <summary>
		/// The parent Job ID or Guid empty if there isn't a parent job.
		/// </summary>
		public Guid? ParentID
		{
			get
			{
				return mParentJob.ID;
			}
		}
		#endregion // ParentID
		#region UserID
		/// <summary>
		/// This is the user ID of the user that made the request.
		/// </summary>
		public string UserID
		{
            get 
            { 
                return null;// token.theSession.UserID; 
            }
		}
		#endregion // UserID
		#region Command
		/// <summary>
		/// The command
		/// </summary>
		public IXimuraCommandRQ Command
		{
			get{return mCommand;}
			set{mCommand = value;}
		}
		#endregion // Command
		#region BaseJob
		/// <summary>
		/// This is the base job for the request.
		/// </summary>
		public JobBase BaseJob
		{
			get{return this.mBaseJob;}
		}
		#endregion // BaseJob
        #region ActiveThread
        /// <summary>
		/// This property is the active thread for the command.
		/// </summary>
		public Thread ActiveThread
		{
			get{return jobThread;}
			set{jobThread = value;}
		}
		#endregion // activeThread
		#region JobType
		/// <summary>
		/// This property is the Dispatcher Job type.
		/// </summary>
		public DJobType JobType
		{
			get{return mDJobType;}
		}
		#endregion // JobType

        #region JobCallback
        /// <summary>
        /// This property is the job callback to the command.
        /// </summary>
        public CommandRequestCallback JobCallback
        {
            get { return mJobCallback; }
        }
        #endregion // JobCallback

		#region IsChildJob
		/// <summary>
		/// This boolean property identifies whether the job is a child job.
		/// </summary>
		public bool IsChildJob
		{
			get{return mParentJob!= null;}
		}
		#endregion
		#region ChildJobDepth
		/// <summary>
		/// This integer property identifies the depth from the original parent. 
		/// </summary>
		public int ChildJobDepth
		{
			get{return mChildJobDepth;}
		}
		#endregion // ChildJobDepth
		#region HasChildren
		/// <summary>
		/// This boolean property identifies whether the job has child jobs.
		/// </summary>
		public bool HasChildren
		{
			get{return false;}
		}
		#endregion
		#region TotalChildJobCount
		/// <summary>
		/// This property is the total number of child job that have 
		/// been executed (but not necessarily completed). This is a recursive value.
		/// </summary>
		public int TotalChildJobCount
		{
			get{return mTotalChildJobCount;}
		}
		#endregion // TotalChildJobCount
		#region IncrementChildJobCount()
		/// <summary>
		/// This method increments the total job count.
		/// </summary>
		protected void IncrementChildJobCount()
		{
			Interlocked.Increment(ref mTotalChildJobCount);
			//This is a recursive call. The parent job is never a child job.
			if (IsChildJob)
				this.ParentJob.IncrementChildJobCount();
		}
		#endregion // IncrementChildJobCount()

		#region ProcessRequest
        public void CancelRequest(Guid jobID)
        {
            token.JobCancel(jobID);
        }
		/// <summary>
		/// Process a synchronous request.
		/// </summary>
		/// <param name="data">The data.</param>
		public void ProcessRequest(IXimuraRQRSEnvelope data)
		{
			ProcessRequest(data,JobPriority.Normal,null);
		}
		/// <summary>
		/// Process a synchronous request.
		/// </summary>
		/// <param name="data">The data.</param>
		/// <param name="priority">The request priority.</param>
		public void ProcessRequest(IXimuraRQRSEnvelope data, JobPriority priority)
		{
            ProcessRequest(data, priority, null);
        }
        /// <summary>
        /// Process a synchronous request.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="ProgressCallback">The progress calback delegate. 
        /// This can be used to report progress during long running processes.</param>
        public void ProcessRequest(IXimuraRQRSEnvelope data, CommandProgressCallback ProgressCallback)
        {
            ProcessRequest(data, JobPriority.Normal, ProgressCallback);
        }
        /// <summary>
        /// Process a synchronous request.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="priority">The request priority.</param>
        /// <param name="ProgressCallback">The progress calback delegate. 
        /// This can be used to report progress during long running processes.</param>
        public void ProcessRequest(IXimuraRQRSEnvelope data, JobPriority priority, 
            CommandProgressCallback ProgressCallback)
        {
            SecurityManagerJob scmJob = PrepareChildJob(Guid.NewGuid(), data, null, ProgressCallback, priority);
            IncrementChildJobCount();
            token.JobProcess(scmJob as JobBase, false);
        }
		#endregion // ProcessRequest
		#region ProcessRequestAsync
		/// <summary>
		/// Process an asychronous request.
		/// </summary>
		/// <param name="jobID">The job ID. This should be set to a new Guid.</param>
		/// <param name="data">The data</param>
		/// <param name="RSCallback">The call back completion delegate.</param>
		/// <param name="ProgessCallback">The request progress delegate. Set this to null if not needed.</param>
		/// <param name="priority">The request priority.</param>
		/// <returns>The job guid.</returns>
		public override Guid ProcessRequestAsync(Guid jobID, IXimuraRQRSEnvelope data, 
			CommandRSCallback RSCallback, CommandProgressCallback ProgessCallback, JobPriority priority)
		{
			SecurityManagerJob scmJob = 
				PrepareChildJob(jobID, data, RSCallback, ProgessCallback, priority);

			token.JobProcess(scmJob as JobBase, true);
			return jobID;
		}
		#endregion // ProcessRequestAsync

		#region PrepareChildJob
		private SecurityManagerJob PrepareChildJob(Guid jobID, IXimuraRQRSEnvelope data, 
			CommandRSCallback RSCallback, CommandProgressCallback ProgessCallback, JobPriority priority)
		{
            Job childJob = delJobGet(SessionID.Value, jobID, data, null, null, JobSignature.Empty, priority);

            SecurityManagerJob newJob = delSecurityManagerJobGet(
                childJob as JobBase, this.token, this, RSCallback, ProgessCallback);

			AddJob(jobID, data, RSCallback, ProgessCallback, priority);

			return newJob;
		}
		#endregion // PrepareChildJob
		#region AddJob
		/// <summary>
		/// This protected method is used to add a job to the collection.
		/// </summary>
		/// <param name="jobID">The job ID. This should be set to a new Guid.</param>
		/// <param name="data">The data</param>
		/// <param name="RSCallback">The call back completion delegate.</param>
		/// <param name="ProgessCallback">The request progress delegate. Set this to null if not needed.</param>
		/// <param name="priority">The request priority.</param>
		/// <returns></returns>
		protected override Guid AddJob(Guid jobID, IXimuraRQRSEnvelope data, 
			CommandRSCallback RSCallback, CommandProgressCallback ProgessCallback, JobPriority priority)
		{
			Guid id = base.AddJob(jobID, data, RSCallback, ProgessCallback, priority);
			IncrementChildJobCount();
			return id;
		}
		#endregion

		#region CreateCompletionJob
		/// <summary>
		/// This method is used to create a completion job.
		/// </summary>
		/// <param name="callback">The callback to use when the job is complete.</param>
		/// <returns>A completion job.</returns>
		public CompletionJob CreateCompletionJob(CompletionJobCallBack callback)
		{
            return CreateCompletionJob(callback, this.Data, false, 0);
		}
		/// <summary>
		/// This method is used to create a completion job.
		/// </summary>
		/// <param name="callback">The callback to use when the job is complete.</param>
		/// <param name="AutoExecute">The boolean property indicates whether the job request should execute immediately.</param>
        /// <param name="throttleThreshold">This parameter specifies the maximum number of jobs that will be processed in parallel. If this is
        /// set to 0 or below throttling will not be implemented.</param>
		/// <returns>A completion job.</returns>
		public CompletionJob CreateCompletionJob(CompletionJobCallBack callback, bool AutoExecute, int throttleThreshold)
		{
            return CreateCompletionJob(callback, this.Data, AutoExecute, throttleThreshold);
		}
		/// <summary>
		/// This method is used to create a completion job.
		/// </summary>
		/// <param name="callback">The callback to use when the job is complete.</param>
		/// <param name="state">The callback state.</param>
		/// <returns>A completion job.</returns>
		public CompletionJob CreateCompletionJob(CompletionJobCallBack callback, object state)
		{
            return CreateCompletionJob(callback, state, false, 0);
		}
		/// <summary>
		/// This method is used to create a completion job.
		/// </summary>
		/// <param name="callback">The callback to use when the job is complete.</param>
		/// <param name="state">The callback state.</param>
		/// <param name="AutoExecute">The boolean property indicates whether the job request should execute immediately.</param>
        /// <param name="throttlingThreshold">This parameter specifies the maximum number of jobs that will be processed in parallel. If this is
        /// set to 0 or below throttling will not be implemented.</param>
		/// <returns>A completion job.</returns>
		public CompletionJob CreateCompletionJob(CompletionJobCallBack callback, object state, bool AutoExecute, int throttlingThreshold)
		{
            return null;// delCompletionJobGet(this, callback, state, AutoExecute, throttlingThreshold, false);
		}
		#endregion // CreateCompletionJob

		#region SignalComplete()
		/// <summary>
		/// This method is used to signal that the job is complete.
		/// </summary>
		public void SignalComplete()
		{
			token.JobComplete(this,true);
		}
		#endregion // SignalComplete()

		#region ChildJobComplete
		/// <summary>
		/// This method is used to signal that a child job is complete.
		/// </summary>
		/// <param name="childJob"></param>
		public void ChildJobComplete(SecurityManagerJob childJob)
		{
			if(!RemoveJob(childJob.ID.Value,null, new CommandRSEventArgs(childJob)))
				return;

			//SecurityManagerJob.SecurityManagerJobReturn(childJob);
		}
		#endregion // ChildJobComplete

		#region JobCompletionType
		/// <summary>
		/// This is the completion job type.
		/// </summary>
		public SCMJobCompletionType JobCompletionType
		{
			get{return mJobCompletionType;}
		}
		#endregion // JobCompletionType
		#region SetCompletionType
		/// <summary>
		/// This method sets the job completion time out.
		/// </summary>
		/// <param name="completionType">The completion type.</param>
		public void SetCompletionType(SCMJobCompletionType completionType)
		{
			SetCompletionType(completionType,-1);
		}
		/// <summary>
		/// This method sets the job completion time out.
		/// </summary>
		/// <param name="completionType">The completion type.</param>
		/// <param name="Timeout">The time out.</param>
		public void SetCompletionType(SCMJobCompletionType completionType, int Timeout)
		{
			mJobCompletionType=completionType;

//			if (completionType==SCMJobCompletionType.SignalOrTimeout)
//			{
//
//			}
		}
		#endregion // SetCompletionType

    }
}