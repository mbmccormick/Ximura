#region using
using System;
using System.Runtime.Serialization;
using System.IO;
using System.ComponentModel;
using System.Collections.Generic;
using System.Text;
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
    /// This is the basic authentication state.
    /// </summary>
    public class BasicAuthManager : AuthManager
    {
        #region Constructors
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        public BasicAuthManager() : this(null) { }
        /// <summary>
        /// This is the component model constructor.
        /// </summary>
        /// <param name="container">The container</param>
        public BasicAuthManager(IContainer container)
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
            return @"Basic realm=""" + context.ScriptRequest.ResponseStateAuthDomain + @"""";
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
            //string authHeader = context.ScriptRequest.RequestAuthorization;

            //if (!authHeader.StartsWith("Basic "))
                return false;

            //IAuthUser rqUser = null;

            //try
            //{
            //    byte[] bsecData = Convert.FromBase64String(authHeader.Substring(6));
            //    string secData = Encoding.UTF8.GetString(bsecData);

            //    int pointer = secData.IndexOf(':');

            //    if (pointer == -1)
            //    {
            //        context.ScriptRequest.ResponseStatus = CH.HTTPCodes.Unauthorized_401;
            //        return false;
            //    }

            //    string userID = secData.Substring(0, pointer - 1);
            //    string userPassword = secData.Substring(pointer);
            //    string userDomain = context.ScriptRequest.ResponseStateAuthDomain;

            //    string username = userID + "@" + userDomain;

            //    string status = RetrieveUserSecurity(context, new MailAddress(username), out rqUser);
 
            //    if (status != CH.HTTPCodes.OK_200)
            //    {
            //        context.ScriptRequest.ResponseStatus = CH.HTTPCodes.Unauthorized_401;
            //        return false;
            //    }

            //    string HA1Member = CH.GetHexDecString(rqUser.TypeID, rqUser.UserName, rqUser.Realm, rqUser.SecurityInfo).ToLowerInvariant();

            //    //Adjust the HA1 if we are using MD5-Sess, if not the value will be passed through.
            //    string HA1Request = CH.HA1Calculate("md5", rqUser.UserName, rqUser.Realm, userPassword);


            //    if (HA1Member != HA1Request)
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
