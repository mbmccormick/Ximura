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
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Reflection;
using System.Drawing;
using System.Diagnostics;

using Ximura;
using Ximura.Data;
using Ximura.Data;
using CH = Ximura.Common;
using Ximura.Framework;

using Ximura.Framework;

#endregion // using
namespace Ximura.Framework
{
    public interface IXimuraFSMSettings<ST, CONF, PERF> : IXimuraFSMSettingsBase
        where ST : class,IXimuraFSMState
        where CONF : FSMCommandConfiguration, new()
        where PERF : FSMCommandPerformance, new()
    {
        /// <summary>
        /// The command configuration.
        /// </summary>
        CONF Configuration { get; }
        /// <summary>
        /// The FSM performance counter.
        /// </summary>
        PERF Performance { get; }
        /// <summary>
        /// This method returns a collection of active states.
        /// </summary>
        IEnumerable<ST> StateCollection();
        /// <summary>
        /// This method returns the initial state, or null if no initial state has been set.
        /// </summary>
        /// <returns>The state requested or null.</returns>
        ST GetInitialState();
        /// <summary>
        /// This method returns the state object based on the name.
        /// </summary>
        /// <param name="State">The state name.</param>
        /// <returns>Returns the state object or null if the name cannot be resolved.</returns>
        ST GetState(string State);
        /// <summary>
        /// This method returns the state identifier for the state object.
        /// </summary>
        /// <param name="CurrentState">The current that you wish to resolve.</param>
        /// <returns>Return the name of the state.</returns>
        string GetStateName(ST CurrentState);
        /// <summary>
        /// This method is used to initialize the FSM settings object.
        /// </summary>
        /// <param name="baseCommand">The base command information.</param>
        /// <param name="baseApplication">The base application information.</param>
        /// <param name="extender">The state extender.</param>
        /// <param name="poolManager">The pool manager.</param>
        /// <param name="sessionManager">The session mamanger.</param>
        void InitializeSettings(IXimuraCommand baseCommand, IXimuraApplicationDefinition baseApplication,
            StateExtender<ST> extender, IXimuraPoolManager poolManager, IXimuraEnvelopeHelper envelopeHelper,
            IXimuraSessionManager sessionManager, IXimuraSession processSession, CONF config, PERF perf);

        IXimuraSession ProcessSession { get;}
    }
    /// <summary>
    /// This is the settings base interface.
    /// </summary>
    public interface IXimuraFSMSettingsBase : IXimuraApplicationDefinition, IXimuraCommand
    {
        //void UpdateSettings(IXimuraFSMConfigSH FSMSettings, ICDSSettingsService CDSSettings);
        /// <summary>
        /// The pool manager.
        /// </summary>
        IXimuraPoolManager PoolManager { get;}
        /// <summary>
        /// The session manager.
        /// </summary>
        IXimuraSessionManager SessionManager { get;}
        /// <summary>
        /// This is the default domian user for creating context sessions.
        /// </summary>
        string DomainDefault { get;}
        /// <summary>
        /// This is the envelope helper.
        /// </summary>
        IXimuraEnvelopeHelper EnvelopeHelper { get; }
    }
}
