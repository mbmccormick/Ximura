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
using System.Runtime.Serialization;
using System.IO;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

using Ximura;
using Ximura.Data;
using Ximura.Data;

using CH = Ximura.Common;
using Ximura.Framework;
using Ximura.Framework;
using Ximura.Communication;
#endregion // using
namespace Ximura.Communication
{
    public class ContentCompilerSettings : ContextSettings<ContentCompilerState, ContentCompilerConfiguration, ContentCompilerPerformance>
    {
        #region Declarations
        private Dictionary<Type, IDictionary> mTypeHolder;

        private object syncTypeHolder = new object();
        private object syncCacheContainer = new object();
        private object syncCache = new object();

        private CDSHelper mCDSHelper;// = new CDSHelper(null);
        #endregion // Declarations
        #region Constructors
        /// <summary>
        /// This constructor is called by the FSM when initiating the settings.
        /// </summary>
        public ContentCompilerSettings()
            : base()
        {
            mTypeHolder = new Dictionary<Type, IDictionary>();
        }
        #endregion

        protected virtual Dictionary<string, Dictionary<string, CacheItemContainer<C>>> GetTypeHolder<C>() where C : Content
        {
            if (mTypeHolder.ContainsKey(typeof(C)))
                return (Dictionary<string, Dictionary<string, CacheItemContainer<C>>>)mTypeHolder[typeof(C)];

            lock (syncTypeHolder)
            {
                if (mTypeHolder.ContainsKey(typeof(C)))
                    return (Dictionary<string, Dictionary<string, CacheItemContainer<C>>>)
                        mTypeHolder[typeof(C)];

                Dictionary<string, Dictionary<string, CacheItemContainer<C>>> data =
                    new Dictionary<string, Dictionary<string, CacheItemContainer<C>>>();

                mTypeHolder.Add(typeof(C), data);

                return data;
            }
        }

        protected virtual Dictionary<string, CacheItemContainer<C>> GetCacheItemContainer<C>(string refType) where C: Content
        {
            Dictionary<string, Dictionary<string, CacheItemContainer<C>>> typeHolder 
                = GetTypeHolder<C>();

            if (typeHolder.ContainsKey(refType))
                return typeHolder[refType];

            lock (syncCacheContainer)
            {
                if (typeHolder.ContainsKey(refType))
                    return typeHolder[refType];

                Dictionary<string, CacheItemContainer<C>> data 
                    = new Dictionary<string, CacheItemContainer<C>>();

                typeHolder.Add(refType, data);

                return data;
            }
        }

        public C CacheGet<C>(string refType, string refValue) where C : Content
        {
            Dictionary<string, CacheItemContainer<C>> cacheHolder 
                = GetCacheItemContainer<C>(refType);

            if (cacheHolder.ContainsKey(refValue))
            {
#if (DEBUG)
                Debug.WriteLine(string.Format("Cache hit {0} --> {1} --> {2}"
                    , typeof(C).FullName, refType, refValue));
#endif
                return cacheHolder[refValue].Item;
            }

            C data;
            string status = CDSHelperProcess.Execute<C>(
                CDSData.Get(CDSStateAction.Read, refType, refValue), out data);

            if (status != CH.HTTPCodes.OK_200)
                throw new ArgumentException("Stylesheet name cannot be found.");

            lock (syncCache)
            {
                if (cacheHolder.ContainsKey(refValue))
                    return cacheHolder[refValue].Item;


                cacheHolder.Add(refValue, new CacheItemContainer<C>(data, false, 0));

                return data;
            } 
        }


        #region CDSHelperProcess
        /// <summary>
        /// This is the Command Session used to retrieve system data.
        /// </summary>
        protected CDSHelper CDSHelperProcess
        {
            get
            {
                if (mCDSHelper == null)
                    mCDSHelper = new CDSHelper(ProcessSession);

                return mCDSHelper;
            }
        }
        #endregion // CDSHelperProcess

    }
}
