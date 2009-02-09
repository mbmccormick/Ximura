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
using System.Reflection;
using System.IO;
#endregion // using
namespace Ximura.Data
{
    /// <summary>
    /// This attribute is used to specify the path to embedded resources.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class XimuraDataContentSchemaReferenceAttribute : XimuraSchemaAttribute
    {
        #region Declarations
        private Uri mUriPath;
        #endregion
        #region Constructors
        /// <summary>
        /// This is extended constructor that should be used when you want to 
        /// provide a new record.
        /// </summary>
        /// <param name="uriPath">The schema name.</param>
        /// <param name="resPath">The name of the default content.</param>
        public XimuraDataContentSchemaReferenceAttribute(string uriPath,
            string resPath)
            : base(resPath)
        {
            mUriPath = new Uri(uriPath);
        }
        #endregion

        #region UriPath
        /// <summary>
        /// This is the Uri of the external path.
        /// </summary>
        public Uri UriPath
        {
            get { return mUriPath; }
        }
        #endregion // UriPath
    }
}
