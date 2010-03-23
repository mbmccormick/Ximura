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
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Security;
using System.Security.Cryptography;

using Ximura;
using Ximura.Helper;
using Ximura.Server;

using Ximura.Command;
#endregion // using
namespace Ximura.Server
{
    /// <summary>
    /// This class is thread synchronized and holds a collection of jobs currently being processed.
    /// </summary>
    public class JobCollection : IList, IDisposable
    {
        #region Declarations
        private object syncCollection = new object();

        private JobPriority PriorityOverride;
        private bool mDisposed;

        /// <summary>
        /// This collection holds the job object along with it's unique trace id.
        /// </summary>
        private Dictionary<Guid, JobBase> jobCollection = new Dictionary<Guid, JobBase>();
        /// <summary>
        /// This object holds the job id as a value
        /// </summary>
        private Dictionary<Guid, Guid> jobCollectionID = new Dictionary<Guid, Guid>();
        /// <summary>
        /// This object holds the track id as a value.
        /// </summary>
        private Dictionary<Guid, Guid> jobCollectionTrackID = new Dictionary<Guid, Guid>();

        //private Hashtable jobDependency = null;
        private bool mFixedSize = false;
        private int mCapacity;
        #endregion // Declarations
        #region Constructors
        /// <summary>
        /// This is the default constructor. There will be no limit to the number of jobs that can be registered.
        /// </summary>
        public JobCollection() : this(-1, JobPriority.AboveNormal, false) { }

        /// <summary>
        /// This is main constructor.
        /// </summary>
        /// <param name="maxCapacity">The maximum number of jobs that the class can contain.</param>
        /// <param name="PriorityOverride">The priority override level. Jobs that have the same or greater priority will be allowed
        /// to submit jobs regardless of the collection status.</param>
        /// <param name="useDependency">This method will track jobs having the same dependency.</param>
        public JobCollection(int maxCapacity, JobPriority PriorityOverride, bool useDependency)
        {
            mCapacity = maxCapacity;
            mFixedSize = maxCapacity != -1;
            mDisposed = false;
            this.PriorityOverride = PriorityOverride;
        }
        #endregion // Declarations

        #region Add
        /// <summary>
        /// This method adds a job to the collection.
        /// </summary>
        /// <param name="value">The JobBase object to add.</param>
        /// <returns>Returns 0.</returns>
        public virtual int Add(object value)
        {
            JobBase jobRQ = value as JobBase;

            if (jobRQ == null)
                throw new ArgumentException("Value must derive from JobBase", "value");

            return Add(jobRQ);
        }
        /// <summary>
        /// This method adds a job to the collection.
        /// </summary>
        /// <param name="jobRQ">The JobBase object to add.</param>
        /// <returns>Returns 0.</returns>
        public virtual int Add(JobBase jobRQ)
        {
            lock (syncCollection)
            {
                DisposeCheck();

                if (IsFixedSize && mCapacity == jobCollection.Count && jobRQ.Priority < PriorityOverride)
                    throw new SCMCapacityException("The job collection capacity has been reached.");

                jobCollection.Add(jobRQ.TrackID, jobRQ);
                jobCollectionID.Add(jobRQ.TrackID, jobRQ.ID.Value);
                jobCollectionTrackID.Add(jobRQ.ID.Value, jobRQ.TrackID);

                return 0;
            }
        }
        #endregion // Add
        #region Remove
        /// <summary>
        /// Removes the job from the collection.
        /// </summary>
        /// <param name="value">A jobbase object.</param>
        public void Remove(object value)
        {
            if (value is Guid)
            {
                Remove((Guid)value);
                return;
            }
            else if (value is JobBase)
            {
                Remove((JobBase)value);
                return;
            }

            throw new ArgumentException("value must derive from JobBase or Guid", "value");
        }
        /// <summary>
        /// This method removes the job from the collection.
        /// </summary>
        /// <param name="jobID">The job ID.</param>
        public void Remove(Guid jobID)
        {
            lock (syncCollection)
            {
                DisposeCheck();

                if (jobID == Guid.Empty)
                    throw new ArgumentNullException("Job ID cannot be guid empty.");

                if (!jobCollectionTrackID.ContainsKey(jobID))
                    return;

                Guid jobTrackID = jobCollectionTrackID[jobID];

                jobCollection.Remove(jobTrackID);
                jobCollectionID.Remove(jobTrackID);
                jobCollectionTrackID.Remove(jobID);
            }
        }
        /// <summary>
        /// Removes the job from the collection.
        /// </summary>
        /// <param name="job">The job to remove.</param>
        public void Remove(JobBase job)
        {
            lock (syncCollection)
            {
                DisposeCheck();
                Guid jobID = job.ID.Value;

                jobCollection.Remove(job.TrackID);

                if (jobID == Guid.Empty)
                    jobID = jobCollectionID[job.TrackID];

                jobCollectionID.Remove(job.TrackID);
                jobCollectionTrackID.Remove(jobID);
            }
        }

        #endregion // Remove
        #region Clear
        /// <summary>
        /// This method clears the collection of all jobs.
        /// </summary>
        public void Clear()
        {
            lock (syncCollection)
            {
                DisposeCheck();
                jobCollection.Clear();
                jobCollectionID.Clear();
                jobCollectionTrackID.Clear();
            }
        }
        #endregion // Clear
        #region Contains
        /// <summary>
        /// Returns true if the job is in the collection.
        /// </summary>
        /// <param name="value">A Guid to identify the job.</param>
        /// <returns>Returns true for success.</returns>
        public virtual bool Contains(object value)
        {
            if (value is Guid)
                return Contains((Guid)value);
            else if (value is JobBase)
                return Contains((JobBase)value);

            throw new ArgumentException("value must be a Guid or derive from JobBase", "value");
        }
        /// <summary>
        /// Returns true if the job is in the collection.
        /// </summary>
        /// <param name="job">A object that derives from JobBase.</param>
        /// <returns>Returns true for success.</returns>
        public virtual bool Contains(JobBase job)
        {
            return Contains(job.ID);
        }
        /// <summary>
        /// Returns true if the job is in the collection.
        /// </summary>
        /// <param name="jobID">A Guid to identify the job.</param>
        /// <returns>Returns true for success.</returns>
        public virtual bool Contains(Guid jobID)
        {
            lock (syncCollection)
            {
                DisposeCheck();
                return jobCollectionTrackID.ContainsKey(jobID);
            }
        }
        #endregion // Contains
        #region this[] - supported
        /// <summary>
        /// This returns a job from the collection based on the job ID.
        /// </summary>
        public JobBase this[Guid jobID]
        {
            get
            {
                lock (syncCollection)
                {
                    if (jobID == Guid.Empty || !jobCollectionTrackID.ContainsKey(jobID))
                        return null;

                    Guid jobTrackID = jobCollectionTrackID[jobID];
                    return jobCollection[jobTrackID] as JobBase;
                }
            }
        }
        #endregion // this - supported

        #region JobIDs
        /// <summary>
        /// This is a collection of JobIDs for the current jobs.
        /// </summary>
        public ICollection JobIDs
        {
            get
            {
                lock (syncCollection)
                {
                    Guid[] keys = new Guid[Count];

                    jobCollectionTrackID.Keys.CopyTo(keys, 0);
                    return keys as ICollection;
                }
            }
        }
        #endregion // JobIDs

        #region IsFixedSize
        /// <summary>
        /// Returns true if the job collection has a maximum size.
        /// </summary>
        public virtual bool IsFixedSize
        {
            get
            {
                return mFixedSize;
            }
        }
        #endregion // IsFixedSize
        #region Capacity
        /// <summary>
        /// The number of jobs currently in the collection.
        /// </summary>
        public int Capacity
        {
            get
            {
                lock (syncCollection)
                {
                    DisposeCheck();
                    return mCapacity;
                }
            }
        }
        #endregion
        #region Count
        /// <summary>
        /// The number of jobs currently in the collection.
        /// </summary>
        public int Count
        {
            get
            {
                lock (syncCollection)
                {
                    DisposeCheck();
                    return jobCollection.Count;
                }
            }
        }
        #endregion // Count
        #region SyncRoot
        /// <summary>
        /// Always returns itself, as this is a synchronized class.
        /// </summary>
        public object SyncRoot
        {
            get
            {
                DisposeCheck();
                return this;
            }
        }
        #endregion // SyncRoot
        #region IsSynchronized
        /// <summary>
        /// Always returns true as this is a synchronized class.
        /// </summary>
        public bool IsSynchronized
        {
            get
            {
                return true;
            }
        }
        #endregion // IsSynchronized
        #region IsReadOnly
        /// <summary>
        /// Returns true if the collection has reached its capacity.
        /// </summary>
        public bool IsReadOnly
        {
            get
            {
                DisposeCheck();
                lock (syncCollection)
                {
                    return mCapacity != -1 && mCapacity != jobCollection.Count;
                }
            }
        }
        #endregion // IsReadOnly

        #region Not supported IList Members
        #region this[int index]
        /// <summary>
        /// Not supported.
        /// </summary>
        public object this[int index]
        {
            get
            {
                throw new NotSupportedException();
            }
            set
            {
                throw new NotSupportedException();
            }
        }
        #endregion // this[int index]

        #region RemoveAt
        /// <summary>
        /// Not supported.
        /// </summary>
        /// <param name="index"></param>
        public void RemoveAt(int index)
        {
            throw new NotSupportedException();
        }
        #endregion // RemoveAt
        #region Insert
        /// <summary>
        /// Not supported.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        public void Insert(int index, object value)
        {
            throw new NotSupportedException();
        }
        #endregion // Insert
        #region IndexOf
        /// <summary>
        /// Not supported.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public int IndexOf(object value)
        {
            throw new NotSupportedException();
        }
        #endregion // IndexOf

        #region CopyTo
        /// <summary>
        /// Not supported.
        /// </summary>
        /// <param name="array"></param>
        /// <param name="index"></param>
        public void CopyTo(Array array, int index)
        {
            throw new NotSupportedException();
        }
        #endregion // CopyTo
        #region GetEnumerator()
        /// <summary>
        /// Not supported.
        /// </summary>
        /// <returns></returns>
        public IEnumerator GetEnumerator()
        {
            throw new NotSupportedException();
        }
        #endregion
        #endregion

        #region Dispose()
        /// <summary>
        /// This method disposes of the collection.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }
        /// <summary>
        /// This method provides specific dispose code for the collection.
        /// </summary>
        /// <param name="disposing">Set to true if this is the first time this is called.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!mDisposed && disposing)
            {
                lock (syncCollection)
                {
                    jobCollection.Clear();
                    mDisposed = true;
                }
            }
        }
        #endregion
        #region DisposeCheck()
        /// <summary>
        /// This method checks whether the collection has been disposed, and if so throws an exception.
        /// </summary>
        protected void DisposeCheck()
        {
            if (mDisposed)
                throw new ObjectDisposedException("SCMJobCollection");
        }
        #endregion // DisposeCheck()

    }
}