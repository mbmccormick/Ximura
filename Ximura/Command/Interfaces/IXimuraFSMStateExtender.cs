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
using System.Reflection;
using System.Drawing;
using System.Diagnostics;

using Ximura;
using Ximura.Data;
using Ximura.Helper;
using Ximura.Server;
using Ximura.Command;

#endregion // using
namespace Ximura
{
	/// <summary>
	/// Summary description for IXimuraFSMStateExtender.
	/// </summary>
	public interface IXimuraFSMStateMetadataExtender<ST>
        where ST : class, IXimuraFSMState
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
        /// This method returns true if the user has set a next state for this state.
        /// </summary>
        /// <param name="State">The state to check.</param>
        /// <returns>Returns true if a next state has been set.</returns>
        bool HasNextState(string State);
        /// <summary>
        /// This method returns the state identifier for the next state.
        /// </summary>
        /// <param name="State">The state to check.</param>
        /// <returns>Returns the state identifier of the next state if set. 
        /// If not set this method will return null.</returns>
        ST GetNextState(string State);
	}
}
