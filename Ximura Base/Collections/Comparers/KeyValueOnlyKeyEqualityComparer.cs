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
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Threading;

using Ximura;
using Ximura.Helper;
#endregion // using
namespace Ximura.Collections
{
    /// <summary>
    /// This comparer is used to only report the key for the hashcode and equality comparer.
    /// </summary>
    /// <typeparam name="TKey">The key type.</typeparam>
    /// <typeparam name="TValue">The value type.</typeparam>
    public class KeyValueOnlyKeyEqualityComparer<TKey, TValue> : IEqualityComparer<KeyValuePair<TKey, TValue>>
    {
        #region Constructors
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        public KeyValueOnlyKeyEqualityComparer() : this(null, null) { }
        /// <summary>
        /// This constructor allows a custom key and value equality comparer to be passed to the class.
        /// </summary>
        /// <param name="keyComparer">The key comparer. If this is null, the default comparer is used.</param>
        public KeyValueOnlyKeyEqualityComparer(IEqualityComparer<TKey> keyComparer) : this(keyComparer, null) { }
        /// <summary>
        /// This constructor allows a custom key and value equality comparers to be passed to the class.
        /// </summary>
        /// <param name="keyComparer">The key comparer. If this is null, the default comparer is used.</param>
        /// <param name="valueComparer">The value comparer. If this is null, the default comparer is used.</param>
        public KeyValueOnlyKeyEqualityComparer(IEqualityComparer<TKey> keyComparer, IEqualityComparer<TValue> valueComparer)
        {
            if (keyComparer == null)
                KeyComparer = EqualityComparer<TKey>.Default;
            else
                KeyComparer = keyComparer;

            if (valueComparer == null)
                ValueComparer = EqualityComparer<TValue>.Default;
            else
                ValueComparer = valueComparer;
        }
        #endregion // Constructors
        #region KeyComparer
        /// <summary>
        /// This is the key comparer.
        /// </summary>
        public IEqualityComparer<TKey> KeyComparer { get; private set; }
        #endregion // Declarations
        #region ValueComparer
        /// <summary>
        /// This is the key comparer.
        /// </summary>
        public IEqualityComparer<TValue> ValueComparer { get; private set; }
        #endregion // Declarations

        #region Equals(KeyValuePair<TKey, TValue> x, KeyValuePair<TKey, TValue> y)
        /// <summary>
        /// This method only compares the key of the keyvalue pair for equality.
        /// </summary>
        /// <param name="x">The first keyvalue pair.</param>
        /// <param name="y">The second keyvalue pair.</param>
        /// <returns>Returns true if the keys are equal.</returns>
        public bool Equals(KeyValuePair<TKey, TValue> x, KeyValuePair<TKey, TValue> y)
        {
            return KeyComparer.Equals(x.Key, y.Key);
        }
        #endregion // Equals(KeyValuePair<TKey, TValue> x, KeyValuePair<TKey, TValue> y)
        #region GetHashCode(KeyValuePair<TKey, TValue> obj)
        /// <summary>
        /// The method returns the hashcode for the key of the keyvalue pair.
        /// </summary>
        /// <param name="obj">The object to get the hashcode.</param>
        /// <returns>Returns the key hashcode.</returns>
        public int GetHashCode(KeyValuePair<TKey, TValue> obj)
        {
            return KeyComparer.GetHashCode(obj.Key);
        }
        #endregion // GetHashCode(KeyValuePair<TKey, TValue> obj)
    }
}
