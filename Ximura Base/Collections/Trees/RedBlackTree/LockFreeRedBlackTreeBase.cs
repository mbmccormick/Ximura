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
using System.Text;

using Ximura;
using Ximura.Helper;
#endregion // using
namespace Ximura.Collections
{
    /// <summary>
    /// This class is a base class for RedBlack Tree implementations.
    /// </summary>
    /// <typeparam name="TKey">The tree key type.</typeparam>
    /// <typeparam name="TVal"></typeparam>
    public class LockFreeRedBlackTreeBase<TKey, TVal, TVert> : LockableBase, IDisposable
        where TVert : LockFreeRedBlackVertex<TKey, TVal>, new()
    {
        #region Declarations
        /// <summary>
        /// This is the current versionID of the collection.
        /// </summary>
        protected int mVersionID;
        
        private int mVertexCount;
        private bool mDisposed = false;
        /// <summary>
        /// This is the root vertex.
        /// </summary>
        protected TVert mRoot;
        #endregion // Declarations

        #region Constructors
        /// <summary>
        /// This is the default constructor. You must supply an appropriate comparer for the collection.
        /// </summary>
        /// <param name="comparer"></param>
        public LockFreeRedBlackTreeBase()
        {
            mVersionID = int.MinValue;
            mVertexCount = 0;
        }
        #endregion // Constructors
        #region IDisposable Members
        /// <summary>
        /// This is the finalizer for the collection.
        /// </summary>
        ~LockFreeRedBlackTreeBase()
        {
            this.Dispose(false);
        }

        #region DisposedCheck()
        /// <summary>
        /// This method identifies when the collection has been disposed and throws an ObjectDisposedException.
        /// </summary>
        /// <exception cref="System.ObjectDisposedException">This exception is thrown when the collection has been disposed.</exception>
        protected virtual void DisposedCheck()
        {
            if (mDisposed)
                throw new ObjectDisposedException(GetType().ToString(), "Collection has been disposed.");
        }
        #endregion // DisposedCheck()

        #region Dispose()
        /// <summary>
        /// This method disposes of the collection.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion // Dispose()

        /// <summary>
        /// This method disposes of the data in the collection. You should override this method if you need to add
        /// custom dispose logic to your collection.
        /// </summary>
        /// <param name="disposing">The class is disposing, i.e. this is called by Dispose and not the finalizer.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                mDisposed = true;
            }
        }

        #endregion

        #region AddInternal(TKey key, TVal item)
        /// <summary>
        /// This method adds an item to the tree.
        /// </summary>
        /// <param name="key">The item key.</param>
        /// <param name="item">The item.</param>
        /// <returns>Returns true if the item is successfully added to the list, false if the item is already in the tree.</returns>
        protected virtual bool AddInternal(TKey key, TVal item)
        {
            LockWait();

            TVert vItem = new TVert();
            vItem.Key = key;
            vItem.Value = item;

            bool success = false;

            //Ok, check whether we are adding the root node.
            if (Interlocked.CompareExchange<TVert>(ref mRoot, vItem, null) == null)
                success = true;
            else
            {
                //No, so we need to lock the root node and continue, the root node will be unlocked as we traverse the tree.
                success = mRoot.Insert(key, vItem);
                if (success)
                    RebalanceTree(vItem);
            }

            if (success)
            {
                Interlocked.Increment(ref mVertexCount);
                Interlocked.Increment(ref mVersionID);
            }

            return success;
        }
        #endregion // AddInternal(TKey key, TVal item)

        #region Tree rebalancing
        #region RebalanceTree(LockFreeRedBlackVertex<TKey, TVal> vItem)
        /// <summary>
        /// This method rebalances the tree based on the specific vertex.
        /// </summary>
        /// <param name="vItem">The vertex to rebalance around.</param>
        protected virtual void RebalanceTree(TVert vItem)
        {
            //Interlocked.CompareExchange<LockFreeRedBlackVertex<TKey, TVal>>(ref mRoot, vItem, mRoot);

        }
        #endregion // RebalanceTree(LockFreeRedBlackVertex<TKey, TVal> vItem)

        /// <summary>
        /// 
        /// </summary>
        /// <param name="vertex"></param>
        protected void RotateLeft(TVert vertex)
        {

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="vertex"></param>
        protected void RotateRight(TVert vertex)
        {

        }
        #endregion // Tree rebalancing

        #region RemoveInternal(TKey key)
        /// <summary>
        /// This method removes the vertex with the key from the collection.
        /// </summary>
        /// <param name="key">The vertex key.</param>
        /// <returns>Returns true if the vertex is removed.</returns>
        protected virtual bool RemoveInternal(TKey key)
        {
            LockWait();

            throw new NotImplementedException();
        }
        #endregion // RemoveInternal(TKey key)

        #region ContainsInternal(TKey key)
        /// <summary>
        /// This method returns true if the key can be satisfied by a vertex in the tree.
        /// </summary>
        /// <param name="key">The key to search.</param>
        /// <returns>Returns true if the comparer returns that the key has satisfied the conditions for a match.</returns>
        protected virtual bool ContainsInternal(TKey key)
        {
            LockWait();

            LockFreeRedBlackVertex<TKey, TVal> vertex;
            return mRoot.Search(key, out vertex);
        }
        #endregion // ContainsInternal(TKey key)

        #region ClearInternal()
        /// <summary>
        /// This method clears the tree of all data.
        /// </summary>
        protected virtual void ClearInternal()
        {
            TVert oldRoot = mRoot;

            while (Interlocked.CompareExchange<TVert>(ref mRoot, oldRoot, null) != oldRoot)
            {
                oldRoot = mRoot;
            }
            //Ok, reset the counters. New threads will get the
            mVertexCount = 0;
            Interlocked.Increment(ref mVersionID);

            //Ok, now we need to wait for any other threads to finish their business on the old collection.
            oldRoot.LockWait();
        }
        #endregion // ClearInternal()

        #region VersionInternal
        /// <summary>
        /// This is the current collection version.
        /// </summary>
        public long VersionInternal
        {
            get
            {
                return mVersionID;
            }
        }
        #endregion // Version
        #region CountInternal
        /// <summary>
        /// This property returns the number of items in the tree.
        /// </summary>
        protected virtual int CountInternal { get { return mVertexCount; } }
        #endregion // Count
        #region IsReadOnly
        /// <summary>
        /// This value specifies whether items can be added or removed from the tree. The default is false.
        /// </summary>
        public virtual bool IsReadOnly { get { return false; } }
        #endregion // IsReadOnly

        #region ToString()
        /// <summary>
        /// This is the debug information for the tree.
        /// </summary>
        /// <returns>Returns the string format of the tree.</returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            LockFreeRedBlackVertex<TKey, TVal> vertex = mRoot;

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
