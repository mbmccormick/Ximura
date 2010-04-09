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
        where T : class
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
        /// <param name="data">The internal data</param>
        public ContentHolder(T data)
        {
            if (!IsSerializable(data))
                throw new ArgumentOutOfRangeException(
                    string.Format("The object type \"{0}\" must implements ISerializable or support the Serializable Attribute", 
                        typeof(T).Name));

            mData = data;
        }
        #endregion 

        public static bool IsSerializable(T obj)
        {
            return typeof(T).IsSerializable || ((obj is ISerializable) || (Attribute.IsDefined(typeof(T), typeof(SerializableAttribute))));
        }

        #region ICloneable Members

        public override object Clone()
        {
            throw new NotImplementedException();
        }

        #endregion

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
