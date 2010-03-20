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
using Ximura.Helper;
using Ximura.Data;

using Ximura.Server;
using Ximura.Command;
#endregion // using
namespace Ximura.Communication
{
    public class POP3ResponseMessage : ResponseMessage
    {
        #region Declarations
        #endregion // Declarations
        #region Static Methods
        static POP3ResponseMessage()
        {
            messages = new Dictionary<int, MessageTemplate>();

            messages.Add((int)POP3StatusCodes.R200_OK,
                new MessageTemplate("+OK\r\n", 0));
            messages.Add((int)POP3StatusCodes.R211_SystemStatus,
                new MessageTemplate("211 {0}\r\n", 1));
            messages.Add((int)POP3StatusCodes.R214_HelpOK,
                new MessageTemplate("214 {0}\r\n", 1));
            messages.Add((int)POP3StatusCodes.R220_ServiceReadyNewUser,
                new MessageTemplate("+OK {0} POP3 Server ready.\r\n", 1));
            messages.Add((int)POP3StatusCodes.R221_ServiceClosingConnection,
                new MessageTemplate("221 {0} See you later Aligator\r\n", 1));
            messages.Add((int)POP3StatusCodes.R250_FileActionOK,
                new MessageTemplate("250 {0}.\r\n", 1));
            messages.Add((int)POP3StatusCodes.R251_UserNotLocal_WillForward,
                new MessageTemplate("251 User not local; will forward.\r\n", 0));
            messages.Add((int)POP3StatusCodes.R252_CannotVerify_WillAccept,
                new MessageTemplate("252 Cannot VRFY user, but will accept message and attempt delivery.\r\n", 0));

            messages.Add((int)POP3StatusCodes.R421_ServiceNotAvailable,
                new MessageTemplate("421 {0} Service not available, closing transmission channel.\r\n", 1));
            messages.Add((int)POP3StatusCodes.R450_RequestedFileActionNotTaken,
                new MessageTemplate("450 Requested mail action not taken: mailbox unavailable.\r\n", 0));
            messages.Add((int)POP3StatusCodes.R451_RequestedActionAborted_localError,
                new MessageTemplate("451 Requested action aborted: local error in processing.\r\n", 0));
            messages.Add((int)POP3StatusCodes.R452_RequestedActionAborted_InsufficientSpace,
                new MessageTemplate("452 Requested action not taken: insufficient system storage.\r\n", 0));

            messages.Add((int)POP3StatusCodes.R500_SyntaxErr_UnregComm,
                new MessageTemplate("500 Syntax error, command unrecognized.\r\n", 0));
            messages.Add((int)POP3StatusCodes.R501_SyntaxErr_Parameter,
                new MessageTemplate("501 Syntax error in parameters or arguments.\r\n", 0));
            messages.Add((int)POP3StatusCodes.R502_Command_Not_Implemented,
                new MessageTemplate("502 Command not implemented.\r\n", 0));
            messages.Add((int)POP3StatusCodes.R503_Command_Bad_Sequence,
                new MessageTemplate("503 Bad sequence of commands.\r\n", 0));
            messages.Add((int)POP3StatusCodes.R504_Command_Not_implemented_Param,
                new MessageTemplate("504 Command parameter not implemented.\r\n", 0));

            messages.Add((int)POP3StatusCodes.R550_ActionNotTaken_FileUnavailable,
                new MessageTemplate("550 Requested action not taken: mailbox unavailable.\r\n", 0));
            messages.Add((int)POP3StatusCodes.R551_UserNotLocal,
                new MessageTemplate("551 User not local.\r\n", 0));
            messages.Add((int)POP3StatusCodes.R552_RequestedMailActionAborted_ExceedStorage,
                new MessageTemplate("552 Requested mail action aborted: exceeded storage allocation.\r\n", 0));
            messages.Add((int)POP3StatusCodes.R553_RequestedActionAborted_NameNotAllowed,
                new MessageTemplate("553 Requested action not taken: mailbox name not allowed.\r\n", 0));
            messages.Add((int)POP3StatusCodes.R554_TransactionFailed,
                new MessageTemplate("554 Transaction failed  .\r\n", 0));
        }
        #endregion // Static Methods


        #region Constructor
        /// <summary>
        /// The default constructor
        /// </summary>
        public POP3ResponseMessage()
            : base()
        {
        }
        #endregion

        public void Load(POP3StatusCodes code, params object[] list)
        {
            Load((int)code,list);
        }

        public void Load(string[] multiLine, POP3StatusCodes code)
        {
            Load(multiLine,(int)code);
        }


        public void Load(string overrideLine, POP3StatusCodes code, params object[] list)
        {
            Load(overrideLine, (int)code, list);
        }
    }
}
