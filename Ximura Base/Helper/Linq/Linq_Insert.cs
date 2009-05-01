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

        public static IEnumerable<T> InsertAtStart<T>(this IEnumerable<T> items, T insert)
        {
            if (items == null) throw new ArgumentNullException("items", "items enumeration is null");

            yield return insert;

            foreach (var item in items)
                yield return item;
        }

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

        public static IEnumerable<T> InsertAtEnd<T>(this IEnumerable<T> items, T insert)
        {
            if (items == null) throw new ArgumentNullException("items", "items enumeration is null");

            foreach (var item in items)
                yield return item;

            yield return insert;
        }
    }
}
