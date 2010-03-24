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
    #region XimuraEntityType
    /// <summary>
    /// This enumeration specifies the type of content.
    /// </summary>
    public enum XimuraEntityType
    {
        /// <summary>
        /// The entity is full content.
        /// </summary>
        Entity,
        /// <summary>
        /// The entity is partial content.
        /// </summary>
        EntityFragment
    }
    #endregion // XimuraEntityType

    #region XimuraContentIDAttribute
    /// <summary>
    /// The XimuraContentTypeID attribute uniquely identifies a content type. This is useful
    /// as it allows the Ximura to identify different content implementations within the system.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public sealed class XimuraContentIDAttribute : Attribute
    {
        #region Declarations
        private Guid mID;
        private string mDescription;
        private XimuraEntityType cType;
        #endregion // Declarations
        #region Constructors
        /// <summary>
        /// The attribute constructor.
        /// </summary>
        /// <param name="ContentID">The content GUID string.</param>
        public XimuraContentIDAttribute(string ContentID) : this(ContentID, "", XimuraEntityType.Entity) { }
        /// <summary>
        /// The attribute constructor.
        /// </summary>
        /// <param name="ContentID">The content GUID string.</param>
        /// <param name="ContentDescription">The content description.</param>
        public XimuraContentIDAttribute(string ContentID,
            string ContentDescription)
            : this(ContentID, ContentDescription, XimuraEntityType.Entity) { }
        /// <summary>
        /// The attribute constructor.
        /// </summary>
        /// <param name="ContentID">The content GUID string.</param>
        /// <param name="entityType">The entity type. This specifies whether the content is an
        /// entity in its own right, or just a fragment of a bigger entity. This is useful for caching
        /// purposes.</param>
        public XimuraContentIDAttribute(string ContentID, XimuraEntityType entityType)
            :
            this(ContentID, "", entityType) { }
        /// <summary>
        /// The attribute constructor.
        /// </summary>
        /// <param name="ContentID">The content GUID string.</param>
        /// <param name="ContentDescription">The content description.</param>
        /// <param name="entityType">The entity type. This specifies whether the content is an
        /// entity in its own right, or just a fragment of a bigger entity. This is useful for caching
        /// purposes.</param>
        public XimuraContentIDAttribute(string ContentID,
            string ContentDescription, XimuraEntityType entityType)
        {
            mID = new Guid(ContentID);
            mDescription = ContentDescription;
            cType = entityType;
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
        #region EntityType
        /// <summary>
        /// This is the content type of
        /// </summary>
        public XimuraEntityType EntityType
        {
            get { return cType; }
        }
        #endregion // EntityType
    }
    #endregion
}
