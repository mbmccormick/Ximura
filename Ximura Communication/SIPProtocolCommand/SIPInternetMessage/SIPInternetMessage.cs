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
using System.Drawing;
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

using XIMS;
using XIMS.Data;
using XIMS.Applications;
using XIMS.Helper;
using CH = XIMS.Helper.Common;
#endregion // using
namespace XIMS.Communication
{
    /// <summary>
    /// The SIPInternetMessage contains specific headers for SIP based internet messaging.
    /// </summary>
    public class SIPInternetMessage: InternetMessage
    {
        #region Constructor
        /// <summary>
        /// The default constructor
        /// </summary>
        public SIPInternetMessage()
            : this((IContainer)null)
        {

        }
        /// <summary>
        /// This constructor is called by .NET when it added as new to a container.
        /// </summary>
        /// <param name="container">The container this component should be added to.</param>
        public SIPInternetMessage(System.ComponentModel.IContainer container)
            : base(container)
        {

        }
        /// <summary>
        /// This is the deserialization constructor. 
        /// </summary>
        /// <param name="info">The Serialization info object that contains all the relevant data.</param>
        /// <param name="context">The serialization context.</param>
        public SIPInternetMessage(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {

        }
        #endregion
    }
}
