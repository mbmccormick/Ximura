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
using System.Data;
using System.Data.SqlClient;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;

using Ximura;
using Ximura.Data;
using Ximura.Helper;
using CH = Ximura.Helper.Common;
using RH = Ximura.Helper.Reflection;
using AH = Ximura.Helper.AttributeHelper;
using Ximura.Framework;


using Ximura.Framework;
#endregion // using
namespace Ximura.Data
{
    /// <summary>
    /// The CDS Cache manager bridge is used to provide common services to the cache manager.
    /// </summary>
    public class CDSCacheManagerBridge : ICDSCacheManagerBridge
    {
        #region Declarations
        private List<ICDSCacheManager> mBridge;
        private Dictionary<ICDSCacheManager, KeyValuePair<int, DateTime?>> mBridgePoll;
        /// <summary>
        /// This is the content type cache settings.
        /// </summary>
        private Dictionary<Type, XimuraContentCachePolicyAttribute> mCacheSupport;

        private object syncObject = new object();
        #endregion // Declarations
        #region Constructor
        /// <summary>
        /// This is the default constructor for the bridge.
        /// </summary>
        public CDSCacheManagerBridge()
        {
            mCacheSupport = new Dictionary<Type, XimuraContentCachePolicyAttribute>();
            mBridge = new List<ICDSCacheManager>();
            mBridgePoll = new Dictionary<ICDSCacheManager, KeyValuePair<int, DateTime?>>();
        }
        #endregion // Constructor
        #region Poll()
        /// <summary>
        /// This method is called to poll all cache managers. This polling can be used to archive or delete expired content.
        /// </summary>
        public void Poll()
        {
            try
            {
                List<KeyValuePair<ICDSCacheManager, int>> toPoll = null;
                lock (syncObject)
                {
                    toPoll = mBridgePoll
                        .Where(kp => kp.Value.Value.HasValue && kp.Value.Value.Value < DateTime.Now)
                        .Select(kp => new KeyValuePair<ICDSCacheManager,int>(kp.Key, kp.Value.Key))
                        .ToList();
                }

                foreach (var cm in toPoll)
                {
                    cm.Key.TimerPoll();

                    lock (syncObject)
                    {
                        mBridgePoll[cm.Key] = new KeyValuePair<int, DateTime?>(cm.Value, DateTime.Now.AddSeconds(cm.Value));
                    }
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion // Poll()


        #region ICDSCacheManagerBridge Members

        public bool EntityTypeSupportsCaching(Type objectType)
        {
            lock (syncObject)
            {
                XimuraContentCachePolicyAttribute attrContentCachePolicy =
                    CacheAttribute(objectType);

                //If we support caching then we return true, else we 
                //returns false to signify that caching is not supported.
                return attrContentCachePolicy != null &&
                    attrContentCachePolicy.CacheStyle != ContentCacheOptions.CannotCache;
            }
        }

        public XimuraContentCachePolicyAttribute CacheAttribute(Type objectType)
        {
            lock (syncObject)
            {
                XimuraContentCachePolicyAttribute attrContentCachePolicy = null;

                if (!mCacheSupport.ContainsKey(objectType))
                    lock (syncObject)
                    {
                        if (!mCacheSupport.ContainsKey(objectType))
                        {
                            attrContentCachePolicy
                                = AH.GetAttribute<XimuraContentCachePolicyAttribute>(objectType);
                            //Note: we add a null value if the content object does not
                            //have a Cache Policy Attribute. This signifies that the content
                            //is not cacheable.
                            mCacheSupport.Add(objectType, attrContentCachePolicy);
                        }
                    }

                attrContentCachePolicy = mCacheSupport[objectType];

                //If we support caching then we return true, else we 
                //returns false to signify that caching is not supported.
                return attrContentCachePolicy;
            }
        }

        bool ICDSCacheManagerBridge.BridgeRegister(ICDSCacheManager cacheManager)
        {
            lock (syncObject)
            {
                mBridge.Add(cacheManager);
                return true;
            }
        }

        bool ICDSCacheManagerBridge.BridgeUnregister(ICDSCacheManager cacheManager)
        {
            lock (syncObject)
            {
                if (!mBridge.Contains(cacheManager))
                    return false;

                mBridge.Remove(cacheManager);
                return true;
            }
        }

        bool ICDSCacheManagerBridge.PollRegister(ICDSCacheManager cacheManager, int timeInSecs)
        {
            lock (syncObject)
            {
                mBridgePoll.Add(cacheManager, new KeyValuePair<int, DateTime?>(timeInSecs, DateTime.Now.AddSeconds(timeInSecs)));
                return true;
            }
        }

        bool ICDSCacheManagerBridge.PollUnregister(ICDSCacheManager cacheManager)
        {
            lock (syncObject)
            {
                if (!mBridgePoll.ContainsKey(cacheManager))
                    return false;

                mBridgePoll.Remove(cacheManager);
                return true;
            }
        }

        #endregion
    }

}
