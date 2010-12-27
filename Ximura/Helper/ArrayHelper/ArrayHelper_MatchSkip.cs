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
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Text;
#endregion
namespace Ximura
{
    /// <summary>
    /// This static class provides a number of extension method for array objects.
    /// </summary>
    public static partial class ArrayHelper
    {
        #region MatchSkip<TSource, TMatch>(this IEnumerable<TSource> source, IEnumerable<TMatch> match, MatchState<TSource> state)
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TMatch"></typeparam>
        /// <param name="source"></param>
        /// <param name="match"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public static MatchState<TSource> MatchSkip<TSource, TMatch>(
            this IEnumerable<TSource> source, IEnumerable<TMatch> match, MatchState<TSource> state)
        {
            Func<TSource, TMatch, bool> predicate = (x, y) => x.Equals(y);
            return source.MatchSkip(match, predicate, state, false);
        }
        #endregion  
        #region MatchSkip<TSource, TMatch>(this IEnumerable<TSource> source,IEnumerable<TMatch> match, Func<TSource, TMatch, bool> predicate, MatchState<TSource> state, bool isMultipartMatch)
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TMatch"></typeparam>
        /// <param name="source"></param>
        /// <param name="match"></param>
        /// <param name="predicate"></param>
        /// <param name="state"></param>
        /// <param name="isMultipartMatch"></param>
        /// <returns></returns>
        public static MatchState<TSource> MatchSkip<TSource, TMatch>(
            this IEnumerable<TSource> source,
            IEnumerable<TMatch> match, Func<TSource, TMatch, bool> predicate, MatchState<TSource> state, bool isMultipartMatch)
        {
            IEnumerator<TSource> sourceEnum = source.GetEnumerator();

            return sourceEnum.MatchSkip(match, predicate, state, isMultipartMatch);
        }
        #endregion  
        #region MatchSkip<TSource, TMatch>(this IEnumerator<TSource> sourceEnum,IEnumerable<TMatch> match, MatchState<TSource> state, bool isMultipartMatch)
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TMatch"></typeparam>
        /// <param name="sourceEnum"></param>
        /// <param name="match"></param>
        /// <param name="state"></param>
        /// <param name="isMultipartMatch"></param>
        /// <returns></returns>
        public static MatchState<TSource> MatchSkip<TSource, TMatch>(
            this IEnumerator<TSource> sourceEnum,
            IEnumerable<TMatch> match, MatchState<TSource> state, bool isMultipartMatch)
        {
            Func<TSource, TMatch, bool> predicate = (x, y) => x.Equals(y);
            return sourceEnum.MatchSkip(match, predicate, state, isMultipartMatch); ;
        }
        #endregion  
        #region MatchSkip<TSource, TMatch>(this IEnumerator<TSource> sourceEnum,IEnumerable<TMatch> match, Func<TSource, TMatch, bool> predicate, MatchState<TSource> state, bool isMultipartMatch)
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TMatch"></typeparam>
        /// <param name="sourceEnum"></param>
        /// <param name="match"></param>
        /// <param name="predicate"></param>
        /// <param name="state"></param>
        /// <param name="isMultipartMatch"></param>
        /// <returns></returns>
        public static MatchState<TSource> MatchSkip<TSource, TMatch>(
            this IEnumerator<TSource> sourceEnum,
            IEnumerable<TMatch> match, Func<TSource, TMatch, bool> predicate, MatchState<TSource> state, bool isMultipartMatch)
        {
            return null;
        }
        #endregion  
    }
}
