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
    /// This is the job priority structure.
    /// </summary>
    public class JobPriorityRecord
    {
        #region Declarations
        /// <summary>
        /// The job priority.
        /// </summary>
        public JobPriority Priority;
        /// <summary>
        /// The push time.
        /// </summary>
        public DateTime PushTime;
        /// <summary>
        /// The priority ID.
        /// </summary>
        public Guid ID;
        private TimeSpan mTTL;
        #endregion // Declarations
        #region Constructor
        /// <summary>
        /// This is the constructor.
        /// </summary>
        /// <param name="level">The job priority level.</param>
        /// <param name="jobID">The job id.</param>
        /// <param name="TTL">The time to live.</param>
        public JobPriorityRecord(JobPriority level, Guid jobID, TimeSpan TTL)
        {
            PushTime = DateTime.Now;
            Priority = level;
            ID = jobID;
            mTTL = TTL;
        }
        #endregion // Constructor

        #region JobExpired
        /// <summary>
        /// This method returns true if the job has expired.
        /// </summary>
        public bool JobExpired
        {
            get
            {
                return ExpireTime < DateTime.Now;
            }
        }
        #endregion // JobExpired
        #region ExpireTime
        /// <summary>
        /// This property increments the time to live.
        /// </summary>
        public DateTime ExpireTime
        {
            get { return PushTime.Add(mTTL); }
        }
        #endregion // ExpireTime
        //#region Static methods Constructor and Empty
        //private static JobPriorityRecord mEmpty;
        ///// <summary>
        ///// This is the static constructor.
        ///// </summary>
        //static JobPriorityRecord()
        //{
        //    mEmpty = new JobPriorityRecord(JobPriority.Normal, Guid.Empty, TimeSpan.Zero);
        //}
        ///// <summary>
        ///// This is the empty priority object.
        ///// </summary>
        //public static JobPriorityRecord Empty
        //{
        //    get
        //    {
        //        return mEmpty;
        //    }
        //}
        //#endregion // Static methods
    }
}
