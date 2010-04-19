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
using System.Data;
using System.Data.SqlClient;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.IO;
using System.Text;
using System.Runtime.Serialization;

using Ximura;
using Ximura.Data;
using Ximura.Framework;
using CH = Ximura.Common;
using RH = Ximura.Reflection;
using AH = Ximura.AttributeHelper;
#endregion // using
namespace Ximura.Data
{
    /// <summary>
    /// This class provides extension methods for classes that implement IXimuraContent.
    /// </summary>
    public static partial class ContentHelper
    {
        #region Static Declarations
        private static object syncIDCollection = new object();
        private static Dictionary<Type, KeyValuePair<Guid?, Guid?>> sTypeGuidDictionary;
        #endregion // Static Declarations
        #region Static Constructor
        static ContentHelper()
        {
            sTypeGuidDictionary = new Dictionary<Type, KeyValuePair<Guid?, Guid?>>();
        }
        #endregion // Static Constructor

        #region GetContentTypeAttributeID(Type itemType)
        /// <summary>
        /// This method returns the content Type ID from the itemType.
        /// </summary>
        /// <param name="itemType">The content type.</param>
        /// <returns>Returns the content type ID.</returns>
        public static Guid GetContentTypeAttributeID(this Type itemType)
        {
            Guid? key = GetContentTypeAttributeKeyPair(itemType).Key;
            if (!key.HasValue)
                throw new ArgumentException("The content type does not have a XimuraContentTypeID attribute defined.");

            return key.Value;
        }
        #endregion // GetContentTypeID(Type itemType)
        #region GetContentTypeAttributeID<T>()
        /// <summary>
        /// This method returns the content Type ID as defined in the XimuraContentTypeIDAttribute.
        /// </summary>
        /// <typeparam name="T">The Content type.</typeparam>
        /// <returns>Returns a Guid containing the content type.</returns>
        public static Guid GetContentTypeAttributeID<T>() where T : IXimuraContent
        {
            return GetContentTypeAttributeID(typeof(T));
        }
        #endregion // GetContentTypeAttributeID<T>()

        #region GetContentTypeAttributeKeyPair(Type itemType)
        /// <summary>
        /// This method returns both the content Type ID and content ID as defined 
        /// in the XimuraContentTypeIDAttribute and XimuraContentIDAttribute.
        /// </summary>
        /// <param name="itemType"></param>
        /// <returns></returns>
        public static KeyValuePair<Guid?, Guid?> GetContentTypeAttributeKeyPair(this Type itemType)
        {
            if (itemType == null)
                throw new ArgumentNullException("itemType cannot be null.");

            if (!RH.ValidateInterface(itemType, typeof(IXimuraContent)))
                throw new ArgumentOutOfRangeException("itemType must implement IXimuraContent");
            
            return GetContentTypeAttributeKeyPairInternal(itemType);
        }
        #endregion // GetContentTypeAttributeKeyPair(Type itemType)
        #region GetContentTypeAttributeKeyPairInternal(Type itemType)
        /// <summary>
        /// 
        /// </summary>
        /// <param name="itemType">The object type.</param>
        /// <returns>Returns the keyvalue pair.</returns>
        private static KeyValuePair<Guid?, Guid?> GetContentTypeAttributeKeyPairInternal(Type itemType)
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

                XimuraContentIDAttribute contentIDAttr =
                    AH.GetAttribute<XimuraContentIDAttribute>(itemType);

                KeyValuePair<Guid?, Guid?> newPair =
                    new KeyValuePair<Guid?, Guid?>(
                     contentTypeIDAttr == null ? (Guid?)null : (Guid?)contentTypeIDAttr.ID
                    ,contentIDAttr == null ? (Guid?)null : (Guid?)contentIDAttr.ID
                    );

                sTypeGuidDictionary.Add(itemType, newPair);

                return newPair;
            }
        }
        #endregion


        #region GetContentAttributeID(Type itemType)
        /// <summary>
        /// This method returns the content Type ID from the itemType.
        /// </summary>
        /// <param name="itemType">The content type.</param>
        /// <returns>Returns the content type ID, or null if no attribute is defined.</returns>
        public static Guid? GetContentAttributeID(this Type itemType)
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
        public static Guid? GetContentAttributeID<T>() where T : IXimuraContent
        {
            return GetContentTypeAttributeKeyPairInternal(typeof(T)).Value;
        }
        #endregion // GetContentAttributeID<T>()
    }
}
