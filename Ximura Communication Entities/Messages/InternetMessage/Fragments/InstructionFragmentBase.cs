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
using System.Threading;
using System.Timers;
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
    public class InternetInstructionFragmentBase : MessageCRLFFragment
    {
        #region Declarations
        string mVerb;
        string mProtocol;
        string mVersion;
        string mInstruction;
        #endregion // Declarations
        #region Constructor
        /// <summary>
        /// The default constructor
        /// </summary>
        public InternetInstructionFragmentBase()
            : base()
        {
        }
        #endregion
        #region Reset()
        /// <summary>
        /// This method resets the fragment.
        /// </summary>
        public override void Reset()
        {
            mVerb = null;
            mProtocol = null;
            mInstruction = null;
            mVersion = null;
            base.Reset();
        }
        #endregion // Reset()
        
        #region IsRequest
        /// <summary>
        /// This property determines whether the message is a request. This is used to determine the order of the 
        /// three parts of the instruction. Request = Verb-Instruction-Protocol, Response = Protocol-Verb-Intruction.
        /// </summary>
        protected virtual bool IsRequest
        {
            get
            {
                return true;
            }
        }
        #endregion // IsRequest

        #region Verb
        /// <summary>
        /// This the the verb such as GET, POST etc for a Request or 200, 403, 404 for a Response message.
        /// </summary>
        public virtual string Verb
        {
            get
            {
                if (!Initializing)
                    MessagePartsBuild();
                return mVerb; 
            }
            set
            {
                if (!Initializing)
                    throw new NotSupportedException("Protocol cannot be set when the message is not initializing.");
                mVerb = value;
            }
        }
        #endregion // Verb

        #region Instruction
        /// <summary>
        /// This is the instruction, either a Uri query for a request or a message for a response.
        /// </summary>
        public virtual string Instruction
        {
            get
            {
                if (!Initializing)
                    MessagePartsBuild();
                return mInstruction;
            }
            set
            {
                if (!Initializing)
                    throw new NotSupportedException("Protocol cannot be set when the message is not initializing.");
                mInstruction = value;
            }
        }
        #endregion // Instruction

        #region Protocol
        /// <summary>
        /// This is the protocol for the message. This property will generally be overriden for a specific
        /// protocol.
        /// </summary>
        public virtual string Protocol
        {
            get
            {
                if (!Initializing)
                    MessagePartsBuild();
                return mProtocol;
            }
            set
            {
                if (!Initializing)
                    throw new NotSupportedException("Protocol cannot be set when the message is not initializing.");
                mProtocol = value;
            }
        }
        #endregion // Protocol

        #region Version
        /// <summary>
        /// This is the protocol for the message. This property will generally be overriden for a specific
        /// protocol.
        /// </summary>
        public virtual string Version
        {
            get
            {
                if (!Initializing)
                    MessagePartsBuild();
                return mVersion;
            }
            set
            {
                if (!Initializing)
                    throw new NotSupportedException("Protocol cannot be set when the message is not initializing.");
                mVersion = value;
            }
        }
        #endregion // Protocol

        #region MessagePartsBuild()
        /// <summary>
        /// This method splits the instruction header in to its constituent parts.
        /// </summary>
        /// <param name="force">Set this parameter to true if you wish to force a rebuild.</param>
        protected virtual void MessagePartsBuild()
        {
            if (mVerb != null && mInstruction != null && mProtocol!=null)
                return;

            string data = DataString;
            if (data == null)
            {
                mVerb = null;
                mInstruction = null;
                mProtocol = null;
                return;
            }

            string [] mFragmentParts = data.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            if (mFragmentParts.Length != 3)
                throw new MessageException("Instruction does not have the required parts.");
            string protver;

            if (IsRequest)
            {
                mVerb = mFragmentParts[0].Trim(' ');
                mInstruction = mFragmentParts[1].Trim(' ');
                protver = mFragmentParts[2].TrimStart(' ').TrimEnd('\r', '\n');
            }
            else
            {
                protver = mFragmentParts[0].Trim(' ');
                mVerb = mFragmentParts[1].Trim(' ');
                mInstruction = mFragmentParts[2].TrimStart(' ').TrimEnd('\r', '\n');
            }

            ParseProtocolVersion(protver, ref mProtocol, ref mVersion);
        }

        protected virtual void ParseProtocolVersion(string protver, ref string protocol, ref string version)
        {
            if (!protver.Contains(@"/"))
            {
                protocol = null;
                version = null;
                return;
            }

            string[] parts = protver.Split(new char[] { '/' }, StringSplitOptions.None);
            protocol = parts[0];
            if (parts.Length > 1)
                version = parts[1];
            else
                version = null;
        }
        #endregion // FragmentCollectionBuild(bool force)

        #region EndInitCustom()
        /// <summary>
        /// This method is used to complete the header collection organization once the initialization phase has ended.
        /// </summary>
        protected override void EndInitCustom()
        {
            if (IsRequest)
                DataString = mVerb + " " + mInstruction + " " + mProtocol + @"/" + mVersion +"\r\n";
            else
                DataString = mProtocol + @"/" + mVersion + " " + mVerb + " " + mInstruction + "\r\n";

            mVerb = null;
            mInstruction = null;
            mProtocol = null;
            mVersion = null;

            base.EndInitCustom();
        }
        #endregion // FragmentCollectionComplete()
    }
}
