﻿#region Copyright
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
using System.Linq;
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

using Ximura.Framework;
using Ximura.Framework;

#endregion // using
namespace Ximura.Framework
{
    /// <summary>
    /// The state extender service object is the intermediary between a Finite State Machine and the FSM extender commands. 
    /// This object allows an extender to register external states with the FSM.
    /// </summary>
    public class StateExtenderService: IXimuraStateExtenderService
    {
        #region Declarations
        Dictionary<KeyValuePair<Guid, Type>, IXimuraStateExtender> mRegisteredExtendees;
        #endregion // Declarations
        #region Constructor
        /// <summary>
        /// This is the default constructor for the StateExtenderService.
        /// </summary>
        public StateExtenderService()
        {
            mRegisteredExtendees = new Dictionary<KeyValuePair<Guid, Type>, IXimuraStateExtender>();
        }
        #endregion // Constructor

        #region Register(Guid CommandID, Type stateType, IXimuraStateExtender mStateExtender)
        /// <summary>
        /// This method registers a new extender for a particular class.
        /// </summary>
        /// <param name="CommandID">The command ID.</param>
        /// <param name="stateType">The state stype.</param>
        /// <param name="mStateExtender">The extender class.</param>
        public void Register(Guid CommandID, Type stateType, IXimuraStateExtender mStateExtender)
        {
            KeyValuePair<Guid, Type> key = new KeyValuePair<Guid, Type>(CommandID, stateType);
            mRegisteredExtendees.Add(key, mStateExtender);
        }
        #endregion 
        #region Unregister(Guid CommandID, Type stateType)
        /// <summary>
        /// This method removes the extender for the particular command and state type.
        /// </summary>
        /// <param name="CommandID">The command ID.</param>
        /// <param name="stateType">The state stype.</param>
        public void Unregister(Guid CommandID, Type stateType)
        {
            KeyValuePair<Guid, Type> key = new KeyValuePair<Guid, Type>(CommandID, stateType);
            if (mRegisteredExtendees.ContainsKey(key))
                mRegisteredExtendees.Remove(key);

        }
        #endregion 
    
        #region Resolve(Guid CommandID, Type stateType)
        /// <summary>
        /// This method returns the extender to allow a remote service to register new states.
        /// </summary>
        /// <param name="CommandID">The command ID.</param>
        /// <param name="stateType">The state type.</param>
        /// <returns>Returns the state extender.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">This exception will be thrown if the extender cannot be found.</exception>
        public IXimuraStateExtender Resolve(Guid CommandID, Type stateType)
        {
            KeyValuePair<Guid, Type> key = new KeyValuePair<Guid, Type>(CommandID, stateType);

            if (mRegisteredExtendees.ContainsKey(key))
                return mRegisteredExtendees[key];

            throw new ArgumentOutOfRangeException();
        }
        #endregion
    }
}
