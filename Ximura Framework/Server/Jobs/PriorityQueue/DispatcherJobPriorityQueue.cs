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
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
#endregion // using
namespace Ximura.Server
{
	/// <summary>
	/// The DispatcherJobPriorityQueue is a threadsafe priority queue which 
	/// holds all the job requests for the dispatcher.
	/// </summary>
    public class DispatcherJobPriorityQueue : IXimuraDispatcherJobPriorityQueue
	{
		#region Declarations
		/// <summary>
		/// This hashtable is used to hold the job list.
		/// </summary>
		private Dictionary<Guid,SecurityManagerJob> JobList;
		/// <summary>
		/// This is the inner array list used to hold the queue information.
		/// </summary>
        private IXimuraJobPriorityQueueList QueueList;
		/// <summary>
		/// This is the internal comparer.
		/// </summary>
		private IComparer Comparer;

		private TimeSpan JobTTL = new TimeSpan(0,0,0,2,0); //2 seconds

		private bool disposed = false;

		private const int DEFAULTCAPACITY = 300;

        private object syncObject = new object();
		#endregion // Declarations
		#region Contructors
		/// <summary>
		/// The main constructor for the BinaryPriorityQueue.
		/// </summary>
		/// <param name="comparer">The comparer to use.</param>
		/// <param name="Capacity">The initial capacity of the queue.</param>
        public DispatcherJobPriorityQueue(DelGetCapacity capacitySetting)
		{
            this.Comparer = null;// comparer;

			JobList = new Dictionary<Guid,SecurityManagerJob>();
            //QueueList = new DispatcherJobPriorityQueueList(Capacity, comparer, 0);
            QueueList = new DispatcherJobPriorityGenericQueueList(capacitySetting);
        }
		#endregion
        #region Dispose()
        /// <summary>
		/// This method disposes the class.
		/// </summary>
		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}
 
		/// <summary>
		/// This is the internal dispose method.
		/// </summary>
		/// <param name="disposing">The primary disposing method.</param>
		protected virtual void Dispose(bool disposing)
		{
			if (!disposing)
			{
				return;
			}
            lock (syncObject)
			{
				disposed=true;
                QueueList.Clear();
                JobList.Clear();
            }
		}

		#endregion

		#region Push
		/// <summary>
		/// This method pushes a job on to the priority queue with the 
		/// normal priority.
		/// </summary>
		/// <param name="job">The new job</param>
		/// <returns>The current position on the queue.</returns>
		public void Push(SecurityManagerJob job)
		{
			Push(job, JobPriority.Normal);
		}
		/// <summary>
		/// This method pushes a job on to the priority queue.
		/// </summary>
		/// <param name="job">The job.</param>
		/// <param name="priority">The job priority.</param>
		/// <returns>The current position on the queue.</returns>
		public void Push(SecurityManagerJob job, JobPriority priority)
		{
            lock (syncObject)
			{
                if (job == null)
                    throw new ArgumentNullException("job", "job cannot be null.");

                if (!job.ID.HasValue)
                    throw new ArgumentNullException("job.ID", "job.ID cannot be null.");

                if (JobList.ContainsKey(job.ID.Value))
					throw new ArgumentException("The job is already queued.","job");

				//Add the job to the job pending queue. This is a seperate object
				//in order to increase the productivity of the Queue.
				JobList.Add(job.ID.Value,job);

				//Push the job to the list. We do this first in case the queue
				//is full and it throws an error.
				QueueList.Push(job.ID.Value,priority,JobTTL);
			}
		}
		#endregion // Push
		#region Pop
		/// <summary>
		/// Gets the highest priority job and returns it.
		/// </summary>
		/// <returns>
		/// The highest priority job or null if there are no jobs 
		/// in the queue.
		/// </returns>
		public SecurityManagerJob Pop()
		{
            lock (syncObject)
			{
				if (QueueList.Count==0) 
                    return null;

				Guid key = QueueList.Pop();

				SecurityManagerJob job = JobList[key];
                //This shouldn't happen, but we don't want to throw exceptions here.
                if (job != null)
				    JobList.Remove(key);

				return job;
			}
		}
		#endregion // Pop
		#region Peek
		/// <summary>
		/// This method allows you to check the next object in the queue
		/// without removing it.
		/// </summary>
		/// <returns>The next object in the queue, or null is the queue is empty.</returns>
		public SecurityManagerJob Peek()
		{
            lock (syncObject)
			{
				if (QueueList.Count==0) 
                    return null;

				Guid key = QueueList.Peek();
                SecurityManagerJob job = null;
                //Being super safe here, as we don't want to throw unexpected exceptions here
                if (key != Guid.Empty)
				    job = JobList[key];

				return job;
			}
		}
		#endregion // Peek

		#region Contains
		/// <summary>
		/// This method identifies whether the job is contained in the queue.
		/// </summary>
		/// <param name="value">The job.</param>
		/// <returns>Returns true if the job exists in the queue.</returns>
		public bool Contains(SecurityManagerJob value)
		{
            if (value == null)
                return false;

            lock (syncObject)
			{
				return JobList.ContainsValue(value);
			}
		}
		#endregion // Contains
		#region Clear()
		/// <summary>
		/// This method clears the queue of all jobs.
		/// </summary>
		public void Clear()
		{
            lock (syncObject)
			{
				QueueList.Clear();
				JobList.Clear();
			}
		}
		#endregion // Clear
		#region Count
		/// <summary>
		/// This returns the number of items in the queue.
		/// </summary>
		public int Count
		{
			get
			{
                lock (syncObject)
				{
					return JobList.Count;
				}
			}
		}
		#endregion // Count

	}
}