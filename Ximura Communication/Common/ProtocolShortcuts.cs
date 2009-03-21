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
using System.Timers;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Reflection;
using System.Drawing;
using System.Diagnostics;
using System.Collections.Generic;

using Ximura;
using Ximura.Helper;
using Ximura.Server;
using Ximura.Command;

#endregion // using
namespace Ximura.Communication
{
    /// <summary>
    /// The Helper class is used to receoncile command address.
    /// </summary>
    public class ProtocolShortcuts
    {
        /// <summary>
        /// This is the terminal and site reconciliation command
        /// </summary>
        public static readonly Guid TelnetProtocol;
        /// <summary>
        /// This is the address for the receonciliation command.
        /// </summary>
        public static readonly EnvelopeAddress TelnetProtocolAddress;
        /// <summary>
        /// This is the terminal and site reconciliation command
        /// </summary>
        public static readonly Guid XRPProtocol;
        /// <summary>
        /// This is the address for the receonciliation command.
        /// </summary>
        public static readonly EnvelopeAddress XRPProtocolAddress;
        /// <summary>
        /// This is the terminal and site reconciliation command
        /// </summary>
        public static readonly Guid SIPProtocol;
        /// <summary>
        /// This is the address for the receonciliation command.
        /// </summary>
        public static readonly EnvelopeAddress SIPProtocolAddress;
        /// <summary>
        /// This is the terminal and site reconciliation command
        /// </summary>
        public static readonly Guid HTTPProtocol;
        /// <summary>
        /// This is the address for the receonciliation command.
        /// </summary>
        public static readonly EnvelopeAddress HTTPProtocolAddress;
        /// <summary>
        /// This is the terminal and site reconciliation command
        /// </summary>
        public static readonly Guid POP3Protocol;
        /// <summary>
        /// This is the address for the receonciliation command.
        /// </summary>
        public static readonly EnvelopeAddress POP3ProtocolAddress;
        /// <summary>
        /// This is the terminal and site reconciliation command
        /// </summary>
        public static readonly Guid SMTPProtocol;
        /// <summary>
        /// This is the address for the receonciliation command.
        /// </summary>
        public static readonly EnvelopeAddress SMTPProtocolAddress;

        /// <summary>
        /// This is the terminal and site reconciliation command
        /// </summary>
        public static readonly Guid FTPProtocol;
        /// <summary>
        /// This is the address for the receonciliation command.
        /// </summary>
        public static readonly EnvelopeAddress FTPProtocolAddress;

        /// <summary>
        /// This is the terminal and site reconciliation command
        /// </summary>
        public static readonly Guid FTPDataProtocol;
        /// <summary>
        /// This is the address for the receonciliation command.
        /// </summary>
        public static readonly EnvelopeAddress FTPDataProtocolAddress;

        /// <summary>
        /// This is the static constructor.
        /// </summary>
        static ProtocolShortcuts()
        {
            TelnetProtocol = new Guid("37D3B71F-6EA5-4ae6-B17F-55C8BA81E596");
            TelnetProtocolAddress = new EnvelopeAddress(TelnetProtocol);
            XRPProtocol = new Guid("B91E843C-89FA-4d1e-9A8C-66749A0FCEB6");
            XRPProtocolAddress = new EnvelopeAddress(XRPProtocol);
            SIPProtocol = new Guid("AE24EC40-C855-4f7f-B501-7802346AE6D0");
            SIPProtocolAddress = new EnvelopeAddress(SIPProtocol);
            HTTPProtocol = new Guid("4F7C8ADF-F85B-4aa6-A983-61BE37F5A1D4");
            HTTPProtocolAddress = new EnvelopeAddress(HTTPProtocol);
            SMTPProtocol = new Guid("B9DAC783-3F0E-452a-95D6-BD93452DFA2A");
            SMTPProtocolAddress = new EnvelopeAddress(SMTPProtocol);
            POP3Protocol = new Guid("A9FD4147-FEAD-492a-B8E7-64F97523E449");
            POP3ProtocolAddress = new EnvelopeAddress(POP3Protocol);

            FTPProtocol = new Guid("3425AFC8-C37A-48ff-8D8A-2F6ACF601D74");
            FTPProtocolAddress = new EnvelopeAddress(FTPProtocol);

            FTPDataProtocol = new Guid("FC54B7D4-AAA6-43ef-9233-3606577D15AB");
            FTPDataProtocolAddress = new EnvelopeAddress(FTPDataProtocol);


        }
    }
}
