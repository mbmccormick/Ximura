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
using Ximura.Framework;
using Ximura.Framework;
using Ximura.Communication;
#endregion // using
namespace Ximura.Communication
{
    /// <summary>
    /// The controller request class handles all real-time SiteManager requests.
    /// </summary>
    [XimuraContentTypeID("C1B9C360-55E0-4394-B9D5-C6E25F8009C1")]
    [XimuraDataContentDefault(
        "xmrres://XimuraCommEntities/Ximura.Communication.ControllerScript/Ximura.Communication.Resources.ControllerRequest_DefaultData.xml",true)]
    [XimuraDataContentSchema("http://schema.ximura.org/controller/request/1.0",
       "xmrres://XimuraCommEntities/Ximura.Communication.ControllerScript/Ximura.Communication.Resources.ControllerRequest.xsd")]
    public class ControllerRequest : DataContent
    {
        #region Declarations
        private Uri mRequestURI;
        private Uri mRequestRefererURI;
        private IPEndPoint mRequestAddressLocal;
        private IPEndPoint mRequestAddressRemote;
        private bool mRequestProtocolSecure;
        private string mRequestProtocolVersion;
        #endregion // Declarations
        #region Constructor
        /// <summary>
        /// The default constructor
        /// </summary>
        public ControllerRequest() { }

        /// <summary>
        /// This is the deserialization constructor. 
        /// </summary>
        /// <param name="info">The Serialization info object that contains all the relevant data.</param>
        /// <param name="context">The serialization context.</param>
        public ControllerRequest(SerializationInfo info, StreamingContext context)
            :
            base(info, context) { }
        #endregion
        #region Delegate Declarations

        delegate string Combiner(XmlNode node, string joinChar);

        Combiner standardCombiner = delegate(XmlNode node, string splitChar)
        {
            XmlAttribute attr = node.Attributes["type"];

            if (attr == null)
                return null;

            if (node.InnerText == null || node.InnerText == string.Empty)
                return attr.InnerText.Trim();
            else
                return attr.InnerText.Trim() + splitChar + node.InnerText.Trim();
        };

        Combiner qCombiner = delegate(XmlNode node, string splitChar)
        {
            XmlAttribute attr = node.Attributes["type"];

            if (attr == null)
                return null;

            string inner = node.InnerText;

            if (inner == null)
                return attr.InnerText.Trim();

            inner = inner.Trim();

            if (inner == string.Empty)
                return attr.InnerText.Trim();
            else
                return attr.InnerText.Trim() + splitChar + "q=" + inner;
        };
        #endregion // Delegate Declarations

        #region Reset()
        /// <summary>
        /// This method resets the content object to its default state.
        /// </summary>
        public override void Reset()
        {
            mRequestURI = null;
            mRequestRefererURI = null;
            mRequestAddressLocal = null;
            mRequestAddressRemote = null;
            mRequestProtocolSecure = false;
            mRequestProtocolVersion = null;
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
            string basePath = "//r:controllerrequest/r:request";
            mappingShortcuts.Add("r", basePath);
            mappingShortcuts.Add("ra", "//r:controllerrequest/r:auth");
            mappingShortcuts.Add("rc", "//r:controllerrequest/r:cookies");
            mappingShortcuts.Add("rp", basePath + "/r:protocol");
            mappingShortcuts.Add("rs", basePath + "/r:secure");
            mappingShortcuts.Add("rh", basePath + "/r:host");
            mappingShortcuts.Add("rhr", basePath + "/r:host/r:remote");
            mappingShortcuts.Add("rhl", basePath + "/r:host/r:local");
            mappingShortcuts.Add("rb", basePath + "/r:browser");
            mappingShortcuts.Add("rv", basePath + "/r:variables");

            mappingShortcuts.Add("rr", "//r:controllerrequest/r:response");
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

        #region RequestHeaderAdd(string field, string data)
        /// <summary>
        /// This method is used to add a generic header to the collection.
        /// </summary>
        /// <param name="field"></param>
        /// <param name="data"></param>
        public void RequestHeaderAdd(string field, string data)
        {
            try
            {
                XmlNode parent = this.XmlDataDoc.SelectSingleNode(XPSc("rp","headers"), NSM);
                
                XmlElementAdd(parent,"value",data,XmlAttributeCreate("type",field.ToLowerInvariant()));
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
        #endregion

        #region VariableAdd(string parameter, string capture)
        /// <summary>
        /// This method is used to add a variable to collection.
        /// </summary>
        /// <param name="parameter">The variable parameter.</param>
        /// <param name="capture">The variable value.</param>
        public void VariableAdd(string parameter, string capture)
        {
            try
            {
                XmlNode parent = this.XmlDataDoc.SelectSingleNode(XPSc("rv"), NSM);

                XmlElementAdd(parent, "value", capture, XmlAttributeCreate("type", parameter));

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

        }
        #endregion // VariableAdd(string parameter, string capture)
        #region VariableGet(string parameter, string capture)
        /// <summary>
        /// This method is used to retrieve a variable from the collection.
        /// </summary>
        /// <param name="parameter">The variable parameter.</param>
        public string VariableGet(string parameter)
        {
            try
            {
                XmlNode parent = this.XmlDataDoc.SelectSingleNode(XPSc("rv", "value") + "[@type='" + parameter + "']", NSM);
                if (parent == null)
                    return null;
                return parent.InnerText;

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw ex;
            }
        }
        #endregion // VariableAdd(string parameter, string capture)

        #region RequestQuery
        /// <summary>
        /// This method returns the query string parameter for the request.
        /// </summary>
        public string RequestQuery
        {
            get 
            {
                return "?" + CombineParamPairs(XPSc("rp", "query", "value"), "&", "=", standardCombiner);
            }
            protected set 
            {
                if (value == null || value=="" || value=="?")
                    return;

                if (!value.StartsWith("?"))
                    throw new ArgumentException("Query string must start with '?' characters.");

                AddParamPairs(XPSc("rp", "query"), 
                    CH.SplitOnChars<string, string>(value.Substring(1),
                    CH.ConvPassthru, CH.ConvPassthru, new char[] { '&' }, new char[] { '=' }));
            }
        }
        #endregion // RequestQuery

        #region RequestAccept
        /// <summary>
        /// This property holds and parses the accept header
        /// </summary>
        public string RequestAccept
        {
            get
            {
                return CombineQPairs(XPSc("rp", "accept", "value"));
            }
            set
            {
                AddParamPairs(XPSc("rp", "accept"), SplitOnCharsForQString(value));
            }
        }
        #endregion // RequestAccept
        #region RequestAcceptLanguage
        /// <summary>
        /// This property holds and parses the accept-language header
        /// </summary>
        public string RequestAcceptLanguage
        {
            get 
            {
                return CombineQPairs(XPSc("rp", "language", "value"));
            }
            set 
            {
                AddParamPairs(XPSc("rp", "language"), SplitOnCharsForQString(value));
            }
        }
        #endregion // RequestLanguage
        #region RequestAcceptEncoding
        /// <summary>
        /// This property holds and parses the accept-encoding header
        /// </summary>
        public string RequestAcceptEncoding
        {
            get
            {
                return CombineQPairs(XPSc("rp", "encoding", "value"));
            }
            set
            {
                AddParamPairs(XPSc("rp", "encoding"), SplitOnCharsForQString(value));
            }
        }
        #endregion // RequestAcceptEncoding
        #region RequestAcceptCharset
        /// <summary>
        /// This property holds and parses the accept-charset header
        /// </summary>
        public string RequestAcceptCharset
        {
            get
            {
                return CombineQPairs(XPSc("rp", "charset", "value"));
            }
            set
            {
                AddParamPairs(XPSc("rp", "charset"),SplitOnCharsForQString(value));
            }
        }
        #endregion // RequestAcceptCharset

        #region Parameter Pair code
        private string CombineQPairs(string xPath)
        {
            return CombineParamPairs(xPath, ", ", ";", qCombiner);
        }

        private void AddParamPairs(string xPath,List<KeyValuePair<string, string>> values)
        {
            AddParamPairs(xPath, "value", "type", values, false);
        }

        private void AddParamPairs(string xPath, string elementName, string attrName,
            List<KeyValuePair<string, string>> values, bool append)
        {
            XmlNode parent = XmlDataDoc.SelectSingleNode(xPath, NSM);

            if (!append && parent.HasChildNodes)
                parent.RemoveAll();

            foreach (KeyValuePair<string, string> key in values)
            {
                XmlElementAdd(parent, elementName, key.Value, XmlAttributeCreate(attrName, key.Key));
            }
        }

        private string CombineParamPairs(string xPath,
            string itemBreak, string charJoin, Combiner appender)
        {
            XmlNodeList children = XmlDataDoc.SelectNodes(xPath, NSM);

            if (children.Count==0)
                return "";

            StringBuilder sb = new StringBuilder();

            foreach(XmlNode child in children)
            {
                sb.Append(appender(child,charJoin));
                sb.Append(itemBreak);
            }

            string output = sb.ToString();
            return output.Substring(0, output.Length - itemBreak.Length);
        }

        private List<KeyValuePair<string, string>> SplitOnCharsForQString(string toSplit)
        {
            return CH.SplitOnChars<string, string>(toSplit, CH.ConvPassthru,
                CH.ConvQParam, new char[] { ',' }, new char[] { ';' });
        }
        #endregion // Parameter Pair code

        #region RequestPreferredCompression
        /// <summary>
        /// This property returns the preferred compression method, or null if there is no
        /// compression method supported.
        /// </summary>
        public string RequestPreferredCompression
        {
            get
            {
                string path = XPSc("rp", "encoding", "value");

                XmlNode comp = XmlDataDoc.SelectSingleNode(path, NSM); ;// XmlMappingGetToString;
                if (comp == null)
                    return null;

                return comp.Attributes["type"].Value ;
            }
        }
        #endregion // RequestPreferredCompression

        #region RequestVerb
        /// <summary>
        /// The request verb.
        /// </summary>
        public string RequestVerb
        {
            get { return XmlMappingGetToString(XPSc("rp", "verb")); }
            set { XmlMappingSet(XPSc("rp", "verb"), value); }
        }
        #endregion // RequestVerb
        #region RequestProtocolVersion
        /// <summary>
        /// The protocol version number.
        /// </summary>
        public string RequestProtocolVersion
        {
            get { return XmlMappingGetToString(XPScA("rp", "version")); }
            set { XmlMappingSet(XPScA("rp", "version"), value); }
        }
        #endregion // RequestProtocolVersion
        #region RequestProtocol
        /// <summary>
        /// The request protocol
        /// </summary>
        public string RequestProtocol
        {
            get { return XmlMappingGetToString(XPScA("rp", "type")); }
            protected set { XmlMappingSet(XPScA("rp", "type"), value); }
        }
        #endregion // RequestProtocol
        #region RequestProtocolSecure
        /// <summary>
        /// This boolean value identifies whether the request is over a secure connection.
        /// </summary>
        public bool RequestProtocolSecure
        {
            get { return XmlMappingGetToBool(XPSc("rs", "client")); }
            set { XmlMappingSet(XPSc("rs", "client"), value); }
        }
        #endregion // RequestProtocolSecure
        #region RequestHost
        /// <summary>
        /// This property returns the host for the request.
        /// </summary>
        public string RequestHost
        {
            get { return XmlMappingGetToString(XPSc("rp", "host")); }
            protected set { XmlMappingSet(XPSc("rp", "host"), value); }
        }
        #endregion // RequestHost
        #region RequestPort
        /// <summary>
        /// THis method returns the port for the request or null if this value is not set.
        /// </summary>
        public int? RequestPort
        {
            get { return XmlMappingGetToInt32Nullable(XPSc("rp", "port")); }
            protected set { XmlMappingSet(XPSc("rp", "port"), value); }
        }
        #endregion // RequestPort
        #region RequestPath
        /// <summary>
        /// This method returns the path requested.
        /// </summary>
        public string RequestPath
        {
            get { return XmlMappingGetToString(XPSc("rp", "path")); }
            protected set { XmlMappingSet(XPSc("rp", "path"), value); }
        }
        #endregion // RequestPath

        #region RequestContentType
        /// <summary>
        /// This method returns the path requested.
        /// </summary>
        public string RequestContentType
        {
            get { return XmlMappingGetToString(XPScA("rp", "type", "body")); }
            set
            {
                RequestBodyNodeCheck();
                XmlMappingSet(XPScA("rp", "type", "body"), value);
            }
        }
        #endregion // RequestPath
        #region RequestContentLength
        /// <summary>
        /// This method returns the path requested.
        /// </summary>
        public string RequestContentLength
        {
            get { return XmlMappingGetToString(XPScA("rp", "length", "body")); }
            set
            {
                RequestBodyNodeCheck();
                XmlMappingSet(XPScA("rp", "length", "body"), value);
            }
        }
        #endregion // RequestPath
        #region RequestContentEncoding
        /// <summary>
        /// This method returns the path requested.
        /// </summary>
        public string RequestContentEncoding
        {
            get { return XmlMappingGetToString(XPScA("rp", "encoding", "body")); }
            set 
            {
                RequestBodyNodeCheck();
                XmlMappingSet(XPScA("rp", "encoding", "body"), value); 
            }
        }
        #endregion // RequestPath
        #region RequestBodyNodeCheck()
        /// <summary>
        /// This method checks whether the request body node exists, and if not creates it.
        /// </summary>
        private void RequestBodyNodeCheck()
        {
            XmlNode body = XmlDataDoc.SelectSingleNode(XPSc("rp", "body"),NSM);

            if (body != null)
                return;

            XmlNode parent = XmlDataDoc.SelectSingleNode(XPSc("rp"),NSM);
            XmlElementAdd(parent, "body", null,
                    new XmlAttribute[] {
                        XmlAttributeCreate("length", ""), 
                        XmlAttributeCreate("type", ""), 
                        XmlAttributeCreate("encoding", "")
                        }
            );
        }
        #endregion // RequestBodyNodeCheck()

        #region RequestConnection
        /// <summary>
        /// This method returns the path requested.
        /// </summary>
        public string RequestConnection
        {
            get { return XmlMappingGetToString(XPSc("rp", "connection")); }
            set { XmlMappingSet(XPSc("rp", "connection"), value); }
        }
        #endregion // RequestPath

        #region RequestIfNoneMatch
        /// <summary>
        /// This method returns the path requested.
        /// </summary>
        public string RequestIfNoneMatch
        {
            get { return XmlMappingGetToString(XPSc("rp", "if-none-match")); }
            set { XmlMappingSet(XPSc("rp", "if-none-match"), value); }
        }
        #endregion // RequestPath

        #region RequestAddressRemote
        /// <summary>
        /// This is the remote endpoint for the request.
        /// </summary>
        public IPEndPoint RequestAddressRemote
        {
            get { return XmlMappingGetToIPEndPoint(XPSc("rhr", "ipaddress")) ; }
            set { XmlMappingSet(XPSc("rhr", "ipaddress"), value); }
        }
        #endregion // RequestAddressRemote
        #region RequestAddressRemoteResolved
        /// <summary>
        /// This is the remote endpoint for the request.
        /// </summary>
        public string RequestAddressRemoteResolved
        {
            get { return XmlMappingGetToString(XPSc("rhr", "ipaddressresolve")) ; }
            set { XmlMappingSet(XPSc("rhr", "ipaddressresolve"), value); }
        }
        #endregion // RequestAddressRemoteResolved
        #region RequestAddressRemoteResolvedCountryCode
        /// <summary>
        /// This is the remote endpoint for the request.
        /// </summary>
        public string RequestAddressRemoteResolvedCountryCode
        {
            get { return XmlMappingGetToString(XPScA("rhr", "isoid", "ipaddressresolve")); }
            set { XmlMappingSet(XPScA("rhr", "isoid", "ipaddressresolve"), value); }
        }
        #endregion // RequestAddressRemoteResolvedCountryCode

        #region RequestAddressLocal
        /// <summary>
        /// This is the local endpoint for the connection.
        /// </summary>
        public IPEndPoint RequestAddressLocal
        {
            get { return XmlMappingGetToIPEndPoint(XPSc("rhl", "ipaddress")); ; }
            set { XmlMappingSet(XPSc("rhl", "ipaddress"), value); }
        }
        #endregion // RequestAddressLocal
        #region RequestRefererURI
        /// <summary>
        /// This is the referer Uri
        /// </summary>
        public string RequestRefererURI
        {
            get { return XmlMappingGetToString(XPSc("rp", "referer")); ; }
            set { XmlMappingSet(XPSc("rp", "referer"), value); }
        }
        #endregion // RequestRefererURI

        #region RequestAuthorization
        /// <summary>
        /// This is the referer Uri
        /// </summary>
        public string RequestAuthorization
        {
            get { return XmlMappingGetToString(XPSc("rp", "authorization")); ; }
            set { XmlMappingSet(XPSc("rp", "authorization"), value); }
        }
        #endregion // RequestRefererURI     

        #region RequestURI
        private void BuildUri()
        {
            if (RequestProtocol == null || RequestProtocol == "")
                return;
            if (RequestHost == null || RequestHost == "")
                return;
            if (!RequestPort.HasValue)
                return;
            if (RequestPath == null || RequestPath == "")
                return; 

            mRequestURI = new Uri(
                RequestProtocol + @"://" + 
                RequestHost + ":" + 
                RequestPort.ToString() + 
                RequestPath);
        }
        /// <summary>
        /// This is the request Uri.
        /// </summary>
        public Uri RequestURI
        {
            get 
            {
                if (mRequestURI == null)
                    BuildUri();

                return mRequestURI; 
            }
            set 
            { 
                mRequestURI = value;
                RequestPort = value.Port;
                RequestHost = value.Host;
                RequestPath = value.AbsolutePath;
                RequestQuery = value.Query;
                RequestProtocol = value.Scheme;

            }
        }
        #endregion // RequestURI

        #region RequestCookie
        /// <summary>
        /// The request verb.
        /// </summary>
        public string RequestCookie
        {
            get { return ""; }
        }
        #endregion // RequestVerb

        #region RequestUserAgentID
        /// <summary>
        /// The request user agent unique id.
        /// </summary>
        public Guid? RequestUserAgentID
        {
            get { return XmlMappingGetToGuidNullable(XPSc("rb", "guid")); }
            set { XmlMappingSet(XPSc("rb", "guid"), value); }
        }
        #endregion // RequestVerb
        #region RequestUserAgent
        /// <summary>
        /// The request user agent.
        /// </summary>
        public string RequestUserAgent
        {
            get { return XmlMappingGetToString(XPSc("rb", "id")); }
            set { XmlMappingSet(XPSc("rb", "id"), value); }
        }
        #endregion // RequestVerb
        #region RequestUserAgentType
        /// <summary>
        /// The request verb.
        /// </summary>
        public string RequestUserAgentType
        {
            get { return XmlMappingGetToString(XPSc("rb", "browsertype")); }
            set { XmlMappingSet(XPSc("rb", "browsertype"), value); }
        }
        #endregion // RequestVerb

        #region RequestUserAgentWapProfile
        /// <summary>
        /// This method returns wap profile value
        /// </summary>
        public string RequestUserAgentWapProfile
        {
            get { return XmlMappingGetToString(XPSc("rp", "x-wap-profile")); }
            set { XmlMappingSet(XPSc("rp", "x-wap-profile"), value); }
        }
        #endregion


        #region RequestHeaderCookies
        /// <summary>
        /// This property returns the listed cookies as objects that can be accessed.
        /// </summary>
        public IEnumerable<string> RequestHeaderCookies
        {
            get
            {
                XmlNodeList nodeCookies =
                    this.XmlDataDoc.SelectNodes("//r:controllerrequest/r:request/r:protocol/r:headers/r:value[@type='cookie']", NSM);

                foreach (XmlNode node in nodeCookies)
                {
                    yield return node.InnerText;
                }

                //return null;
            }
        }
        #endregion // RequestHeaderCookies

        #region RequestQueryParameterExists(string parameter)
        /// <summary>
        /// This method returns the true if the query parameter is in the request.
        /// </summary>
        /// <param name="parameter">The paramter required.</param>
        /// <returns>Returns true if the parameter exists.</returns>
        public bool RequestQueryParameterExists(string parameter)
        {
            XmlNode node = XmlDataDoc.SelectSingleNode(XPSc("rp", "query", "value") + "[@type='" + parameter + "']", NSM);

            return node != null;
        }
        #endregion // RequestQueryParameter(string parameter)

        #region RequestQueryParameterGet(string parameter)
        /// <summary>
        /// This method returns the query parameter specified.
        /// </summary>
        /// <param name="parameter">The paramter required.</param>
        /// <returns>Returns the query parameter value or null if the parameter cannot be found.</returns>
        public string RequestQueryParameterGet(string parameter)
        {
            XmlNode node = XmlDataDoc.SelectSingleNode(XPSc("rp", "query", "value") + "[@type='" +parameter +"']", NSM);

            return node == null?null:node.InnerText; 
        }
        #endregion // RequestQueryParameter(string parameter)

        #region RequestQueryParameterSet(string parameter)
        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="value"></param>
        public void RequestQueryParameterSet(string parameter, string value)
        {
            XmlNode node = XmlDataDoc.SelectSingleNode(XPSc("rp", "query", "value") + "[@type='" + parameter + "']", NSM);

            if (node != null)
            {
                node.InnerText = value;
                return;
            }
            //Ok, we need to create the node.
            node = XmlDataDoc.SelectSingleNode(XPSc("rp", "query"), NSM);
            XmlElementAdd(node, "value", value, XmlAttributeCreate("type", parameter));
        }
        #endregion // RequestQueryParameter(string parameter)

        #region RequestHeaderGet
        /// <summary>
        /// This enumerator requests the request headers
        /// </summary>
        public string RequestHeaderGet(string field)
        {
            return XmlMappingGetToString(
                string.Format(
                "//r:controllerrequest/r:request/r:protocol/r:headers/r:value[@type='{0}']", 
                field.ToLowerInvariant()));
        }
        #endregion // RequestHeaders
        #region RequestHeaders
        /// <summary>
        /// This enumerator requests the request headers
        /// </summary>
        public IEnumerable<KeyValuePair<string, string>> RequestHeaders
        {
            get
            {
                XmlNodeList nodeCookies =
                    this.XmlDataDoc.SelectNodes("//r:controllerrequest/r:request/r:protocol/r:headers/r:value", NSM);

                foreach (XmlNode node in nodeCookies)
                {
                    yield return new KeyValuePair<string,string>(node.Attributes["type"].Value,node.InnerText);
                }
            }
        }
        #endregion // RequestHeaders

        #region ResponseHeaders
        /// <summary>
        /// This enumerator returns the response headers.
        /// </summary>
        public IEnumerable<KeyValuePair<string, string>> ResponseHeaders
        {
            get
            {
                XmlNodeList nodeCookies =
                    this.XmlDataDoc.SelectNodes("//r:controllerrequest/r:response/r:protocol/r:headers/r:value", NSM);

                foreach (XmlNode node in nodeCookies)
                {
                    yield return new KeyValuePair<string, string>(node.Attributes["type"].Value, node.InnerText);
                }
            }
        }
        #endregion // ResponseHeaders

        #region RequestUserID
        /// <summary>
        /// This is the userID for the primary realm.
        /// </summary>
        public Guid? RequestUserID
        {
            get { return XmlMappingGetToGuidNullable(XPSc("r", "userid")); }
            set { XmlMappingSet(XPSc("r", "userid"), value); }
        }
        #endregion // RequestUserID

        #region SessionID
        /// <summary>
        /// This is the session ID stored in the cookie. This maps to the CID of the object.
        /// </summary>
        public Guid? SessionID
        {
            get { return XmlMappingGetToGuidNullable(XPSc("r", "sessionid")); }
            set { XmlMappingSet(XPSc("r", "sessionid"), value); }
        }
        #endregion // SessionID
        #region ResponseID
        /// <summary>
        /// This method returns the path requested.
        /// </summary>
        public string ResponseID
        {
            get { return XmlMappingGetToString(XPSc("rr", "id")); }
            set { XmlMappingSet(XPSc("rr", "id"), value); }
        }
        #endregion // ResponseID
        #region ResponseStylesheet
        /// <summary>
        /// This method returns the path requested.
        /// </summary>
        public string ResponseStylesheet
        {
            get { return XmlMappingGetToString(XPSc("rr", "output")); }
            set { XmlMappingSet(XPSc("rr", "output"), value); }
        }
        #endregion // ResponseStylesheet
        #region ResponseTemplate
        /// <summary>
        /// This method returns the path requested.
        /// </summary>
        public string ResponseTemplate
        {
            get { return XmlMappingGetToString(XPSc("rr", "template")); }
            set { XmlMappingSet(XPSc("rr", "template"), value); }
        }
        #endregion // ResponseTemplate
        #region ResponseOutputMIMEType
        /// <summary>
        /// This method returns the path requested.
        /// </summary>
        public string ResponseOutputMIMEType
        {
            get { return XmlMappingGetToString(XPSc("rr", "outputmimetype")); }
            set { XmlMappingSet(XPSc("rr", "outputmimetype"), value); }
        }
        #endregion // ResponseOutputMIMEType
        #region ResponseOutput
        /// <summary>
        /// This method returns the path requested.
        /// </summary>
        public string ResponseOutput
        {
            get { return XmlMappingGetToString(XPSc("rr", "output")); }
            set { XmlMappingSet(XPSc("rr", "output"), value); }
        }
        #endregion // ResponseOutput
        #region ResponseOutputType
        /// <summary>
        /// This method returns the path requested.
        /// </summary>
        public string ResponseOutputType
        {
            get { return XmlMappingGetToString(XPSc("rr", "outputtype")); }
            set { XmlMappingSet(XPSc("rr", "outputtype"), value); }
        }
        #endregion // ResponseOutput

        #region ResponseStateOutput
        /// <summary>
        /// This property contains the initial state used to process the output.
        /// </summary>
        public string ResponseStateOutput
        {
            get { return XmlMappingGetToString(XPSc("rr", "stateoutput")); }
            set { XmlMappingSet(XPSc("rr", "stateoutput"), value); }
        }
        #endregion

        #region ResponseStateAuth
        /// <summary>
        /// This property contains the initial state used to process the output.
        /// </summary>
        public string ResponseStateAuth
        {
            get { return XmlMappingGetToString(XPSc("rr", "stateauth")); }
            set { XmlMappingSet(XPSc("rr", "stateauth"), value); }
        }
        #endregion
        #region ResponseStateAuthDomain
        /// <summary>
        /// This property contains the initial state used to process the authorization for the resource.
        /// </summary>
        public string ResponseStateAuthDomain
        {
            get { return XmlMappingGetToString(XPSc("rr", "stateauthdomain")); }
            set { XmlMappingSet(XPSc("rr", "stateauthdomain"), value); }
        }
        #endregion

        #region ResponseStatus
        /// <summary>
        /// The response code.
        /// </summary>
        public string ResponseStatus
        {
            get { return XmlMappingGetToString(XPSc("rr", "protocol", "verb")); }
            set { XmlMappingSet(XPSc("rr", "protocol", "verb"), value); }
        }
        #endregion
        #region ResponseStatusMessage
        /// <summary>
        /// The response message.
        /// </summary>
        public string ResponseStatusMessage
        {
            get { return XmlMappingGetToString(XPSc("rr", "protocol", "path")); }
            set { XmlMappingSet(XPSc("rr", "protocol", "path"), value); }
        }
        #endregion

        #region ResponseHeaderAdd(string field, string data)
        /// <summary>
        /// This method is used to add a generic header to the collection.
        /// </summary>
        /// <param name="field"></param>
        /// <param name="data"></param>
        public void ResponseHeaderAdd(string field, string data)
        {
            try
            {
                XmlNode parent = this.XmlDataDoc.SelectSingleNode(XPSc("rr", "protocol", "headers"), NSM);

                XmlElementAdd(parent, "value", data, XmlAttributeCreate("type", field));
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

        }
        #endregion

        #region ResponseProtocolVersion
        /// <summary>
        /// The protocol version number.
        /// </summary>
        public string ResponseProtocolVersion
        {
            get { return XmlMappingGetToString(XPScA("rr", "protocol", "version")); }
            set { XmlMappingSet(XPScA("rr", "protocol", "version"), value); }
        }
        #endregion // RequestProtocolVersion
        #region ResponseProtocol
        /// <summary>
        /// The request protocol
        /// </summary>
        public string ResponseProtocol
        {
            get { return XmlMappingGetToString(XPScA("rr", "protocol", "type")); }
            set { XmlMappingSet(XPScA("rr", "protocol", "type"), value); }
        }
        #endregion // RequestProtocol

        #region OutputRawXML
        /// <summary>
        /// This property checks whether the raw tag is supplied in the request.
        /// </summary>
        public bool OutputRawXML
        {
            get 
            {
                XmlNode node = XmlDataDoc.SelectSingleNode(XPSc("rp", "query","value") + "[@type='raw']", NSM);

                return node != null; 
            }
        }
        #endregion // RequestPath

        #region AuthenticationGet(string realm)
        /// <summary>
        /// This method gets the specific realm authentication
        /// </summary>
        /// <param name="realm">The realm required.</param>
        /// <returns>Returns the realm authentication or null if the realm cannot be found.</returns>
        public RealmAuthentication AuthenticationGet(string realm)
        {
            XmlNode nodeAuth = XmlDataDoc.SelectSingleNode(
                XPSc("ra", "domain[@domainid='" + realm + "']"), NSM);

            if (nodeAuth == null)
                return null;

            return new RealmAuthentication(
                        nodeAuth.Attributes["domainid"].Value,
                        nodeAuth.Attributes["userid"].Value,
                        nodeAuth.Attributes["authenticated"].Value == "true",
                        nodeAuth.Attributes["cookiepersist"].Value == "true"
                        );
        }
        #endregion // AuthenticationGet(string realm)
        #region AuthenticationSet(RealmAuthentication auth)
        /// <summary>
        /// This method creates or updates the member authentication.
        /// </summary>
        /// <param name="auth">The realm authentication.</param>
        /// <returns>Returns true.</returns>
        public bool AuthenticationSet(RealmAuthentication auth)
        {
            XmlNode nodeAuth = XmlDataDoc.SelectSingleNode(
                XPSc("ra", "domain[@domainid='" + auth.Realm + "' and userid='" + auth.Username + "']"), NSM);

            if (nodeAuth == null)
            {
                XmlNode parent = XmlDataDoc.SelectSingleNode(XPSc("ra"), NSM);
                XmlElementAdd(parent, "domain", null,
                    new XmlAttribute[] {
                        XmlAttributeCreate("domainid", auth.Realm), 
                        XmlAttributeCreate("userid", auth.Username), 
                        XmlAttributeCreate("authenticated", (string)(auth.Authenticated?"true":"false")),
                        XmlAttributeCreate("cookiepersist", (string)(auth.CookiePersist?"true":"false"))}
                        );

            }
            else
            {
                nodeAuth["authenticated"].Value = auth.Authenticated ? "true" : "false";
                nodeAuth["cookiepersist"].Value = auth.CookiePersist ? "true" : "false";
            }

            return true;
        }
        #endregion // AuthenticationSet(RealmAuthentication auth)

        #region CookieAdd(string cookieName, string cookieValue)
        /// <summary>
        /// This method adds the cookie to the response.
        /// </summary>
        /// <param name="cookieName">The cookie name.</param>
        /// <param name="cookieValue">The cookie value.</param>
        public void CookieAdd(string cookieName, string cookieValue)
        {
            XmlNode nodeCookie = XmlDataDoc.SelectSingleNode(
                XPSc("rc", "cookie[@id='" + cookieName + "']"), NSM);

            if (nodeCookie == null)
            {
                XmlNode parent = XmlDataDoc.SelectSingleNode(XPSc("rc"), NSM);
                XmlElement elem = XmlElementAdd(parent, "cookie", null,
                    new XmlAttribute[] {
                        XmlAttributeCreate("id", cookieName)}
                        );
                elem.InnerText = cookieValue;
            }
            else
            {
                nodeCookie.InnerText = nodeCookie.InnerText + cookieValue;
            }
        }
        #endregion // CookieAdd(string cookieName, string cookieValue)
        #region CookieGet(string cookieName)
        /// <summary>
        /// This method returns the specific cookie, or null if the cookie cannot be found.
        /// </summary>
        /// <param name="cookieName"></param>
        /// <returns></returns>
        public IEnumerable<Cookie> CookieGet(string cookieName)
        {
            XmlNodeList nodeCookies = XmlDataDoc.SelectNodes(
                XPSc("rc", "cookie[@id='" + cookieName + "']"), NSM);

            if (nodeCookies.Count == 0)
                yield break;

            foreach (XmlNode nodeCookie in nodeCookies)
                yield return new Cookie(nodeCookie, NSM);
        }
        #endregion // CookieGet(string cookieName)
        #region Cookie
        /// <summary>
        /// This class encapsulates the cookie.
        /// </summary>
        public class Cookie
        {
            #region Declarations
            XmlNode mNodeCookie;
            XmlNamespaceManager mNSM;
            #endregion // Declarations
            #region Constructor
            /// <summary>
            /// The cookie constructor
            /// </summary>
            /// <param name="nodeCookie"></param>
            /// <param name="NSM"></param>
            public Cookie(XmlNode nodeCookie, XmlNamespaceManager NSM)
            {
                mNodeCookie = nodeCookie;
                mNSM = NSM;
            }
            #endregion // Constructor

            #region Value
            /// <summary>
            /// The cookie value.
            /// </summary>
            public string Value
            {
                get
                {
                    return mNodeCookie.InnerText;
                }
                set
                {
                    mNodeCookie.InnerText = value;
                }
            }
            #endregion // Value
        }
        #endregion // Cookie
    }
}
