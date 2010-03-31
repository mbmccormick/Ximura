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

using CH=Ximura.Common;

#endregion // using
namespace Ximura.Framework
{
	/// <summary>
	/// SessionJob is used to hold the job requests while they are being processed.
	/// </summary>
	public class SessionJob : JobWrapper
	{
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
		public void Initialize(
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