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

using Ximura;
using Ximura.Helper;
using CH = Ximura.Helper.Common;
using Ximura.Server;
using Ximura.Command;

#endregion // using
namespace Ximura.Communication
{
    /// <summary>
    /// This interface is implemented by the server settings class.
    /// </summary>
    /// <typeparam name="ST">The state type.</typeparam>
    public interface IXimuraServerSettings<ST, RQ, RS, CBRQ, CBRS, CONF, PERF> : IXimuraFSMSettings<ST, CONF, PERF>
        where ST : class, IXimuraServerState<RQ, RS, CBRQ, CBRS, CONF, PERF>
        where RQ : RQServer, new()
        where RS : RSServer, new()
        where CBRQ : RQCallbackServer, new()
        where CBRS : RSCallbackServer, new()
        where CONF : FSMCommandConfiguration, new()
        where PERF : FSMCommandPerformance, new()
    {
        /// <summary>
        /// This method is used to initialize the Server settings object.
        /// </summary>
        /// <param name="authHandlerExtender">The authentication extender.</param>
        /// <param name="baseCommand">The base command information.</param>
        /// <param name="baseApplication">The base application information.</param>
        /// <param name="extender">The state extender.</param>
        /// <param name="poolManager">The pool manager.</param>
        /// <param name="sessionManager">The session mamanger.</param>
        void InitializeSettings(AuthHandlerExtender authHandlerExtender, IXimuraCommand baseCommand,
            IXimuraApplicationDefinition baseApplication, StateExtender<ST> extender,
            IXimuraPoolManager poolManager, IXimuraSessionManager sessionManager);
        /// <summary>
        /// This method resolves the specific authentication handler based on the scheme presented.
        /// </summary>
        /// <param name="scheme">The authentication scheme required.</param>
        /// <returns>Returns the authentication handler or null if the authentication handler cannot be resolved.</returns>
        IXimuraAuthHandler AuthHandlerResolve(string scheme);
        /// <summary>
        /// This method returns the message template for the specific code.
        /// </summary>
        /// <param name="code">The message code.</param>
        /// <returns>The message specified, or the default message is the code cannot be resolved.</returns>
        MessageTemplate ResponseMessageTemplateGet(int code);
    }
}
