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
    /// 
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <typeparam name="TMatch"></typeparam>
    public class MatchSequenceSkipOrFailTerminator<TSource, TMatch> : MatchTerminator<TSource, TMatch>
    {
        #region Constructor
        public MatchSequenceSkipOrFailTerminator(IEnumerable<TMatch> Terminator)
            : base(Terminator, false)
        {

        }

        public MatchSequenceSkipOrFailTerminator(IEnumerable<TMatch> Terminator, bool CanScan,
            Func<TSource, MatchTerminatorResult, MatchTerminatorStatus> Predicate, Func<MatchTerminatorResult, Queue<TSource>, TSource, long, bool> PredicateTerminator)
            : base(Terminator, CanScan, Predicate, PredicateTerminator)
        {

        }
        #endregion // Constructor

        #region Validate(TSource item, MatchTerminatorResult currentResult)
        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <param name="currentResult"></param>
        /// <returns></returns>
        protected override MatchTerminatorStatus Validate(TSource item, MatchTerminatorResult currentResult)
        {
            bool result = item.Equals(CurrentTerminator.Current);

            if (!result)
                return currentResult.Length == 0 ? MatchTerminatorStatus.SuccessNoLength : MatchTerminatorStatus.Fail;

            return CurrentTerminator.MoveNext() ?
                MatchTerminatorStatus.SuccessPartial : MatchTerminatorStatus.Success;
        }
        #endregion  
        #region ValidateTerminator(MatchTerminatorResult result, Queue<TSource> terminator, TSource currentItem, long length)
        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        /// <param name="terminator"></param>
        /// <param name="currentItem"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        protected override bool ValidateTerminator(MatchTerminatorResult result, Queue<TSource> terminator, TSource currentItem, long length)
        {
            return result.Length > 0;
        }
        #endregion  
    }
}
