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
    public class MatchExceptionTerminator<TSource, TMatch> : MatchTerminator<TSource, TMatch>
    {
        #region Constructor
        public MatchExceptionTerminator(IEnumerable<TMatch> Terminator)
            : base(Terminator, false)
        {

        }

        public MatchExceptionTerminator(IEnumerable<TMatch> Terminator, bool CanScan,
            Func<TSource, MatchTerminatorResult, MatchTerminatorStatus> Predicate,
            Func<MatchTerminatorResult, Queue<TSource>, TSource, long, bool> PredicateTerminator)
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
            bool result = Terminator.Contains(t => t.Equals(item));

            return result ? MatchTerminatorStatus.Fail : MatchTerminatorStatus.SuccessNoLength;
        }
        #endregion  
    }
}
