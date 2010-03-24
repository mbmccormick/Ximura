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
using System.Data;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Collections;
using System.Collections.Generic;

using Ximura;
using Ximura.Framework;
using Ximura.Framework;
using Ximura.Data;

#endregion // using
namespace Ximura
{
    /// <summary>
    /// This interface implements the public methods and properties for the IXimuraRQRSEnvelope
    /// and contract definitions.
    /// </summary>
    public interface IXimuraRQRSEnvelope : IXimuraPoolReturnable, IXimuraPoolableObjectDeserializable
    {
        /// <summary>
        /// The destination address for the envelope/contract.
        /// </summary>
        EnvelopeAddress DestinationAddress{get;set;}
        /// <summary>
        /// The originator/owner of the request.
        /// </summary>
        Guid Sender { get;set;}
        /// <summary>
        /// The internal reference for the sender.
        /// </summary>
        Guid? SenderReference { get;set;}
        /// <summary>
        /// This is the job user ID.
        /// </summary>
        Guid JobUserID{get;set;}
        /// <summary>
        /// This is the job user plain text reference ID.
        /// </summary>
        string JobUserReferenceID {get;set;}
        /// <summary>
        /// This is the unique job security reference ID.
        /// </summary>
        byte[] JobSecurityIdentifier { get;set;}

        /// <summary>
		/// The Request property
		/// </summary>
        RQRSFolder Request { get;}//set;}
        /// <summary>
		/// The Response property
		/// </summary>
        RQRSFolder Response { get;}//set;}
        /// <summary>
        /// This shortcut is used to prepare the response status and substatus.
        /// </summary>
        /// <param name="status">The status.</param>
        /// <param name="subStatus">The substatus.</param>
        void PrepareResponse(string status, string subStatus);
    }
}
