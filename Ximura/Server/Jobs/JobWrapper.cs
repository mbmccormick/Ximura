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
namespace Ximura.Server
{
	/// <summary>
	/// The Job Wrapper class is used by inherited method to add
	/// additional properties to the request while maintaining the 
	/// original job request.
	/// </summary>
    public class JobWrapper : JobBase, IXimuraPoolReturnable
	{
		#region Declarations
		/// <summary>
		/// This is the base job.
		/// </summary>
		protected internal JobBase mBaseJob;

		protected CommandRSCallback mRSCallback;
		protected CommandProgressCallback mProgressCallback;

		/// <summary>
		/// This is the thread timer used for time out operations
		/// </summary>
		protected Timer timeoutTimer = null;
		/// <summary>
		/// The time out callback
		/// </summary>
		private TimerCallback timeoutCallback;
		#endregion // Declarations
		#region Constructor
		/// <summary>
		/// This is the wrapper constructor.
		/// </summary>
		/// <param name="baseJob">The base job.</param>
		internal JobWrapper(JobBase baseJob)
		{
			mBaseJob=baseJob;
			InitializeJob();
			Reset();
		}
		#endregion // Constructor

		#region InitializeJob()
		/// <summary>
		/// This method is used to initialize any variables before the initial
		/// Reset() is called.
		/// </summary>
		protected virtual void InitializeJob()
		{
			timeoutCallback = new TimerCallback(TimeoutFire);
		}
		#endregion // InitializeJob()
		#region Reset()
		/// <summary>
		/// This virtual method resets the base properties for the JobWrapper.
		/// </summary>
		public override void Reset()
		{
			mBaseJob = null;
			mRSCallback = null;
			mProgressCallback = null;
			timeoutCallback = null;
            ObjectPool = null;
		}
		#endregion // Reset()
		#region Inititialize
		/// <summary>
		/// This is the root initialization for the JobWrapper.
		/// </summary>
		/// <param name="baseJob"></param>
		/// <param name="RSCallback"></param>
		/// <param name="ProgressCallback"></param>
		protected virtual void Inititialize(JobBase baseJob, 
			CommandRSCallback RSCallback, CommandProgressCallback ProgressCallback)
		{
			mBaseJob = baseJob;
			mRSCallback = RSCallback;
			mProgressCallback = null;
		}
		#endregion // Inititialize

		#region Priority
		/// <summary>
		/// This is the job priority.
		/// </summary>
		public override JobPriority Priority
		{
			get{return mBaseJob.Priority;}
			set{mBaseJob.Priority = value;}
		}
		#endregion // Priority
		#region SessionID
		/// <summary>
		/// The Session ID
		/// </summary>
		public override Guid? SessionID
		{
			get
			{
				return mBaseJob.SessionID;
			}
		}
		#endregion // ID
		#region ID
		/// <summary>
		/// The Job ID
		/// </summary>
		public override Guid? ID
		{
			get
			{
				if (mBaseJob == null)
					return Guid.Empty;
				return mBaseJob.ID;
			}
		}
		#endregion // ID
		#region Data
		/// <summary>
		/// The job data
		/// </summary>
        public override IXimuraRQRSEnvelope Data
		{
			get
			{
				return mBaseJob.Data;
			}
		}
		#endregion // Data
		#region Signature
		/// <summary>
		/// The originator signature.
		/// </summary>
		public override JobSignature? Signature
		{
			get{return mBaseJob.Signature;}
		}
		#endregion // Signature
		#region IDBuffer()
		/// <summary>
		/// This method returns the buffer for the request.
		/// </summary>
		/// <returns>A byte array containing the identifying IDs.</returns>
		public override byte[] IDBuffer()
		{
			return mBaseJob.IDBuffer();
		}
		#endregion // CreateBuffer()

		#region Timer functionality
		/// <summary>
		/// This method initializes the timer callback
		/// </summary>
		protected virtual void OnTimeOut(object state)
		{
			ResetTimer();
		}
		/// <summary>
		/// This method sets the timer to the delay requested.
		/// </summary>
		/// <param name="Timeout"></param>
		protected void SetTimer(int Timeout)
		{
			SetTimer(Timeout,null);
		}
		/// <summary>
		/// This method sets the timer to the delay requested.
		/// </summary>
		/// <param name="Timeout">The time out in milliseconds.</param>
		/// <param name="state">The object state.</param>
		protected void SetTimer(int Timeout, object state)
		{
			if (Timeout==-1)
				return;

			if (timeoutTimer == null)
				timeoutTimer = new Timer(timeoutCallback,state,0,Timeout);
			else
				timeoutTimer.Change(0, Timeout);
		}
		/// <summary>
		/// This method resets the timer and stops it firing.
		/// </summary>
		protected void ResetTimer()
		{
			if (timeoutTimer == null)
				return;
			timeoutTimer.Change(Timeout.Infinite,0);
		}
		/// <summary>
		/// This method is called when the timer fires. 
		/// Override this object to implement your own functionality.
		/// </summary>
		/// <param name="state">The object state.</param>
		private void TimeoutFire(object state)
		{
			OnTimeOut(state);
		}
		#endregion // Timer functionality

		#region RSCallback
		/// <summary>
		/// This is the completion callback delegate.
		/// </summary>
		public virtual CommandRSCallback RSCallback
		{
			get
			{
				return mRSCallback;
			}
			set
			{
				mRSCallback = value;
			}
		}
		#endregion // RSCallback

        #region ProgressReport
        /// <summary>
        /// This method reports the progress to the calling party.
        /// </summary>
        /// <param name="progress">The progress percentage as an integer.</param>
        /// <param name="message">An optional progress message.</param>
        public virtual bool ProgressReport(int progress)
        {
            return ProgressReport(progress, null);
        }
        /// <summary>
        /// This method reports the progress to the calling party.
        /// </summary>
        /// <param name="progress">The progress percentage as an integer.</param>
        /// <param name="message">An optional progress message.</param>
        /// <returns>Returns true if the progress message was successfully delivered.</returns>
        public virtual bool ProgressReport(int progress, string message)
        {
            if (!SupportsProgressNotification)
                return false;

            try
            {
                ProgressCallback(this.ID, new CommandProgressEventArgs(progress,message));
                return true;
            }
            catch
            {
                return false;
            }

        }
        #endregion // ProgressReport
		#region ProgressCallback
		/// <summary>
		/// This is the progress call back delegate
		/// </summary>
		public virtual CommandProgressCallback ProgressCallback
		{
			get
			{
				return mProgressCallback;
			}
			set
			{
				mProgressCallback = value;
			}
		}
		#endregion // ProgressCallback
		#region SupportsProgressNotification
		/// <summary>
		/// This property identifies whether the request supports 
		/// progress notification.
		/// </summary>
		public virtual bool SupportsProgressNotification
		{
			get{return mProgressCallback!=null;}
		}
		#endregion // SupportsProgressNotification

        #region IXimuraPoolReturnable Members
        /// <summary>
        /// This is the object pool for the job.
        /// </summary>
        public IXimuraPool ObjectPool
        {
            get;
            set;
        }
        /// <summary>
        /// This boolean property identifies whether the job can be returned to the pool.
        /// </summary>
        public bool ObjectPoolCanReturn
        {
            get { return ObjectPool != null; }
        }
        /// <summary>
        /// This method returns the job to the pool.
        /// </summary>
        public void ObjectPoolReturn()
        {
            ObjectPool.Return(this);
        }

        #endregion
	}
}