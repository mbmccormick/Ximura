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
        where STAT : class, IXimuraFSMState
    {
        List<KeyValuePair<string, STAT>> mExtensionStates;
        #region Constructor
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        public FSMCommandContainer():this(null)
        {
        }

        public FSMCommandContainer(IEnumerable<KeyValuePair<string, STAT>> states)
        {
            if (states != null)
                mExtensionStates = states.ToList();
            else
                mExtensionStates = new List<KeyValuePair<string, STAT>>();
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
            //Connect the components to the messaging architecture.
            ConnectComponents();

            mExtensionStates.AddRange(ExtensionStates);

            mCommand.ExternalStatesAllow = mExtensionStates.Count > 0;

            IXimuraStateExtenderService stEx = GetService<IXimuraStateExtenderService>();

            IXimuraStateExtender<STAT> ex = (IXimuraStateExtender<STAT>)stEx.Resolve(mCommand.CommandID, typeof(STAT));

            mExtensionStates.ForEach(a => ex.SetStateID(a.Value, a.Key));

            CommandsStart();

            CommandExtender.CommandsNotify(typeof(XimuraServiceStatus), XimuraServiceStatus.Started);
        }


        protected virtual IEnumerable<KeyValuePair<string,STAT>> ExtensionStates
        {
            get
            {
                yield break;
            }
        }
    }
}
