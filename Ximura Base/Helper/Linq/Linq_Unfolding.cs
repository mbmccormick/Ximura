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
        #region Unfold
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
        #endregion
    }
}
