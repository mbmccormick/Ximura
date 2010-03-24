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

using CH = Ximura.Common;
#endregion // using
namespace Ximura.Data
{
    #region ContentCacheOptions
    /// <summary>
    /// Summary description for ContentAttributes.
    /// </summary>
    [Flags()]
    public enum ContentCacheOptions
    {
        /// <summary>
        /// The content object is always cacheable
        /// </summary>
        Cacheable = 2,
        /// <summary>
        /// Each individual content object should be polled to check the cache status.
        /// </summary>
        VersionCheck = 1,
        /// <summary>
        /// The content cannot be cached under any circumstances.
        /// </summary>
        CannotCache = 0
    }
    #endregion // ContentCacheOptions

    #region XimuraContentCachePolicyAttribute
    /// <summary>
    /// This attribute sets the cache metadata properties for particular content objects.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public sealed class XimuraContentCachePolicyAttribute : Attribute
    {
        #region Declarations
        private ContentCacheOptions mCacheStyle;
        private int mTimeOut;
        #endregion // Declarations
        #region Constructor
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        /// <param name="CacheStyle">The cache policy for the content object.</param>
        public XimuraContentCachePolicyAttribute(ContentCacheOptions CacheStyle)
            : this(CacheStyle, -1)
        {
        }
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        /// <param name="CacheStyle">The cache policy for the content object.</param>
        /// <param name="TimeOut">The time out in seconds.</param>
        public XimuraContentCachePolicyAttribute(ContentCacheOptions CacheStyle, int TimeOut)
        {
            mCacheStyle = CacheStyle;
            mTimeOut = TimeOut;
        }
        #endregion // Constructor
        #region CacheStyle
        /// <summary>
        /// This is cache policy.
        /// </summary>
        public ContentCacheOptions CacheStyle
        {
            get { return mCacheStyle; }
        }
        #endregion // CacheStyle

        #region TimeOut
        /// <summary>
        /// This is the time in seconds that the item can remain in the cache
        /// without expiring, or being version checked against the CDS.
        /// </summary>
        public int TimeOut
        {
            get { return mTimeOut; }
        }
        #endregion // TimeOut
    }
    #endregion // XimuraContentCachePolicyAttribute
}
