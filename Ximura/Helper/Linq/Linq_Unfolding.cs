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
using Ximura.Framework;
#endregion // using
namespace Ximura
{
    public static partial class LinqHelper
    {
        #region Unfold
        /// <summary>
        /// The Unfold extension method "unfolds" and object in to a collection.
        /// </summary>
        /// <param name="start">The initial object to unfold.</param>
        /// <param name="generator">The generator function that will create the collection.</param>
        /// <returns>Returns an enumeration of object.</returns>
        public static IEnumerable Unfold(this object start, Func<object, Tuple<object, object>?> generator)
        {
            return start.Unfold<object, object>(generator);
        }
        /// <summary>
        /// The Unfold extension method "unfolds" and object in to a collection.
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="start">The initial object to unfold.</param>
        /// <param name="generator">The generator function that will create the collection.</param>
        /// <returns>Returns an enumeration of objects of type TResult.</returns>
        public static IEnumerable<TResult> Unfold<TResult>(this object start, Func<object, Tuple<TResult, object>?> generator)
        {
            return start.Unfold<object, TResult>(generator);
        }
        /// <summary>
        /// The Unfold extension method "unfolds" and object in to a collection.
        /// </summary>
        /// <typeparam name="T">The source type.</typeparam>
        /// <typeparam name="TResult">The enumeration type.</typeparam>
        /// <param name="start">The initial object to unfold.</param>
        /// <param name="generator">The generator function that will create the collection.</param>
        /// <returns>Returns an enumeration of objects of type TResult.</returns>
        public static IEnumerable<TResult> Unfold<T, TResult>(this T start, Func<T, Tuple<TResult, T>?> generator)
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
        #endregion
    }
}
