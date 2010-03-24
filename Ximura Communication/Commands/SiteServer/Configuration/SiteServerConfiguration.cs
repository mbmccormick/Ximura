#region using
using System;
using System.Runtime.Serialization;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.ComponentModel;
using System.Xml;

using Ximura;
using Ximura.Data;
using Ximura.Helper;
using CH = Ximura.Helper.Common;
using Ximura.Framework;
using Ximura.Framework;
using Ximura.Communication;
using AH = Ximura.Helper.AttributeHelper;
#endregion // using
namespace Ximura.Communication
{
    /// <summary>
    /// 
    /// </summary>
    [XimuraContentTypeID("13122A62-2E86-44ec-AD59-6478D4D56799")]
    [XimuraDataContentSchema("http://schema.ximura.org/configuration/siteserver/1.0",
       "xmrres://XimuraComm/Ximura.Communication.SiteServerConfiguration/Ximura.Communication.Commands.SiteServerCommand.Configuration.SiteServerConfiguration.xsd")]
    public class SiteServerConfiguration : FSMCommandConfiguration
    {
        #region Declarations
        private static readonly Guid sSiteControllerID = new Guid("62A74574-7425-4cc2-B2F1-74EF38689725");
        #endregion // Declarations
        #region Constructor
        /// <summary>
        /// The default constructor
        /// </summary>
        public SiteServerConfiguration() { }

        /// <summary>
        /// This is the deserialization constructor. 
        /// </summary>
        /// <param name="info">The Serialization info object that contains all the relevant data.</param>
        /// <param name="context">The serialization context.</param>
        public SiteServerConfiguration(SerializationInfo info, StreamingContext context)
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
            string basePath = "//r:SiteServerConfiguration";
            mappingShortcuts.Add("r", basePath);
        }
        #endregion // XPScAdd(Dictionary<string, string> mappingShortcuts)

        #region Listeners
        /// <summary>
        /// Returns a enumerable collection of the listeners for the server.
        /// </summary>
        public IEnumerable<Uri> Listeners
        {
            get
            {
                //XmlNodeList nl = this.XmlDataDoc.SelectNodes(XPSc("r", "min", "fsmconf:pool"), NSM);
                XmlNodeList nl = this.XmlDataDoc.SelectNodes("//r:SiteServerConfiguration/r:listeners/r:listener[@enabled='true']", NSM);
                foreach (XmlElement elem in nl)
                {
                    yield return new Uri(elem.Attributes["address"].Value);
                }
            }
        }
        #endregion // Listeners

        #region ResolveScheme
        /// <summary>
        /// This method resolves the protocol command id for the specified scheme.
        /// </summary>
        /// <param name="location">The uri containing the scheme</param>
        /// <returns>Returns the GUID of the protocol command, or null if the scheme cannot be resolved.</returns>
        public virtual Guid? ResolveScheme(Uri location)
        {
            return ResolveScheme(location.Scheme);
        }
        /// <summary>
        /// This method resolves the protocol command id for the specified scheme.
        /// </summary>
        /// <param name="scheme">The scheme</param>
        /// <returns>Returns the GUID of the protocol command, or null if the scheme cannot be resolved.</returns>
        public virtual Guid? ResolveScheme(string scheme)
        {
            switch (scheme)
            {
                case "tcp":
                    return new Guid(TransportCommandTCPIP.ID);
                case "udp":
                    return new Guid(TransportCommandUDP.ID);
                default:
                    return null;
            }
        }
        #endregion // ResolveScheme

        #region TransportCommandIDs
        /// <summary>
        /// This enumeration returns a collection of valid transport command ids. These IDs will be used to register permitted
        /// callback commands.
        /// </summary>
        public IEnumerable<Guid> TransportCommandIDs
        {
            get
            {
                yield return new Guid(TransportCommandUDP.ID);
                yield return new Guid(TransportCommandTCPIP.ID);
                yield return new Guid("4F7C8ADF-F85B-4aa6-A983-61BE37F5A1D4");
                yield return new Guid("3425AFC8-C37A-48ff-8D8A-2F6ACF601D74");
                yield return new Guid("FC54B7D4-AAA6-43ef-9233-3606577D15AB");
            }
        }
        #endregion // ProtocolCommandIDs

        #region ConnectionTimeoutInSeconds
        /// <summary>
        /// This is the maximum permitted inactive time in seconds.
        /// </summary>
        public virtual int ConnectionTimeoutInSeconds
        {
            get
            {
                return 30;
            }
        }
        #endregion // ConnectionTimeoutInSeconds

        #region SiteControllerID
        /// <summary>
        /// This is the ID for the site controller.
        /// </summary>
        public virtual Guid SiteControllerID
        {
            get { return sSiteControllerID; }
        }
        #endregion // SiteControllerID

        #region ListenerAdd
        /// <summary>
        /// This method adds a listener to the configuration.
        /// </summary>
        /// <param name="address"></param>
        /// <param name="enabled"></param>
        /// <param name="sessionTimeoutInS"></param>
        protected virtual void ListenerAdd(Uri address, bool enabled, int sessionTimeoutInS, string defaultSecurityDomain)
        {
            XmlDocumentFragment frag = CreateFragment(
                delegate(XmlWriter writer)
                {
                    writer.WriteStartElement("listener", "http://schema.aegea.biz/configuration/siteserver/1.0");
                    //writer.WriteStartElement("listener");
                    writer.WriteAttributeString("enabled", (string)(enabled ? "true" : "false"));
                    writer.WriteAttributeString("sessiontimeoutins", sessionTimeoutInS.ToString());
                    writer.WriteAttributeString("address", address.OriginalString);
                    writer.WriteAttributeString("defaultdomain", defaultSecurityDomain);
                    writer.WriteEndElement();
                });

            XmlNode rootNode = XmlDataDoc.SelectSingleNode("//r:SiteServerConfiguration/r:listeners", NSM);

            rootNode.AppendChild(frag);
        }
        #endregion // ListenerAdd

        #region LoadConfigInitialize
        /// <summary>
        /// This override adds the listener collection from the base configuration.
        /// </summary>
        /// <param name="appDef">The application definition.</param>
        /// <param name="commDef">The command definition</param>
        /// <param name="sh">The config section handler for the command.</param>
        /// <returns>Returns true if the initialization was successful.</returns>
        //protected override bool LoadConfigInitialize(IXimuraApplicationDefinition appDef, IXimuraCommand commDef, IXimuraConfigSH sh)
        //{
        //    bool result = base.LoadConfigInitialize(appDef, commDef, sh);

        //    if (!result)
        //        return false;

        //    //ServerCommandConfigSH ServerCommandSettings = sh as ServerCommandConfigSH;

        //    //if (ServerCommandSettings == null)
        //        //return result;


        //    //List<Uri> listeners = ServerCommandSettings.Listeners;

        //    //if (listeners != null && listeners.Count > 0)
        //    //{
        //    //    //Process each listener
        //    //    foreach (Uri listenOn in listeners)
        //    //    {
        //    //        ListenerAdd(listenOn, true, ConnectionTimeoutInSeconds, "");
        //    //    }
        //    //}

        //    return result;
        //}
        #endregion // LoadConfigInitialize
    }
}
