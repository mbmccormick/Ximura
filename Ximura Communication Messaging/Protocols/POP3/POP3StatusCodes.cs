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

using Ximura.Framework;
using Ximura.Framework;
#endregion // using
namespace Ximura.Communication
{
    /// <summary>
    /// These are the specific service codes for the POP3 system, these specific codes are used 
    /// so that the particular template can be used.
    /// </summary>
    public enum POP3StatusCodes : int
    {
        R200_OK = 200,
        R211_SystemStatus = 211,
        R214_HelpOK = 214,
        R220_ServiceReadyNewUser = 220,
        R221_ServiceClosingConnection = 221,
        R250_FileActionOK = 250,
        R251_UserNotLocal_WillForward = 251,
        R252_CannotVerify_WillAccept = 252,

        R421_ServiceNotAvailable = 421,
        R450_RequestedFileActionNotTaken = 450,
        R451_RequestedActionAborted_localError = 451,
        R452_RequestedActionAborted_InsufficientSpace = 452,

        R500_SyntaxErr_UnregComm = 500,
        R501_SyntaxErr_Parameter = 501,
        R502_Command_Not_Implemented = 502,
        R503_Command_Bad_Sequence = 503,
        R504_Command_Not_implemented_Param = 504,

        R550_ActionNotTaken_FileUnavailable = 550,
        R551_UserNotLocal = 551,
        R552_RequestedMailActionAborted_ExceedStorage = 552,
        R553_RequestedActionAborted_NameNotAllowed = 553,
        R554_TransactionFailed = 554
    }
}
