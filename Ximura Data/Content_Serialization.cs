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
using Ximura.Data;

using CH = Ximura.Common;
using RH = Ximura.Reflection;
#endregion // using
namespace Ximura.Data
{
    public abstract partial class Content
    {
        #region Serialization Constructor
        /// <summary>
        /// This is the deserialization constructor. 
        /// </summary>
        /// <param name="info">The Serialization info object that contains all the relevant data.</param>
        /// <param name="context">The serialization context.</param>
        public Content(SerializationInfo info, StreamingContext context): this()
        {
            DeserializeIncoming(info, context);
        }
        #endregion

        #region GetObjectData(SerializationInfo info, StreamingContext context)
        /// <summary>
        /// This code adds each of the content components to the serialization object. 
        /// This function should be overriden by derived objects that have specific serialization
        /// requirements.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
                throw new ArgumentNullException("info", "info cannot be null");

            info.AddValue("cid", this.IDContent);
            info.AddValue("vid", this.IDVersion);
            info.AddValue("tid", this.IDType);
            info.AddValue("dirty", this.IsDirty());

            BodyDataAdd(info, context);
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
        public override object Clone()
        {
            try
            {
                //If this ibject is not connected to an object pool, then use the base clone method.
                if (this.ObjectPool == null)
                {
                    return base.Clone();
                }

                SerializationInfo info = new SerializationInfo(GetType(), new FormatterConverter());
                StreamingContext context = new StreamingContext(StreamingContextStates.Clone);

                this.GetObjectData(info, context);

                return ObjectPool.Get(info, context);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region OnDeserialization(object sender)
        /// <summary>
        /// This is deserialization callback method.
        /// </summary>
        /// <param name="sender">The sender.</param>
        public override void OnDeserialization(object sender)
        {
            base.OnDeserialization(sender);

            bool contentDirty = this.mInfo.GetBoolean("dirty");
            this.Dirty = contentDirty;
        }
        #endregion
    }
}
