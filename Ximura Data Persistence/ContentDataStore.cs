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
using System.Data;
using System.Data.SqlClient;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;

using Ximura;
using Ximura.Data;
using Ximura.Data;


using CH = Ximura.Common;
using RH = Ximura.Reflection;
using Ximura.Framework;


using Ximura.Framework;
#endregion // using
namespace Ximura.Data
{
    /// <summary>
    /// This command is the new content data store based fully on the Finite State Machine architecture.
    /// </summary>
#if (DEBUG)
    [XimuraAppConfiguration(ConfigurationLocation.Hybrid,
        "xmrres://XimuraDataPersistence/Ximura.Data.CDSConfiguration/Ximura.Data.Configuration.CDSConfiguration_Default.xml")]
#else
    [XimuraAppConfiguration(ConfigurationLocation.Hybrid, 
        "xmrres://XimuraDataPersistence/Ximura.Data.CDSConfiguration/Ximura.Data.Configuration.CDSConfiguration_Default.xml")]
#endif
    [XimuraAppModule(CDSHelper.ID, CDSHelper.Name)]
    public class ContentDataStore : FiniteStateMachine<CDSRequestFolder, CDSResponseFolder, 
        RQRSFolder, RQRSFolder, CDSContext, ICDSState, CDSSettings, CDSConfiguration, CDSPerformance>
    {
        #region Declarations
        private System.ComponentModel.Container components;
        /// <summary>
        /// The start state.
        /// </summary>
        protected StartCDSState startState;
        /// <summary>
        /// The cache manager.
        /// </summary>
        protected CacheManagerCDSState cacheState;
        /// <summary>
        /// The finish state.
        /// </summary>
        protected FinishCDSState finishState;

        private CDSCacheManagerBridge mCacheManagerBridge;

        //private SQLServerStorageManager storageSQL;
        //private FileSystemStorageManager storageFilesystem;

        #endregion
        #region Constructors
		/// <summary>
		/// Empty constructor used during the design mode.
		/// </summary>
		public ContentDataStore():this((IContainer)null){}
		/// <summary>
		/// This is the component model constructor.
		/// </summary>
        public ContentDataStore(IContainer container)
            : base(container)
		{
			InitializeComponents();
            InitializeCacheManagerBridge();
			RegisterContainer(components);
		}
		#endregion
        #region InitializeCacheManagerBridge()
        /// <summary>
        /// This method initializes the cache manager bridge.
        /// </summary>
        protected virtual void InitializeCacheManagerBridge()
        {
            mCacheManagerBridge = new CDSCacheManagerBridge();
        }
        #endregion // InitializeCacheManagerBridge()

        #region InitializeComponents()
        private void InitializeComponents()
        {
            components = new System.ComponentModel.Container();

            startState = new StartCDSState(components);
            cacheState = new CacheManagerCDSState(components);
            finishState = new FinishCDSState(components);
            //storageSQL = new SQLServerStorageManager(components);
            //storageFilesystem = new FileSystemStorageManager(components);

            ((System.ComponentModel.ISupportInitialize)(this.mStateExtender)).BeginInit();

            // 
            // startState
            // 
            this.mStateExtender.SetEnabled(this.startState, true);
            this.startState.Identifier = "Start";
            this.mStateExtender.SetNextStateID(this.startState, null);
            this.mStateExtender.SetStateID(this.startState, "Start");
            // 
            // cacheState
            // 
            this.mStateExtender.SetEnabled(this.cacheState, true);
            this.cacheState.Identifier = "CacheManager";
            this.mStateExtender.SetNextStateID(this.cacheState, null);
            this.mStateExtender.SetStateID(this.cacheState, "CacheManager");
            // 
            // finishState
            // 
            this.mStateExtender.SetEnabled(this.finishState, true);
            this.finishState.Identifier = "Finish";
            this.mStateExtender.SetNextStateID(this.finishState, null);
            this.mStateExtender.SetStateID(this.finishState, "Finish");
            // 
            // State_Extender
            // 
            this.mStateExtender.InitialState = "Start";
            ((System.ComponentModel.ISupportInitialize)(this.mStateExtender)).EndInit();
        }
        #endregion // InitializeComponents()

        #region ProcessRequestInternalCallback
        /// <summary>
        /// This override identifies that the request originates from within the CDS. 
        /// This is used when Persistence Managers call back using the CDSDirect class.
        /// </summary>
        /// <param name="job">The current job.</param>
        /// <param name="Data">The data.</param>
        protected virtual void ProcessRequestInternalCallback(SecurityManagerJob job, 
            RQRSContract<CDSRequestFolder, CDSResponseFolder> Data)
        {
            Data.ContractRequest.InternalCall = true;
            ProcessRequest(job, Data);
        }
        #endregion // ProcessRequestInternalCallback
        #region ProcessRequest --> MAIN ENTRY POINT
        /// <summary>
        /// This is the main entry point for a CDS request.
        /// </summary>
        /// <param name="context">The job context to process.</param>
        protected override void ProcessRequest(CDSContext context)
        {
            if (ProcessRequestIsSubCommand(context))
            {
                ProcessRequestSubCommand(context);
                return;
            }

            CDSAction action = context.CDSStateActionResolve();
            if (action == CDSAction.Cache || 
                (action == CDSAction.ResolveReference && !context.Request.InternalCall))
            {
                //OK, the request could not be resolved.
                context.Response.Status = CH.HTTPCodes.BadRequest_400;
                context.Response.Substatus = "The requested action is only supported internally.";
                return;
            }

            try
            {
                try
                {
                    //Initialize the request and set the initial state.
                    context.Initialize();

                    //Special case for internal Resolve Reference requests.
                    if (action == CDSAction.ResolveReference && context.Request.InternalCall)
                    {
                        if (!context.CDSStateProcessDirective(CDSAction.ResolveReference))
                        {
                            //OK, the request could not be resolved.
                            context.Response.Status = CH.HTTPCodes.NotImplemented_501;
                            context.Response.Substatus = "The action is not supported.";
                        }
                        return;
                    }

                    //If the request is by reference, then resolve the reference before continuing.
                    if (context.RequestIsByReference)
                        if (!context.CDSStateProcessDirective(CDSAction.ResolveReference))
                        {
                            ProcessRequestFailed(CH.HTTPCodes.NotFound_404,
                                string.Format("Reference cannot be resolved: {0}/{1}", context.Request.DataReferenceType, context.Request.DataReferenceValue), 
                                context.Job, context.Data);
                            return;
                        }

                    //OK, we need to check whether we have all the necessary IDs for the request.
                    //If we do not then we cannot use the cache.
                    if (context.ContentIsCacheable && action == CDSAction.Read)
                    {
                        if (!context.Request.DataVersionID.HasValue || !context.Request.DataContentID.HasValue)
                        {
                            Guid? vid, cid;
                            CDSResponse status = context.Job.CDSExecute(
                                context.Request.DataType, CDSData.Get(CDSAction.VersionCheck,
                                    context.Request.DataContentID, context.Request.DataVersionID), 
                                        out cid, out vid);

                            if (status == CDSResponse.OK)
                            {
                                context.Request.DataContentID = cid;
                                context.Request.DataVersionID = vid;
                            }
                            //context.CDSStateProcessDirective(CDSStateAction.VersionCheck);
                            context.ContentIsCacheable = context.Request.DataVersionID.HasValue && context.Request.DataContentID.HasValue;
                        }
                    }
                    else
                        context.ContentIsCacheable = false;

                    //Execute the CRUD command.
                    if (!context.CDSStateProcessDirective(action))
                    {
                        //OK, the request could not be resolved.
                        context.Response.Status = CH.HTTPCodes.NotImplemented_501;
                        context.Response.Substatus = "The action is not supported.";
                    }

                    //Can the resulting data be cached after the operation.
                    if (context.ContentIsCacheable && 
                        context.Response.Status == CH.HTTPCodes.OK_200)
                        context.CDSStateProcessDirective(CDSAction.Cache);
                }
                catch (NotImplementedException niex)
                {
                    //This error will be thrown if there is a problem with the execution path.
                    ProcessRequestFailed(CH.HTTPCodes.NotImplemented_501, niex.Message, context.Job, context.Data);
                    return;
                }

                context.Finish();
            }
            catch (ContentDataStoreException cdsEx)
            {
                ProcessRequestFailed(cdsEx.ResponseCode.ToString(), cdsEx.Message, context.Job, context.Data);
            }
            catch (Exception ex)
            {
                ProcessRequestFailed(CH.HTTPCodes.InternalServerError_500, ex.Message, context.Job, context.Data);
            }
        }
        #endregion

        #region SubCommands
        #region ProcessRequestIsSubCommand(CDSContext context)
        /// <summary>
        /// This method returns true if the request is a subcommand.
        /// </summary>
        /// <param name="context">the current job context.</param>
        /// <returns></returns>
        protected virtual bool ProcessRequestIsSubCommand(CDSContext context)
        {
            EnvelopeAddress address = context.Data.DestinationAddress;

            return address.SubCommand is ContentDataStoreSubCommand;
        }
        #endregion // ProcessRequestIsSubCommand(CDSContext context)
        #region ProcessRequestSubCommand(CDSContext context)
        /// <summary>
        /// This method processes sny specific subcommands sent to the Content Data Store.
        /// </summary>
        /// <param name="context">The current job context.</param>
        protected virtual void ProcessRequestSubCommand(CDSContext context)
        {
            EnvelopeAddress address = context.Data.DestinationAddress;
            ContentDataStoreSubCommand subCommand = (ContentDataStoreSubCommand)address.SubCommand;

            switch (subCommand)
            {
                case ContentDataStoreSubCommand.CacheManagersPoll:
                    PRSC_CacheManagerPoll(context);
                    return;
            }

            context.Response.Status = CH.HTTPCodes.MethodNotAllowed_405;
        }
        #endregion // ProcessRequestSubCommand(CDSContext context)
        #region PRSC_CacheManagerPoll(CDSContext context)
        /// <summary>
        /// This method polls the cache managers.
        /// </summary>
        /// <param name="context"></param>
        protected virtual void PRSC_CacheManagerPoll(CDSContext context)
        {
            mCacheManagerBridge.Poll();
            context.Response.Status = CH.HTTPCodes.OK_200;
        }
        #endregion // PRSC_CachePoll(CDSContext context)
        #endregion // SubCommands

        #region ContextInitialize
        protected override void ContextInitialize(CDSContext newContext,
            SecurityManagerJob job, RQRSContract<CDSRequestFolder, CDSResponseFolder> Data)
        {
            //Reset the context with the job and the data, as well as the CDS direct access method.
            newContext.Reset(ContextConnection, job, Data, 
                mRemoteContextPoolAccess, ProcessRequestInternalCallback);
        }
        #endregion // ContextInitialize
        #region ProcessRequestFailed
        /// <summary>
        /// This override should place the appropriate HTML error response in the response object.
        /// </summary>
        /// <param name="status">The status code.</param>
        /// <param name="subStatus">The status description.</param>
        /// <param name="job">The job.</param>
        /// <param name="Data">The job data.</param>
        protected override void ProcessRequestFailed(string status, string subStatus, SecurityManagerJob job, RQRSContract<CDSRequestFolder, CDSResponseFolder> Data)
        {
            XimuraAppTrace.WriteLine(status + " - " + subStatus, this.CommandName, EventLogEntryType.Warning);
            base.ProcessRequestFailed(status, subStatus, job, Data);
        }
        #endregion // ProcessRequestFailed

        #region InternalStart()
        /// <summary>
        /// This overriden method starts the protocols
        /// </summary>
        protected override void InternalStart()
        {
            base.InternalStart();
            //This method verifies connectivity to any remote databases. If any of the connections cannot
            //be made, an error is raised and the service will not start.
            VerifyExternalConnectivity();


            //Start the persistence managers and then the cache managers
            //ComponentsStatusChange(
            //    XimuraServiceStatusAction.Start, ServiceComponents, typeof(ICDSState));

            //Record the component status in the event log.
            ServiceStatusChange(XimuraServiceStatusAction.Start);
        }
        #endregion // InternalStart()
        #region InternalStop()
        /// <summary>
        /// This overriden method stops the protocols
        /// </summary>
        protected override void InternalStop()
        {
            ServiceStatusChange(XimuraServiceStatusAction.Stop);

            //ComponentsStatusChange(
            //    XimuraServiceStatusAction.Stop, ServiceComponents, typeof(ICDSState));


            base.InternalStop();
        }
        #endregion // InternalStop()
        #region InternalPause()
        /// <summary>
        /// This overriden pauses stops the RBP protocol
        /// </summary>
        protected override void InternalPause()
        {
            ServiceStatusChange(XimuraServiceStatusAction.Pause);

            ComponentsStatusChange(
                XimuraServiceStatusAction.Pause, ServiceComponents, typeof(ICDSState));

            base.InternalPause();
        }
        #endregion // InternalPause()
        #region InternalContinue()
        /// <summary>
        /// This overriden resumes the protocols after they have been paused.
        /// </summary>
        protected override void InternalContinue()
        {
            base.InternalContinue();

            ComponentsStatusChange(
                XimuraServiceStatusAction.Continue, ServiceComponents, typeof(ICDSState));

            ServiceStatusChange(XimuraServiceStatusAction.Continue);
        }
        #endregion // InternalContinue()

        #region StatesStart()
        /// <summary>
        /// This override starts the storage persistence states.
        /// </summary>
        protected override void StatesStart()
        {
            //Ensure that the storage is started before the states.
            ComponentsStatusChange(XimuraServiceStatusAction.Start, ServiceComponents, typeof(IXimuraStorage));

            base.StatesStart();
        }
        #endregion 
        #region StatesStop()
        /// <summary>
        /// This override stops the storage persistence states.
        /// </summary>
        protected override void StatesStop()
        {
            base.StatesStop();

            //Ensure that the storage is stopped after the states have stopped.
            ComponentsStatusChange(XimuraServiceStatusAction.Stop, ServiceComponents, typeof(IXimuraStorage));

        }
        #endregion 

        #region ServiceStatusChange
        /// <summary>
        /// This method is used to announce detailed announcements on service status changes.
        /// </summary>
        /// <param name="status">The protocol status, i.e. start, stop etc.</param>
        protected virtual void ServiceStatusChange(XimuraServiceStatusAction status)
        {
            switch (status)
            {
                case XimuraServiceStatusAction.Start:
                    XimuraAppTrace.WriteLine("CDS started.",
                        this.AppCommandAttribute.Name, EventLogEntryType.Information);
                    break;
                case XimuraServiceStatusAction.Stop:
                    XimuraAppTrace.WriteLine("CDS stopped.",
                        this.AppCommandAttribute.Name, EventLogEntryType.Information);
                    break;
                case XimuraServiceStatusAction.Pause:
                    XimuraAppTrace.WriteLine("CDS paused.",
                        this.AppCommandAttribute.Name, EventLogEntryType.Information);
                    break;
                case XimuraServiceStatusAction.Continue:
                    XimuraAppTrace.WriteLine("CDS resumed.",
                        this.AppCommandAttribute.Name, EventLogEntryType.Information);
                    break;
            }
        }
        #endregion // ProtocolStatusChange

        #region VerifyExternalConnectivity()
        /// <summary>
        /// This method verifies all the external connections.
        /// </summary>
        protected virtual void VerifyExternalConnectivity()
        {
            //ICDSConfigSH cdsSettings = CommandSettings as ICDSConfigSH;
            //if (cdsSettings == null)
            //    throw new DataException("No CDS Settings has been found.");

            //if (!cdsSettings.SkipConnectivityTest)
            //{
            //    Dictionary<string, string> htConnectionString = cdsSettings.GetAllConnectionStrings();

            //    foreach (string sqlConnStr in htConnectionString.Keys)
            //    {
            //        using (SqlConnection sqlConn = new SqlConnection(htConnectionString[sqlConnStr]))
            //        {
            //            try
            //            {
            //                sqlConn.Open();
            //            }
            //            catch (SqlException sqlEx)
            //            {
            //                XimuraAppTrace.WriteLine("Cannot connect to the database for connection: " + sqlConnStr,
            //                    "VerifyExternalConnectivity", EventLogEntryType.Error);
            //                throw sqlEx;
            //            }
            //            catch (Exception ex)
            //            {
            //                throw ex;
            //            }
            //            finally
            //            {
            //                sqlConn.Close();
            //            }
            //        }
            //    }
            //}
        }
        #endregion // VerifyExternalConnectivity()

        #region Services
        #region ServicesProvide()
        /// <summary>
        /// This override add the CDS IXimuraCDSSettingsService settings object.
        /// </summary>
        protected override void ServicesProvide()
        {
            base.ServicesProvide();

            //AddService(typeof(ICDSCacheManagerBridge), mCacheManagerBridge, false);

            //ICDSConfigSH cdsSettings = CommandSettings as ICDSConfigSH;
            //if (cdsSettings == null)
            //    return;

            //AddService(typeof(ICDSConfigSH), cdsSettings, false);

            //if (cdsSettings is ICDSSettingsService)
            //    AddService(typeof(ICDSSettingsService), CommandSettings as ICDSSettingsService, true);
        }
        #endregion // ServicesProvide()
        #region ServicesRemove()
        /// <summary>
        /// This override removes the IXimuraCDSSettingsService settings object
        /// </summary>
        protected override void ServicesRemove()
        {
            //ICDSConfigSH cdsSettings = CommandSettings as ICDSConfigSH;
            //if (cdsSettings != null)
            //{
            //    if (cdsSettings is ICDSSettingsService)
            //        RemoveService(typeof(ICDSSettingsService));

            //    RemoveService(typeof(ICDSConfigSH));
            //}

            //RemoveService(typeof(ICDSCacheManagerBridge));

            base.ServicesRemove();
        }
        #endregion // ServicesRemove()
        #endregion // Services

        #region InitializeContextConnection(IContainer baseContainer)
        /// <summary>
        /// This method should be overriden to provide extended Context collection settings.
        /// </summary>
        /// <param name="baseContainer">The base container for the collection.</param>
        protected override void InitializeContextConnection(IContainer baseContainer)
        {
            //mContextConnection = new ContextSettings<ST>(mStateExtender) as SET;
            mContextSettings = new CDSSettings();
            mContextSettings.InitializeSettings(
                CommandDefinition, 
                ApplicationDefinition,
                mStateExtender, 
                PoolManager, 
                EnvelopeHelper,
                mSessionMan,
                mProcessSession,Configuration,Performance, 
                mCacheManagerBridge);
        }
        #endregion // InitializeContextConnection(IContainer baseContainer)
    }
}
