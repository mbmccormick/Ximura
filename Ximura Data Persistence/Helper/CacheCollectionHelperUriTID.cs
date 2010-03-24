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
using System.Collections;
using System.Configuration;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Runtime.Serialization;
using System.IO;

using Ximura;
using Ximura.Data;
using Ximura.Data.Serialization;
using Ximura.Helper;
using CH = Ximura.Helper.Common;
using AH = Ximura.Helper.AttributeHelper;

using Ximura.Framework;


using Ximura.Framework;
#endregion // using
namespace Ximura.Data
{
    /// <summary>
    /// This helper class can be used to hold a set of Uri - Cid/Vid references.
    /// </summary>
    public class CacheCollectionHelperUriTID
    {
        #region Declarations
        private Dictionary<Uri, KeyValuePair<Guid, Guid>> mCollectionUriLookup;
        private Dictionary<Guid, Uri> mCollectionCIDLookup;
        private object syncLock = new object();
        #endregion // Declarations

        #region Constructor
        /// <summary>
        /// This is the default constrcutor for the Uri collection cache.
        /// </summary>
        public CacheCollectionHelperUriTID()
        {
            mCollectionUriLookup = new Dictionary<Uri, KeyValuePair<Guid, Guid>>();
            mCollectionCIDLookup = new Dictionary<Guid, Uri>();
        }
        #endregion // Constructor

        #region ResolveReference(Uri id, out Guid cid, out Guid vid)
        /// <summary>
        /// This method resolves the Uri and returns the cid and vid. If the
        /// cid and vid are no already in the system, the collection will create 
        /// a new key pair.
        /// </summary>
        /// <param name="id">The uri to resolve.</param>
        /// <param name="cid">The cid for the uri.</param>
        /// <param name="vid">The vid for the uri.</param>
        /// <returns>Returns true if a new record is created.</returns>
        public virtual bool ResolveReference(Uri id, out Guid cid, out Guid vid)
        {
            bool newRecord = false;
            lock (syncLock)
            {
                KeyValuePair<Guid, Guid> data;

                if (!mCollectionUriLookup.ContainsKey(id))
                {
                    data = new KeyValuePair<Guid, Guid>(Guid.NewGuid(), Guid.NewGuid());
                    mCollectionUriLookup.Add(id, data);
                    mCollectionCIDLookup.Add(data.Key, id);
                    newRecord = true;
                }
                else
                    data = mCollectionUriLookup[id];

                cid = data.Key;
                vid = data.Value;
            }
            return newRecord;
        }
        #endregion // ResolveReference(Uri id, out Guid cid, out Guid vid)
        #region ResolveReference(Guid cid, out Uri id)
        public virtual bool ResolveReference(Guid cid, out Uri id, out Guid? vid)
        {
            lock (syncLock)
            {
                bool response = ResolveReferenceInternal(cid, out id);

                if (response)
                {
                    vid = mCollectionUriLookup[id].Value;
                }
                else
                    vid = null;

                return response;
            }
        }

        protected virtual bool ResolveReferenceInternal(Guid cid, out Uri id)
        {
            id = null;
            if (mCollectionCIDLookup.ContainsKey(cid))
            {
                id = mCollectionCIDLookup[cid];
                return true;
            }

            return false;
        }
        /// <summary>
        /// This method returns the Uri data for the CID.
        /// </summary>
        /// <param name="cid">The Cid to resolve.</param>
        /// <param name="id">The resolved Uri or null if the cid cannot be resolved.</param>
        public virtual bool ResolveReference(Guid cid, out Uri id)
        {
            lock (syncLock)
            {
                return ResolveReferenceInternal(cid, out id);
            }
        }
        #endregion // ResolveReference(Guid cid, out Uri id)

        #region UpdateReference(Uri id, Guid vid)
        /// <summary>
        /// This method updates the reference value.
        /// </summary>
        /// <param name="id">The path.</param>
        /// <param name="vid">The version update to update to.</param>
        /// <returns>Returns true if updated.</returns>
        public virtual bool UpdateReference(Uri id, Guid vid)
        {
            lock (syncLock)
            {
                if (!mCollectionUriLookup.ContainsKey(id))
                    return false;

                KeyValuePair<Guid, Guid> data= mCollectionUriLookup[id];
                if (data.Value != vid)
                    mCollectionUriLookup[id] = new KeyValuePair<Guid, Guid>(data.Key, vid);
                return true;
            }
        }
        #endregion // UpdateReference(Uri id, Guid vid)

    }
}
