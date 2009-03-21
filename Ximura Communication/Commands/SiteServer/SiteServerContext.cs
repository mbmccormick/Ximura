#region using
using System;
using System.Runtime.Serialization;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.ComponentModel;
using System.Linq;
using System.Collections.Generic;
using System.Net.Mail;

using Ximura;
using Ximura.Data;
using Ximura.Helper;
using CH = Ximura.Helper.Common;
using Ximura.Server;
using Ximura.Command;
using Ximura.Communication;
using AH = Ximura.Helper.AttributeHelper;
#endregion // using
namespace Ximura.Communication
{
    public class SiteServerContext<RQ, RS> :
        JobContext
        <
            SiteServerState<RQ, RS>,
            SiteServerSettings<SiteServerState<RQ,RS>>,
            RQ, RS,
            SiteServerConfiguration, SiteServerPerformance
        >
        where RQ : SiteServerRQ, new()
        where RS : SiteServerRS, new()
    {
        #region Declarations
        private int? mConnectionTimeoutInSeconds;
        #endregion // Declarations
        #region Constructor
        /// <summary>
        /// This is the default constructor
        /// </summary>
        public SiteServerContext():base()
        {
        }
        #endregion // Constructor

        #region Reset()
        /// <summary>
        /// This method resets the context to its default state.
        /// </summary>
        public override void Reset()
        {
            HA1 = null;
            UserID = null;

            mConnectionTimeoutInSeconds = null;
            DefaultRequestMessageType = typeof(RequestMessage); 
            InitialMessageType = null;
            DefaultRequestMaxLength = 20000;

            ExpiryTime = null;
            ClosePending = false;

            ListenerLocation = null;
            ListenerConnectionLimit = null;

            ProtocolCommandID = null;
            ProtocolContextID = null;
            ProtocolUri = null;

            URILocal = null;
            URIRemote = null;

            SiteControllerSessionID = null;
            SiteControllerServerID = null;
            SiteControllerRealm = null;
            Authenticated = false;

            base.Reset();
        }
        #endregion // Reset()

        #region ProcessSession
        /// <summary>
        /// This method sets the context session to the base command session. This can be used for protocols 
        /// that do not use specific security sessions for each user, but just require a single session.
        /// </summary>
        public IXimuraSessionRQ ProcessSession
        {
            get
            {
                return ContextSettings.ProcessSession as IXimuraSessionRQ;
            }
        }
        #endregion // UseProcessSession()

        #region SiteControllerSessionID
        /// <summary>
        /// This is the session ID from the Site Controller.
        /// </summary>
        public Guid? SiteControllerSessionID
        {
            get;
            set;
        }
        #endregion // SiteControllerSessionID
        #region SiteControllerServerID
        /// <summary>
        /// This is the session ID from the Site Controller.
        /// </summary>
        public string SiteControllerServerID
        {
            get;
            set;
        }
        #endregion // SiteControllerSessionID
        #region SiteControllerRealm
        /// <summary>
        /// This is the realm for the account.
        /// </summary>
        public string SiteControllerRealm
        {
            get;
            set;
        }
        #endregion // SiteControllerRealm
        #region Authenticated
        /// <summary>
        /// This property identifies whether the context is authenticated.
        /// </summary>
        public virtual bool Authenticated
        {
            get;
            set;
        }
        #endregion // Authenticated
        #region UserID
        /// <summary>
        /// This is the username of the current connection.
        /// </summary>
        public MailAddress UserID
        {
            get;
            set;
        }
        #endregion // Username

        #region HA1
        /// <summary>
        /// This is the authroization data.
        /// </summary>
        public string HA1
        {
            get;
            set;
        }
        #endregion // AuthorizationData
        #region HA1Set(string data)
        /// <summary>
        /// This method sets the HA1 value.
        /// </summary>
        /// <param name="data">The HA1 data</param>
        public void HA1Set(string data)
        {
            if (data == null)
                throw new ArgumentNullException("data cannot be null");
            if (UserID == null)
                throw new ArgumentNullException("UserID is null");

            HA1 = CH.HA1Calculate("md5", UserID.User, UserID.Host, data);
        }
        #endregion // HA1Set(string data)

        #region URILocal
        /// <summary>
        /// The local URI
        /// </summary>
        public Uri URILocal
        {
            get;
            set;
        }
        #endregion // URILocal
        #region URIRemote
        /// <summary>
        /// The remote URI
        /// </summary>
        public Uri URIRemote
        {
            get;
            set;
        }
        #endregion // URIRemote

        #region IN --> Listen(Uri location)
        /// <summary>
        /// This method initializes the context.
        /// </summary>
        public bool Listen(Uri location)
        {
            //OK, set the initial state.
            return CurrentState.Listen(this, location);
        }
        #endregion
        #region IN --> ListenConfirm(Guid ProtocolContextID, TransportConnectionType ConnectionType)
        /// <summary>
        /// This method confirms the listening connection.
        /// </summary>
        /// <param name="ProtocolContextID">this is the protocol context id.</param>
        /// <param name="ConnectionType">This is the connection type.</param>
        /// <returns>Returns true if the listen confirmation was successful.</returns>
        public bool ListenConfirm(IXimuraRQRSEnvelope Env)
        {
            return CurrentState.ListenConfirm(this, Env);
        }
        #endregion // ListenConfirm
        #region ListenerConnectionLimit
        /// <summary>
        /// This property defines the limit to the number of connections allowed on the remote listener. Set
        /// this value to null if you do not require a limit.
        /// </summary>
        public int? ListenerConnectionLimit { get; protected set; }
        #endregion // ListenerConnectionLimit

        #region IN --> Initialize()
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

        #region IN --> ConnectionRequest(SecurityManagerJob job, RQRSContract<RQCallbackServer, RSCallbackServer> Data)
        /// <summary>
        /// This method processes the incoming connection request.
        /// </summary>
        /// <param name="job">The current job request.</param>
        /// <returns>Returns true of the session should be reset.</returns>
        public bool ConnectionRequest(SecurityManagerJob job, RQRSContract<RQCallbackServer, RSCallbackServer> Data)
        {
            return CurrentState.ConnectionRequest(this, job, Data);
        }
        #endregion // ConnectionRequest(SecurityManagerJob job, RQRSContract<RQCallbackServer, RSCallbackServer> Data)
        #region IN --> Receive(SecurityManagerJob job, RQRSContract<RQCallbackServer, RSCallbackServer> Data)
        public bool Receive(SecurityManagerJob job, RQRSContract<RQCallbackServer, RSCallbackServer> Data)
        {
            return CurrentState.Receive(this, job, Data);
        }
        #endregion // Receive(SecurityManagerJob job, RQRSContract<RQCallbackServer, RSCallbackServer> Data)
        #region IN --> Transmit
        public void Transmit(IXimuraMessageStream message, bool SignalClose)
        {
            //Transmit(ProtocolConnectionDefault, message, SignalClose);
        }

        public void Transmit(ProtocolConnectionIdentifiers identifier, IXimuraMessageStream messageOut, bool SignalClose)
        {
            CurrentState.Transmit(this, identifier, messageOut, SignalClose);
        }
        #endregion // Transmit

        #region IN --> Close()/Close(SecurityManagerJob job, RQRSContract<RQCallbackServer, RSCallbackServer> Data)
        public void Close()
        {
            CurrentState.Close(this);
        }

        public bool Close(SecurityManagerJob job, RQRSContract<RQCallbackServer, RSCallbackServer> Data)
        {
            return CurrentState.Close(this, job, Data);
        }
        #endregion

        #region RQRSContract<RQ, RS> GetContract(SecurityManagerJob job)
        /// <summary>
        /// The contract for the summary.
        /// </summary>
        /// <param name="job"></param>
        /// <returns></returns>
        protected RQRSContract<RQ, RS> GetContract(SecurityManagerJob job)
        {
            return job.Data as RQRSContract<RQ, RS>;
        }
        #endregion // RQRSContract<RQ, RS> GetContract(SecurityManagerJob job)
        #region RQRSContract<RQCallbackServer, RSCallbackServerS> GetContractCallback(SecurityManagerJob job)
        /// <summary>
        /// The contract for the summary.
        /// </summary>
        /// <param name="job"></param>
        /// <returns></returns>
        protected RQRSContract<RQCallbackServer, RSCallbackServer> GetContractCallback(SecurityManagerJob job)
        {
            return job.Data as RQRSContract<RQCallbackServer, RSCallbackServer>;
        }
        #endregion // RQRSContract<RQ, RS> GetContract(SecurityManagerJob job)

        #region VerifyIncomingUri(Uri location)
        /// <summary>
        /// The default is to accept all incoming connections. You can override this to provide more specific support.
        /// </summary>
        /// <param name="location">The connection location.</param>
        /// <returns>Returns true by default.</returns>
        public virtual bool VerifyIncomingUri(Uri location)
        {
            return true;
        }
        #endregion

        #region ListenerSchemeSet
        /// <summary>
        /// This method resolves the scheme and sets the protocol command ID for the context.
        /// </summary>
        /// <param name="listenOn"></param>
        /// <returns></returns>
        public bool ListenerSchemeSet(Uri listenOn)
        {
            Guid? commandID = ContextSettings.Configuration.ResolveScheme(listenOn.Scheme);

            if (commandID.HasValue)
            {
                ProtocolCommandID = commandID.Value;
                ListenerLocation = listenOn;
                return true;
            }

            return false;
        }
        #endregion // ListenerSchemeSet
        #region ListenerLocation
        /// <summary>
        /// THis is the listener location. This is null if the context is not listening.
        /// </summary>
        public Uri ListenerLocation { get; protected set; }
        #endregion // ListenerLocation

        #region ProtocolCommandID
        /// <summary>
        /// This is the remote ID of the protocol command.
        /// </summary>
        public virtual Guid? ProtocolCommandID
        {
            get;
            set;
        }
        #endregion
        #region ProtocolContextID
        /// <summary>
        /// This is the identifier of the remte protocol context.
        /// </summary>
        public virtual Guid? ProtocolContextID
        {
            get;
            set;
        }
        #endregion
        #region ProtocolUri
        /// <summary>
        /// This is the protocol uri.
        /// </summary>
        public Uri ProtocolUri
        {
            get;
            set;
        }
        #endregion // ProtocolUri

        #region Protocol Addresses
        #region ProtocolListenRequestAddress
        /// <summary>
        /// This is the protocol listen request address.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public EnvelopeAddress ProtocolListenRequestAddress{get{return new EnvelopeAddress(ProtocolCommandID.Value, "ListenRequest");}}
        #endregion
        #region ProtocolListenConfirmAddress
        /// <summary>
        /// This is the protocol listen request address.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public EnvelopeAddress ProtocolListenConfirmAddress { get { return new EnvelopeAddress(ProtocolCommandID.Value, "ListenConfirm"); } }
        #endregion

        #region ProtocolPassiveOpenRequestAddress
        /// <summary>
        /// This is the protocol connection open request.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public EnvelopeAddress ProtocolPassiveOpenRequestAddress { get { return new EnvelopeAddress(ProtocolCommandID.Value, "OpenPassive"); } }
        #endregion
        #region ProtocolActiveOpenRequestAddress
        /// <summary>
        /// This is the protocol connection open request.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public EnvelopeAddress ProtocolActiveOpenRequestAddress { get { return new EnvelopeAddress(ProtocolCommandID.Value, "OpenActive"); } }
        #endregion
        #region ProtocolCloseRequestAddress
        /// <summary>
        /// This is the protocol close request.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public EnvelopeAddress ProtocolCloseRequestAddress { get { return new EnvelopeAddress(ProtocolCommandID.Value, "Close"); } }
        #endregion
        #region ProtocolTransmitRequestAddress
        /// <summary>
        /// This is the protocol close request.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public EnvelopeAddress ProtocolTransmitRequestAddress { get { return new EnvelopeAddress(ProtocolCommandID.Value, "Transmit"); } }
        #endregion
        #endregion

        #region ServerConnectionRequestAddress
        /// <summary>
        /// This is the address of this command. This value is passed as a parameter to
        /// the protocol when initiating a listening connection to allow the protocol to
        /// call back the server when new connection requests are received.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public EnvelopeAddress ServerConnectionRequestAddress { get { return new EnvelopeAddress(ContextSettings.CommandID, "ConnRQ"); } }
        #endregion

        #region DefaultRequestMessageType
        /// <summary>
        /// This is the default request message type.
        /// </summary>
        public virtual Type DefaultRequestMessageType
        {
            get;
            set;
        }
        #endregion // DefaultRequestMessageType
        #region DefaultRequestMaxLength
        /// <summary>
        /// This is the default maximum length of the response message in bytes.
        /// </summary>
        public virtual int DefaultRequestMaxLength
        {
            get;
            set;
        }
        #endregion // DefaultRequestMaxLength

        #region InitialMessageType(Listener listener)
        /// <summary>
        /// This method returns the initial message type when receiving connection.
        /// </summary>
        /// <param name="Listener">The listener for the connection.</param>
        /// <returns>Returns the object type, or null if there is no type specified.</returns>
        public virtual Type InitialMessageType
        {
            get;
            set;
        }
        #endregion // Type InitialMessageType(Listener listener)

        #region FormatSCRequest
        /// <summary>
        /// This command provides an easy way to format a request to SiteControllerCommand
        /// </summary>
        /// <param name="context">The current context.</param>
        /// <param name="method">The request method.</param>
        /// <param name="location">The request location.</param>
        /// <param name="message">The message object.</param>
        /// <param name="action">Any additional action.</param>
        /// <returns>Returns the envelpoe containing the request data.</returns>
        public virtual RQRSContract<SiteControllerRequest, SiteControllerResponse> FormatSCRequest(
            string method, Uri location, Message message, string serverType,
                Action<RQRSContract<SiteControllerRequest, SiteControllerResponse>> action)
        {
            RQRSContract<SiteControllerRequest, SiteControllerResponse> Env =
                RQRSEnvelopeHelper.Get(ContextSettings.Configuration.SiteControllerID) as RQRSContract<SiteControllerRequest, SiteControllerResponse>;

            SenderIdentitySet((IXimuraRQRSEnvelope)Env);
            Env.DestinationAddress = new EnvelopeAddress(ContextSettings.Configuration.SiteControllerID, "Receive");

            Env.ContractRequest.MessageMethod = method;
            Env.ContractRequest.Message = message;
            Env.ContractRequest.MessageUri = location;
            Env.ContractRequest.ServerType = serverType;

            if (action != null)
                action(Env);

            return Env;
        }
        #endregion // FormatSCRequest
        #region FormatSCRequestMessage
        /// <summary>
        /// This command provides an easy way to format a request to SiteControllerCommand
        /// </summary>
        /// <param name="context">The current context.</param>
        /// <param name="method">The request method.</param>
        /// <param name="location">The request location.</param>
        /// <param name="message">The message object.</param>
        /// <param name="action">Any additional action.</param>
        /// <returns>Returns the envelope containing the request data.</returns>
        public virtual RQRSContract<SiteControllerRequest, SiteControllerResponse> FormatSCRequestMessage(
            string method, Uri location, string serverType, 
                Action<Message> actionMessage,
                Action<RQRSContract<SiteControllerRequest, SiteControllerResponse>> actionEnv)
        {
            RQRSContract<SiteControllerRequest, SiteControllerResponse> Env =
                RQRSEnvelopeHelper.Get(ContextSettings.Configuration.SiteControllerID) as RQRSContract<SiteControllerRequest, SiteControllerResponse>;

            InternetMessageRequest message = GetObjectPool<InternetMessageRequest>().Get();

            message.BeginInit();

            message.Instruction.Verb = method;
            message.Instruction.Instruction = location.PathAndQuery;
            message.Instruction.Protocol = location.Scheme.ToUpperInvariant();
            message.Instruction.Version = "1.0";


            message.HeaderAdd("SessionID", SiteControllerSessionID.ToString());
            message.HeaderAdd("Host", location.Host);

            //if (SecurityToken() != null)
            //    message.HeaderAdd("Authorization", "Basic " + SecurityToken());

            SenderIdentitySet((IXimuraRQRSEnvelope)Env);
            Env.DestinationAddress = new EnvelopeAddress(ContextSettings.Configuration.SiteControllerID, "Receive");

            Env.ContractRequest.MessageMethod = method;
            Env.ContractRequest.Message = message;
            Env.ContractRequest.MessageUri = location;
            Env.ContractRequest.ServerType = serverType;

            if (actionEnv != null)
                actionEnv(Env);

            if (actionMessage != null)
                actionMessage(message);

            message.EndInit();

            return Env;
        }
        #endregion // FormatSCRequest

        #region ExpiryTime
        /// <summary>
        /// This is the expiry time for the connection. Null signifies that the context never expires.
        /// </summary>
        public DateTime? ExpiryTime
        {
            get;
            set;
        }
        #endregion // ExpiryTime
        #region ExpiryTimeSet()
        /// <summary>
        /// This method sets the expiry time for the context.
        /// </summary>
        public virtual void ExpiryTimeSet()
        {
            ExpiryTime = DateTime.Now.AddSeconds(ConnectionTimeoutInSeconds); 
        }
        #endregion // ExpiryTimeSet()

        #region ClosePending
        /// <summary>
        /// This property specifies that the close is pending.
        /// </summary>
        public bool ClosePending
        {
            get;
            set;
        }
        #endregion // ClosePending
        #region ConnectionTimeoutInSeconds
        /// <summary>
        /// This is the permitted inactive time for a connection.
        /// </summary>
        public int ConnectionTimeoutInSeconds
        {
            get
            {
                if (!mConnectionTimeoutInSeconds.HasValue)
                    mConnectionTimeoutInSeconds = ContextSettings.Configuration.ConnectionTimeoutInSeconds;

                return mConnectionTimeoutInSeconds.HasValue?mConnectionTimeoutInSeconds.Value:30;
            }
        }
        #endregion // ConnectionTimeoutInSeconds

    }
}
