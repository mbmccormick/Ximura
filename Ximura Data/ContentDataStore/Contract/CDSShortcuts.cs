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
        public static readonly Guid CDSCommandID = new Guid(ContentDataStoreShortcuts.ID);
        /// <summary>
        /// This is the command ID
        /// </summary>
        public static Guid ID
        {
            get { return CDSCommandID; }
        }
        /// <summary>
        /// The Save command
        /// </summary>
        public static EnvelopeAddress Create
        {
            get { return new EnvelopeAddress(CDSCommandID, CDSStateAction.Create); }
        }
        ///// <summary>
        ///// The Load command
        ///// </summary>
        //public static EnvelopeAddress ReadByReference
        //{
        //    get{return new EnvelopeAddress(guidCommand,PMCapabilities.ReadByReference);}
        //}
        /// <summary>
        /// The Load command
        /// </summary>
        public static EnvelopeAddress Read
        {
            get { return new EnvelopeAddress(CDSCommandID, CDSStateAction.Read); }
        }
        /// <summary>
        /// The Load command
        /// </summary>
        public static EnvelopeAddress Update
        {
            get { return new EnvelopeAddress(CDSCommandID, CDSStateAction.Update); }
        }
        /// <summary>
        /// The Update command
        /// </summary>
        public static EnvelopeAddress Delete
        {
            get { return new EnvelopeAddress(CDSCommandID, CDSStateAction.Delete); }
        }
        /// <summary>
        /// The VersionCheck command
        /// </summary>
        public static EnvelopeAddress VersionCheck
        {
            get { return new EnvelopeAddress(CDSCommandID, CDSStateAction.VersionCheck); }
        }
        /// <summary>
        /// The custom command
        /// </summary>
        public static EnvelopeAddress Custom
        {
            get { return new EnvelopeAddress(CDSCommandID, CDSStateAction.Custom); }
        }
    }

}