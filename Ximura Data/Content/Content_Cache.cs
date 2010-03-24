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
using System.IO;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Diagnostics;

using System.Text;

using Ximura;
using Ximura.Data;
using Ximura.Data;

using CH = Ximura.Common;
#endregion // using
namespace Ximura.Data
{
    public abstract partial class Content
    {
        #region CacheOptions
        /// <summary>
        /// This is the cache options as specified by the ContentCachePolicy attribute.
        /// You can override this method for more fine-grained caching functionality.
        /// </summary>
        public virtual ContentCacheOptions CacheOptions
        {
            get
            {
                if (attrContentCachePolicy == null)
                    return ContentCacheOptions.CannotCache;

                return attrContentCachePolicy.CacheStyle;
            }
        }
        #endregion // CacheOptions

        #region CacheExpiry
        /// <summary>
        /// This is the cache expiry time in milliseconds.
        /// </summary>
        public virtual int CacheExpiry
        {
            get
            {
                if (attrContentCachePolicy == null)
                    return -1;

                return attrContentCachePolicy.TimeOut;
            }
        }
        #endregion // CacheExpiry
    }
}
