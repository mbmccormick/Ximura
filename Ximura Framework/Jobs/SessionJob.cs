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
using Ximura.Helper;
using CH=Ximura.Helper.Common;

#endregion // using
namespace Ximura.Framework
{
	/// <summary>
	/// SessionJob is used to hold the job requests while they are being processed.
	/// </summary>
	public class SessionJob : JobWrapper
	{
		#region Static JobPool
//        private static PoolInvocator<SessionJob> sJobPool;
//        private static bool sPoolDisposed = false;
//        private static bool sPoolInitiated = false;
//        private static int mActiveJobs = 0;

//        #region ActiveJobs
//        /// <summary>
//        /// This property returns a count of the currently active jobs on the system.
//        /// </summary>
//        public static int ActiveJobs
//        {
//            get
//            {
//                return mActiveJobs;
//            }
//        }
//        #endregion // ActiveJobs


//        internal static void InitializePool()
//        {
//            //sJobPool = new XimuraObjectPool(
//            //    XimuraObjectPoolType.StandardPool,typeof(SessionJob),"SessionJob Pool","",false);
//            sJobPool = new PoolInvocator<SessionJob>(internalCreateJobRequest, internalGetPool);
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
//        private static SessionJob internalCreateJobRequest()
//        {
//            return new SessionJob();
//        }

//        private static IXimuraPool internalGetPool()
//        {
//            return sJobPool;
//        }
//        #endregion // Static -> internalCreateCDSRequest()

//        /// <summary>
//        /// Session Job is used to submit jobs to the dispatcher from processing.
//        /// </summary>
//        /// <param name="sessionid">The session id.</param>
//        /// <param name="id">The job id.</param>
//        /// <param name="data">The envelope data to process.</param>
//        /// <param name="RSCallback">The response call back delegate. This will
//        /// be used to respond after the job is complete.</param>
//        /// <param name="ProgressCallback">The progress call back delegate. 
//        /// This should be null if callbacks are not required.</param>
//        /// <param name="signature">The job signature.</param>
//        internal static SessionJob GetSessionJob(Guid sessionid, Guid id, IXimuraRQRSEnvelope data, 
//            CommandRSCallback RSCallback, CommandProgressCallback ProgressCallback, 
//            JobSignature signature)
//        {
//            return GetSessionJob(sessionid,id,data,RSCallback,
//                ProgressCallback,signature,JobPriority.Normal);
//        }
//        /// <summary>
//        /// Session Job is used to submit jobs to the dispatcher from processing.
//        /// </summary>
//        /// <param name="sessionid">The session id.</param>
//        /// <param name="id">The job id.</param>
//        /// <param name="data">The envelope data to process.</param>
//        /// <param name="RSCallback">The response call back delegate. This will
//        /// be used to respond after the job is complete.</param>
//        /// <param name="ProgressCallback">The progress call back delegate. 
//        /// This should be null if callbacks are not required.</param>
//        /// <param name="signature">The job signature.</param>
//        /// <param name="priority">The job priority.</param>
//        internal static SessionJob GetSessionJob(Guid sessionid, Guid id, IXimuraRQRSEnvelope data, 
//            CommandRSCallback RSCallback, CommandProgressCallback ProgressCallback, 
//            JobSignature signature, JobPriority priority)
//        {
//#if (USEJOBPOOL)
//            SessionJob newJob = sJobPool.Get() as SessionJob;
//#else
//            SessionJob newJob = new SessionJob();
//#endif
//            Interlocked.Increment(ref mActiveJobs);
//            try
//            {
//                newJob.Initialize(sessionid,id,data,RSCallback,ProgressCallback,signature,priority);

//                return newJob;
//            }
//            catch (Exception ex)
//            {
//                if (newJob != null)
//                    SessionJobReturn(newJob);
//                else
//                    Interlocked.Decrement(ref mActiveJobs);

//                throw ex;
//            }

//        }
//        /// <summary>
//        /// This method returns a job to the pool.
//        /// </summary>
//        /// <param name="completedJob"></param>
//        internal static void SessionJobReturn(SessionJob completedJob)
//        {
//            if (completedJob == null)
//                return;

//            Interlocked.Decrement(ref mActiveJobs);

//            completedJob.Reset();
//#if (USEJOBPOOL)
//            sJobPool.ObjectReturn(completedJob);
//#endif
//        }
		#endregion

		#region Declarations
		private ManualResetEvent signalComplete;
        private object syncJobReturn = new object();

        private Action<Job> delJobReturn = null;
        private JobGet<Job> delJobGet = null;
        #endregion
		#region Constructors
		/// <summary>
		/// This is the internal constructor used by the object pool.
		/// </summary>
		internal SessionJob(): base(null)
		{
			Reset();
		}
		#endregion
		#region Reset()
		/// <summary>
		/// This method is used to reset the session job to its initial state.
		/// </summary>
		public override void Reset()
		{
			Job oldJob = this.mBaseJob as Job;
			if (oldJob != null)
			{
                lock (syncJobReturn)
				{
					mBaseJob = null;
                    delJobReturn(oldJob);

				}
			}
			signalComplete = null;
			mRSCallback = null;
			mProgressCallback = null;
            ObjectPool = null;

            delJobGet = null;
            delJobReturn = null;
		}
		#endregion // Reset()
		#region Initialize
		/// <summary>
		/// This method initializes the SessionJob.
		/// </summary>
		/// <param name="sessionid">The session id.</param>
		/// <param name="id">The job id.</param>
		/// <param name="data">The envelope data to process.</param>
		/// <param name="RSCallback">The response call back delegate. This will
		/// be used to respond after the job is complete.</param>
		/// <param name="ProgressCallback">The progress call back delegate. 
		/// This should be null if callbacks are not required.</param>
		/// <param name="signature">The job signature.</param>
		/// <param name="priority">The job priority.</param>
		private void Initialize(
            Guid sessionid, Guid id, IXimuraRQRSEnvelope data, 
			CommandRSCallback RSCallback, CommandProgressCallback ProgressCallback, 
			JobSignature signature, JobPriority priority)
		{
			Job newJob = null;
			try
			{
                newJob = delJobGet(sessionid, id, data, null,null, signature, priority);

                mBaseJob=newJob as JobBase;

				mRSCallback = RSCallback;
				mProgressCallback = ProgressCallback;

				if (SynchRQ)
					signalComplete = new ManualResetEvent(false);
			}
			catch (Exception ex)
			{
				Reset();
				if (newJob!=null)
					delJobReturn(newJob);
				throw ex;
			}
		}
		#endregion // Initialize

		#region SynchRQ
		/// <summary>
		/// This property returns true if this is a synchronous request.
		/// </summary>
		public bool SynchRQ
		{
			get
			{
				return (mRSCallback == null && mProgressCallback==null);
			}
		}
		#endregion // SynchRQ
		#region RequiresProgressUpdates
		/// <summary>
		/// This property returns true is the job requires Progress Updates
		/// </summary>
		public bool RequiresProgressUpdates
		{
			get{return mProgressCallback!= null;}
		}
		#endregion // RequiresProgressUpdates

		#region WaitForCompletion()
		/// <summary>
		/// This method will hold the current thread until the job is complete
		/// or this request times out.
		/// </summary>
		/// <returns>True if the job has completed.</returns>
		public bool WaitForCompletion()
		{
			return WaitForCompletion(-1);
		}
		/// <summary>
		/// This method will hold the current thread until the job is complete
		/// or this request times out.
		/// </summary>
		/// <param name="timeout">The time out.</param>
		/// <returns>True if the job has completed.</returns>
		public bool WaitForCompletion(int timeout)
		{
			if (signalComplete == null) return false;

			return signalComplete.WaitOne(timeout, false);
		}
		#endregion // WaitForCompletion()
		#region SignalCompletion()
		/// <summary>
		/// This method will signal completion and allow the blocked thread to 
		/// continue.
		/// </summary>
		public void SignalCompletion()
		{
			if (SynchRQ)
			{
				signalComplete.Set();
			}
			else
			{
				RSCallback.BeginInvoke(
					null, new CommandRSEventArgs(base.mBaseJob), null, null);
			}
		}
		#endregion // SignalCompletion()
    }
}