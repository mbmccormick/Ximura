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
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;

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
    /// The resource persistence manager is used to extract resources embedded in the application assemblies.
    /// </summary>
    /// <typeparam name="CONT">The content type to handle.</typeparam>
    /// <typeparam name="DCONT">The base content type to scan.</typeparam>
    [CDSStateActionPermit(CDSAction.Read)]
    [CDSStateActionPermit(CDSAction.VersionCheck)]
    [CDSStateActionPermit(CDSAction.ResolveReference)]
    public class ResourceBasePersistenceManager<CONT, DCONT, CONF> : CollectionBasePersistenceManager<CONT, DCONT, CONF>
        where CONT : DCONT
        where DCONT : Content
        where CONF : CommandConfiguration, new()
    {
        #region Declarations
        private object syncObject = new object();
        /// <summary>
        /// This collection holds the lookup table for the refType and refValue parameters.
        /// </summary>
        protected Dictionary<KeyValuePair<string,string>, ContentIdentifier?> mReferenceLookUp;
        #endregion // Declarations
        #region Constructor
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        public ResourceBasePersistenceManager() : this(null) { }
        /// <summary>
        /// This is the component model constructor.
        /// </summary>
        /// <param name="container">The container</param>
        public ResourceBasePersistenceManager(IContainer container)
            : base(container)
        {
            mReferenceLookUp = new Dictionary<KeyValuePair<string, string>, ContentIdentifier?>();
        }
        #endregion // Constructors

        #region Read(CDSContext context)
        /// <summary>
        /// This is the read method which is used by cache managers to return the relevant content.
        /// </summary>
        /// <param name="context">The job context.</param>
        protected override bool Read(CDSContext context)
        {

            Type objectType = context.Request.DataType;
            //OK, get the unique identifier for the request.
            ContentIdentifier? id;
            if (!ProcessIdentifier(context, out id))
                return false;

            //If the request does not include a version ID then we cannot read from the cache
            if (!context.Request.DataVersionID.HasValue)
                return false;

            Guid vid = context.Request.DataVersionID.Value;

            CacheItemContainer<CONT> cacheContainer = null;
            lock (syncObject)
            {
                //Check whether we have the value in the cache.
                if (!mDataCache.ContainsKey(id.Value))
                    return false;
                //Get the cache item.
                cacheContainer = mDataCache[id.Value];
                //If the cache item is valid, then clone the content and return.
                if (!cacheContainer.CacheHasExpired)
                {
                    context.Response.Data = (CONT)cacheContainer.ItemCloned(context.ContextSettings.PoolManager);
                    context.Response.Status = CH.HTTPCodes.OK_200;
                    return true;
                }

                //Ok, the cache has expired, but we can check whether the version is still valid and return the content.
                Guid? CID, VID;
                string vidStatus = context.Job.Execute(objectType,
                    CDSData.Get(CDSAction.VersionCheck, id.Value.ContentID, id.Value.VersionID),
                    out CID, out VID);
                //Ok, the cached item is valid so we can return.
                if (vidStatus == CH.HTTPCodes.OK_200 && VID.HasValue && id.Value.VersionID == VID.Value)
                {
                    cacheContainer.CacheRevalidate();

                    context.Response.Data = (CONT)cacheContainer.ItemCloned(context.ContextSettings.PoolManager);

                    context.Response.Status = CH.HTTPCodes.OK_200;
                    return true;
                }

                //So we cannot cache and need to remove the entity reference.
                cacheContainer.CacheClear();
                mDataCache.Remove(id.Value);
                return false;
            }
        }
        #endregion // Read(CDSContext context)
        #region VersionCheck(CDSContext context)
        /// <summary>
        /// This method resolves the version ID of the entity. This ID will be regenerated each time the persistence manager is started.
        /// </summary>
        /// <param name="context">The current request context.</param>
        /// <returns>Returns true, as this is the definitive authority for the specified resource type.</returns>
        protected override bool VersionCheck(CDSContext context)
        {
            Guid? typeID = context.Request.DataTypeID;
            if (!typeID.HasValue)
                typeID = Content.GetContentTypeAttributeID(context.Request.DataType);

            //Get the unique identifier for the context data object.
            ContentIdentifier? id;
            if (!ProcessIdentifier(context, out id))
            {
                context.Response.Status = CH.HTTPCodes.BadRequest_400;
                return true;
            }

            string status = CH.HTTPCodes.NotFound_404;

            lock (syncObject)
            {
                if (mDataCache.ContainsKey(id.Value))
                {
                    status = CH.HTTPCodes.OK_200;
                    context.Response.CurrentContentID = context.Request.DataContentID;
                    context.Response.CurrentVersionID = context.Request.DataVersionID;
                }
            }

            context.Response.Status = status;

            return true;
        }
        #endregion // VersionCheck(CDSContext context)

        #region ResolveReference(CDSContext context)
        /// <summary>
        /// This method resolves the reference parameters against the resources stored in the assembly, and loads them in to the collection.
        /// </summary>
        /// <param name="context">the request context.</param>
        /// <returns>Returns true, as this is the definitive authority for the specified resource type.</returns>
        protected override bool ResolveReference(CDSContext context)
        {
            Guid? tid = context.Request.DataTypeID;
            if (!tid.HasValue)
                tid = Content.GetContentTypeAttributeID(context.Request.DataType);

            Guid? cid, vid;
            string response = CH.HTTPCodes.NotFound_404;

            KeyValuePair<string, string> key = 
                new KeyValuePair<string, string>(context.Request.DataReferenceType, context.Request.DataReferenceValue);

            if (ResolveResourceReference(context, tid.Value, key, out cid, out vid))
            {
                response = CH.HTTPCodes.Continue_100;
                context.Request.DataVersionID = vid;
                context.Request.DataContentID = cid;
                context.Request.DataTypeID = tid;
            }

            context.Response.Status = response;
            return true;
        }
        #endregion // ResolveReference(CDSContext context)

        #region ResolveResourceReference(CDSContext context, Guid tid, KeyValuePair<string, string> key, out Guid? cid, out Guid? vid)
        /// <summary>
        /// This method resolves the request in the assembly, and loads the entity.
        /// </summary>
        /// <param name="context">The request context.</param>
        /// <param name="tid">The content type ID/</param>
        /// <param name="key">The refType/refValue key value pair.</param>
        /// <param name="cid">The content ID value.</param>
        /// <param name="vid">The version ID value.</param>
        /// <returns>Returns true if the request has been resolved and the entity has been loaded successfully.</returns>
        protected virtual bool ResolveResourceReference(
            CDSContext context, Guid tid, KeyValuePair<string, string> key, out Guid? cid, out Guid? vid)
        {
            cid = null;
            vid = null;
            ContentIdentifier? id = null;
            bool success = true;

            lock (syncObject)
            {
                //Check whether we have previously resolved this reference.
                if (mReferenceLookUp.ContainsKey(key))
                {
                    id = mReferenceLookUp[key];
                    if (!id.HasValue)
                        return false;

                    cid = id.Value.ContentID;
                    vid = id.Value.VersionID;

                    return true;
                }

                //OK, this is the first time, so we need to check whether the resmurce exists, and if so we need to add it to the collection.
                Uri resourceUri = null;

                byte[] blob;
                try
                {
                    resourceUri = BaseResourceLocationResolve(typeof(CONT), tid, key);
                    blob = RH.ResourceLoadFromUri(resourceUri);
                }
                catch (Exception ex)
                {
                    success = false;
                    blob = null;
                }

                if (success)
                {
                    CONT data = null;
                    try
                    {
                        data = ContentCreate(context, tid, blob, key, resourceUri);

                        id = new ContentIdentifier(data);

                        cid = id.Value.ContentID;
                        vid = id.Value.VersionID;

                        if (!mDataCache.ContainsKey(id.Value))
                            mDataCache.Add(id.Value, new CacheItemContainer<CONT>(data, true, 0));
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        if (data != null && data.ObjectPoolCanReturn)
                            data.ObjectPoolReturn();
                    }
                }
                //Add the request to the collection, note: if the request is unsuccessful the ContentIdentifier will be null. 
                //This is required to ensure that the Persistence Manager only resolves the request a single time.
                mReferenceLookUp.Add(key, id);
            }
            return success;
        }
        #endregion // ResolveResourceReference(CDSContext context, Guid tid, KeyValuePair<string, string> key, out Guid? cid, out Guid? vid)

        #region ContentCreate(CDSContext context, Guid tid, byte[] blob, KeyValuePair<string, string> key, Uri resourceUri)
        /// <summary>
        /// This method creates the default content type as specified by the generic CONT parameter.
        /// You may override this method to provide more finegrained support when the Persistence Manager supports multiple content types.
        /// </summary>
        /// <param name="context">The current request context.</param>
        /// <param name="tid">the request content type id/</param>
        /// <returns>Returns a new object from the object pool.</returns>
        protected virtual CONT ContentCreate(CDSContext context, Guid tid, byte[] blob, KeyValuePair<string, string> key, Uri resourceUri)
        {
            CONT data = context.GetObjectPool<CONT>().Get();

            data.Load(blob, 0, blob.Length);

            PrepareData(context, key, resourceUri, data);

            return data;
        }
        #endregion // ContentCreate(CDSContext context, Guid tid)
        #region PrepareData(CDSContext context, KeyValuePair<string, string> key, Uri resourceUri, CONT data)
        /// <summary>
        /// This method prepares the content and sets any necessary parameters
        /// before it is returned. This method is useful when the data embedded in the assembly is a native type such as an image, and 
        /// you need to set the entity paramters such as ID and Version before it is returned to the requestor.
        /// By default this method does not alter the data passed.
        /// </summary>
        /// <param name="context">the current context.</param>
        /// <param name="key">The request refType/refValue key pair.</param>
        /// <param name="resourceUri">The request resource Uri.</param>
        /// <param name="data">The entity to adjust.</param>
        protected virtual void PrepareData(CDSContext context, KeyValuePair<string, string> key, Uri resourceUri, CONT data)
        {
        }
        #endregion // PrepareData(CDSContext context, KeyValuePair<string, string> key, Uri resourceUri, CONT data)

        #region BaseResourceString
        /// <summary>
        /// This string returns the path to the base folder in the assembly. This Uri should use the xmrres scheme. Any other scheme will be rejected.
        /// </summary>
        protected virtual string BaseResourceString
        {
            get
            {
                throw new NotImplementedException("BaseResourceString is not implemented.");
            }
        }
        #endregion // BaseResourceString
        #region BaseResourceLocationResolve(Type type, Guid tid, KeyValuePair<string, string> key)
        /// <summary>
        /// This method resolves the resource loacation for the requested content type. This method can be overriden to give
        /// more finegrained support when the Persistence Manager supports multiple content types.
        /// </summary>
        /// <param name="type">The request content type.</param>
        /// <param name="tid">The request content type id.</param>
        /// <param name="key">The reftype/refValue key value pair.</param>
        /// <returns>Returns the resource Uri the resolves the location of the content.</returns>
        protected virtual Uri BaseResourceLocationResolve(Type type, Guid tid, KeyValuePair<string, string> key)
        {
            return new Uri(string.Format(BaseResourceString, key.Value));
        }
        #endregion // BaseResourceLocationResolve(Type type, Guid tid, KeyValuePair<string, string> key)
    }
}
