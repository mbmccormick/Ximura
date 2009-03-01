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
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Diagnostics;

using Ximura;
using Ximura.Data;
using Ximura.Persistence;
using Ximura.Helper;
using CH=Ximura.Helper.Common;

using Ximura.Server;

#endregion // using
namespace Ximura.Command
{
    #region AppCommandProcess
    /// <summary>
    /// AppCommandProcess is the base class for Command that require a system session.
    /// </summary>
    public class AppCommandProcess : AppCommandProcess<RQRSFolder, RQRSFolder, CommandConfiguration>
    {
        #region Constructors
        /// <summary>
        /// This is the empty constructor
        /// </summary>
        public AppCommandProcess() : this((IContainer)null) { }
        /// <summary>
        /// This is the constrcutor used by the Ximura Application model.
        /// </summary>
        /// <param name="container">The command container.</param>
        public AppCommandProcess(System.ComponentModel.IContainer container)
            : base(container)
        {
        }
        /// <summary>
        /// This is the base constructor for a Ximura command
        /// </summary>
        /// <param name="commandID">This is the explicitly set command id, leave this as null if you want to use the default id.</param>
        /// <param name="container">The container to be added to</param>
        public AppCommandProcess(Guid? commandID, System.ComponentModel.IContainer container) : base(commandID, container) { }
        #endregion
    }
    #endregion // AppCommandProcess
    #region AppCommandProcess<RQ, RS>
    /// <summary>
    /// AppCommandProcess is the base class for Command that require a system session.
    /// </summary>
    /// <typeparam name="RQ">The main request RQRSFolder type.</typeparam>
    /// <typeparam name="RS">The main response RQRSFolder type.</typeparam>
    public class AppCommandProcess<RQ, RS> : AppCommandProcess<RQ, RS, RQRSFolder, RQRSFolder, CommandConfiguration, CommandPerformance>
        where RS : RQRSFolder, new()
        where RQ : RQRSFolder, new()
    {
        #region Constructors
        /// <summary>
        /// This is the empty constructor
        /// </summary>
        public AppCommandProcess() : this((IContainer)null) { }
        /// <summary>
        /// This is the constrcutor used by the Ximura Application model.
        /// </summary>
        /// <param name="container">The command container.</param>
        public AppCommandProcess(System.ComponentModel.IContainer container)
            : base(container)
        {
        }
        /// <summary>
        /// This is the base constructor for a Ximura command
        /// </summary>
        /// <param name="commandID">This is the explicitly set command id, leave this as null if you want to use the default id.</param>
        /// <param name="container">The container to be added to</param>
        public AppCommandProcess(Guid? commandID, System.ComponentModel.IContainer container) : base(commandID, container) { }
        #endregion
    }
    #endregion // AppCommandProcess<RQ, RS>
    #region AppCommandProcess<RQ, RS, CONF>
    /// <summary>
    /// AppCommandProcess is the base class for Command that require a system session.
    /// </summary>
    /// <typeparam name="RQ">The main request RQRSFolder type.</typeparam>
    /// <typeparam name="RS">The main response RQRSFolder type.</typeparam>
    /// <typeparam name="CONF">The command configuration object.</typeparam>
    public class AppCommandProcess<RQ, RS, CONF> : AppCommandProcess<RQ, RS, RQRSFolder, RQRSFolder, CONF, CommandPerformance>
        where RS : RQRSFolder, new()
        where RQ : RQRSFolder, new()
        where CONF : CommandConfiguration, new()
    {
		#region Constructors
		/// <summary>
		/// This is the empty constructor
		/// </summary>
		public AppCommandProcess():this((IContainer)null){}
		/// <summary>
		/// This is the constrcutor used by the Ximura Application model.
		/// </summary>
		/// <param name="container">The command container.</param>
        public AppCommandProcess(System.ComponentModel.IContainer container)
            : base(container)
		{
		}
        /// <summary>
        /// This is the base constructor for a Ximura command
        /// </summary>
        /// <param name="commandID">This is the explicitly set command id, leave this as null if you want to use the default id.</param>
        /// <param name="container">The container to be added to</param>
        public AppCommandProcess(Guid? commandID, System.ComponentModel.IContainer container) : base(commandID, container) { }
		#endregion
    }
    #endregion // AppCommandProcess<RQ, RS, CONF>

    #region AppCommandProcess<RQ, RS, CBRQ, CBRS, CONF, PERF>
    /// <summary>
    /// AppCommandProcess is the base class for Command that require a system session.
    /// </summary>
    /// <typeparam name="RQ">The main request RQRSFolder type.</typeparam>
    /// <typeparam name="RS">The main response RQRSFolder type.</typeparam>
    /// <typeparam name="CBRQ">The callback request RQRSFolder type.</typeparam>
    /// <typeparam name="CBRS">The callback response RQRSFolder type.</typeparam>
    /// <typeparam name="CONF">The command configuration object.</typeparam>
    /// <typeparam name="PERF">The command performance monitor object.</typeparam>
    public class AppCommandProcess<RQ, RS, CBRQ, CBRS, CONF, PERF> : 
        AppCommandStandard<RQ, RS, CBRQ, CBRS, CONF, PERF>, IXimuraCommandProcess
        where RS : RQRSFolder, new()
        where RQ : RQRSFolder, new()
        where CBRQ : RQRSFolder, new()
        where CBRS : RQRSFolder, new()
        where CONF : CommandConfiguration, new()
        where PERF : CommandPerformance, new()
    {
		#region Declarations

        ICDSSettingsService mCDSSettings = null;
		/// <summary>
		/// This is the session that the process command will run under.
		/// </summary>
		protected IXimuraSession mProcessSession = null;
		/// <summary>
		/// This is a reference to the application session manager.
		/// </summary>
		protected IXimuraSessionManager mSessionMan = null;
		/// <summary>
		/// The Command Process Settings
		/// </summary>
		protected IXimuraCommandProcessConfigSH mSettings = null;

        private CDSHelper mCDSHelper = null;

		#endregion
		#region Constructors
		/// <summary>
		/// This is the empty constructor
		/// </summary>
		public AppCommandProcess():this((IContainer)null){}
		/// <summary>
		/// This is the constrcutor used by the Ximura Application model.
		/// </summary>
		/// <param name="container">The command container.</param>
        public AppCommandProcess(System.ComponentModel.IContainer container)
            : base(container)
		{
		}
        /// <summary>
        /// This is the base constructor for a Ximura command
        /// </summary>
        /// <param name="commandID">This is the explicitly set command id, leave this as null if you want to use the default id.</param>
        /// <param name="container">The container to be added to</param>
        public AppCommandProcess(Guid? commandID, System.ComponentModel.IContainer container) : base(commandID, container) { }
		#endregion

		#region InternalStart/InternalStop
		/// <summary>
		/// This overrides the Internal start and negotiates a session for the process
		/// </summary>
		protected override void InternalStart()
		{
			base.InternalStart();

			//Negotiate a process session
			ProcessSessionNegotiate();

		}
		/// <summary>
		/// This overrides the stop process and closes the process session.
		/// </summary>
		protected override void InternalStop()
		{
            TimerStop();

			//Close a process session
			ProcessSessionClose();

			base.InternalStop();
		}
		#endregion

		#region Services
		/// <summary>
		/// Creates a reference to the IXimuraSessionManager
		/// </summary>
		protected override void ServicesReference()
		{
			base.ServicesReference ();

            mSessionMan = GetService<IXimuraSessionManager>();
		}
		/// <summary>
		/// Removes a reference to IXimuraSessionManager
		/// </summary>
		protected override void ServicesDereference()
		{
			mCDSSettings = null;

			mSessionMan = null;

			base.ServicesDereference ();
		}
		#endregion

		#region ProcessSession Methods
		/// <summary>
		/// This method negotiates a session for the process.
		/// </summary>
		protected virtual void ProcessSessionNegotiate()
		{
            if (!Configuration.ProcessSessionRequired)
                return;

            IXimuraSession theSession = mSessionMan.SessionCreate(
                Configuration.ProcessSessionRealmDomain, Configuration.ProcessSessionUserName);

            //byte[] seed = theSession.GetSeed();

            //if (seed != null)
            //{

            //    byte[] hash = Configuration.ProcessSessionHash(seed);

            //    theSession.Authenticate(hash);
            //}

            mProcessSession = theSession;
		}
		/// <summary>
		/// This method closes a sessions for the process
		/// </summary>
		protected virtual void ProcessSessionClose()
		{
			//Close the active session
            if (mProcessSession!=null)
			    mProcessSession.Close();

			//Remove any reference to the session, this is a critical
			//section as we do not multiple references.
			lock (this)
			{
				mProcessSession = null;
			}
		}
		#endregion

		#region CDSSettings
		/// <summary>
		/// This protected property is the CDS settings object.
		/// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        protected virtual ICDSSettingsService CDSSettings
		{
			get
			{
				if (mCDSSettings == null)
					mCDSSettings = GetService(typeof(ICDSSettingsService)) 
						as ICDSSettingsService;

				return mCDSSettings;
			}
		}
		#endregion // CDSSettings

        #region CDSHelper
        /// <summary>
        /// The CDSHelper is the object that encapsulates the functionality needed
        /// to receive and manage content from the Content Data Store.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        protected virtual CDSHelper CDSHelper
        {
            get
            {
                if (mCDSHelper != null)
                    return mCDSHelper;

                if (this.mProcessSession == null)
                    return null;

                mCDSHelper = new CDSHelper(this.mProcessSession);

                return mCDSHelper;
            }
        }
        #endregion // DCWrapper

		#region PrepareJob
		/// <summary>
		/// This method prepares the parameters for the job request.
		/// </summary>
		/// <param name="job">The job.</param>
		/// <param name="Request">The request.</param>
		/// <param name="Response">The response.</param>
		protected void PrepareJob(SecurityManagerJob job, out RQ Request, out RS Response)
		{
			try
			{
                Request = job.Data.Request as RQ;
				Response = job.Data.Response as RS;
                //IXimuraRQRSEnvelope.PrepareResponse(job.Data, out Request, out Response);
			}
			catch (Exception ex)
			{
				XimuraAppTrace.WriteLine(ex, this.CommandName + "_Error_Envelope");
				throw ex;
			}
		}
        /// <summary>
        /// This helper method is provides for methods that do not want to reference the Request
        /// and Response methods directly.
        /// </summary>
        /// <param name="job">The job to prepare.</param>
        protected void PrepareJob(SecurityManagerJob job)
        {
            RQ Request;
            RS Response;
            PrepareJob(job, out Request, out Response);
        }
		#endregion // PrepareJob

        #region Timer Functionality
        #region TimerPollJobExecute(CommandConfiguration.TimerPollJob job)
        /// <summary>
        /// This method initiates a timer poll job and handles the response.
        /// </summary>
        /// <param name="job">The time poll job.</param>
        protected override void TimerPollJobExecute(TimerPollJob job)
        {
            if (!job.Enabled)
                return;

            if (job.Active)
                return;

            job.Active = true;

            TimerHelperAsyncSelfCall(job,
                delegate(object sender, CommandRSEventArgs args)
                {
                    try
                    {
                        if (args.Data != null && args.Data.ObjectPoolCanReturn)
                            args.Data.ObjectPoolReturn();

                        job.TimerRecalculate(false);
                        job.Active = false;
                    }
                    catch (Exception) { }

                }
            );
        }
        #endregion // TimerPoll(CommandConfiguration.TimerPollJob job)
        #region TimerHelperAsyncSelfCall
        /// <summary>
        /// This method can be used to initiate a callback.
        /// </summary>
        /// <param name="subCommand">The subcommand.</param>
        /// <param name="callback">The callback delegate.</param>
        protected virtual void TimerHelperAsyncSelfCall(TimerPollJob job, CommandRSCallback callback)
        {
            TimerHelperAsyncSelfCall(job, callback, JobPriority.Low, null);
        }
        /// <summary>
        /// This method can be used to initiate a callback.
        /// </summary>
        /// <param name="subCommand">The subcommand.</param>
        /// <param name="callback">The callback delegate.</param>
        /// <param name="priority">The request priority.</param>
        /// <param name="rqFormat">The delegate used to format the request. This parameter can be null.</param>
        protected virtual void TimerHelperAsyncSelfCall(TimerPollJob job, CommandRSCallback callback, 
            JobPriority priority, Action<RQRSFolder> rqFormat)
        {
            RQRSContract<RQ, RS> Env = null;
            try
            {
                Env = RQRSEnvelopeHelper.Get<RQ, RS>();
                Env.DestinationAddress = new EnvelopeAddress(CommandID, job.Subcommand);

                if (rqFormat != null)
                    rqFormat(Env.Request);
                else if (job.RequestFormat != null)
                    job.RequestFormat(Env.Request);

                //Ok, call this command async as we do not want to use the system thread, 
                //also we call this command with a low priority so that real-time jobs
                //take priority.
                mProcessSession.ProcessRequestAsync(Env, callback, null, priority);
            }
            catch (Exception ex)
            {
                //We do not throw exception when this is not our thread, as this will not be caught
                //and may kill the service.
                if (Env != null && Env.ObjectPoolCanReturn)
                    Env.ObjectPoolReturn();
            }
        }
        #endregion // TimerHelperAsyncSelfCall
        #endregion // Timer Functionality
    }
    #endregion // AppCommandProcess<RQ, RS, CBRQ, CBRS, CONF, PERF>
}