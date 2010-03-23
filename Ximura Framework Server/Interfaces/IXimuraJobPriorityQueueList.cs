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
    /// This interface is implemented by classes that implements a specific job queue algorithm class.
    /// </summary>
    public interface IXimuraJobPriorityQueueList
    {
        void Push(Guid jobID, JobPriority priority, TimeSpan jobTTL);
        void Push(JobPriorityRecord newJob);
        Guid Pop();
        Guid Peek();
        Guid[] GetExpiredJobs(bool purge);
        void Clear();
        int Count { get;}
    }
}
