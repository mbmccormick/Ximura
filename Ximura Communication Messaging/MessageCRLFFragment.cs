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
    /// This is the default CRLF fragment that does not support folding.
    /// </summary>
    public class MessageCRLFFragment: MessageCRLFFragment<MessageTerminatorCRLFNoFolding>
    {
        #region Constructor
        /// <summary>
        /// The default constructor
        /// </summary>
        public MessageCRLFFragment()
            : base()
        {
        }
        #endregion
    }

    /// <summary>
    /// This is the base generic CRLF fragment. You can set the folding characteristics by setting the 
    /// generic term parameter to either TerminatorCRLF or TerminatorCRLFNoFolding.
    /// </summary>
    /// <typeparam name="TERM">The terminator class.</typeparam>
    public class MessageCRLFFragment<TERM> : MessageFragment<TERM>
        where TERM: MessageTerminatorCRLFFolding
    {
        #region Declarations
        Encoding mDefaultEncoding;
        #endregion
        #region Constructor
        /// <summary>
        /// The default constructor
        /// </summary>
        public MessageCRLFFragment()
            : base()
        {
        }
        #endregion

        #region Reset()
        /// <summary>
        /// This override resets the default encoding.
        /// </summary>
        public override void Reset()
        {
            mDefaultEncoding = null;
            base.Reset();
        }
        #endregion // Reset()

        #region DataString
        /// <summary>
        /// This is the string representation of the data using the default encoding.
        /// </summary>
        public virtual string DataString
        {
            get
            {
                if (InternalBuffer == null)
                    return null;
                return DefaultEncoding.GetString(InternalBuffer, 0, (int)Length);
            }
            set
            {
                if (!Initializing)
                    throw new NotSupportedException("The DataString cannot be set when the fragment is not initializing.");
                InternalBuffer = DefaultEncoding.GetBytes(value);
            }
        }
        #endregion // DataString
        #region DefaultEncoding
        /// <summary>
        /// This is the default encoding for the message
        /// </summary>
        public virtual Encoding DefaultEncoding
        {
            get
            {
                if (mDefaultEncoding == null)
                    return Encoding.UTF8;
                return mDefaultEncoding;
            }
            protected set { mDefaultEncoding = value; }
        }
        #endregion // DefaultEncoding

    }
}
