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

using Ximura;
using Ximura.Data;
#endregion
namespace Ximura
{
    /// <summary>
    /// 
    /// </summary>
    public static partial class ArrayHelper
    {
        #region GetGenericEnumerator<T>(this IEnumerable<T> data)
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public static IEnumerator<T> GetGenericEnumerator<T>(this IEnumerable<T> data)
        {
            IEnumerator<T> enumData = data.GetEnumerator();
            enumData.MoveNext();
            return enumData;
        }
        #endregion  

        #region GetEnumeratorAtPosition<T>(this IEnumerable<T> source, int pos)
        /// <summary>
        /// This method gets a enumerator for the IEnumerable object and moves it forward 
        /// by the required number of places.
        /// </summary>
        /// <typeparam name="T">The enumeration type.</typeparam>
        /// <param name="source">The boject source.</param>
        /// <param name="pos">The number of positions to move forward.</param>
        /// <returns>Returns the object or null if the method has moved passed the end of the collection.</returns>
        public static IEnumerator<T> GetEnumeratorAtPosition<T>(this IEnumerable<T> source, int pos)
        {
            IEnumerator<T> matchEnum = source.GetEnumerator();
            matchEnum.Reset();
            matchEnum.MoveNext();

            //OK, skip through the enumerator until we reach the correct position.
            while (pos > 0)
            {
                if (!matchEnum.MoveNext())
                    return null;
                pos--;
            }

            return matchEnum;
        }
        #endregion

        #region DequeueRemove<T>(this Queue<T> queue, int count)
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queue"></param>
        /// <param name="count"></param>
        public static void DequeueRemove<T>(this Queue<T> queue, int count)
        {
            while (count-- > 0 && queue.Count > 0)
                queue.Dequeue();
        }
        #endregion  

        #region ValidateCollectionSlidingWindow<TSource, TMatch>(MatchCollectionState<TSource, TMatch> state, bool deQueue)
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TMatch"></typeparam>
        /// <param name="state"></param>
        /// <param name="deQueue"></param>
        /// <returns></returns>
        private static MatchCollectionState<TSource, TMatch> ValidateCollectionSlidingWindow<TSource, TMatch>(
            MatchCollectionState<TSource, TMatch> state, bool deQueue)
        {
            //Remove the first item from the collection.
            if (deQueue)
            {
                state.SlidingWindow.Dequeue();
                state.Length++;
                state.Length -= state.SlidingWindow.Count;
            }

#if (DEBUG)
            if (state.DebugTrace)
                state.DebugTraceCollection.Add(
                    string.Format("Recurse -> {0} Q={1}, ({2})", state.DebugTraceRecursion,
                    state.SlidingWindow == null ? "null" : state.SlidingWindow.Count.ToString(),
                        !state.Status.HasValue ? "null" : state.Status.Value.ToString()));
#endif

            //Ok, check whether the queue is empty. This can happen when the queue only contained
            //1 item and was called recursively.
            if (state.SlidingWindow.Count == 0)
            {
                state.Status = MatchTerminatorStatus.NotSet;
                return state;
            }

            Queue<TSource> window = state.SlidingWindow;

            state.CurrentEnumerator.Reset();
            state.CurrentEnumerator.MoveNext();
            state.SlidingWindow = new Queue<TSource>();
            //OK, we recursively call the window to allow the queue to 
            //be processed.

            return window.MatchCollection(state);
        }
        #endregion  
        #region ValidateSlidingWindow<TSource, TMatch>(MatchState<TSource> state, IEnumerator<TMatch> matchEnum, Func<TSource, TMatch, bool> predicate)
        /// <summary>
        /// This method validates the sliding windows of previous records. This is needed because
        /// there may be partial matches in the previous array records. This is especially important
        /// when the match array is long.
        /// </summary>
        /// <typeparam name="TSource">The source array type.</typeparam>
        /// <typeparam name="TMatch">The match array type.</typeparam>
        /// <param name="state">The current match state.</param>
        /// <param name="matchEnum">The match enumerator.</param>
        /// <param name="predicate">The predicate used to validate the source and match items.</param>
        /// <returns>The match position or 0 if there is no match.</returns>
        private static int ValidateSlidingWindow<TSource, TMatch>(MatchState<TSource> state, IEnumerator<TMatch> matchEnum,
            Func<TSource, TMatch, bool> predicate)
        {
            int posMatch;

            //Remove the start of the previous point.
            state.SlidingWindow.Dequeue();

            //OK, no match so reset the match buffer to its default position
            while (state.SlidingWindow.Count > 0 && !SlidingWindowMatch(predicate, state.SlidingWindow, matchEnum))
                state.SlidingWindow.Dequeue();

            posMatch = state.SlidingWindow.Count;
            //Reset the match enumerator to its initial position.
            if (posMatch == 0)
            {
                matchEnum.Reset();
                matchEnum.MoveNext();
            }

            return posMatch;
        }
        #endregion // ValidateSlidingWindow
        #region SlidingWindowMatch
        /// <summary>
        /// This method matches the sliding window with the match.
        /// </summary>
        /// <typeparam name="TSource">The source array type.</typeparam>
        /// <typeparam name="TMatch">The match array type.</typeparam>
        /// <param name="predicate">The predicate used to validate the source and match items.</param>
        /// <param name="queue">The sliding window queue.</param>
        /// <param name="matchEnum">The match enumerator.</param>
        /// <returns>Returns true if there is a partial match.</returns>
        private static bool SlidingWindowMatch<TSource, TMatch>(Func<TSource, TMatch, bool> predicate,
            Queue<TSource> queue, IEnumerator<TMatch> matchEnum)
        {
            matchEnum.Reset();
            matchEnum.MoveNext();

            TMatch match;

            foreach (TSource item in queue)
            {
                match = matchEnum.Current;
                if (!predicate(item, match))
                {
                    return false;
                }
                //Ok, move to the next item in the match enumeration.
                if (!matchEnum.MoveNext())
                    throw new Exception("ArrayHelper/SlidingWindowMatch --> matchEnum.MoveNext() was not successful. This should not happen.");
            }

            return true;
        }
        #endregion // SlidingWindowMatch

        #region Contains<T>(this IEnumerable<T> items, Predicate<T> action)
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static bool Contains<T>(this IEnumerable<T> items, Predicate<T> action)
        {
            if (items == null) throw new ArgumentNullException("items");
            if (action == null) throw new ArgumentNullException("action");

            foreach (T item in items)
                if (action(item))
                    return true;

            return false;
        }
        #endregion  

        #region Range<TSource>(this IList<TSource> source, int offset, int count)
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
        #endregion  
    }
}
