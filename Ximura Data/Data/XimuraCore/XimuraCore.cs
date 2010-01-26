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
using System.Text;
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
using RH = Ximura.Helper.Reflection;
#endregion // using
namespace Ximura.Data
{
    /// <summary>
    /// The ximura core content has the necessary overrides to automatically implement
    /// the ximura:entity namespace overrides for the TypeID, ID and Version attributes.
    /// </summary>
    public class XimuraCore : XmlContent
	{
        #region Constructor
        /// <summary>
        /// The default constructor
        /// </summary>
        public XimuraCore() { }

        /// <summary>
        /// This is the deserialization constructor. 
        /// </summary>
        /// <param name="info">The Serialization info object that contains all the relevant data.</param>
        /// <param name="context">The serialization context.</param>
        public XimuraCore(SerializationInfo info, StreamingContext context)
            :
            base(info, context) { }
        #endregion

        #region IDType
        /// <summary>
        /// This is the TypeID of the entity.
        /// </summary>
        public override Guid IDType
        {
            get
            {
                XmlNode node = XmlDataDoc.SelectSingleNode("//ximura:entity/ximura:tid", NSM);
                if (node != null && node.InnerText != null && node.InnerText != "")
                    return new Guid(node.InnerText);

                return base.IDType;
            }
            protected set
            {
                XmlNode node = XmlDataDoc.SelectSingleNode("//ximura:entity/ximura:tid", NSM);
                if (node != null)
                    node.InnerText = value.ToString().ToUpperInvariant();
                else
                    EntityNodeCreate(value, IDContent, IDVersion);
            }
        }
        #endregion // TypeID
        #region IDContent
        /// <summary>
        /// This is the ID of the entity.
        /// </summary>
        public override Guid IDContent
        {
            get
            {
                XmlNode node = XmlDataDoc.SelectSingleNode("//ximura:entity/ximura:cid", NSM);
                if (node != null && node.InnerText != null && node.InnerText != "")
                    return new Guid(node.InnerText);

                return base.IDContent;
            }
            set
            {
                XmlNode node = XmlDataDoc.SelectSingleNode("//ximura:entity/ximura:cid", NSM);
                if (node != null)
                    node.InnerText = value.ToString().ToUpperInvariant();
                else
                    EntityNodeCreate(IDType, value, IDVersion);
            }
        }
        #endregion // ID
        #region IDVersion
        /// <summary>
        /// This is the version of the entity.
        /// </summary>
        public override Guid IDVersion
        {
            get
            {
                XmlNode node = XmlDataDoc.SelectSingleNode("//ximura:entity/ximura:vid", NSM);
                if (node != null && node.InnerText != null && node.InnerText != "")
                    return new Guid(node.InnerText);

                return base.IDVersion;
            }
            set
            {
                XmlNode node = XmlDataDoc.SelectSingleNode("//ximura:entity/ximura:vid", NSM);
                if (node != null)
                    node.InnerText = value.ToString().ToUpperInvariant();
                else
                    EntityNodeCreate(IDType, IDContent, value);
            }
        }
        #endregion // Version

        #region NamespaceManagerAdd(XmlNamespaceManager nsm)
        /// <summary>
        /// This override adds the ximura namespace to the default Namespace manager.
        /// </summary>
        /// <param name="nsm">The system default namespace manager.</param>
        protected override void NamespaceManagerAdd(XmlNamespaceManager nsm)
        {
            base.NamespaceManagerAdd(nsm);
            nsm.AddNamespace("ximura", "http://schema.ximura.org/core");
        }
        #endregion // NamespaceManagerAdd(XmlNamespaceManager nsm)

        #region EntityNodeCreate(Guid tid, Guid cid, Guid vid)
        /// <summary>
        /// This method creates the entity node. You should override this method if want to alter the behaviour.
        /// </summary>
        /// <param name="tid">The type id.</param>
        /// <param name="cid">The content id.</param>
        /// <param name="vid">The version id.</param>
        protected virtual void EntityNodeCreate(Guid tid, Guid cid, Guid vid)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(@"<entity xmlns=""http://schema.ximura.org/core"">");

            sb.Append(@"<tid xmlns=""http://schema.ximura.org/core"">");
            sb.Append(tid.ToString().ToUpperInvariant());
            sb.AppendLine(@"</tid>");

            sb.Append(@"<cid xmlns=""http://schema.ximura.org/core"">");
            sb.Append(cid.ToString().ToUpperInvariant());
            sb.AppendLine(@"</cid>");

            sb.Append(@"<vid xmlns=""http://schema.ximura.org/core"">");
            sb.Append(vid.ToString().ToUpperInvariant());
            sb.AppendLine(@"</vid>");

            sb.AppendLine(@"</entity>");

            //bool constraint = XmlDataDoc.DataSet.EnforceConstraints;
            //XmlDataDoc.DataSet.EnforceConstraints = false;

            XmlDocumentFragment frag = XmlDataDoc.CreateDocumentFragment();
            frag.InnerXml = sb.ToString();

            XmlNode rootNode = XmlDataDoc.SelectSingleNode(XPSc("r"),NSM);
            //this is tricky one, but basically the EntityID should be the first item in the document.
            if (rootNode.HasChildNodes)
                rootNode.InsertBefore(frag, rootNode.FirstChild);
            else
                rootNode.AppendChild(frag);

            //XmlDataDoc.DataSet.EnforceConstraints = constraint;

            //return elem;
        }
        #endregion // EntityNodeCreate(Guid tid, Guid cid, Guid vid)
    }
}
