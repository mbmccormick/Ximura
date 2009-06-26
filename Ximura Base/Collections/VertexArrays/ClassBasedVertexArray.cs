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
namespace Ximura.Collections
{
    /// <summary>
    /// This is the base class for class based data arrays.
    /// </summary>
    /// <typeparam name="T">The data type.</typeparam>
    public abstract class ClassBasedVertexArray<T> : VertexArray<T>
    {
        #region Struct -> VertexWindow<T>

        #endregion // Struct -> Vertex<T>

        #region Initialize(IEqualityComparer<T> eqComparer, bool isFixedSize, int capacity, bool allowNullValues, bool allowMultipleEntries)
        /// <summary>
        /// This override initializes the collection.
        /// </summary>
        /// <param name="eqComparer">The equality comparer for the collection.</param>
        /// <param name="isFixedSize">Specifies whether the collection is a fixed size.</param>
        /// <param name="capacity">The initial capacity.</param>
        /// <param name="allowNullValues">This boolean values specifies whether null values are allowed in the collection.</param>
        /// <param name="allowMultipleEntries">This boolean value specicifies whether the collection allows items to exist 
        /// more than once in the collection.</param>
        public override void Initialize(IEqualityComparer<T> eqComparer, bool isFixedSize, int capacity, bool allowNullValues, bool allowMultipleEntries)
        {
            base.Initialize(eqComparer, isFixedSize, capacity, allowNullValues, allowMultipleEntries);

            InitializeData();
        }
        #endregion // Initialize(IEqualityComparer<T> eqComparer, bool isFixedSize, int capacity, bool allowNullValues, bool allowMultipleEntries)
        #region InitializeData()
        /// <summary>
        /// This method initializes the data collection.
        /// </summary>
        protected abstract void InitializeData();
        #endregion // InitializeData()

        #region Root
        /// <summary>
        /// This is the root vertex for the data collection.
        /// </summary>
        protected abstract VertexClassBase<T> Root { get; }
        #endregion

        #region GetSentinelID(int hashCode, bool createSentinel, out int hashID)
        /// <summary>
        /// This method is responsible for implementing the sentinel collection for fast lookup of data.
        /// </summary>
        /// <param name="hashCode"></param>
        /// <param name="createSentinel"></param>
        /// <param name="hashID"></param>
        /// <returns></returns>
        protected abstract VertexClassBase<T> GetSentinelID(int hashCode, bool createSentinel, out int hashID);
        #endregion // GetSentinelID(int hashCode, bool createSentinel, out int hashID)

        #region VertexWindowGet()
        /// <summary>
        /// This method returns a vertex window for the first item in the array.
        /// </summary>
        /// <returns>A vertex window.</returns>
        public override IVertexWindow<T> VertexWindowGet()
        {
            return new ClassBasedVertexWindow<T>(this, Root, mEqComparer, 0, default(T));
        }
        #endregion // VertexWindowGet()
        #region VertexWindowGet(T item, bool createSentinel)
        /// <summary>
        /// This method returns a vertex window for the item specified.
        /// </summary>
        /// <param name="item">The item that requires a search window.</param>
        /// <param name="createSentinel">The value specifies whether any missing sentinels should be created.</param>
        /// <returns>A vertex window.</returns>
        public override IVertexWindow<T> VertexWindowGet(T item, bool createSentinel)
        {
            int hashCode = mEqComparer.GetHashCode(item);
            int hashID;
            VertexClassBase<T> vertex = GetSentinelID(mEqComparer.GetHashCode(item), createSentinel, out hashID);

            //Ok, set the MSB to indicate the value is a sentinel.
            return new ClassBasedVertexWindow<T>(this, vertex, mEqComparer, hashID, item);
        }
        #endregion // VertexWindowGet(T item, bool createSentinel)

        #region GetEnumerator()
        /// <summary>
        /// This method returns an enumeration through the sentinels and data in the collection.
        /// </summary>
        /// <returns>Returns an enumeration containing the collection data.</returns>
        public override IEnumerator<KeyValuePair<int, ICollectionVertex<T>>> GetEnumerator()
        {
            VertexClassBase<T> item = Root;
            item.LockWait();
            int index = 0;

            yield return new KeyValuePair<int, ICollectionVertex<T>>(index, item);

            while (!item.IsTerminator)
            {
                index++;
                item = item.Next;
                item.LockWait();

                yield return new KeyValuePair<int, ICollectionVertex<T>>(index, item);
            }
        }
        #endregion // GetEnumerator()
    }
}
