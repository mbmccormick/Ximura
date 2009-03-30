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
using Ximura.Command;
using CH=Ximura.Helper.Common;
#endregion // using
namespace Ximura.Server
{
	/// <summary>
	/// This is the base request job object.
	/// </summary>
	public class Job : JobBase
	{
		#region Static JobPool
//        #region Static Declarations
//        private static PoolInvocator<Job> sJobPool;
//        private static bool sPoolDisposed = false;
//        private static bool sPoolInitiated = false;
//        #endregion // Static Declarations

//        #region InitializePool()
//        internal static void InitializePool()
//        {
//            //sJobPool = new XimuraObjectPool(
//            //    XimuraObjectPoolType.StandardPool, typeof(Job), "Job Pool", "", false);
//            sJobPool = new PoolInvocator<Job>(internalCreateJobRequest, internalGetPool);
//            sPoolInitiated = true;
//            sPoolDisposed = false;
//        }
//        #endregion // InitializePool()

//        #region Static -> internalCreateJobRequest()
//        /// <summary>
//        /// This internal static method is used by the pool manager to create a new object.
//        /// </summary>
//        /// <returns></returns>
//        private static Job internalCreateJobRequest()
//        {
//            return new Job();
//        }

//        private static IXimuraPool internalGetPool()
//        {
//            return sJobPool;
//        }
//        #endregion // Static -> internalCreateCDSRequest()

//        #region DisposePool()
//        internal static void DisposePool()
//        {
//            sPoolInitiated = false;
//            sPoolDisposed = true;
//            sJobPool.Dispose();
//            sJobPool = null;
//        }
//        #endregion // DisposePool()

//        #region GetJob()
//        /// <summary>
//        /// The job constructor.
//        /// </summary>
//        /// <param name="sessionid">The session id</param>
//        /// <param name="id">The job id</param>
//        /// <param name="data">The data</param>
//        /// <param name="signature">The signature</param>
//        internal static Job GetJob(Guid sessionid, Guid id, IXimuraRQRSEnvelope data, 
//            JobSignature signature)
//        {
//            return GetJob(sessionid,id,data,signature,JobPriority.Normal);
//        }
//        /// <summary>
//        /// The job constructor.
//        /// </summary>
//        /// <param name="sessionid">The session id</param>
//        /// <param name="id">The job id</param>
//        /// <param name="data">The data</param>
//        /// <param name="signature">The signature</param>
//        /// <param name="priority">The job priority.</param>
//        internal static Job GetJob(Guid sessionid, Guid id, IXimuraRQRSEnvelope data, 
//            JobSignature signature, JobPriority priority)
//        {
//#if (USEJOBPOOL)
//            Job newJob = sJobPool.ObjectGet() as Job;
//#else
//            Job newJob = new Job();
//#endif

//            try
//            {
//                newJob.Initialize(sessionid,id,data,signature,priority);
//                return newJob;
//            }
//            catch (Exception ex)
//            {
//                if (newJob!=null)
//                    JobReturn(newJob);
//                throw ex;
//            }
//        }
//        #endregion // GetJob()
//        #region JobReturn
//        /// <summary>
//        /// This method returns the job to the pool.
//        /// </summary>
//        /// <param name="completedJob">The job to return.</param>
//        internal static void JobReturn(Job completedJob)
//        {
//            completedJob.Reset();
//#if (USEJOBPOOL)
//            sJobPool.ObjectReturn(completedJob);
//#endif
//        }
//        #endregion // JobReturn
		#endregion

		#region Declarations
		private JobPriority mPriority;
		private Guid? mSessionID;
		private Guid? mJobID;
		private IXimuraRQRSEnvelope mData;
		private JobSignature? mSignature;
        private IXimuraEnvelopeHelper mEnvelopeHelper;
		#endregion // Declarations
		#region Constructors
		/// <summary>
		/// This is the private constructor used by the job pool.
		/// </summary>
		internal Job()
		{
			Reset();
		}
		#endregion // Constructors
		#region Reset()
		/// <summary>
		/// This method resets the job to its unitialized state.
		/// </summary>
		public override void Reset()
		{
			mSessionID = null;
			mJobID = null;
			mData = null;
			mSignature = null;
			mPriority= JobPriority.Normal;
            mEnvelopeHelper = null;
		}
		#endregion // Reset()
		#region Initialize()
		/// <summary>
		/// This method initializes the job.
		/// </summary>
		/// <param name="sessionid">The session id</param>
		/// <param name="id">The job id</param>
		/// <param name="data">The data</param>
		/// <param name="signature">The signature</param>
		/// <param name="priority">The job priority.</param>
		private void Initialize(Guid sessionid, Guid id, IXimuraRQRSEnvelope data,
            JobSignature signature, JobPriority priority, IXimuraEnvelopeHelper envelopeHelper)
		{
			try
			{
				mSessionID = sessionid;
				mJobID = id;
				mData = data;
				mSignature = signature;
				mPriority = priority;
                mEnvelopeHelper = envelopeHelper;
			}
			catch (Exception ex)
			{
				Reset();
				throw ex;
			}
		}
		#endregion // Initialize()

		#region Priority
		/// <summary>
		/// This is the job priority.
		/// </summary>
		public override JobPriority Priority
		{
			get{return mPriority;}
			set{mPriority = value;}
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
				return mSessionID;
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
				return mJobID;
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
				return mData;
			}
		}
		#endregion // Data
		#region Signature
		/// <summary>
		/// The originator signature.
		/// </summary>
		public override JobSignature? Signature
		{
			get{return mSignature;}
		}
		#endregion // Signature
		#region IDBuffer()
		/// <summary>
		/// This method returns the buffer for the request.
		/// </summary>
		/// <returns>A byte array containing the identifying IDs.</returns>
		public override byte[] IDBuffer()
		{
			return JobBase.IDBuffer(
				this.mSessionID.Value, this.mJobID.Value, this.Data.Request.ID);
		}
		#endregion // IDBuffer()

        #region EnvelopeHelper
        /// <summary>
        /// This property provides access to the envelope object pool.
        /// </summary>
        public override IXimuraEnvelopeHelper EnvelopeHelper
        {
            get
            {
                return mEnvelopeHelper;
            }
        }
        #endregion // EnvelopeHelper
	}
}