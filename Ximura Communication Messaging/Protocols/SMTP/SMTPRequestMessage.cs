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
    /// This is the SMTP request message.
    /// </summary>
    public class SMTPRequestMessage : RequestMessage
    {
        #region Declarations
        #endregion // Declarations
        #region Constructor
        /// <summary>
        /// The default constructor
        /// </summary>
        public SMTPRequestMessage()
            : base()
        {
        }
        #endregion

        #region Verb
        /// <summary>
        /// The request verb
        /// </summary>
        public string Verb
        {
            get
            {
                string data = ((MessageCRLFFragment)FragmentFirst).DataString;
                string[] list;
                if (data.Contains(":"))
                    list = data.Trim().Split(new char[] { ':' }, StringSplitOptions.None);
                else
                    list = data.Trim().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                return list[0];
            }
        }
        #endregion // Verb
        #region Data
        /// <summary>
        /// The request data
        /// </summary>
        public string Data
        {
            get
            {
                string data = ((MessageCRLFFragment)FragmentFirst).DataString;
                string[] list;
                if (data.Contains(":"))
                    list = data.Trim().Split(new char[] { ':' }, StringSplitOptions.None);
                else
                    list = data.Trim().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (list.Length < 2)
                    return null;
                return list[1];
            }
        }
        #endregion // Data
    }
}
