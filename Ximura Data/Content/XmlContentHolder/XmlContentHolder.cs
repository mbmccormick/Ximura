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
using System.IO;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Diagnostics;
using System.Text;
using System.Xml.XPath;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml;

using Ximura;
using Ximura.Data;
using CH = Ximura.Common;
using AH = Ximura.AttributeHelper;
#endregion // using
namespace Ximura.Data
{
    /// <summary>
    /// This abstract class wraps a content based entity around a XML base object.
    /// </summary>
    /// <typeparam name="T">An XML object that implements IXPathNavigable.</typeparam>
    public abstract partial class XmlContentHolder<T> : ContentHolder<T>
        where T : class
    {
        #region Declarations
        /// <summary>
        /// This dictionary contains the XML mapping shortcuts.
        /// </summary>
        protected Dictionary<string, string> mXMLMappingShortcuts = null;
        /// <summary>
        /// This is the namespace manager for the collection.
        /// </summary>
        protected XmlNamespaceManager mNSM; 
        #endregion

        #region Payload
        /// <summary>
        /// This is the XML base object payload.
        /// </summary>
        public override T Payload
        {
            get
            {
                return base.Payload;
            }
            set
            {
                base.Payload = value;

                if (value == null)
                    mNSM = null;
            }
        } 
        #endregion
    }
}
