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
    /// <summary>
    /// The XimuraContentTypeID attribute uniquely identifies a content type. This is useful
    /// as it allows the Ximura to identify different content implementations within the system.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public sealed class XimuraContentTypeIDAttribute : Attribute
    {
        #region Declarations
        private Guid mID;
        private string mDescription;
        #endregion // Declarations
        #region Constructors
        /// <summary>
        /// The attribute constructor.
        /// </summary>
        /// <param name="ContentID">The content GUID string.</param>
        public XimuraContentTypeIDAttribute(string ContentID) : this(ContentID, "") { }
        /// <summary>
        /// The attribute constructor.
        /// </summary>
        /// <param name="ContentID">The content GUID string.</param>
        /// <param name="ContentDescription">The content description.</param>
        /// entity in its own right, or just a fragment of a bigger entity. This is useful for caching
        /// purposes.</param>
        public XimuraContentTypeIDAttribute(string ContentID, string ContentDescription)
        {
            mID = new Guid(ContentID);
            mDescription = ContentDescription;
        }
        #endregion // Constructors

        #region ID
        /// <summary>
        /// This is the ID of the content.
        /// </summary>
        public Guid ID
        {
            get
            {
                return mID;
            }
        }
        #endregion // ID
        #region Description
        /// <summary>
        /// This is the description fo the content type. This description may be output during debugging.
        /// </summary>
        public string Description
        {
            get
            {
                return mDescription;
            }
        }
        #endregion // Description
    }
}
