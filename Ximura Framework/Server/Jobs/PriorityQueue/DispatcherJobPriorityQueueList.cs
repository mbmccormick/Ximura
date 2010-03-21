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
    /// The job priority queue holds the job list.
    /// </summary>
    public class DispatcherJobPriorityQueueList : IXimuraJobPriorityQueueList
    {
        #region Declarations
        private const int DEFAULTSIZE = 16;
        private const int DEFAULTGROWTH = 16;
        private JobPriorityRecord[] mItems;
        private int mSize, mGrowth, mCapacity;
        private IComparer externalComparer = null;
        #endregion // Declarations
        #region Constructor
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        public DispatcherJobPriorityQueueList()
            : this(DEFAULTSIZE, null, DEFAULTGROWTH)
        {
        }
        /// <summary>
        /// This is the main constructor for the JobPriorityQueue.
        /// </summary>
        /// <param name="Capacity">The initial capacity.</param>
        /// <param name="comparer">The comparer or null if the default is required.</param>
        /// <param name="GrowthFactor">The growth factor.</param>
        public DispatcherJobPriorityQueueList(int Capacity, IComparer comparer, int GrowthFactor)
        {
            mItems = new JobPriorityRecord[Capacity];
            externalComparer = comparer;
            mSize = 0;
            mCapacity = Capacity;
            mGrowth = GrowthFactor;
        }
        #endregion // Constructor

        #region Push
        /// <sumary>
        /// This method pushes a new item onto the queue.
        /// </summary>
        /// <param name="jobID">The job id</param>
        /// <param name="priority">The job priority</param>
        /// <param name="jobTTL">The job Time To Live</param>
        /// <returns>The job position.</returns>
        public void Push(Guid jobID, JobPriority priority, TimeSpan jobTTL)
        {
            JobPriorityRecord newJob = new JobPriorityRecord(priority, jobID, jobTTL);
            Push(newJob);
        }
        /// <sumary>
        /// This method pushes a new item onto the queue.
        /// </sumary>
        /// <param name="newJob">The job to be inserted in to the queue</param>
        /// <returns>The job position.</returns>
        public void Push(JobPriorityRecord newJob)
        {
            int position = ResolveInsertPosition(newJob);
            Insert(position, newJob);
        }

        private int ResolveInsertPosition(JobPriorityRecord newJob)
        {
            if (this.mSize == 0) return 0;

            int pos = this.mSize - 1;
            try
            {
                if (externalComparer == null)
                {
                    while (pos > 0 &&
                        (Compare(newJob, mItems[pos]) < 0))
                    {
                        pos--;
                    }
                }
                else
                {
                    while (pos > 0 &&
                        (externalComparer.Compare(newJob, mItems[pos]) < 0))
                    {
                        pos--;
                    }
                }
            }
            catch (IndexOutOfRangeException)
            {
                throw new ArgumentException("Bad IComparer result");
            }
            catch (Exception exception1)
            {
                throw new InvalidOperationException("IComparer failed", exception1);
            }

            return pos;
        }
        #endregion // Push
        #region Pop
        public Guid Pop()
        {
            JobPriorityRecord nextJob = PeekJob();
            RemoveAt(0);
            return nextJob.ID;
        }
        #endregion // Pop
        #region Peek
        public Guid Peek()
        {
            return PeekJob().ID;
        }

        private JobPriorityRecord PeekJob()
        {
            if (mSize == 0)
                throw new ArgumentNullException("The queue is empty");

            return mItems[0];

        }
        #endregion // Peek

        #region GetExpiredJobs
        /// <summary>
        /// This method returns the Guids of the expired jobs
        /// </summary>
        /// <param name="purge">Pass true if they require the jobs to be deleted</param>
        /// <returns>An array of Guids containing the expired jobs.</returns>
        public Guid[] GetExpiredJobs(bool purge)
        {
            ArrayList expired = InternalGetExpiredJobsPosition();
            Guid[] guids = new Guid[expired.Count];

            for (int item = 0; item < expired.Count; item++)
            {
                guids[item] = this.mItems[(int)expired[item]].ID;
            }

            return guids;
        }
        #endregion // GetExpiredJobs

        #region Clear()
        /// <summary>
        /// This method clears the queue.
        /// </summary>
        public virtual void Clear()
        {
            Array.Clear(mItems, 0, mSize);
            mSize = 0;
        }
        #endregion // Clear
        #region Count
        /// <summary>
        /// This is the size of the current collection.
        /// </summary>
        public int Count
        {
            get
            {
                return this.mSize;
            }
        }
        #endregion // Count

        #region Insert
        private void Insert(int index, JobPriorityRecord value)
        {
            if ((index < 0) || (index > mSize))
            {
                throw new ArgumentOutOfRangeException("index", "Index is out of range.");
            }
            if (mSize == mItems.Length)
            {
                this.EnsureCapacity(mSize + 1);
            }
            if (index < mSize)
            {
                Array.Copy(mItems, index, mItems, (int)(index + 1), (int)(mSize - index));
            }
            mItems[index] = value;
            mSize++;
        }
        #endregion // Insert
        #region RemoveAt
        private void RemoveAt(int index)
        {
            if ((index < 0) || (index >= mSize))
            {
                throw new ArgumentOutOfRangeException("index", "Index is out of range.");
            }
            mSize--;
            if (index < mSize)
            {
                Array.Copy(mItems, (int)(index + 1), mItems, index, (int)(mSize - index));
            }
            mItems[mSize] = null;
        }
        #endregion // RemoveAt
        #region InternalGetExpiredJobsPosition
        private ArrayList InternalGetExpiredJobsPosition()
        {
            ArrayList positionExpired = new ArrayList();

            for (int loop = 0; loop < mSize; loop++)
            {
                if (mItems[loop].JobExpired)
                    positionExpired.Add(loop);
            }

            return positionExpired;
        }
        #endregion // InternalGetExpiredJobsPosition

        #region Capacity and EnsureCapacity
        /// <summary>
        /// This property changes the capacity of the Queue.
        /// </summary>
        private int Capacity
        {
            get
            {
                return mItems.Length;
            }
            set
            {
                if (value == mItems.Length)
                {
                    return;
                }
                if (value < mSize)
                {
                    throw new ArgumentOutOfRangeException("value", "You cannot shrink to less that the current length.");
                }
                if (value > 0)
                {
                    JobPriorityRecord[] newItems = new JobPriorityRecord[value];
                    if (this.mSize > 0)
                    {
                        Array.Copy(this.mItems, 0, newItems, 0, mSize);
                    }
                    mItems = newItems;
                }
                else
                {
                    mItems = new JobPriorityRecord[DEFAULTSIZE];
                }
            }
        }

        private void EnsureCapacity(int min)
        {
            if (mItems.Length >= min)
            {
                return;
            }
            int num1 = (mItems.Length == 0) ? mCapacity : (mItems.Length + mGrowth);
            if (num1 < min)
            {
                num1 = min;
            }
            this.Capacity = num1;
        }

        #endregion // Capacity and EnsureCapacity

        #region DeepSort
        private void DeepSort(JobPriorityRecord[] mItems, int left, int right)
        {
            while (true)
            {
                int num1 = left;
                int num2 = right;
                JobPriorityRecord obj1 = mItems[(num1 + num2) >> 1];
                do
                {
                    try
                    {
                        if (externalComparer == null)
                        {
                            while (Compare(mItems[num1], obj1) < 0)
                            {
                                num1++;
                            }
                            while (Compare(obj1, mItems[num2]) < 0)
                            {
                                num2--;
                            }
                        }
                        else
                        {
                            while (externalComparer.Compare(mItems[num1], obj1) < 0)
                            {
                                num1++;
                            }
                            while (externalComparer.Compare(obj1, mItems[num2]) < 0)
                            {
                                num2--;
                            }
                        }
                    }
                    catch (IndexOutOfRangeException)
                    {
                        throw new ArgumentException("Bad IComparer result");
                    }
                    catch (Exception exception1)
                    {
                        throw new InvalidOperationException("IComparer failed", exception1);
                    }
                    if (num1 > num2)
                    {
                        break;
                    }
                    if (num1 < num2)
                    {
                        JobPriorityRecord obj2 = mItems[num1];
                        mItems[num1] = mItems[num2];
                        mItems[num2] = obj2;
                    }
                    num1++;
                    num2--;
                }
                while (num1 <= num2);
                if ((num2 - left) <= (right - num1))
                {
                    if (left < num2)
                    {
                        DeepSort(mItems, left, num2);
                    }
                    left = num1;
                }
                else
                {
                    if (num1 < right)
                    {
                        DeepSort(mItems, num1, right);
                    }
                    right = num2;
                }
                if (left >= right)
                {
                    return;
                }
            }
        }
        #endregion // DeepSort
        #region Compare
        private int Compare(JobPriorityRecord x, JobPriorityRecord y)
        {
            return (int)x.Priority - (int)y.Priority;
        }
        #endregion // Compare
    }
}
