#region using
using System;
using System.Runtime.Serialization;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.ComponentModel;
using System.Security.Cryptography;

using Ximura;
using Ximura.Data;
using Ximura.Data;

using CH = Ximura.Common;
using RH = Ximura.Reflection;
using Ximura.Framework;
using Ximura.Framework;
using Ximura.Communication;
#endregion // using
namespace Ximura.Communication
{
    /// <summary>
    /// This class contains the settings for the context.
    /// </summary>
    public class SiteControllerSettings : ContextSettings<SiteControllerState, SiteControllerConfiguration, SiteControllerPerformance>
    {
        #region Declarations
        ///// <summary>
        ///// This is the resolver for the site.
        ///// </summary>
        //protected ControllerScriptResolver mCSR;
        private Dictionary<string, ControllerScript> mScripts;
        private string mRootScript;

        private RijndaelSecHolder mCookieSessionKey = null;
        private RijndaelSecHolder mCookieMemberKey = null;
        private object syncKey = new object();
        #endregion // Declarations
        #region Constructors
        /// <summary>
        /// This constructor is called by the FSM when initiating the settings.
        /// </summary>
        public SiteControllerSettings():base()
        {
            mScripts = new Dictionary<string, ControllerScript>();
        }
        #endregion

        #region ServerID
        /// <summary>
        /// This is the server ID for the system.
        /// </summary>
        public virtual string ServerID
        {
            get
            {
                return ApplicationName;
            }
        }
        #endregion // ServerID

        #region ScriptRecurseLimit
        /// <summary>
        /// This is the script recurse limit. The default value is set to 10.
        /// </summary>
        protected virtual int ScriptRecurseLimit
        {
            get
            {
                return 10;
            }
        }
        #endregion // ScriptRecurseLimit

        #region ScriptParametersSet(ControllerRequest ScriptRequest)
        /// <summary>
        /// This method resolves the request.
        /// </summary>
        /// <param name="ScriptRequest">The request cache, to add the necessary fields.</param>
        /// <returns>Returns true on a successful match.</returns>
        //public bool ScriptParametersSet(ControllerRequest ScriptRequest)
        //{
        //    Uri id = ScriptRequest.RequestURI;
        //    string userAgent = ScriptRequest.RequestUserAgent;
        //    string method = ScriptRequest.RequestVerb;

        //    Dictionary<string, string> variables = new Dictionary<string, string>();
        //    MappingSettings map = PoolManager.GetPoolManager<MappingSettings>().Get();

        //    if (!PathResolve("http", mRootScript, id, userAgent, method, variables, 10, map))
        //    {
        //        ScriptRequest.ResponseStatus = CH.HTTPCodes.NotFound_404;
        //        return false;
        //    }

        //    foreach(KeyValuePair<string,string> item in variables)
        //    {
        //        ScriptRequest.VariableAdd(item.Key, item.Value);
        //    }

        //    ScriptRequest.ResponseID = map.MappingID;
        //    ScriptRequest.ResponseTemplate = map.Template;

        //    //ScriptRequest.ResponseOutputType = map.OutputType;
        //    //ScriptRequest.ResponseOutputMIMEType = map.OutputMIMEType;
        //    //ScriptRequest.ResponseOutput = map.OutputHolder;
        //    ScriptRequest.ResponseOutputType = map.OutputColl[0].OutputType;
        //    ScriptRequest.ResponseOutputMIMEType = map.OutputColl[0].OutputMIMEType;
        //    ScriptRequest.ResponseOutput = map.OutputColl[0].Output;

        //    ScriptRequest.ResponseStateOutput = map.MappingState;
        //    ScriptRequest.ResponseStateAuth = map.AuthState;
        //    ScriptRequest.ResponseStateAuthDomain = map.AuthDomain;


        //    return true;
        //}
        #endregion // ResolveUri(Uri RequestURI, ControllerRequest ScriptRequest)

        public Type ResolveSecurityObjectType(string realm)
        {
            ControllerScript script = mScripts[mRootScript];

            string objectType = script.ResolveAuthType(realm);

            return RH.CreateTypeFromString(objectType);
        }

        #region Public    --> ResolvePath
        /// <summary>
        /// This method resolves the path and returns the settings for the incoming request.
        /// </summary>
        /// <param name="server">The server type.</param>
        /// <param name="path">The request path.</param>
        /// <param name="userAgent">The requesting user agent.</param>
        /// <param name="method">The request method.</param>
        /// <param name="variables">A return collection containing any variables from the request path.</param>
        /// <param name="map">A return class containing the script settings.</param>
        /// <returns>Returns true is the path has been successfully resolved.</returns>
        public bool ResolvePath(string server, Uri path, string userAgent, string method,
            out IDictionary<string, string> variables, out MappingSettings map)
        {
            //Get the mapping setting class to return to the controller.
            map = PoolManager.GetPoolManager<MappingSettings>().Get();

            variables = new Dictionary<string, string>();
            if (!ResolvePath(mRootScript, server, path, userAgent, 
                method, variables, ScriptRecurseLimit, map))
            {
                return false;
            }
            return true;
        }
        #endregion // ResolvePath
        #region Protected --> ResolvePath
        /// <summary>
        /// This recursive method will resolve the navigation uri from the various scripts stored in the collection.
        /// </summary>
        /// <param name="scriptName">The initial script name to resolve.</param>
        /// <param name="id">The uri request.</param>
        /// <param name="variables">The output variables from tha navigation.</param>
        /// <param name="recurseLimit">The recurse limit. 
        /// When this value reaches 0 the method will throw an exception. 
        /// This is to stop scripts the have circular references.</param>
        /// <param name="map">The selected mapping.</param>
        /// <returns>Returns true if the uri id was successfully resolved.</returns>
        protected bool ResolvePath(string scriptName, string server, Uri id, string userAgent, string method,
            IDictionary<string, string> variables, int recurseLimit, MappingSettings map)
        {
            if (recurseLimit == 0)
                throw new ArgumentOutOfRangeException("Script recursion limit reached.");
            if (!mScripts.ContainsKey(scriptName))
                throw new ArgumentOutOfRangeException(scriptName + " cannot be resolved.");
            if (map==null)
                throw new ArgumentNullException("Map settings are null.");

            recurseLimit--;

            ControllerScript script = mScripts[scriptName];

            bool success;

            bool resolved = script.ResolveUri(server, id, userAgent, method, variables, out success, map);

            if (resolved && !success)
            {
                if (map.Redirect != null)
                {
                    resolved = ResolvePath(map.Redirect, server, id, userAgent, method, variables, recurseLimit, map);
                }
                else
                    return false;
            }

            return resolved;
        }
        #endregion // ResolvePath(string scriptName, Uri id, Dictionary<string, string> variables, int recurseLimit, out Mapping map)

        #region InitializeScripts(string rootScriptID)
        public delegate string ExternalResolveScript(string refScript, out ControllerScript script);

        private ExternalResolveScript mExtScriptResolver = null;
        /// <summary>
        /// This method initializes the scripts ready for processing.
        /// </summary>
        /// <param name="rootScriptID">The root script ID.</param>
        /// <returns>Returns true if the script initialized successfully.</returns>
        public bool InitializeScripts(string rootScriptID, ExternalResolveScript extScriptResolver)
        {
            mExtScriptResolver = extScriptResolver;
            mRootScript = rootScriptID;
            ProcessReferenceScript(mScripts, rootScriptID);

            return true;
        }
        #endregion // InitializeScripts(string rootScriptID)
        #region ProcessReferenceScript(Dictionary<string, ControllerScript> scripts, string refScript)
        /// <summary>
        /// This recursive function is used to load the root script and any child scripts in to the collection.
        /// </summary>
        /// <param name="scripts">The script collection.</param>
        /// <param name="refScript">The script name.</param>
        protected virtual void ProcessReferenceScript(Dictionary<string, ControllerScript> scripts, string refScript)
        {
            if (scripts.ContainsKey(refScript))
                return;

            ControllerScript newScript;
            throw new NotImplementedException();
            string status;
            //= CDSHelper.Execute<ControllerScript>(
            //    CDSData.Get(CDSAction.Read, "name", refScript), out newScript);

            if (status != CH.HTTPCodes.OK_200 && mExtScriptResolver != null)
            {
                status = mExtScriptResolver(refScript, out newScript);
            }
            
            if (status != CH.HTTPCodes.OK_200)
            {
                throw new SiteControllerException(string.Format("The script '{0}' cannot be loaded.", refScript));
            }

            newScript.Compile();

            scripts.Add(refScript, newScript);

            foreach (string refChild in newScript.ReferenceScripts)
            {
                ProcessReferenceScript(scripts, refChild);
            }
        }
        #endregion // ProcessReferenceScript(Dictionary<string, ControllerScript> scripts, string refScript)

        #region CookieName1
        /// <summary>
        /// This is the cookie name 1 for the session cookie.
        /// </summary>
        public string CookieName1
        {
            get
            {
                return "_ASM1";
            }
        }
        #endregion // CookieName1
        #region CookieName2
        /// <summary>
        /// This is the cookie name 2 for the member ID cookie.
        /// </summary>
        public string CookieName2
        {
            get
            {
                return "_ASM2";
            }
        }
        #endregion // CookieName2
        #region CookieName3
        /// <summary>
        /// This is the cookie name 3 for the browser ID cookie.
        /// </summary>
        public string CookieName3
        {
            get
            {
                return "_ASM3";
            }
        }
        #endregion // CookieName2

        #region CookieDomain
        /// <summary>
        /// This is the domain name stored in the cookie.
        /// </summary>
        public string CookieDomain
        {
            get
            {
                return ".snagsta.com";
            }
        }
        #endregion // CookieDomain

        #region RijndaelSecHolder
        /// <summary>
        /// This class holds the Rijndael class.
        /// </summary>
        protected class RijndaelSecHolder
        {
            #region Declarations
            private Rfc2898DeriveBytes mPwdGenSession = null;
            byte[] mRGBKey;
            byte[] mRGBIV;
            RijndaelManaged rjn;
            #endregion // Declarations
            #region RijndaelSecHolder(byte[] pass, byte[] salt)
            /// <summary>
            /// This is the default constructor for the security holder.
            /// </summary>
            public RijndaelSecHolder()
            {
                rjn = new RijndaelManaged();
                rjn.GenerateKey();
                rjn.GenerateIV();

                mRGBKey = rjn.Key;
                mRGBIV = rjn.IV;
            }
            /// <summary>
            /// This is the constrcutor for the security holder.
            /// </summary>
            /// <param name="pass">The password.</param>
            /// <param name="salt">The salt.</param>
            public RijndaelSecHolder(byte[] pass, byte[] salt, int iterations)
            {
                Rfc2898DeriveBytes pwdGenSession =
                    new Rfc2898DeriveBytes(pass, salt, iterations);

                rjn = new RijndaelManaged();

                mRGBKey = pwdGenSession.GetBytes(32);
                mRGBIV = pwdGenSession.GetBytes(16);

            }
            #endregion // SecHolder(byte[] pass, byte[] salt)

            #region RGBKey
            /// <summary>
            /// The key.
            /// </summary>
            public byte[] RGBKey
            {
                get { return mRGBKey; }
            }
            #endregion // RGBKey
            #region RGBIV
            /// <summary>
            /// The initialization vector.
            /// </summary>
            public byte[] RGBIV
            {
                get { return mRGBIV; }
            }
            #endregion // RGBIV

            #region GetDecryptor()
            /// <summary>
            /// This method returns a new decryptor for the key.
            /// </summary>
            /// <returns>Returns the decryption object.</returns>
            public ICryptoTransform GetDecryptor()
            {
                return rjn.CreateDecryptor(mRGBKey, mRGBIV);
            }
            #endregion // GetDecryptor()
            #region GetEncryptor()
            /// <summary>
            /// This method returns a new encrytor for the key.
            /// </summary>
            /// <returns>Returns the encryption object.</returns>
            public ICryptoTransform GetEncryptor()
            {
                return rjn.CreateEncryptor(mRGBKey, mRGBIV);
            }
            #endregion // GetEncryptor()
        }
        #endregion // RijndaelSecHolder

        #region CookieSessionKey
        /// <summary>
        /// This is the cookie session key.
        /// </summary>
        protected RijndaelSecHolder CookieSessionKey
        {
            get
            {
                if (mCookieSessionKey != null)
                    return mCookieSessionKey;

                lock (syncKey)
                {
                    if (mCookieSessionKey != null)
                        return mCookieSessionKey;
                    
                    mCookieSessionKey = new RijndaelSecHolder();
                    return mCookieSessionKey;
                }
            }
        }
        #endregion // CookieSessionKey
        #region CookieMemberKey
        /// <summary>
        /// This is the member encryption key.
        /// </summary>
        protected RijndaelSecHolder CookieMemberKey
        {
            get
            {
                if (mCookieMemberKey != null)
                    return mCookieMemberKey;

                lock (syncKey)
                {
                    if (mCookieMemberKey != null)
                        return mCookieMemberKey;

                    mCookieMemberKey = new RijndaelSecHolder(this.ApplicationID.ToByteArray(), this.ApplicationID.ToByteArray(), 1000);
                    return mCookieMemberKey;
                }
            }
        }
        #endregion // CookieMemberKey

        #region Session Cookie Encryption methods
        /// <summary>
        /// This method encrypts the data with the session cookie key
        /// </summary>
        /// <param name="data">The data to encrypt.</param>
        /// <returns>Returns the base64 encrypted data.</returns>
        public string SessionCookieDecrypt(string data)
        {
            return Decrypt(data, CookieSessionKey);
        }
        /// <summary>
        /// This method decrypts the data with the session cookie key.
        /// </summary>
        /// <param name="data">The base64 encrypted data.</param>
        /// <returns>Returns the unencrypted data.</returns>
        public string SessionCookieEncrypt(string data)
        {
            return Encrypt(data, CookieSessionKey);
        }
        #endregion // Session Cookie Encryption methods
        #region Member Cookie Encryption methods
        /// <summary>
        /// This method encrypts the data with the member cookie key
        /// </summary>
        /// <param name="data">The data to encrypt.</param>
        /// <returns>Returns the base64 encrypted data.</returns>
        public string MemberCookieDecrypt(string data)
        {
            return Decrypt(data, CookieMemberKey);
        }
        /// <summary>
        /// This method decrypts the data with the member cookie key.
        /// </summary>
        /// <param name="data">The base64 encrypted data.</param>
        /// <returns>Returns the unencrypted data.</returns>
        public string MemberCookieEncrypt(string data)
        {
            return Encrypt(data, CookieMemberKey);
        }
        #endregion // Member Cookie Encryption methods

        #region Decrypt(string data, RijndaelSecHolder secHolder)
        /// <summary>
        /// This method decrypts the data.
        /// </summary>
        /// <param name="data">The data to decrpyt.</param>
        /// <param name="secHolder">The security key holder.</param>
        /// <returns>Returns the decrypted data.</returns>
        protected string Decrypt(string data, RijndaelSecHolder secHolder)
        {
            if (data == null || data == "")
                return null;

            try
            {
                byte[] bufRead;
                //if (data.StartsWith("H="))
                //    bufRead = CH.Enc_DecodeHexString(data.Substring(2));
                //else
                    bufRead = Convert.FromBase64String(data);

                using (ICryptoTransform eTransform = secHolder.GetDecryptor())
                {
                    using (MemoryStream ms = new MemoryStream(bufRead))
                    {
                        using (CryptoStream cs = new CryptoStream(ms, eTransform, CryptoStreamMode.Read))
                        {
                            using (BinaryReader br = new BinaryReader(cs, Encoding.UTF8))
                            {
                                int len = br.ReadInt32();
                                byte[] blob = br.ReadBytes(len);

                                return Encoding.UTF8.GetString(blob);
                            }
                        }
                    }
                }
            }
            catch (OutOfMemoryException oomex)
            {
                XimuraAppTrace.WriteLine("OutOfMemoryException: " + oomex.Message, "SiteControllerSettings/Decrypt", EventLogEntryType.Error);
                throw oomex;
            }
            catch (CryptographicException cex)
            {
                return null;
            }
            catch (Exception ex)
            {
                XimuraAppTrace.WriteLine("Unhandled exception: " + ex.Message, "SiteControllerSettings/Decrypt", EventLogEntryType.Warning);
                return null;
            }
        }
        #endregion // Decrypt(string data, RijndaelSecHolder secHolder)
        #region Encrypt(string data, RijndaelSecHolder secHolder)
        /// <summary>
        /// This method encrypts the data.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="secHolder">The security key holder</param>
        /// <returns>Returns the base64 encrypted data.</returns>
        protected string Encrypt(string data, RijndaelSecHolder secHolder)
        {
            using (ICryptoTransform eTransform = secHolder.GetEncryptor())
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, eTransform, CryptoStreamMode.Write))
                    {
                        using (BinaryWriter bw = new BinaryWriter(cs, Encoding.UTF8))
                        {
                            byte[] blob = Encoding.UTF8.GetBytes(data);
                            bw.Write(blob.Length);
                            bw.Write(blob);

                            bw.Flush();
                            cs.FlushFinalBlock();
                        }
                    }

                    ms.Flush();

                    string response = Convert.ToBase64String(ms.ToArray());
                    //string response = "H=" + CH.Enc_EncodeByteToHex(ms.ToArray());
                    return response;
                }
            }
        }
        #endregion // Encrypt(string data, RijndaelSecHolder secHolder)


    }
}
