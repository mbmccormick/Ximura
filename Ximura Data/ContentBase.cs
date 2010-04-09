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

        #region Static Declarations
        private static object syncIDCollection = new object();
        private static Dictionary<Type, KeyValuePair<Guid, Guid?>> sTypeGuidDictionary;
        #endregion // Static Declarations
        #region Static Constructor
        static ContentBase()
        {
            sTypeGuidDictionary = new Dictionary<Type, KeyValuePair<Guid, Guid?>>();
        }
        #endregion // Static Constructor

        #region GetContentTypeAttributeID()
        /// <summary>
        /// This method returns the content Type ID for the current content.
        /// </summary>
        /// <returns>Returns the content type ID.</returns>
        public Guid GetContentTypeAttributeID()
        {
            return GetContentTypeAttributeKeyPair(GetType()).Key;
        }
        #endregion
        #region GetContentTypeAttributeID(Type itemType)
        /// <summary>
        /// This method returns the content Type ID from the itemType.
        /// </summary>
        /// <param name="itemType">The content type.</param>
        /// <returns>Returns the content type ID.</returns>
        public static Guid GetContentTypeAttributeID(Type itemType)
        {
            return GetContentTypeAttributeKeyPair(itemType).Key;
        }
        #endregion // GetContentTypeID(Type itemType)
        #region GetContentTypeAttributeID<T>()
        /// <summary>
        /// This method returns the content Type ID as defined in the XimuraContentTypeIDAttribute.
        /// </summary>
        /// <typeparam name="T">The Content type.</typeparam>
        /// <returns>Returns a Guid containing the content type.</returns>
        public static Guid GetContentTypeAttributeID<T>() where T : Content
        {
            return GetContentTypeAttributeKeyPairInternal(typeof(T)).Key;
        }
        #endregion // GetContentTypeAttributeID<T>()

        #region GetContentTypeAttributeKeyPair(Type itemType)
        /// <summary>
        /// This method returns both the content Type ID and content ID as defined 
        /// in the XimuraContentTypeIDAttribute and XimuraContentIDAttribute.
        /// </summary>
        /// <param name="itemType"></param>
        /// <returns></returns>
        public static KeyValuePair<Guid, Guid?> GetContentTypeAttributeKeyPair(Type itemType)
        {
            if (itemType == null)
                throw new ArgumentNullException("itemType cannot be null.");

            if (!itemType.IsSubclassOf(typeof(Content)))
                throw new ArgumentOutOfRangeException("itemType must derive from Content");

            return GetContentTypeAttributeKeyPairInternal(itemType);
        }
        #endregion // GetContentTypeAttributeKeyPair(Type itemType)
        #region GetContentTypeAttributeKeyPairInternal(Type itemType)
        /// <summary>
        /// 
        /// </summary>
        /// <param name="itemType">The object type.</param>
        /// <returns>Returns the keyvalue pair.</returns>
        private static KeyValuePair<Guid, Guid?> GetContentTypeAttributeKeyPairInternal(Type itemType)
        {
            if (itemType == null)
                throw new ArgumentNullException("itemType cannot be null.");

            if (sTypeGuidDictionary.ContainsKey(itemType))
                return sTypeGuidDictionary[itemType];

            lock (syncIDCollection)
            {
                if (sTypeGuidDictionary.ContainsKey(itemType))
                    return sTypeGuidDictionary[itemType];

                XimuraContentTypeIDAttribute contentTypeIDAttr =
                    AH.GetAttribute<XimuraContentTypeIDAttribute>(itemType);

                if (contentTypeIDAttr == null)
                    throw new ArgumentException("The content type does not have a XimuraContentTypeID attribute defined.");

                XimuraContentIDAttribute contentIDAttr =
                    AH.GetAttribute<XimuraContentIDAttribute>(itemType);

                KeyValuePair<Guid, Guid?> newPair =
                    new KeyValuePair<Guid, Guid?>(contentTypeIDAttr.ID,
                    contentIDAttr == null ? (Guid?)null : (Guid?)contentIDAttr.ID);

                sTypeGuidDictionary.Add(itemType, newPair);

                return newPair;
            }
        }
        #endregion

        #region GetContentAttributeID()
        /// <summary>
        /// This method returns the content ID for the current content
        /// </summary>
        /// <returns>Returns the content type ID, or null if no attribute is defined.</returns>
        public Guid? GetContentAttributeID()
        {
            return GetContentTypeAttributeKeyPair(GetType()).Value;
        }
        #endregion // GetContentTypeID(Type itemType)
        #region GetContentAttributeID(Type itemType)
        /// <summary>
        /// This method returns the content Type ID from the itemType.
        /// </summary>
        /// <param name="itemType">The content type.</param>
        /// <returns>Returns the content type ID, or null if no attribute is defined.</returns>
        public static Guid? GetContentAttributeID(Type itemType)
        {
            return GetContentTypeAttributeKeyPair(itemType).Value;
        }
        #endregion // GetContentTypeID(Type itemType)
        #region GetContentAttributeID<T>()
        /// <summary>
        /// This method returns the content ID as defined in the XimuraContentIDAttribute.
        /// </summary>
        /// <typeparam name="T">The Content type.</typeparam>
        /// <returns>Returns a Guid or null if the attribute is not defined.</returns>
        public static Guid? GetContentAttributeID<T>() where T : Content
        {
            return GetContentTypeAttributeKeyPairInternal(typeof(T)).Value;
        }
        #endregion // GetContentAttributeID<T>()

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
    }
}
