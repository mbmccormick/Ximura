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
    /// The content holder is used to manage non-standard entities within the persistence framework.
    /// </summary>
    /// <typeparam name="T">The core entity type.</typeparam>
    public class ContentHolder<T>: ContentBase
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
            set
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
                int code = mData.GetHashCode();
                IDContent = StringConvertToGuid(string.Format("{0}:{1}", IDType, code));
                IDVersion = IDContent;
            }
        }
        #endregion // IDUpdate(T data)

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

        #region OnDeserialization(object sender)
        /// <summary>
        /// This is deserialization callback method , and deserializes the payload entity.
        /// </summary>
        /// <param name="sender">The sender.</param>
        public override void OnDeserialization(object sender)
        {
            try
            {
                if (this.mInfo.GetInt32("bodycount") > 0)
                {
                    bool contentDirty = this.mInfo.GetBoolean("dirty");
                    byte[] blob = (byte[])this.mInfo.GetValue("body0", typeof(byte[]));

                    DataContractSerializer dcSerializer = new DataContractSerializer(typeof(T));

                    using (MemoryStream ms = new MemoryStream(blob))
                    {
                        mData = (T)dcSerializer.ReadObject(ms);
                        IDUpdate(mData);
                    } 
                }
            }
            catch (Exception ex)
            {
                throw ex;
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

                DataContractSerializer dcSerializer = new DataContractSerializer(typeof(T));
                try
                {
                    byte[] blob = null;

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
    }
}
