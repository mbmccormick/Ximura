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
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

using Ximura;
using Ximura.Data;
using Ximura.Helper;
#endregion // using
namespace Ximura.Server
{
    /// <summary>
    /// This is an extended job priority queue list that uses generics to hold
    /// multiple queues based on the job priority.
    /// </summary>
    public class DispatcherJobPriorityGenericQueueList : IXimuraJobPriorityQueueList
    {
        #region Declarations
        private Dictionary<JobPriority, Queue<JobPriorityRecord>> QueueList = null;
        private Dictionary<JobPriority, int> QueueListCapacity = null;
        #endregion // DEclarations

        #region Constructor
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        /// <param name="settings">The dispatcher settings</param>
        public DispatcherJobPriorityGenericQueueList(DelGetCapacity capacitySetting)
        {
            QueueList = new Dictionary<JobPriority, Queue<JobPriorityRecord>>();
            QueueListCapacity = new Dictionary<JobPriority, int>();

            Enum.GetValues(typeof(JobPriority))
                .Cast<JobPriority>()
                .ForEach(e => ConfigureQueueList(e, capacitySetting == null ? 500 : capacitySetting(e)));

        }

        private void ConfigureQueueList(JobPriority priority, int capacity)
        {
            QueueListCapacity.Add(priority, capacity);
            QueueList.Add(priority, new Queue<JobPriorityRecord>(capacity));
        }
        #endregion // Constructor

        #region Push
        /// <summary>
        /// This method inserts the job in to the queue.
        /// </summary>
        /// <param name="jobID">The job ID.</param>
        /// <param name="priority">The job priority.</param>
        /// <param name="jobTTL">The job time to live.</param>
        public void Push(Guid jobID, JobPriority priority, TimeSpan jobTTL)
        {
            Push(new JobPriorityRecord(priority, jobID, jobTTL));
        }
        /// <summary>
        /// This method inserts the job in to the queue, based on the priority set for the job.
        /// </summary>
        /// <param name="newJob"></param>
        public void Push(JobPriorityRecord newJob)
        {
            lock (this)
            {
                JobPriority priority = newJob.Priority;
                if (QueueList[priority].Count >= QueueListCapacity[priority])
                    throw new SCMCapacityException(newJob.ID.ToString());

                QueueList[priority].Enqueue(newJob);
            }
        }
        #endregion // Push
        #region Pop()
        /// <summary>
        /// This method removes the job with the highest priority from the priority queue.
        /// </summary>
        /// <returns>Returns the top job or null if there are no jobs in the queue.</returns>
        public Guid Pop()
        {
            lock (this)
            {
                JobPriorityRecord job = PopJob();

                if (job == null)
                    return Guid.Empty;
                else
                    return job.ID;
            }
        }

        private JobPriorityRecord PopJob()
        {
            if (QueueList[JobPriority.Realtime].Count > 0)
                return QueueList[JobPriority.Realtime].Dequeue();
            else if (QueueList[JobPriority.High].Count > 0)
                return QueueList[JobPriority.High].Dequeue();
            else if (QueueList[JobPriority.AboveNormal].Count > 0)
                return QueueList[JobPriority.AboveNormal].Dequeue();
            else if (QueueList[JobPriority.Normal].Count > 0)
                return QueueList[JobPriority.Normal].Dequeue();
            else if (QueueList[JobPriority.BelowNormal].Count > 0)
                return QueueList[JobPriority.BelowNormal].Dequeue();
            else if (QueueList[JobPriority.Low].Count > 0)
                return QueueList[JobPriority.Low].Dequeue();

            return null;
        }
        #endregion // Pop()
        #region Peek()
        /// <summary>
        /// This method peeks to the top of the queue and return the next job.
        /// </summary>
        /// <returns>This method returns the job with the highest priority 
        /// or returns null if there are no jobs to process.</returns>
        public Guid Peek()
        {
            lock (this)
            {
                JobPriorityRecord job = PeekJob();

                if (job == null)
                    return Guid.Empty;
                else
                    return job.ID;
            }
        }

        private JobPriorityRecord PeekJob()
        {
            if (QueueList[JobPriority.Realtime].Count > 0)
                return QueueList[JobPriority.Realtime].Peek();
            else if (QueueList[JobPriority.High].Count > 0)
                return QueueList[JobPriority.High].Peek();
            else if (QueueList[JobPriority.AboveNormal].Count > 0)
                return QueueList[JobPriority.AboveNormal].Peek();
            else if (QueueList[JobPriority.Normal].Count > 0)
                return QueueList[JobPriority.Normal].Peek();
            else if (QueueList[JobPriority.BelowNormal].Count > 0)
                return QueueList[JobPriority.BelowNormal].Peek();
            else if (QueueList[JobPriority.Low].Count > 0)
                return QueueList[JobPriority.Low].Peek();

            return null;
        }
        #endregion // Peek

        #region GetExpiredJobs
        /// <summary>
        /// This method is currently not implemented.
        /// </summary>
        /// <param name="purge"></param>
        /// <returns></returns>
        public Guid[] GetExpiredJobs(bool purge)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }
        #endregion // GetExpiredJobs

        #region Clear
        /// <summary>
        /// This method clears the priority list of all pending jobs.
        /// </summary>
        public void Clear()
        {
            lock (this)
            {
                QueueList[JobPriority.Low].Clear();
                QueueList[JobPriority.BelowNormal].Clear();
                QueueList[JobPriority.Normal].Clear();
                QueueList[JobPriority.AboveNormal].Clear();
                QueueList[JobPriority.High].Clear();
                QueueList[JobPriority.Realtime].Clear();
           }
        }
        #endregion // Clear

        #region ToString()
        /// <summary>
        /// This method returns a text representation of the queue.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return base.ToString();
        }
        #endregion // ToString()

        #region Count
        /// <summary>
        /// This property returns the total number of jobs pending.
        /// </summary>
        public int Count
        {
            get 
            {
                lock (this)
                {
                    return 
                        QueueList[JobPriority.Realtime].Count +
                        QueueList[JobPriority.High].Count +
                        QueueList[JobPriority.AboveNormal].Count +
                        QueueList[JobPriority.Normal].Count +
                        QueueList[JobPriority.BelowNormal].Count +
                        QueueList[JobPriority.Low].Count;
                }
            }
        }
        #endregion // Count
        #region CountPriority
        /// <summary>
        /// This method returns the total number of jobs pending for a particular job priority.
        /// </summary>
        /// <param name="priority"></param>
        /// <returns></returns>
        public int CountPriority(JobPriority priority)
        {
            lock (this)
            {
                return QueueList[priority].Count;
            }
        }
        #endregion // CountPriority
    }
}
