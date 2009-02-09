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
using RH = Ximura.Helper.Reflection;
using AH = Ximura.Helper.AttributeHelper;
#endregion // using
namespace Ximura.Data
{
    public abstract partial class Content //: MarshalByValueComponent,
        //IXimuraContent, IEquatable<IXimuraContent>, IXimuraMessageLoadData,
        //IXimuraMessageLoad, ISupportInitialize, IXimuraContentEntityFragment, IXimuraPoolManagerDirectAccess
    {
        #region Static Declarations
        private static object syncIDCollection = new object();
        private static Dictionary<Type, KeyValuePair<Guid, Guid?>> sTypeGuidDictionary;
        #endregion // Static Declarations
        #region Static Constructor
        static Content()
        {
            sTypeGuidDictionary = new Dictionary<Type, KeyValuePair<Guid,Guid?>>();
        }
        #endregion // Static Constructor

        #region SerializeToStream
        /// <summary>
        /// This method serializes an entity to a stream.
        /// </summary>
        /// <param name="content">The content to serialize.</param>
        /// <param name="stream">The stream to serialize to.</param>
        public static void SerializeToStream(Content content, Stream stream)
        {
            IFormatter formatter = ResolveFormatter(content);
            SerializeToStream(content, stream, formatter);
        }
        /// <summary>
        /// This method serializes an entity to a stream.
        /// </summary>
        /// <param name="content">The content to serialize.</param>
        /// <param name="stream">The stream to serialize to.</param>
        /// <param name="formatter">The formatter to use for the serialization.</param>
        public static void SerializeToStream(Content content, Stream stream, IFormatter formatter)
        {
            formatter.Serialize(stream, content);
        }
        #endregion // SerializeToStream
        #region SerializeEntity
        /// <summary>
        /// This method serializes an entity in to a blob.
        /// </summary>
        /// <param name="content">The content to serialize.</param>
        /// <returns>A byte array containing the serialized entity.</returns>
        public static byte[] SerializeEntity(Content content)
        {
            Type contentType = content.GetType();
            IFormatter formatter = ResolveFormatter(contentType);
            return SerializeEntity(content, formatter);
        }
        /// <summary>
        /// This method serializes an entity in to a blob.
        /// </summary>
        /// <param name="content">The content to serialize.</param>
        /// <param name="formatter">The formatter to serialize the entity with.</param>
        /// <returns>A byte array containing the serialized entity.</returns>
        public static byte[] SerializeEntity(Content content, IFormatter formatter)
        {
            MemoryStream memStream = new MemoryStream();
            SerializeToStream(content, memStream, formatter);

            long length = memStream.Length;
            byte[] returnByte = new byte[length];

            Array.Copy(memStream.GetBuffer(), 0, returnByte, 0, length);

            return returnByte;
        }
        #endregion // SerializeEntity

        #region DeserializeEntity
        /// <summary>
        /// This method deserializes a blob in to the correct content entity.
        /// </summary>
        /// <param name="blob">The binary blob to deserialize.</param>
        /// <returns>A content object</returns>
        public static Content DeserializeEntity(byte[] blob, IXimuraPoolManager pMan)
        {
            return DeserializeEntity(blob, 0, blob.Length, pMan);
        }
        /// <summary>
        /// This method deserializes a blob in to the correct content entity.
        /// </summary>
        /// <param name="blob">The binary blob to deserialize.</param>
        /// <param name="formatter">The formatter to use.</param>
        /// <returns>A content object</returns>
        public static Content DeserializeEntity(byte[] blob, IXimuraFormatter formatter, IXimuraPoolManager pMan)
        {
            return DeserializeEntity(blob, 0, blob.Length, formatter,pMan);
        }
        /// <summary>
        /// This method deserializes a blob in to the correct content entity.
        /// </summary>
        /// <param name="blob">The binary blob to deserialize.</param>
        /// <param name="index">The index.</param>
        /// <param name="length">The length.</param>
        /// <returns>A content object</returns>
        public static Content DeserializeEntity(byte[] blob, int index, int length, IXimuraPoolManager pMan)
        {
            IXimuraFormatter formatter = ResolveFormatter(blob);

            return DeserializeEntity(blob, index, length, formatter, pMan);
        }
        /// <summary>
        /// This method deserializes a blob in to the correct content entity.
        /// </summary>
        /// <param name="blob">The binary blob to deserialize.</param>
        /// <param name="index">The index.</param>
        /// <param name="length">The length.</param>
        /// <param name="formatter">The formatter to use.</param>
        /// <returns>A content object</returns>
        public static Content DeserializeEntity(byte[] blob, int index, int length,
            IXimuraFormatter formatter, IXimuraPoolManager pMan)
        {
            using (MemoryStream memStream = new MemoryStream(blob, index, length))
            {
                return DeserializeEntity(memStream, formatter, pMan);
            }
        }
        /// <summary>
        /// This method deserializes the stream in to the correct content entity.
        /// </summary>
        /// <param name="stream">The stream to deserialize from.</param>
        /// <returns>The content entity.</returns>
        public static Content DeserializeEntity(Stream stream, IXimuraPoolManager pMan)
        {
            IXimuraFormatter formatter = ResolveFormatter(stream);
            return DeserializeEntity(stream, formatter, pMan);
        }
        /// <summary>
        /// This method deserializes the stream in to the correct content entity.
        /// </summary>
        /// <param name="stream">The stream to deserialize from.</param>
        /// <param name="formatter">The formatter.</param>
        /// <param name="pMan">The pool manager to retrieve a new object.</param>
        /// <returns>The content entity.</returns>
        public static Content DeserializeEntity(Stream stream, IXimuraFormatter formatter, IXimuraPoolManager pMan)
        {
            Content content = formatter.Deserialize(stream, pMan) as Content;

            return content;
        }
        #endregion // DeserializeEntity

        #region ResolveFormatter
        /// <summary>
        /// This method resolves the correct formatter for the content and the blob.
        /// </summary>
        /// <param name="contentType"></param>
        /// <returns>Returns the formatter if it can be found.</returns>
        public static IXimuraFormatter ResolveFormatter(Type contentType)
        {
            try
            {
                XimuraContentSerializationAttribute csAttr =
                    AH.GetAttribute<XimuraContentSerializationAttribute>(contentType);

                if (csAttr == null || csAttr.Formatter==null)
                    return new ContentFormatter();

                return csAttr.Formatter;
            }
            catch (Exception ex)
            {
                return new ContentFormatter();
            }
        }
        /// <summary>
        /// This method resolves the formatter for the content.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <returns>Returns the formatter if it can be found.</returns>
        public static IXimuraFormatter ResolveFormatter(Content content)
        {
            Type contentType = content.GetType();
            IXimuraFormatter formatter = ResolveFormatter(contentType);
            return formatter;
        }
        /// <summary>
        /// This method resolves the formatter for the blob.
        /// </summary>
        /// <param name="blob">The blob.</param>
        /// <returns>Returns the formatter if it can be found.</returns>
        public static IXimuraFormatter ResolveFormatter(byte[] blob)
        {
            return new ContentFormatter();
        }
        /// <summary>
        /// This method resolves the formatter for the blob.
        /// </summary>
        /// <param name="stream">The stream to check.</param>
        /// <returns>Returns the formatter if it can be found.</returns>
        public static IXimuraFormatter ResolveFormatter(Stream stream)
        {
            return new ContentFormatter();
        }
        #endregion // ResolveFormatter

        #region GetContentTypeAttributeID(Type itemType)
        /// <summary>
        /// This method returns the content Type ID for the current content.
        /// </summary>
        /// <returns>Returns the content type ID.</returns>
        public Guid GetContentTypeAttributeID()
        {
            return GetContentTypeAttributeKeyPair(GetType()).Key;
        }
        #endregion // GetContentTypeID(Type itemType)
        #region GetContentAttributeID(Type itemType)
        /// <summary>
        /// This method returns the content ID for the current content
        /// </summary>
        /// <returns>Returns the content type ID, or null if no attribute is defined.</returns>
        public Guid? GetContentAttributeID()
        {
            return GetContentTypeAttributeKeyPair(GetType()).Value;
        }
        #endregion // GetContentTypeID(Type itemType)

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
                    contentIDAttr==null?(Guid?)null:(Guid?)contentIDAttr.ID);

                sTypeGuidDictionary.Add(itemType, newPair);

                return newPair;
            }
        }
    }
}
