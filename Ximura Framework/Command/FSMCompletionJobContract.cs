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
using CH = Ximura.Common;
using Ximura.Framework;

using Ximura.Framework;

#endregion // using
namespace Ximura.Framework
{
    /// <summary>
    /// This overriden FSM is for processes that require Completion job support.
    /// </summary>
    /// <typeparam name="RQ">The incoming request type.</typeparam>
    /// <typeparam name="RS">The outgoing response type.</typeparam>
    /// <typeparam name="CBRQ">The callback request type.</typeparam>
    /// <typeparam name="CBRS">The callback response type.</typeparam>
    /// <typeparam name="CNTX">The context type.</typeparam>
    /// <typeparam name="ST">The FSM state type.</typeparam>
    /// <typeparam name="SET">The FSM Context Settings.</typeparam>
    public class FSMCompletionJobContract<RQ, RS, CBRQ, CBRS, CNTX, ST, SET, CONF, PERF> :
        FiniteStateMachine<RQ, RS, CBRQ, CBRS, CNTX, ST, SET, CONF, PERF>
        where RQ : RQRSFolder, new()
        where RS : RQRSFolder, new()
        where CBRQ : RQRSFolder, new()
        where CBRS : RQRSFolder, new()
        where CNTX : class, IXimuraJobContext<RQ, RS, CONF, PERF>, new()
        where ST : class,IXimuraFSMState
        where SET : class, IXimuraFSMSettings<ST, CONF, PERF>, new()
        where CONF : FSMCommandConfiguration, new()
        where PERF : FSMCommandPerformance, new()
    {
        #region Declarations
        /// <summary>
        /// This is the job completion callback delegate.
        /// </summary>
        private CompletionJobCallBack jobFSMCompleteCallBack; 
        #endregion // Declarations
        #region Constructors
        /// <summary>
        /// Empty constructor for use by the component model.
        /// </summary>
        public FSMCompletionJobContract() : this(null) { }
        /// <summary>
        /// Default constructor for use by the component model.
        /// </summary>
        /// <param name="container">The container that the transport should be added to.</param>
        public FSMCompletionJobContract(System.ComponentModel.IContainer container)
            : base(container)
        {
            jobFSMCompleteCallBack = new CompletionJobCallBack(internalOnFSMCompletionJobComplete);
        }

        /// <summary>
        /// This is the base constructor for a Ximura command
        /// </summary>
        /// <param name="commandID">This is the explicitly set command id, leave this as null if you want to use the default id.</param>
        /// <param name="container">The container to be added to</param>
        public FSMCompletionJobContract(Guid? commandID, System.ComponentModel.IContainer container)
            :
        base(commandID, container)
        {
            jobFSMCompleteCallBack = new CompletionJobCallBack(internalOnFSMCompletionJobComplete);
        }
        #endregion

        #region ContextInitialize(CNTX newContext, SecurityManagerJob job, RQRSContract<RQ, RS> Data)
        /// <summary>
        /// This method allows generic operations to be performed on the new context.
        /// </summary>
        /// <param name="newContext">The new context for the incoming request.</param>
        /// <param name="job">The incoming job.</param>
        /// <param name="Data">The incoming generic data.</param>
        protected override void ContextInitialize(CNTX newContext, SecurityManagerJob job, RQRSContract<RQ, RS> Data)
        {
            //Set the context state
            newContext.Reset(ContextConnection, job, Data, null);
        }
        #endregion

        #region GetFSMCompletionJob
        ///// <summary>
        ///// This method is used to create a FSM callback for the context.
        ///// </summary>
        ///// <param name="context">The context to process.</param>
        ///// <returns>Returns a new completion job.</returns>
        //public override CompletionJob GetFSMCompletionJob(CNTX context)
        //{
        //    int throttleLimit = 0;
        //    string limit = CommandSettings.GetSetting("throttle");
        //    bool trace = CommandSettings.GetSetting("cjtrace") == "true";

        //    CompletionJob cJob;

        //    if (!int.TryParse(limit, out throttleLimit))
        //        throttleLimit = DEFAULTTHROTTLELIMIT;

        //    cJob = context.Job.CreateCompletionJob(jobFSMCompleteCallBack,
        //        (object)(new CJobSignature<CNTX, ST, RQ, RS, SET>(context)), true, throttleLimit);

        //    return cJob;
        //}
        #endregion // GetFSMCompletionJob
        #region OnFSMCompletionJobComplete
        /// <summary>
        /// This method is called when the job is complete.
        /// </summary>
        /// <param name="parentJob">The parentJob.</param>
        /// <param name="status">The completion status.</param>
        /// <param name="state">The object state.</param>
        private void internalOnFSMCompletionJobComplete(SecurityManagerJob parentJob,
            CompletionJobStatus status, object state)
        {
            try
            {
                CJobSignature<CNTX, ST, RQ, RS, SET, CONF, PERF> sig = state as 
                    CJobSignature<CNTX, ST, RQ, RS, SET, CONF, PERF>;
                if (sig.ValidateContext())
                    OnFSMCompletionJobComplete(sig.Context, parentJob, status);
                else
                    XimuraAppTrace.WriteLine("Completion job signatures do not match: " + sig.SignatureID.ToString(),
                        this.CommandName, EventLogEntryType.Error);

            }
            catch (Exception ex)
            {
                XimuraAppTrace.WriteLine("Unhandled Exception: " + ex.ToString(), this.CommandName, EventLogEntryType.Error);
            }
        }
        /// <summary>
        /// This virtual method should be overriden to capture callback responses.
        /// </summary>
        /// <param name="context">The job context.</param>
        /// <param name="status">The status of the completion job.</param>
        protected virtual void OnFSMCompletionJobComplete(CNTX context, SecurityManagerJob parentJob, CompletionJobStatus status)
        {

        }
        #endregion // OnFSMCompletionJobComplete
    }
}
