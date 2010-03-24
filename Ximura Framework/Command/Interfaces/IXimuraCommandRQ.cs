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

using Ximura;
using Ximura.Data;


using Ximura.Framework;
using Ximura.Framework;

#endregion // using
namespace Ximura.Framework
{
    /// <summary>
    /// This is the default command processing interface.
    /// </summary>
    public interface IXimuraCommandRQ : IXimuraCommand
    {
        /// <summary>
        /// This boolean property identifies whether the command will support callback.s
        /// </summary>
        bool SupportsCallbacks { get; }
        /// <summary>
        /// This is the default entry point for the job request.
        /// </summary>
        /// <param name="job">The security manager job.</param>
        void ProcessRequestSCM(SecurityManagerJob job);
        /// <summary>
        /// This is the command request type.
        /// </summary>
        Type RequestType { get; }
        /// <summary>
        /// This is the command response type.
        /// </summary>
        Type ResponseType { get; }

        /// <summary>
        /// This is the callback request type.
        /// </summary>
        Type CallbackRequestType { get; }
        /// <summary>
        /// This is the callback request type.
        /// </summary>
        Type CallbackResponseType { get; }

        /// <summary>
        /// This is the envelope contract type. 
        /// </summary>
        Type EnvelopeContractType { get; }
        /// <summary>
        /// This is the envelope callback contract type.
        /// </summary>
        Type EnvelopeCallbackContractType { get; }
    }
}
