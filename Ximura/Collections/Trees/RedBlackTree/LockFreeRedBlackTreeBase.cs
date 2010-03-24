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
using System.Text;

using Ximura;

#endregion // using
namespace Ximura.Collections
{
    /// <summary>
    /// This class is a base class for Red-Black Tree implementations.
    /// </summary>
    /// <typeparam name="TKey">The key type.</typeparam>
    /// <typeparam name="TVal">The value type.</typeparam>
    public class LockFreeRedBlackTreeBase<TKey, TVal> : LockFreeRedBlackTreeBase<TKey, TVal, RedBlackTreeLockableVertex<TKey, TVal>>
    {
        #region Constructors
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        public LockFreeRedBlackTreeBase()
            : this(Comparer<TKey>.Default, EqualityComparer<TVal>.Default) { }
        /// <summary>
        /// This constructor requires custom comparers for both the key data and an equality comparer for the value data.
        /// </summary>
        /// <param name="vertexComparer">The key comparer.</param>
        /// <param name="valueEqComparer">The value equality comparer. </param>
        public LockFreeRedBlackTreeBase(Comparer<TKey> keyComparer, EqualityComparer<TVal> valueEqComparer)
            : base(keyComparer, valueEqComparer) { }
        #endregion // Constructors
    }
}
