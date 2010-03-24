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
using RH = Ximura.Helper.Reflection;
#endregion // using
namespace Ximura.Framework
{
    /// <summary>
    /// This interface is used to provide access the Envelope pool for the application.
    /// </summary>
    public interface IXimuraEnvelopeHelper
    {
        RQRSContract<RQ, RS> Get<RQ, RS>()
            where RQ : RQRSFolder, new()
            where RS : RQRSFolder, new();

        IXimuraRQRSEnvelope Get(EnvelopeAddress address);
        IXimuraRQRSEnvelope Get(EnvelopeAddress address, Guid? requestID);
        IXimuraRQRSEnvelope Get(EnvelopeAddress address, bool setAddress, Guid? requestID);
        IXimuraRQRSEnvelope Get(Guid commandID);
        IXimuraRQRSEnvelope Get(Type objectType);

        IXimuraRQRSEnvelope GetCallback(EnvelopeAddress address);
        IXimuraRQRSEnvelope GetCallback(EnvelopeAddress address, Guid? requestID);
        IXimuraRQRSEnvelope GetCallback(EnvelopeAddress address, bool setAddress, Guid? requestID);
        IXimuraRQRSEnvelope GetCallback(Guid commandID);

        Type ResolveType(Guid commandID);
        Type ResolveTypeCallback(Guid commandID);
    }
}
