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
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Reflection;
using System.Diagnostics;

using Ximura;
using Ximura.Data;
#endregion // using
namespace Ximura.Communication
{
    /// <summary>
    /// This is the request message.
    /// </summary>
    public class RequestMessage : Message
    {
        #region Declarations
        #endregion // Declarations
        #region Constructor
        /// <summary>
        /// The default constructor
        /// </summary>
        public RequestMessage()
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

        //#region DefaultEncoding
        ///// <summary>
        ///// The default encoding for a FTP message is in ASCII.
        ///// </summary>
        //public override Encoding DefaultEncoding
        //{
        //    get
        //    {
        //        return Encoding.ASCII;
        //    }
        //    protected set
        //    {

        //    }
        //}
        //#endregion // DefaultEncoding

        #region FragmentInitialType
        /// <summary>
        /// This is the fragment type for the outgoing message.
        /// </summary>
        protected override Type FragmentHeaderInitialType
        {
            get
            {
                return typeof(MessageCRLFFragment);
            }
        }
        #endregion // FragmentInitialType

        #region Verb
        /// <summary>
        /// The request verb.
        /// </summary>
        public string Verb
        {
            get
            {
                string data = ((MessageCRLFFragment)FragmentFirst).DataString;
                string[] list = data.Trim().Split(new char[] { ' ' }, StringSplitOptions.None);
                return list[0];
            }
        }
        #endregion // Verb
        #region Data
        /// <summary>
        /// The request data.
        /// </summary>
        public string Data
        {
            get
            {
                string data = ((MessageCRLFFragment)FragmentFirst).DataString;
                string[] list = data.Trim().Split(new char[] { ' ' }, StringSplitOptions.None);
                if (list.Length < 2)
                    return null;
                return list[1];
            }
        }
        #endregion // Data

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
