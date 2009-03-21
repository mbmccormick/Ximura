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
namespace Ximura.Communication
{
    /// <summary>
    /// This meta data container is used for component in the protocol and server containers.
    /// </summary>
    public class CommunicationMetadataContainer
    {
        #region Declarations
        private bool mEnabled;
        private string mIdentifier;
        #endregion // Declarations
        #region Constructors
        /// <summary>
        /// This is the primary constructor.
        /// </summary>
        public CommunicationMetadataContainer()
        {
        }
        #endregion // Constructors

        #region Enabled
        /// <summary>
        /// This property specifies whether the transport is enabled.
        /// </summary>
        public bool Enabled
        {
            get { return mEnabled; }
            set { mEnabled = value; }
        }
        #endregion // Enabled

        #region Identifier
        /// <summary>
        /// This property specifies whether the transport is enabled.
        /// </summary>
        public string Identifier
        {
            get { return mIdentifier; }
            set { mIdentifier = value; }
        }
        #endregion // Enabled
    }
}
