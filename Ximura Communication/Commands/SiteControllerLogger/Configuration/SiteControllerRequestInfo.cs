#region using
using System;
using System.Runtime.Serialization;
using System.IO;
using System.Net;
using System.Text;
using System.ComponentModel;
using System.Security.Cryptography;

using Ximura;

using CH = Ximura.Common;
using Ximura.Framework;
using Ximura.Framework;
using Ximura.Communication;
#endregion // using
namespace Ximura.Communication
{
    /// <summary>
    /// This class contains the request information, and is used to send this to the W3C logger.
    /// </summary>
    public class SiteControllerRequestInfo: PoolableReturnableObjectBase
    {
        #region Declarations
        public Guid? SessionID;
        public Guid? BrowserID;
        public string UserID;
        public Guid? PageID;
        public string MappingID;
        public string ISOCountryCode;

        public DateTime? DateTimeLog;
        public IPEndPoint AddressClient;
        public IPEndPoint AddressServer;

        public string UserName;
        public string ServiceName;
        public string ServerName;
        public string ProtocolMethod;
        public string ProtocolStatus;

        public string URIStem;
        public string URIQuery;
        public long? BytesSent;
        public long? BytesReceived;
        public long? TimeTaken;
        public string ProtocolVersion;
        public string Host;
        public string UserAgent;
        public string Cookie;
        public string Referer;
        public string Debug;
        #endregion // Properties
        #region Constructor
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        public SiteControllerRequestInfo()
        {

        }
        #endregion // Constructor

        #region Reset()
        /// <summary>
        /// This method resets the request info.
        /// </summary>
        public override void Reset()
        {
            base.Reset();
            SessionID = null;
            BrowserID = null;
            UserID = null;
            PageID = null;
            MappingID = null;
            ISOCountryCode = null;

            DateTimeLog = null;
            AddressClient = null;
            AddressServer = null;

            UserName = null;
            ServiceName = null;
            ServerName = null;
            ProtocolMethod = null;
            ProtocolStatus = null;

            URIStem = null;
            URIQuery = null;
            BytesSent = null;
            BytesReceived = null;
            TimeTaken = null;
            ProtocolVersion = null;
            Host = null;
            UserAgent = null;
            Cookie = null;
            Referer = null;
            Debug = null;
        }
        #endregion // Reset()

        #region Load(SiteControllerContext context)
        /// <summary>
        /// This method loads the relevant information in to the info class.
        /// </summary>
        /// <param name="context"></param>
        public void Load(SiteControllerContext context)
        {
            ControllerRequest cr = context.ScriptRequest;
            ControllerSession cs = context.ScriptSession;

            SessionID = cr.SessionID;
            PageID = cr.IDContent;
            MappingID = context.ScriptSettings.MappingID;

            BrowserID = cr.RequestUserAgentID;

            DateTimeLog = DateTime.UtcNow;
            AddressClient = cr.RequestAddressRemote;
            AddressServer = cr.RequestAddressLocal;
            ISOCountryCode = cr.RequestAddressRemoteResolvedCountryCode;

            UserID = "";
            RealmAuthentication ra = cs.AuthenticationGet(context.ContextSettings.DomainDefault);
            if (ra != null)
                UserID = ra.Username;

            UserName = "";

            if (cs != null)
                foreach (RealmAuthentication auth in cs.Authentication)
                {
                    UserName += (auth.Authenticated ? "1" : "0") + "|" + auth.Username + "@" + auth.Realm + ";";
                }

            Debug = context.Debug;
            ServiceName = context.ContextSettings.ApplicationName;
            ServerName = "";
            ProtocolMethod = cr.RequestVerb;
            ProtocolStatus = cr.ResponseStatus;
            URIStem = cr.RequestPath;
            URIQuery = cr.RequestQuery == null || cr.RequestQuery == "?" ? "" : cr.RequestQuery;
            BytesSent = context.Response.Message == null ? 0 : context.Response.Message.Length;
            BytesReceived = context.Request.Message == null ? 0 : context.Request.Message.Length;
            TimeTaken = (long)context.TimeElapsed.TotalMilliseconds;
            ProtocolVersion = ((InternetMessage)context.Request.Message).Instruction.Protocol + "/" 
                + ((InternetMessage)context.Request.Message).Instruction.Version;
            Host = cr.RequestHost;
            UserAgent = cr.RequestUserAgent;
            Cookie = cr.RequestCookie;
            Referer = cr.RequestRefererURI;
        }
        #endregion // Load(SiteControllerContext context)
    }
}
