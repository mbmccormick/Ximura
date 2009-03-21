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
    /// <summary>
    /// This structure contains the message string.
    /// </summary>
    public class MessageTemplate
    {
        #region Static methods
        static MessageTemplate()
        {
            Default = new MessageTemplate(" {0}\r\n", 1);
            Default.isDefault = true;
        }
        public static readonly MessageTemplate Default;
        #endregion // Static methods
        #region Declarations
        public string Message;
        public int NumParams;
        private bool isDefault;
        #endregion // Declarations

        #region Constructors
        public MessageTemplate(string Message, int NumParams)
        {
            this.Message = Message;
            this.NumParams = NumParams;
            isDefault = false;
        }
        #endregion // Constructors

        #region FormatMessage
        /// <summary>
        /// This message formats the parameters
        /// </summary>
        /// <param name="code"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        public string FormatMessage(int code, object[] list)
        {
            if (isDefault)
            {
                if (list.Length == 0)
                    list = new object[] { "" };
                return code.ToString() + " " + String.Format(Message, list);
            }

            return String.Format(Message, list);
        }

        public string FormatMessage(string[] multiLine, int code)
        {
            return ExtendedFormat(code, multiLine);
        }
        #endregion // FormatMessage

        #region ExtendedFormat
        /// <summary>
        /// This method prepares the extended string format.
        /// </summary>
        /// <param name="code"></param>
        /// <param name="items"></param>
        /// <returns></returns>
        private string ExtendedFormat(int code, string[] items)
        {
            StringBuilder output = new StringBuilder();
            int len = items.Length;
            if (len == 0)
                return null;

            int pointer = 0;
            string strCode = code.ToString();

            while (pointer < len)
            {
                output.Append(strCode);

                if (pointer < len - 1)
                    output.Append("-");
                else
                    output.Append(" ");

                output.Append(items[pointer]);
                if (!items[pointer].EndsWith("\r\n"))
                    output.Append("\r\n");
                pointer++;
            }

            return output.ToString();
        }
        #endregion // ExtendedFormat

    }
}
