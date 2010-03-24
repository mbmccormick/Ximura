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
        #region Curry<T1, TResult>(this Func<T1, TResult> f)
        /// <summary>
        /// This method curries the function to return a new function with the first parameter T1 wrapped in the new function.
        /// </summary>
        /// <typeparam name="T1">Parameter 1</typeparam>
        /// <typeparam name="TResult">The parameter result from the function.</typeparam>
        /// <param name="f">The function to be curried.</param>
        /// <returns>Returns a new function with the T1 parameter contained in the new function.</returns>
        public static Func<T1, Func<TResult>> Curry<T1, TResult>(this Func<T1, TResult> f)
        {
            return p1 => () => f(p1);
        }
        #endregion // Curry<T1, TResult>(this Func<T1, TResult> f)
        #region Curry<T1, T2, TResult>(this Func<T1, T2, TResult> f)
        /// <summary>
        /// This method curries the function to return a new function with the first parameter T1 wrapped in the new function.
        /// </summary>
        /// <typeparam name="T1">Parameter 1</typeparam>
        /// <typeparam name="T2">Parameter 2</typeparam>
        /// <typeparam name="TResult">The parameter result from the function.</typeparam>
        /// <param name="f">The function to be curried.</param>
        /// <returns>Returns a new function with the T1 parameter contained in the new function.</returns>
        public static Func<T1, Func<T2, TResult>> Curry<T1, T2, TResult>(this Func<T1, T2, TResult> f)
        {
            return p1 => p2 => f(p1, p2);
        }
        #endregion // Curry<T1, T2, TResult>(this Func<T1, T2, TResult> f)
        #region Curry<T1, T2, T3, TResult>(this Func<T1, T2, T3, TResult> f)
        /// <summary>
        /// This method curries the function to return a new function with the first parameter T1 wrapped in the new function.
        /// </summary>
        /// <typeparam name="T1">Parameter 1</typeparam>
        /// <typeparam name="T2">Parameter 2</typeparam>
        /// <typeparam name="T3">Parameter 3</typeparam>
        /// <typeparam name="TResult">The parameter result from the function.</typeparam>
        /// <param name="f">The function to be curried.</param>
        /// <returns>Returns a new function with the T1 parameter contained in the new function.</returns>
        public static Func<T1, Func<T2, T3, TResult>> Curry<T1, T2, T3, TResult>(this Func<T1, T2, T3, TResult> f)
        {
            return p1 => (p2, p3) =>  f(p1, p2, p3); 
        }
        #endregion // Curry<T1, T2, T3, TResult>(this Func<T1, T2, T3, TResult> f)
        #region Curry<T1, T2, T3, T4, TResult>(this Func<T1, T2, T3, T4, TResult> f)
        /// <summary>
        /// This method curries the function to return a new function with the first parameter T1 wrapped in the new function.
        /// </summary>
        /// <typeparam name="T1">Parameter 1</typeparam>
        /// <typeparam name="T2">Parameter 2</typeparam>
        /// <typeparam name="T3">Parameter 3</typeparam>
        /// <typeparam name="T4">Parameter 4</typeparam>
        /// <typeparam name="TResult">The parameter result from the function.</typeparam>
        /// <param name="f">The function to be curried.</param>
        /// <returns>Returns a new function with the T1 parameter contained in the new function.</returns>
        public static Func<T1, Func<T2, T3, T4, TResult>> Curry<T1, T2, T3, T4, TResult>(this Func<T1, T2, T3, T4, TResult> f)
        {
            return p1 => (p2, p3, p4) => f(p1, p2, p3, p4);
        }
        #endregion // Curry<T1, T2, T3, T4, TResult>(this Func<T1, T2, T3, T4, TResult> f)

        #region Curry<T1>(this Action<T1> f)
        /// <summary>
        /// This method curries the action to return a new action with the first parameter T1 wrapped in the new action.
        /// </summary>
        /// <typeparam name="T1">Parameter 1</typeparam>
        /// <param name="f">The action to be curried.</param>
        /// <returns>Returns a new action with the T1 parameter contained in the new function.</returns>
        public static Func<T1, Action> Curry<T1>(this Action<T1> f)
        {
            return p1 => () => f(p1);
        }
        #endregion // Curry<T1>(this Action<T1> f)
        #region Curry<T1, T2>(this Action<T1, T2> f)
        /// <summary>
        /// This method curries the action to return a new action with the first parameter T1 wrapped in the new action.
        /// </summary>
        /// <typeparam name="T1">Parameter 1</typeparam>
        /// <typeparam name="T2">Parameter 2</typeparam>
        /// <param name="f">The action to be curried.</param>
        /// <returns>Returns a new action with the T1 parameter contained in the new function.</returns>
        public static Func<T1, Action<T2>> Curry<T1, T2>(this Action<T1, T2> f)
        {
            return p1 => p2 => f(p1, p2);
        }
        #endregion // Curry<T1, T2>(this Action<T1, T2> f)
        #region Curry<T1, T2, T3>(this Action<T1, T2, T3> f)
        /// <summary>
        /// This method curries the action to return a new action with the first parameter T1 wrapped in the new action.
        /// </summary>
        /// <typeparam name="T1">Parameter 1</typeparam>
        /// <typeparam name="T2">Parameter 2</typeparam>
        /// <typeparam name="T3">Parameter 3</typeparam>
        /// <param name="f">The action to be curried.</param>
        /// <returns>Returns a new action with the T1 parameter contained in the new function.</returns>
        public static Func<T1, Action<T2, T3>> Curry<T1, T2, T3>(this Action<T1, T2, T3> f)
        {
            return p1 => (p2, p3) => f(p1, p2, p3);
        }
        #endregion // Curry<T1, T2, T3>(this Action<T1, T2, T3> f)
        #region Curry<T1, T2, T3, T4>(this Action<T1, T2, T3, T4> f)
        /// <summary>
        /// This method curries the action to return a new action with the first parameter T1 wrapped in the new action.
        /// </summary>
        /// <typeparam name="T1">Parameter 1</typeparam>
        /// <typeparam name="T2">Parameter 2</typeparam>
        /// <typeparam name="T3">Parameter 3</typeparam>
        /// <typeparam name="T4">Parameter 4</typeparam>
        /// <param name="f">The action to be curried.</param>
        /// <returns>Returns a new action with the T1 parameter contained in the new function.</returns>
        public static Func<T1, Action<T2, T3, T4>> Curry<T1, T2, T3, T4>(this Action<T1, T2, T3, T4> f)
        {
            return p1 => (p2, p3, p4) => f(p1, p2, p3, p4);
        }
        #endregion // Curry<T1, T2, T3, T4>(this Action<T1, T2, T3, T4> f)
    }
}
