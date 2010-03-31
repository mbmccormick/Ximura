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
using CH = Ximura.Common;
using Ximura.Framework;
#endregion // using
namespace Ximura.Framework
{
    /// <summary>
    /// This is the standard container for hosting a Application command that has a standard constructor.
    /// </summary>
    /// <typeparam name="COMM">The FSM command type.</typeparam>
    /// <typeparam name="STAT">The FSM command state type.</typeparam>
    public class FSMCommandContainer<COMM, STAT> : CommandContainer<COMM>
        where COMM : class, IXimuraFSM, new()
        where STAT : IXimuraFSMState
    {
        #region Constructor
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        public FSMCommandContainer()
        {
        }
        #endregion
        #region CommandCreate()
        /// <summary>
        /// This override creates the command.
        /// </summary>
        protected override void CommandCreate()
        {
            mCommand = new COMM();
        }
        #endregion

        protected override void InternalStart()
        {
            List<STAT> extensionStates = ExtensionStates.ToList();
            mCommand.ExternalStatesAllow = true;// extensionStates.Count > 0;
            base.InternalStart();

            IXimuraStateExtenderService stEx = GetService<IXimuraStateExtenderService>();
        }


        protected virtual IEnumerable<STAT> ExtensionStates
        {
            get
            {
                yield break;
            }
        }
    }
}
