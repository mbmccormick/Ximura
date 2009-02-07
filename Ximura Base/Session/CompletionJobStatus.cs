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
using System.Configuration;
#endregion // using
namespace Ximura.Server
{
    /// <summary>
    /// This enumeration is used to indicate the completion job status.
    /// </summary>
    public enum CompletionJobStatus
    {
        /// <summary>
        /// The job is unset.
        /// </summary>
        Unset,
        /// <summary>
        /// This property is used when manual signaling is required for the completion job.
        /// </summary>
        Submitting,
        /// <summary>
        /// The job is complete.
        /// </summary>
        Complete,
        /// <summary>
        /// The job is currently processing.
        /// </summary>
        Processing,
        /// <summary>
        /// The job has timed out.
        /// </summary>
        TimeOut,
        /// <summary>
        /// The parent job has been cancelled.
        /// </summary>
        Cancelled,
        /// <summary>
        /// The completion job has been aborted.
        /// </summary>
        Aborted
    }
}