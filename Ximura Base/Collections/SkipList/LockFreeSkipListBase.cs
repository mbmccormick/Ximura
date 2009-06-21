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
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Threading;

using Ximura;
using Ximura.Helper;
#endregion // using
namespace Ximura.Collections
{
    /// <summary>
    /// The LockFreeSkipListBase class provides generic multi-threaded collection based functionality. The class is designed
    /// to maximize the throughput of the collection in high speed multi-threaded scenarios.
    /// </summary>
    /// <typeparam name="T">The collection class or structure type.</typeparam>
    public abstract partial class LockFreeSkipListBase<T> : CollectionHelperStructBase<T>
    {
        #region Constructor
        /// <summary>
        /// This is constructor for the abstract list class.
        /// </summary>
        /// <param name="comparer">The comparer for the collection items.</param>
        /// <param name="capacity">The initial capacity for the collection.</param>
        /// <param name="collection">The initial data to load to the collection.</param>
        /// <param name="isFixedSize">This property determines whether the collection is a fixed size.
        /// Fixed size collections will reject new records when the capacity has been reached, 
        /// although they may deliver performance improvements as they do not need to use a growable data structure.</param>
        protected LockFreeSkipListBase(IEqualityComparer<T> comparer, int capacity, IEnumerable<T> collection, bool isFixedSize)
        {
            Initialize(comparer, capacity, collection, isFixedSize);
        }
        #endregion // Constructor

        #region CollectionAllowNullValues
        /// <summary>
        /// This property determines whether the collection will accept null values. The default setting is true.
        /// </summary>
        /// <remarks>This property is ignored if the collection is for a value type such as int.</remarks>
        protected override bool CollectionAllowNullValues { get { return true; } }
        #endregion

        #region InitializeData()
        /// <summary>
        /// This method initializes the data collection.
        /// </summary>
        protected override void InitializeData()
        {
            mData = new SkipListVertexArray<T>(mState.IsFixedSize, mState.InitialCapacity, 0.5D, 16);
        }
        #endregion // InitializeData()
    }
}