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
using System.Xml.Schema;
using System.Xml.XPath;
using System.Xml.Xsl;
using System.Reflection;

using Ximura;
using Ximura.Data;
using Ximura.Server;
using Ximura.Helper;
using CH = Ximura.Helper.Common;
#endregion // using
namespace Ximura.Data
{
    /// <summary>
    /// This abstract class contains support for XML schema caching.
    /// </summary>
    public abstract partial class XmlContentBase : Content, IXPathNavigable
    {
        #region Declarations
        private XmlDocument internalXmlDataDoc;
        private Dictionary<string, string> mXMLMappingShortcuts = null;
        private XmlNamespaceManager mNSM;
        #endregion // Declarations
		#region Constructors
		/// <summary>
		/// This is the default constructor for the Content object.
		/// </summary>
		public XmlContentBase():this((IContainer)null){}
		/// <summary>
		/// This constructor is called by .NET when it added as new to a container.
		/// </summary>
		/// <param name="container">The container this component should be added to.</param>
		public XmlContentBase(System.ComponentModel.IContainer container):base(container)
		{
            XPScBuild();
		}
		/// <summary>
		/// This is the deserialization constructor. 
		/// </summary>
		/// <param name="info">The Serialization info object that contains all the relevant data.</param>
		/// <param name="context">The serialization context.</param>
        public XmlContentBase(SerializationInfo info, StreamingContext context)
            : base(info, context)
		{
            XPScBuild();
		}
		#endregion // Deserialization Constructor

        #region XmlDataDoc
        /// <summary>
        /// This is the base XML data document.
        /// </summary>
        public XmlDocument XmlDataDoc
        {
            get
            {
                return internalXmlDataDoc;
            }
            set
            {
                internalXmlDataDoc = value;

                if (value == null)
                    mNSM = null;

                //NamespaceManagerBuild();
            }
        }
        #endregion // XmlDataDoc

        #region IXPathNavigable Members

        XPathNavigator IXPathNavigable.CreateNavigator()
        {
            return internalXmlDataDoc.CreateNavigator();
        }

        #endregion
    }
}