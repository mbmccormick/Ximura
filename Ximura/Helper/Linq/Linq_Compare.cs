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

using System.Threading;

using Ximura;
#endregion // using
namespace Ximura
{
    public static partial class LinqHelper
    {
        #region Compare<T>(this IEnumerable<T> source, IEnumerable<T> comparand)
        /// <summary>
        /// This extension method compares two collections for equality. The comparison of individual items is done using 
        /// the default comparer for the collection type, i.e. EqualityComparer<T>.Default.
        /// </summary>
        /// <typeparam name="T">The collection type.</typeparam>
        /// <param name="source">The source collection.</param>
        /// <param name="comparand">The collection to compare against.</param>
        /// <returns>Returns true if the two collections are identical, otherwise returns false. 
        /// Note: two empty collections will return true.</returns>
        public static bool Compare<T>(this IEnumerable<T> source, IEnumerable<T> comparand)
        {
            return source.Compare(comparand, EqualityComparer<T>.Default);
        }
        #endregion // Compare<T>(this IEnumerable<T> source, IEnumerable<T> comparand)
        #region Compare<T>(this IEnumerable<T> source, IEnumerable<T> comparand, IEqualityComparer<T> eqComparer)
        /// <summary>
        /// This extension method compares two collections for equality. The comparison of individual items is done using 
        /// the equality comparer.
        /// </summary>
        /// <typeparam name="T">The collection type.</typeparam>
        /// <param name="source">The source collection.</param>
        /// <param name="comparand">The collection to compare against.</param>
        /// <param name="comparer">The equality comparer.</param>
        /// <returns>Returns true if the two collections are identical, otherwise returns false. 
        /// Note: two empty collections will return true.</returns>
        public static bool Compare<T>(this IEnumerable<T> source, IEnumerable<T> comparand, IEqualityComparer<T> comparer)
        {
            return source.Compare(comparand, (first, second) => comparer.Equals(first, second));
        }
        #endregion // Compare<T>(this IEnumerable<T> source, IEnumerable<T> comparand, IEqualityComparer<T> eqComparer)
        #region Compare<T>(this IEnumerable<T> source, IEnumerable<T> comparand, Func<T, T, bool> eqFunc)
        /// <summary>
        /// This extension method compares two collections for equality. The comparison of individual items is done using 
        /// the equality function passed as a parameter.
        /// </summary>
        /// <typeparam name="T">The collection type.</typeparam>
        /// <param name="source">The source collection.</param>
        /// <param name="comparand">The collection to compare against.</param>
        /// <param name="eqFunc">The comparison function.</param>
        /// <returns>Returns true if the two collections are identical, otherwise returns false. 
        /// Note: two empty collections will return true.</returns>
        public static bool Compare<T>(this IEnumerable<T> source, IEnumerable<T> comparand, Func<T, T, bool> eqFunc)
        {
            IEnumerator<T> sourceEnum = source.GetEnumerator();
            IEnumerator<T> comprdEnum = comparand.GetEnumerator();

            //Ok, reset the collections to the beginning.
            sourceEnum.Reset();
            comprdEnum.Reset();

            bool sourceOK, comprdOK, success;
            do
            {
                sourceOK = sourceEnum.MoveNext();
                comprdOK = comprdEnum.MoveNext();

                //OK, return true only if both cannot read or both can read.
                success = !(sourceOK ^ comprdOK);

                //One of the collections cannot read, so it is smaller that the other, so return false.
                if (!success)
                    return false;

                //Ok, neither can be read so return success, so we have reached the end of both collections without failure.
                if (success & !sourceOK)
                    return true;

            }
            //OK check that both items are equal, if not return false.
            while (eqFunc(sourceEnum.Current, comprdEnum.Current));

            //The collections do not match as the comparison of individual items has failed. 
            return false;
        }
        #endregion // Compare<T>(this IEnumerable<T> source, IEnumerable<T> comparand, Func<T, T, bool> eqFunc)
    }
}
