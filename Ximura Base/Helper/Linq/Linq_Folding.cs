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
        #region Fold<T, U>(this IEnumerable<U> items, Func<T, U, T> f, T result)
        // Seq.fold
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="U"></typeparam>
        /// <param name="items"></param>
        /// <param name="f">The folding function.</param>
        /// <returns></returns>
        public static T Fold<T, U>(this IEnumerable<U> items, Func<T, U, T> f)
        {
            return items.FoldLeft(f, default(T));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="U"></typeparam>
        /// <param name="items"></param>
        /// <param name="f"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static T Fold<T, U>(this IEnumerable<U> items, Func<T, U, T> f, T result)
        {
            return items.FoldLeft(f, result);
        }
        #endregion // Fold<T, U>(this IEnumerable<U> items, Func<T, U, T> f, T result)


        #region FoldLeft<T, U>(this IEnumerable<U> items, Func<T, U, T> f, T result)
        // F# List.fold_left
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="U"></typeparam>
        /// <param name="items"></param>
        /// <param name="f">The folding function.</param>
        /// <returns></returns>
        public static T FoldLeft<T, U>(this IEnumerable<U> items, Func<T, U, T> f)
        {
            return items.FoldLeft(f, default(T));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="U"></typeparam>
        /// <param name="items"></param>
        /// <param name="f"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static T FoldLeft<T, U>(this IEnumerable<U> items, Func<T, U, T> f, T result)
        {
            foreach (var item in items)
                result = f(result, item);

            return result;
        }
        #endregion // FoldLeft<T, U>(this IEnumerable<U> items, Func<T, U, T> f, T result)


        #region FoldRight<T, U>(this IEnumerable<U> items, Func<T, U, T> f, T result)
        // F# List.fold_right
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T">The result type.</typeparam>
        /// <typeparam name="U">The enumeration type.</typeparam>
        /// <param name="items">The enumeration.</param>
        /// <param name="f">The folding function.</param>
        /// <returns></returns>
        public static T FoldRight<T, U>(this IEnumerable<U> items, Func<T, U, T> f)
        {
            return items.FoldRight(f, default(T));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="U"></typeparam>
        /// <param name="items"></param>
        /// <param name="f"></param>
        /// <param name="result">The folding result initial value.</param>
        /// <returns></returns>
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
        #endregion // FoldRight<T, U>(this IEnumerable<U> items, Func<T, U, T> f, T result)


        #region Unfold<T, TResult>(Func<T, Tuple<TResult, T>?> generator, T start)
        // F# Seq.unfold
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="generator">The sequence generator.</param>
        /// <returns>Returns an enumeration of items based on the generator function.</returns>
        public static IEnumerable<TResult> Unfold<T, TResult>(Func<T, Tuple<TResult, T>?> generator)
        {
            return Unfold(generator, default(T));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="generator"></param>
        /// <param name="start"></param>
        /// <returns></returns>
        public static IEnumerable<TResult> Unfold<T, TResult>(Func<T, Tuple<TResult, T>?> generator, T start)
        {
            var next = start;

            while (true)
            {
                Tuple<TResult, T>? result = generator(next);

                if (!result.HasValue)
                    yield break;

                yield return result.Value.Item1;

                next = result.Value.Item2;
            }
        }
        #endregion // Unfold<T, TResult>(Func<T, Tuple<TResult, T>?> generator, T start)


        #region Forward
        // F# - |>
        public static TResult Forward<T1, T2, TResult>(this T1 p1, Func<T1, T2, TResult> f, T2 p2)
        {
            return f(p1, p2);
        }

        // F# - |>
        public static void Forward<T1, T2>(this T1 p1, Action<T1, T2> f, T2 p2)
        {
            f(p1, p2);
        }
        #endregion // Forward

        #region Reverse
        // F# <|
        public static TResult Reverse<T1, T2, TResult>(this T2 p2, Func<T1, T2, TResult> f, T1 p1)
        {
            return f(p1, p2);
        }

        // F# - <|
        public static void Reverse<T1, T2>(this T2 p2, Action<T1, T2> f, T1 p1)
        {
            f(p1, p2);
        }
        #endregion // Reverse



    }
}
