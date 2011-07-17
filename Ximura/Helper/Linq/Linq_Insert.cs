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

using System.Threading;

using Ximura;
#endregion // using
namespace Ximura
{
    public static partial class LinqHelper
    {
        #region InsertAtStart<T>(this IEnumerable<T> items, T insert)
        /// <summary>
        /// This extension method inserts an item at the beginning of the enumerated collection.
        /// </summary>
        /// <typeparam name="T">The collection type.</typeparam>
        /// <param name="items">The collection.</param>
        /// <param name="insert">The item to insert at the beginning.</param>
        /// <returns>Returns the collection and an additional item at the beginning.</returns>
        public static IEnumerable<T> InsertAtStart<T>(this IEnumerable<T> items, T insert)
        {
            if (items == null) throw new ArgumentNullException("items", "items enumeration is null");

            yield return insert;

            foreach (var item in items)
                yield return item;
        }
        #endregion // InsertAtStart<T>(this IEnumerable<T> items, T insert)

        #region InsertAtPosition<T>(this IEnumerable<T> items, T insert, int position)
        /// <summary>
        /// The extension method inserts an item at the specified position. 
        /// If the collection is not of the specified length, then no item will be inserted.
        /// </summary>
        /// <typeparam name="T">The collection type.</typeparam>
        /// <param name="items">The collection.</param>
        /// <param name="insert">The item to insert.</param>
        /// <param name="position">The position within the collection to insert.</param>
        /// <returns>Returns the collection and an additional item inserted at the position specified.</returns>
        public static IEnumerable<T> InsertAtPosition<T>(this IEnumerable<T> items, T insert, int position)
        {
            if (items == null) throw new ArgumentNullException("items", "items enumeration is null");

            int counter = 0;
            foreach (var item in items)
            {
                if (counter == position)
                    yield return insert;

                yield return item;
                counter++;
            }
        }
        #endregion // InsertAtPosition<T>(this IEnumerable<T> items, T insert, int position)

        #region InsertAtEnd<T>(this IEnumerable<T> items, T insert)
        /// <summary>
        /// This extension method inserts an item at the end of the enumerated collection.
        /// </summary>
        /// <typeparam name="T">The collection type.</typeparam>
        /// <param name="items">The collection.</param>
        /// <param name="insert">The item to insert at the end.</param>
        /// <returns>Returns the collection and an additional item at the end.</returns>
        public static IEnumerable<T> InsertAtEnd<T>(this IEnumerable<T> items, T insert)
        {
            if (items == null) throw new ArgumentNullException("items", "items enumeration is null");

            foreach (var item in items)
                yield return item;

            yield return insert;
        }
        #endregion // InsertAtEnd<T>(this IEnumerable<T> items, T insert)
    }
}
