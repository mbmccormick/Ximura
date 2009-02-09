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

using Ximura;
using Ximura.Data;
#endregion
namespace Ximura.Data
{
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
        #endregion // Constructors

        #region SlidingWindow
        /// <summary>
        /// The sliding window queue.
        /// </summary>
        public Queue<TSource> SlidingWindow { get; set; }
        #endregion // SlidingWindow

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


        #region IEnumerable<MatchTerminator<TSource,TMatch>> Members

        public abstract IEnumerator<MatchTerminator<TSource, TMatch>> GetEnumerator();
        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }

    #region Class --> MatchCollection<TSource, TMatch>
    /// <summary>
    /// This structure is used to hold the match terminator collection.
    /// </summary>
    public abstract class MatchCollection<TSource, TMatch> : IEnumerator<MatchTerminator<TSource, TMatch>>
    {
        #region Declarations
        private int position = -1;
        private bool disposed = false;
        #endregion // Declarations
        #region Constructors
        /// <summary>
        /// This protected constructor initializes the collection.
        /// </summary>
        /// <param name="terminators">The terminators enumerator.</param>
        protected MatchCollection(IEnumerator<MatchTerminator<TSource, TMatch>> terminators)
        {
        }
        #endregion // Constrcuctors

        #region Current
        /// <summary>
        /// This property returns the current record.
        /// </summary>
        public virtual MatchTerminator<TSource, TMatch> Current
        {
            get { return this[position]; }
        }

        object IEnumerator.Current
        {
            get { return this[position]; }
        }
        #endregion

        #region Dispose()/Finalize()
        /// <summary>
        /// This method disposes the collection.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public virtual void Dispose(bool disposing)
        {
            if (!disposed)
                if (disposing)
                {
                    disposed = true;
                }
        }
        #endregion

        #region MoveNext()
        /// <summary>
        /// This method moves the enumerator to the next position.
        /// </summary>
        /// <returns>Returns true if successful, or false if the end of the collection has been reached.</returns>
        public virtual bool MoveNext()
        {
            return ++position < Count;
        }
        #endregion // MoveNext()

        #region Reset()
        /// <summary>
        /// This method resets the collection to before the first record.
        /// </summary>
        public virtual void Reset()
        {
            position = -1;
        }
        #endregion // Reset()

        #region Position
        /// <summary>
        /// This is the current position in the match collection.
        /// </summary>
        public int Position { get { return position; } }
        #endregion // Position

        /// <summary>
        /// This method returns the specified item for the collection. You should override this indexer.
        /// </summary>
        /// <param name="index">The position index.</param>
        /// <returns></returns>
        public abstract MatchTerminator<TSource, TMatch> this[int index] { get; }

        /// <summary>
        /// This property returns the number of items in the collection. You should override this property.
        /// </summary>
        public abstract int Count { get; }
    }
    #endregion // Class --> MatchCollection<TSource, TMatch>

}
