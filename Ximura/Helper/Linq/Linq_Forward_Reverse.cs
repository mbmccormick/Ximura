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
namespace Ximura.Helper
{
    public static partial class LinqHelper
    {
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
