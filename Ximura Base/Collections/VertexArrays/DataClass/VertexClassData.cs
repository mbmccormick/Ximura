#region Copyright
// *******************************************************************************
// Copyright (c) 2000-2009 Paul Stancer.
// All rights reserved. This program and the accompanying materials
// are made available under the terms of the Eclipse Public License v1.0
// which accompanies this distribution, and is available at
// http://www.eclipse.org/legal/epl-v10.html
//
// For more details see http://ximura.org
//
// Contributors:
//     Paul Stancer - initial implementation
// *******************************************************************************
#endregion
#region using
using System;
using System.Linq;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using System.Runtime.Serialization;
using System.Runtime.InteropServices;
using System.Text;

using Ximura;
using Ximura.Helper;
#endregion // using
namespace Ximura.Collections.Data
{
    /// <summary>
    /// This vertex class holds the collection data.
    /// </summary>
    /// <typeparam name="T">The data type.</typeparam>
    public class CollectionVertexClassData<T> : CollectionVertexClass<T>
    {
        #region Constructor
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        public CollectionVertexClassData():base()
        {
            Value = default(T);
        }

        /// <summary>
        /// This is the default constructor.
        /// </summary>
        public CollectionVertexClassData(T item, int hashID):base()
        {
            Value = item;
            HashID = hashID;
        }
        #endregion

        #region Value
        /// <summary>
        /// This is the data for the vertex.
        /// </summary>
        public override T Value
        {
            get;
            set;
        }
        #endregion // Value

        #region IsSentinel
        /// <summary>
        /// This property specifies that the data item is a sentinel.
        /// </summary>
        public override bool IsSentinel { get { return false; } }
        #endregion // IsSentinel

        #region IDisposable Members
        /// <summary>
        /// This method removes and object references.
        /// </summary>
        public override void Dispose()
        {
            if (typeof(T).IsClass)
                Value = default(T);

            base.Dispose();
        }
        #endregion
    }
}
