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

        public override Guid GetContentTypeAttributeID()
        {
            return base.GetContentTypeAttributeID();
        }

        protected virtual void IDUpdate(T data)
        {

        }

        public static bool IsSerializable(Type objType)
        {
            return objType.IsSerializable || Attribute.IsDefined(objType, typeof(SerializableAttribute));// || ((obj is ISerializable));
        }

        public virtual bool IsSerializable()
        {
            return IsSerializable(typeof(T));
        }


        public override void OnDeserialization(object sender)
        {
            throw new NotImplementedException();
        }

        protected override byte[] ContentBody
        {
            get { throw new NotImplementedException(); }
        }
    }
}
