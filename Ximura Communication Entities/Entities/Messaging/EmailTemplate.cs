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
using Ximura.Communication;
#endregion // using
namespace Ximura.Communication
{
    /// <summary>
    /// This is the email template class.
    /// </summary>
    [XimuraContentTypeID("D27CAB41-91C4-4e0c-AEF6-DEF73A74EFF6")]
    [XimuraDataContentSchema("http://schema.ximura.org/email/1.0",
        "xmrres://XimuraCommEntities/Ximura.Communication.EmailTemplate/Ximura.Communication.Resources.EmailTemplate.xsd")]
    [XimuraDataContentDefault(
        "xmrres://XimuraCommEntities/Ximura.Communication.EmailTemplate/Ximura.Communication.Resources.EmailTemplate_NewData.xml", false)]
    public class EmailTemplate : MessageTemplateBase
    {
        #region Constructor
        /// <summary>
        /// The default constructor
        /// </summary>
        public EmailTemplate()
        { }

        /// <summary>
        /// This is the deserialization constructor. 
        /// </summary>
        /// <param name="info">The Serialization info object that contains all the relevant data.</param>
        /// <param name="context">The serialization context.</param>
        public EmailTemplate(SerializationInfo info, StreamingContext context)
            :
            base(info, context) { }
        #endregion

        #region XPScAdd(Dictionary<string, string> mappingShortcuts)
        /// <summary>
        /// This method adds the XPath shortcuts in to the collection. You should
        /// override this method to add your own shorcuts.
        /// </summary>
        /// <param name="mappingShortcuts">The mapping shorcut collection.</param>
        protected override void XPScAdd(Dictionary<string, string> mappingShortcuts)
        {
            string basePath = "//r:EmailTemplate";
            mappingShortcuts.Add("r", basePath);
            mappingShortcuts.Add("rr", basePath + "/r:Recipients");
            mappingShortcuts.Add("m", basePath + "/r:Message");
        }
        #endregion // XPScAdd(Dictionary<string, string> mappingShortcuts)
        #region NamespaceDefaultShortName
        /// <summary>
        /// This is the short name used in the namespace manager to refer to the root namespace.
        /// </summary>
        protected override string NamespaceDefaultShortName
        {
            get
            {
                return "r";
            }
        }
        #endregion // NamespaceDefaultShortName

        #region Load()
        /// <summary>
        /// This override sets the unique IDs for the new content.
        /// </summary>
        /// <returns>Returns true.</returns>
        public override bool Load()
        {
            bool response = base.Load();
            IDContent = Guid.NewGuid();
            IDVersion = Guid.NewGuid();
            return response;
        }
        #endregion // Load()

        #region AddressSender
        /// <summary>
        /// This is the sender address.
        /// </summary>
        public MailAddress AddressSender
        {
            get { return EmailAddressGet(XPSc("r", "AddressSender")); }
            set { EmailAddressSet(XPSc("r", "AddressSender"), value); }
        }
        #endregion // AddressSender
        #region AddressFrom
        /// <summary>
        /// This is the address from.
        /// </summary>
        public MailAddress AddressFrom
        {
            get { return EmailAddressGet(XPSc("r", "AddressFrom")); }
            set { EmailAddressSet(XPSc("r", "AddressFrom"), value); }
        }
        #endregion // AddressFrom
        #region AddressReplyTo
        /// <summary>
        /// This is the reply to address.
        /// </summary>
        public MailAddress AddressReplyTo
        {
            get { return EmailAddressGet(XPSc("r", "AddressReplyTo")); }
            set { EmailAddressSet(XPSc("r", "AddressReplyTo"), value); }
        }
        #endregion // AddressReplyTo


        protected void EmailAddressSet(string xPath, MailAddress address)
        {
            XmlElement elem = (XmlElement)XmlDataDoc.SelectSingleNode(xPath, NSM);

            elem.InnerText = address.DisplayName;
            elem.Attributes["address"].Value = address.Address;
        }

        protected MailAddress EmailAddressGet(string xPath)
        {
            XmlElement elem = (XmlElement)XmlDataDoc.SelectSingleNode(xPath, NSM);

            return EmailAddressGet(elem);
        }

        protected MailAddress EmailAddressGet(XmlElement elem)
        {
            try
            {
                if (elem == null)
                    return null;

                XmlAttribute addr = (XmlAttribute)elem.Attributes["address"];

                if (addr == null || addr.Value == "")
                    return null;

                return new MailAddress(addr.Value, elem.InnerText);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        [CDSReference("emailpath")]
        public string EmailPath
        {
            get
            {
                return TemplateAddress.ToString();
            }
        }

        /// <summary>
        /// This enumeration contains a collection of the email destinations for the email.
        /// </summary>
        public IEnumerable<MailAddress> AddressDestinations(EmailDestinationType type)
        {
            //Get the address type shorthand.
            string addressType = EmailDestinationTypeConvert(type);

            XmlNodeList items = this.XmlDataDoc.SelectNodes(XPSc("r", string.Format("Recipients[@type='{0}']", addressType), "Recipient"), NSM);

            foreach (XmlNode node in items)
            {
                MailAddress addr = EmailAddressGet((XmlElement)node);
                if (addr == null)
                    continue;

                yield return addr;
            }
        }

        private string EmailDestinationTypeConvert(EmailDestinationType type)
        {
            string addressType = "to";
            switch (type)
            {
                case EmailDestinationType.AddressTo:
                    break;
                case EmailDestinationType.AddressCc:
                    addressType = "cc";
                    break;
                case EmailDestinationType.AddressBcc:
                    addressType = "bcc";
                    break;
            }
            return addressType;
        }


        public bool AddressDestinationAdd(EmailDestinationType type, MailAddress destination)
        {
            //Get the address type shorthand.
            string addressType = EmailDestinationTypeConvert(type);
            
            try
            {
                XmlNode parent = this.XmlDataDoc.SelectSingleNode(XPSc("r", string.Format("Recipients[@type='{0}']", addressType)), NSM);

                XmlElementAdd(parent, "Recipient", destination.DisplayName, XmlAttributeCreate("address", destination.Address));
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }

            return true;
        }

        public bool AddressDestinationRemove(EmailDestinationType type, MailAddress destination)
        {
            return false;
        }


    }
    /// <summary>
    /// This is the email destination type.
    /// </summary>
    public enum EmailDestinationType
    {
        /// <summary>
        /// To.
        /// </summary>
        AddressTo,
        /// <summary>
        /// Carbon Copy
        /// </summary>
        AddressCc,
        /// <summary>
        /// Blind Carbon Copy
        /// </summary>
        AddressBcc
    }
}
