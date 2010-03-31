#region using
using System;
using System.IO;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Schema;
using System.Net;

using Ximura;
using Ximura.Data;
using CH = Ximura.Common;
using Ximura.Communication;
#endregion // using
namespace Ximura.Communication
{
    [XimuraContentTypeID("D5F7FDC7-90CE-4962-84EA-2C7FEF4D6B0B")]
    [XimuraDataContentDefault(
        "xmrres://XimuraCommEntities/Ximura.Communication.ControllerSession/Ximura.Communication.Resources.ControllerSession_NewData.xml", false)]
    [XimuraDataContentSchema("http://schema.ximura.org/controller/session/1.0",
       "xmrres://XimuraCommEntities/Ximura.Communication.ControllerSession/Ximura.Communication.Resources.ControllerSession.xsd")]
    [XimuraContentCachePolicy(ContentCacheOptions.CannotCache)]
    public class ControllerSession : XimuraCore
    {
        #region Declarations
        private Guid? mNonce;
        private object syncAuth = new object();
        #endregion // Declarations'
        #region Constructor
        /// <summary>
        /// The default constructor
        /// </summary>
        public ControllerSession() 
        { }

        /// <summary>
        /// This is the deserialization constructor. 
        /// </summary>
        /// <param name="info">The Serialization info object that contains all the relevant data.</param>
        /// <param name="context">The serialization context.</param>
        public ControllerSession(SerializationInfo info, StreamingContext context)
            :
            base(info, context) { }
        #endregion
        #region Reset()
        /// <summary>
        /// This method resets any cached values in the session.
        /// </summary>
        public override void Reset()
        {
            mNonce = null;
            base.Reset();
        }
        #endregion // Reset()

        #region XPScAdd(Dictionary<string, string> mappingShortcuts)
        /// <summary>
        /// This method adds the XPath shortcuts in to the collection. You should
        /// override this method to add your own shorcuts.
        /// </summary>
        /// <param name="mappingShortcuts">The mapping shorcut collection.</param>
        protected override void XPScAdd(Dictionary<string, string> mappingShortcuts)
        {
            string basePath = "//r:controllersession";

            mappingShortcuts.Add("ra", basePath + "/r:auth");
            mappingShortcuts.Add("rd", basePath + "/r:data");
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

        #region AuthenticationValidate
        /// <summary>
        /// This method validates the session security for the realm.
        /// </summary>
        /// <param name="userRealm">The realm to check.</param>
        /// <returns>Returns true when the session is validated.</returns>
        public bool AuthenticationValidate(string userRealm)
        {
            lock (syncAuth)
            {
                XmlNode nodeAuth = XmlDataDoc.SelectSingleNode(
                    XPSc("ra", "domain[@id='" + userRealm + "']"), NSM);

                if (nodeAuth == null)
                    return false;

                return nodeAuth.Attributes["authenticated"].Value == "true";
            }
        }
        #endregion // AuthenticationValidate
        #region AuthenticationGet
        /// <summary>
        /// This method returns the authentication settings for the required realm.
        /// </summary>
        /// <param name="userRealm">The realm to check.</param>
        /// <returns>Returns null if the userRealm is not recognised, or a RealmAuthentication object containing the settings.</returns>
        public RealmAuthentication AuthenticationGet(string userRealm)
        {
            lock (syncAuth)
            {
                XmlNode nodeAuth = XmlDataDoc.SelectSingleNode(
                    XPSc("ra", "domain[@id='" + userRealm + "']"), NSM);

                if (nodeAuth == null)
                    return null;

                return new RealmAuthentication(
                           nodeAuth.Attributes["id"].Value,
                           nodeAuth.Attributes["userid"].Value,
                           nodeAuth.Attributes["authenticated"].Value == "true",
                           nodeAuth.Attributes["persist"].Value == "true"
                           );
            }
         }
        #endregion // AuthenticationGet
        #region AuthenticationRemove
        /// <summary>
        /// This method removes the authentication from the session.
        /// </summary>
        /// <param name="userRealm">The realm to remove.</param>
        /// <returns>Returns true if the authentication is removed, or false if the realm is not present.</returns>
        public bool AuthenticationRemove(string userRealm)
        {
            lock (syncAuth)
            {

                XmlNodeList nodeAuths = XmlDataDoc.SelectNodes(
                    XPSc("ra", "domain[@id='" + userRealm + "']"), NSM);

                foreach (XmlNode node in nodeAuths)
                    node.ParentNode.RemoveChild(node);

                return nodeAuths.Count == 0;
            }
        }
        #endregion // AuthenticationRemove
        #region AuthenticationSet

        public bool AuthenticationSet(string userRealm, string userID, bool persist, bool? authenticated)
        {
            lock (syncAuth)
            {
                //XmlNode nodeAuth = XmlDataDoc.SelectSingleNode(
                //    XPSc("ra", "domain[@id='" + userRealm + "' and @userid='" + userID + "']"), NSM);
                XmlNode nodeAuth = null;
                XmlNodeList nodeAuthList = XmlDataDoc.SelectNodes(
                    XPSc("ra", "domain[@id='" + userRealm + "']"), NSM);

                XmlNode parent = XmlDataDoc.SelectSingleNode(XPSc("ra"), NSM);

                foreach (XmlNode auth in nodeAuthList)
                {
                    XmlAttribute attr = auth.Attributes["userid"];
                    if (nodeAuth == null && attr != null && attr.Value == userID)
                        nodeAuth = auth;
                    else
                        parent.RemoveChild(auth);
                }

                if (nodeAuth == null)
                {
                    XmlElementAdd(parent, "domain", null,
                        new XmlAttribute[] {
                        XmlAttributeCreate("id", userRealm), 
                        XmlAttributeCreate("userid", userID), 
                        (authenticated.HasValue)?
                            XmlAttributeCreate("authenticated", authenticated.Value ? "true" : "false"):
                            XmlAttributeCreate("authenticated", persist?"true":"false"),
                        XmlAttributeCreate("persist", persist?"true":"false")}
                            );
                }
                else
                {
                    if (authenticated.HasValue)
                        nodeAuth.Attributes["authenticated"].Value = authenticated.Value ? "true" : "false";
                    nodeAuth.Attributes["persist"].Value = persist ? "true" : "false";
                }

                return authenticated.HasValue ? authenticated.Value : false;
            }
        }

        public bool AuthenticationSet(IAuthUser rqUser, bool persist, bool? authenticated)
        {
            string userID = rqUser.UserName;
            string userRealm = rqUser.RealmDomain;

            bool response =
                AuthenticationSet(userRealm, userID, persist, authenticated);


            return response;
        }
        #endregion // AuthenticationSet
        #region Authentication
        /// <summary>
        /// This iterator returns the realm authentications for the session.
        /// </summary>
        public IEnumerable<RealmAuthentication> Authentication
        {
            get
            {
                    XmlNodeList nodesAuth = XmlDataDoc.SelectNodes(XPSc("ra", "domain"), NSM);
                    if (nodesAuth == null)
                        yield break;

                    foreach (XmlNode nodeAuth in nodesAuth)
                    {
                        yield return new RealmAuthentication(
                            nodeAuth.Attributes["id"].Value,
                            nodeAuth.Attributes["userid"].Value,
                            nodeAuth.Attributes["authenticated"].Value == "true",
                            nodeAuth.Attributes["persist"].Value == "true"
                            );
                    }
            }
        }
        #endregion // Authentication

        #region Nonce
        /// <summary>
        /// This is the session nonce, used for any current authentications.
        /// </summary>
        public string Nonce
        {
            get 
            {
                if (mNonce == null)
                    return null;

                return mNonce.ToString().ToLowerInvariant(); 
            }
        }
        #endregion // Nonce
        #region SetNonce()
        /// <summary>
        /// This method resets the nonce to a new value.
        /// </summary>
        public string SetNonce()
        {
            mNonce = Guid.NewGuid();
            return Nonce;
        }
        #endregion // SetNonce()
    }
}
