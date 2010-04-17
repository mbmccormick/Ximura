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
    public partial class ContentHolder<T> : ContentBase
    {
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
    
        protected override void BodyDataProcess(byte[] blob)
        {
            DataContractSerializer dcSerializer = new DataContractSerializer(typeof(T));

            using (MemoryStream ms = new MemoryStream(blob))
            {
                mData = (T)dcSerializer.ReadObject(ms);
                IDUpdate(mData);
            }
        }
    }
}
