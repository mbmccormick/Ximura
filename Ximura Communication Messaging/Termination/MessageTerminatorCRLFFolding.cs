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
namespace Ximura.Communication
{
    /// <summary>
    /// This is the default terminator for the MessageCRLFFragment message class.
    /// </summary>
    public class MessageTerminatorCRLFFolding : MessageGenericTerminatorBase<CRLFMatchCollectionState>
    {
        #region Declarations
        #endregion // Declarations
        #region Constructor
        /// <summary>
        /// The default constructor
        /// </summary>
        public MessageTerminatorCRLFFolding()
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
            mState = new CRLFMatchCollectionState(AllowFolding);
#if (DEBUG)
            mState.DebugTrace = true;
#endif
        }
        #endregion // Reset()

        #region AllowFolding
        /// <summary>
        /// This property specifies whether the message allows folding, that is a CRLF followed by a TAB or SPC character
        /// is not a termination. Otherwise the terminator will signal a match on CRLF.
        /// </summary>
        public virtual bool AllowFolding
        {
            get { return true; }
        }
        #endregion // AllowFolding
    }
}
