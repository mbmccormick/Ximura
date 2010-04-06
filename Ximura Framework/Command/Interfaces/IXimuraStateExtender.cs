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
    #region IXimuraStateExtender
    /// <summary>
    /// This interface is used to extend a finite machine.
    /// </summary>
    public interface IXimuraStateExtender
    {
        /// <summary>
        /// This property is used to return the state ID.
        /// </summary>
        /// <param name="state">The state.</param>
        /// <returns>The ID of the state.</returns>
        string GetStateID(IXimuraFSMState state);

        /// <summary>
		/// This property is used to set the state id
		/// </summary>
		/// <param name="state">The state object.</param>
		/// <param name="value">The state ID.</param>
        void SetStateID(IXimuraFSMState state, string value);

        /// <summary>
		/// This property is used to return the enabled state.
		/// </summary>
		/// <param name="state">The state.</param>
		/// <returns>The enabled boolean value of the state.</returns>
        bool GetEnabled(IXimuraFSMState state);

        /// <summary>
		/// This property is used to set the enabled state.
		/// </summary>
		/// <param name="state">The state object.</param>
		/// <param name="value">The state enabled value.</param>
        void SetEnabled(IXimuraFSMState state, bool value);
    }
    #endregion 
    #region IXimuraStateExtender<ST> 
    /// <summary>
    /// This is the generic extender for the finite state machine.
    /// </summary>
    /// <typeparam name="ST">The finite state machine state.</typeparam>
    public interface IXimuraStateExtender<ST> : IXimuraStateExtender
        where ST : class, IXimuraFSMState
    {

    }
    #endregion 
}
