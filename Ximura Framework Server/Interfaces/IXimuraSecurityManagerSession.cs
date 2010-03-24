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
namespace Ximura.Framework
{
    /// <summary>
    /// This interface is shared with a session object and enables it to make system requests.
    /// </summary>
    internal interface IXimuraSecurityManagerSession
    {
        /// <summary>
        /// This method is used to submit jobs to the security manager.
        /// </summary>
        /// <param name="jobRQ">The job.</param>
        /// <returns>The job Guid.</returns>
        Guid JobProcess(JobBase jobRQ, bool async);
        /// <summary>
        /// This method cancels a pending request.
        /// </summary>
        /// <param name="jobID">The job ID to cancel.</param>
        void JobCancel(Guid jobID);
    }
}
