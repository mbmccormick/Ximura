﻿#region Copyright
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

using Ximura;
using Ximura.Helper;
#endregion // using
namespace Ximura.Collections
{
    /// <summary>
    /// The LockFreeCollectionBase class provides generic multi-threaded collection based functionality. The class is designed
    /// to maximize the throughput of the collection in high speed multi-threaded scenarios.
    /// </summary>
    /// <typeparam name="T">The collection class or structure type.</typeparam>
    public abstract partial class LockFreeCollectionBase<T> : CollectionHelperStructBase<T>
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
        protected LockFreeCollectionBase(IEqualityComparer<T> comparer, int capacity, IEnumerable<T> collection, bool isFixedSize)
        {
#if (PROFILING)
            ProfilingSetup();
#endif
            Initialize(comparer, capacity, collection, isFixedSize);
        }
        #endregion // Constructor
        #region Dispose(bool disposing)
        /// <summary>
        /// This method disposes of the data in the collection. You should override this method if you need to add
        /// custom dispose logic to your collection.
        /// </summary>
        /// <param name="disposing">The class is disposing, i.e. this is called by Dispose and not the finalizer.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                //Clear the collection. This removes all references to any contained objects.
                ClearInternal();
                mData = null;
            }
        }
        #endregion // Dispose(bool disposing)

        #region CollectionAllowMultipleEntries
        /// <summary>
        /// This setting determines whether the collection allows multiple entries of the same object in the collection.
        /// The default setting is true.
        /// </summary>
        protected override bool CollectionAllowMultipleEntries{get{return false;}}
        #endregion
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
            mData = new CombinedVertexArray<T>(mState.IsFixedSize, mState.InitialCapacity);
        }
        #endregion // InitializeData()
    }
}
