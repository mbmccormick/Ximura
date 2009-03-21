#region using
using System;
using System.Runtime.Serialization;
using System.IO;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Xml;
using System.Xml.Schema;
using System.Xml.XPath;
using System.Xml.Xsl;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

using Ximura;
using Ximura.Data;
using Ximura.Helper;
using CH = Ximura.Helper.Common;
using Ximura.Server;
using Ximura.Command;
using Ximura.Communication;
#endregion // using
namespace Ximura.Communication
{
    /// <summary>
    /// This is the site context, which is used to hold the site requests.
    /// </summary>
    public class SiteControllerContext :
        JobContext<SiteControllerState, SiteControllerSettings, SiteControllerRequest, SiteControllerResponse, 
            SiteControllerConfiguration, SiteControllerPerformance>
    {
        #region Declarations
        private MappingSettings mMappingSettings;

        private ControllerRequest mControllerRequest = null;
        private ControllerSession mScriptSession = null;

        private DateTime? mTimeStamp;
        private bool mScriptRequestResolved;
        /// <summary>
        /// This is the default value for a HTTP/1.1 connection.
        /// </summary>
        private bool mConnectionClose = false;

        private string mDebug;
        #endregion
        #region Constructor
        /// <summary>
        /// This is the default context for the PPC.
        /// </summary>
        public SiteControllerContext()
            : base()
        {
        }
        #endregion // PurchaseContext

        #region Reset()
        /// <summary>
        /// This method resets the server context.
        /// </summary>
        public override void Reset()
        {
            if (mControllerRequest != null && mControllerRequest.ObjectPoolCanReturn)
                mControllerRequest.ObjectPoolReturn();
            mControllerRequest = null;

            //if (mScriptSession != null && mScriptSession.ObjectPoolCanReturn)
            //    mScriptSession.ObjectPoolReturn();
            mScriptSession = null;

            if (mMappingSettings != null && mMappingSettings.ObjectPoolCanReturn)
                mMappingSettings.ObjectPoolReturn();
            mMappingSettings = null;
            mScriptRequestResolved = false;
            mTimeStamp = null;
            mDebug = "";
            base.Reset();
        }
        #endregion // Reset()

        #region ScriptRequest
        /// <summary>
        /// This is the request cache object.
        /// </summary>
        public virtual ControllerRequest ScriptRequest
        {
            get { return mControllerRequest; }
        }
        #endregion // ScriptRequest
        #region ScriptRequestResolved
        /// <summary>
        /// This is the request cache object.
        /// </summary>
        public virtual bool ScriptRequestResolved
        {
            get { return mScriptRequestResolved; }
            set { mScriptRequestResolved = value; }
        }
        #endregion // ScriptRequest
        #region ScriptSettings
        /// <summary>
        /// This class contains the script settings for the request
        /// </summary>
        public MappingSettings ScriptSettings
        {
            get { return mMappingSettings; }
            set { mMappingSettings = value; }
        }
        #endregion // ScriptSettings
        #region ScriptSession
        /// <summary>
        /// This is the controller session request.
        /// </summary>
        public virtual ControllerSession ScriptSession
        {
            get { return mScriptSession; }
            set { mScriptSession = value; }
        }
        #endregion // ScriptSession

        #region Debug
        /// <summary>
        /// This value is set by the SiteController Command when an exception condition is hit.
        /// </summary>
        public virtual string Debug
        {
            get { return mDebug; }
            set { mDebug = value; }
        }
        #endregion

        #region Start()
        /// <summary>
        /// This method prepares the context for processing.
        /// </summary>
        public virtual void Start()
        {
            mTimeStamp = DateTime.Now;
            mControllerRequest = 
                ContextSettings.PoolManager.GetPoolManager(typeof(ControllerRequest)).Get() 
                    as ControllerRequest;

            if (mControllerRequest == null)
                throw new SiteControllerTooBusyException();


            mControllerRequest.ID = this.SignatureID.Value;
            mControllerRequest.Version = Guid.NewGuid();
        }
        #endregion // Start()

        #region FSM --> Initialize()            [Start]
        /// <summary>
        /// This method initializes the context.
        /// </summary>
        public void Initialize()
        {
            //OK, set the initial state.
            this.ChangeState();
            CurrentState.Initialize(this);
        }
        #endregion

        #region FSM --> MessageDecode()         [Protocol]
        /// <summary>
        /// This method initializes the context.
        /// </summary>
        public bool MessageDecode()
        {
            return CurrentState.MessageDecode(this);
        }
        #endregion
        #region FSM --> SessionResolve()        [Protocol]
        /// <summary>
        /// This method initializes the context.
        /// </summary>
        public bool SessionResolve()
        {
            return CurrentState.SessionResolve(this);
        }
        #endregion

        #region FSM --> RequestAuthenticate()   [Auth]
        /// <summary>
        /// This method authenticates the incoming request if an authentication scheme has been set in the script; otherwise this check is skipped.
        /// </summary>
        /// <returns>Returns true if the request has been authenticated.</returns>
        public bool RequestAuthenticate()
        {
            if (ScriptSettings.AuthState == null || ScriptSettings.AuthState == "")
                return true;

            CheckChangeState("AU_" + ScriptSettings.AuthState);
            return CurrentState.RequestAuthenticate(this);
        }
        #endregion // RequestAuthenticate()

        #region FSM --> RequestScriptAuthSet()  [Resolver]
        /// <summary>
        /// This method authenticates the incoming request if an authentication scheme has been set in the script; otherwise this check is skipped.
        /// </summary>
        /// <returns>Returns true if the request has been authenticated.</returns>
        public void RequestScriptAuthSet()
        {
            CheckChangeState("RV_" + ScriptSettings.ResolverState);
            CurrentState.RequestScriptAuthSet(this);
        }
        #endregion // RequestAuthenticate()
        #region FSM --> RequestResolve()        [Resolver]
        /// <summary>
        /// This method initializes the context.
        /// </summary>
        public bool RequestResolve()
        {
            CheckChangeState("RV_" + ScriptSettings.ResolverState);
            return CurrentState.RequestResolve(this);
        }
        #endregion

        #region FSM --> RequestProcess()        [Response]
        /// <summary>
        /// This method initializes the context.
        /// </summary>
        public bool RequestProcess()
        {
            return CurrentState.RequestProcess(this);
        }
        #endregion

        #region FSM --> ResponsePrepare()       [Protocol]
        /// <summary>
        /// This method initializes the context.
        /// </summary>
        public void ResponsePrepare()
        {
            CheckChangeState("PR_" + ScriptSettings.ProtocolState);
            CurrentState.ResponsePrepare(this);
        }
        #endregion
        #region FSM --> ResponseComplete()      [Protocol]
        /// <summary>
        /// This method initializes the context.
        /// </summary>
        public void ResponseComplete()
        {
            CurrentState.ResponseComplete(this);
        }
        #endregion

        #region FSM --> Log()                   [Protocol]
        /// <summary>
        /// This methog logs the current request.
        /// </summary>
        public void Log()
        {
            CurrentState.Log(this);
        }
        #endregion // Log()

        #region Finish()
        /// <summary>
        /// This method tidies up any processing data.
        /// </summary>
        public virtual void Finish()
        {
        }
        #endregion // Finish()

        #region ProtocolRequest
        /// <summary>
        /// This is the HTTP request
        /// </summary>
        public InternetMessage ProtocolRequest
        {
            get { return Data.ContractRequest.Message as InternetMessage; }
        }
        #endregion // ProtocolRequest
        #region ProtocolResponse
        /// <summary>
        /// This is the HTTP response.
        /// </summary>
        public InternetMessage ProtocolResponse
        {
            get { return Data.ContractResponse.Message as InternetMessage; }
            set { Data.ContractResponse.Message = value; }
        }
        #endregion // ProtocolResponse

        #region RequestURI
        /// <summary>
        /// This is the request uri
        /// </summary>
        public virtual Uri RequestURI
        {
            get { return Request.MessageUri; }
        }
        #endregion
        #region RequestUserAgent
        /// <summary>
        /// This is the request user agent
        /// </summary>
        public virtual string RequestUserAgent
        {
            get { return Request.MessageUserAgent; }
        }
        #endregion
        #region RequestServerType
        /// <summary>
        /// This is the request user agent
        /// </summary>
        public virtual string RequestServerType
        {
            get { return Request.ServerType; }
        }
        #endregion
        #region RequestMethod
        /// <summary>
        /// This is the request method
        /// </summary>
        public virtual string RequestMethod
        {
            get { return Request.MessageMethod; }
        }
        #endregion
        #region RequestMessage
        /// <summary>
        /// This is the request message
        /// </summary>
        public virtual Message RequestMessage
        {
            get { return Request.Message; }
        }
        #endregion
        #region RequestURILocal
        /// <summary>
        /// This is the request message
        /// </summary>
        public virtual Uri RequestURILocal
        {
            get { return Request.URILocal; }
        }
        #endregion        
        #region RequestURIRemote
        /// <summary>
        /// This is the request message
        /// </summary>
        public virtual Uri RequestURIRemote
        {
            get { return Request.URIRemote; }
        }
        #endregion

        #region ResponseMessage
        /// <summary>
        /// This is the request message
        /// </summary>
        public Message ResponseMessage
        {
            get { return Response.Message; }
            set { Response.Message = value; }
        }
        #endregion

        #region TimeElapsed
        /// <summary>
        /// This is the time that has elapsed since the request was received.
        /// </summary>
        public TimeSpan TimeElapsed
        {
            get
            {
                if (!mTimeStamp.HasValue)
                    return TimeSpan.Zero;

                return DateTime.Now - mTimeStamp.Value;
            }
        }
        #endregion // TimeElapsed

        #region ConnectionClose
        /// <summary>
        /// This method indicates whether the connection should be closed after the response has been sent.
        /// </summary>
        public bool ConnectionClose
        {
            get { return mConnectionClose; }
            set { mConnectionClose = value; }
        }
        #endregion // ConnectionClose

        #region RequestProcessMaxLoops
        /// <summary>
        /// This is the maximum premitted loops for the RequestProcess stage.
        /// </summary>
        public virtual int RequestProcessMaxLoops
        {
            get
            {
                return 10;
            }
        }
        #endregion // RequestProcessMaxLoops
    }
}
