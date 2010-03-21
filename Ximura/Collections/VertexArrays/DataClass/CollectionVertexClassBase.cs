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
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.Serialization;
using System.Threading;

using Ximura;
using Ximura.Helper;
#endregion // using
namespace Ximura.Collections.Data
{
    /// <summary>
    /// This is the base tree vertex class.
    /// </summary>
    /// <typeparam name="T">The value data type.</typeparam>
    public abstract class CollectionVertexClassBase<T> : LockableBase
    {
        #region Constructor
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        public CollectionVertexClassBase()
        {
            Value = default(T);
        }
        #endregion // BalancedBinaryVertex

        #region Value
        /// <summary>
        /// This is the data stored in the vertex.
        /// </summary>
        public abstract T Value { get; set; }
        #endregion // Value

        #region IsSentinel
        /// <summary>
        /// This property specifies whether the vertex is a sentinel.
        /// </summary>
        public virtual bool IsSentinel { get { return false; } }
        #endregion // IsSentinel
        #region IsTerminator
        /// <summary>
        /// This property specifies whether the vertex is a terminator.
        /// </summary>
        public abstract bool IsTerminator{get;}
        #endregion // IsTerminator
    }
}
