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
using Ximura.Helper;
using CH = Ximura.Helper.Common;
using Ximura.Server;
using Ximura.Command;
#endregion // using
namespace Ximura
{
    /// <summary>
    /// The XimuraComponentPermissionCollection class contains a list of 
    /// security permissions for the class. Permissions are binary, i.e. a user
    /// can have a yes/no right to the permission set.
    /// </summary>
    public class XimuraComponentPermissionCollection
    {
        private long? mSupportedPermissions = null;
        private long? mUnsupportedPermissions = null;
        private object[] classAttributes;
        /// <summary>
        /// This method returns true if the permission is supported.
        /// </summary>
        /// <param name="permissionBitmap"></param>
        /// <returns></returns>
        public bool PermissionsAreSupported(long permissionBitmap)
        {
            return (permissionBitmap & SupportedPermissionBitmap) == permissionBitmap;
        }
        /// <summary>
        /// This property contains the supported permissions for the bitmap.
        /// </summary>
        public long SupportedPermissionBitmap
        {
            get
            {
                if (!mUnsupportedPermissions.HasValue || !mSupportedPermissions.HasValue)
                    CalculatePermissions();

                return mSupportedPermissions.Value;
            }
        }
        /// <summary>
        /// These are the unsupported permissions. Denied permissions cannot be overriden.
        /// </summary>
        public long UnsupportedPermissionBitmap
        {
            get
            {
                if (!mUnsupportedPermissions.HasValue || !mSupportedPermissions.HasValue)
                    CalculatePermissions();

                return mUnsupportedPermissions.Value;
            }
        }

        /// <summary>
        /// This method is called to calculate the permissions.
        /// </summary>
        private void CalculatePermissions()
        {
            classAttributes = this.GetType().GetCustomAttributes(true);

            foreach (object attr in classAttributes)
            {
                if (attr.GetType().IsSubclassOf(typeof(XimuraComponentPermissionAttribute)))
                    SetPermission(attr as XimuraComponentPermissionAttribute);
            }
        }
        /// <summary>
        /// This method sets the permissions
        /// </summary>
        /// <param name="attr">The permission attribute to process.</param>
        private void SetPermission(XimuraComponentPermissionAttribute attr)
        {
            lock (this)
            {
                if (!mUnsupportedPermissions.HasValue)
                    mUnsupportedPermissions = 0;

                if (!mSupportedPermissions.HasValue)
                    mSupportedPermissions = 0;

                if (attr.PermissionIsSupported)
                {
                    mSupportedPermissions &= attr.PermissionBitmap;
                }
                else
                {
                    mUnsupportedPermissions |= attr.PermissionBitmap;
                }

                if (mUnsupportedPermissions != 0)
                    mSupportedPermissions &= (~mUnsupportedPermissions);
            }
        }

    }

}
