#region using
using System;
using System.Runtime.Serialization;
using System.IO;
using System.Diagnostics;
using System.ComponentModel;

using Ximura;
using Ximura.Data;
using Ximura.Helper;
using CH = Ximura.Helper.Common;
using Ximura.Framework;
using Ximura.Framework;
using Ximura.Communication;
using AH = Ximura.Helper.AttributeHelper;
#endregion // using
namespace Ximura.Communication
{
    /// <summary>
    /// The Site Manager command is the primary command that processes a user's request, and
    /// returns the relevant content, based on the MVC pattern.
    /// </summary>
#if (DEBUG)
    [XimuraAppConfiguration(ConfigurationLocation.Hybrid,
       "xmrres://XimuraComm/Ximura.Communication.SiteControllerConfiguration/Ximura.Communication.Commands.SiteController.Configuration.SiteControllerConfiguration_Default.xml")]
#else
    [XimuraAppConfiguration(ConfigurationLocation.Hybrid,
       "xmrres://XimuraComm/Ximura.Communication.SiteControllerConfiguration/Ximura.Communication.Commands.SiteController.Configuration.SiteControllerConfiguration_Default.xml")]
#endif
    [XimuraAppModule("62A74574-7425-4CC2-B2F1-74EF38689725", "SiteControllerCommand")]
    public class SiteControllerCommand :
        FiniteStateMachine<SiteControllerRequest, SiteControllerResponse, RQRSFolder, RQRSFolder,
                SiteControllerContext, SiteControllerState, SiteControllerSettings,
                    SiteControllerConfiguration, SiteControllerPerformance>
    {
        #region Declarations
        protected SiteControllerRootScriptAttribute rootScriptAttr = null;

        private System.ComponentModel.IContainer components = null;

        protected StartState startState;
        protected ResolverState resolverState;

        protected ResponseResourceRetrieveState rs_ResourceState;
        protected ResponseViewBuildState rs_ViewBuild;
        protected ResponseHTTPRedirectState rs_HTTPRedirect;

        protected ProtocolHTTPState httpProtocolState;
        protected ProtocolEmailState emailProtocolState;
        protected ProtocolSMTPState smtpProtocolState;
        #endregion
        #region Constructors
        /// <summary>
        /// Empty constructor for use by the component model.
        /// </summary>
        public SiteControllerCommand() : this(null) { }

        /// <summary>
        /// Default constructor for use by the component model.
        /// </summary>
        /// <param name="container">The container that the transport should be added to.</param>
        public SiteControllerCommand(IContainer container)
            : base(container)
        {
            InitializeControllerAttributes();
            InitializeComponent();
            RegisterContainer(components);
        }
        #endregion

        #region InitializeControllerAttributes()
        /// <summary>
        /// This method initializes any attibutes for the command.
        /// </summary>
        protected virtual void InitializeControllerAttributes()
        {
            rootScriptAttr = AH.GetAttribute<SiteControllerRootScriptAttribute>(GetType());
        }
        #endregion // InitializeAttributes()
        #region InitializeComponent()
        /// <summary>
        /// This method initializes the components for the Site Manager.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            startState = new StartState(components);
            resolverState = new ResolverState(components);
            httpProtocolState = new ProtocolHTTPState(components);
            emailProtocolState = new ProtocolEmailState(components);
            smtpProtocolState = new ProtocolSMTPState(components);

            rs_ResourceState = new ResponseResourceRetrieveState(components);
            rs_ViewBuild = new ResponseViewBuildState(components);
            rs_HTTPRedirect = new ResponseHTTPRedirectState(components);

            ((System.ComponentModel.ISupportInitialize)(this.mStateExtender)).BeginInit();
            // 
            // startState
            // 
            this.mStateExtender.SetEnabled(this.startState, true);
            this.startState.Identifier = "Start";
            this.mStateExtender.SetNextStateID(this.startState, null);
            this.mStateExtender.SetStateID(this.startState, "Start");
            // 
            // resolverState
            // 
            this.mStateExtender.SetEnabled(this.resolverState, true);
            this.resolverState.Identifier = "RV_Resolver";
            this.mStateExtender.SetNextStateID(this.resolverState, null);
            this.mStateExtender.SetStateID(this.resolverState, "RV_Resolver");

            // 
            // smtpProtocolState
            // 
            this.mStateExtender.SetEnabled(this.smtpProtocolState, true);
            this.smtpProtocolState.Identifier = "PR_SMTPProtocol";
            this.mStateExtender.SetNextStateID(this.smtpProtocolState, null);
            this.mStateExtender.SetStateID(this.smtpProtocolState, "PR_SMTPProtocol");
            // 
            // emailProtocolState
            // 
            this.mStateExtender.SetEnabled(this.emailProtocolState, true);
            this.emailProtocolState.Identifier = "PR_EmailProtocol";
            this.mStateExtender.SetNextStateID(this.emailProtocolState, null);
            this.mStateExtender.SetStateID(this.emailProtocolState, "PR_EmailProtocol");
            // 
            // httpProtocolState
            // 
            this.mStateExtender.SetEnabled(this.httpProtocolState, true);
            this.httpProtocolState.Identifier = "PR_HttpProtocol";
            this.mStateExtender.SetNextStateID(this.httpProtocolState, null);
            this.mStateExtender.SetStateID(this.httpProtocolState, "PR_HttpProtocol");

            // 
            // rs_ResourceState
            // 
            this.mStateExtender.SetEnabled(this.rs_ResourceState, true);
            this.rs_ResourceState.Identifier = "RS_Resource";
            this.mStateExtender.SetNextStateID(this.rs_ResourceState, null);
            this.mStateExtender.SetStateID(this.rs_ResourceState, "RS_Resource");
            // 
            // rs_ViewBuild
            // 
            this.mStateExtender.SetEnabled(this.rs_ViewBuild, true);
            this.rs_ViewBuild.Identifier = "RS_ViewBuild";
            this.mStateExtender.SetNextStateID(this.rs_ViewBuild, null);
            this.mStateExtender.SetStateID(this.rs_ViewBuild, "RS_ViewBuild");
            // 
            // rs_HTTPRedirect
            // 
            this.mStateExtender.SetEnabled(this.rs_HTTPRedirect, true);
            this.rs_HTTPRedirect.Identifier = "RS_HTTPRedirect";
            this.mStateExtender.SetNextStateID(this.rs_HTTPRedirect, null);
            this.mStateExtender.SetStateID(this.rs_HTTPRedirect, "RS_HTTPRedirect");
            // 
            // State_Extender
            // 
            this.mStateExtender.InitialState = "Start";
            ((System.ComponentModel.ISupportInitialize)(this.mStateExtender)).EndInit();
        }
        #endregion

        #region MAIN ENTRY POINT --> ProcessRequest
        /// <summary>
        /// This method processing incoming HTTP requests from the HTTP Server Command.
        /// </summary>
        /// <param name="context">The request context.</param>
        protected override void ProcessRequest(SiteControllerContext context)
        {
            try
            {
                //This method sets the initial conditions to process the incoming request.
                context.Start();

                //Initialize the request, this parses the incoming request
                //and extracts the necessary parameters to process it.
                context.Initialize();                               //State => Start

                //Decrypt the cookie, or create a new cookie.
                if (!context.MessageDecode())                       //State => Protocol
                {
                    context.Debug = "MessageDecode fail.";
                    return;
                }

                //Get the session handler, or create a new session handler.
                if (!context.SessionResolve())                      //State => Protocol
                {
                    context.Debug = "SessionResolve fail.";
                    return;
                }

                //This method authenticates the incoming request.
                if (!context.RequestAuthenticate())                 //State => Auth
                {
                    context.Debug = "RequestAuthenticate fail.";
                    return;
                }

                //This method sets any specific session permissions 
                //in to the script request. 
                context.RequestScriptAuthSet();                     //State => Resolver

                //This method resolves the incoming request 
                //and sets the initial state to process the request.
                if (!context.RequestResolve())                      //State => Resolver
                {
                    context.Debug = "RequestResolve fail.";
                    return;
                }

                int maxLoops = context.RequestProcessMaxLoops;
                //Resolve the state to process the request, i.e. FormHandlers -> ViewBuild etc
                while (maxLoops > 0 && context.RequestProcess())
                    maxLoops--;
            }
            catch (SiteControllerException smex)
            {
                ProcessRequestFailed(smex, context.Job, context.Data);
                context.ScriptRequest.ResponseStatus = CH.HTTPCodes.InternalServerError_500;
                context.ScriptRequest.ResponseStatusMessage = "Internal Server Error";
                XimuraAppTrace.WriteLine(smex.Message, "SiteManagerCommand-->500", EventLogEntryType.Error);
                context.Debug = smex.Message;
            }
            catch (InvalidStateNameFSMException isex)
            {
                context.ScriptRequest.ResponseStatus = CH.HTTPCodes.NotImplemented_501;
                context.ScriptRequest.ResponseStatusMessage = "Not implemented";
                XimuraAppTrace.WriteLine("Invalid state name: " + isex.Message, "SiteManagerCommand-->500", EventLogEntryType.Error);
                context.Debug = isex.Message;
            }
            catch (NotImplementedException niex)
            {
                context.ScriptRequest.ResponseStatus = CH.HTTPCodes.NotImplemented_501;
                context.ScriptRequest.ResponseStatusMessage = "Not implemented";
                XimuraAppTrace.WriteLine(niex.Message, "SiteManagerCommand-->501", EventLogEntryType.Error);
                context.Debug = "Not implemented";
            }
            catch (Exception ex)
            {
                context.ScriptRequest.ResponseStatus = CH.HTTPCodes.InternalServerError_500;
                context.ScriptRequest.ResponseStatusMessage = "Internal Server Error";
                XimuraAppTrace.WriteLine(ex.Message, "SiteManagerCommand-->500", EventLogEntryType.Error);
                context.Debug = ex.Message;
            }
            finally
            {
                try
                {
                    //Encrypt the cookie, if required.
                    context.ResponsePrepare();                      //State => Protocol
                    //This method completes the response 
                    //and adds any default headers.
                    context.ResponseComplete();                     //State => Protocol
                }
                catch (Exception ex)
                {
                    XimuraAppTrace.WriteLine(ex.Message, "SiteManagerCommand final-->500", EventLogEntryType.Error);
                    context.ScriptRequest.ResponseStatus = CH.HTTPCodes.InternalServerError_500;
                    context.ScriptRequest.ResponseStatusMessage = "Internal Server Error";
                }

                try
                {
                    //OK, log the request. We do not want to affect any 
                    //of the previous processing if this throws an error.
                    context.Log();                                  //State => Protocol
                }
                finally
                {
                    //OK, finish up. Finish should ensure that no exceptions are thrown.
                    context.Finish();
                }
            }
        }
        #endregion // ProcessRequest

        #region ContextInitialize
        protected override void ContextInitialize(SiteControllerContext newContext, 
            SecurityManagerJob job, RQRSContract<SiteControllerRequest, SiteControllerResponse> Data)
        {
            //Reset the context with the job and the data.
            newContext.Reset(ContextConnection, job, Data, mRemoteContextPoolAccess);
        }
        #endregion // ContextInitialize

        #region ProcessRequestFailed
        protected virtual void ProcessRequestFailed(SiteControllerException smex, SecurityManagerJob job, RQRSContract<SiteControllerRequest, SiteControllerResponse> Data)
        {
            ProcessRequestFailed(smex.ResponseCode.ToString(), smex.Message, job, Data);
        }
        /// <summary>
        /// This override should place the appropriate HTML error response in the response object.
        /// </summary>
        /// <param name="status">The status code.</param>
        /// <param name="subStatus">The status description.</param>
        /// <param name="job">The job.</param>
        /// <param name="Data">The job data.</param>
        protected override void ProcessRequestFailed(string status, string subStatus, SecurityManagerJob job, RQRSContract<SiteControllerRequest, SiteControllerResponse> Data)
        {
            XimuraAppTrace.WriteLine(status + " - " + subStatus, this.CommandName, EventLogEntryType.Warning);
            base.ProcessRequestFailed(status, subStatus, job, Data);
        }
        #endregion // ProcessRequestFailed

        #region InternalStart/InternalStop
        /// <summary>
        /// This is the overriden start method.
        /// </summary>
        protected override void InternalStart()
        {
            base.InternalStart();
            InitializeControllerScripts();
        }
        /// <summary>
        /// This is the overriden stop method.
        /// </summary>
        protected override void InternalStop()
        {
            base.InternalStop();
        }
        #endregion // InternalStart/InternalStop

        #region InitializeControllerScripts()
        /// <summary>
        /// This method is used the intilize the controller scripts. Controller scripts match the incomong request to the correct 
        /// states for processing and suthentication.
        /// </summary>
        protected virtual void InitializeControllerScripts()
        {
            if (rootScriptAttr!=null)
                ContextConnection.InitializeScripts(rootScriptAttr.RootScript, null);
        }
        #endregion // InitializeControllerScripts()
    }
}
