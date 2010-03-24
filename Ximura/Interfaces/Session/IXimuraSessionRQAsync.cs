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
﻿#region using
using System;
using System.Globalization;
using System.Security.Cryptography;

using Ximura;
using Ximura.Framework;
using Ximura.Data;
using Ximura.Framework;

#endregion // using
namespace Ximura
{
    /// <summary>
    /// This is the async interface
    /// </summary>
    public interface IXimuraSessionRQAsync
    {
        /// <summary>
        /// Process an asychronous request.
        /// </summary>
        /// <param name="data">The data</param>
        /// <param name="RSCallback">The call back completion delegate.</param>
        /// <returns>The job guid.</returns>
        Guid ProcessRequestAsync(IXimuraRQRSEnvelope data, CommandRSCallback RSCallback);
        /// <summary>
        /// Process an asychronous request.
        /// </summary>
        /// <param name="data">The data</param>
        /// <param name="RSCallback">The call back completion delegate.</param>
        /// <param name="ProgessCallback">The request progress delegate. Set this to null if not needed.</param>
        /// <param name="priority">The request priority.</param>
        /// <returns>The job guid.</returns>
        Guid ProcessRequestAsync(IXimuraRQRSEnvelope data, CommandRSCallback RSCallback,
            CommandProgressCallback ProgressCallback, JobPriority priority);
        /// <summary>
        /// Process an asychronous request.
        /// </summary>
        /// <param name="jobID">The job ID. This should be set to a new Guid.</param>
        /// <param name="data">The data</param>
        /// <param name="RSCallback">The call back completion delegate.</param>
        /// <returns>The job guid.</returns>
        Guid ProcessRequestAsync(Guid jobID, IXimuraRQRSEnvelope data, CommandRSCallback RSCallback);
        /// <summary>
        /// Process an asychronous request.
        /// </summary>
        /// <param name="jobID">The job ID. This should be set to a new Guid.</param>
        /// <param name="data">The data</param>
        /// <param name="RSCallback">The call back completion delegate.</param>
        /// <param name="ProgessCallback">The request progress delegate. Set this to null if not needed.</param>
        /// <param name="priority">The request priority.</param>
        /// <returns>The job guid.</returns>
        Guid ProcessRequestAsync(Guid jobID, IXimuraRQRSEnvelope data, CommandRSCallback RSCallback,
            CommandProgressCallback ProgressCallback, JobPriority priority);

        /// <summary>
        /// This is the envelope helper, used for creating requests.
        /// </summary>
        IXimuraEnvelopeHelper EnvelopeHelper { get; }
    }
}
