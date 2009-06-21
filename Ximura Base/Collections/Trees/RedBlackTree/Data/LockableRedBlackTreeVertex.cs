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
namespace Ximura.Collections
{
    /// <summary>
    /// This class is the vertex that contains both the key and value data within the tree.
    /// </summary>
    /// <typeparam name="TKey">The vertex key type.</typeparam>
    /// <typeparam name="TVal">The vertex value type.</typeparam>
    public class LockableRedBlackTreeVertex<TKey, TVal> : LockableTreeVertexBase<TKey, TVal>
    {
        #region Constructor
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        public LockableRedBlackTreeVertex():base()
        {
            Parent = null;
            Left = null;
            Right = null;

            IsRed = true;
        }
        #endregion // BalancedBinaryVertex

        #region Parent
        /// <summary>
        /// This is the parent vertex.
        /// </summary>
        public LockableRedBlackTreeVertex<TKey, TVal> Parent { get; set; }
        #endregion // Parent
        #region Left
        /// <summary>
        /// This is the left vertex.
        /// </summary>
        public LockableRedBlackTreeVertex<TKey, TVal> Left { get; set; }
        #endregion // Left
        #region Right
        /// <summary>
        /// This is the right vertex.
        /// </summary>
        public LockableRedBlackTreeVertex<TKey, TVal> Right { get; set; }
        #endregion // Right

        #region IsRed
        /// <summary>
        /// This is the state of the vertex, either black or red.
        /// </summary>
        public bool IsRed { get; set; }
        #endregion

        public bool IsBlack { get { return !IsRed; } set { IsRed = !value; } }

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
        public bool IsSentinel { get { return Left == null && Right == null; } }
        #endregion
        #region IsLeaf
        /// <summary>
        /// This property determines whether the vertex is a sentinel, i.e. a vertex without any child vertexes.
        /// </summary>
        public bool IsLeaf { get { return !IsSentinel; } }
        #endregion

        #region ToString()
        private string RB(bool isRed)
        {
            return isRed ? "R" : "B";
        }
        /// <summary>
        /// This override provides useful debug information.
        /// </summary>
        /// <returns>Returns a string representation of the vertex data.</returns>
        public override string ToString()
        {
            return string.Format("{0}K={1} V={2} |{3}| L:{4}{5} R:{6}{7}"
                , IsRoot?"ROOT ":"CHLD "
                , Key
                , Value
                , RB(IsRed)
                , Left == null ? "NULL" : "K=" + Left.Key.ToString()
                , Left == null ? "" : RB(Left.IsRed)
                , Right == null ? "NULL" : "K=" + Right.Key.ToString()
                , Right == null ? "" : RB(Right.IsRed));
        }
        #endregion
    }
}