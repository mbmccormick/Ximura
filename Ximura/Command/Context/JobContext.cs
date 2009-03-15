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
using System.Collections;
using System.Diagnostics;
using System.Data;
using System.Reflection;
using System.ComponentModel;
using System.Collections.Generic;

using Ximura;
using Ximura.Data;
using CH = Ximura.Helper.Common;
using Ximura.Helper;
using Ximura.Server;


using Ximura.Command;
#endregion
namespace Ximura.Command
{
    /// <summary>
    /// This context is used by finite state machines that require application support.
    /// </summary>
    public class JobContext<ST, SET, RQ, RS, CONF, PERF> : 
        Context<ST, SET, CONF, PERF>, IXimuraJobContext<RQ, RS, CONF, PERF>
        where ST : class,IXimuraFSMState
        where SET : class, IXimuraFSMSettings<ST, CONF, PERF>, new()
        where RQ : RQRSFolder, new()
        where RS : RQRSFolder, new()
        where CONF : FSMCommandConfiguration, new()
        where PERF : FSMCommandPerformance, new()
    {
        #region Declarations
        /// <summary>
        /// The job for the request.
        /// </summary>
        private SecurityManagerJob mJob;

        private RQRSContract<RQ, RS> mData = null;

        #endregion
        #region Constructor
        /// <summary>
        /// This is the default context for the PPC.
        /// </summary>
        public JobContext()
            : base()
        {
        }
        #endregion

        #region Job
        /// <summary>
        /// The job.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual SecurityManagerJob Job
        {
            get
            {
                return mJob;
            }
            set
            {
                mJob = value;
            }
        }
        #endregion
        #region Destination
        /// <summary>
        /// The envelope data destination.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual EnvelopeAddress Destination
        {
            get
            {
                return mJob.Data.DestinationAddress;
            }
        }
        #endregion

        #region Data Shortcut
        /// <summary>
        /// This is the job envelope.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual RQRSContract<RQ, RS> Data
        {
            get
            {
                return mData;
            }
        }
        #endregion // Envelope Data Shortcut

        #region Request shortcut
        /// <summary>
        /// This is the job request.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual RQ Request
        {
            get
            {
                return Data.ContractRequest;
            }
        }
        #endregion // Request shortcut
        #region Response shortcut
        /// <summary>
        /// This is the job response.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual RS Response
        {
            get
            {
                return Data.ContractResponse;
            }
        }
        #endregion // Response shortcut

        #region ChildJobPriority
        /// <summary>
        /// This property determines the child job priorities
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public JobPriority ChildJobPriority
        {
            get
            {
                if (Job.Priority < JobPriority.Normal)
                    return Job.Priority;

                return JobPriority.AboveNormal;
            }
        }
        #endregion

        #region Reset
        /// <summary>
        /// This method rests the context
        /// </summary>
        public override void Reset()
        {
            base.Reset();
            mJob = null;
            mData = null;
        }
        #endregion
        #region Reset(ContextSettings<ST> fsm, SecurityManagerJob job)
        /// <summary>
        /// This method resets a connection and sets the connection state to ClosedRBPState.
        /// </summary>
        /// <param name="fsm">A reference to the finite state machine.</param>
        /// <param name="job">The job request.</param>
        public virtual void Reset(IXimuraFSMSettingsBase fsm, SecurityManagerJob job, IXimuraFSMContextPoolAccess contextGet)
        {
            RQRSContract<RQ, RS> data = null;

            if (job != null)
                data = (RQRSContract<RQ, RS>)job.Data;

            this.Reset(fsm, job, data, contextGet);
        }
        #endregion // Reset(SET fsm, SecurityManagerJob job)
        #region Reset(IXimuraFSMSettingsBase fsm, SecurityManagerJob job,RQRSContract<RQ, RS> data, IXimuraFSMContextPoolAccess contextGet)
        /// <summary>
        /// This method resets a connection and sets the connection state to ClosedRBPState.
        /// </summary>
        /// <param name="fsm">A reference to the finite state machine.</param>
        /// <param name="job">The job request.</param>
        public virtual void Reset(IXimuraFSMSettingsBase fsm, SecurityManagerJob job,
            RQRSContract<RQ, RS> data, IXimuraFSMContextPoolAccess contextGet)
        {
            base.Reset(fsm, (IXimuraSessionRQ)job, contextGet);
            this.Job = job;
            mData = data;
        }
        #endregion

    }
}
