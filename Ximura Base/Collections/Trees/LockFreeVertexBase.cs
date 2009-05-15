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
namespace Ximura.Collections
{
    /// <summary>
    /// This is the base tree vertex class.
    /// </summary>
    /// <typeparam name="TKey">The key type.</typeparam>
    /// <typeparam name="TVal">The value type.</typeparam>
    public class LockFreeVertexBase<TKey, TVal> : LockableBase
    {
        #region Constructor
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        public LockFreeVertexBase()
        {
            Key = default(TKey);
            Value = default(TVal);
        }
        #endregion // BalancedBinaryVertex

        #region Key
        /// <summary>
        /// This is the vertex key used for partitioning.
        /// </summary>
        public TKey Key { get; set; }
        #endregion // Key
        #region Value
        /// <summary>
        /// This is the value encapsulated by the vertex.
        /// </summary>
        public TVal Value { get; set; }
        #endregion // Value
    }
}
