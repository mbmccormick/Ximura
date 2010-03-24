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
using Ximura.Data;
using Ximura.Helper;
#endregion // using
namespace Ximura.Framework
{
    /// <summary>
    /// EnvelopeKnownTypes are used by IXimuraRQRSEnvelope to specify special folders 
    /// within the Envelope
    /// </summary>
    public enum RQRSEnvTypes
    {
        /// <summary>
        /// This specifies a request
        /// </summary>
        Request,
        /// <summary>
        /// This specifies a response
        /// </summary>
        Response,
        /// <summary>
        /// This is for custom folders
        /// </summary>
        Custom
    }
}
