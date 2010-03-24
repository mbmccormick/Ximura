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

using Ximura.Collections;
using Ximura.Collections.Data;
#endregion // using
namespace Ximura.Collections
{
    /// <summary>
    /// This structture holds the array data for the red black tree.
    /// </summary>
    /// <typeparam name="K">The key type.</typeparam>
    /// <typeparam name="U">The valuw type.</typeparam>
    [Serializable, StructLayout(LayoutKind.Sequential)]
    public struct RedBlackTreeVertexStruct<K, V>
    {
        #region Constructor
        /// <summary>
        /// This constructor sets the key and value data.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public RedBlackTreeVertexStruct(K key, V value)
        {
            Key = key;
            Value = value;
            ParentIDPlus1 = 0;
            LeftIDPlus1 = 0;
            RightIDPlus1 = 0;
            IsBlack = true;
        }
        #endregion // Constructor

        /// <summary>
        /// The key data.
        /// </summary>
        public K Key;
        /// <summary>
        /// The value data.
        /// </summary>
        public V Value;

        /// <summary>
        /// The parent ID plus 1. 0 denotes an empty value.
        /// </summary>
        public int ParentIDPlus1;
        /// <summary>
        /// The left ID plus 1. 0 denotes an empty value.
        /// </summary>
        public int LeftIDPlus1;
        /// <summary>
        /// The right ID plus 1. 0 denotes an empty value.
        /// </summary>
        public int RightIDPlus1;
        /// <summary>
        /// This boolean value denotes whether the vertex is black.
        /// </summary>
        public bool IsBlack;

        #region IsRoot
        /// <summary>
        /// This property determines whether the vertex is the root vertex, i.e. it does not have a parent.
        /// </summary>
        public bool IsRoot { get { return ParentIDPlus1 == 0; } }
        #endregion
        #region IsSentinel
        /// <summary>
        /// This property determines whether the vertex is a sentinel, i.e. a vertex without any child vertexes.
        /// </summary>
        public bool IsSentinel { get { return LeftIDPlus1 == 0 && RightIDPlus1 == 0; } }
        #endregion
    }
}
