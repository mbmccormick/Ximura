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
namespace Ximura.Framework
{
    public delegate int DelGetCapacity(JobPriority priority);

    /// <summary>
    /// The dispatcher priority queue interface is used to implement a specific queuing algortihm,
    /// depending on the type of work being done by the system.
    /// </summary>
    public interface IXimuraDispatcherJobPriorityQueue : IDisposable
    {
        /// <summary>
        /// This method pushes a job on to the priority queue with the 
        /// normal priority.
        /// </summary>
        /// <param name="job">The new job</param>
        void Push(SecurityManagerJob job);
        /// <summary>
        /// This method pushes a job on to the priority queue.
        /// </summary>
        /// <param name="job">The job.</param>
        /// <param name="priority">The job priority.</param>
        void Push(SecurityManagerJob job, JobPriority priority);
        /// <summary>
        /// Gets the highest priority job and returns it.
        /// </summary>
        /// <returns>
        /// The highest priority job or null if there are no jobs 
        /// in the queue.
        /// </returns>
        SecurityManagerJob Pop();
        /// <summary>
        /// This method allows you to check the next object in the queue
        /// without removing it.
        /// </summary>
        /// <returns>The next object in the queue, or null is the queue is empty.</returns>
        SecurityManagerJob Peek();
        /// <summary>
        /// This method identifies whether the job is contained in the queue.
        /// </summary>
        /// <param name="value">The job.</param>
        /// <returns>Returns true if the job exists in the queue.</returns>
        bool Contains(SecurityManagerJob value);
        /// <summary>
        /// This method clears the queue of all jobs.
        /// </summary>
        void Clear();
        /// <summary>
        /// This returns the number of items in the queue.
        /// </summary>
        int Count { get;}
    }
}
