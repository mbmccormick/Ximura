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
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;

using Ximura;
using Ximura.Framework;
using Ximura.Framework;

#endregion // using
namespace Ximura.Framework
{
    /// <summary>
    /// This interface is a bridge between the command and the context.
    /// </summary>
    public interface IXimuraConnectionToFSM<CNTX, ST, SET, CONF, PERF> : IXimuraCommand
        where CNTX : class, IXimuraFSMContext, new()
        where ST : class,IXimuraFSMState
        where SET : class, IXimuraFSMSettings<ST, CONF, PERF>
        where CONF : FSMCommandConfiguration, new()
        where PERF : FSMCommandPerformance, new()
    {
        /// <summary>
        /// This method returns the initial state.
        /// </summary>
        /// <param name="State">The type parameter for the state required.</param>
        /// <returns>The state.</returns>
        ST GetState(string State);
        /// <summary>
        /// This is the initial state.
        /// </summary>
        /// <returns>The initial state. If there is no initial state set 
        /// this will return null;</returns>
        ST GetInitialState();
        /// <summary>
        /// This method returns the string identifier for the specified state.
        /// </summary>
        /// <param name="CurrentState">The state to identify.</param>
        /// <returns>Returns a string identifying the state.</returns>
        string GetStateName(ST CurrentState);
        /// <summary>
        /// This method allows a context to signal to the FSM that it has completed its tasks.
        /// </summary>
        /// <param name="signaller">The context object which is signalling that it is now in a completed state.</param>
        void SignalCompletion(CNTX signaller);
        /// <summary>
        /// This method is used to create a FSM callback for the context.
        /// </summary>
        /// <param name="context">The context to process.</param>
        /// <returns>Returns a new completion job.</returns>
        CompletionJob GetFSMCompletionJob(CNTX context);
    }
}