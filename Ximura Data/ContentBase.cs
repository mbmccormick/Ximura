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
using System.Text;
using System.Security.Cryptography;

using Ximura;
using Ximura.Data;
using CH = Ximura.Common;
using AH = Ximura.AttributeHelper;
#endregion // using
namespace Ximura.Data
{
    /// <summary>
    /// This class is is the base abstract class for both Content and ContentHolder.
    /// </summary>
    public abstract partial class ContentBase: IXimuraContent
    {
        #region Declarations
        /// <summary>
        /// This is the protected property that stores the content instance ID
        /// </summary>
        protected Guid mIDContent = Guid.Empty;
        /// <summary>
        /// This is the protected property that stores the version ID of the Entity.
        /// </summary>
        protected Guid mIDVersion = Guid.Empty;
        #endregion 

        #region Constructor
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        public ContentBase() { }
        #endregion // Constructor

        #region IDType
        /// <summary>
        /// This is the type ID of the content. If not specified in the constructor the type ID 
        /// will be taken from the XimuraContentTypeID Attribute.
        /// </summary>
        [Browsable(false)]
        public virtual Guid IDType
        {
            get
            {
                return GetContentTypeAttributeID();
            }
            protected set
            {
                throw new NotSupportedException("TypeID set is not supported as this is implemented by use of the XimuraContentTypeID attribute.");
            }
        }
        #endregion // TypeID
        #region IDContent
        /// <summary>
        /// This is the ID of the Entity.
        /// </summary>
        [Browsable(false)]
        public virtual Guid IDContent
        {
            get
            {
                Guid? idAttr = GetContentAttributeID();
                if (!idAttr.HasValue)
                    return mIDContent;

                return idAttr.Value;
            }
            set
            {
                if (!GetContentAttributeID().HasValue)
                    mIDContent = value;
            }
        }
        #endregion // ID
        #region IDVersion
        /// <summary>
        /// This is the version ID of the entity.
        /// </summary>
        [Browsable(false)]
        public virtual Guid IDVersion
        {
            get
            {
                return mIDVersion;
            }
            set
            {
                mIDVersion = value;
            }
        }
        #endregion // Version

        #region StringConvertToGuid(string ID)
        /// <summary>
        /// This method converts a string in to a new Guid. This can be used to ensure that the same string always 
        /// transforms in to the same Guid.
        /// </summary>
        /// <param name="ID">The string to convert.</param>
        /// <returns>Returns a Guid.</returns>
        protected Guid StringConvertToGuid(string ID)
        {
            byte[] blob = null;

            using (HashAlgorithm hash = new MD5CryptoServiceProvider())
            {
                blob = hash.ComputeHash(Encoding.UTF8.GetBytes(ID));
            }

            return new Guid(blob);
        }
        #endregion
    }
}
