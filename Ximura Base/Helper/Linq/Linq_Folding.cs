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
        // Seq.fold
        public static T Fold<T, U>(this IEnumerable<U> items,
            Func<T, U, T> fn, T acc)
        {
            foreach (var item in items)
                acc = fn(acc, item);

            return acc;
        }

        // F# List.fold_left
        public static T FoldLeft<T, U>(this IList<U> list, 
            Func<T, U, T> fn, T acc)
        {
            for (int index = 0; index < list.Count; index++)
                acc = fn(acc, list[index]);

            return acc;
        }

        // F# List.fold_right
        public static T FoldRight<T, U>(this IList<U> list,
            Func<T, U, T> fn, T acc)
        {
            for (int index = list.Count - 1; index >= 0; index--)
                acc = fn(acc, list[index]);

            return acc;
        }

        // F# Seq.unfold
        public static IEnumerable<TResult> Unfold<T, TResult>(
            Func<T, Option<Tuple<TResult, T>>> generator,
            T start)
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
        public static TResult Forward<T1, T2, TResult>(this T1 arg1, Func<T1, T2, TResult> fn, T2 arg2)
        {
            return fn(arg1, arg2);
        }

        // F# - |>
        public static void Forward<T1, T2>(this T1 arg1, Action<T1, T2> action, T2 arg2)
        {
            action(arg1, arg2);
        }

        // F# <|
        public static TResult Rev<T1, T2, TResult>(this T2 arg2, Func<T1, T2, TResult> fn, T1 arg1)
        {
            return fn(arg1, arg2);
        }

        // F# - <|
        public static void Rev<T1, T2>(this T2 arg2, Action<T1, T2> action, T1 arg1)
        {
            action(arg1, arg2);
        }
    }
}
