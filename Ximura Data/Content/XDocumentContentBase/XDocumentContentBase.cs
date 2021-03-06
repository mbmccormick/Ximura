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
﻿#region using
using System;
using System.Data;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Runtime.Serialization;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

using Ximura;
using Ximura.Data;
using CH = Ximura.Common;
#endregion // using
namespace Ximura.Data
{
    /// <summary>
    /// This is the base XML content class using the new .NET 3.5 Linq XML classes.
    /// </summary>
    public partial class XDocumentContentBase : XmlContentHolder<XDocument>
    {
        #region Declarations
        private Dictionary<string, XNamespace> mNSM = null;
        #endregion // Declarations
        #region Constructors
        /// <summary>
        /// This is the default constructor for the Content object.
        /// </summary>
        public XDocumentContentBase()
        { 
            XPScBuild();
        }

        ///// <summary>
        ///// This is the deserialization constructor. 
        ///// </summary>
        ///// <param name="info">The Serialization info object that contains all the relevant data.</param>
        ///// <param name="context">The serialization context.</param>
        //public XDocumentContentBase(SerializationInfo info, StreamingContext context)
        //    : base(info, context)
        //{
        //    XPScBuild();
        //}
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
                return mData;
            }
            set
            {
                mData = value;

                if (value != null)
                    mNSM.Clear();

                //NamespaceManagerBuild();
            }
        }
        #endregion // XmlDataDoc


        protected override XmlNamespaceManager NamespaceManagerCreate()
        {
            throw new NotImplementedException();
        }
    }
}
