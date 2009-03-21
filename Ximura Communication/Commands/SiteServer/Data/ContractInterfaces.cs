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
using System.Threading;
using System.Timers;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Reflection;
using System.Drawing;
using System.Diagnostics;
using System.Collections.Generic;
using System.Runtime.Serialization;

using Ximura;
using Ximura.Data;
using Ximura.Helper;
using CH = Ximura.Helper.Common;
using Ximura.Server;
using Ximura.Command;

#endregion // using
namespace Ximura.Communication
{
    public interface IXimuraProtocolBase
    {
        Guid? ProtocolContextID { get;set; }
        Guid? ServerContextID { get;set; }
    }

    public interface IXimuraProtocolResponseBase : IXimuraProtocolBase
    {
        bool CloseNotify { get;set;}
    }

    public interface IXimuraProtocolCloseNotificationRequest : IXimuraProtocolBase
    {
        TransportCloseType CloseType { get;set;}
    }

    public interface IXimuraProtocolCloseNotificationResponse : IXimuraProtocolResponseBase
    {
    }

    public interface IXimuraProtocolConnectionRequest : IXimuraProtocolBase
    {
        Uri RemoteUri { get;set;}
        Uri LocalUri { get;set;}
        TransportConnectionType ConnectionType { get;set;}
    }

    public interface IXimuraProtocolConnectionResponse : IXimuraProtocolResponseBase
    {
        IXimuraMessageStream MessageResponse { get;set; }
        IXimuraMessageStream MessageRequest { get;set; }
        Type MessageRequestType { get;set; }
        long MaxLength { get;set;}
    }

    public interface IXimuraProtocolMessageReceived : IXimuraProtocolBase
    {
        IXimuraMessageStream Message { get;set;}
    }

    public interface IXimuraProtocolMessageResponse : IXimuraProtocolConnectionResponse
    {
    }
}
