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
    public static partial class ArrayHelper
    {
        #region MatchCollection<TSource, TMatch>(this IEnumerable<TSource> source, MatchCollectionState<TSource, TMatch> matchCollectionState)
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TMatch"></typeparam>
        /// <param name="source"></param>
        /// <param name="matchCollectionState"></param>
        /// <returns></returns>
        public static MatchCollectionState<TSource, TMatch> MatchCollection<TSource, TMatch>(
            this IEnumerable<TSource> source, MatchCollectionState<TSource, TMatch> matchCollectionState)
        {
            IEnumerator<TSource> sourceEnum = source.GetEnumerator();
            sourceEnum.MoveNext();
            return sourceEnum.MatchCollection(matchCollectionState);
        }
        #endregion
        #region MatchCollection<TSource, TMatch>(this IEnumerator<TSource> sourceEnum, MatchCollectionState<TSource, TMatch> state)
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TMatch"></typeparam>
        /// <param name="sourceEnum"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public static MatchCollectionState<TSource, TMatch> MatchCollection<TSource, TMatch>(
            this IEnumerator<TSource> sourceEnum, MatchCollectionState<TSource, TMatch> state)
        {
            if (state == null)
                throw new ArgumentNullException("state cannot be null.");

#if (DEBUG)
            state.DebugTraceRecursion++;
#endif
            try
            {
#if (DEBUG)
                if (state.DebugTrace)
                    state.DebugTraceCollection.Add(
                        string.Format("Enter -> {0} Q={1}, ({2})", state.DebugTraceRecursion,
                        state.SlidingWindow == null ? "null" : state.SlidingWindow.Count.ToString(),
                        !state.Status.HasValue ? "null" : state.Status.Value.ToString()));
#endif
                //Ok, have we already matched in which case return the current state unchanged.
                if (state.Status.HasValue && ((state.Status.Value & MatchTerminatorStatus.Success) > 0))
                    return state;

                if (!state.Status.HasValue)
                {
                    state.Start = state.Length;
                    state.Status = MatchTerminatorStatus.NotSet;
                }

                //OK, get the MatchTerminator enumerator.
                IEnumerator<MatchTerminator<TSource, TMatch>> currentEnum;
                if (state.CurrentEnumerator != null)
                {
                    currentEnum = state.CurrentEnumerator;
                }
                else
                {
                    currentEnum = state.GetEnumerator();
                    currentEnum.Reset();
                    currentEnum.MoveNext();
                    state.CurrentEnumerator = currentEnum;
                }

                //Check whether there is any data from the sliding window
                //and if so process it first.
                if (state.SlidingWindow.Count > 0 && state.Status == MatchTerminatorStatus.NotSet)
                    state = ValidateCollectionSlidingWindow(state, false);

                //If the sliding window data has completed the match, then exit
                if ((state.Status & MatchTerminatorStatus.Success) > 0)
                    return state;

                MatchTerminatorResult result;
                bool reset = false;
                do
                {
                    MatchTerminator<TSource, TMatch> term = currentEnum.Current;
                    result = term.Match(sourceEnum, state.SlidingWindow, state.Length - state.Start);

                    reset = (result.Status & MatchTerminatorStatus.Reset) > 0;
#if (DEBUG)
                    if (state.DebugTrace)
                        state.DebugTraceCollection.Add(
                            string.Format("Match ({0})={1} [{2:X}]", term.GetType().Name, result.Status, state.Length));
#endif
                    state.IsTerminator |= result.IsTerminator;
                    switch (result.Status)
                    {
                        case MatchTerminatorStatus.Fail:
                            term.Reset();
                            state.Length += result.Length - 1;// +state.SlidingWindow.Count - 1;
                            if (state.SlidingWindow.Count > 0)
                            {
                                int oldLength = state.Length;
                                //Dispose of the first item, and process the queue.
                                state = ValidateCollectionSlidingWindow<TSource, TMatch>(state, true);
                                //Ok, we need to check whether we have a partial match in the enqueued bytes.
                                currentEnum = state.CurrentEnumerator;
                            }

                            if (state.Status != MatchTerminatorStatus.SuccessPartial)
                                currentEnum.Reset();

                            break;

                        case MatchTerminatorStatus.SuccessPartial:
                            //Ok, we have a partial match but have reached the end of sourceEnum, so return
                            //and wait for the next piece.
                            state.Status = MatchTerminatorStatus.SuccessPartial;
                            state.Length += result.Length;
                            if (state.MatchPosition == -1)
                                state.MatchPosition = state.Length - state.SlidingWindow.Count;
                            return state;

                        case MatchTerminatorStatus.NotSet:
                            //Ok, we have scanned to the end of the array and not found a match.
                            state.MatchPosition = -1;
                            state.Length += result.Length;
                            return state;

                        case MatchTerminatorStatus.SuccessNoLength:
                        case MatchTerminatorStatus.SuccessNoLengthReset:
                            term.Reset();
                            state.Length += result.Length;
                            if (state.MatchPosition == -1)
                                state.MatchPosition = state.Length - state.SlidingWindow.Count;

                            if (reset)
                            {
                                currentEnum.Reset();
                                currentEnum.MoveNext();
                            }


                            break;

                        case MatchTerminatorStatus.Success:
                        case MatchTerminatorStatus.SuccessReset:
                            term.Reset();
                            state.Length += result.Length;
                            if (state.MatchPosition == -1)
                                state.MatchPosition = state.Length - state.SlidingWindow.Count;

                            if (reset)
                            {
                                currentEnum.Reset();
                                currentEnum.MoveNext();
                            }
                            //Ok, we are successful, so we need to move on to the next step.

                            if (!result.CanContinue || reset)
                            {
                                //We have reached the end of the byte stream so we need to check whether we have
                                //actually terminated.
                                bool moreTerminatorParts = reset ? false : currentEnum.MoveNext();
                                if (!state.IsTerminator && moreTerminatorParts)
                                {
                                    //OK, we only have a partial match as the terminator flag is not set and we have more parts
                                    //of the terminator to process.
                                    state.Status = MatchTerminatorStatus.SuccessPartial;
                                }
                                else
                                {
                                    //Ok, either the termination flag is set, meaning the terminator chars match a specific character
                                    //or there are no more parts of the terminator to match.
                                    state.Status = MatchTerminatorStatus.Success;
                                    state.SlidingWindow.Clear();
                                }

                                return state;
                            }

                            break;
                        default:
                            throw new ArgumentOutOfRangeException("Unknown MatchTerminatorStatus value.");
                    }

                }
                while (result.CanContinue && currentEnum.MoveNext() && !reset);

                //OK, we have completed. Either we have failed or succeeded
                state.Status = result.Status;
                //OK, time to do some tidy up.
                if ((result.Status & MatchTerminatorStatus.Success) > 0)
                {
                    int extra = state.Length - state.MatchPosition;
                    state.SlidingWindow.DequeueRemove(extra);
                    state.CurrentEnumerator = null;
                }
                //Get the match array enumerator at the correct position.
                //IEnumerator<MatchTerminator<TSource, TMatch>> matchEnum = match.GetEnumeratorAtPosition(state.MatchCollectionPosition);

                return state;
            }
            finally
            {
#if (DEBUG)
                if (state.DebugTrace)
                    state.DebugTraceCollection.Add(
                        string.Format("Exit <- {0} Q={1}, ({2})", state.DebugTraceRecursion,
                        state.SlidingWindow == null ? "null" : state.SlidingWindow.Count.ToString(),
                        !state.Status.HasValue ? "null" : state.Status.Value.ToString()));

                state.DebugTraceRecursion--;
#endif
            }
        }
        #endregion  
    }
}
