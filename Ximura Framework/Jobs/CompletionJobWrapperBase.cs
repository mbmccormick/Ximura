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
using Ximura.Helper;
using CH=Ximura.Helper.Common;
#endregion // using
namespace Ximura.Framework
{
	/// <summary>
	/// Summary description for CompletionJobWrapperBase.
	/// </summary>
	public abstract class CompletionJobWrapperBase : JobWrapper, IXimuraSessionRQAsync
	{
		#region Declarations
		private JobCollection mJobCollection;
		/// <summary>
		/// The job requests.
		/// </summary>
		protected int jobRequests = 0;
		/// <summary>
		/// The parent job.
		/// </summary>
		protected SecurityManagerJob mParentJob;
		#endregion // Declarations
		#region Constructor
		internal CompletionJobWrapperBase(JobBase baseJob):base(baseJob){}
		#endregion // Constructor
		#region Reset()
		/// <summary>
		/// This private method reset the Completion job.
		/// </summary>
		public override void Reset()
		{
			base.Reset();
			if (mJobCollection!=null)
				mJobCollection.Clear();
			mParentJob = null;
			jobRequests = 0;
		}
		#endregion

		#region ProcessRequestAsync Helper Methods
		/// <summary>
		/// Process an asychronous request.
		/// </summary>
		/// <param name="data">The data</param>
		/// <param name="RSCallback">The call back completion delegate.</param>
		/// <returns>The job guid.</returns>
		public Guid ProcessRequestAsync(IXimuraRQRSEnvelope data, CommandRSCallback RSCallback)
		{
			return ProcessRequestAsync(Guid.NewGuid(),data,RSCallback,null, JobPriority.Normal);
		}
		/// <summary>
		/// Process an asychronous request.
		/// </summary>
		/// <param name="data">The data</param>
		/// <param name="RSCallback">The call back completion delegate.</param>
		/// <param name="ProgessCallback">The request progress delegate. Set this to null if not needed.</param>
		/// <param name="priority">The request priority.</param>
		/// <returns>The job guid.</returns>
		public Guid ProcessRequestAsync(IXimuraRQRSEnvelope data, 
			CommandRSCallback RSCallback, CommandProgressCallback ProgessCallback, JobPriority priority)
		{
			return ProcessRequestAsync(Guid.NewGuid(),data,RSCallback,ProgessCallback, priority);
		}
		/// <summary>
		/// Process an asychronous request.
		/// </summary>
		/// <param name="jobID">The job ID. This should be set to a new Guid.</param>
		/// <param name="data">The data</param>
		/// <param name="RSCallback">The call back completion delegate.</param>
		/// <returns>The job guid.</returns>
		public Guid ProcessRequestAsync(Guid jobID, IXimuraRQRSEnvelope data, 
			CommandRSCallback RSCallback)
		{
			return ProcessRequestAsync(jobID,data,RSCallback,null,JobPriority.Normal);
		}
		#endregion // Helper Methods
		#region ProcessRequestAsync abstract method
		/// <summary>
		/// Process an asychronous request.
		/// </summary>
		/// <param name="jobID">The job ID. This should be set to a new Guid.</param>
		/// <param name="data">The data</param>
		/// <param name="RSCallback">The call back completion delegate.</param>
		/// <param name="ProgessCallback">The request progress delegate. Set this to null if not needed.</param>
		/// <param name="priority">The request priority.</param>
		/// <returns>The job guid.</returns>
		public abstract Guid ProcessRequestAsync(Guid jobID, IXimuraRQRSEnvelope data, 
			CommandRSCallback RSCallback, CommandProgressCallback ProgessCallback, JobPriority priority);
		#endregion

		#region AddJob
		/// <summary>
		/// This protected method is used to add a job to the collection.
		/// </summary>
		/// <param name="jobID">The job ID. This should be set to a new Guid.</param>
		/// <param name="data">The data</param>
		/// <param name="RSCallback">The call back completion delegate.</param>
		/// <param name="ProgessCallback">The request progress delegate. Set this to null if not needed.</param>
		/// <param name="priority">The request priority.</param>
		/// <returns>The guid of the job queued.</returns>
		protected virtual Guid AddJob(Guid jobID, IXimuraRQRSEnvelope data, 
			CommandRSCallback RSCallback, CommandProgressCallback ProgessCallback, 
			JobPriority priority)
		{
			//Create a new job holder.
			JobHolder jh = new JobHolder(jobID, data, RSCallback, ProgessCallback, priority);

			//We add the job to the queue as it will only be executed when the Execute()
			//command is called.
			lock (this)
			{
				WorkTable.Add(jh);
				Interlocked.Increment(ref jobRequests);
#if (DEBUG)
				//System.Diagnostics.Debug.WriteLine("Inc: " + jobID.ToString() + " -> " + jobRequests.ToString());
#endif
			}

			return jobID;
		}
		#endregion // AddJob
		#region RemoveJob
		/// <summary>
		/// This method will remove a child job and call any delegate.
		/// </summary>
		/// <param name="jobID">The job ID</param>
		/// <param name="sender">The sender.</param>
		/// <param name="args">The arguments.</param>
		/// <returns></returns>
		protected virtual bool RemoveJob(Guid jobID, object sender, CommandRSEventArgs args)
		{
            JobHolder jh;

            lock (this)
            {
                if (!WorkTable.Contains(jobID))
                    return false;

#if (DEBUG)
                //System.Diagnostics.Debug.WriteLine("Dec: " + jobID.ToString() + " -> " + jobRequests.ToString());
#endif
			    jh = WorkTable[jobID] as JobHolder;

                if (jh != null)
                {
                    Interlocked.Decrement(ref jobRequests);
                    //Remove the job.
                    WorkTable.Remove(jh);
                }
            }

			if (jh.RSCallback!=null)
				jh.RSCallback(sender,args);

			//Stop any time-out timer.
			if (jobRequests == 0)
				ResetTimer();

			return true;
		}
		#endregion // RemoveJob

		#region WorkTable
		/// <summary>
		/// This is the worktable that contains the jobs.
		/// </summary>
		protected JobCollection WorkTable
		{
			get
			{
				if (mJobCollection == null)
					mJobCollection = new JobCollection();
				return mJobCollection;
			}
		}
		#endregion // WorkTable

		#region ParentJob
		/// <summary>
		/// This is the parent security manager job
		/// </summary>
		public SecurityManagerJob ParentJob
		{
			get{return mParentJob;}
		}
		#endregion // ParentJob
    }
}