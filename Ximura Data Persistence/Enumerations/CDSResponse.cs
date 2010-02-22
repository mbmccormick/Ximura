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
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Security;
using System.Security.Cryptography;
using System.Globalization;
using System.Runtime.Serialization;

using Ximura;
using Ximura.Helper;
using CH = Ximura.Helper.Common;
using RH = Ximura.Helper.Reflection;
using AH = Ximura.Helper.AttributeHelper;
using Ximura.Data;
using Ximura.Server;
using Ximura.Command;
#endregion // using
namespace Ximura.Persistence
{
    /// <summary>
    /// This enumeration contains the standard response codes from the Content Data Store (CDS)
    /// and its associated persistence agents.
    /// </summary>
    public enum CDSResponse : int
    {
        /// <summary>
        /// The request can continue. This is primarily used by resolve reference.
        /// </summary>
        Continue = 100,

        /// <summary>
        /// The request was processed successfully.
        /// </summary>
        OK = 200,
        /// <summary>
        /// The entity was created/updated or deleted successfully.
        /// </summary>
        Created = 201,

        /// <summary>
        /// The request parameters or the request entities were invalid or badly formed.
        /// </summary>
        BadRequest = 400,
        /// <summary>
        /// The request parameters could not be resolved to an entity.
        /// </summary>
        NotFound = 404,
        /// <summary>
        /// The request parameters or the entity was missing required information.
        /// </summary>
        NotAcceptable = 406,
        /// <summary>
        /// This error is usually returned when a reference ID conflicts with another entity.
        /// </summary>
        Conflict = 409,
        /// <summary>
        /// The version ID supplied was invalid.
        /// </summary>
        VersionIDInvalid = 412,
        /// <summary>
        /// The action could not be performed because the entity is locked and the correct lock ID was not presented.
        /// </summary>
        EntityLocked = 428,

        /// <summary>
        /// The CDS has encountered an unexpected error processing the request.
        /// </summary>
        SystemError = 500,
        /// <summary>
        /// The requested action for the requested entity type is not supported.
        /// </summary>
        NotImplemented = 501,
        /// <summary>
        /// The CDS has not started.
        /// </summary>
        CDSNotStarted = 503,

        /// <summary>
        /// The response code was not recognised.
        /// </summary>
        ResponseCodeUnknown = 0,
        /// <summary>
        /// The response code was null or an empty string.
        /// </summary>
        ResponseCodeNotSet = -1
    }
}
