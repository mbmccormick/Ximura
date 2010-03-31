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
using Ximura.Framework;

using CH=Ximura.Common;

#endregion // using
namespace Ximura.Framework
{
	/// <summary>
	/// The job holder class is used to reference the requests in the completion job.
	/// </summary>
	public class JobHolder: JobBase
	{
		#region Declarations
		/// <summary>
		/// The job priority.
		/// </summary>
		private JobPriority mPriority;
		/// <summary>
		/// The request data.
		/// </summary>
		private IXimuraRQRSEnvelope mData;
		/// <summary>
		/// The request completion callback.
		/// </summary>
		private CommandRSCallback mRSCallback;
		/// <summary>
		/// The request progress callback.
		/// </summary>
		private CommandProgressCallback mProgressCallback;
		/// <summary>
		/// This is the child job ID.
		/// </summary>
		private Guid? mjobID = Guid.Empty;
		/// <summary>
		/// This boolean method indicates whether the request has been executed.
		/// </summary>
		public bool Executed = false;
        /// <summary>
        /// This is the next job in the chain.
        /// </summary>
        private Guid? mNextJob;
        /// <summary>
        /// This is the last job in the chain.
        /// </summary>
        private Guid? mLastJob;
        /// <summary>
        /// This is the job holder dependency ID.
        /// </summary>
        private string mDependencyID = null;

		#endregion // Declarations
		#region Constructor
		/// <summary>
		/// This is the internal constructor for the job.
		/// </summary>
		/// <param name="jobID">The job ID.</param>
		/// <param name="data">The data.</param>
		/// <param name="RSCallback">The request progress callback.</param>
		/// <param name="ProgressCallback">The request progress callback.</param>
		/// <param name="priority">The job priority.</param>
		public JobHolder(Guid? jobID, IXimuraRQRSEnvelope data, CommandRSCallback RSCallback,
			CommandProgressCallback ProgressCallback, JobPriority priority):
            this(jobID,data,RSCallback,ProgressCallback,priority,null,null, null){}
        /// <summary>
        /// This is the internal constructor for the job.
        /// </summary>
        /// <param name="jobID">The job ID.</param>
        /// <param name="data">The data.</param>
        /// <param name="RSCallback">The request progress callback.</param>
        /// <param name="ProgressCallback">The request progress callback.</param>
        /// <param name="priority">The job priority.</param>
        /// <param name="NextJob">The next job for linked jobs.</param>
        /// <param name="LastJob">The last job for linked jobs.</param>
        public JobHolder(Guid? jobID, IXimuraRQRSEnvelope data, CommandRSCallback RSCallback,
            CommandProgressCallback ProgressCallback, JobPriority priority,
            Guid? NextJob, Guid? LastJob, DependencyValidateRSCallback ValidateRSCallBack)
        {
            mjobID = jobID;
            mPriority = priority;
            mData = data;
            mRSCallback = RSCallback;
            mProgressCallback = ProgressCallback;
            mNextJob = NextJob;
            mLastJob = LastJob;
        }
        #endregion // Constructor

        #region Reset()
        /// <summary>
        /// This method should be overriden to provide specific clean up code.
        /// Specifically, any delegates references in the object should be set to null;
        /// </summary>
        /// <param name="disposing">This parameter is true if the call is from the disposable interface.</param>
        public override void Reset()
        {
		    mData = null;
            mRSCallback = null;
		    mProgressCallback = null;
        }
        #endregion // Dispose

		#region SessionID
		/// <summary>
		/// The session ID.
		/// </summary>
		public override Guid? SessionID
		{
			get
			{
				throw new NotSupportedException();
			}
		}
		#endregion // SessionID
		#region ID
		/// <summary>
		/// The job ID.
		/// </summary>
		public override Guid? ID
		{
			get
			{
				return mjobID;
			}
		}
		#endregion // ID

		#region RSCallback
		/// <summary>
		/// The Response callback.
		/// </summary>
		public virtual CommandRSCallback RSCallback
		{
			get
			{
				return mRSCallback;
			}
		}
		#endregion // RSCallback
		#region ProgressCallback
		/// <summary>
		/// The progress callback.
		/// </summary>
		public virtual CommandProgressCallback ProgressCallback
		{
			get
			{
				return mProgressCallback;
			}
		}
		#endregion // ProgressCallback
		#region Data
		/// <summary>
		/// The data.
		/// </summary>
		public override IXimuraRQRSEnvelope Data
		{
			get
			{
				return this.mData;
			}
		}
		#endregion // Data
		#region Priority
		/// <summary>
		/// The job priority.
		/// </summary>
		public override JobPriority Priority
		{
			get
			{
				return this.mPriority;
			}
			set
			{
				mPriority = value;
			}
		}
		#endregion // Priority
		
		#region Signature
		/// <summary>
		/// Not supported.
		/// </summary>
		public override JobSignature? Signature
		{
			get
			{
				throw new NotSupportedException();
			}
		}
		#endregion // Signature

        #region Expired
        /// <summary>
        /// This property indicates whether the job has exceeded its timeslice.
        /// </summary>
        public bool Expired
        {
            get { return false; }
        }
        #endregion // Expired
		
		#region IDBuffer()
		/// <summary>
		/// Not supported.
		/// </summary>
		/// <returns></returns>
		public override byte[] IDBuffer()
		{
			throw new NotSupportedException();
		}
		#endregion // IDBuffer()

        #region NextJob
        /// <summary>
        /// This is the next job in the chain.
        /// </summary>
        public Guid? NextJob
        {
            get { return mNextJob; }
            set { mNextJob = value; }
        }
        #endregion // NextJob
        #region LastJob
        /// <summary>
        /// This is the last job in the chain.
        /// </summary>
        public Guid? LastJob
        {
            get { return mLastJob; }
            set { mLastJob = value; }
        }
        #endregion // LastJob

        #region DependencyID
        /// <summary>
        /// This is the job holder dependency ID.
        /// </summary>
        public string DependencyID
        {
            get { return mDependencyID; }
            set { mDependencyID = value; }
        }
        #endregion // DependencyID

        #region EnvelopeHelper
        /// <summary>
        /// This property is not supported.
        /// </summary>
        /// <exception cref="System.NotSupportedException">This exception is thrown.</exception>
        public override IXimuraEnvelopeHelper EnvelopeHelper
        {
            get { throw new NotSupportedException("EnvelopeHelper is not supported in the JobHolder class."); }
        }
        #endregion // EnvelopeHelper
	}
}