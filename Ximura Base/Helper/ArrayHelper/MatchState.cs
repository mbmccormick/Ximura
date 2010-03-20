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
    /// This class contains the match state.
    /// </summary>
    /// <typeparam name="TSource">The source type.</typeparam>
    public class MatchState<TSource>
    {
        #region Constructors
        /// <summary>
        /// The default constructor.
        /// </summary>
        public MatchState()
        {
            SlidingWindow = new Queue<TSource>();
            MatchPosition = -1;
            Position = 0;
            Length = 0;
            CarryOver = 0;
            Success = false;
            MultipartMatch = false;
        }
        /// <summary>
        /// The constructor.
        /// </summary>
        /// <param name="Position">The current position.</param>
        /// <param name="CarryOver">The current carry over position.</param>
        public MatchState(int Position, int CarryOver)
            : this()
        {
            this.Position = Position;
            this.CarryOver = CarryOver;
        }
        #endregion // Constructors

        #region SlidingWindow
        /// <summary>
        /// The sliding window queue.
        /// </summary>
        public Queue<TSource> SlidingWindow { get; set; }
        #endregion // SlidingWindow

        #region MatchPosition
        /// <summary>
        /// The match position in the source array
        /// </summary>
        public int MatchPosition { get; set; }
        #endregion // Position
        #region Position
        /// <summary>
        /// The match position in the source array
        /// </summary>
        public int Position { get; set; }
        #endregion // Position
        #region Length
        /// <summary>
        /// The length of the match. This is needed because some matches are of a variable length.
        /// </summary>
        public int Length { get; set; }
        #endregion // Position

        #region CarryOver
        /// <summary>
        /// The number of carry over position in the match array.
        /// </summary>
        public int CarryOver { get; set; }
        #endregion // CarryOver

        #region MultipartMatch
        /// <summary>
        /// The multipart match informs the routine that this is only part of a maultiple match.
        /// </summary>
        public bool MultipartMatch { get; set; }
        #endregion // MultipartMatch

        #region Success
        /// <summary>
        /// Indicates whether the match is a success.
        /// </summary>
        public bool Success { get; set; }
        #endregion // Success
        #region IsTerminator
        /// <summary>
        /// Indicates whether the match is a terminator. This additional functionality is needed for complex matches.
        /// </summary>
        public bool IsTerminator { get; set; }
        #endregion // IsTerminator
        #region IsPartialMatch
        /// <summary>
        /// Indicates whether we are currently processing a partial match.
        /// </summary>
        public bool IsPartialMatch
        {
            get;
            set;
        }
        #endregion // IsPartialMatch
        #region IsMatch
        /// <summary>
        /// Identifies when there is a match.
        /// </summary>
        public bool IsMatch
        {
            get;
            set;
        }
        #endregion // IsMatch

        #region SetMatch(int Position)
        /// <summary>
        /// This method sets the match at the specific position.
        /// </summary>
        /// <param name="Position">The position in the source array.</param>
        public void SetMatch(int Position)
        {
            Success = true;
            IsMatch = true;
            IsPartialMatch = false;
            this.CarryOver = CarryOver;
            this.Position += Position;
        }
        #endregion // SetMatch(int Position)
        #region SetPartialMatch(int Position, int CarryOver)
        /// <summary>
        /// This method sets a partial match.
        /// </summary>
        /// <param name="Position">The position in the source array.</param>
        /// <param name="CarryOver">The carry over position.</param>
        public void SetPartialMatch(int Position, int CarryOver)
        {
            Success = true;
            IsMatch = false;
            IsPartialMatch = true;
            this.CarryOver = CarryOver;
            this.Position += Position;
        }
        #endregion // SetPartialMatch(int Position, int CarryOver)
    }
}
