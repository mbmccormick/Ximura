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
    public class ResponseMessage : Message
    {
        #region Declarations
        protected static Dictionary<int, MessageTemplate> messages;
        #endregion // Declarations
        #region Static Methods


        public static MessageTemplate MessageTemplateGet(int code)
        {
            if (messages.ContainsKey(code))
                return messages[code];

            return MessageTemplate.Default;
        }

        public static string MessageBuild(string overrideLine, int code, object[] list)
        {
            MessageTemplate template = MessageTemplateGet(code);

            return template.FormatMessage(code, list);
        }
        public static string MessageBuild(int code, object[] list)
        {
            MessageTemplate template = MessageTemplateGet(code);

            return template.FormatMessage(code, list);
        }

        public static string MessageBuild(string[] multiLine, int code)
        {
            MessageTemplate template = MessageTemplateGet(code);

            return template.FormatMessage(multiLine, code);
        }

        public static void MessageTemplateSet(int code, string message, int NoParams)
        {
            messages[code] = new MessageTemplate(code.ToString() + " " + message, NoParams);
        }
        #endregion // Static Methods

        #region Constructor
        /// <summary>
        /// The default constructor
        /// </summary>
        public ResponseMessage()
            : base()
        {
        }
        #endregion
        #region Reset()
        /// <summary>
        /// This is the reset method to set the content.
        /// </summary>
        public override void Reset()
        {
            base.Reset();
        }
        #endregion // Reset()

        public void Load(string overrideLine, int code, params object[] list)
        {
            Load(code.ToString() + " " + string.Format(overrideLine,list), Encoding.ASCII);
        }

        public void Load(int code, params object[] list)
        {
            Load(MessageBuild(code, list), Encoding.ASCII);
        }
        /// <summary>
        /// This method formats a multiline message
        /// </summary>
        /// <param name="multiLine"></param>
        /// <param name="code"></param>
        public void Load(string[] multiLine, int code)
        {
            Load(MessageBuild(multiLine,code), Encoding.ASCII);
        }

        #region FragmentInitialType
        /// <summary>
        /// This is the fragment type for the outgoing message.
        /// </summary>
        protected override Type FragmentHeaderInitialType
        {
            get
            {
                return typeof(BodyFragment);
            }
        }
        #endregion // FragmentInitialType

        #region CompletionCheck()
        /// <summary>
        /// The default behaviour is to have a single fragment. 
        /// Once it has completed we complete the message.
        /// </summary>
        /// <returns></returns>
        protected override bool CompletionCheck()
        {
            return FragmentFirst != null;
        }
        #endregion // CompletionCheck()
    }
}
