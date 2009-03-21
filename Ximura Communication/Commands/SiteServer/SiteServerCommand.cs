#region using
using System;
using System.Runtime.Serialization;
using System.IO;
using System.Diagnostics;
using System.ComponentModel;
using System.Linq;
using System.Collections.Generic;

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
    /// <summary>
    /// The site server command is used to provide protocol support for Site Controller connected protocols.
    /// </summary>
    public class SiteServerCommand : SiteServerCommand<SiteServerRQ, SiteServerRS>
    {
        #region Constructors
        /// <summary>
        /// Empty constructor for use by the component model.
        /// </summary>
        public SiteServerCommand() : this(null) { }
        /// <summary>
        /// Default constructor for use by the component model.
        /// </summary>
        /// <param name="container">The container that the transport should be added to.</param>
        public SiteServerCommand(System.ComponentModel.IContainer container)
            : base(container)
        {
        }
        #endregion
    }
    /// <summary>
    /// The site server command is used to provide protocol support for Site Controller connected protocols.
    /// </summary>
    /// <typeparam name="RQ">The server protocol request type.</typeparam>
    /// <typeparam name="RS">The server protocol response type.</typeparam>
    public class SiteServerCommand<RQ, RS> :
        FiniteStateMachine<
            RQ, RS, 
            RQCallbackServer, RSCallbackServer, 
            SiteServerContext<RQ, RS>, 
            SiteServerState<RQ, RS>, 
            SiteServerSettings<SiteServerState<RQ, RS>>, 
            SiteServerConfiguration, SiteServerPerformance
        >
        where RQ : SiteServerRQ, new()
        where RS : SiteServerRS, new()
    {
        #region Declarations
        private System.ComponentModel.IContainer components = null;

        private object syncContextObject = new object();

        /// <summary>
        /// This is trackID for debugging the command instance.
        /// </summary>
        private readonly Guid mTrackID = Guid.NewGuid();
        /// <summary>
        /// This extender is responsible for managing the message resolvers.
        /// </summary>
        protected MessageResolverExtender MessageResolver_Extender;
        /// <summary>
        /// This collection holds the listeners active on the protocol.
        /// </summary>
        private List<SiteServerContext<RQ, RS>> mListeners;
        /// <summary>
        /// This is a list of the active protocol contexts listed by their unique identifier.
        /// </summary>
        private Dictionary<Guid, SiteServerContext<RQ, RS>> mContexts;

        /// <summary>
        /// The connection start state.
        /// </summary>
        protected StartSiteServerState<RQ, RS> startState;
        /// <summary>
        /// The connectionful listen state.
        /// </summary>
        protected ListenConnectionfulSSState<RQ, RS> listenCfulState;
        /// <summary>
        /// The connectionless listen state.
        /// </summary>
        protected ListenConnectionlessSSState<RQ, RS> listenClessState;
        /// <summary>
        /// The connection close state.
        /// </summary>
        protected CloseSiteServerState<RQ, RS> closeState;
        #endregion // Declarations
        #region Constructors
        /// <summary>
        /// Empty constructor for use by the component model.
        /// </summary>
        public SiteServerCommand() : this(null) { }
        /// <summary>
        /// Default constructor for use by the component model.
        /// </summary>
        /// <param name="container">The container that the transport should be added to.</param>
        public SiteServerCommand(System.ComponentModel.IContainer container)
            : base(container)
        {
            mListeners = new List<SiteServerContext<RQ, RS>>();
            mContexts = new Dictionary<Guid, SiteServerContext<RQ, RS>>();

            InitializeComponent();
            RegisterContainer(components);
        }
        #endregion

        #region InternalStart/InternalStop/InternalPause/InternalContinue
        /// <summary>
        /// This overriden method starts the protocols
        /// </summary>
        protected override void InternalStart()
        {
            base.InternalStart();

            ComponentsStatusChange(XimuraServiceStatusAction.Start, ServiceComponents, typeof(AuthHandler));

            CallbackRegister();

            ListenersStart();

            ProtocolStatusChange(XimuraServiceStatusAction.Start);
        }
        /// <summary>
        /// This overriden method stops the protocols
        /// </summary>
        protected override void InternalStop()
        {
            ProtocolStatusChange(XimuraServiceStatusAction.Stop);

            ContextsClose();

            ListenersStop();

            CallbackUnregister();

            ComponentsStatusChange(XimuraServiceStatusAction.Stop, ServiceComponents, typeof(AuthHandler));

            base.InternalStop();
        }
        /// <summary>
        /// This overriden method pauses stops the RBP protocol
        /// </summary>
        protected override void InternalPause()
        {
            ProtocolStatusChange(XimuraServiceStatusAction.Pause);

            base.InternalPause();
        }
        /// <summary>
        /// This overriden resumes the protocols after they have been paused.
        /// </summary>
        protected override void InternalContinue()
        {
            base.InternalContinue();

            ProtocolStatusChange(XimuraServiceStatusAction.Continue);
        }
        /// <summary>
        /// This method is used to announce detailed announcements on protocol
        /// service changes.
        /// </summary>
        /// <param name="status">The protocol status, i.e. start, stop etc.</param>
        protected virtual void ProtocolStatusChange(XimuraServiceStatusAction status)
        {
            switch (status)
            {
                case XimuraServiceStatusAction.Start:
                    XimuraAppTrace.WriteLine(CommandName + " started.",
                        this.CommandName, EventLogEntryType.Information);
                    break;
                case XimuraServiceStatusAction.Stop:
                    XimuraAppTrace.WriteLine(CommandName + " stopped.",
                        this.CommandName, EventLogEntryType.Information);
                    break;
                case XimuraServiceStatusAction.Pause:
                    XimuraAppTrace.WriteLine(CommandName + " paused.",
                        this.CommandName, EventLogEntryType.Information);
                    break;
                case XimuraServiceStatusAction.Continue:
                    XimuraAppTrace.WriteLine(CommandName + " resumed.",
                        this.CommandName, EventLogEntryType.Information);
                    break;
            }
        }
        #endregion

        #region InitializeComponent()
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();

            startState = new StartSiteServerState<RQ, RS>(components);
            closeState = new CloseSiteServerState<RQ, RS>(components);

            listenCfulState = new ListenConnectionfulSSState<RQ, RS>(components);
            listenClessState = new ListenConnectionlessSSState<RQ, RS>(components);

            ((System.ComponentModel.ISupportInitialize)(this.MessageResolver_Extender)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mStateExtender)).BeginInit();

            // 
            // startState
            // 
            this.mStateExtender.SetEnabled(this.startState, true);
            this.mStateExtender.SetNextStateID(this.startState, null);
            this.mStateExtender.SetStateID(this.startState, "Start");
            // 
            // closeState
            // 
            this.mStateExtender.SetEnabled(this.closeState, true);
            this.mStateExtender.SetNextStateID(this.closeState, null);
            this.mStateExtender.SetStateID(this.closeState, "Close");

            // 
            // listenCfulState
            // 
            this.mStateExtender.SetEnabled(this.listenCfulState, true);
            this.mStateExtender.SetNextStateID(this.listenCfulState, null);
            this.mStateExtender.SetStateID(this.listenCfulState, "ListenConnectionful");
            // 
            // listenClessState
            // 
            this.mStateExtender.SetEnabled(this.listenClessState, true);
            this.mStateExtender.SetNextStateID(this.listenClessState, null);
            this.mStateExtender.SetStateID(this.listenClessState, "ListenConnectionless");

            // 
            // MessageResolver_Extender
            // 
            this.MessageResolver_Extender.Enabled = false;
            // 
            // State_Extender
            // 
            this.mStateExtender.InitialState = "Start";

            ((System.ComponentModel.ISupportInitialize)(this.MessageResolver_Extender)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mStateExtender)).EndInit();
        }
        #endregion
        #region InitializeExtenders()
        /// <summary>
        /// This method can be overriden to add additional extenders to the
        /// protocol, or to remove existing extenders.
        /// </summary>
        protected override void InitializeExtenders(IContainer baseContainer)
        {
            base.InitializeExtenders(baseContainer);
            MessageResolver_Extender = new MessageResolverExtender(baseContainer);
        }
        #endregion // InitializeExtenders()

        #region ListenersStart()
        /// <summary>
        /// This method is called to start any transport listeners.
        /// </summary>
        protected virtual void ListenersStart()
        {
            //Process each listener
            foreach (Uri listenOn in Configuration.Listeners)
            {
                SiteServerContext<RQ, RS> listener = null;
                bool success = false;
                try
                {
                    listener = ContextGet();
                    success = listener.Listen(listenOn);
                }
                catch (Exception ex)
                {
                    success = false;
                }

                if (!success && listener != null)
                {
                    listener.Close();
                    ContextReturn(listener);
                    continue;
                }

                mListeners.Add(listener);
            }
        }
        #endregion
        #region ListenersStop()
        /// <summary>
        /// This method is called to stop and transport listeners
        /// </summary>
        protected virtual void ListenersStop()
        {
            foreach (SiteServerContext<RQ, RS> listener in mListeners)
            {
                listener.Close();
                ContextReturn(listener);
            }

            mListeners.Clear();
        }
        #endregion // ListenersStop

        #region ContextsClose()
        /// <summary>
        /// This method is called when the close request is received from the application.
        /// </summary>
        protected virtual void ContextsClose()
        {
            //if (mContexts == null || mContexts.Count == 0)
            //    return;

            //foreach (CNTX context in mContexts.Values)
            //    context.Close();

            //mContexts.Clear();

        }
        #endregion // ContextsClose()

        #region MAIN ENTRY POINT HERE --> ProcessRequest
        /// <summary>
        /// This method processes
        /// </summary>
        /// <param name="job"></param>
        /// <param name="Data"></param>
        protected override void ProcessRequest(SecurityManagerJob job, RQRSContract<RQ, RS> Data)
        {
            base.ProcessRequest(job, Data);
        }
        #endregion

        #region MAIN CALL BACK POINT --> ProcessCallback(SecurityManagerJob job, RQRSContract<CBRQ, CBRS> Data)
        /// <summary>
        /// This method processes the callbacks from the Protocol commands.
        /// </summary>
        /// <param name="job">The job.</param>
        /// <param name="Data">The request data.</param>
        protected override void ProcessCallback(SecurityManagerJob job, RQRSContract<RQCallbackServer, RSCallbackServer> Data)
        {
            RQCallbackServer Request = Data.ContractRequest;
            RSCallbackServer Response = Data.ContractResponse;
            string subC = Data.DestinationAddress.SubCommand as string;
            SiteServerContext<RQ, RS> context = null;
            //Retrieve the listening location
            Guid? connID = Request.ServerContextID;

            //If the connection ID is not set, try and resolve the context or get a new context.
            if (!connID.HasValue)
            {
                switch (subC)
                {
                    case "Receive":
                        //If this is a stateless message, check whether we have the message resolver enabled.
                        //If not, we must quit as there is no way to resolve the correct context for the message.
                        if (!MessageResolver_Extender.Enabled)
                        {
                            Data.Response.Status = CH.HTTPCodes.BadRequest_400;
                            return;
                        }
                        connID = MessageResolver_Extender.Resolve(Request.Message);
                        //OK, this message needs a new context.
                        if (!connID.HasValue)
                        {
                            context = ContextGet();
                            context.Initialize();
                        }
                        break;

                    case "Close":
                        Data.Response.Status = CH.HTTPCodes.Accepted_202;
                        return;

                    default:
                        context = ContextGet();
                        context.Initialize();
                        break;
                }
            }
            else
                context = ContextResolve(connID.Value);

            //We should have a context by now, if not, there is something wrong.
            if (context == null)
            {
                Data.Response.Status = CH.HTTPCodes.BadRequest_400;
                return;
            }

            Response.Status = CH.HTTPCodes.Continue_100;
            bool resetContext = true;
            //OK, let's process the request.
            try
            {
                switch (subC)
                {
                    case "ConnRQ":
                        resetContext = context.ConnectionRequest(job, Data);
                        break;
                    case "Receive":
                        resetContext = context.Receive(job, Data);
                        break;
                    case "Close":
                        resetContext = context.Close(job, Data);
                        break;
                    default:
                        throw new ServerCallbackException("Unknown subcommand: " + subC, CH.HTTPCodes.InternalServerError_500, "");
                }

                //if (!resetContext)
                //    Response.ContextID = context.SignatureID;
            }
            catch (ServerCallbackException vsdex)
            {
                XimuraAppTrace.WriteLine("Unexpected protocol error: " + vsdex.Message + Environment.NewLine + vsdex.ToString(),
                    this.CommandName + " -> ProcessRequest", EventLogEntryType.Error, this.CommandName);
                Response.Status = vsdex.Status;
                Response.Substatus = "(" + vsdex.SubStatus + ") " + vsdex.Message + Environment.NewLine + vsdex.ToString();
            }
            catch (Exception ex)
            {
                XimuraAppTrace.WriteLine("Unexpected protocol error: " + ex.ToString(),
                    this.CommandName + " -> ProcessRequest", EventLogEntryType.Error, this.CommandName);
                Response.Status = CH.HTTPCodes.InternalServerError_500;
                Response.Substatus = ex.Message;
            }
            finally
            {
                if (resetContext && context != null)
                    ContextReturn(context);
            }
        }
        #endregion // ProcessCallback(SecurityManagerJob job, RQRSContract<CBRQ, CBRS> Data)

        #region ContextGet()
        protected override SiteServerContext<RQ, RS> ContextGetContext()
        {
            return ContextGet();
        }

        /// <summary>
        /// This method gets a new protocol context and set its security to run under the process session of the command.
        /// </summary>
        /// <returns>The new context object.</returns>
        protected virtual SiteServerContext<RQ, RS> ContextGet()
        {
            try
            {
                lock (syncContextObject)
                {
                    SiteServerContext<RQ, RS> context = ContextPool.Get();
                    ContextReset(context);
                    mContexts.Add(context.SignatureID.Value, context);

#if DEBUG
                    Debug.WriteLine(
                        String.Format(
                        "Context Pool Get ({0}) current pool count = {1}/{2}/{3} [{4}] -> {5}/{6}"
                        , CommandName
                        , ContextPool.Available
                        , mContexts.Count
                        , ContextPool.Count
                        , ContextPool.Stats
                        , context.SignatureID.Value.ToString()
                        , mTrackID.ToString()
                        ));
#endif
                    return context;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion // ContextGet()
        #region ContextReturn(CNTX context)
        protected override void ContextReturnContext(SiteServerContext<RQ, RS> context)
        {
            ContextReturn(context);
        }
        /// <summary>
        /// This method unregisters a context and returns it to the pool.
        /// </summary>
        protected virtual void ContextReturn(SiteServerContext<RQ, RS> context)
        {
            try
            {
                lock (syncContextObject)
                {
                    //Check whether the object has already been returned. This may happen during error
                    //conditions.
                    if (!mContexts.ContainsKey(context.SignatureID.Value))
                        return;

                    mContexts.Remove(context.SignatureID.Value);
                    ContextPool.Return(context);
#if DEBUG
                    //Debug.WriteLine("Context Pool Return (" + CommandName + ") current pool count after return = "
                    //    + mContexts.Count.ToString() + "/" + ContextPool.Count.ToString() + " [" + context.TrackID.ToString() + "]");
                    Debug.WriteLine(
                        String.Format(
                        "Context Pool Return ({0}) current pool count = {1}/{2}/{3} [{4}] -> {5}/{6}"
                        , CommandName
                        , ContextPool.Available
                        , mContexts.Count
                        , ContextPool.Count
                        , ContextPool.Stats
                        , context.SignatureID.Value.ToString()
                        , mTrackID.ToString()
                        ));
#endif
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion // ContextReturn(CNTX context)
        #region ContextReset(SiteServerContext<RQ, RS> context)
        /// <summary>
        /// This method resets the session with the settings and a new empty session.
        /// </summary>
        /// <param name="context"></param>
        protected virtual void ContextReset(SiteServerContext<RQ, RS> context)
        {
            //IXimuraSession newSession = this.sessionMan.createSession(CommandProcessSettings.UserSessionRealm, null);
            //context.Reset(ContextConnection, newSession);

            context.Reset(ContextConnection, null, mRemoteContextPoolAccess);
        }
        #endregion // ContextReset(CNTX context)
        #region ContextResolve(Guid connID)
        /// <summary>
        /// This method resolves an existing connection from the incoming request.
        /// </summary>
        /// <param name="job">The job to resolve the connection ID></param>
        /// <returns>Returns the protocol context or null if the context cannot be resolved.</returns>
        protected virtual SiteServerContext<RQ, RS> ContextResolve(Guid connID)
        {
            lock (syncContextObject)
            {
                if (!mContexts.ContainsKey(connID))
                    return null;

                return mContexts[connID];
            }
        }
        #endregion // ContextResolve(Guid connID)

        #region ServiceParentSettingsSet(IXimuraService service)
        /// <summary>
        /// This method is called when a component inmplements the IXimuraServiceParentSettings interface.
        // You should override it and set any service specific values from the parent component.
        /// </summary>
        /// <param name="service">The service.</param>
        protected override void ServiceParentSettingsSet(IXimuraServiceParentSettings service)
        {
            if (this.ParentCommandName != null && this.ParentCommandName != "")
                service.ParentCommandName = this.ParentCommandName + '/' + this.CommandName;
            else
                service.ParentCommandName = this.CommandName;
        }
        #endregion // ServiceParentSettingsSet(IXimuraService service)

        #region OnTimerEvent()
        /// <summary>
        /// This timer method closes the expired connections.
        /// </summary>
        /// <param name="state">The timer state.</param>
        protected override void OnTimerEvent(bool autoStart)
        {
            if (!autoStart) TimerPause();
            try
            {
                //OK, close any expired contexts.
                List<SiteServerContext<RQ, RS>> expired = null;
                lock (syncContextObject)
                {
                    DateTime now = DateTime.Now;
                    expired = mContexts.Values
                        .Where(cx => !cx.ClosePending && cx.ExpiryTime != null && cx.ExpiryTime < now)
                        .ToList();
                }

                //We move this in to a queue as close operations will modify the underlying mContexts collection.
                foreach (var item in expired)
                    item.Close();

                //Now return to the pool and closed contexts.
                List<SiteServerContext<RQ, RS>> closed = null;
                lock (syncContextObject)
                {
                    closed = mContexts.Values
                        .Where(cx => cx.ClosePending)
                        .ToList();
                }

                foreach (var item in closed)
                    ContextReturn(item);
            }
            catch (Exception ex)
            {
                //Must catch all errors as this code executes on a different thread and could crash the application if not handled.
                XimuraAppTrace.WriteLine(ex.Message, "SiteServerCommand/OnTimerEvent", EventLogEntryType.Warning);
            }
            //Resume the timer.
            if (!autoStart) TimerResume();
        }
        #endregion // OnTimerEvent(object state)

        #region CallbackRegister()
        /// <summary>
        /// This method sets the protocol command as a permitted callback.
        /// </summary>
        protected override void CallbackRegister()
        {
            base.CallbackRegister();

            foreach(Guid transportID in Configuration.TransportCommandIDs)
                CallbackRegister(transportID);
        }
        #endregion // CallbackRegister()
    }
}
