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
using System.Linq;
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
namespace Ximura.Command
{
    [ProvideProperty("StateID", "Ximura.IXimuraFSMState, Ximura")]
    [ProvideProperty("Enabled", "Ximura.IXimuraFSMState, Ximura")]
    [ProvideProperty("CommandID", "Ximura.IXimuraFSMState, Ximura")]
    public class FSMExtenderStateExtender: MetadataExtender<IXimuraFSMState, FSMExtenderStateMetadataContainer>
    {
    }

    /// <summary>
    /// This is the Protocol state meta data container.
    /// </summary>
    public class FSMExtenderStateMetadataContainer
    {
        #region Declarations
        private IXimuraFSMState myState;
        private string myStateID;
        private Dictionary<object, object> extendedProperties = null;

        /// <summary>
        /// This is the enabled status.
        /// </summary>
        public bool Enabled = false;
        #endregion // Declarations
        #region Constructors
        /// <summary>
        /// This is the primary constructor.
        /// </summary>
        public FSMExtenderStateMetadataContainer()
        {
            myStateID = "";
        }
        /// <summary>
        /// This is the primary constructor.
        /// </summary>
        /// <param name="theState">The state object.</param>
        public FSMExtenderStateMetadataContainer(IXimuraFSMState theState)
            : this()
        {
            myState = theState;
        }
        #endregion // Constructors

        #region StateID
        /// <summary>
        /// The is the state ID
        /// </summary>
        public string StateID
        {
            get { return myStateID; }
            set { myStateID = value; }
        }
        #endregion // StateID
        #region State
        /// <summary>
        /// The is the state ID
        /// </summary>
        public IXimuraFSMState State
        {
            get { return myState; }
            set { myState = value; }
        }
        #endregion // State

        #region this[object key]
        /// <summary>
        /// This is the public accessor for the extended properties.
        /// </summary>
        public object this[object key]
        {
            get
            {
                if (extendedProperties == null ||
                    !extendedProperties.ContainsKey(key))
                    return null;

                return extendedProperties[key];
            }
            set
            {
                if (extendedProperties == null)
                    extendedProperties = new Dictionary<object, object>();

                extendedProperties[key] = value;
            }
        }
        #endregion // this[object key]
    }
}
