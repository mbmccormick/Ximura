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
using Ximura.Collections;
using Ximura.Collections.Data;
#endregion // using
namespace Ximura.Collections
{
    /// <summary>
    /// This class is the vertex that contains both the key and value data within the tree.
    /// </summary>
    /// <typeparam name="TKey">The vertex key type.</typeparam>
    /// <typeparam name="TVal">The vertex value type.</typeparam>
    public abstract class TreeLockableVertex<TVal> : CollectionVertexClassBase<TVal>
    {
        #region Constructor
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        public TreeLockableVertex()
            : base()
        {
            Parent = null;
            Left = null;
            Right = null;
        }
        #endregion

        #region Parent
        /// <summary>
        /// This is the parent vertex.
        /// </summary>
        public TreeLockableVertex<TVal> Parent { get; set; }
        #endregion // Parent
        #region Left
        /// <summary>
        /// This is the left vertex.
        /// </summary>
        public TreeLockableVertex<TVal> Left { get; set; }
        #endregion // Left
        #region Right
        /// <summary>
        /// This is the right vertex.
        /// </summary>
        public TreeLockableVertex<TVal> Right { get; set; }
        #endregion // Right

        #region IsRoot
        /// <summary>
        /// This property determines whether the vertex is the root vertex, i.e. it does not have a parent.
        /// </summary>
        public bool IsRoot { get { return Parent == null; } }
        #endregion
        #region IsSentinel
        /// <summary>
        /// This property determines whether the vertex is a sentinel, i.e. a vertex without any child vertexes.
        /// </summary>
        public override bool IsSentinel { get { return Left == null && Right == null; } }
        #endregion
        #region IsTerminator
        /// <summary>
        /// This property determines whether the vertex is a sentinel, i.e. a vertex without any child vertexes.
        /// </summary>
        public override bool IsTerminator { get { return !IsSentinel; } }
        #endregion
    }
}