#region using
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Security;

using CH = Ximura.Common;
using System.Security.Principal;
using System.Configuration;
using System.Security.Permissions;
#endregion // using
namespace Ximura.Auth
{
    /// <summary>
    /// This class is used to provide authentication 
    /// </summary>
    [AspNetHostingPermission(SecurityAction.LinkDemand, Level = AspNetHostingPermissionLevel.Minimal)]
    public class AuthenticationModule : IHttpModule
    {
        #region Declarations
        /// <summary>
        /// This is the configuration.
        /// </summary>
        protected AuthenticationConfigSection mConfig;
        /// <summary>
        /// The authentication type.
        /// </summary>
        protected AuthenticationType mAuthType;
        /// <summary>
        /// The authentication realm.
        /// </summary>
        protected string mRealm;
        #endregion // Declarations
        #region Constructor
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        public AuthenticationModule()
        {

        }
        #endregion // Constructor

        #region Init(HttpApplication context)
        /// <summary>
        /// This method is used to initialize the module. 
        /// </summary>
        /// <param name="context">The application context.</param>
        public void Init(HttpApplication context)
        {
            mConfig = ConfigurationManager.GetSection("ximuraAuthSection") as AuthenticationConfigSection;

            context.AuthenticateRequest += new EventHandler(ContextAuthenticateRequest);
            context.EndRequest += new EventHandler(ContextEndRequest);
        }
        #endregion // Init(HttpApplication context)
        #region Dispose()
        /// <summary>
        /// This method does nothing.
        /// </summary>
        public virtual void Dispose()
        {
        }
        #endregion // Dispose()

        #region IN --> ContextEndRequest(object sender, EventArgs e)
        /// <summary>
        /// This method inserts the authenticate header 
        /// </summary>
        /// <param name="sender">The application.</param>
        /// <param name="e">The empty event arguments.</param>
        protected virtual void ContextEndRequest(object sender, EventArgs e)
        {
            HttpApplication application = (HttpApplication)sender;

            if (application.Response.StatusCode == 401)
            {
                if (Membership.Providers.Count == 0)
                    return;

                //Add the authentication headers. Note we allow multiple headers.
                if ((mAuthType & AuthenticationType.Digest) > 0)
                    application.Response.AddHeader("WWW-Authenticate", HeaderDigest(application, mRealm));

                if ((mAuthType & AuthenticationType.Basic) > 0)
                    application.Response.AddHeader("WWW-Authenticate", HeaderBasic(application, mRealm));
            }
        }
        #endregion // ContextEndRequest(object sender, EventArgs e)
        #region IN --> ContextAuthenticateRequest(object sender, EventArgs e)
        /// <summary>
        /// This method authenticates the incoming request.
        /// </summary>
        /// <param name="sender">The application.</param>
        /// <param name="e">The empty arguments.</param>
        protected virtual void ContextAuthenticateRequest(object sender, EventArgs e)
        {
            HttpApplication application = (HttpApplication)sender;
            if (application.Context.Request.IsAuthenticated)
                return;

            try
            {
                string httpAuth = application.Request.ServerVariables["HTTP_AUTHORIZATION"];
                if (httpAuth == null)
                    return;

                //OK, search for the first space in the string.
                int delimPos = httpAuth.IndexOf(' ');
                if (delimPos == -1)
                    return;

                //Ok, get the auth type.
                string auth = httpAuth.Substring(0, delimPos);
                string userName = null;

                //Ok, which auth type are we processing? 
                //Also check that we actually support the auth type being presented.
                switch (auth.ToUpperInvariant())
                {
                    case "DIGEST":
                        if (((mAuthType & AuthenticationType.Digest) == 0)
                            || !AuthenticateDigest(application, httpAuth.Substring(delimPos), out userName))
                            return;
                        break;
                    case "BASIC":
                        if (((mAuthType & AuthenticationType.Basic) == 0)
                            || !AuthenticateBasic(application, httpAuth.Substring(delimPos), out userName))
                            return;
                        break;
                }

                string[] Roles = null;
                GenericPrincipal UserPrincipal = new GenericPrincipal(new GenericIdentity(userName, auth), Roles);
                application.Context.User = UserPrincipal;
            }
            catch (Exception)
            {
                //Just to ensure, if we throw an exception during this stage, we ensure that the user is set to null.
                application.Context.User = null;
            }
        }
        #endregion // ContextAuthenticateRequest(object sender, EventArgs e)

        #region HeaderBasic(HttpApplication application, string realm)
        /// <summary>
        /// The basic realm.
        /// </summary>
        /// <param name="AuthDomain">The domain.</param>
        /// <param name="realm">This is the authentication realm, i.e. "Wally World"</param>
        /// <returns>Returns the configured string.</returns>
        private string HeaderBasic(HttpApplication application, string realm)
        {
            return string.Format(
                @"Basic realm=""{0}"""
                , realm);
        }
        #endregion
        #region AuthenticateBasic(HttpApplication application, string sAuth, out string userName)
        /// <summary>
        /// This method processes the standard authentication string.
        /// </summary>
        /// <param name="application">The application.</param>
        /// <param name="sAuth">The auth string.</param>
        /// <param name="userName">The output username.</param>
        /// <returns>Returns true if the user is authenticated.</returns>
        protected virtual bool AuthenticateBasic(HttpApplication application, string sAuth, out string userName)
        {
            byte[] data = Convert.FromBase64String(sAuth);

            string httpAuth = ASCIIEncoding.ASCII.GetString(data);

            int delimPos = httpAuth.IndexOf(':');
            if (delimPos == -1)
            {
                userName = null;
                return false;
            }

            userName = httpAuth.Substring(0, delimPos);

            //Get the HA1 string from the data store.
            string HA1;
            if (!HA1Retrieve(mRealm, userName, out HA1))
                return false;

            string HA1Compare = CH.HA1Calculate("md5", userName, mRealm, httpAuth.Substring(delimPos+1));

            return HA1 == HA1Compare;
        }
        #endregion // ProcessStandard(HttpApplication application, string sAuth, out string userName)

        #region HeaderDigest(HttpApplication application, string realm)
        /// <summary>
        /// The digest realm.
        /// </summary>
        /// <param name="realm">This is the authentication realm, i.e. "Wally World"</param>
        /// <returns>Returns the configured string.</returns>
        private string HeaderDigest(HttpApplication application, string realm)
        {
            return HeaderDigest(application,
                realm
                , "md5"
                , Guid.NewGuid().ToString("N").ToLowerInvariant()
                , Guid.NewGuid().ToString("N").ToLowerInvariant());

        }
        #endregion
        #region HeaderDigest(HttpApplication application, string realm, string algorithm, string Nonce, string Opaque)
        /// <summary>
        /// The digest realm.
        /// </summary>
        /// <param name="realm">This is the authentication realm, i.e. "Wally World"</param>
        /// <param name="algorithm">The algorithm, either md5</param>
        /// <param name="Nonce">The nonce string.</param>
        /// <param name="Opaque">The opaque string.</param>
        /// <returns>Returns the configured string.</returns>
        private string HeaderDigest(HttpApplication application, string realm, string algorithm, string Nonce, string Opaque)
        {
            return string.Format(
                @"Digest realm=""{0}"", qop=""auth"", algorithm={1}, stale=false, nonce=""{2}"", opaque=""{3}"""
                , realm
                , algorithm
                , Nonce
                , Opaque);
        }
        #endregion
        #region AuthenticateDigest(HttpApplication application, string sAuth, out string userName)
        /// <summary>
        /// This method processes the standard authentication string.
        /// </summary>
        /// <param name="application">The application.</param>
        /// <param name="sAuth">The auth string.</param>
        /// <param name="userName">The output username.</param>
        /// <returns>Returns true if the user is authenticated.</returns>
        protected virtual bool AuthenticateDigest(HttpApplication application, string sAuth, out string userName)
        {
            Dictionary<string, string> AuthParams =
                CH.SplitOnCharsUnique<string, string>(sAuth
                , CH.ConvPassthruLowerCase
                , CH.ConvStripSpeechMarks
                , new char[] { ',' }
                , new char[] { '=' });

            userName = AuthParams["username"];

            if (mRealm != AuthParams["realm"])
                return false;

            //Get the HA1 string from the data store.
            string HA1;
            if (!HA1Retrieve(mRealm, userName, out HA1))
                return false;

            //Adjust the HA1 if we are using MD5-Sess, if not the value will be passed through.
            string HA1Calc = CH.HA1CalculateMD5Sess(
                AuthParams.ContainsKey("algorithm") ? AuthParams["algorithm"] : null,
                HA1,
                AuthParams.ContainsKey("nonce") ? AuthParams["nonce"] : null,
                AuthParams.ContainsKey("cnonce") ? AuthParams["cnonce"] : null);

            string HA2Calc = CH.HA2Calculate(
                AuthParams.ContainsKey("algorithm") ? AuthParams["algorithm"] : null,
                application.Request.HttpMethod,
                AuthParams.ContainsKey("uri") ? AuthParams["uri"] : null);

            string res = CH.DigestResponseCalculate(
                AuthParams.ContainsKey("algorithm") ? AuthParams["algorithm"] : null,
                HA1Calc, HA2Calc,
                AuthParams.ContainsKey("nonce") ? AuthParams["nonce"] : null,
                AuthParams.ContainsKey("qop") ? AuthParams["qop"] : null,
                AuthParams.ContainsKey("nc") ? AuthParams["nc"] : null,
                AuthParams.ContainsKey("cnonce") ? AuthParams["cnonce"] : null);

            string result = AuthParams["response"];

            return (result == res);
        }
        #endregion // ProcessDigest(HttpApplication application, string sAuth, out string userName)

        #region HA1Retrieve(string realm, string userName, out string HA1)
        /// <summary>
        /// This method retrieves the HA1 string for the specific user.
        /// </summary>
        /// <param name="realm">The realm.</param>
        /// <param name="userName">The username.</param>
        /// <param name="HA1">The case-sensitive HA1 string.</param>
        /// <returns>The true is the username can be resolved for that realm.</returns>
        protected virtual bool HA1Retrieve(string realm, string userName, out string HA1)
        {
            HA1 = CH.HA1Calculate("md5", userName, mRealm, "p00ntag");

            return true;
        }
        #endregion

    }

    #region AuthenticationType
    /// <summary>
    /// This is the authentication type used by the application.
    /// </summary>
    [Flags]
    public enum AuthenticationType
    {
        /// <summary>
        /// Basic authentication.
        /// </summary>
        Basic = 1,
        /// <summary>
        /// Digest authentication.
        /// </summary>
        Digest = 2
    }
    #endregion // AuthenticationType

}
