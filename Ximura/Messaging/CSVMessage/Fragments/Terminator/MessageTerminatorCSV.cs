#region Copyright
// *******************************************************************************
// Copyright (c) 2000-2011 Paul Stancer.
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
using System.IO;
using System.Runtime.Serialization;
using System.ComponentModel;
using System.Collections;
using System.Text;

using Ximura;

using Ximura.Data;
using CH = Ximura.Common;
#endregion
namespace Ximura
{
    /// <summary>
    /// This class is used to identify the termination of a CSV line and is coded to be in accordance with RFC 4180.
    /// </summary>
    public class MessageTerminatorCSV : MessageGenericTerminatorBase<CSVTerminationMatchCollectionState>
    {
        #region Constructor
        /// <summary>
        /// The default constructor
        /// </summary>
        public MessageTerminatorCSV()
            : base()
        {
        }
        #endregion

        #region Reset()
        /// <summary>
        /// This method resets the terminator.
        /// </summary>
        public override void Reset()
        {
            base.Reset();
            Initialized = true;
            mState = new CSVTerminationMatchCollectionState(true);
#if (DEBUG)
            mState.DebugTrace = true;
#endif
        }
        #endregion // Reset()

        public override bool Match(byte[] buffer, int offset, int count, out int length, out long? bodyLength)
        {
            return base.Match(buffer, offset, count, out length, out bodyLength);
        }
    }
}
