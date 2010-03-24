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
using System.Data;
using System.Data.SqlClient;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;

using Ximura;
using Ximura.Data;

using CH = Ximura.Common;
using RH = Ximura.Reflection;
using Ximura.Framework;


using Ximura.Framework;
#endregion // using
namespace Ximura.Data
{
    /// <summary>
    /// This class provides shorcuts for the ContentDataStore
    /// </summary>
    public static class ContentDataStoreShortcuts
    {
        #region Declarations
        /// <summary>
        /// The command ID.
        /// </summary>
        public const string ID = "FE21CBF6-2CDC-4549-9F13-49385CAE8DDA";
        /// <summary>
        /// The command name.
        /// </summary>
        public const string Name = "ContentDataStore";

        static EnvelopeAddress sAddressRead;
        static EnvelopeAddress sAddressReadByReference;
        static EnvelopeAddress sAddressUpdate;
        static EnvelopeAddress sAddressCreate;
        static EnvelopeAddress sAddressDelete;
        static EnvelopeAddress sAddressDeleteByReference;
        static EnvelopeAddress sAddressVersionCheck;
        static EnvelopeAddress sAddressCustom;

        static Guid sID;
        static string sName;
        static Type sType;
        #endregion

        #region Constructor
        static ContentDataStoreShortcuts()
        {
            sID = new Guid(ID);
            sType = typeof(RQRSContract<RQRSFolder, RQRSFolder>);

            sAddressRead = new EnvelopeAddress(sID, CDSStateAction.Read);
            sAddressUpdate = new EnvelopeAddress(sID, CDSStateAction.Update);
            sAddressCreate = new EnvelopeAddress(sID, CDSStateAction.Create);
            sAddressDelete = new EnvelopeAddress(sID, CDSStateAction.Delete);
            sAddressVersionCheck = new EnvelopeAddress(sID, CDSStateAction.VersionCheck);
            sAddressCustom = new EnvelopeAddress(sID, CDSStateAction.Custom);
        }
        #endregion

        /// <summary>
        /// The read envelope.
        /// </summary>
        public static EnvelopeAddress Read { get { return sAddressRead; } }
        /// <summary>
        /// The update envelope.
        /// </summary>
        public static EnvelopeAddress Update { get { return sAddressUpdate; } }
        /// <summary>
        /// The create envelope.
        /// </summary>
        public static EnvelopeAddress Create { get { return sAddressCreate; } }
        /// <summary>
        /// The delete envelope.
        /// </summary>
        public static EnvelopeAddress Delete { get { return sAddressDelete; } }
        /// <summary>
        /// The version check envelope.
        /// </summary>
        public static EnvelopeAddress VersionCheck { get { return sAddressVersionCheck; } }
        /// <summary>
        /// The custom method envelope.
        /// </summary>
        public static EnvelopeAddress Custom { get { return sAddressCustom; } }
        /// <summary>
        /// The CDS command ID.
        /// </summary>
        public static Guid CommandID { get { return sID; } }
    }
}
