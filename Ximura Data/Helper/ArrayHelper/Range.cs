﻿#region Copyright
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
        /// This extension selects a range of array values based on the offset and the count value.
        /// </summary>
        /// <typeparam name="TSource">This extension method can be applied to any object that implements the IList interface.</typeparam>
        /// <param name="source">The array source.</param>
        /// <param name="offset">The offset value.</param>
        /// <param name="count">The number of records to process.</param>
        /// <returns>Returns a enumerable collection containing the records.</returns>
        public static IEnumerable<TSource> Range<TSource>(this IList<TSource> source, int offset, int count)
        {
            int num = offset + count;
            for (int i = offset; i < num; i++)
            {
                yield return source[i];
            }
        }
    }
}
