#region using
using System;
using System.Runtime.Serialization;
using System.IO;
using System.Net.Mail;
using System.ComponentModel;
using System.Collections.Generic;

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
    /// This is the base authentication manager state. Authentication managers should inherit from this base state.
    /// </summary>
    public class AuthManager : SiteControllerState
    {
        #region Constructors
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        public AuthManager() : this(null) { }
        /// <summary>
        /// This is the component model constructor.
        /// </summary>
        /// <param name="container">The container</param>
        public AuthManager(IContainer container)
            : base(container)
        {
        }
        #endregion // Constructors

        #region IN --> RequestAuthenticate(SiteManagerContext context)
        /// <summary>
        /// This emthod loads the list to ensure the privacy settings and that
        /// the correct URI is accessed
        /// </summary>
        /// <param name="context">The current context.</param>
        /// <returns>Returns true if the list should be displayed.</returns>
        public override bool RequestAuthenticate(SiteControllerContext context)
        {
            if (context.ScriptRequest.RequestAuthorization != null && context.ScriptRequest.RequestAuthorization != "")
                if (RequestValidate(context))
                    return true;

            if (context.ScriptSettings.AuthDomain != null && context.ScriptSettings.AuthDomain != "")
                context.ScriptRequest.ResponseHeaderAdd("WWW-Authenticate", WWWAuthString(context));

            context.ScriptRequest.ResponseStatus = CH.HTTPCodes.Unauthorized_401;

            return false;
        }
        #endregion // RequestAuthenticate(SiteManagerContext context)

        #region RetrieveUserSecurity(SiteControllerContext context, MailAddress username, out User rqUser)
        /// <summary>
        /// This method retrieves the correct user object from the CDS for the specific domain.
        /// </summary>
        /// <param name="context">The request context.</param>
        /// <param name="username">The username containing the userid and the domain.</param>
        /// <param name="rqUser">An out parameter that contains the user obejct if the request is successful.</param>
        /// <returns>Returns the HTTP status code for the request. 200 indicated success, everything else indicates a fail.</returns>
        protected virtual string RetrieveUserSecurity(SiteControllerContext context, MailAddress username, out IAuthUser rqUser)
        {
            Content secEnt = null;
            try
            {
                Type userType = context.ContextSettings.ResolveSecurityObjectType(username.Host);

                string status = context.CDSHelper.Execute(userType, CDSData.Get(CDSStateAction.Read, "userid", username.Address), out secEnt);

                rqUser = secEnt as IAuthUser;

                return status;
            }
            catch (Exception ex)
            {
                //OK, we have an error. Tidy up any mess and returns any objects to the pool.
                if (secEnt != null && secEnt.ObjectPoolCanReturn)
                    secEnt.ObjectPoolReturn();

                rqUser = null;
                return CH.HTTPCodes.InternalServerError_500;
            }
        }
        #endregion // UserRetrieve(SiteControllerContext context, MailAddress username, out User rqUser)


        #region WWWAuthString(SiteManagerContext context)
        /// <summary>
        /// This method formats the WWW-Authenticate header.
        /// </summary>
        /// <param name="context">The current context.</param>
        /// <returns>The WWW-Authenticate header value.</returns>
        protected virtual string WWWAuthString(SiteControllerContext context)
        {
            throw new NotImplementedException("AuthManager->WWWAuthString is not implemented.");
        }
        #endregion // WWWAuthString(SiteManagerContext context)
        #region RequestValidate(SiteManagerContext context)
        /// <summary>
        /// This method validates the digest authentication.
        /// </summary>
        /// <param name="context">The current context.</param>
        /// <returns>Returns true of the response has been validated.</returns>
        protected virtual bool RequestValidate(SiteControllerContext context)
        {
            throw new NotImplementedException("AuthManager->RequestValidate is not implemented.");
        }
        #endregion
        #region SessionSet(SiteManagerContext context, Member rqMember)
        /// <summary>
        /// This method sets the session value
        /// </summary>
        /// <param name="context">The current context.</param>
        /// <param name="rqMember">The member account to set.</param>
        /// <returns>Returns </returns>
        protected virtual bool SessionSet(SiteControllerContext context, IAuthUser rqUser)
        {
            if (context.ScriptSession == null || rqUser == null)
                return false;

            return context.ScriptSession.AuthenticationSet(rqUser, false, true);
        }
        #endregion // SessionSet(SiteManagerContext context, Member rqMember)

    }
}
