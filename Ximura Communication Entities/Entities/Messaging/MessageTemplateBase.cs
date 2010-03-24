#region using
using System;
using System.Data;
using System.Text;
using System.Xml;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Net.Mail;

using Ximura;
using Ximura.Data;

using CH = Ximura.Common;
using Ximura.Framework;
using Ximura.Framework;
using Ximura.Communication;
#endregion // using
namespace Ximura.Communication
{
    /// <summary>
    /// The message template class is the base class for external messaging.
    /// </summary>
    [XimuraDataContentSchema("http://schema.ximura.org/messagetemplate/1.0",
        "xmrres://XimuraCommEntities/Ximura.Communication.MessageTemplate/Ximura.Communication.Resources.MessageTemplate.xsd")]
    public class MessageTemplateBase : XimuraCore
    {
        #region Constructor
        /// <summary>
        /// The default constructor
        /// </summary>
        public MessageTemplateBase() { }

        /// <summary>
        /// This is the deserialization constructor. 
        /// </summary>
        /// <param name="info">The Serialization info object that contains all the relevant data.</param>
        /// <param name="context">The serialization context.</param>
        public MessageTemplateBase(SerializationInfo info, StreamingContext context)
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
            nsm.AddNamespace("mtemp", "http://schema.ximura.org/messagetemplate/1.0");
        }
        #endregion // NamespaceManagerAdd(XmlNamespaceManager nsm)

        #region TemplateAddress
        /// <summary>
        /// This is the template address for the Site Controller
        /// </summary>
        public Uri TemplateAddress
        {
            get
            {
                string uri = XmlMappingGetToString(XPSc("m", "mtemp:TemplateAddress"));
                if (uri == null || uri == "")
                    return null;

                return new Uri(uri);
            }
            set { XmlMappingSet(XPSc("m", "mtemp:TemplateAddress"), value.ToString()); }
        }
        #endregion // TemplateAddress


        #region ParameterAdd(string type, string value)
        /// <summary>
        /// This method adds a parameter to the message class.
        /// </summary>
        /// <param name="type">The parameter type.</param>
        /// <param name="value">The paramter value.</param>
        /// <returns>Returns true if the parameter was added successfully.</returns>
        public bool ParameterAdd(string type, string value)
        {
            try
            {
                XmlNode parent = this.XmlDataDoc.SelectSingleNode(XPSc("m", "mtemp:Parameters"), NSM);

                XmlElement elem = XmlDataDoc.CreateElement("mtemp", "Parameter", "http://schema.ximura.org/messagetemplate/1.0");

                elem.Attributes.Append(XmlAttributeCreate("type", type));

                elem.InnerText = value;

                elem = (XmlElement)parent.AppendChild(elem);

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }
            return true;
        }
        #endregion // ParameterAdd(string type, string value)
        #region ParameterRemove(string type)
        /// <summary>
        /// This method removes a parameter from the collection.
        /// </summary>
        /// <param name="type">The collection name.</param>
        /// <returns>Returns true if the parameter was found and removed successfully.</returns>
        public bool ParameterRemove(string type)
        {

            XmlNode node = (XmlElement)this.XmlDataDoc.SelectSingleNode(XPSc("m", "mtemp:Parameters",
                string.Format("mtemp:Parameter[@type='{0}']", type)), NSM);

            if (node == null)
                return false;

            node.ParentNode.RemoveChild(node);

            return true;
        }
        #endregion // ParameterRemove(string type)
        #region ParameterGet(string type)
        /// <summary>
        /// This method retrieves a parameter from the collection.
        /// </summary>
        /// <param name="type">The parameter type to display.</param>
        /// <returns>Returns the parameter as a string.</returns>
        public string ParameterGet(string type)
        {
            XmlNode node = (XmlElement)this.XmlDataDoc.SelectSingleNode(XPSc("m", "mtemp:Parameters",
                string.Format("mtemp:Parameter[@type='{0}']", type)), NSM);

            if (node == null)
                return null;

            return node.InnerText;
        }
        #endregion // ParameterGet(string type)


        #region Priority
        /// <summary>
        /// Priority
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual EmailPriority Priority
        {
            get
            {
                switch (XmlMappingGetToString(XPSc("m", "mtemp:Priority")))
                {
                    case "1":
                        return EmailPriority.Low;
                    case "2":
                        return EmailPriority.High;
                    default:
                        return EmailPriority.Normal;
                }
            }
            set { XmlMappingSet(XPSc("m", "mtemp:Priority"), value.ToString()); }
        }
        #endregion // Priority
        #region CreateDate
        /// <summary>
        /// Create Date
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual DateTime? CreateDate
        {
            get
            {
                string item = XmlMappingGetToString(XPSc("m", "mtemp:CreateDate"));
                return (item == null || item == "") ? (DateTime?)null : DateTime.Parse(item);
            }
            set
            {
                if (value.HasValue)
                    XmlMappingSet(XPSc("m", "mtemp:CreateDate"), CH.ConvertToISO8601DateStringWithOffset(value.Value));
                else
                    XmlMappingSet(XPSc("m", "mtemp:CreateDate"), "");
            }
        }
        #endregion // CreateDate

        #region Subject
        /// <summary>
        /// Subject
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual string Subject
        {
            get { return XmlMappingGetToString(XPSc("m", "mtemp:Subject")); }
            set { XmlMappingSet(XPSc("m", "mtemp:Subject"), value); }
        }
        #endregion // Subject
        #region SubjectEncoding
        /// <summary>
        /// SubjectEncoding
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual Encoding SubjectEncoding
        {
            get
            {
                string encName = XmlMappingGetToString(XPSc("m", "mtemp:SubjectEncoding"));
                return (encName == null || encName == "") ?
                    Encoding.UTF8 : Encoding.GetEncoding(encName);
            }
            set { XmlMappingSet(XPSc("m", "mtemp:SubjectEncoding"), value.EncodingName); }
        }
        #endregion // SubjectEncoding

        #region Status
        /// <summary>
        /// Subject
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual string Status
        {
            get { return XmlMappingGetToString(XPSc("m", "mtemp:Status")); }
            set { XmlMappingSet(XPSc("m", "mtemp:Status"), value); }
        }
        #endregion
    }
}
