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
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.XPath;
using System.Xml.Xsl;
using System.Reflection;
using System.Net;

using Ximura;
using Ximura.Data;
using CH = Ximura.Common;
#endregion // using
namespace Ximura.Data
{
    /// <summary>
    /// The XMLContent object wraps basic persistence functionality around the XMl Data object.
    /// </summary>
    public partial class XmlContentBase
    {
        #region XmlAttributeAdd
        protected void XmlAttributeAdd(XmlElement elem, string field, string data)
        {
            elem.Attributes.Append(XmlAttributeCreate(field, data));
        }
        #endregion // XmlAttributeAdd
        #region XmlAttributeCreate
        protected XmlAttribute XmlAttributeCreate(string field, string data)
        {
            XmlAttribute attr = XmlDataDoc.CreateAttribute(field);
            attr.Value = data;
            return attr;
        }
        #endregion // XmlAttributeCreate
        #region XmlElementAdd
        protected XmlElement XmlElementAdd(XmlNode parent, string field, string data)
        {
            return XmlElementAdd(parent, field, data, (XmlAttribute[])null);
        }

        protected XmlElement XmlElementAdd(XmlNode parent, string field, string data, XmlAttribute attr)
        {
            return XmlElementAdd(parent, field, data, new XmlAttribute[] { attr });
        }

        protected XmlElement XmlElementAdd(XmlNode parent, string field, string data, XmlAttribute[] attrs)
        {
            //bool constraint = XmlDataDoc.DataSet.EnforceConstraints;
            //XmlDataDoc.DataSet.EnforceConstraints = false;
            XmlElement elem = XmlElementCreate(field, data, attrs);

            elem = (XmlElement)parent.AppendChild(elem);
            //XmlDataDoc.DataSet.EnforceConstraints = constraint;

            return elem;
        }
        #endregion // XmlElementAdd
        #region XmlElementCreate
        protected XmlElement XmlElementCreate(string field, string data, XmlAttribute[] attrs)
        {
            XmlElement elem = XmlDataDoc.CreateElement(field, NamespaceDefault.ToString());

            if (attrs != null)
                foreach (XmlAttribute attr in attrs)
                {
                    elem.Attributes.Append(attr);
                }

            if (data != null)
                elem.InnerText = data;

            return elem;
        }
        #endregion // XmlElementCreate
        #region XmlElementInsertAfter
        protected XmlElement XmlElementInsertAfter(XmlNode before, string field, string data)
        {
            return XmlElementInsertAfter(before, field, data, (XmlAttribute[])null);
        }

        protected XmlElement XmlElementInsertAfter(XmlNode before, string field, string data, XmlAttribute attr)
        {
            return XmlElementInsertAfter(before, field, data, new XmlAttribute[] { attr });
        }

        protected XmlElement XmlElementInsertAfter(XmlNode before, string field, string data, XmlAttribute[] attrs)
        {
            //bool constraint = XmlDataDoc.DataSet.EnforceConstraints;
            //XmlDataDoc.DataSet.EnforceConstraints = false;
            XmlElement elem = XmlDataDoc.CreateElement(field, NamespaceDefault.ToString());

            if (attrs != null)
                foreach (XmlAttribute attr in attrs)
                {
                    elem.Attributes.Append(attr);
                }

            if (data != null)
                elem.InnerText = data;

            //elem = (XmlElement)parent.AppendChild(elem);
            elem = (XmlElement)before.ParentNode.InsertAfter(elem, before);
            //XmlDataDoc.DataSet.EnforceConstraints = constraint;

            return elem;
        }
        #endregion // XmlElementAdd

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

            return mXMLMappingShortcuts[mapID]+toReturn;
        }
        #endregion // XPSc
        #region XPScA
        protected virtual string XPScA(string mapID, string attr, params string[] items)
        {
            return XPSc(mapID,items)+"/@"+ attr;
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

        #region NamespaceManagerBuild()
        /// <summary>
        /// This method is called by the constructor, and is used to construct the default 
        /// namespace manager.
        /// </summary>
        protected virtual void NamespaceManagerBuild()
        {
            if (XmlDataDoc == null)
            {
                mNSM = null;
                return;
            }

            XmlNamespaceManager nsm = new XmlNamespaceManager(XmlDataDoc.NameTable);
            NamespaceManagerAdd(nsm);
            mNSM = nsm;
        }
        #endregion // NamespaceManagerBuild()
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
            if (NamespaceDefaultShortName!=string.Empty)
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

        #region CreateFragmentAction
        /// <summary>
        /// This delegate is used to provide a generic framework for creating elements.
        /// </summary>
        /// <param name="writer">The writer the new element will be written to.</param>
        public delegate void CreateFragmentAction(XmlWriter writer);
        #endregion // CreateFragmentAction
        #region CreateFragment(CreateFragmentAction action)
        /// <summary>
        /// This method is used to create a specific element for the document.
        /// </summary>
        /// <param name="action">The action for the particular element.</param>
        /// <returns>Returns a new element.</returns>
        protected XmlDocumentFragment CreateFragment(CreateFragmentAction action)
        {
            XmlDocumentFragment frag = XmlDataDoc.CreateDocumentFragment();
            StringBuilder sb = new StringBuilder();
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.OmitXmlDeclaration = true;
            using (XmlWriter writer = XmlWriter.Create(sb, settings))
            {
                action(writer);
            }

            frag.InnerXml = sb.ToString();

            return frag;
        }
        #endregion // CreateFragment(CreateFragmentAction action)

        #region FragmentSet(string xPath, bool append, XmlDocumentFragment frag)
        protected virtual bool FragmentSet(string xPath, bool append, XmlDocumentFragment frag)
        {
            try
            {
                XmlNodeList data = XmlDataDoc.SelectNodes(xPath, NSM);
                XmlNode rootNode = XmlDataDoc.SelectSingleNode(XPSc("r"), NSM);
                if (rootNode == null)
                    return false;

                if (data.Count == 0)
                {
                    rootNode.AppendChild(frag);
                    return true;
                }

                XmlNode sibling = data[append ? data.Count - 1 : 0];
                sibling.ParentNode.InsertAfter(frag, sibling);
                if (!append)
                    sibling.ParentNode.RemoveChild(sibling);

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion // FragmentSet(string xPath, bool append, XmlDocumentFragment frag)

#if (DEBUG)
        #region DumpXML  
        /// <summary>
        /// This random generator is used to assist export the content with a unique id.
        /// </summary>
        private static Random rndUse = new Random();

        /// <summary>
        /// This method dumps xml to the current user's desktop folder.
        /// </summary>
        public virtual string DumpXML()
        {
            string id = this.IDContent.ToString() + "_" + this.IDVersion.ToString() + "_" + rndUse.Next(100).ToString();

            return DumpXML(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), id + ".xml"));
        }
        ///// <summary>
        ///// This method is used in debug to dump the internal contents of the xml object to a file
        ///// </summary>
        ///// <param name="filename">The filename to save the file to.</param>
        //public virtual string DumpXML(string filename)
        //{
        //    File.WriteAllBytes(filename, ContentBody);
        //    //Debug.WriteLine(filename);
        //    return filename;
        //}

        /// <summary>
        /// This method is used in debug to dump the internal contents of the xml object to a file
        /// </summary>
        /// <param name="filename">The filename to save the file to.</param>
        public virtual string DumpXML(string filename)
        {
            this.XmlDataDoc.Save(filename);
            //Debug.WriteLine(filename);
            return filename;
        }
        #endregion
#endif
    }
}
