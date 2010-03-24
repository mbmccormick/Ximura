#region Copyright
// *******************************************************************************
// Copyright (c) 2000-2009 Paul Stancer.
// All rights reserved. This program and the accompanying materials
// are made available under the terms of the Eclipse Public License v1.0
// which accompanies this distribution, and is available at
// http://www.eclipse.org/legal/epl-v10.html
//
// Contributors:
//     Paul Stancer - initial implementation
// *******************************************************************************
#endregion
#region using
using System;
using System.Linq;
using System.Threading;
using System.Timers;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Reflection;
using System.Drawing;
using System.Diagnostics;

using Ximura;
using Ximura.Data;
using CH = Ximura.Helper.Common;
using Ximura.Framework;

using Ximura.Framework;

#endregion // using
namespace Ximura.Framework
{
    #region FiniteStateMachine<RQ, RS, CBRQ, CBRS, CNTX, ST, SET, CONF, PERF>
    /// <summary>
    /// The FiniteStateMachine is one of the root command objects. 
    /// It is used to implement system that operate using a well defined system of states.
    /// </summary>
    /// <typeparam name="RQ">The request type.</typeparam>
    /// <typeparam name="RS">The response type.</typeparam>
    /// <typeparam name="CBRQ">The callback request type.</typeparam>
    /// <typeparam name="CBRS">The callback response type.</typeparam>
    /// <typeparam name="CNTX">The FSM context type.</typeparam>
    /// <typeparam name="ST">The FSM base state type.</typeparam>
    /// <typeparam name="SET">The FSM base settings.</typeparam>
    public class FiniteStateMachine<RQ, RS, CBRQ, CBRS, CNTX, ST, SET, CONF, PERF> :
        FiniteStateMachine<RQ, RS, CBRQ, CBRS, CNTX, ST, SET, CONF, PERF, CONF>
        where RQ : RQRSFolder, new()
        where RS : RQRSFolder, new()
        where CBRQ : RQRSFolder, new()
        where CBRS : RQRSFolder, new()
        where CNTX : class, IXimuraFSMContext, new()
        where ST : class, IXimuraFSMState
        where SET : class, IXimuraFSMSettings<ST, CONF, PERF>, new()
        where CONF : FSMCommandConfiguration, new()
        where PERF : FSMCommandPerformance, new()
    {
        #region Constructors
        /// <summary>
        /// Empty constructor for use by the component model.
        /// </summary>
        public FiniteStateMachine() : this(null) { }
        /// <summary>
        /// Default constructor for use by the component model.
        /// </summary>
        /// <param name="container">The container that the transport should be added to.</param>
        public FiniteStateMachine(System.ComponentModel.IContainer container)
            : base(container)
        {
        }

        /// <summary>
        /// This is the base constructor for a Ximura command
        /// </summary>
        /// <param name="commandID">This is the explicitly set command id, leave this as null if you want to use the default id.</param>
        /// <param name="container">The container to be added to</param>
        public FiniteStateMachine(Guid? commandID, System.ComponentModel.IContainer container)
            : base(commandID, container)
        {
        }
        #endregion
    }
    #endregion
    #region FiniteStateMachine<RQ, RS, CBRQ, CBRS, CNTX, ST, SET, CONF, PERF, EXTCONF>
    /// <summary>
    /// The FiniteStateMachine is one of the root command objects. 
    /// It is used to implement system that operate using a well defined system of states.
    /// </summary>
    /// <typeparam name="RQ">The request type.</typeparam>
    /// <typeparam name="RS">The response type.</typeparam>
    /// <typeparam name="CBRQ">The callback request type.</typeparam>
    /// <typeparam name="CBRS">The callback response type.</typeparam>
    /// <typeparam name="CNTX">The FSM context type.</typeparam>
    /// <typeparam name="ST">The FSM base state type.</typeparam>
    /// <typeparam name="SET">The FSM base settings.</typeparam>
    /// <typeparam name="EXTCONF">The external configuration object type which contains the settings for the internal commands.</typeparam>
    public class FiniteStateMachine<RQ, RS, CBRQ, CBRS, CNTX, ST, SET, CONF, PERF, EXTCONF> :
        AppCommandProcess<RQ, RS, CBRQ, CBRS, CONF, PERF, EXTCONF>
        where RQ : RQRSFolder, new()
        where RS : RQRSFolder, new() 
        where CBRQ : RQRSFolder, new()
        where CBRS : RQRSFolder, new()
        where CNTX : class, IXimuraFSMContext, new()
        where ST : class,IXimuraFSMState
        where SET : class, IXimuraFSMSettings<ST, CONF, PERF>, new()
        where CONF : FSMCommandConfiguration, new()
        where PERF : FSMCommandPerformance, new()
        where EXTCONF : CommandConfiguration, new()
    {
        #region Declarations
        private bool mExternalStatesAllow = true;
        private bool mStateExtenderRemove = false;

        private System.ComponentModel.IContainer components;

        /// <summary>
        /// This is the state extender service.
        /// </summary>
        protected IXimuraStateExtenderService mExtenderService = null;
        /// <summary>
        /// This is the default throttle limit for the context completion job.
        /// </summary>
        protected const int DEFAULTTHROTTLELIMIT = 10;
        /// <summary>
        /// This is the context pool.
        /// </summary>
        protected ContextPool<CNTX> mPool = null;
        /// <summary>
        /// This is the context connection class that hold context connection settings and services.
        /// </summary>
        protected SET mContextSettings = null;
        /// <summary>
        /// This is the base state extender.
        /// </summary>
        protected StateExtender<ST> mStateExtender = null;
        /// <summary>
        /// This class allows contexts to create additional contexts. This is primarily used for 
        /// connection based FSM services.
        /// </summary>
        protected FSMContextPoolAccess<CNTX> mRemoteContextPoolAccess;
        #endregion // Declarations
        #region Constructors
        /// <summary>
        /// Empty constructor for use by the component model.
        /// </summary>
        public FiniteStateMachine() : this(null) { }
        /// <summary>
        /// Default constructor for use by the component model.
        /// </summary>
        /// <param name="container">The container that the transport should be added to.</param>
        public FiniteStateMachine(System.ComponentModel.IContainer container)
            : base(container)
        {
            InitializeComponent();
            InitializeExtenders();
            InitializeBaseStates();
            RegisterContainer(components);
        }

        /// <summary>
        /// This is the base constructor for a Ximura command
        /// </summary>
        /// <param name="commandID">This is the explicitly set command id, leave this as null if you want to use the default id.</param>
        /// <param name="container">The container to be added to</param>
        public FiniteStateMachine(Guid? commandID, System.ComponentModel.IContainer container)
            :
        base(commandID, container)
        {
            InitializeComponent();
            InitializeExtenders();
            InitializeBaseStates();
            RegisterContainer(components);
        }

        #endregion

        #region InitializeComponent()
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
        }
        #endregion // InitializeComponent()
        #region InitializeExtenders()
        private void InitializeExtenders()
        {
            InitializeExtenders(this.components);
        }
        /// <summary>
        /// This method can be overriden to add additional extenders to the
        /// protocol, or to remove existing extenders.
        /// </summary>
        /// <param name="baseContainer">This is the base container in the FSM.</param>
        protected virtual void InitializeExtenders(IContainer baseContainer)
        {
            mStateExtender = new StateExtender<ST>(baseContainer);
        }
        #endregion // InitializeExtenders()
        #region InitializeBaseStates()
        private void InitializeBaseStates()
        {
            InitializeBaseStates(this.components);
        }
        /// <summary>
        /// This method can be overriden to add additional extenders to the
        /// protocol, or to remove existing extenders.
        /// </summary>
        /// <param name="baseContainer">This is the base container in the FSM.</param>
        protected virtual void InitializeBaseStates(IContainer baseContainer)
        {
        }
        #endregion // InitializeExtenders()

        #region InternalStart/InternalStop
        /// <summary>
        /// This method initializes the context pool.
        /// </summary>
        protected override void InternalStart()
        {
            base.InternalStart();
            InitializeContextConnection();
            InitializeContextConnectionSettings();
            InitializeContextPool();
            InitializeContextPoolAccess();
            StatesStart();
        }
        /// <summary>
        /// This
        /// </summary>
        protected override void InternalStop()
        {
            StatesStop();
            ClearContextPool();
            base.InternalStop();
        }
        #endregion // InternalStart/InternalStop

        #region StatesStart()
        /// <summary>
        /// This method will start any states that implement IXimuraService
        /// </summary>
        protected virtual void StatesStart()
        {
            mStateExtender.Start();
        }
        #endregion // StatesStart()
        #region StatesStop()
        /// <summary>
        /// This method will stop any states that have been started.
        /// </summary>
        protected virtual void StatesStop()
        {
            mStateExtender.Stop();
        }
        #endregion // StatesStop()

        #region ServicesProvide()
        /// <summary>
        /// This override creates the state extender service.
        /// </summary>
        protected override void ServicesProvide()
        {
            base.ServicesProvide();

            if (ExternalStatesAllow)
            {
                mExtenderService = GetService<IXimuraStateExtenderService>();

                if (mExtenderService == null)
                {
                    mExtenderService = new StateExtenderService();

                    AddService<IXimuraStateExtenderService>(mExtenderService, true);

                    mStateExtenderRemove = true;
                }

                mExtenderService.Register(CommandID, typeof(ST), mStateExtender);
            }
        }
        #endregion // ServicesProvide()
        #region ServicesRemove()
        /// <summary>
        /// This override removes the state extender service.
        /// </summary>
        protected override void ServicesRemove()
        {
            if (ExternalStatesAllow && mExtenderService!=null)
            {
                mExtenderService.Unregister(CommandID, typeof(ST));
            }

            if (mStateExtenderRemove)
                RemoveService<IXimuraStateExtenderService>(true);

            base.ServicesRemove();
        }
        #endregion // ServicesRemove()

        #region CreateSystemRequest
        /// <summary>
        /// This method creates the system request.
        /// </summary>
        /// <returns>A new envelope with the correct destination address.</returns>
        protected IXimuraRQRSEnvelope CreateSystemRequest(EnvelopeAddress address)
        {
            IXimuraRQRSEnvelope Env = CommandBridge.EnvelopeHelper.Get(address.command);
            Env.Request.ID = Guid.NewGuid();
            Env.DestinationAddress = address;
            return Env;
        }
        #endregion // CreateSystemRequest

        #region FSMConfig
        ///// <summary>
        ///// This is the finite state machine configuration object.
        ///// </summary>
        //[Browsable(false)]
        //[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        //public virtual IXimuraFSMConfigSH FSMConfig
        //{
        //    get
        //    {
        //        return CommandSettings as IXimuraFSMConfigSH;
        //    }
        //}
        #endregion // FSMSettings

        #region ProcessRequest(SecurityManagerJob job, RQRSContract<RQ, RS> Data)
        /// <summary>
        /// This override is the main entry point for the incoming FSM request.
        /// </summary>
        /// <param name="job">The job.</param>
        /// <param name="Data">The request data.</param>
        protected override void ProcessRequest(SecurityManagerJob job, RQRSContract<RQ, RS> Data)
        {
			//Check whether the service is actually running, and if not quit.
			if (this.ServiceStatus != XimuraServiceStatus.Started)
			{
                ProcessRequestFailed(CH.HTTPCodes.ServiceUnavailable_503, "Service unavailable", job, Data);
				return;
			}

            Data.ContractResponse.Status = CH.HTTPCodes.Continue_100;
            CNTX currentContext = null;

            try
            {
                //Get the context
                currentContext = ContextPool.Get();

                if (currentContext == null)
                {
                    //Check whether there is a custom code to handle this situation.
                    ProcessRequestFailed(CH.HTTPCodes.InternalServerError_500, "13 - Too Busy", job, Data);
                    return;
                }
                //Initialize the context with the current information.
                ContextInitialize(currentContext, job, Data);

                if (GatekeeperEnabled)
                    GatekeeperProcessRequest(currentContext);
                else
                    //Process the internal logic
                    ProcessRequest(currentContext);
            }
            catch (Exception ex)
            {
                ProcessRequestFailed(CH.HTTPCodes.InternalServerError_500, "Unhandled error: " + ex.Message, job, Data);
                XimuraAppTrace.WriteLine("FSM Process Request Error: " + ex.ToString(), 
                    CommandName, EventLogEntryType.Error);
            }
            finally
            {
                if (currentContext != null && 
                    job.JobCompletionType != SCMJobCompletionType.ManualSignal)
                {
                    if (GatekeeperEnabled)
                        GatekeeperRelease(currentContext);

                    ContextPool.Return(currentContext);
                }
            }
        }
        #endregion // ProcessRequest(SecurityManagerJob job)
        #region ProcessRequestFailed(string status, SecurityManagerJob job, RQRSContract<RQ, RS> Data)
        /// <summary>
        /// This method allow customization to the response should there not be any contexts available 
        /// to process the request.
        /// </summary>
        /// <param name="job">The job.</param>
        /// <param name="Data">The data containing the request and the response.</param>
        /// <returns>Return true if you do not wish the command to set the standard response properties to the response.</returns>
        protected virtual void ProcessRequestFailed(string status, string subStatus, SecurityManagerJob job, RQRSContract<RQ, RS> Data)
        {
            //The server is too busy and has no contexts to service the request.
            Data.ContractResponse.Status = status;
            Data.ContractResponse.Substatus = subStatus;
        } 
        #endregion
        #region ProcessRequest(CNTX jobContext)
        /// <summary>
        /// This is the entry point for Finite State Machines that require a context.
        /// </summary>
        /// <param name="jobContext">The job context that contains the requested parameters.</param>
        protected virtual void ProcessRequest(CNTX jobContext)
        {
            //This method should be overriden to provide specific FSM logic.
            throw new NotImplementedException("ProcessRequest is not implemented.");
        }
        #endregion

        #region InitializeContextPool()
        private void InitializeContextPool()
        {
            InitializeContextPool(this.components);
        }
        /// <summary>
        /// This method initializes the context pool. Override this method if you need
        /// additional pool creation logic.
        /// </summary>
        protected virtual void InitializeContextPool(IContainer baseContainer)
        {
            if (mPool == null)
                mPool = new ContextPool<CNTX>(Configuration.PoolMin, Configuration.PoolMax, Configuration.PoolPrefer);
        }
        #endregion // InitializeContextPool()
        #region InitializeContextConnection()
        private void InitializeContextConnection()
        {
            InitializeContextConnection(this.components);
        }
        /// <summary>
        /// This method should be overriden to provide extended Context collection settings.
        /// </summary>
        /// <param name="baseContainer">The base container for the collection.</param>
        protected virtual void InitializeContextConnection(IContainer baseContainer)
        {
            //mContextConnection = new ContextSettings<ST>(mStateExtender) as SET;
            mContextSettings = new SET();
            mContextSettings.InitializeSettings(CommandDefinition,ApplicationDefinition,
                mStateExtender, PoolManager, EnvelopeHelper, mSessionMan, mProcessSession, Configuration, Performance);
        }
        #endregion // InitializeContextCollection()
        #region InitializeContextConnectionSettings()
        private void InitializeContextConnectionSettings()
        {
            InitializeContextConnectionSettings(this.components);
        }
        /// <summary>
        /// This method should be overriden to provide extended Context collection settings.
        /// </summary>
        /// <param name="baseContainer">The base container for the collection.</param>
        protected virtual void InitializeContextConnectionSettings(IContainer baseContainer)
        {
            //mContextSettings.UpdateSettings(FSMConfig,CDSSettings);
        }
        #endregion // InitializeContextCollection()
        #region ContextPool
        /// <summary>
        /// This is the context pool for the FSM.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        protected ContextPool<CNTX> ContextPool
        {
            get
            {
                return mPool;
            }
        }
        #endregion // ContextPool
        #region ContextConnection
        /// <summary>
        /// This is the default context connection.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        protected virtual SET ContextConnection
        {
            get
            {
                return mContextSettings;
            }
        }
        #endregion // ContextConnection
        #region ContextInitialize(CNTX newContext, SecurityManagerJob job, RQRSContract<RQ, RS> Data)
        /// <summary>
        /// This method allows generic operations to be performed on the new context.
        /// </summary>
        /// <param name="newContext"></param>
        /// <param name="job">The incoming job.</param>
        /// <param name="Data">The incoming generic data.</param>
        protected virtual void ContextInitialize(CNTX newContext, SecurityManagerJob job, RQRSContract<RQ, RS> Data)
        {
            //Set the context state
            //currentContext.Reset(this as IXimuraConnectionToFSM<CNTX, ST>, job);
            newContext.Reset(ContextConnection, job, mRemoteContextPoolAccess);
        }
        #endregion // protected virtual void ResetContext(CNTX newContext, SecurityManagerJob job, RQRSContract<RQ, RS> Data)
        #region ClearContextPool()
        /// <summary>
        /// This method empties the context pool.
        /// </summary>
        public virtual void ClearContextPool()
        {
            //TODO
        }
        #endregion // ClearContextPool()

        #region Gatekeeper methods
        protected virtual void GatekeeperProcessRequest(CNTX jobContext)
        {
            ProcessRequest(jobContext);
        }

        protected virtual void GatekeeperRelease(CNTX jobContext)
        {

        }
        #endregion // Gatekeeper methods
        #region GatekeeperEnabled
        /// <summary>
        /// This protected property determines whether the context gatekeeper is enabled. If you wish 
        /// to enable the gatekeeper, you should override this property and return true.
        /// </summary>
        protected virtual bool GatekeeperEnabled
        {
            get
            {
                return false;
            }
        }
        #endregion // GatekeeperEnabled

        #region InitializeContextPoolAccess()
        /// <summary>
        /// This command initializes the context pool.
        /// </summary>
        protected virtual void InitializeContextPoolAccess()
        {
            mRemoteContextPoolAccess = 
                new FSMContextPoolAccess<CNTX>(ContextGetContext,ContextReturnContext);
        }
        #endregion // InitializeContextPoolAccess()
        #region ContextGetContext()
        /// <summary>
        /// This method retrieves a context from the pool.
        /// </summary>
        /// <returns></returns>
        protected virtual CNTX ContextGetContext()
        {
            return null;
        }
        #endregion // ContextGetContext()
        #region ContextReturnContext(CNTX context)
        /// <summary>
        /// This method returns a context to the pool.
        /// </summary>
        /// <param name="context"></param>
        protected virtual void ContextReturnContext(CNTX context)
        {

        }
        #endregion // ContextReturnContext(CNTX context)

        #region ExternalStatesAllow
        /// <summary>
        /// This property determines whether the FSM will allow external states contained in a FSMExtenderCommand object to register
        /// with this command.
        /// </summary>
        [Category("Command Settings")]
        [DefaultValue(false)]
        [Description("This property determines whether the FSM will allow external states contained in a FSMExtenderCommand object to register.")]
        public virtual bool ExternalStatesAllow
        {
            get { return mExternalStatesAllow; }
            set { mExternalStatesAllow = value; }
        }
        #endregion // ExternalStatesAllow

    }
    #endregion

}