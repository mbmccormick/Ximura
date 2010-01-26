#region using
using System;
using System.Xml;
using System.Data;
using System.ComponentModel;
using System.Collections;
using System.Diagnostics;
using System.Runtime.Serialization;

using Ximura;
using Ximura.Data;
using Ximura.Helper;
using CH = Ximura.Helper.Common;
using Ximura.Server;
using Ximura.Command;
using Ximura.Communication;
#endregion // using
namespace Ximura.Communication
{
    [XimuraDataContentSchemaReference("http://purl.org/dc/elements/1.1/",
      "xmrres://XimuraCommEntities/Ximura.Communication.DublinCore/Ximura.Communication.Resources.DublinCore.xsd")]
    public class DublinCore : XimuraCore
    {
        #region Constructor
        /// <summary>
        /// The default constructor
        /// </summary>
        public DublinCore() 
        { }

        /// <summary>
        /// This is the deserialization constructor. 
        /// </summary>
        /// <param name="info">The Serialization info object that contains all the relevant data.</param>
        /// <param name="context">The serialization context.</param>
        public DublinCore(SerializationInfo info, StreamingContext context)
            :
            base(info, context) { }
        #endregion

        #region NamespaceManagerAdd(XmlNamespaceManager nsm)
        /// <summary>
        /// This override adds the ximura namespace to the default Namespace manager.
        /// </summary>
        /// <param name="nsm">The system default namespace manager.</param>
        protected override void NamespaceManagerAdd(XmlNamespaceManager nsm)
        {
            base.NamespaceManagerAdd(nsm);
            nsm.AddNamespace("dublin", "http://purl.org/dc/elements/1.1/");
        }
        #endregion // NamespaceManagerAdd(XmlNamespaceManager nsm)

        #region DCTitle
        /// <summary>
        /// This is the Dublin Core Title
        /// </summary>
        public virtual string DCTitle
        {
            get { return XmlMappingGetToString(XPSc("r", "dublin:title")); }
            set { XmlMappingSet(XPSc("r", "dublin:title"), value); }
        }
        #endregion // DCTitle
        #region DCDescription
        /// <summary>
        /// This is the Dublin Core Description
        /// </summary>
        public virtual string DCDescription
        {
            get { return XmlMappingGetToString(XPSc("r", "dublin:description")); }
            set { XmlMappingSet(XPSc("r", "dublin:description"), value); }
        }
        #endregion // DCDescription

        #region DCCreator
        /// <summary>
        /// This is the Dublin Core Creator
        /// </summary>
        public virtual string DCCreator
        {
            get { return XmlMappingGetToString(XPSc("r", "dublin:creator")); }
            set { XmlMappingSet(XPSc("r", "dublin:creator"), value); }
        }
        #endregion // DCCreator

        #region DCPublisher
        /// <summary>
        /// This is the Dublin Core Publisher
        /// </summary>
        public virtual string DCPublisher
        {
            get { return XmlMappingGetToString(XPSc("r", "dublin:publisher")); }
            set { XmlMappingSet(XPSc("r", "dublin:publisher"), value); }
        }
        #endregion // DCPublisher

        #region DCDate
        /// <summary>
        /// This is the Dublin Core Date
        /// </summary>
        public virtual string DCDate
        {
            get { return XmlMappingGetToString(XPSc("r", "dublin:date")); }
            set { XmlMappingSet(XPSc("r", "dublin:date"), value); }
        }
        #endregion // DCDate

        #region DCContributor
        /// <summary>
        /// This is the Dublin Core Contributor
        /// </summary>
        public virtual string DCContributor
        {
            get { return XmlMappingGetToString(XPSc("r", "dublin:contributor")); }
            set 
            {
                string xPath = XPSc("r", "dublin:contributor");
                XmlNode toSet = this.XmlDataDoc.SelectSingleNode(xPath, NSM);
                if (toSet == null)
                {
                    XmlNode before = XmlDataDoc.SelectSingleNode(XPSc("r", "dublin:publisher"), NSM);
                    XmlElement elem = XmlDataDoc.CreateElement("contributor", "http://purl.org/dc/elements/1.1/");
                    elem.InnerText = value;
                    before.ParentNode.InsertAfter(elem, before);
                    return;
                }
                XmlMappingSet<string>(toSet, value, delegate(string inputData) { return inputData; });
            }
        }
        #endregion // DCContributor

        #region DCSource
        /// <summary>
        /// This is the Dublin Core Source
        /// </summary>
        public virtual string DCSource
        {
            get { return XmlMappingGetToString(XPSc("r", "dublin:source")); }
            set { XmlMappingSet(XPSc("r", "dublin:source"), value); }
        }
        #endregion // DCSource

    }
}
