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
        #region MatchAny
        public static MatchState<TSource> MatchAny<TSource, TMatch>(this IEnumerable<TSource> source,
            IEnumerable<TMatch> match, MatchState<TSource> state)
        {
            Func<TSource, TMatch, bool> validate = (x, y) => x.Equals(y);

            return source.MatchAny(match, validate, state);
        }

        public static MatchState<TSource> MatchAny<TSource, TMatch>(this IEnumerable<TSource> source,
            IEnumerable<TMatch> match, Func<TSource, TMatch, bool> predicate, MatchState<TSource> state)
        {
            return source.MatchAny(match, predicate, state, false);
        }

        public static MatchState<TSource> MatchAny<TSource, TMatch>(this IEnumerable<TSource> source,
            IEnumerable<TMatch> match, Func<TSource, TMatch, bool> predicate, MatchState<TSource> state, bool isMultipartMatch)
        {
            IEnumerator<TSource> sourceEnum = source.GetEnumerator();
            return sourceEnum.MatchAny(match, predicate, state, isMultipartMatch);
        }

        public static MatchState<TSource> MatchAny<TSource, TMatch>(this IEnumerator<TSource> sourceEnum,
            IEnumerable<TMatch> match, Func<TSource, TMatch, bool> predicate, MatchState<TSource> state, bool isMultipartMatch)
        {
            return null;
        }
        #endregion // MatchAny

        #region MatchSkip
        public static MatchState<TSource> MatchSkip<TSource, TMatch>(this IEnumerable<TSource> source,
            IEnumerable<TMatch> match, MatchState<TSource> state)
        {
            Func<TSource, TMatch, bool> predicate = (x, y) => x.Equals(y);
            return source.MatchSkip(match, predicate, state, false);
        }

        public static MatchState<TSource> MatchSkip<TSource, TMatch>(this IEnumerable<TSource> source,
            IEnumerable<TMatch> match, Func<TSource, TMatch, bool> predicate, MatchState<TSource> state, bool isMultipartMatch)
        {
            IEnumerator<TSource> sourceEnum = source.GetEnumerator();

            return sourceEnum.MatchSkip(match, predicate, state, isMultipartMatch);
        }

        public static MatchState<TSource> MatchSkip<TSource, TMatch>(this IEnumerator<TSource> sourceEnum,
            IEnumerable<TMatch> match, MatchState<TSource> state, bool isMultipartMatch)
        {
            Func<TSource, TMatch, bool> predicate = (x, y) => x.Equals(y);
            return sourceEnum.MatchSkip(match, predicate, state, isMultipartMatch); ;
        }

        public static MatchState<TSource> MatchSkip<TSource, TMatch>(this IEnumerator<TSource> sourceEnum,
            IEnumerable<TMatch> match, Func<TSource, TMatch, bool> predicate, MatchState<TSource> state, bool isMultipartMatch)
        {
            return null;
        }
        #endregion // MatchAny

        #region MatchSequence
        /// <summary>
        /// This method matches the sequence against the source array.
        /// </summary>
        /// <typeparam name="TSource">The source array type.</typeparam>
        /// <typeparam name="TMatch">The match array type.</typeparam>
        /// <param name="source">The source array.</param>
        /// <param name="match">The source array.</param>
        /// <param name="state">The current match state. This can be passed in when matching chucks of data from multiple source arrays.</param>
        /// <returns>The outgoing match state. This will indicate whether the match was successful or partially successful, i.e. there
        /// is a partial match at the end of the array that cannot be fully resolved.</returns>
        public static MatchState<TSource> MatchSequence<TSource, TMatch>(this IEnumerable<TSource> source,
            IEnumerable<TMatch> match, MatchState<TSource> state)
        {
            Func<TSource, TMatch, bool> validate = (x, y) => x.Equals(y);
            IEnumerator<TSource> sourceEnum = source.GetEnumerator();

            return sourceEnum.MatchSequence(match, validate, state);
        }

        public static MatchState<TSource> MatchSequence<TSource, TMatch>(this IEnumerator<TSource> sourceEnum,
            IEnumerable<TMatch> match, MatchState<TSource> state)
        {
            Func<TSource, TMatch, bool> predicate = (x, y) => x.Equals(y);

            return sourceEnum.MatchSequence(match, predicate, state);
        }

        /// <summary>
        /// This method matches the sequence against the source array.
        /// </summary>
        /// <typeparam name="TSource">The source array type.</typeparam>
        /// <typeparam name="TMatch">The match array type.</typeparam>
        /// <param name="source">The source array.</param>
        /// <param name="match">The source array.</param>
        /// <param name="predicate">The prediciate used to match the source and match array elements.</param>
        /// <param name="state">The current match state. This can be passed in when matching chucks of data from multiple source arrays.</param>
        /// <returns>The outgoing match state. This will indicate whether the match was successful or partially successful, i.e. there
        /// is a partial match at the end of the array that cannot be fully resolved.</returns>
        public static MatchState<TSource> MatchSequence<TSource, TMatch>(this IEnumerable<TSource> source,
            IEnumerable<TMatch> match, Func<TSource, TMatch, bool> predicate, MatchState<TSource> state)
        {
            IEnumerator<TSource> sourceEnum = source.GetEnumerator();
            return sourceEnum.MatchSequence(match, predicate, state);
        }

        public static MatchState<TSource> MatchSequence<TSource, TMatch>(this IEnumerator<TSource> sourceEnum,
            IEnumerable<TMatch> match, Func<TSource, TMatch, bool> predicate, MatchState<TSource> state)
        {
            try
            {
                //Get an internal state to track the match progress, 
                //either create a new one, or get one from the incoming state object.
                MatchState<TSource> stateInternal;
                if (state == null)
                    stateInternal = new MatchState<TSource>();
                else
                    stateInternal = state;

                //Shortcut to skip checking if the match has already been reached in the collection.
                if (stateInternal.IsMatch)
                    return stateInternal;

                //Check whether we are currently matching a block, this can happen when
                //matches are split over chunks of source arrays, such as a byte block passed from a remote socket.
                bool matchActive = stateInternal.CarryOver > 0;
                //Get the match array enumerator at the correct position.
                IEnumerator<TMatch> matchEnum = match.GetEnumeratorAtPosition(stateInternal.CarryOver);

                //Get the current record in the match array.
                TMatch matchItem = matchEnum.Current;

                int posSource = 0;
                int posMatch = stateInternal.CarryOver;

                //Ok, loop through each record in the source and compare against the match array.
                //foreach (TSource item in source)
                while (sourceEnum.MoveNext())
                {
                    TSource item = sourceEnum.Current;
                    //OK, do we have a match?
                    bool isMatch = predicate(item, matchItem);

                    if (isMatch)
                    {
                        matchActive = true;
                        stateInternal.Length++;

                        stateInternal.SlidingWindow.Enqueue(item);

                        if (matchEnum.MoveNext())
                        {
                            //Add the item to the sliding window for back checking
                            matchItem = matchEnum.Current;
                            posMatch++;
                        }
                        else
                        {
                            matchActive = false;
                            //Ok, we have completed a match.
                            stateInternal.SetMatch(posSource - posMatch);


                            break;
                        }
                    }
                    else
                    {
                        if (matchActive)
                        {


                            if (stateInternal.MultipartMatch)
                            {
                                //We cannot continue as we are only matching part of the records.
                                matchActive = false;
                                stateInternal.Success = false;
                                break;
                            }

                            //We need to check the previous record to see whether there is a partial match in there.
                            //This method will also set the match enumerator to the correct position
                            posMatch = ValidateSlidingWindow<TSource, TMatch>(stateInternal, matchEnum, predicate);
                            stateInternal.Length = posMatch;

                            //OK, no match so reset the match buffer to its default position
                            matchItem = matchEnum.Current;

                            //Set the match to active as we have matched based on the previous set of bytes.
                            matchActive = posMatch > 0;
                        }
                    }
                    posSource++;
                }

                //If the match is still active then we have a partial match and need to pass this out.
                if (matchActive)
                    stateInternal.SetPartialMatch(posSource, posMatch);

                return stateInternal;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion // MatchSequence

        #region ValidateSlidingWindow
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
                    throw new ApplicationException("ArrayHelper/SlidingWindowMatch --> matchEnum.MoveNext() was not successful. This should not happen.");
            }

            return true;
        }
        #endregion // SlidingWindowMatch

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
        #endregion // GetEnumeratorAtPosition<T>(this IEnumerable<T> source, int pos)
    }
}
