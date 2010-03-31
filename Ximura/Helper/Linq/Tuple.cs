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
﻿#region using
using System;
using System.Text;
using System.IO;
using System.Security;
using System.Reflection;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Threading;

using Ximura;
#endregion // using
namespace Ximura
{
    /// <summary>
    /// The tuple structure is used to hold two values, predominantly for fast comparison.
    /// </summary>
    /// <typeparam name="T1">The first item type.</typeparam>
    /// <typeparam name="T2">The second item type.</typeparam>
    public struct Tuple<T1, T2> : IEquatable<Tuple<T1, T2>>
    {
        #region Declarations
        private T1 mItem1;
        private T2 mItem2;
        #endregion // Declarations
        #region Constructor
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        /// <param name="item1">The first item.</param>
        /// <param name="item2">The second item.</param>
        public Tuple(T1 item1, T2 item2)
        {
            mItem1 = item1;
            mItem2 = item2;
        }
        #endregion // Constructor

        #region Item1
        /// <summary>
        /// The first item.
        /// </summary>
        public T1 Item1
        {
            get { return mItem1; }
        }
        #endregion // Item1
        #region Item2
        /// <summary>
        /// The second item.
        /// </summary>
        public T2 Item2
        {
            get { return mItem2; }
        }
        #endregion // Item2

        #region Equals(object obj)
        /// <summary>
        /// Returns true if the two items are the same type and the same value.
        /// </summary>
        /// <param name="obj">The object to compare against.</param>
        /// <returns>Returns true if the two items are identical, false otherwise.</returns>
        public override bool Equals(object obj)
        {
            return (obj is Tuple<T1, T2>) ? Equals((Tuple<T1, T2>)obj) : false;
        }
        #endregion // Equals(object obj)
        #region Equals(Tuple<T1, T2> other)
        /// <summary>
        /// Returns true if the two Tuples are the same value.
        /// </summary>
        /// <param name="other">The tuple to compare against.</param>
        /// <returns>Returns true if the two tuples are identical, false otherwise.</returns>
        public bool Equals(Tuple<T1, T2> other)
        {
            return this==other;
        }
        #endregion // Equals(Tuple<T1, T2> other)
        #region Operator !=
        /// <summary>
        /// This is the inequality operator for the Tuples.
        /// </summary>
        /// <param name="a">Tuple 1</param>
        /// <param name="b">Tuple 2</param>
        /// <returns>Returns true if the two tuples are different, otherwise false.</returns>
        public static bool operator !=(Tuple<T1, T2> a, Tuple<T1, T2> b)
        {
            return !(a==b);
        }
        #endregion // !=
        #region Operator ==
        /// <summary>
        /// This is the equality operator for the Tuples.
        /// </summary>
        /// <param name="a">Tuple 1</param>
        /// <param name="b">Tuple 2</param>
        /// <returns>Returns true if the two tuples are identical, otherwise false.</returns>
        public static bool operator ==(Tuple<T1, T2> a, Tuple<T1, T2> b)
        {
            if (!EqualityComparer<T1>.Default.Equals(a.Item1, b.Item1) ||
                !EqualityComparer<T2>.Default.Equals(a.Item2, b.Item2))
                return false;

            return true;
        }
        #endregion // ==

        #region GetHashCode()
        /// <summary>
        /// This method returns the hash code for the combined items.
        /// </summary>
        /// <returns>The hash code.</returns>
        public override int GetHashCode()
        {
            int result = EqualityComparer<T1>.Default.GetHashCode(mItem1);
            result ^= EqualityComparer<T2>.Default.GetHashCode(mItem2);
            return result;
        }
        #endregion // GetHashCode()
        #region ToString()
        /// <summary>
        /// This override provides an easy way to view the contents of the Tuple.
        /// </summary>
        /// <returns>Returns a string representation of the Tuple.</returns>
        public override string ToString()
        {
            return string.Format("Tuple:[{0}]-[{1}]",mItem1.ToString(),mItem2.ToString());
        }
        #endregion // ToString()
    }
}


