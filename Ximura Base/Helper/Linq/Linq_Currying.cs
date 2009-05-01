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
        // Currying Functions
        public static Func<T1, Func<TResult>> Curry<T1, TResult>(this Func<T1, TResult> f)
        {
            return p1 => () => f(p1);
        }

        public static Func<T1, Func<T2, TResult>> Curry<T1, T2, TResult>(this Func<T1, T2, TResult> f)
        {
            return p1 => p2 => f(p1, p2);
        }

        public static Func<T1, Func<T2, T3, TResult>> Curry<T1, T2, T3, TResult>(this Func<T1, T2, T3, TResult> f)
        {
            return p1 => (p2, p3) =>  f(p1, p2, p3); 
        }

        public static Func<T1, Func<T2, T3, T4, TResult>> Curry<T1, T2, T3, T4, TResult>(this Func<T1, T2, T3, T4, TResult> f)
        {
            return p1 => (p2, p3, p4) => f(p1, p2, p3, p4);
        }

        // Currying Actions
        public static Func<T1, Action> Curry<T1>(this Action<T1> f)
        {
            return p1 => () => f(p1);
        }

        public static Func<T1, Action<T2>> Curry<T1, T2>(this Action<T1, T2> f)
        {
            return p1 => p2 => f(p1, p2);
        }

        public static Func<T1, Action<T2, T3>> Curry<T1, T2, T3>(this Action<T1, T2, T3> f)
        {
            return p1 => (p2, p3) => f(p1, p2, p3);
        }

        public static Func<T1, Action<T2, T3, T4>> Curry<T1, T2, T3, T4>(this Action<T1, T2, T3, T4> f)
        {
            return p1 => (p2, p3, p4) => f(p1, p2, p3, p4);
        }
    }
}
