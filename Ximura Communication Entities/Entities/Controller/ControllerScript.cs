#region using
using System;
using System.Data;
using System.Text;
using System.Xml;
using System.Text.RegularExpressions;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Net;

using Ximura;
using Ximura.Data;
using Ximura.Data;

using Ximura.Framework;
using Ximura.Framework;
using Ximura.Communication;
using CH = Ximura.Common;
using XH = Ximura.XMLHelper;
#endregion // using
namespace Ximura.Communication
{
    /// <summary>
    /// This is the script which controls how the incoming URI are parsed and processed.
    /// </summary>
    [XimuraContentTypeID("B7D0B27E-DCCF-4ED7-8CD8-413E40379144")]
    [XimuraDataContentSchema("http://schema.ximura.org/controller/script/1.0",
       "xmrres://XimuraCommEntities/Ximura.Communication.ControllerScript/Ximura.Communication.Resources.ControllerScript.xsd")]
    [XimuraContentCachePolicy(ContentCacheOptions.CannotCache)]
    public class ControllerScript : DublinCore, IXimuraPoolManagerDirectAccess
    {
        #region Declarations
        private Dictionary<int, Mapping> mMatchServer = null;
        private Dictionary<int, Mapping> mMatchProtocol = null;
        private Dictionary<int, Mapping> mMatchUserAgent = null;
        private Dictionary<int, Mapping> mMatchDomain = null;
        private Dictionary<int, Mapping> mMatchPath = null;

        private List<string> mRedirects = null;

        private Dictionary<string, Regex> mParameters;
        private bool mCompiled;
        #endregion // Declarations
        #region Constructor
        /// <summary>
        /// The default constructor
        /// </summary>
        public ControllerScript() { }

        /// <summary>
        /// This is the deserialization constructor. 
        /// </summary>
        /// <param name="info">The Serialization info object that contains all the relevant data.</param>
        /// <param name="context">The serialization context.</param>
        public ControllerScript(SerializationInfo info, StreamingContext context) : base(info, context) { }
        #endregion

        #region Reset()
        /// <summary>
        /// This method resets the 
        /// </summary>
        public override void Reset()
        {
            mCompiled = false;

            if (mRedirects != null)
                mRedirects.Clear();
            else
                mRedirects = new List<string>();

            if (mMatchServer != null)
                mMatchServer.Clear();
            else
                mMatchServer = new Dictionary<int, Mapping>();

            if (mMatchProtocol != null)
                mMatchProtocol.Clear();
            else
                mMatchProtocol = new Dictionary<int, Mapping>();

            if (mMatchUserAgent != null)
                mMatchUserAgent.Clear();
            else
                mMatchUserAgent = new Dictionary<int, Mapping>();

            if (mMatchDomain != null)
                mMatchDomain.Clear();
            else
                mMatchDomain = new Dictionary<int, Mapping>();

            if (mMatchPath != null)
                mMatchPath.Clear();
            else
                mMatchPath = new Dictionary<int, Mapping>();

            if (mParameters != null)
                mParameters.Clear();
            else
                mParameters = new Dictionary<string, Regex>();
            
            base.Reset();
        }
        #endregion // Reset()
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
        #region XPScAdd(Dictionary<string, string> mappingShortcuts)
        /// <summary>
        /// This method adds the XPath shortcuts in to the collection. You should
        /// override this method to add your own shorcuts.
        /// </summary>
        /// <param name="mappingShortcuts">The mapping shorcut collection.</param>
        protected override void XPScAdd(Dictionary<string, string> mappingShortcuts)
        {
            string basePath = "//r:controllerscript";
            mappingShortcuts.Add("r", basePath);
            mappingShortcuts.Add("rp", basePath + "/r:parameters");
            mappingShortcuts.Add("rm", basePath + "/r:path");

            mappingShortcuts.Add("ra", basePath + "/r:auth");

            mappingShortcuts.Add("rpp", basePath + "/r:parameters:r/parameter");

        }
        #endregion // XPScAdd(Dictionary<string, string> mappingShortcuts)

        #region Compile()
        /// <summary>
        /// This method prepares the Regex collection ready to start resolving request.
        /// </summary>
        public void Compile()
        {
            Compile(mMatchServer, XPSc("r", "server", "mapping"));
            Compile(mMatchProtocol, XPSc("r","protocol","mapping"));
            Compile(mMatchDomain, XPSc("r", "domain", "mapping"));
            Compile(mMatchUserAgent, XPSc("r", "useragent", "mapping"));
            Compile(mMatchPath, XPSc("r", "paths", "mapping"));

            //Now process the parameters.
            try
            {
                //Ok, process the mappings.
                XmlNodeList nlParam = XmlDataDoc.SelectNodes(XPSc("r", "parameters", "parameter"), NSM);
                foreach (XmlNode node in nlParam)
                {
                    string key = node.Attributes["id"].Value;
                    string strRX = node.FirstChild.InnerText;
                    Regex RX = new Regex(strRX);
                    mParameters.Add(key, RX);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            //Flag that the collection has been successfully compiled.
            mCompiled = true;
        }
        #endregion // Compile()
        #region Compile(Dictionary<int, Mapping> coll, string xPath)
        /// <summary>
        /// This method compiles the mappings for the specific collection.
        /// </summary>
        /// <param name="coll">The collection.</param>
        /// <param name="xPath">The xpath to the list of mapping nodes</param>
        /// <returns>Returns the number of mappings added.</returns>
        protected int Compile(Dictionary<int, Mapping> coll, string xPath)
        {
            int count = 0;

            //Ok, process the mappings.
            XmlNodeList nlMatches = this.XmlDataDoc.SelectNodes(xPath, NSM);
            if (nlMatches != null)
            {
                foreach (XmlNode nodeMatch in nlMatches)
                {
                    Mapping map = new Mapping(nodeMatch, NSM);
                    coll.Add(count, map);
                    count++;

                    if (map.Redirect != null && !mRedirects.Contains(map.Redirect))
                        mRedirects.Add(map.Redirect);
                }
            }

            return count;
        }
        #endregion // Compile(Dictionary<int, Mapping> coll, string xPath)

        #region ResolveAuthType(string realm)
        /// <summary>
        /// This method resolves the auth type from the script.
        /// </summary>
        /// <param name="realm">The realm to resolve.</param>
        /// <returns>Returns the </returns>
        /// <exception cref="System.ArgumentNullException">This exception will be thrown if the realm parameter is null.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">This exception will be thrown if the realm cannot be resolved.</exception>
        public string ResolveAuthType(string realm)
        {
            if (realm == null)
                throw new ArgumentNullException("realm", "realm cannot be null");
                
            XmlElement nodeAuth = (XmlElement)XmlDataDoc.SelectSingleNode(
                XPSc("ra", "realm[@id='" + realm.ToLowerInvariant() + "']"), NSM);

            if (nodeAuth == null)
                throw new ArgumentOutOfRangeException("realm", string.Format("Realm {0} is not recognised.", realm));

            return nodeAuth.InnerText;
        }
        #endregion // ResolveAuthType(string realm)



        #region ReferenceScripts
        /// <summary>
        /// This enumeration contains the list of external scripts referenced by this script.
        /// </summary>
        public virtual IEnumerable<string> ReferenceScripts
        {
            get
            {
                foreach (string refScr in mRedirects)
                    yield return refScr;
            }
        }
        #endregion // ReferenceScripts

        #region ResolveUri
        /// <summary>
        /// This method resolves the Uri against the script.
        /// </summary>
        /// <param name="id">The uri to match.</param>
        /// <param name="userAgent">The current user agent.</param>
        /// <param name="variables">The request variables.</param>
        /// <param name="success">Returns success if the uri and included parameters were matched successfully.</param>
        /// <param name="map">The map or null is not matched.</param>
        /// <returns>Returns true if the uri was identified.</returns>
        public bool ResolveUri(string server, Uri id, string userAgent, string method, 
            IDictionary<string, string> variables, out bool success, MappingSettings map)
        {
            if (!mCompiled)
                throw new InvalidOperationException("The ControllerScript has not been compiled.");

            success = false;

            //Match the server
            if (!ResolveMapping(mMatchServer, server, method, variables, out success, map))
                return false;

            if (success) return true;

            //Match the protocol
            if (!ResolveMapping(mMatchProtocol, id.Scheme, method, variables, out success, map))
                return false;

            if (success) return true;

            //Match the domain
            if (!ResolveMapping(mMatchDomain, id.Authority, method, variables, out success, map))
                return false;

            if (success) return true;

            //Match the user-agent if there is one.
            if (userAgent != null && userAgent != "" && 
                !ResolveMapping(mMatchUserAgent, userAgent, method, variables, out success, map))
                return false;

            if (success) return true;

            //Match the path
            if (!ResolveMapping(mMatchPath, id.LocalPath, method, variables, out success, map))
                return false;

            return true;
        }
        #endregion // Resolve(Uri id, out bool success, out Mapping map, out string redirect, out Match match)

        #region ResolveMapping
        /// <summary>
        /// This method validates the particular piece of data against the script.
        /// </summary>
        /// <param name="matchCollection">The match collection.</param>
        /// <param name="data">The data to match.</param>
        /// <param name="method">The method to match.</param>
        /// <param name="variables">The variable collection.</param>
        /// <param name="success">An out paramterer denoting a successful match.</param>
        /// <param name="map">The map setting for the Uri.</param>
        /// <returns>Returns true if a match was made successfully, or no data was available to match.</returns>
        protected virtual bool ResolveMapping(Dictionary<int, Mapping> matchCollection,
            string data, string method, IDictionary<string, string> variables, out bool success, 
                MappingSettings map)
        {
            success = false;

            if (matchCollection == null || matchCollection.Count == 0)
                return true;

            foreach (int mapID in matchCollection.Keys)
            {
                Mapping mapCheck = matchCollection[mapID];
                Match tempMatch = null;
                Regex tempRegex = null;
                if (!mapCheck.RXMatch(data, out tempMatch, out tempRegex))
                    continue;

                if (mapCheck.MappingVerb != null 
                    && mapCheck.MappingVerb != "*" 
                    && mapCheck.MappingVerb != method
                    && MethodCheckMultiple(mapCheck.MappingVerb, method))
                    continue;

                map.Set(mapCheck);

                //If this is not a redirect we should verify the variables.
                if (mapCheck.Redirect == null)
                    success = VerifyVariables(variables, tempMatch, tempRegex);

                if (mapCheck.OutputColl.Count > 0)
                    success = true;

                return true;
            }

            return false;
        }

        private bool MethodCheckMultiple(string multiple, string method)
        {
            if (!multiple.Contains(","))
                return true;

            string[] methods = multiple.Split(new char[]{','});

            foreach (string methoditem in methods)
            {
                if (methoditem.Trim() == method)
                    return false;
            }

            return true;
        }
        #endregion // ResolveMapping
        #region VerifyVariables
        /// <summary>
        /// This method validates all the variables and ensures that they are of the correct format.
        /// </summary>
        /// <param name="variables">The variable collection.</param>
        /// <param name="match"></param>
        /// <param name="rx"></param>
        /// <returns></returns>
        protected virtual bool VerifyVariables(IDictionary<string, string> variables, 
            Match match, Regex rx)
        {
            string[] groupNames = rx.GetGroupNames();

            if (groupNames.Length > 1)
                for (int loop = 1; loop < groupNames.Length; loop++)
                {
                    string parameter = groupNames[loop];
                    if (!mParameters.ContainsKey(parameter))
                        throw new ArgumentException(@"Parameter """ + parameter + @""" is not recognised.");

                    string capture = match.Groups[parameter].Value;
                    if (!mParameters[parameter].IsMatch(capture))
                        return false;

                    variables.Add(parameter, capture);
                }
            else
                return false;

            return true;
        }
        #endregion // VerifyVariables

        #region AuthSettings
        /// <summary>
        /// This enumerator returns the authentication settings
        /// </summary>
        //public IEnumerator<AuthSetting> AuthSettings
        //{
        //    get
        //    {
        //        XmlNodeList nodeAuths = XmlDataDoc.SelectNodes(XPSc("ra", "auth"), NSM);
        //        foreach (XmlNode nodeAuth in nodeAuths)
        //            yield return new AuthSetting(
        //                nodeAuth.Attributes["realm"].Value,
        //                nodeAuth.Attributes["persist"].Value == "true",
        //                nodeAuth.Attributes["autologon"].Value == "true"
        //                );
        //    }
        //}
        #endregion // AuthSettings
        #region AuthSettingGet(string realm)
        ///// <summary>
        ///// This method returns the authentication settings for the particular realm.
        ///// </summary>
        ///// <param name="realm">The realm to return.</param>
        ///// <returns>Returns the authentication settings.</returns>
        //public AuthSetting AuthSettingGet(string realm)
        //{
        //    XmlNode nodeAuth = XmlDataDoc.SelectSingleNode(
        //        XPSc("ra", "auth[@domain='" + realm + "']"), NSM);

        //    if (nodeAuth == null)
        //        return null;

        //    return new AuthSetting(
        //                nodeAuth.Attributes["realm"].Value,
        //                nodeAuth.Attributes["persist"].Value == "true",
        //                nodeAuth.Attributes["autologon"].Value == "true"
        //                );
        //}
        #endregion // AuthSettingGet(string realm)
        #region AuthSetting Class
        ///// <summary>
        ///// This structure holds the realm authorization settings.
        ///// </summary>
        //public class AuthSetting
        //{
        //    #region Declarations
        //    private string mRealm;
        //    private bool mAutologon;
        //    private bool mPersist;
        //    #endregion // Declarations

        //    #region Constructor
        //    /// <summary>
        //    /// This is the authentication settings class, that hold the settings from the ControllerScript.
        //    /// </summary>
        //    /// <param name="Realm"></param>
        //    /// <param name="Persist"></param>
        //    /// <param name="Autologon"></param>
        //    public AuthSetting(string Realm, bool Persist, bool Autologon)
        //    {
        //        mRealm=Realm;
        //        mPersist=Persist;
        //        mAutologon=Autologon;
        //    }
        //    #endregion // Constructor

        //    #region Realm
        //    /// <summary>
        //    /// The realm.
        //    /// </summary>
        //    public string Realm
        //    {
        //        get { return mRealm; }
        //    }
        //    #endregion // Realm
        //    #region Persist
        //    /// <summary>
        //    /// The persist settings
        //    /// </summary>
        //    public bool Persist
        //    {
        //        get { return mPersist; }
        //    }
        //    #endregion // Persist
        //    #region Autologon
        //    /// <summary>
        //    /// The sutologon property
        //    /// </summary>
        //    public bool Autologon
        //    {
        //        get { return mAutologon; }
        //    }
        //    #endregion // Autologon
        //}
        #endregion // AuthSetting

        #region ScriptName
        /// <summary>
        /// This is the Dublin Core Publisher
        /// </summary>
        [CDSReference("name")]
        public virtual string ScriptName
        {
            get { return XmlMappingGetToString(XPSc("r", "scriptname")); }
            set { XmlMappingSet(XPSc("r", "scriptname"), value); }
        }
        #endregion // DCPublisher

    }
}
