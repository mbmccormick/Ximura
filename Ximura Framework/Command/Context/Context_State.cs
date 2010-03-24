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
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;

using Ximura;
using Ximura.Data;
using CH = Ximura.Common;
using Ximura.Framework;

#endregion // using
namespace Ximura.Framework
{
    public partial class Context<ST, SET, CONF, PERF>
    {
        #region Declarations
        /// <summary>
        /// This is the current state.
        /// </summary>
        protected ST mState;
        #endregion // Declarations

        #region CheckChangeState(string stateName)
        /// <summary>
        /// This method checks and then changes the state. If the state is not valid an exception is thrown.
        /// </summary>
        /// <param name="stateName">The state name to check.</param>
        /// <exception cref="Ximura.InvalidStateNameFSMException">This exception is thrown if the stateName cannot be resolved.</exception>
        public virtual void CheckChangeState(string stateName)
        {
            if (!CheckState(stateName))
                throw new InvalidStateNameFSMException(@"The state name """ + stateName + @""" is not recognised.");
            ChangeState(stateName);
        }
        #endregion
        #region ChangeState()
        /// <summary>
        /// This method changes the states to the initial state.
        /// </summary>
        public virtual void ChangeState()
        {
            ChangeState(null);
        }
        /// <summary>
        /// This method changes the state.
        /// </summary>
        /// <param name="stateName">The state. If this is set to null the initial state will be set.</param>
        public virtual void ChangeState(string stateName)
        {
            if (mContextSettings == null)
                return;
            if (stateName == null)
                this.CurrentState = mContextSettings.GetInitialState();
            else
                this.CurrentState = mContextSettings.GetState(stateName);
        }
        /// <summary>
        /// This method returns true if the state exists in the FSM.
        /// </summary>
        /// <param name="stateName">The state.</param>
        public virtual bool CheckState(string stateName)
        {
            if (stateName == null)
                return false;

            if (mContextSettings == null)
                return false;

            return mContextSettings.GetState(stateName) != null;
        }
        #endregion // ChangeState
        #region CurrentState
        /// <summary>
        /// This is the current purchase state.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual ST CurrentState
        {
            get
            {
                return mState;
            }
            set
            {
#if DEBUG
                if (value == null)
                    Debug.WriteLine("State set to null.");
#endif
                mState = value;
            }
        }
        #endregion // CurrentState
        #region StateCollection
        /// <summary>
        /// This method returs a collection of states.
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<ST> StateCollection()
        {
            return ContextSettings.StateCollection();
        }
        #endregion // StateCollection()
    }
}
