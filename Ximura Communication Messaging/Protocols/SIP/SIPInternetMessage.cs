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
using System.Data;

using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Runtime.Serialization;
using System.Diagnostics;
using System.IO;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;
using System.Reflection;

using Ximura;
using Ximura.Data;
using Ximura.Framework;

using CH = Ximura.Common;
#endregion // using
namespace Ximura.Communication
{
    /// <summary>
    /// The SIPInternetMessage contains specific headers for SIP based internet messaging.
    /// </summary>
    public class SIPRequestInternetMessage : InternetMessageRequest
    {
        #region Constructor
        /// <summary>
        /// The default constructor
        /// </summary>
        public SIPRequestInternetMessage()
            : base()
        {

        }
        #endregion

        protected override Type FragmentHeaderInitialType
        {
            get
            {
                return base.FragmentHeaderInitialType;
            }
        }
    }

    /// <summary>
    /// The SIPInternetMessage contains specific headers for SIP based internet messaging.
    /// </summary>
    public class SIPResponseInternetMessage : InternetMessageResponse
    {
        #region Constructor
        /// <summary>
        /// The default constructor
        /// </summary>
        public SIPResponseInternetMessage()
            : base()
        {

        }
        #endregion
    }
}
