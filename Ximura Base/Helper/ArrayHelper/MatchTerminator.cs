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
namespace Ximura
{
    /// <summary>
    /// This structure is used to hold the matching array.
    /// </summary>
    public abstract class MatchTerminator<TSource, TMatch>
    {
        #region Declarations
        protected IEnumerator<TMatch> mCurrentTerminator = null;
        protected Func<TSource, MatchTerminatorResult, MatchTerminatorStatus> mPredicate;
        protected Func<MatchTerminatorResult, Queue<TSource>, TSource, long, bool> mPredicateTerminator;
        #endregion
        #region Constructor
        public MatchTerminator(IEnumerable<TMatch> Terminator, bool CanScan)
            : this(Terminator, CanScan, null, null)
        {

        }
        public MatchTerminator(IEnumerable<TMatch> Terminator, bool CanScan,
            Func<TSource, MatchTerminatorResult, MatchTerminatorStatus> Predicate, 
            Func<MatchTerminatorResult, Queue<TSource>, TSource, long, bool> PredicateTerminator)
        {
            this.CanScan = CanScan;
            this.Terminator = Terminator;
            mPredicate = Predicate == null ? Validate : Predicate;
            mPredicateTerminator = PredicateTerminator == null ? ValidateTerminator : PredicateTerminator;
        }
        #endregion

        #region Terminator
        /// <summary>
        /// This is the current terminator.
        /// </summary>
        protected virtual IEnumerable<TMatch> Terminator
        {
            get;
            set;
        }
        #endregion // Terminator
        #region CurrentTerminator
        /// <summary>
        /// This is the current terminator.
        /// </summary>
        protected virtual IEnumerator<TMatch> CurrentTerminator
        {
            get
            {
                if (mCurrentTerminator == null)
                {
                    mCurrentTerminator = Terminator.GetEnumerator();
                    mCurrentTerminator.Reset();
                    mCurrentTerminator.MoveNext();
                }
                return mCurrentTerminator;
            }
        }
        #endregion // CurrentTerminator

        #region CanScan
        /// <summary>
        /// This property determines whether the terminator can scan through the source 
        /// for a match.
        /// </summary>
        protected bool CanScan{get;set;}
        #endregion // CanScan

        #region Match
        /// <summary>
        /// The predicate used to match the source and the match collection. By the default the equals parameter is passed.
        /// </summary>
        public virtual MatchTerminatorResult Match(IEnumerator<TSource> sourceEnum, Queue<TSource> slidingWindow, long length)
        {
            MatchTerminatorResult result = new MatchTerminatorResult();
            result.Length = 0;

            do
            {
                TSource item;
                MatchTerminatorStatus status = MatchTerminatorStatus.Exception;

                try
                {
                    item = sourceEnum.Current;
                    status = mPredicate(item, result);
                }
                catch (Exception ex)
                {
                    throw ex;
                }

                try
                {
                    switch (status)
                    {
                        case MatchTerminatorStatus.SuccessPartial:
                            slidingWindow.Enqueue(item);
                            result.Status = MatchTerminatorStatus.SuccessPartial;
                            break;

                        case MatchTerminatorStatus.Success:
                        case MatchTerminatorStatus.SuccessReset:
                        case MatchTerminatorStatus.SuccessNoLength:
                        case MatchTerminatorStatus.SuccessNoLengthReset:
                        case MatchTerminatorStatus.Fail:
                        case MatchTerminatorStatus.FailNoLength:
                            if (CanScan && status == MatchTerminatorStatus.Fail
                                && result.Status == MatchTerminatorStatus.NotSet)
                                break;

                            result.Status = status;
                            if (((status & MatchTerminatorStatus.NoLength) > 0))
                            {
                                result.CanContinue = true;
                            }
                            else
                            {
                                result.Length++;
                                slidingWindow.Enqueue(item);
                                result.CanContinue = sourceEnum.MoveNext();
                            }

                            //Check whether we have a termination condition.
                            if ((result.Status & MatchTerminatorStatus.Success) > 0)
                                result.IsTerminator = mPredicateTerminator(result, slidingWindow, item, length);


                            return result;

                        case MatchTerminatorStatus.NotSet:
                            break;

                        default:
                            //This shouldn't happen
                            break;
                    }
                    result.Length++;
                }
                catch (Exception ex)
                {
                    result.Status = MatchTerminatorStatus.Exception;
                }
            }
            while (sourceEnum.MoveNext());

            result.CanContinue = false;


            return result;
        }
        #endregion // Predicate

        #region Reset()
        /// <summary>
        /// This method resets the terminator to the beginning.
        /// </summary>
        public virtual void Reset()
        {
            CurrentTerminator.Reset();
            CurrentTerminator.MoveNext();
        }
        #endregion // Reset()

        #region Validate(TSource item)
        protected virtual MatchTerminatorStatus Validate(TSource item, MatchTerminatorResult currentResult)
        {
            throw new NotImplementedException("Validate is not implemented.");
        }
        #endregion // Validate(TSource item)

        #region ValidateTerminator(TSource item)
        protected virtual bool ValidateTerminator(MatchTerminatorResult result, Queue<TSource> terminator, TSource currentItem, long length)
        {
            return false;
        }
        #endregion // Validate(TSource item)
    }
}
