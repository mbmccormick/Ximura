#region using
using System;
using System.Runtime.Serialization;
using System.IO;
using System.ComponentModel;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Net;
using System.Net.Mail;
using System.Diagnostics;

using Ximura;
using Ximura.Data;

using Ximura.Data;
using CH = Ximura.Common;
using Ximura.Framework;
using Ximura.Framework;
using Ximura.Communication;
#endregion // using
namespace Ximura.Communication
{
    /// <summary>
    /// This class is responsible for handling the cookie information. This may require the cookie
    /// information to be encrypted or decrypted.
    /// </summary>
    public class ProtocolHTTPState : ProtocolBaseState
    {
        #region Declarations
        RijndaelManaged cookieKey = null;
        private ISiteControllerLogger mSClogger = null;
        #endregion // Declarations
        #region Constructors
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        public ProtocolHTTPState() : this(null) { }
        /// <summary>
        /// This is the component model constructor.
        /// </summary>
        /// <param name="container">The container</param>
        public ProtocolHTTPState(IContainer container)
            : base(container)
        {
        }
        #endregion // Constructors

        #region IN --> MessageDecode(SiteControllerContext context)
        /// <summary>
        /// This state is used to decode the incoming message, and extract important
        /// information such as the HTTP cookie value for the HTTP protocol.
        /// </summary>
        /// <param name="context">This is the request context</param>
        public override bool MessageDecode(SiteControllerContext context)
        {
            //Set the HTTP response message.
            context.ProtocolResponse = context.GetObjectPool<InternetMessageResponse>().Get();
            context.ProtocolResponse.BeginInit();

            InternetInstructionFragmentBase baseFragment = context.ProtocolRequest.Instruction;
            ControllerRequest contRQ = context.ScriptRequest;
            HTTPRequestMessage rq = context.RequestMessage as HTTPRequestMessage;

            contRQ.RequestURI = context.RequestURI;
            contRQ.RequestAddressLocal = new IPEndPointExtended(context.RequestURILocal);
            contRQ.RequestAddressRemote = new IPEndPointExtended(context.RequestURIRemote);

            contRQ.RequestVerb = context.RequestMethod;

            contRQ.RequestProtocolVersion = baseFragment.Version;
            contRQ.ResponseProtocolVersion = baseFragment.Version;
            contRQ.ResponseProtocol = contRQ.RequestURI.Scheme;

            HeadersProcess(context, contRQ, rq);

            CookiesProcess(context);

            return context.ScriptRequestResolved;
        }
        #endregion // CookiePrepare(SiteManagerContext context)
        #region HeadersProcess(ControllerRequest contRQ, HTTPRequestMessage rq)
        /// <summary>
        /// This processes the incoming headers and adds them to the Controller Request.
        /// </summary>
        /// <param name="contRQ"></param>
        /// <param name="rq"></param>
        protected virtual void HeadersProcess(SiteControllerContext context, ControllerRequest contRQ, HTTPRequestMessage rq)
        {
            string isoCode;
            if (mSClogger != null &&
                mSClogger.ResolveAddress(contRQ.RequestAddressRemote.Address, out isoCode))
            {
                contRQ.RequestAddressRemoteResolvedCountryCode = isoCode;
            }

            foreach (HeaderFragment frag in rq.HeaderFragments())
            {
                switch (frag.Field.ToLower())
                {
                    case "x-wap-profile":
                        contRQ.RequestUserAgentWapProfile = frag.FieldData;
                        break;
                    case "if-none-match":
                        contRQ.RequestIfNoneMatch = frag.FieldData;
                        break;
                    case "connection":
                        contRQ.RequestConnection = frag.FieldData;
                        break;
                    case "accept":
                        contRQ.RequestAccept = frag.FieldData;
                        break;
                    case "accept-language":
                        contRQ.RequestAcceptLanguage = frag.FieldData;
                        break;
                    case "accept-encoding":
                        contRQ.RequestAcceptEncoding = frag.FieldData;
                        break;
                    case "accept-charset":
                        contRQ.RequestAcceptCharset = frag.FieldData;
                        break;
                    case "user-agent":
                        contRQ.RequestUserAgent = frag.FieldData;
                        break;
                    case "referer":
                        contRQ.RequestRefererURI = frag.FieldData;
                        break;
                    case "authorization":
                        contRQ.RequestAuthorization = frag.FieldData;
                        break;
                    case "content-type":
                        contRQ.RequestContentType = frag.FieldData;
                        break;
                    case "content-length":
                        contRQ.RequestContentLength = frag.FieldData;
                        break;
                    case "content-encoding":
                        contRQ.RequestContentEncoding = frag.FieldData;

                        break;
                    case "host":
                        break;

                    default:
                        contRQ.RequestHeaderAdd(frag.Field, frag.FieldData);
                        break;
                }
            }
        }
        #endregion // HeadersProcess(ControllerRequest contRQ, InternetMessageRequest rq)
        #region CookiesProcess(SiteControllerContext context)
        /// <summary>
        /// This method processes the incoming HTTP cookies
        /// </summary>
        /// <param name="context">The current request context.</param>
        protected void CookiesProcess(SiteControllerContext context)
        {
            foreach (string cookieString in context.ScriptRequest.RequestHeaderCookies)
            {
                string[] cookies = cookieString.Split(';');
                foreach (string cookie in cookies)
                    CookieProcessIncoming(context, cookie);
            }
        }
        #endregion // CookiesProcess(SiteControllerContext context)
        #region CookieProcessIncoming(string cookie)
        /// <summary>
        /// This method processes an incoming session.
        /// </summary>
        /// <param name="cookie">The cookie to process.</param>
        protected virtual void CookieProcessIncoming(SiteControllerContext context, string cookie)
        {
            if (cookie.Trim().StartsWith(context.ContextSettings.CookieName1))
            {
                string dataS = CookieExtractValue(cookie);

                string sessionCookieData = context.ContextSettings.SessionCookieDecrypt(dataS);

                try
                {
                    if (sessionCookieData != null)
                        context.ScriptRequest.SessionID = new Guid(sessionCookieData);
                }
                catch (Exception ex)
                {
                    context.ScriptRequest.SessionID = Guid.Empty;
                }
            }

            if (cookie.Trim().StartsWith(context.ContextSettings.CookieName2))
            {
                string dataM = CookieExtractValue(cookie);

                string memberCookieData = context.ContextSettings.MemberCookieDecrypt(dataM);

                if (memberCookieData != null)
                    context.ScriptRequest.CookieAdd(context.ContextSettings.CookieName2, memberCookieData);
            }

            if (cookie.Trim().StartsWith(context.ContextSettings.CookieName3))
            {
                string dataB = CookieExtractValue(cookie);

                string browserCookieData = context.ContextSettings.MemberCookieDecrypt(dataB);

                try
                {
                    if (browserCookieData != null)
                        context.ScriptRequest.RequestUserAgentID = new Guid(browserCookieData);
                }
                catch (Exception ex)
                {
                    context.ScriptRequest.RequestUserAgentID = Guid.NewGuid();
                }

            }
        }
        #endregion // CookieProcess(string cookie)
        #region CookieExtractValue(string cookie)
        /// <summary>
        /// This method extracts the cookie value.
        /// </summary>
        /// <param name="cookie">The cookie key and value.</param>
        /// <returns>The cookie value.</returns>
        private string CookieExtractValue(string cookie)
        {
            try
            {
                int eqPos = cookie.IndexOf('=');
                if (eqPos == -1)
                    return null;

                return cookie.Substring(eqPos + 1);
            }
            catch
            {
                return null;
            }
        }
        #endregion // ExtractCookieValue(string cookie)

        #region ProcessMemberSecurity(SiteControllerContext context)
        /// <summary>
        /// This method processes any security parameters retrieved from the Member Session.
        /// </summary>
        /// <param name="context">The current context.</param>
        protected override void ProcessMemberSecurity(SiteControllerContext context)
        {
            //Check we have a member account stored in the cookie.
            foreach (ControllerRequest.Cookie cookieMember in context.ScriptRequest.CookieGet(context.ContextSettings.CookieName2))
            {
               ProcessMemberCookie(context, cookieMember);
            }
        }
        #endregion // ProcessMemberSecurity(SiteControllerContext context)
        #region ProcessMemberCookie(SiteControllerContext context, ControllerRequest.Cookie cookieMember)
        /// <summary>
        /// This method sets the authentication for a stored cookie credentials.
        /// </summary>
        /// <param name="context">The current context.</param>
        /// <param name="cookieMember">The member cookie.</param>
        protected virtual void ProcessMemberCookie(SiteControllerContext context, ControllerRequest.Cookie cookieMember)
        {
            string[] members = cookieMember.Value.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string item in members)
            {
                ProcessMemberCookieItem(context, item);
            }
        }
        #endregion // ProcessMemberCookie(SiteControllerContext context, ControllerRequest.Cookie cookieMember)
        #region ProcessMemberCookieItem(SiteControllerContext context, string item)
        /// <summary>
        /// This method processes the individual section from the incoming cookie.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="item">The incoming cookie item.</param>
        protected virtual void ProcessMemberCookieItem(SiteControllerContext context, string item)
        {
            string[] itemSplit = item.Split(new char[] { '|' });

            if (itemSplit.Length != 3)
                return;

            string realm = itemSplit[0];
            string persist = itemSplit[1];
            string userID = itemSplit[2];

            if (userID.Contains("@"))
            {
                MailAddress address = new MailAddress(userID);
                if (realm.ToLowerInvariant() == address.Host.ToLowerInvariant())
                    userID = address.User.ToLowerInvariant();
                else
                    return;
            }

            if (context.ScriptSession != null)
                context.ScriptSession.AuthenticationSet(realm, userID, persist == "1", null);
        }
        #endregion // ProcessMemberCookieItem(SiteControllerContext context, string item)

        #region IN --> ResponsePrepare(SiteManagerContext context)
        /// <summary>
        /// This method builds the response.
        /// </summary>
        /// <param name="context"></param>
        public override void ResponsePrepare(SiteControllerContext context)
        {
            SessionCookiePrepare(context);
        }
        #endregion
        #region SessionCookiePrepare(SiteManagerContext context)
        /// <summary>
        /// This method is specifically used to set the session cookie.
        /// </summary>
        /// <param name="context">The request context.</param>
        protected virtual void SessionCookiePrepare(SiteControllerContext context)
        {
            //Set the session cookie
            if (context.ScriptSession != null)
                context.ScriptRequest.ResponseHeaderAdd("Set-Cookie", 
                    context.ContextSettings.CookieName1 
                    + @"=" + SessionCookieID(context) 
                    + "; path=/; httponly;");

            string authCookie = null;

            if (context.ScriptSession!=null)
                foreach(RealmAuthentication ra in context.ScriptSession.Authentication)
                {
                    if (ra.CookiePersist && ra.Authenticated)
                    {
                        authCookie += ra.Realm + "|1|" + ra.Username + ";";
                    }
                    else
                    {
                        authCookie += ra.Realm + "|0|" + ra.Username + ";";
                    }
                }

            if (authCookie!=null)
                context.ScriptRequest.ResponseHeaderAdd("Set-Cookie",
                    context.ContextSettings.CookieName2 + @"=" + context.ContextSettings.MemberCookieEncrypt(authCookie)
                    + @";path=/;domain=" + context.ScriptRequest.RequestHost + ";expires="
                    + DateTime.UtcNow.AddMonths(12).ToString("ddd, dd MMM yyyy HH:mm:ss") 
                    + " GMT; httponly");
            //else
            //    context.ScriptRequest.ResponseHeaderAdd("Set-Cookie",
            //        context.FSM.CookieName2 + @"="
            //        + @";path=/;domain=" + context.ScriptRequest.RequestHost + ";expires="
            //        + DateTime.UtcNow.AddMonths(1).ToString("ddd, dd MMM yyyy HH:mm:ss") + " GMT; httponly");

            //Set the browserID cookie.
            if (!context.ScriptRequest.RequestUserAgentID.HasValue)
                context.ScriptRequest.RequestUserAgentID = Guid.NewGuid();

            context.ScriptRequest.ResponseHeaderAdd("Set-Cookie",
                context.ContextSettings.CookieName3 + @"=" + context.ContextSettings.MemberCookieEncrypt(context.ScriptRequest.RequestUserAgentID.ToString())
                + @";path=/;domain=" + context.ScriptRequest.RequestHost + ";expires="
                + DateTime.UtcNow.AddMonths(24).ToString("ddd, dd MMM yyyy HH:mm:ss")
                + " GMT; httponly");

        }
        #endregion // SessionCookiePrepare(SiteManagerContext context)
        #region SessionCookieID(SiteManagerContext context)
        /// <summary>
        /// This method gets the excrypted session id from the session cookie.
        /// </summary>
        /// <param name="context">The current context.</param>
        /// <returns>Returns an encrypted base64 string.</returns>
        protected virtual string SessionCookieID(SiteControllerContext context)
        {
            Guid sessionID = context.ScriptRequest.SessionID.Value;

            return context.ContextSettings.SessionCookieEncrypt(sessionID.ToString().ToUpperInvariant());
        }
        #endregion // SessionIDCookie(SiteManagerContext context)

        #region IN --> ResponseComplete(SiteManagerContext context)
        /// <summary>
        /// This method sets any common headers to the response.
        /// </summary>
        /// <param name="context"></param>
        public override void ResponseComplete(SiteControllerContext context)
        {
            TranslateResponseCode(context);

            context.ProtocolResponse.Instruction.Protocol = context.ProtocolRequest.Instruction.Protocol;
            context.ProtocolResponse.Instruction.Version = context.ProtocolRequest.Instruction.Version;

            context.ProtocolResponse.HeaderAdd("Server", context.ContextSettings.ServerID);
            context.ProtocolResponse.HeaderAdd("Date", DateTime.UtcNow.ToString("ddd, dd MMM yyyy HH:mm:ss") + " GMT");
            context.ProtocolResponse.HeaderAdd("Connection", "Keep-Alive");

            foreach (KeyValuePair<string, string> header in context.ScriptRequest.ResponseHeaders)
            {
                //if (header.Key.ToLowerInvariant() != "etag" && context.ProtocolResponse.)
                //    continue;

                context.ProtocolResponse.HeaderAdd(header.Key, header.Value);
            }

            context.ProtocolResponse.EndInit();

            context.Response.Status = CH.HTTPCodes.OK_200;
        }
        #endregion

        #region IN --> Log(SiteControllerContext context)
        /// <summary>
        /// This state method logs the request is a W3C format for analysis.
        /// </summary>
        /// <param name="context">The current context.</param>
        public override void Log(SiteControllerContext context)
        {
            SiteControllerRequestInfo info = null;
            bool exception = false;
            try
            {
                info = context.GetObject<SiteControllerRequestInfo>();
                info.Load(context);

                if (mSClogger != null)
                    mSClogger.LogRequestEnqueue(info);
            }
            catch (Exception ex)
            {
                XimuraAppTrace.Write(string.Format("SiteController logging error: {0}/r/n/r/n{1}", ex.Message, ex.StackTrace),
                    context.CommandName, EventLogEntryType.Warning, "EventLog");

                exception = true;
            }
            finally
            {
                if ((exception || mSClogger == null) && info != null && info.ObjectPoolCanReturn)
                    info.ObjectPoolReturn();
            }
        }
        #endregion // IN --> Log(SiteControllerContext context)

        #region ServicesReference/ServicesDereference
        /// <summary>
        /// This override retrieves the logger reference.
        /// </summary>
        protected override void ServicesReference()
        {
            base.ServicesReference();
            mSClogger = GetService(typeof(ISiteControllerLogger)) as ISiteControllerLogger;
        }
        /// <summary>
        /// this override clears the logger reference.
        /// </summary>
        protected override void ServicesDereference()
        {
            mSClogger = null;
            base.ServicesDereference();
        }
        #endregion // ServicesReference/ServicesDereference

    }
}