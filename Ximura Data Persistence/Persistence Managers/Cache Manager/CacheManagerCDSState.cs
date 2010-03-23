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
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Text;

using Ximura;
using Ximura.Data;
using Ximura.Helper;
using CH = Ximura.Helper.Common;
using RH = Ximura.Helper.Reflection;
using AH = Ximura.Helper.AttributeHelper;
using Ximura.Server;


using Ximura.Command;
#endregion // using
namespace Ximura.Persistence
{
    /// <summary>
    /// This is the cache manager PM state.
    /// </summary>
    [CDSStateActionPermit(CDSStateAction.Cache)]
    [CDSStateActionPermit(CDSStateAction.Read)]
    public class CacheManagerCDSState : FileSystemBasePMCDSState<Content, Content, CacheManagerCDSConfiguration>, ICDSCacheManager
    {
        #region Declarations
        private ICDSCacheManagerBridge bridge = null;
        /// <summary>
        /// This is the file system storage manager.
        /// </summary>
        protected IXimuraStorageFileSystem storageFileSystem = null;
        private object syncObject = new object();
        #endregion // Declarations
        #region Constructors
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        public CacheManagerCDSState() : this(null) { }
        /// <summary>
        /// This is the component model constructor.
        /// </summary>
        /// <param name="container">The container</param>
        public CacheManagerCDSState(IContainer container)
            : base(container)
        {
        }
        #endregion // Constructors

        #region Read(CDSContext context)
        /// <summary>
        /// This is the read method which is used by cache managers to return the relevant content.
        /// </summary>
        /// <param name="context">The job context.</param>
        protected override bool Read(CDSContext context)
        {
            if (!context.ContentIsCacheable)
                return false;

            Type objectType = context.Request.DataType;
            //OK, get the unique identifier for the request.
            ContentIdentifier? id;
            if (!ProcessIdentifier(context, out id))
                return false;
            //If the request does not include a version ID then we cannot read from the cache
            if (!context.Request.DataVersionID.HasValue)
                return false;

            Guid vid = context.Request.DataVersionID.Value;

            CacheItemContainer<Content> cacheContainer = null;
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
                    context.Response.Data = (Content)cacheContainer.ItemCloned(context.ContextSettings.PoolManager);
                    context.Response.Status = CH.HTTPCodes.OK_200;
                    return true;
                }

                //Ok, the cache has expired, but we can check whether the version is still valid and return the content.
                Guid? CID, VID;
                string vidStatus = context.CDSHelperDirect.Execute(objectType,
                    CDSData.Get(CDSStateAction.VersionCheck, id.Value.ContentID, id.Value.VersionID), 
                    out CID, out VID);
                //Ok, the cached item is valid so we can return.
                if (vidStatus == CH.HTTPCodes.OK_200 && VID.HasValue && id.Value.VersionID == VID.Value)
                {
                    cacheContainer.CacheRevalidate();

                    context.Response.Data = (Content)cacheContainer.ItemCloned(context.ContextSettings.PoolManager);

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
        #region Cache(CDSContext context)
        /// <summary>
        /// This is the cache method which is used by cache managers to add new content to the cache.
        /// </summary>
        /// <param name="context">The job context.</param>
        protected override bool Cache(CDSContext context)
        {
            if (!context.ContentIsCacheable)
                return false;

            CDSStateAction action = context.CDSStateActionResolve();

            //Cache call can only support read at the moment.
            switch (action)
            {
                case CDSStateAction.Read:
                    break;
                default:
                    return false;
            }

            //Get the unique identifier for the context data object.
            ContentIdentifier? id;
            if (!ProcessIdentifier(context, out id))
                return false;

            lock (syncObject)
            {
                Content newContent = null;
                try
                {
                    //Ok, nothing to do as the content is already in the cache.
                    if (mDataCache.ContainsKey(id.Value))
                        return false;

                    newContent = (Content)context.Response.Data.Clone();

                    mDataCache.Add(id.Value, new CacheItemContainer<Content>(newContent, true, 0));
                }
                catch (Exception ex)
                {
                    //ok, if there are any errors at this point we wipe out the cache value
                    //and returns the objects to the pool.
                    if (mDataCache.ContainsKey(id.Value))
                        mDataCache.Remove(id.Value);

                    if (newContent != null && newContent.ObjectPoolCanReturn)
                    {
                        newContent.ObjectPoolReturn();
                        newContent = null;
                    }
                    return false;
                }

                return true;
            }
        }
        #endregion // Cache(CDSContext context)

        #region SupportsEntityAction(CDSStateAction action, Type objectType)
        /// <summary>
        /// This method returns the 
        /// </summary>
        /// <param name="action">The actions.</param>
        /// <param name="objectType">The object type.</param>
        /// <returns>Returns int.max if the content type supports caching.</returns>
        public override int SupportsEntityAction(CDSStateAction action, Type objectType)
        {
            //Only read and cache are supported.
            if (action != CDSStateAction.Read && action != CDSStateAction.Cache)
                return -1;

            //If we support caching then we returns the highest priority 0, else we 
            //returns -1 to signify that caching is not supported.
            return (bridge.EntityTypeSupportsCaching(objectType))? 0 : -1;
        }
        #endregion // SupportsEntityAction(CDSStateAction action, Type objectType)

        #region ServicesReference()
        /// <summary>
        /// This method registers the cache manager with the cachemanagerbridge service.
        /// </summary>
        protected override void ServicesReference()
        {
            base.ServicesReference();

            bridge = GetService<ICDSCacheManagerBridge>();

            if (bridge != null)
            {
                bridge.BridgeRegister(this);
                bridge.PollRegister(this, 15);
            }

            storageFileSystem = GetService<IXimuraStorageFileSystem>();
        }
        #endregion // ServicesReference()
        #region ServicesDereference()
        /// <summary>
        /// This method removes the cache manager from the bridge service.
        /// </summary>
        protected override void ServicesDereference()
        {
            storageFileSystem = null;

            if (bridge != null)
            {
                bridge.BridgeUnregister(this);
                bridge.PollUnregister(this);
                bridge = null;
            }

            base.ServicesDereference();
        }
        #endregion // ServicesDereference()

        #region CacheHits
        /// <summary>
        /// This text property contains a brief description of the number of cache hits for the cache collection.
        /// </summary>
        private string CacheHits
        {
            get
            {
                StringBuilder sb = new StringBuilder();

                mDataCache.Values
                    .OrderBy(ci => ci.TotalHitCount)
                    .ForEach(ci =>
                        {
                            sb.AppendFormat("{0} -> {1}", ci.TotalHitCount, ci.Identifier.ContentTypeAQNTruncated);
                            sb.AppendLine();
                        });

                return sb.ToString();
            }
        }
        #endregion // CacheHits
        #region EntityCount
        /// <summary>
        /// This text property contains thebreakdown of cached entities in the cache manager.
        /// </summary>
        private string EntityCount
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                mDataCache.Values
                    .GroupBy(ci => ci.Identifier.ContentTypeAQNTruncated)
                    .OrderByDescending(g => g.Count())
                    .ForEach(item =>
                        {
                            sb.AppendFormat("{0} -> {1}", item.Key, item.Count());
                            sb.AppendLine();
                        });

                return sb.ToString();
            }
        }
        #endregion // EntityCount

        #region TimerPoll()
        /// <summary>
        /// This method is used to poll cache items that have aged.
        /// </summary
        public void TimerPoll()
        {
            try
            {
                List<KeyValuePair<ContentIdentifier, CacheItemContainer<Content>>> expired = null;

                lock (syncObject)
                {
                    long size = mDataCache.Values.Sum(ci => ci.Size);

                    //Ok, we will remove 10% if we have exceeded the maximum value.
                    int number = mDataCache.Count / 10;

                    if (number==0 || size < (134217728/2))
                        return;

                    expired = mDataCache
                        .OrderByDescending(i => i.Value.ExpiryRating)
                        .Take(number)
                        .ToList();
                }

                foreach (var cacheItem in expired)
                {
                    //So we cannot cache and need to remove the entity reference.
                    lock (syncObject)
                    {
                        if (mDataCache.ContainsKey(cacheItem.Key))
                        {
                            cacheItem.Value.CacheClear();
                            mDataCache.Remove(cacheItem.Key);
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }

        }

        #endregion
    }
}
