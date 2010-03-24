#region using
using System;
using System.Runtime.Serialization;
using System.IO;
using System.ComponentModel;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;

using Ximura;
using Ximura.Data;
using Ximura.Helper;
using Ximura.Data;
using CH = Ximura.Helper.Common;
using Ximura.Framework;
using Ximura.Framework;
using Ximura.Communication;
#endregion // using
namespace Ximura.Communication
{
    /// <summary>
    /// This is the digest authentication manager state.
    /// </summary>
    public class DigestAuthManager : AuthManager
    {
        #region Constructors
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        public DigestAuthManager() : this(null) { }
        /// <summary>
        /// This is the component model constructor.
        /// </summary>
        /// <param name="container">The container</param>
        public DigestAuthManager(IContainer container)
            : base(container)
        {
        }
        #endregion // Constructors

        #region WWWAuthString(SiteManagerContext context)
        /// <summary>
        /// This method formats the WWW-Authenticate header.
        /// </summary>
        /// <param name="context">The current context.</param>
        /// <returns>The WWW-Authenticate header value.</returns>
        protected override string WWWAuthString(SiteControllerContext context)
        {
            return  @"Digest realm=""" + context.ScriptSettings.AuthDomain +
                    @""", qop=""auth"", algorithm=md5, stale=false, nonce=""" +
                    Guid.NewGuid().ToString("N").ToLowerInvariant() + @""", opaque=""" +
                    context.ScriptRequest.SessionID.Value.ToString("N").ToLowerInvariant() + @"""";
        }
        #endregion // WWWAuthString(SiteManagerContext context)

        #region RequestValidate(SiteManagerContext context)
        /// <summary>
        /// This method validates the digest authentication.
        /// </summary>
        /// <param name="context">The current context.</param>
        /// <returns>Returns true of the response has been validated.</returns>
        protected override bool RequestValidate(SiteControllerContext context)
        {
            string authHeader = context.ScriptRequest.RequestAuthorization;

            //if (!authHeader.StartsWith("Digest "))
                return false;

            //IAuthUser rqUser = null;
            //try
            //{
            //    Dictionary<string, string> AuthParams = CH.SplitOnCharsUnique<string, string>(authHeader.Substring(7),
            //        CH.ConvPassthruLowerCase, CH.ConvStripSpeechMarks, new char[] { ',' }, new char[] { '=' });

            //    string username = AuthParams["username"] + "@" + AuthParams["realm"];

            //    //Ok, get the user object
            //    string status = RetrieveUserSecurity(context, new MailAddress(username), out rqUser);

            //    if (status != CH.HTTPCodes.OK_200)
            //    {
            //        context.ScriptRequest.ResponseStatus = CH.HTTPCodes.Unauthorized_401;
            //        return false;
            //    }

            //    //Adjust the HA1 if we are using MD5-Sess, if not the value will be passed through.
            //    string HA1 = CH.HA1CalculateMD5Sess(
            //        AuthParams.ContainsKey("algorithm") ? AuthParams["algorithm"] : null,
            //        CH.GetHexDecString(rqUser.TypeID, rqUser.UserName, rqUser.RealmDomain, rqUser.SecurityInfo).ToLowerInvariant(),
            //        AuthParams.ContainsKey("nonce") ? AuthParams["nonce"] : null,
            //        AuthParams.ContainsKey("cnonce") ? AuthParams["cnonce"] : null);

            //    string HA2 = CH.HA2Calculate(
            //        AuthParams.ContainsKey("algorithm") ? AuthParams["algorithm"] : null,
            //        context.ScriptRequest.RequestVerb,
            //        AuthParams.ContainsKey("uri") ? AuthParams["uri"] : null);

            //    string res = CH.DigestResponseCalculate(
            //        AuthParams.ContainsKey("algorithm") ? AuthParams["algorithm"] : null, HA1, HA2,
            //        AuthParams.ContainsKey("nonce") ? AuthParams["nonce"] : null,
            //        AuthParams.ContainsKey("qop") ? AuthParams["qop"] : null,
            //        AuthParams.ContainsKey("nc") ? AuthParams["nc"] : null,
            //        AuthParams.ContainsKey("cnonce") ? AuthParams["cnonce"] : null);

            //    if (res != AuthParams["response"])
            //    {
            //        context.ScriptRequest.ResponseStatus = CH.HTTPCodes.Unauthorized_401;
            //        return false;
            //    }

            //    if (!SessionSet(context, rqUser))
            //    {
            //        context.ScriptRequest.ResponseStatus = CH.HTTPCodes.Unauthorized_401;
            //        return false;
            //    }
            //}
            //finally
            //{
            //    if (rqUser != null && rqUser.ObjectPoolCanReturn)
            //        rqUser.ObjectPoolReturn();
            //}

            //return true;
        }
        #endregion // RequestValidate(SiteManagerContext context)
       
    }
}
