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
﻿#region using
using System;
using System.Data;
using System.Linq;
using System.Xml.Linq;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Runtime.Serialization;
using System.Diagnostics;
using System.IO;
using System.Reflection;

using Ximura;
using Ximura.Data;
using Ximura.Server;
using Ximura.Helper;
using CH = Ximura.Helper.Common;
#endregion // using
namespace Ximura.Data.Linq
{
    /// <summary>
    /// This is the base XML content class using the new .NET 3.5 Linq XML classes.
    /// </summary>
    public partial class XContent : Content
    {
        #region Declarations
        private XDocument internalXDataDoc;
        private Dictionary<string, string> mXMLMappingShortcuts = null;
        private Dictionary<string, XNamespace> mNSM = null;
        #endregion // Declarations
        #region Constructors
        /// <summary>
        /// This is the default constructor for the Content object.
        /// </summary>
        public XContent()
        { 
            XPScBuild();
        }

        /// <summary>
        /// This is the deserialization constructor. 
        /// </summary>
        /// <param name="info">The Serialization info object that contains all the relevant data.</param>
        /// <param name="context">The serialization context.</param>
        public XContent(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            XPScBuild();
        }
        #endregion // Deserialization Constructor

        #region Reset()
        /// <summary>
        /// This method resets the XmlContent object to its initial state.
        /// </summary>
        public override void Reset()
        {
            XDataDoc = null;

            if (mNSM != null)
                mNSM.Clear();
            else
                mNSM = new Dictionary<string, XNamespace>();

            base.Reset();
        }
        #endregion // Reset()

        #region XmlDataDoc
        /// <summary>
        /// This is the base XML data document.
        /// </summary>
        public XDocument XDataDoc
        {
            get
            {
                return internalXDataDoc;
            }
            set
            {
                internalXDataDoc = value;

                if (value != null)
                    mNSM.Clear();

                //NamespaceManagerBuild();
            }
        }
        #endregion // XmlDataDoc

    }
}
