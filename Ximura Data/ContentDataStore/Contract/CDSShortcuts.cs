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

using Ximura;
using Ximura.Framework;
using Ximura.Framework;
using Ximura.Data;
using Ximura.Data;
#endregion // using
namespace Ximura.Data
{
    /// <summary>
    /// The Content Data Store shortcut collection
    /// </summary>
    public struct scContentDataStore
    {
        #region CDSCommandID
        public static readonly Guid CDSCommandID = new Guid(ContentDataStoreShortcuts.ID);
        #endregion // CDSCommandID
        #region ID
        /// <summary>
        /// This is the command ID
        /// </summary>
        public static Guid ID
        {
            get { return CDSCommandID; }
        }
        #endregion // ID

        #region Create
        /// <summary>
        /// The Save command
        /// </summary>
        public static EnvelopeAddress Create
        {
            get { return new EnvelopeAddress(CDSCommandID, CDSStateAction.Create); }
        }
        #endregion // Create

        ///// <summary>
        ///// The Load command
        ///// </summary>
        //public static EnvelopeAddress ReadByReference
        //{
        //    get{return new EnvelopeAddress(guidCommand,PMCapabilities.ReadByReference);}
        //}

        #region Read
        /// <summary>
        /// The Load command
        /// </summary>
        public static EnvelopeAddress Read
        {
            get { return new EnvelopeAddress(CDSCommandID, CDSStateAction.Read); }
        }
        #endregion // Read
        #region Update
        /// <summary>
        /// The Load command
        /// </summary>
        public static EnvelopeAddress Update
        {
            get { return new EnvelopeAddress(CDSCommandID, CDSStateAction.Update); }
        }
        #endregion // Update
        #region Delete
        /// <summary>
        /// The Update command
        /// </summary>
        public static EnvelopeAddress Delete
        {
            get { return new EnvelopeAddress(CDSCommandID, CDSStateAction.Delete); }
        }
        #endregion // Delete
        #region VersionCheck
        /// <summary>
        /// The VersionCheck command
        /// </summary>
        public static EnvelopeAddress VersionCheck
        {
            get { return new EnvelopeAddress(CDSCommandID, CDSStateAction.VersionCheck); }
        }
        #endregion // VersionCheck
        #region Custom
        /// <summary>
        /// The custom command
        /// </summary>
        public static EnvelopeAddress Custom
        {
            get { return new EnvelopeAddress(CDSCommandID, CDSStateAction.Custom); }
        }
        #endregion // Custom
    }
}