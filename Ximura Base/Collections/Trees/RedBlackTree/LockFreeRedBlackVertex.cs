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
    /// This class is the vertex that contains both the key and value data within the tree.
    /// </summary>
    /// <typeparam name="TKey">The vertex key type.</typeparam>
    /// <typeparam name="TVal">The vertex value type.</typeparam>
    public abstract class LockFreeRedBlackVertex<TKey, TVal> : LockableBase
    {
        #region Constructor
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        public LockFreeRedBlackVertex()
        {
            Key = default(TKey);
            Value = default(TVal);

            Parent = null;
            Left = null;
            Right = null;

            State = LockFreeRedBlackVertexState.Black;
        }
        #endregion // BalancedBinaryVertex

        #region Abstract --> Comparer(LockFreeRedBlackVertex<TKey, TVal> vertex, TKey key);
        /// <summary>
        /// This abstract method should be overriden to provide the correct compare logic.
        /// </summary>
        /// <param name="vertex">The vertex to compare against.</param>
        /// <param name="key">The key to compare with.</param>
        /// <returns>
        /// Returns 0 if the key is equal to the vertex. 
        /// Returns -1 if the key is less than the vertex, and returns 1 if the key is greater than the vertex.
        /// </returns>
        protected abstract int Comparer(LockFreeRedBlackVertex<TKey, TVal> vertex, TKey key);
        #endregion

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

        #region Parent
        /// <summary>
        /// This is the parent vertex.
        /// </summary>
        public LockFreeRedBlackVertex<TKey, TVal> Parent { get; set; }
        #endregion // Parent
        #region Left
        /// <summary>
        /// This is the left vertex.
        /// </summary>
        public LockFreeRedBlackVertex<TKey, TVal> Left { get; set; }
        #endregion // Left
        #region Right
        /// <summary>
        /// This is the right vertex.
        /// </summary>
        public LockFreeRedBlackVertex<TKey, TVal> Right { get; set; }
        #endregion // Right

        #region State
        /// <summary>
        /// This is the state of the vertex, either black or red.
        /// </summary>
        public LockFreeRedBlackVertexState State { get; set; }
        #endregion // State

        #region Search(TKey key, out LockFreeRedBlackVertex<TKey, TVal> vertex)
        /// <summary>
        /// This method traverses the tree
        /// </summary>
        /// <param name="key">The key to search for.</param>
        /// <param name="vertex">The vertex that matches the key.</param>
        /// <returns>Returns true if the search is matched, false if not found.</returns>
        public virtual bool Search(TKey key, out LockFreeRedBlackVertex<TKey, TVal> vertex)
        {
            //If the vertex is locked we wait until it is unlocked before proceeding.
            LockWait();

            vertex = null;
            switch (Comparer(this, key))
            {
                case 0:
                    vertex = this;
                    return true;
                case 1:
                    if (Right != null)
                        return Right.Search(key, out vertex);
                    break;
                case -1:
                    if (Left != null)
                        return Left.Search(key, out vertex);
                    break;
            }

            return false;
        }
        #endregion
        #region Insert(TKey key, LockFreeRedBlackVertex<TKey, TVal> vertex)
        /// <summary>
        /// This method inserts the vertex in the correct part of the tree.
        /// </summary>
        /// <param name="key">The insert key.</param>
        /// <param name="vertex">The vertex.</param>
        /// <returns>Returns true if the vertex is inserted successfully, false if the vertex already exists or clashes with another
        /// key and is rejected.</returns>
        public virtual bool Insert(TKey key, LockFreeRedBlackVertex<TKey, TVal> vertex)
        {
            //We lock the vertex so that other threads cannot proceed until we are unlocked.
            Lock();

            bool traversed = false;

            if (Parent != null)
                Parent.Unlock();

            try
            {
                switch (Comparer(this, key))
                {
                    case 0:
                        //Ok, we have a match, so we cannot insert duplicate items.
                        return false;
                    case 1:
                        if (Right != null)
                        {
                            traversed = true;
                            return Right.Insert(key, vertex);
                        }

                        Right = vertex;
                        Right.Parent = this;
                        break;
                    case -1:
                        if (Left != null)
                        {
                            traversed = true;
                            return Left.Insert(key, vertex);
                        }

                        Left = vertex;
                        Left.Parent = this;
                        break;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (!traversed)
                    Unlock();
            }

            return true;
        }
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
        public bool IsLeaf { get { return Left != null || Right != null; } }
        #endregion

        #region ToString()
        /// <summary>
        /// This override provides useful debug information.
        /// </summary>
        /// <returns>Returns a string representation of the vertex data.</returns>
        public override string ToString()
        {
            return string.Format("{0} -> {1} ==>", Key, Value);
        }
        #endregion
    }
}