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
using System.Collections;
using System.Diagnostics;
using System.Data;
using System.Reflection;
using System.ComponentModel;
using System.Collections.Generic;

using Ximura;
using Ximura.Data;
using CH = Ximura.Helper.Common;
using Ximura.Helper;
using Ximura.Framework;


using Ximura.Framework;
#endregion // using
namespace Ximura.Communication
{
    /// <summary>
    /// This class is used to identify a protocol connection.
    /// </summary>
    public class ProtocolConnectionIdentifiers : PoolableReturnableObjectBase
    {
        #region Declarations
        Guid? mTransportID;
        Guid? mProtocolCommandID;
        TransportConnectionType mProtocolTransportType;
        Uri mUriRemote;
        Uri mUriLocal;
        /// <summary>
        /// This is the syncronization object used for ensuring that only one value is changed at a time.
        /// </summary>
        private object syncObj = new object();
        #endregion // Declarations
        #region Constructor
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        public ProtocolConnectionIdentifiers()
            : base()
        {
        }
        #endregion // Constructor

        #region Reset()
        /// <summary>
        /// This method resets the connection to its default parameters.
        /// </summary>
        public override void Reset()
        {
            lock (syncObj)
            {
                mTransportID = null;
                mProtocolCommandID = null;
                mProtocolTransportType = TransportConnectionType.Undefined;
                mUriRemote = null;
                mUriLocal = null;
            }
            base.Reset();
        }
        #endregion // Reset()

        public TransportConnectionType ProtocolTransportType
        {
            get { lock (syncObj) { return mProtocolTransportType; } }
            set { lock (syncObj) { mProtocolTransportType = value; } }
        }

        public Guid? ProtocolContextID
        {
            get { lock (syncObj) { return mTransportID; } }
            set { lock (syncObj) { mTransportID = value; } }
        }

        public Guid? ProtocolCommandID
        {
            get { lock (syncObj) { return mProtocolCommandID; } }
            set { lock (syncObj) { mProtocolCommandID = value; } }
        }

        public Uri UriRemote
        {
            get { lock (syncObj) { return mUriRemote; } }
            set { lock (syncObj) { mUriRemote = value; } }
        }

        public Uri UriLocal
        {
            get { lock (syncObj) { return mUriLocal; } }
            set { lock (syncObj) { mUriLocal = value; } }
        }
    }

}
