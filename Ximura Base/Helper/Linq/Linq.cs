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
using Ximura.Server;
#endregion // using
namespace Ximura.Helper
{
    public static class Linq
    {
        public static IEnumerable<U> Convert<T, U>(this IEnumerable<T> items, Func<T, U> convert)
        {
            if (items == null) throw new ArgumentNullException("items");
            if (convert == null) throw new ArgumentNullException("convert");

            foreach (var item in items)
                yield return convert(item);
        }

        public static IEnumerable<T> InsertStart<T>(this IEnumerable<T> items, T insert)
        {
            yield return insert;

            foreach (var item in items)
                yield return item;
        }

        public static IEnumerable<T> InsertEnd<T>(this IEnumerable<T> items, T insert)
        {
            foreach (var item in items)
                yield return item;

            yield return insert;
        }

        // F# Seq.iter
        public static void ForEach<T>(this IEnumerable<T> items, Action<T> action)
        {
            if (items == null) throw new ArgumentNullException("items");
            if (action == null) throw new ArgumentNullException("action");

            foreach (var item in items)
                action(item);
        }

        public static void ForIndex<T>(this IEnumerable<T> items, Action<int, T> action)
        {
            if (items == null) throw new ArgumentNullException("items");
            if (action == null) throw new ArgumentNullException("action");

            int index = 0;
            foreach (var item in items)
            {
                action(index, item);
                index++;
            }
        }

        #region Currying
        // F# - Currying
        public static Func<TArg1, Func<TArg2, TResult>> Curry<TArg1, TArg2, TResult>(this Func<TArg1, TArg2, TResult> func)
        {
            return a1 => a2 => func(a1, a2);
        }

        // F# - Currying
        public static Func<TArg1, Action<TArg2>> Curry<TArg1, TArg2>(this Action<TArg1, TArg2> action)
        {
            return a1 => a2 => action(a1, a2);
        }
        #endregion // Currying


        // Seq.fold
        public static T Fold<T, U>(this IEnumerable<U> items, Func<T, U, T> func, T acc)
        {
            foreach (var item in items)
                acc = func(acc, item);

            return acc;
        }

        // F# List.fold_left
        public static T FoldLeft<T, U>(this IList<U> list, Func<T, U, T> func, T acc)
        {
            for (int index = 0; index < list.Count; index++)
                acc = func(acc, list[index]);

            return acc;
        }

        // F# List.fold_right
        public static T FoldRight<T, U>(this IList<U> list, Func<T, U, T> func, T acc)
        {
            for (int index = list.Count - 1; index >= 0; index--)
                acc = func(acc, list[index]);

            return acc;
        }

        // F# Seq.unfold
        public static IEnumerable<TResult> Unfold<T, TResult>(Func<T, Option<Tuple<TResult, T>>> generator, T start)
        {
            var next = start;

            while (true)
            {
                var res = generator(next);
                if (res.IsNone)
                    yield break;

                yield return res.Value.Item1;

                next = res.Value.Item2;
            }
        }

        // F# - |>
        public static TResult Forward<TArg1, TArg2, TResult>(this TArg1 arg1, Func<TArg1, TArg2, TResult> func, TArg2 arg2)
        {
            return func(arg1, arg2);
        }

        // F# - |>
        public static void Forward<TArg1, TArg2>(this TArg1 arg1, Action<TArg1, TArg2> action, TArg2 arg2)
        {
            action(arg1, arg2);
        }

        // F# <|
        public static TResult Rev<TArg1, TArg2, TResult>(this TArg2 arg2, Func<TArg1, TArg2, TResult> func, TArg1 arg1)
        {
            return func(arg1, arg2);
        }

        // F# - <|
        public static void Rev<TArg1, TArg2>(this TArg2 arg2, Action<TArg1, TArg2> action, TArg1 arg1)
        {
            action(arg1, arg2);
        }

    }
}
