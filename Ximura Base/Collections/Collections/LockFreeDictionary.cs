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
using System.Linq;
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
    public partial class LockFreeDictionary<TKey, TValue> : LockFreeCollectionBase<KeyValuePair<TKey, TValue>>, IDictionary<TKey, TValue>
    {
        #region Constructor
        public LockFreeDictionary()
            : base(null, 0, null, false)
        {

        }
        #endregion // Constructor


        #region Add
        public void Add(TKey key, TValue value)
        {
            Add(new KeyValuePair<TKey, TValue>(key, value));
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            throw new NotImplementedException();
        }
        #endregion // Add


        public bool TryGetValue(TKey key, out TValue value)
        {
            throw new NotImplementedException();
        }


        public TValue this[TKey key]
        {
            get
            {
                throw new KeyNotFoundException();
            }
            set
            {
                Add(key, value);
            }
        }

        #region Contains
        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            throw new NotImplementedException();
        }

        public bool ContainsKey(TKey key)
        {
            throw new NotImplementedException();
        }
        #endregion // Contains


        #region Remove
        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            if (item.Key == null)
            {
                throw new ArgumentNullException();
            }

            return false;
        }

        public bool Remove(TKey key)
        {
            if (key == null)
            {
                throw new ArgumentNullException();
            }


            return false;
        }
        #endregion // Remove


        #region Clear()
        public void Clear()
        {
            DisposedCheck();
            ClearInternal();
        }
        #endregion // Clear()


        #region Count
        public int Count
        {
            get { return CountInternal; }
        }
        #endregion // Count


        #region IsReadOnly
        public bool IsReadOnly
        {
            get { return false; }
        }
        #endregion // IsReadOnly


    }
}
