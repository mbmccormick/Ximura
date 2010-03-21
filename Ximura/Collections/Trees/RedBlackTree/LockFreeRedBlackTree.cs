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
    /// This class is the default red-black tree that uses the generic comparer to balance the tree.
    /// </summary>
    /// <typeparam name="TKey">The key type.</typeparam>
    /// <typeparam name="TVal">The value payload.</typeparam>
    public class LockFreeRedBlackTree<TKey, TVal> : LockFreeRedBlackTreeBase<TKey, TVal>, IDictionary<TKey, TVal>
    {
        #region Constructors
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        public LockFreeRedBlackTree()
            : base()
        {
        }
        #endregion // Constructors

        #region Add
        public void Add(TKey key, TVal value)
        {
            AddInternal(key, value);
        }

        public void Add(KeyValuePair<TKey, TVal> item)
        {
            AddInternal(item.Key, item.Value);
        }
        #endregion // Add
        #region Remove

        public bool Remove(TKey key)
        {
            TreeTraversalWindow<TKey, TVal> window;

            if (!FindInternal(key, true, out window))
                return false;

            return RemoveInternal(window);
        }

        public bool Remove(KeyValuePair<TKey, TVal> item)
        {
            TreeTraversalWindow<TKey, TVal> window;

            if (!FindInternal(item.Key, true, out window))
                return false;

            if (EqualityComparer<TVal>.Default.Equals(item.Value, window.Current.Value))
                return RemoveInternal(window);

            window.Release();
            return false;
        }
        #endregion // Remove
        #region Contains
        public bool ContainsKey(TKey key)
        {
            TreeTraversalWindow<TKey, TVal> window;

            return FindInternal(key, false, out window);
        }

        public bool Contains(KeyValuePair<TKey, TVal> item)
        {
            TreeTraversalWindow<TKey, TVal> window;

            if (!FindInternal(item.Key, false, out window))
                return false;

            return EqualityComparer<TVal>.Default.Equals(item.Value, window.Current.Value);
        }
        #endregion // Contains

        #region Clear()
        /// <summary>
        /// This method clears the tree.
        /// </summary>
        public void Clear()
        {
            ClearInternal();
        }
        #endregion // Clear()

        #region Count
        /// <summary>
        /// This is the item count for the collection.
        /// </summary>
        public int Count
        {
            get { return CountInternal; }
        }
        #endregion // Count

        #region this[TKey key]
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public TVal this[TKey key]
        {
            get
            {
                TVal value;

                if (!TryGetValue(key, out value))
                    throw new KeyNotFoundException();

                return value;
            }
            set
            {
                AddInternal(key, value);
            }
        }
        #endregion // this[TKey key]
        #region TryGetValue(TKey key, out TVal value)
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryGetValue(TKey key, out TVal value)
        {
            TreeTraversalWindow<TKey, TVal> window;

            if (!FindInternal(key, false, out window))
            {
                value = default(TVal);
                return false;
            }

            value = window.Current.Value;
            return true;
        }
        #endregion // TryGetValue(TKey key, out TVal value)

        #region Keys
        public ICollection<TKey> Keys
        {
            get 
            {
                throw new NotImplementedException(); 
                //IEnumerator<KeyValuePair<TKey, TVal>> data = GetEnumerator();
                //data.Reset();
                //while (data.MoveNext())
                //    yield return data.Current.Key;
            }
        }
        #endregion // Keys
        #region Values
        public ICollection<TVal> Values
        {
            get { throw new NotImplementedException(); }
        }
        #endregion // Values
        #region CopyTo(KeyValuePair<TKey, TVal>[] array, int arrayIndex)
        public void CopyTo(KeyValuePair<TKey, TVal>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }
        #endregion // CopyTo(KeyValuePair<TKey, TVal>[] array, int arrayIndex)

        #region GetEnumerator()
        public IEnumerator<KeyValuePair<TKey, TVal>> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator)GetEnumerator();
        }
        #endregion // GetEnumerator()
    }
}
