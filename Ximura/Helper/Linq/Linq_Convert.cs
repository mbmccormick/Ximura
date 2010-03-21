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
