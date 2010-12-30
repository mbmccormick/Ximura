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

using Ximura;
using Ximura.Data;
#endregion
namespace Ximura
{
    /// <summary>
    /// This class is used to match the two collections.
    /// </summary>
    /// <typeparam name="TSource">The source type.</typeparam>
    /// <typeparam name="TMatch">The match type.</typeparam>
    public abstract class MatchCollectionState<TSource, TMatch> : IEnumerable<MatchTerminator<TSource, TMatch>>
    {
        #region Declarations
        private List<string> mDebugTraceCollection = null;
        private bool mIsTerminator = false;
        #endregion // Declarations
        #region Constructors
        /// <summary>
        /// The default constructor.
        /// </summary>
        public MatchCollectionState()
        {
            DebugTraceRecursion = 0;
            DebugTrace = false;
            SlidingWindow = new Queue<TSource>();
            IsTerminator = false;
            Status = null;
            Start = 0;
            Length = 0;
            MatchPosition = -1;
            CurrentEnumerator = null;
        }
        #endregion

        #region SlidingWindow
        /// <summary>
        /// The sliding window queue.
        /// </summary>
        public Queue<TSource> SlidingWindow { get; set; }
        #endregion

        #region IsTerminator
        /// <summary>
        /// Indicates whether the match is a terminator. This additional functionality is needed for complex matches.
        /// </summary>
        public bool IsTerminator 
        { 
            get {return mIsTerminator;}
            set { mIsTerminator = value; } 
        }
        #endregion // IsTerminator

        #region Status
        /// <summary>
        /// Identifies when there is a match.
        /// </summary>
        public MatchTerminatorStatus? Status
        {
            get;
            set;
        }
        #endregion // IsMatch

        #region DebugTrace
        /// <summary>
        /// Identifies when there is a match.
        /// </summary>
        public bool DebugTrace
        {
            get;
            set;
        }

        public int DebugTraceRecursion
        {
            get;
            set;
        }
        #endregion // DebugTrace
        #region DebugTraceCollection()
        /// <summary>
        /// This collection should be used to debug the tract route for the collection.
        /// </summary>
        /// <returns></returns>
        public List<string> DebugTraceCollection
        {
            get
            {
                if (!DebugTrace) 
                    return null;

                //Ok, create the trace collection
                if (mDebugTraceCollection == null)
                    mDebugTraceCollection = new List<string>();

                return mDebugTraceCollection;
            }
        }
        #endregion // DebugTraceCollection()

        #region Start
        /// <summary>
        /// Identifies the start of a match.
        /// </summary>
        public int Start
        {
            get;
            set;
        }
        #endregion // Length
        #region Length
        /// <summary>
        /// Identifies the position in the data, including the terminator.
        /// </summary>
        public int Length
        {
            get;
            set;
        }
        #endregion // Length
        #region MatchPosition
        /// <summary>
        /// This is the match position for the collection.
        /// </summary>
        public int MatchPosition { get; set; }
        #endregion // MatchPosition

        #region CurrentEnumerator
        /// <summary>
        /// This method holds the current enumerator during a partial match
        /// </summary>
        public IEnumerator<MatchTerminator<TSource, TMatch>> CurrentEnumerator { get; set; }
        #endregion // IsPartialMatch
        #region ActualEnumerator
        /// <summary>
        /// This property gets the CurrentEnumerator or creates a new enumerator if that is null.
        /// </summary>
        public IEnumerator<MatchTerminator<TSource, TMatch>> ActualEnumerator
        {
            get
            {
                IEnumerator<MatchTerminator<TSource, TMatch>> currentEnum;
                if (CurrentEnumerator != null)
                {
                    //The enumerator is active, so get it at the current position.
                    currentEnum = CurrentEnumerator;
                }
                else
                {
                    //The enumerator is not active so create a new one at the initial position.
                    currentEnum = GetEnumerator();
                    currentEnum.Reset();
                    currentEnum.MoveNext();
                    CurrentEnumerator = currentEnum;
                }

                return currentEnum;
            }
        }
        #endregion  
        #region IEnumerable<MatchTerminator<TSource,TMatch>> Members

        public abstract IEnumerator<MatchTerminator<TSource, TMatch>> GetEnumerator();
        #endregion
        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region ResetMatchCounters()
        /// <summary>
        /// This method resets the match counter between calls to the MatchCollection method.
        /// </summary>
        public virtual void ResetMatchCounters()
        {
            IsTerminator = false;
            Status = null;
            MatchPosition = -1;
            Start = 0;
        }
        #endregion // ResetMatchCounters()
    }
}
