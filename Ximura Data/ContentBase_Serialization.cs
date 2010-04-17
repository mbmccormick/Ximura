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
using RH = Ximura.Reflection;
#endregion // using
namespace Ximura.Data
{
    public abstract partial class ContentBase : ISerializable, IDeserializationCallback
    {
        #region Declarations
        /// <summary>
        /// This is the deserialization information
        /// </summary>
        protected SerializationInfo mInfo = null;
        #endregion // Declarations

        #region Serialization Constructor
        /// <summary>
        /// This is the deserialization constructor. 
        /// </summary>
        /// <param name="info">The Serialization info object that contains all the relevant data.</param>
        /// <param name="context">The serialization context.</param>
        public ContentBase(SerializationInfo info, StreamingContext context)
        {
            DeserializeIncoming(info, context);
        }
        #endregion
        #region DeserializeIncoming(SerializationInfo info, StreamingContext context)
        /// <summary>
        /// This protected method can be overriden by inherited data classes to handle the deserialization
        /// process without the need to add code within the constructor method.
        /// </summary>
        /// <param name="info">The Serialization info object that contains all the relevant data.</param>
        /// <param name="context">The serialization context.</param>
        protected virtual void DeserializeIncoming(SerializationInfo info, StreamingContext context)
        {
            mInfo = info;
        }
        #endregion

        #region OnDeserialization(object sender)
        /// <summary>
        /// This method is called to complete the deserialization.
        /// </summary>
        /// <param name="sender">The sender object.</param>
        public virtual void OnDeserialization(object sender)
        {
            this.IDContent = (Guid)mInfo.GetValue("cid", typeof(Guid));
            this.IDVersion = (Guid)mInfo.GetValue("vid", typeof(Guid));
            if (this.IDType != (Guid)mInfo.GetValue("tid", typeof(Guid)))
                throw new SerializationException("The type ID is the wrong value: " + ((Guid)mInfo.GetValue("tid", typeof(Guid))).ToString());

            BodyDataSet();
        }
        #endregion
        #region BodyDataSet()
        /// <summary>
        /// This method strips out the binary body data from the serialization info.
        /// </summary>
        protected virtual void BodyDataSet()
        {
            if (this.mInfo.GetInt32("bodycount") > 0)
            {
                byte[] blob = (byte[])this.mInfo.GetValue("body0", typeof(byte[]));

                BodyDataProcess(blob);
            }
        }
        #endregion

        #region Abstract -> BodyDataProcess(byte[] blob)
        /// <summary>
        /// This abstract method is used to process the body data.
        /// </summary>
        /// <param name="blob">The binary data that should be processed.</param>
        protected abstract void BodyDataProcess(byte[] blob);
        #endregion 

        #region GetObjectData(SerializationInfo info, StreamingContext context)
        /// <summary>
        /// This code adds each of the content components to the serialization object. 
        /// This function should be overriden by derived objects that have specific serialization
        /// requirements.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
                throw new ArgumentNullException("info", "info cannot be null");

            info.AddValue("cid", this.IDContent);
            info.AddValue("vid", this.IDVersion);
            info.AddValue("tid", this.IDType);

            BodyDataAdd(info, context);
        }
        #endregion
        #region BodyDataAdd(SerializationInfo info, StreamingContext context)
        /// <summary>
        /// This is the default constructor. It does not include any content.
        /// </summary>
        /// <param name="info">The Serialization info.</param>
        /// <param name="context">The serialization context.</param>
        protected virtual void BodyDataAdd(SerializationInfo info, StreamingContext context)
        {
            //By default we do not include any body.
            byte[] body = ContentBody;
            info.AddValue("bodycount", (int)(body != null ? 1 : 0));
            if (body != null)
                info.AddValue("body0", body);
            return;
        }
        #endregion

        #region Abstract --> ContentBody
        /// <summary>
        /// This property gets the content body as a byte array.
        /// </summary>
        protected abstract byte[] ContentBody { get; }
        #endregion // ContentBody

        #region ToArray()
        /// <summary>
        /// This method returns the content body as a byte array.
        /// </summary>
        /// <returns>Returns the content as a byte array.</returns>
        public virtual byte[] ToArray()
        {
            return ContentBody;
        }
        #endregion

        #region ICloneable Members -> Clone()
        /// <summary>
        /// This method clones the current object in to a new object from the same
        /// object pool. If the current object is not a member of an object pool then
        /// a new object is created.
        /// 
        /// This method serializes and then deserializes the object to provide a copy.
        /// You can override this method to provide more efficient cloning for custom object.
        /// </summary>
        /// <returns>Returns a copy of the current object.</returns>
        public virtual object Clone()
        {
            try
            {
                SerializationInfo info = new SerializationInfo(GetType(), new FormatterConverter());
                StreamingContext context = new StreamingContext(StreamingContextStates.Clone);

                this.GetObjectData(info, context);

                ContentBase clone = (ContentBase)RH.CreateObjectFromType(GetType(), new object[] { info, context });

                clone.OnDeserialization(null);

                return clone;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion
    }
}
