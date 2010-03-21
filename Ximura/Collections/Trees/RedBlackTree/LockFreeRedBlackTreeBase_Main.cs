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
using Ximura.Helper;
#endregion // using
namespace Ximura.Collections
{
    /// <summary>
    /// This class is a base class for Red-Black Tree implementations.
    /// </summary>
    /// <typeparam name="TKey">The key type.</typeparam>
    /// <typeparam name="TVal">The value type.</typeparam>
    /// <typeparam name="TVert">The vertex type.</typeparam>
    public partial class LockFreeRedBlackTreeBase<TKey, TVal, TVert> : ConcurrentTreeBase
        where TVert : RedBlackTreeLockableVertex<TKey, TVal>, new()
    {
        #region Declarations
        /// <summary>
        /// This is the current versionID of the collection.
        /// </summary>
        protected volatile int mVersion;

        /// <summary>
        /// This is the internal count for the collection.
        /// </summary>
        private volatile int mCount;

        /// <summary>
        /// This is the root vertex.
        /// </summary>
        protected RedBlackTreeLockableVertex<TKey, TVal> mRoot;

        /// <summary>
        /// This is the equality comparer for the values.
        /// </summary>
        protected EqualityComparer<TVal> mTvalEqComparer;
        /// <summary>
        /// This is the key comparer.
        /// </summary>
        protected Comparer<TKey> mTKeyComparer;
        #endregion // Declarations

        #region Constructors
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        public LockFreeRedBlackTreeBase() : this(Comparer<TKey>.Default, EqualityComparer<TVal>.Default) { }
        /// <summary>
        /// This constructor requires a comparison function and an equality comparer for the value data.
        /// </summary>
        /// <param name="keyComparer">The key comparer.</param>
        /// <param name="valueEqComparer">The value equality comparer. </param>
        public LockFreeRedBlackTreeBase(Comparer<TKey> keyComparer, EqualityComparer<TVal> valueEqComparer)
        {
            if (keyComparer == null)
                throw new ArgumentNullException("keyComparer cannot be null.");

            if (valueEqComparer == null)
                throw new ArgumentNullException("valueEqComparer cannot be null.");

            mVersion = int.MinValue;
            mCount = 0;

            mTKeyComparer = keyComparer;
            mTvalEqComparer = valueEqComparer;
        }

        #endregion // Constructors

        protected override void Dispose(bool disposing)
        {
        }

        #region Compare(TKey key, LockFreeRedBlackVertex<TKey, TVal> vertex)
        /// <summary>
        /// This method compares the key with the vertex key and returns an integer which identifies which is the greater value.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="vertex">The vertex to compare.</param>
        /// <returns>
        /// Returns 0 if the key is equal to the vertex. 
        /// Returns -1 if the key is less than the vertex, and returns 1 if the key is greater than the vertex.
        /// </returns>
        protected virtual int Compare(TKey key, RedBlackTreeLockableVertex<TKey, TVal> vertex)
        {
            return mTKeyComparer.Compare(key, vertex.Key);
        }
        #endregion // Compare(TKey key, LockFreeRedBlackVertex<TKey, TVal> vertex)

        #region AddInternal(TKey key, TVal item)
        /// <summary>
        /// This method adds an item to the tree.
        /// </summary>
        /// <param name="key">The item key.</param>
        /// <param name="item">The item.</param>
        /// <returns>Returns true if the item is successfully added to the list, false if the item is already in the tree.</returns>
        protected virtual bool AddInternal(TKey key, TVal item)
        {
            //Create a new vertex and set the values in the vertex.
            RedBlackTreeLockableVertex<TKey, TVal> insert = new TVert();
            insert.Key = key;
            insert.Value = item;
            return AddInternal(insert);
        }

        protected virtual bool AddInternal(RedBlackTreeLockableVertex<TKey, TVal> insert)
        {
            bool success = false;
            //We lock the insert node in case it becomes the root node, as we need to set the colour before we can allow other 
            //actions to proceed past the root node.
            insert.Lock();

            try
            {
                RedBlackTreeLockableVertex<TKey, TVal> rootVertex = null;

                //Ok, we need to get the root node, or set the root node.
                while (rootVertex == null)
                {
                    //Ok, check whether we are adding the root node.
                    if (Interlocked.CompareExchange<RedBlackTreeLockableVertex<TKey, TVal>>(ref mRoot, insert, null) == null)
                    {
                        //OK, as this is now the root node, we need to set it to black.
                        insert.IsBlack = true;
                        //Unlock the root node.
                        insert.Unlock();
                        success = true;
                        return true;
                    }
                    else
                        rootVertex = mRoot;
                }

                //Unlock the insert node as we are no longer working on it.
                insert.Unlock();

                //OK, need to insert the item in the tree.
                success = InsertInternal(rootVertex, insert);

                return success;
            }
            finally
            {
                if (success)
                {
                    Interlocked.Increment(ref mCount);
                    Interlocked.Increment(ref mVersion);
                }

            }
        }
        #endregion // AddInternal(TKey key, TVal item)
        #region InsertInternal(LockFreeRedBlackVertex<TKey, TVal> vertex)
        /// <summary>
        /// This method inserts the new vertex in the correct part of the tree.
        /// </summary>
        /// <param name="parent">The parent vertex where the search should begin.</param>
        /// <param name="newVertex">The new vertex to insert.</param>
        /// <returns>Returns true if the vertex is inserted successfully.</returns>
        protected virtual bool InsertInternal(RedBlackTreeLockableVertex<TKey, TVal> parent, RedBlackTreeLockableVertex<TKey, TVal> newVertex)
        {
            TreeTraversalWindow<TKey, TVal> window = new TreeTraversalWindow<TKey, TVal>(true);
            try
            {
                window.MoveDown(parent);

                while (true)
                {
                    int result = Compare(newVertex.Key, window.Current);

                    if (result == 0)
                    {
                        //Ok, we have a match, and we cannot insert duplicate items.
                        window.Release();
                        return false;
                    }

                    if (result > 0)
                    {
                        //Ok, the new vertex is greater than the source vertex.
                        if (window.Current.Right == null)
                        {
                            newVertex.Parent = window.Current;
                            window.Current.Right = newVertex;
                            window.MoveDown(newVertex);
                            break;
                        }
                        //Ok, we are searching right.
                        window.MoveDown(window.Current.Right);
                    }
                    else
                    {
                        //OK, the new vertex is less than the source vertex.
                        if (window.Current.Left == null)
                        {
                            newVertex.Parent = window.Current;
                            window.Current.Left = newVertex;
                            window.MoveDown(newVertex);
                            break;
                        }
                        //Ok, we are searching left.
                        window.MoveDown(window.Current.Left);
                    }
                }

                //Enforce the red-black tree rules.
                InsertFixup(window);

                //Release any vertexes currently held.
                window.Release();

                return true;
            }
            catch (Exception ex)
            {
                window.Release();
                throw ex;
            }
        }
        #endregion

        #region FindInternal(TKey key, bool lockVertexes, out TreeTraversalWindow<TKey, TVal> window)
        /// <summary>
        /// This method returns true if the key can be matched by a vertex in the tree.
        /// </summary>
        /// <param name="key">The key to search.</param>
        /// <returns>Returns true if the comparer returns that the key has satisfied the conditions for a match.</returns>
        protected virtual bool FindInternal(TKey key, bool lockVertexes, out TreeTraversalWindow<TKey, TVal> window)
        {

            window = new TreeTraversalWindow<TKey, TVal>(lockVertexes);

            window.MoveDown(mRoot);

            //Check whether we have a root vertex.
            if (window.Current == null)
                return false;

            while (true)
            {
                int result = Compare(key, window.Current);

                if (result == 0)
                {
                    //Ok, we have a match.
                    return true;
                }

                if (result > 0)
                {
                    //Ok, the new vertex is greater than the source vertex.
                    if (window.Current.Right == null)
                        return false;

                    //Ok, we are searching right.
                    window.MoveDown(window.Current.Right);
                }
                else
                {
                    //OK, the new vertex is less than the source vertex.
                    if (window.Current.Left == null)
                        return false;

                    //Ok, we are searching left.
                    window.MoveDown(window.Current.Left);
                }
            }

        }
        #endregion // ContainsInternal(TKey key)

        #region RemoveInternal(TreeTraversalWindow<TKey, TVal> window)
        /// <summary>
        /// This method removes the vertex with the key from the collection.
        /// </summary>
        /// <param name="key">The vertex key.</param>
        /// <returns>Returns true if the vertex is removed.</returns>
        protected virtual bool RemoveInternal(TreeTraversalWindow<TKey, TVal> window)
        {
            ThreadEnter(TreeAction.Remove);

            try
            {
                if (window.Current == null)
                {
                    window.Release();
                    return false;
                }

                throw new NotImplementedException();
            }
            finally
            {
                ThreadExit();
            }
        }
        #endregion // RemoveInternal(TKey key)

        #region ClearInternal()
        /// <summary>
        /// This method clears the tree of all data.
        /// </summary>
        protected virtual void ClearInternal()
        {
            RedBlackTreeLockableVertex<TKey, TVal> oldRoot = mRoot;

            while (Interlocked.CompareExchange<RedBlackTreeLockableVertex<TKey, TVal>>(ref mRoot, oldRoot, null) != oldRoot)
            {
                oldRoot = mRoot;
            }
            //Ok, reset the counters. New threads will get the
            mCount = 0;
            Interlocked.Increment(ref mVersion);

            //Ok, now we need to wait for any other threads to finish their business on the old collection.
            oldRoot.LockWait();
        }
        #endregion // ClearInternal()

        #region Version
        /// <summary>
        /// This is the current collection version.
        /// </summary>
        public int Version { get { return mVersion; } }
        #endregion // Version
        #region CountInternal
        /// <summary>
        /// This property returns the number of items in the tree.
        /// </summary>
        protected virtual int CountInternal { get { return mCount; } }
        #endregion // Count

        #region ToString()
        /// <summary>
        /// This is the debug information for the tree.
        /// </summary>
        /// <returns>Returns the string format of the tree.</returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            RedBlackTreeLockableVertex<TKey, TVal> vertex = mRoot;

            do
            {
                sb.Append(vertex.ToString());
                sb.AppendLine();
                vertex = vertex.Right;
            }
            while (vertex != null);

            return sb.ToString();
        }
        #endregion // ToString()
    }
}
