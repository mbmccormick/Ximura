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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;

using Ximura;
using Ximura.Data;
using Ximura.Helper;
using CH=Ximura.Helper.Common;

using Ximura.Framework;
 
 
#endregion // using
namespace Ximura.Framework
{
    #region AppCommandStandard
    /// <summary>
    /// AppCommandStandard is the base command object that all standard commands should inherit from.
    /// </summary>
    public class AppCommandStandard : AppCommandStandard<RQRSFolder, RQRSFolder, CommandConfiguration>
    {
        #region Constructors
        /// <summary>
        /// This is the empty constructor
        /// </summary>
        public AppCommandStandard() : this((IContainer)null) { }
        /// <summary>
        /// This is the constrcutor used by the Ximura Application model.
        /// </summary>
        /// <param name="container">The command container.</param>
        public AppCommandStandard(System.ComponentModel.IContainer container)
            : base(container)
        {
        }
        /// <summary>
        /// This is the base constructor for a Ximura command
        /// </summary>
        /// <param name="commandID">This is the explicitly set command id, leave this as null if you want to use the default id.</param>
        /// <param name="container">The container to be added to</param>
        public AppCommandStandard(Guid? commandID, System.ComponentModel.IContainer container) : base(commandID, container) { }
        #endregion
    }
    #endregion // AppCommandStandard
    #region AppCommandStandard<RQ, RS>
    /// <summary>
    /// AppCommandStandard is the base command object that all standard commands should inherit from.
    /// </summary>
    /// <typeparam name="RQ">The main request RQRSFolder type.</typeparam>
    /// <typeparam name="RS">The main response RQRSFolder type.</typeparam>
    public class AppCommandStandard<RQ, RS> : AppCommandStandard<RQ, RS, RQRSFolder, RQRSFolder, CommandConfiguration, CommandPerformance>
        where RS : RQRSFolder, new()
        where RQ : RQRSFolder, new()
    {
        #region Constructors
        /// <summary>
        /// This is the empty constructor
        /// </summary>
        public AppCommandStandard() : this((IContainer)null) { }
        /// <summary>
        /// This is the constrcutor used by the Ximura Application model.
        /// </summary>
        /// <param name="container">The command container.</param>
        public AppCommandStandard(System.ComponentModel.IContainer container)
            : base(container)
        {
        }
        /// <summary>
        /// This is the base constructor for a Ximura command
        /// </summary>
        /// <param name="commandID">This is the explicitly set command id, leave this as null if you want to use the default id.</param>
        /// <param name="container">The container to be added to</param>
        public AppCommandStandard(Guid? commandID, System.ComponentModel.IContainer container) : base(commandID, container) { }
        #endregion
    }
    #endregion // AppCommandStandard<RQ, RS>
    #region AppCommandStandard<RQ, RS, CONF>
    /// <summary>
    /// AppCommandStandard is the base command object that all standard commands should inherit from.
    /// </summary>
    /// <typeparam name="RQ">The main request RQRSFolder type.</typeparam>
    /// <typeparam name="RS">The main response RQRSFolder type.</typeparam>
    /// <typeparam name="CONF">The command configuration object.</typeparam>
    public class AppCommandStandard<RQ, RS, CONF> : AppCommandStandard<RQ, RS, RQRSFolder, RQRSFolder, CONF, CommandPerformance>
        where RS : RQRSFolder, new()
        where RQ : RQRSFolder, new()
        where CONF : CommandConfiguration, new()
    {
        #region Constructors
        /// <summary>
        /// This is the empty constructor
        /// </summary>
        public AppCommandStandard() : this((IContainer)null) { }
        /// <summary>
        /// This is the constrcutor used by the Ximura Application model.
        /// </summary>
        /// <param name="container">The command container.</param>
        public AppCommandStandard(System.ComponentModel.IContainer container)
            : base(container)
        {
        }
        /// <summary>
        /// This is the base constructor for a Ximura command
        /// </summary>
        /// <param name="commandID">This is the explicitly set command id, leave this as null if you want to use the default id.</param>
        /// <param name="container">The container to be added to</param>
        public AppCommandStandard(Guid? commandID, System.ComponentModel.IContainer container) : base(commandID, container) { }
        #endregion
    }
    #endregion // AppCommandStandard<RQ, RS, CONF>

    #region AppCommandStandard<RQ, RS, CBRQ, CBRS, CONF, PERF>
    /// <summary>
    /// AppCommandStandard is the base command object that all standard commands should inherit from.
    /// </summary>
    /// <typeparam name="RQ">The main request RQRSFolder type.</typeparam>
    /// <typeparam name="RS">The main response RQRSFolder type.</typeparam>
    /// <typeparam name="CBRQ">The callback request RQRSFolder type.</typeparam>
    /// <typeparam name="CBRS">The callback response RQRSFolder type.</typeparam>
    /// <typeparam name="CONF">The command configuration object.</typeparam>
    /// <typeparam name="PERF">The command performance monitor object.</typeparam>
    public class AppCommandStandard<RQ, RS, CBRQ, CBRS, CONF, PERF> : AppCommandStandard<RQ, RS, CBRQ, CBRS, CONF, PERF, CONF>
        where RQ : RQRSFolder, new() //Request
        where RS : RQRSFolder, new() //Response
        where CBRQ : RQRSFolder, new() //Callback Request
        where CBRS : RQRSFolder, new() //Callback Response
        where CONF : CommandConfiguration, new()
        where PERF : CommandPerformance, new()
    {
        #region Constructors
        /// <summary>
        /// This is the empty constructor
        /// </summary>
        public AppCommandStandard() : this((IContainer)null) { }
        /// <summary>
        /// This is the constrcutor used by the Ximura Application model.
        /// </summary>
        /// <param name="container">The command container.</param>
        public AppCommandStandard(System.ComponentModel.IContainer container)
            : base(container)
        {
        }
        /// <summary>
        /// This is the base constructor for a Ximura command
        /// </summary>
        /// <param name="commandID">This is the explicitly set command id, leave this as null if you want to use the default id.</param>
        /// <param name="container">The container to be added to</param>
        public AppCommandStandard(Guid? commandID, System.ComponentModel.IContainer container) : base(commandID, container) { }
        #endregion
    }
    #endregion

    #region AppCommandStandard<RQ, RS, CBRQ, CBRS, CONF, PERF>
    /// <summary>
    /// AppCommandStandard is the base command object that all standard commands should inherit from.
    /// </summary>
    /// <typeparam name="RQ">The main request RQRSFolder type.</typeparam>
    /// <typeparam name="RS">The main response RQRSFolder type.</typeparam>
    /// <typeparam name="CBRQ">The callback request RQRSFolder type.</typeparam>
    /// <typeparam name="CBRS">The callback response RQRSFolder type.</typeparam>
    /// <typeparam name="CONF">The command configuration object.</typeparam>
    /// <typeparam name="PERF">The command performance monitor object.</typeparam>
    /// <typeparam name="EXTCONF">The external command object that contains a set of user configurable settings.</typeparam>
    public class AppCommandStandard<RQ, RS, CBRQ, CBRS, CONF, PERF, EXTCONF> : AppCommandBase<CONF, PERF, EXTCONF>, IXimuraCommandRQ
        where RQ : RQRSFolder, new() //Request
        where RS : RQRSFolder, new() //Response
        where CBRQ : RQRSFolder, new() //Callback Request
        where CBRS : RQRSFolder, new() //Callback Response
        where CONF : CommandConfiguration, new()
        where PERF : CommandPerformance, new()
        where EXTCONF : CommandConfiguration, new() //Internal Configuration which contains the settings for the internal commands
    {
		#region Declarations
        private System.Threading.Timer pickUpTimer = null;
        private object syncTimerChange = new object();
        private bool? mTimerEnabled = null;

        private bool mSchedulerAutoRegister = false;
        private List<Guid> mRegisteredCallbacks = new List<Guid>();
        private IXimuraSchedulerCommandRegister mScheduler;

		#endregion
		#region Constructor
		/// <summary>
		/// This is the empty constructor
		/// </summary>
		public AppCommandStandard():this((IContainer)null){}
		/// <summary>
		/// This is the base constructor for a Ximura command
		/// </summary>
		/// <param name="container">The command container to be added to.</param>
        public AppCommandStandard(System.ComponentModel.IContainer container) : 
            base(container) { }
        /// <summary>
        /// This is the base constructor for a Ximura command
        /// </summary>
        /// <param name="commandID">This is the explicitly set command id, leave this as null if you want to use the default id.</param>
        /// <param name="container">The container to be added to</param>
        public AppCommandStandard(Guid? commandID, System.ComponentModel.IContainer container) : base(commandID, container) { }
		#endregion

		#region ServicesProvide()
		/// <summary>
		/// This override registers the command with the dispatcher commands collection
		/// </summary>
		protected override void ServicesProvide()
		{
			base.ServicesProvide ();

            //Get a reference to the system wide pool manager
            PoolManagerStart();

            CommandBridgeStart();
		}
		#endregion // ServicesProvide()
		#region ServicesRemove()
		/// <summary>
		/// This overriden method removes the dispatcherCollection services
		/// </summary>
		protected override void ServicesRemove()
		{
            CommandBridgeStop();

            PoolManagerStop();

			base.ServicesRemove ();
		}
		#endregion // ServicesRemove()
        #region ServicesReference()
        /// <summary>
        /// This overriden method reference the scheduled services.
        /// </summary>
        protected override void ServicesReference()
        {
            base.ServicesReference();

            SchedulerStart();
        }
        #endregion // ServicesReference()
        #region ServicesDereference()
        /// <summary>
        /// This overriden method unregisters the scheduled services.
        /// </summary>
        protected override void ServicesDereference()
        {
            SchedulerStop();

            base.ServicesDereference();
        }
        #endregion // ServicesDereference()

        #region Start()
        /// <summary>
        /// This method starts the service based on the default async settings
        /// </summary>
        public override void Start()
        {
            //if (this.CommandSettings == null)
            if (!ConfigurationStart())
            {
                XimuraAppTrace.WriteLine(CommandName + " - there are no settings for the command so it cannot start.",
                    CommandName, EventLogEntryType.Error);
                return;
            }

            PerformanceStart();

            //Check whether this command is enabled, and if so then start.
            if (this.Configuration.CommandEnabled)
                base.Start();
            else
                XimuraAppTrace.WriteLine(CommandName + " - this command is currently disabled and did not start.",
                    CommandName, EventLogEntryType.Warning);

        }
        #endregion // Start()
        #region Stop()
        /// <summary>
        /// This override removes the performance and configuration.
        /// </summary>
        public override void Stop()
        {
            TimerStop();
            PerformanceStop();
            ConfigurationStop();

            base.Stop();
        }
        #endregion // Stop()

        #region GetSettings()
        /// <summary>
		/// This protected member returns the default settings for the command
		/// </summary>
		/// <returns>The settings object</returns>
		protected virtual object GetSettings()
		{
            if (this.ParentCommandName != null && this.ParentCommandName !="")
                return base.GetSettings(@"Commands/" + this.ParentCommandName + "/" + this.CommandName);
            else
    			return base.GetSettings(@"Commands/" + this.CommandName);
		}
		#endregion // CommandSettings
		#region CommandSettings
        ///// <summary>
        ///// This object contains the command settings.
        ///// </summary>
        //[Browsable(false)]
        //[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        //protected virtual AppCommandConfigSH CommandSettings
        //{
        //    get
        //    {
        //        try
        //        {
        //            if (mCmdConfig == null)
        //                mCmdConfig = this.GetSettings() as AppCommandConfigSH;

        //            return mCmdConfig;
        //        }
        //        catch (Exception ex)
        //        {
        //            if (AppConfigurationAttribute.ConfigType != ConfigurationLocation.Hybrid)
        //                return null;

        //            throw ex;
        //        }
        //    }
        //    set
        //    {
        //        mCmdConfig = value;
        //    }
        //}
		#endregion
        #region BaseSettings
        ///// <summary>
        ///// This override provides the base settings.
        ///// </summary>
        //protected override IXimuraConfigSH BaseSettings
        //{
        //    get
        //    {
        //        return CommandSettings as IXimuraConfigSH;
        //    }
        //}
        #endregion // BaseSettings

        #region SCM MAIN ENTRY POINT -> IXimuraCommandRQ -> ProcessRequest
        /// <summary>
        /// This is the default entry point for the job request from the security manager. By default
        /// the command will track job processes and update statistics for each job.
        /// </summary>
        /// <param name="job">The job to process.</param>
        public virtual void ProcessRequestSCM(SecurityManagerJob job)
        {
            //Sanity check that the job is not null.
            if (job == null)
                throw new ArgumentNullException("The Job is null.", "SecurityManagerJob job");

            //Check whether the service is actually running, and if not quit.
            if (this.ServiceStatus != XimuraServiceStatus.Started)
            {
                job.Data.Response.Status = CH.HTTPCodes.ServiceUnavailable_503;
                job.Data.Response.Substatus = "Service not started.";

                return;
            }

            //Check whether this is a call back and whether call backs are supported.
            if (!SupportsCallbacks && IsCallback(job.Data))
            {
                XimuraAppTrace.WriteLine("Callbacks are not supported.",
                    CommandName, EventLogEntryType.Warning);

                job.Data.Response.Status = CH.HTTPCodes.BadRequest_400;
                job.Data.Response.Substatus = "Callbacks are not supported by " + CommandName + ".";
                return;
            }
            //OK, process the incoming job.
            try
            {
                if (IsCallback(job.Data))
                {
                    if (CallbackValidate(job))
                    {
                        Performance.RequestCallbackStart(job.ID.Value);
                        RQRSContract<CBRQ, CBRS> incomingCBRequest = (RQRSContract<CBRQ, CBRS>)job.Data;
                        ProcessCallback(job, incomingCBRequest);
                        Performance.RequestCallbackEnd(job.ID.Value);
                    }
                    else
                    {
                        job.Data.Response.Status = CH.HTTPCodes.Conflict_409;
                        job.Data.Response.Substatus = "The callback cannot be validated.";
                    }
                }
                else
                {
                    Performance.RequestStart(job.ID.Value);
                    RQRSContract<RQ, RS> incomingRequest = (RQRSContract<RQ, RS>)job.Data;
                    ProcessRequest(job, incomingRequest);
                    Performance.RequestEnd(job.ID.Value);
                }
            }
            catch (InvalidCastException ivex)
            {
                //Check whether there was an invalid cast during the EnvelopeContract conversion.
                XimuraAppTrace.WriteLine("Invalid cast exception: " + ivex.Message
                    + Environment.NewLine + Environment.NewLine + ivex.ToString(), 
                    CommandName, EventLogEntryType.Error);
                throw ivex;
            }
            catch (Exception ex)
            {
                //Log the unhandled job exception and pass to the security manager.
                XimuraAppTrace.WriteLine("Unhandled exception: " + ex.Message
                    + Environment.NewLine + Environment.NewLine + ex.ToString(), 
                    CommandName, EventLogEntryType.Error);
                throw ex;
            }
        }
		#endregion
        #region ProcessRequest -> MAIN ENTRY POINT
        /// <summary>
        /// This is the default command process logic. You should override this method to implement your
        /// specific business logic.
        /// </summary>
        /// <param name="job">The job to process.</param>
		protected virtual void ProcessRequest(SecurityManagerJob job)
		{
			throw new NotImplementedException("ProcessRequest is not implemented - " + CommandName);
		}
        /// <summary>
        /// This is the default generic process point. This will call the non-generic method for backwards
        /// compatibility.
        /// </summary>
        /// <param name="job"></param>
        /// <param name="Data"></param>
        protected virtual void ProcessRequest(SecurityManagerJob job, RQRSContract<RQ,RS> Data)
        {
            ProcessRequest(job);
        }
		#endregion

        #region ProcessCallback -> MAIN ENTRY POINT
        /// <summary>
        /// This is the standard method for processing callbacks.
        /// </summary>
        /// <param name="job">The job to process.</param>
        /// <param name="Data">The callback data.</param>
        protected virtual void ProcessCallback(SecurityManagerJob job, RQRSContract<CBRQ, CBRS> Data)
        {
            throw new NotImplementedException("ProcessCallback is not implemented: " + this.CommandName);
        }
		#endregion
        #region Callback support methods
        /// <summary>
        /// This property identifies whether the command supports callbacks.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual bool SupportsCallbacks 
        { 
            get { return mRegisteredCallbacks.Count > 0; } 
        }
        /// <summary>
        /// The method returns true if the Data is a callback type.
        /// </summary>
        /// <param name="Data">The envelope to check.</param>
        /// <returns>Return true if this is a callback.</returns>
        protected virtual bool IsCallback(IXimuraRQRSEnvelope Data)
        {
            return Data is RQRSContract<CBRQ,CBRS>;
        }
        /// <summary>
        /// This method will be called when the command starts and should
        /// be overriden and permitted callback commands should be registered.
        /// </summary>
        protected virtual void CallbackRegister()
        {

        }
        /// <summary>
        /// This method registers the callback.
        /// </summary>
        /// <param name="jobID">The command ID to be registered.</param>
        protected virtual void CallbackRegister(Guid commandID)
        {
            mRegisteredCallbacks.Add(commandID);
        }
        /// <summary>
        /// This method will be called when the command stops and will automatically unregister any callback commands.
        /// </summary>
        protected virtual void CallbackUnregister()
        {
            mRegisteredCallbacks.Clear();
        }
        /// <summary>
        /// This method unregisters the callback.
        /// </summary>
        /// <param name="commandID">The commandID to unregister.</param>
        protected virtual void CallbackUnregister(Guid commandID)
        {
            if (mRegisteredCallbacks.Contains(commandID))
                mRegisteredCallbacks.Remove(commandID);
        }
        /// <summary>
        /// This method validates a callback for a command ID.
        /// </summary>
        /// <param name="commandID">The command ID to validate</param>
        /// <returns>Returns true of the command is registered.</returns>
        protected virtual bool CallbackValidate(Guid commandID)
        {
            return mRegisteredCallbacks.Contains(commandID);
        }
        /// <summary>
        /// This method validates a callback for a job request.
        /// </summary>
        /// <param name="job">The callback job to validate</param>
        /// <returns>Returns true if the job is validated.</returns>
        protected virtual bool CallbackValidate(SecurityManagerJob job)
        {
            return CallbackValidate(job.Data.Sender);
        }
        #endregion

        #region CommandBridgeRegister(bool register)
        /// <summary>
        /// This method determines whether the command is registered or unregistered with the command bridge.
        /// </summary>
        /// <param name="register">A boolean value indicating whether this command should be registered or unregistered.</param>
        protected override void CommandBridgeRegister(bool register)
        {
            if (CommandBridge == null)
                return;

            if (register)
                CommandBridge.Register(this);
            else
                CommandBridge.Unregister(this);
        }
        #endregion // CommandBridgeRegister(bool register)

        #region Scheduler
        #region Scheduler
        /// <summary>
        /// This is the scheduler.
        /// </summary>
        protected IXimuraSchedulerCommandRegister Scheduler
        {
            get
            {
                return mScheduler;
            }
        }
        #endregion // Scheduler
        #region SchedulerAutoRegister
        /// <summary>
        /// This property determines whether the command will automatically register with the
        /// scheduler on start up.
        /// </summary>
        [Category("Command Settings")]
        [DefaultValue(false)]
        [Description("This property determines whether the command will automatically register with the scheduler on start up.")]
        public virtual bool SchedulerAutoRegister
        {
            get
            {
                return mSchedulerAutoRegister;
            }
            set
            {
                mSchedulerAutoRegister = value;
            }
        }
        #endregion // SchedulerAutoRegister
        #region SchedulerServices
        /// <summary>
        /// Use this method registers scheduler callbacks.
        /// </summary>
        protected virtual void SchedulerRegister()
        {
            Scheduler.RegisterCommand(this.CommandID);
            SchedulerRegisterSubCommands();
        }
        /// <summary>
        /// This method registers specific subcommands for the scheduler. By default this method will
        /// register the blank subcommand. 
        /// </summary>
        protected virtual void SchedulerRegisterSubCommands()
        {
            Scheduler.RegisterSubCommand(CommandID, "",
                this.CommandName, this.CommandDescription, null, Configuration.CommandPriority, true);
        }
        /// <summary>
        /// Use this method to unregister scheduler callbacks.
        /// </summary>
        protected virtual void SchedulerUnregister()
        {
            //Tell the scheduler that we are not active.
            Scheduler.UnregisterCommand(this.CommandID);
        }
        #endregion // Scheduler
        #region SchedulerStart()
        /// <summary>
        /// This method gets a reference to the scheduler service.
        /// </summary>
        protected virtual void SchedulerStart()
        {
            mScheduler = GetService<IXimuraSchedulerCommandRegister>();

            if (mScheduler != null && SchedulerAutoRegister)
                SchedulerRegister();
        }
        #endregion // SchedulerStart()
        #region SchedulerStop()
        /// <summary>
        /// THis method removes and scheduler notifications and removes the scheduler reference.
        /// </summary>
        protected virtual void SchedulerStop()
        {
            if (mScheduler != null)
            {
                SchedulerUnregister();
                mScheduler = null;
            }
        }
        #endregion // SchedulerStop()
        #endregion // Scheduler

        #region Command Types
        #region RequestType
        /// <summary>
        /// This is the command request type.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual Type RequestType
        {
            get { return typeof(RQ); }
        } 
        #endregion
        #region ResponseType
        /// <summary>
        /// This is the command response type.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual Type ResponseType
        {
            get { return typeof(RS); }
        } 
        #endregion
        #region CallbackRequestType
        /// <summary>
        /// This is the command callback request type.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual Type CallbackRequestType
        {
            get { return typeof(CBRQ); }
        }
        #endregion
        #region CallbackResponseType
        /// <summary>
        /// This is the command callback response type.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual Type CallbackResponseType
        {
            get { return typeof(CBRS); }
        }
        #endregion
        #region EnvelopeContractType
        /// <summary>
        /// This is the envelope contract type. 
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual Type EnvelopeContractType
        {
            get
            {
                return typeof(RQRSContract<RQ, RS>);
            }
        } 
        #endregion
        #region EnvelopeCallbackContractType
        /// <summary>
        /// This is the envelope callback contract type. 
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual Type EnvelopeCallbackContractType
        {
            get
            {
                return typeof(RQRSContract<CBRQ, CBRS>);
            }
        }
        #endregion
        #endregion // Command Types

        #region SupportsNotifications/Notify(Type notificationType, object notification)
        /// <summary>
        /// This method returns true as the command uses notification to enable and disable the timer poll.
        /// </summary>
        [Category("Command Settings")]
        [DefaultValue(true)]
        [Description("This method returns true as the command uses notification to enable and disable the timer poll.")]
        [ReadOnly(true)]
        public override bool SupportsNotifications
        {
            get
            {
                return true;
            }
        }
        /// <summary>
        /// This method starts and stops the timer based on the system start/stop notification.
        /// </summary>
        /// <param name="notificationType">The notification type.</param>
        /// <param name="notification">The notification.</param>
        public override void Notify(Type notificationType, object notification)
        {
            if (notificationType == typeof(XimuraServiceStatus))
            {
                XimuraServiceStatus status = (XimuraServiceStatus)notification;
                switch (status)
                {
                    case XimuraServiceStatus.Started:
                        if (ServiceStatus == XimuraServiceStatus.Started && TimerEnabled)
                            TimerStart();
                        break;
                    case XimuraServiceStatus.Stopped:
                    case XimuraServiceStatus.Stopping:
                        if (ServiceStatus == XimuraServiceStatus.Started && TimerEnabled)
                            TimerStop();
                        break;
                }
            }
        }
        #endregion // SupportsNotifications

        #region Timer Functionality
        #region TimerChange(object state,int dueTime)
        /// <summary>
        /// This method is used to change the timer settings.
        /// </summary>
        /// <param name="span">The due time.</param>
        protected virtual void TimerChange(TimeSpan span)
        {
            //Convert the span to milliseconds.
            TimerChange((long)span.TotalMilliseconds);
        }
        /// <summary>
        /// This method is used to change the timer settings.
        /// </summary>
        /// <param name="dueTime">The due time.</param>
        protected virtual void TimerChange(long dueTime)
        {
            try
            {
                lock (syncTimerChange)
                {
                    if (pickUpTimer != null)
                        pickUpTimer.Change(dueTime, System.Threading.Timeout.Infinite);
                    else
                    {
                        if (pickUpTimer != null)
                            pickUpTimer.Change(System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);

                        pickUpTimer = new Timer(InternalTimerCallback, null, dueTime, System.Threading.Timeout.Infinite);
                    }
                }
            }
            catch (Exception) { }
        }
        #endregion // TimerChange(object state,int dueTime, int period)

        #region TimerEnabled
        /// <summary>
        /// This method is used to override the default auto start for the timer in the base command.
        /// You can override this method if you need more fine-grained timer start
        /// functionality.
        /// </summary>
        [Category("Command Settings")]
        [DefaultValue(false)]
        [Description("This property determines whether the timer will start.")]
        public virtual bool TimerEnabled
        {
            get
            {
                if (mTimerEnabled.HasValue)
                    return mTimerEnabled.Value;

                if (this.DesignMode && Configuration == null)
                    return false;

                return Configuration.TimerEnabled;
            }
            set
            {
                mTimerEnabled = value;
            }
        }
        #endregion // TimerBaseAutoStart

        #region TimerStart()
        /// <summary>
        /// This method initializes the timer used for pick up polling.
        /// The default settings for this command is to wait indefintely, so
        /// you should override this method if you require different polling.
        /// </summary>
        protected virtual void TimerStart()
        {
            //If the AutoStart property is set, then we call the OnTimerEvent on initialization.
            if (Configuration.TimerAutoStart)
                OnTimerEvent(true);

            //Ok, if the poll enable is set then we will start the timer.
            if (!Configuration.TimerPollEnabled)
                return;

            Configuration.TimerRecalculate();
            TimerChange(Configuration.TimerDueTime);
        }
        #endregion // TimerStart()
        #region TimerStop()
        /// <summary>
        /// This method stops the timer.
        /// </summary>
        protected virtual void TimerStop()
        {
            if (pickUpTimer != null)
                TimerChange(System.Threading.Timeout.Infinite);
        }
        #endregion // TimerStop()

        #region TimerPause()
        /// <summary>
        /// This method will pause the timer.
        /// </summary>
        protected virtual void TimerPause()
        {
            TimerChange(System.Threading.Timeout.Infinite);
        }
        #endregion // TimerPause()
        #region TimerResume()
        /// <summary>
        /// This method will resume the timer based on the settings.
        /// </summary>
        protected virtual void TimerResume()
        {
            Configuration.TimerRecalculate();
            TimerChange(Configuration.TimerDueTime);
        }
        #endregion // TimerResume()

        #region InternalTimerCallback
        /// <summary>
        /// This internal method ensures that a unhandled exception in a ontimer method called 
        /// does not result in the system crashing out.
        /// </summary>
        /// <param name="state"></param>
        private void InternalTimerCallback(object state)
        {
            try
            {
                OnTimerEvent(false);
            }
            catch (Exception ex)
            {
                XimuraAppTrace.WriteLine("OnTimerEvent Unhandled exception: " + ex.ToString()
                    , CommandName, EventLogEntryType.Error);
            }
        }
        #endregion // AppCommandProcessBase_InternalTimerCallback

        #region OnTimerEvent(bool autoStart)
        /// <summary>
        /// This method will be called when a timer is called.
        /// </summary>
        protected virtual void OnTimerEvent(bool autoStart)
        {
            if (autoStart)
                Configuration.TimerPollJobs.ForEach(job => job.TimerRecalculate(true));
            else
                TimerPause();

            try
            {
                Configuration.TimerPollJobs
                    .Where(job => job.NextPollTime.HasValue && job.NextPollTime.Value <= DateTime.Now)
                    .ForEach(job => TimerPollJobExecute(job));
            }
            catch (Exception)
            {
            }

            if (!autoStart)
            {
                TimeSpan? nextPoll = Configuration.TimerPollInterval;
                if (nextPoll.HasValue)
                    TimerChange(nextPoll.Value);
            }
        }
        #endregion // OnTimerEvent(object state)

        #region TimerPollJobExecute(CommandConfiguration.TimerPollJob job)
        /// <summary>
        /// This method executes a timer poll job and handles the response. You should override this method to provide
        /// your own logic.
        /// </summary>
        /// <param name="job">The timer poll job.</param>
        protected virtual void TimerPollJobExecute(TimerPollJob job)
        {
            
        }
        #endregion // TimerPoll(CommandConfiguration.TimerPollJob job)
        #endregion // Timer Functionality
	}
    #endregion // AppCommandStandard<RQ, RS, CBRQ, CBRS, CONF, PERF>
}