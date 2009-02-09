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
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Text;
#endregion
namespace Ximura.Data
{
    /// <summary>
    /// This static class provides a number of extension method for array objects.
    /// </summary>
    public static partial class ArrayHelper
    {
        /// <summary>
        /// This extension method will find the first position in the list based on the predicate.
        /// </summary>
        /// <typeparam name="TSource">The object type.</typeparam>
        /// <param name="source">The array list.</param>
        /// <param name="predicate">The match condition.</param>
        /// <returns>Return the position in the list, or -1 if the predicate cannot be matched.</returns>
        public static int FindFirstPosition<TSource>(this IList<TSource> source, Func<TSource, bool> predicate)
        {
            return FindPositionInternal<TSource>(source, 0, source.Count, predicate);
        }
        /// <summary>
        /// This extension method will find the first position in the list based on the predicate.
        /// </summary>
        /// <typeparam name="TSource">The object type.</typeparam>
        /// <param name="source">The array list.</param>
        /// <param name="offset">The list offset.</param>
        /// <param name="count">The number of items to process.</param>
        /// <param name="predicate">The match condition.</param>
        /// <returns>Return the position in the list, or -1 if the predicate cannot be matched.</returns>
        public static int FindFirstPosition<TSource>(this IList<TSource> source, int offset, int count, Func<TSource, bool> predicate)
        {
            return FindPositionInternal<TSource>(source, offset, count, predicate);
        }

        static int FindPositionInternal<TSource>(IList<TSource> source, int offset, int count, Func<TSource, bool> predicate)
        {
            int num = offset + count;
            int i;

            for (i = offset; i < num; i++)
            {
                if (predicate(source[i]))
                    break;
            }

            return i == num ? -1 : i;
        }
    }
}
