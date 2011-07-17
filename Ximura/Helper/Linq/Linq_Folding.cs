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
        #region Fold
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T">The result type.</typeparam>
        /// <typeparam name="U">The enumeration type.</typeparam>
        /// <param name="items">The collection to fold.</param>
        /// <param name="f">The folding function.</param>
        /// <returns>Returns the result of the folding process.</returns>
        public static T Fold<T, U>(this IEnumerable<U> items, Func<T, U, T> f)
        {
            return items.FoldLeft(f, default(T));
        }
        /// <summary>
        /// This method applies the folding function on the collection and returns the result.
        /// </summary>
        /// <typeparam name="T">The result type.</typeparam>
        /// <typeparam name="U">The enumeration type.</typeparam>
        /// <param name="items">The collection to fold.</param>
        /// <param name="f">The folding function.</param>
        /// <param name="result"></param>
        /// <returns>Returns the result of the folding process.</returns>
        public static T Fold<T, U>(this IEnumerable<U> items, Func<T, U, T> f, T result)
        {
            return items.FoldLeft(f, result);
        }
        #endregion
        #region FoldLeft
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T">The result type.</typeparam>
        /// <typeparam name="U">The enumeration type.</typeparam>
        /// <param name="items">The collection to fold.</param>
        /// <param name="f">The folding function.</param>
        /// <returns>Returns the result of the folding process.</returns>
        public static T FoldLeft<T, U>(this IEnumerable<U> items, Func<T, U, T> f)
        {
            return items.FoldLeft(f, default(T));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T">The result type.</typeparam>
        /// <typeparam name="U">The enumeration type.</typeparam>
        /// <param name="items">The collection to fold.</param>
        /// <param name="f">The folding function.</param>
        /// <param name="result">The folding result initial value.</param>
        /// <returns>Returns the result of the folding process.</returns>
        public static T FoldLeft<T, U>(this IEnumerable<U> items, Func<T, U, T> f, T result)
        {
            foreach (var item in items)
                result = f(result, item);

            return result;
        }
        #endregion
        #region FoldRight
        /// <summary>
        /// This method folds the collection from the right, i.e the collection is reversed and the folding function is applied.
        /// </summary>
        /// <typeparam name="T">The result type.</typeparam>
        /// <typeparam name="U">The enumeration type.</typeparam>
        /// <param name="items">The collection to fold.</param>
        /// <param name="f">The folding function.</param>
        /// <returns>Returns the result of the folding process.</returns>
        public static T FoldRight<T, U>(this IEnumerable<U> items, Func<T, U, T> f)
        {
            return items.FoldRight(f, default(T));
        }
        /// <summary>
        /// This method folds the collection from the right, i.e the collection is reversed and the folding function is applied.
        /// </summary>
        /// <typeparam name="T">The result type.</typeparam>
        /// <typeparam name="U">The enumeration type.</typeparam>
        /// <param name="items">The collection to fold.</param>
        /// <param name="f">The folding function.</param>
        /// <param name="result">The folding result initial value.</param>
        /// <returns>Returns the result of the folding process.</returns>
        public static T FoldRight<T, U>(this IEnumerable<U> items, Func<T, U, T> f, T result)
        {
            if (items is IList<U>)
            {
                IList<U> list = items as IList<U>;
                for (int index = list.Count - 1; index >= 0; index--)
                    result = f(result, list[index]);
            }
            else
                foreach (var item in items.Reverse())
                    result = f(result, item);

            return result;
        }
        #endregion
    }
}
