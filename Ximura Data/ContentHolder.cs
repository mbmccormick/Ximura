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

using Ximura;
using Ximura.Data;
using CH = Ximura.Common;
using AH = Ximura.AttributeHelper;
using System.Runtime.Serialization.Formatters.Binary;
#endregion // using
namespace Ximura.Data
{
    /// <summary>
    /// The content holder is used to manage non-standard entities within the persistence framework.
    /// </summary>
    /// <typeparam name="T">The core entity type.</typeparam>
    public partial class ContentHolder<T>: ContentBase
    {
        #region Static Declarations
        private static Dictionary<Type, Guid> sTypeIds;
        private static object lockTypeIds;
        #endregion // Static Declarations
        #region Static Constructor
        static ContentHolder()
        {
            lockTypeIds = new object();
            sTypeIds = new Dictionary<Type, Guid>();
        }
        #endregion // Static constructor

        #region Declarations
        /// <summary>
        /// This is the core data to the persisted.
        /// </summary>
        protected T mData;
        #endregion
        #region Constructor
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        public ContentHolder()
        {
            if (!IsSerializable())
                throw new ArgumentOutOfRangeException(
                    string.Format("The object type \"{0}\" must implements ISerializable or support the Serializable Attribute",
                        typeof(T).Name));
        }
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        /// <param name="data">The internal data</param>
        public ContentHolder(T data):this()
        {
            Value = data;
        }
        #endregion

        #region Value
        /// <summary>
        /// This method holds the internal value.
        /// </summary>
        public virtual T Value
        {
            get
            {
                return mData;
            }
            protected set
            {
                mData = value;
                IDUpdate(value);
            }
        }
        #endregion 

        #region GetContentTypeAttributeID()
        /// <summary>
        /// This method returns the content Type ID for the current content.
        /// </summary>
        /// <returns>Returns the content type ID.</returns>
        public override Guid GetContentTypeAttributeID()
        {
            KeyValuePair<Guid?, Guid?> keyPair = GetContentTypeAttributeKeyPair(GetType());
            if (keyPair.Key.HasValue)
                return keyPair.Key.Value;
            else
            {
                Type thisType = GetType();
                if (sTypeIds.ContainsKey(thisType))
                    return sTypeIds[thisType];

                lock (lockTypeIds)
                {
                    if (sTypeIds.ContainsKey(thisType))
                        return sTypeIds[thisType];

                    Guid typeID = StringConvertToGuid(typeof(T).FullName);
                    sTypeIds.Add(thisType, typeID);

                    return typeID;
                }
            }
        }
        #endregion

        #region IDUpdate(T data)
        /// <summary>
        /// This method constructs content and version IDs for the entity.
        /// </summary>
        /// <param name="data">The object to construct the IDs</param>
        protected virtual void IDUpdate(T data)
        {
            if (data is IXimuraContent)
            {
                IDContent = ((IXimuraContent)data).IDContent;
                IDVersion = ((IXimuraContent)data).IDVersion;
            }
            else
            {
                IDContent = IDContentConvert();
                IDVersion = IDVersionConvert();
            }
        }
        #endregion // IDUpdate(T data)

        #region IDContentConvert()
        /// <summary>
        /// This method converts the incoming data in to a Guid based upon the IDType and the hashcode of the object.
        /// </summary>
        /// <returns>Returns a GUID representation of the payload data.</returns>
        protected virtual Guid IDContentConvert()
        {
            int code = mData.GetHashCode();
            return StringConvertToGuid(string.Format("{0}:{1}", IDType, code));
        } 
        #endregion
        #region IDVersionConvert()
        /// <summary>
        /// This method currently returns the same value as the IDContent. You should override
        /// this value if you require a more finegrained approach.
        /// </summary>
        /// <returns>Returns a GUID representation of the payload data.</returns>
        protected virtual Guid IDVersionConvert()
        {
            return IDContent;
        } 
        #endregion


    }
}
