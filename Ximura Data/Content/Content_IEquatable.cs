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
using CH = Ximura.Helper.Common;
#endregion // using
namespace Ximura.Data
{
    public abstract partial class Content : IEquatable<IXimuraContent>
    {
        #region Equals(object obj)
        /// <summary>
        /// This is an override of the equals method for the content object.
        /// This returns true if the object is derived from Content and the ID and Version
        /// and the dirty flag are the same.
        /// </summary>
        /// <param name="obj">The object to compare with this object.</param>
        /// <returns>A boolean value indicating whether the content is the same object.</returns>
        public override bool Equals(object obj)
        {
            //First check it implements the content interface
            IXimuraContent cObj = obj as IXimuraContent;

            if (cObj == null)
                return false;

            return Equals(cObj);
        }
        #endregion // Equals(object obj)
        #region Equals(IXimuraContent cObj)
        /// <summary>
        /// This is the generic equals method.
        /// </summary>
        /// <param name="cObj">The object to compare.</param>
        /// <returns>Returns true if the objects are the same.</returns>
        public bool Equals(IXimuraContent cObj)
        {
            //Then check the three IDs, we do not care about object type.
            if (cObj.IDType != this.IDType)
                return false;
            if (cObj.IDContent != this.IDContent)
                return false;
            if (cObj.IDVersion != this.IDVersion)
                return false;

            return true;
        }
        #endregion // Equals(IXimuraContent cObj)
        #region GetHashCode()
        /// <summary>
        /// This method gets the hash code for the content object.
        /// </summary>
        /// <returns>The content hash code.</returns>
        public override int GetHashCode()
        {
            // TODO:  Add Content.GetHashCode implementation
            return base.GetHashCode();
        }
        #endregion // GetHashCode()
    }
}
