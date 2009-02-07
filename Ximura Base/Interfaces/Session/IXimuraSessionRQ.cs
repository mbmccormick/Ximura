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
using System.Security.Cryptography;

using Ximura;
using Ximura.Data;
using Ximura.Server;

#endregion // using
namespace Ximura
{
    /// <summary>
    /// This interface is used by the session object to support requests.
    /// </summary>
    public interface IXimuraSessionRQ : IXimuraSessionRQAsync
    {
        /// <summary>
        /// This method cancels a pending request.
        /// </summary>
        /// <param name="jobID">The job id.</param>
        void CancelRequest(Guid jobID);
        /// <summary>
        /// Process a synchronous request.
        /// </summary>
        /// <param name="Data">The data.</param>
        void ProcessRequest(IXimuraRQRSEnvelope Data);
        /// <summary>
        /// Process a synchronous request.
        /// </summary>
        /// <param name="Data">The data.</param>
        /// <param name="priority">The request priority.</param>
        void ProcessRequest(IXimuraRQRSEnvelope Data, JobPriority priority);
        /// <summary>
        /// Process a synchronous request.
        /// </summary>
        /// <param name="Data">The data.</param>
        /// <param name="ProgressCallback">The progress calback delegate. 
        /// This can be used to report progress during long running processes.</param>
        void ProcessRequest(IXimuraRQRSEnvelope Data, CommandProgressCallback ProgressCallback);
        /// <summary>
        /// Process a synchronous request.
        /// </summary>
        /// <param name="Data">The data.</param>
        /// <param name="priority">The request priority.</param>
        /// <param name="ProgressCallback">The progress calback delegate. 
        /// This can be used to report progress during long running processes.</param>
        void ProcessRequest(IXimuraRQRSEnvelope Data, JobPriority priority, CommandProgressCallback ProgressCallback);
    }
}
