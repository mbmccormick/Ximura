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
using System.ComponentModel.Design.Serialization;
using System.Reflection;
using System.Drawing;
using System.Diagnostics;
using System.Collections.Generic;

using Ximura;
using Ximura.Data;
using Ximura.Helper;
using CH = Ximura.Helper.Common;
using Ximura.Server;

using Ximura.Command;

#endregion // using
namespace Ximura.Communication
{
    /// <summary>
    /// This structure is used to hold protocol/server information.
    /// </summary>
    public struct ConnectionInfo
    {
        Guid mConnID;
        Guid mServerID;
        Uri mUri;

        public ConnectionInfo(Guid connID, Guid serverID)
        {
            mConnID = connID;
            mServerID = serverID;
            mUri = null;
        }

        public ConnectionInfo(Guid connID, Guid serverID, Uri uri)
        {
            mConnID = connID;
            mServerID = serverID;
            mUri = uri;
        }

        /// <summary>
        /// This is the command ID of the server.
        /// </summary>
        public Guid ServerID
        {
            get
            {
                return mServerID;
            }
        }
        /// <summary>
        /// This shortcut method allows an envelope address to be easily created for
        /// the sub-command.
        /// </summary>
        /// <param name="subCommand">The subcommand object.</param>
        /// <returns>Returns an envelope address. If the server ID is empty, then this 
        /// method will return a null destination.</returns>
        public EnvelopeAddress ServerAddress(object subCommand)
        {
            if (mServerID == Guid.Empty)
                return EnvelopeAddress.NullDestination;

            return new EnvelopeAddress(mServerID, subCommand);
        }
        /// <summary>
        /// This is the connection ID shared between the protocol and the server.
        /// </summary>
        public Guid ConnectionID
        {
            get
            {
                return mConnID;
            }
        }
    }
}
