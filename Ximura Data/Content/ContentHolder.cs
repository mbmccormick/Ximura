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
    public class ContentHolder<T>: Content
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
            Payload = data;
        }
        #endregion
        #region Deserialization Constructor
        /// <summary>
        /// This is the deserialization constructor. 
        /// </summary>
        /// <param name="info">The Serialization info object that contains all the relevant data.</param>
        /// <param name="context">The serialization context.</param>
        public ContentHolder(SerializationInfo info, StreamingContext context)
            : this()
        {
            DeserializeIncoming(info, context);
        }
        #endregion

        #region Payload
        /// <summary>
        /// This property holds the internal payload object.
        /// </summary>
        public virtual T Payload
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
        #region ContentBody
        /// <summary>
        /// This method returns the content holder payload as a serialized blob.
        /// </summary>
        protected override byte[] ContentBody
        {
            get
            {
                if (!IsSerializable())
                    throw new SerializationException(string.Format("{0} is not marked as serializable. You must provide custom serialization support.", typeof(T).Name));

                //BinaryFormatter bf = new BinaryFormatter();
                byte[] blob = null;


                DataContractSerializer dcSerializer = new DataContractSerializer(typeof(T));

                try
                {
                    using (MemoryStream ms = new MemoryStream())
                    {

                        dcSerializer.WriteObject(ms, mData);

                        ms.Flush();
                        blob = ms.ToArray();
                    }

                    return blob;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
        #endregion // ContentBody

        #region GetContentTypeAttributeID()
        /// <summary>
        /// This method returns the content Type ID for the current content.
        /// </summary>
        /// <returns>Returns the content type ID.</returns>
        public override Guid GetContentTypeAttributeID()
        {
            KeyValuePair<Guid?, Guid?> keyPair = GetType().GetContentTypeAttributeKeyPair();
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

        #region IsSerializable(Type objType)
        /// <summary>
        /// This static method checks whether the object type is serializable.
        /// </summary>
        /// <param name="objType">The object type to check.</param>
        /// <returns>Returns true if the object can be natively serialized.</returns>
        public static bool IsSerializable(Type objType)
        {
            return objType.IsSerializable
                || Attribute.IsDefined(objType, typeof(SerializableAttribute))
                || Attribute.IsDefined(objType, typeof(DataContractAttribute));// || ((obj is ISerializable));
        }
        #endregion
        #region IsSerializable()
        /// <summary>
        /// This public property specifies whether the container object type is natively serializable.
        /// </summary>
        /// <returns>Returns true if the template type can be natively serialized.</returns>
        public virtual bool IsSerializable()
        {
            return IsSerializable(typeof(T));
        }
        #endregion

        #region Load(byte[] buffer, int offset, int count)
        /// <summary>
        /// This method initializes the object from the byte biffer.
        /// </summary>
        /// <param name="buffer">The byte buffer.</param>
        /// <param name="offset">The byte buffer start position.</param>
        /// <param name="count">The number of bytes to read from the buffer.</param>
        /// <returns>Returns the number of bytes read from the buffer.</returns>
        public override int Load(byte[] buffer, int offset, int count)
        {
            DataContractSerializer dcSerializer = new DataContractSerializer(typeof(T));

            using (MemoryStream ms = new MemoryStream(buffer, offset, count))
            {
                mData = (T)dcSerializer.ReadObject(ms);
                IDUpdate(mData);

                return (int)ms.Position;
            }
        }
        #endregion 
    }
}
