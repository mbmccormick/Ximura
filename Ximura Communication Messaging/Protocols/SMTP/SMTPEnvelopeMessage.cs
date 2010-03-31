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
using System.Threading;
using System.Text;
using System.Net;
using System.Net.Mail;
using System.Timers;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Reflection;

using System.Diagnostics;

using Ximura;
using Ximura.Data;
#endregion // using
namespace Ximura.Communication
{
    /// <summary>
    /// This message holds the message request details.
    /// </summary>
    public class SMTPEnvelopeMessage: Message
    {
        #region Declarations
        private Guid? mSMTPMessageID;
        private MailAddress mMailFrom;
        private List<MailAddress> mRcptTo = null;
        private string mDeclaredDomain;
        private DateTime? mMessageReceived;
        private string mIPAddress;
        #endregion // Declarations
        #region Constructor
        /// <summary>
        /// The default constructor
        /// </summary>
        public SMTPEnvelopeMessage()
            : base()
        {
        }
        #endregion

        #region Reset()
        /// <summary>
        /// This method resets the message envelope to its default state.
        /// </summary>
        public override void Reset()
        {
            mSMTPMessageID = null;
            mMailFrom = null;

            if (mRcptTo == null)
                mRcptTo = new List<MailAddress>();
            else
                mRcptTo.Clear(); 
            
            mDeclaredDomain = null; ;
            mMessageReceived = null;;
            mIPAddress = null;

            base.Reset();
        }
        #endregion // Reset()

    }
}
