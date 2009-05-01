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
using System.Security.Cryptography;
using System.Threading;

using Ximura;
using Ximura.Server;
#endregion // using
namespace Ximura.Helper
{
    public static partial class LinqHelper
    {
        #region ForEach<T>(this IEnumerable<T> items, Action<T> action)
        /// <summary>
        /// The ForEach extension iterates through the items collection, and executes the action for each item.
        /// </summary>
        /// <example>
        /// A quick use of the method would be as follows:
        /// 
        ///     Enumerable.Range(0,40).ForEach(i => Console.WriteLine(i));
        ///     
        /// which is equivalent to the following code:
        /// 
        ///     foreach(var i in Enumerable.Range(0,40))
        ///         Console.WriteLine(i);
        /// </example>
        /// <typeparam name="T">The item type to process.</typeparam>
        /// <param name="items">The collection of items to process.</param>
        /// <param name="action">The action to be executed against each item in the collection.</param>
        public static void ForEach<T>(this IEnumerable<T> items, Action<T> action)
        {
            if (items == null) throw new ArgumentNullException("items","items enumeration is null");
            if (action == null) throw new ArgumentNullException("action","the action delegate is null");

            foreach (var item in items)
                action(item);
        }
        #endregion

        #region ForIndex<T>(this IEnumerable<T> items, Action<int, T> action)
        /// <summary>
        /// The ForIndex extension method iterates through the items collection, and executes the action for each item and provides 
        /// a 32-bit integer index parameter that identifies the position of the item in the collection.
        /// </summary>
        /// <typeparam name="T">The item type to process.</typeparam>
        /// <param name="items">The collection of items to process.</param>
        /// <param name="action">The action to be executed against each item in the collection.</param>
        public static void ForIndex<T>(this IEnumerable<T> items, Action<int, T> action)
        {
            if (items == null) throw new ArgumentNullException("items", "items enumeration is null");
            if (action == null) throw new ArgumentNullException("action", "the action delegate is null");

            int index = 0;
            foreach (var item in items)
            {
                action(index, item);
                index++;
            }
        }
        #endregion // ForIndex<T>(this IEnumerable<T> items, Action<int, T> action)
        #region ForBigIndex<T>(this IEnumerable<T> items, Action<long, T> action)
        /// <summary>
        /// The ForBigIndex extension method iterates through the items collection, and executes the action for each item and provides 
        /// a 64-bit integer parameter that identifies the position of the item in the collection.
        /// </summary>
        /// <typeparam name="T">The item type to process.</typeparam>
        /// <param name="items">The collection of items to process.</param>
        /// <param name="action">The action to be executed against each item in the collection.</param>
        public static void ForBigIndex<T>(this IEnumerable<T> items, Action<long, T> action)
        {
            if (items == null) throw new ArgumentNullException("items", "items enumeration is null");
            if (action == null) throw new ArgumentNullException("action", "the action delegate is null");

            long index = 0;
            foreach (var item in items)
            {
                action(index, item);
                index++;
            }
        }
        #endregion // ForIndex<T>(this IEnumerable<T> items, Action<long, T> action)

        #region Convert<T, U>(this IEnumerable<T> items, Func<T, U> convert)
        /// <summary>
        /// This method converts an enumerable collection in to a collection of converted items.
        /// </summary>
        /// <typeparam name="T">The type to convert.</typeparam>
        /// <typeparam name="U">The output type.</typeparam>
        /// <param name="items">The collection to convert.</param>
        /// <param name="convert">The conversion function.</param>
        /// <returns>Returns a enumeration of converted items.</returns>
        public static IEnumerable<U> Convert<T, U>(this IEnumerable<T> items, Func<T, U> convert)
        {
            if (items == null) throw new ArgumentNullException("items", "items enumeration is null");
            if (convert == null) throw new ArgumentNullException("convert", "convert function is null");

            foreach (var item in items)
                yield return convert(item);
        }
        #endregion // Convert<T, U>(this IEnumerable<T> items, Func<T, U> convert)
    }
}
