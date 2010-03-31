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
    public class SMTPResponseMessage : ResponseMessage
    {
        #region Declarations
        #endregion // Declarations
        #region Static Methods
        static SMTPResponseMessage()
        {
            messages = new Dictionary<int, MessageTemplate>();

            messages.Add((int)SMTPStatusCodes.R211_SystemStatus,
                new MessageTemplate("211 {0}\r\n", 1));
            messages.Add((int)SMTPStatusCodes.R214_HelpOK,
                new MessageTemplate("214 {0}\r\n", 1));
            messages.Add((int)SMTPStatusCodes.R220_ServiceReadyNewUser,
                new MessageTemplate("220 {0} ready {1}\r\n", 2));
            messages.Add((int)SMTPStatusCodes.R221_ServiceClosingConnection,
                new MessageTemplate("221 {0} See you later Aligator\r\n", 1));
            messages.Add((int)SMTPStatusCodes.R250_FileActionOK,
                new MessageTemplate("250 {0}.\r\n", 1));
            messages.Add((int)SMTPStatusCodes.R251_UserNotLocal_WillForward,
                new MessageTemplate("251 User not local; will forward.\r\n", 0));
            messages.Add((int)SMTPStatusCodes.R252_CannotVerify_WillAccept,
                new MessageTemplate("252 Cannot VRFY user, but will accept message and attempt delivery.\r\n", 0));

            messages.Add((int)SMTPStatusCodes.R354_StartMainInput,
                new MessageTemplate("354 Start mail input; end with <CRLF>.<CRLF>\r\n", 0));

            messages.Add((int)SMTPStatusCodes.R421_ServiceNotAvailable,
                new MessageTemplate("421 {0} Service not available, closing transmission channel.\r\n", 1));
            messages.Add((int)SMTPStatusCodes.R450_RequestedFileActionNotTaken,
                new MessageTemplate("450 Requested mail action not taken: mailbox unavailable.\r\n", 0));
            messages.Add((int)SMTPStatusCodes.R451_RequestedActionAborted_localError,
                new MessageTemplate("451 Requested action aborted: local error in processing.\r\n", 0));
            messages.Add((int)SMTPStatusCodes.R452_RequestedActionAborted_InsufficientSpace,
                new MessageTemplate("452 Requested action not taken: insufficient system storage.\r\n", 0));

            messages.Add((int)SMTPStatusCodes.R500_SyntaxErr_UnregComm,
                new MessageTemplate("500 Syntax error, command unrecognized.\r\n", 0));
            messages.Add((int)SMTPStatusCodes.R501_SyntaxErr_Parameter,
                new MessageTemplate("501 Syntax error in parameters or arguments.\r\n", 0));
            messages.Add((int)SMTPStatusCodes.R502_Command_Not_Implemented,
                new MessageTemplate("502 Command not implemented.\r\n", 0));
            messages.Add((int)SMTPStatusCodes.R503_Command_Bad_Sequence,
                new MessageTemplate("503 Bad sequence of commands.\r\n", 0));
            messages.Add((int)SMTPStatusCodes.R504_Command_Not_implemented_Param,
                new MessageTemplate("504 Command parameter not implemented.\r\n", 0));

            messages.Add((int)SMTPStatusCodes.R550_ActionNotTaken_FileUnavailable,
                new MessageTemplate("550 Requested action not taken: mailbox unavailable.\r\n", 0));
            messages.Add((int)SMTPStatusCodes.R551_UserNotLocal,
                new MessageTemplate("551 User not local.\r\n", 0));
            messages.Add((int)SMTPStatusCodes.R552_RequestedMailActionAborted_ExceedStorage,
                new MessageTemplate("552 Requested mail action aborted: exceeded storage allocation.\r\n", 0));
            messages.Add((int)SMTPStatusCodes.R553_RequestedActionAborted_NameNotAllowed,
                new MessageTemplate("553 Requested action not taken: mailbox name not allowed.\r\n", 0));
            messages.Add((int)SMTPStatusCodes.R554_TransactionFailed,
                new MessageTemplate("554 Transaction failed  .\r\n", 0));
        }
        #endregion // Static Methods


        #region Constructor
        /// <summary>
        /// The default constructor
        /// </summary>
        public SMTPResponseMessage()
            : base()
        {
        }
        #endregion

        public void Load(SMTPStatusCodes code, params object[] list)
        {
            Load((int)code,list);
        }

        public void Load(string[] multiLine, SMTPStatusCodes code)
        {
            Load(multiLine,(int)code);
        }


        public void Load(string overrideLine, SMTPStatusCodes code, params object[] list)
        {
            Load(overrideLine, (int)code, list);
        }
    }
}
