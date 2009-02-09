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
using System.IO;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Diagnostics;
using System.Drawing;
using System.Text;

using Ximura;
using Ximura.Data;
using Ximura.Data.Serialization;
using Ximura.Helper;
using AH = Ximura.Helper.AttributeHelper;
using CH = Ximura.Helper.Common;
#endregion // using
namespace Ximura.Data
{
    public abstract partial class Content
    {
        #region Declarations
        /// <summary>
        /// This collection holds the content reference attributes.
        /// </summary>
        protected XimuraContentReferenceAttribute[] attrsContentReference;
        /// <summary>
        /// This is the cache policy for the content.
        /// </summary>
        protected XimuraContentCachePolicyAttribute attrContentCachePolicy;
        /// <summary>
        /// This is the fragmentation attribute.
        /// </summary>
        protected XimuraDataContentFragmentAttribute attrContentFragment;

        /// <summary>
        /// This is the ID mapping attribute
        /// </summary>
        protected XimuraDataContentIDMappingAttribute attrContentMappingContentID;
        /// <summary>
        /// This is the version mapping attribute
        /// </summary>
        protected XimuraDataContentVersionMappingAttribute attrContentMappingVersionID;

        #endregion

        #region SetAttributes()
        /// <summary>
        /// This protected method sets the mapping attributes for the content.
        /// </summary>
        protected virtual void SetAttributes()
        {
            attrContentMappingContentID = AH.GetAttribute<XimuraDataContentIDMappingAttribute>(GetType());

            attrContentMappingVersionID = AH.GetAttribute<XimuraDataContentVersionMappingAttribute>(GetType());

            attrContentFragment = AH.GetAttribute<XimuraDataContentFragmentAttribute>(GetType());

            attrContentCachePolicy = AH.GetAttribute<XimuraContentCachePolicyAttribute>(GetType());

            attrsContentReference = AH.GetAttributes<XimuraContentReferenceAttribute>(GetType());
        }
        #endregion
    }
}
