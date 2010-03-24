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
using System.ComponentModel;
using System.Collections.Generic;

using Ximura;

using Ximura.Data;
using CH = Ximura.Common;
#endregion 
namespace Ximura.Communication
{
    public class MessageGenericTerminatorBase<STATE> : PoolableReturnableObjectBase, IXimuraMessageTermination
        where STATE: MatchCollectionState<byte, byte>
    {
        #region Declarations
        protected MatchCollectionState<byte, byte> mState = null;
        #endregion // Declarations
        #region Constructor
        /// <summary>
        /// The default constructor
        /// </summary>
        public MessageGenericTerminatorBase()
            : base()
        {
        }
        #endregion

        #region Reset()
        /// <summary>
        /// This method resets the state.
        /// </summary>
        public override void Reset()
        {
            mState = null;
            Initialized = false;
            base.Reset();
        }
        #endregion // Reset()

        #region Initialized
        /// <summary>
        /// This property identifies whether the terminator has been initialized with the boundary.
        /// </summary>
        public virtual bool Initialized
        {
            get;
            protected set;
        }
        #endregion

        #region IsTerminator
        /// <summary>
        /// Returns true if the fragment is a termination setting.
        /// </summary>
        public bool IsTerminator
        {
            get { return mState.IsTerminator; }
        }
        #endregion // IsTerminator
        #region CarryOver
        /// <summary>
        /// The number of carry over bytes in the buffer.
        /// </summary>
        public int CarryOver
        {
            get 
            { 
                return mState.SlidingWindow.Count; 
            }
        }
        #endregion // CarryOver

        #region Match(byte[] buffer, int offset, int count, out long length, out long? bodyLength)
        /// <summary>
        /// This is the match method. This method maps the incoming buffer to the match parameters.
        /// </summary>
        /// <param name="buffer">The byte buffer.</param>
        /// <param name="offset">The byte offset/</param>
        /// <param name="count">The number of bytes that can be read.</param>
        /// <param name="length">The length of the bytes read.</param>
        /// <returns>Returns true if a match has been found.</returns>
        public bool Match(byte[] buffer, int offset, int count, out int length, out long? bodyLength)
        {
            if (!Initialized)
                throw new NotSupportedException("Match is not supported without Initialization.");

            int lengthStart = mState.Length;
            mState = buffer.Range(offset, count).MatchCollection(mState);

            length = mState.Length - lengthStart;

            if (mState.Status.HasValue && (mState.Status.Value & MatchTerminatorStatus.Success) > 0)
                bodyLength = mState.MatchPosition - mState.Start;
            else
                bodyLength = null;

            return (mState.Status & MatchTerminatorStatus.Success) > 0;
        }
        #endregion // Match(byte[] buffer, int offset, int count, out int length)
    }
}
