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
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <param name="action"></param>
        public static void ForEach<T>(this IEnumerable<T> items, Action<T> action)
        {
            if (items == null) throw new ArgumentNullException("items","items enumeration is null");
            if (action == null) throw new ArgumentNullException("action","the action delegate is null");

            foreach (var item in items)
                action(item);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <param name="action"></param>
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

        /// <summary>
        /// This method converts an enumerable collection in to a collection of items that have been converted.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="U"></typeparam>
        /// <param name="items"></param>
        /// <param name="convert"></param>
        /// <returns></returns>
        public static IEnumerable<U> Convert<T, U>(this IEnumerable<T> items, Func<T, U> convert)
        {
            if (items == null) throw new ArgumentNullException("items", "items enumeration is null");
            if (convert == null) throw new ArgumentNullException("convert", "convert function is null");

            foreach (var item in items)
                yield return convert(item);
        }
    }
}
