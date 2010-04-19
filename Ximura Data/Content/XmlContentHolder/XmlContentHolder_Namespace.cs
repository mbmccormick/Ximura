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
    public abstract partial class XmlContentHolder<T>
    {
        #region NamespaceManagerBuild()
        /// <summary>
        /// This method is called by the constructor, and is used to construct the default 
        /// namespace manager.
        /// </summary>
        protected virtual void NamespaceManagerBuild()
        {
            if (Payload == null)
            {
                mNSM = null;
                return;
            }

            mNSM = NamespaceManagerCreate();

            //XmlNamespaceManager nsm = new XmlNamespaceManager(Payload.NameTable);
            //NamespaceManagerAdd(nsm);
            //mNSM = nsm;
        }
        #endregion // NamespaceManagerBuild()

        #region NamespaceManagerCreate()
        /// <summary>
        /// This abstract method creates the namespace manager.
        /// </summary>
        /// <returns>The namespace manager for the XML data.</returns>
        protected abstract XmlNamespaceManager NamespaceManagerCreate(); 
        #endregion

        #region NSM
        /// <summary>
        /// This is the default namespace manager for the content.
        /// </summary>
        protected virtual XmlNamespaceManager NSM
        {
            get
            {
                if (mNSM == null)
                    NamespaceManagerBuild();
                return mNSM;
            }
        }
        #endregion // NSM
        #region NamespaceManagerAdd(XmlNamespaceManager nsm)
        /// <summary>
        /// This method add the namespaces for the document. You can add custom namespaces by overriding 
        /// this method.
        /// </summary>
        /// <param name="nsm">The namespace manager for the document.</param>
        protected virtual void NamespaceManagerAdd(XmlNamespaceManager nsm)
        {
            nsm.AddNamespace(string.Empty, NamespaceDefault.ToString());
            if (NamespaceDefaultShortName != string.Empty)
                nsm.AddNamespace(NamespaceDefaultShortName, NamespaceDefault.ToString());
        }
        #endregion // NamespaceManagerAdd(XmlNamespaceManager nsm)
        #region NamespaceDefault
        /// <summary>
        /// This is the default namespace for the document.
        /// </summary>
        protected virtual Uri NamespaceDefault
        {
            get
            {
                if (attrSchemaPrimary == null)
                    return null;

                return attrSchemaPrimary.UriPath;
            }
        }
        #endregion // DefaultNamespace
        #region NamespaceDefaultShortName
        /// <summary>
        /// This is the short name used in the namespace manager to refer to the root namespace.
        /// </summary>
        protected virtual string NamespaceDefaultShortName
        {
            get
            {
                return string.Empty;
            }
        }
        #endregion // NamespaceDefaultShortName

        #region XPSc
        protected virtual string XPSc(string mapID, params string[] items)
        {
            if (!mXMLMappingShortcuts.ContainsKey(mapID))
                throw new ArgumentOutOfRangeException(@"The mapID """ + mapID + @""" is not found.");

            string toReturn = "";
            foreach (string item in items)
            {
                bool doNotAddNamespace = NamespaceDefaultShortName == string.Empty || item.Contains(":");
                toReturn += @"/" + (string)(!doNotAddNamespace ? NamespaceDefaultShortName + @":" : @"") + item;
            }

            return mXMLMappingShortcuts[mapID] + toReturn;
        }
        #endregion // XPSc
        #region XPScA
        protected virtual string XPScA(string mapID, string attr, params string[] items)
        {
            return XPSc(mapID, items) + "/@" + attr;
        }
        #endregion // XPScA

        #region XPScBuild()
        /// <summary>
        /// This method creates the XPath shorcut collection.
        /// </summary>
        private void XPScBuild()
        {
            mXMLMappingShortcuts = new Dictionary<string, string>();
            XPScAdd(mXMLMappingShortcuts);
        }
        #endregion // XPScBuild()

        #region XPScAdd(Dictionary<string, string> mappingShortcuts)
        /// <summary>
        /// This method adds the XPath shortcuts in to the collection. You should
        /// override this method to add your own shorcuts.
        /// </summary>
        /// <param name="mappingShortcuts">The mapping shorcut collection.</param>
        protected virtual void XPScAdd(Dictionary<string, string> mappingShortcuts)
        {
        }
        #endregion // XPScAdd(Dictionary<string, string> mappingShortcuts)
    }
}
