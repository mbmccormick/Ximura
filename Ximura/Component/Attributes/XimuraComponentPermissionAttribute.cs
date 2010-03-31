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
using System.Collections;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Threading;
using System.Reflection;

using Ximura;
using Ximura.Data;
using CH = Ximura.Common;
#endregion // using
namespace Ximura
{
    /// <summary>
    /// XimuraComponentPermissionAttribute is used to specify the security permissions for the 
    /// component.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class XimuraComponentPermissionAttribute : System.Attribute
    {
        #region Declarations
        private long mPermissionBitmap;
        private string mDescription;
        private string mLocalizationID;
        private bool mPermissionIsSupported;
        #endregion

        #region Constructors
        /// <summary>
        /// The controller permission attribute.
        /// </summary>
        /// <param name="PermissionBitmap">The controller permission type.</param>
        /// <param name="description">The controller description.</param>
        public XimuraComponentPermissionAttribute(long PermissionBitmap,
            string description)
            : this(PermissionBitmap, description, null, true) { }
        /// <summary>
        /// The controller permission attribute.
        /// </summary>
        /// <param name="PermissionBitmap">The controller permission type.</param>
        /// <param name="description">The controller description.</param>
        /// <param name="localizationID">The controller localization reference ID.</param>
        public XimuraComponentPermissionAttribute(long PermissionBitmap,
            string description, string localizationID)
            : this(PermissionBitmap, description, localizationID, true) { }
        /// <summary>
        /// The controller permission attribute.
        /// </summary>
        /// <param name="PermissionBitmap">The controller permission type.</param>
        /// <param name="description">The controller description.</param>
        /// <param name="supported">This property defines whether the permission is supported.
        /// For situations where classes inherit permissions, 
        /// you may want to actually turn off support for a a permission.</param>
        public XimuraComponentPermissionAttribute(long PermissionBitmap,
            string description, bool supported)
            : this(PermissionBitmap, description, null, supported) { }
        /// <summary>
        /// The controller permission attribute.
        /// </summary>
        /// <param name="PermissionBitmap">The controller permission type.</param>
        /// <param name="description">The controller description.</param>
        /// <param name="localizationID">The controller localization reference ID.</param>
        /// <param name="supported">This property defines whether the permission is supported.
        /// For situations where classes inherit permissions, 
        /// you may want to actually turn off support for a a permission.</param>
        public XimuraComponentPermissionAttribute(long PermissionBitmap,
            string description, string localizationID, bool supported)
        {
            mPermissionBitmap = PermissionBitmap;
            mDescription = description;
            mLocalizationID = localizationID;
            mPermissionIsSupported = supported;
        }
        #endregion

        #region PermissionBitmap
        /// <summary>
        /// The permission type.
        /// </summary>
        public virtual long PermissionBitmap
        {
            get { return mPermissionBitmap; }
        }
        #endregion // PermissionBitmap
        #region Description
        /// <summary>
        /// The controller description.
        /// </summary>
        public virtual string Description
        {
            get { return mDescription; }
        }
        #endregion // Description

        #region LocalizationID
        /// <summary>
        /// The controller localization reference ID.
        /// </summary>
        public virtual string LocalizationID
        {
            get { return mLocalizationID; }
        }
        #endregion // LocalizationID
        #region PermissionIsSupported
        /// <summary>
        /// This property specifies whether the permission has been set.
        /// </summary>
        public virtual bool PermissionIsSupported
        {
            get { return mPermissionIsSupported; }
        }
        #endregion // PermissionIsSupported
    }
}
