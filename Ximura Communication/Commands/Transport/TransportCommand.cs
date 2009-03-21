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
﻿#region using
using System;
using System.Threading;
using System.Timers;
using System.Data;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Reflection;
using System.Drawing;
using System.Diagnostics;
using System.Runtime.Serialization;

using Ximura;
using Ximura.Data;
using Ximura.Helper;
using CH = Ximura.Helper.Common;
using Ximura.Server;
using Ximura.Command;

#endregion // using
namespace Ximura.Communication
{
    /// <summary>
    /// This command provides the base functionality for all transport providers.
    /// </summary>
    /// <typeparam name="CNTX">The transport context. This is customized for the specific type of transport.</typeparam>
    public class TransportCommand<CNTX> :
        ConnectionFSMBase<TransportCommandRequest, TransportCommandResponse, TransportCommandCBRequest, TransportCommandCBResponse,
            CNTX, TransportState, TransportSettings, TransportConfiguration, TransportPerformance>
        where CNTX : TransportContext, new()
    {
        #region Declarations
        private System.ComponentModel.IContainer components = null;
        /// <summary>
        /// This is the start state.
        /// </summary>
        protected StartTransportState startState;
        #endregion // Declarations
        #region Constructors
        /// <summary>
        /// Empty constructor for use by the component model.
        /// </summary>
        public TransportCommand() : this(null) { }
        /// <summary>
        /// Default constructor for use by the component model.
        /// </summary>
        /// <param name="container">The container that the transport should be added to.</param>
        public TransportCommand(System.ComponentModel.IContainer container)
            : base(container)
        {
            InitializeComponent();
            RegisterContainer(components);
        }
        #endregion

        #region InitializeComponent()
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();

            startState = new StartTransportState(components);

            ((System.ComponentModel.ISupportInitialize)(this.mStateExtender)).BeginInit();
            // 
            // startState
            // 
            this.mStateExtender.SetEnabled(this.startState, true);
            this.mStateExtender.SetNextStateID(this.startState, null);
            this.mStateExtender.SetStateID(this.startState, "Start");
            // 
            // State_Extender
            // 
            this.mStateExtender.InitialState = "Start";
            ((System.ComponentModel.ISupportInitialize)(this.mStateExtender)).EndInit();
        }
        #endregion


        #region ProcessRequest --> MAIN ENTRY POINT HERE
        /// <summary>
        /// This method processes the requests from the protocol servers.
        /// </summary>
        /// <param name="job">The incoming job.</param>
        /// <param name="Data">The incoming data.</param>
        protected override void ProcessRequest(SecurityManagerJob job, RQRSContract<TransportCommandRequest, TransportCommandResponse> Data)
        {
            TransportCommandRequest Request = Data.ContractRequest;
            TransportCommandResponse Response = Data.ContractResponse;

            //Retrieve the listening location
            EnvelopeAddress? address = Data.ContractRequest.ServerAddress;
            Guid? connID = Request.TransportContextID;
            CNTX context = null;

            if (connID.HasValue)
            {
                context = ContextResolve(connID.Value);
                if (context == null)
                {
                    Data.Response.Status = CH.HTTPCodes.NotFound_404;
                    return;
                }
            }
            else
            {
                context = ContextGet();
                if (context == null)
                {
                    Data.Response.Status = CH.HTTPCodes.ServiceUnavailable_503;
                    return;
                }
                //Initialize the context with the server address.
                context.Initialize();
                EnvelopeAddress serverAddress = Data.ContractRequest.ServerAddress.Value;
                context.ServerCommandID = serverAddress.command;
            }

            Response.Status = CH.HTTPCodes.Continue_100;
            //This property determines whether the context will be reset and returned to the pool.
            //The default is true unless indicated otherwise by the context.
            bool resetContext = true;

            try
            {
                string subC = job.Data.DestinationAddress.SubCommand as string;

                switch (subC)
                {
                    case "ListenRequest":
                        resetContext = context.ListenRequest(job, Data);
                        break;
                    case "ListenConfirm":
                        resetContext = context.ListenConfirm(job, Data);
                        break;
                    case "OpenRequest":
                        resetContext = context.OpenRequest(job, Data);
                        break;
                    case "OpenConfirm":
                        resetContext = context.OpenConfirm(job, Data);
                        break;
                    case "Transmit":
                        resetContext = context.Transmit(job, Data);
                        break;
                    case "Close":
                        resetContext = context.Close(job, Data);
                        break;
                    default:
                        throw new TransportRequestException("Unknown subcommand: " + subC, CH.HTTPCodes.InternalServerError_500, "");
                }

                if (!resetContext)
                    Response.TransportContextID = context.SignatureID;
            }
            catch (TransportRequestException vsdex)
            {
                XimuraAppTrace.WriteLine("Unexpected protocol error: " + vsdex.Message + Environment.NewLine + vsdex.ToString(),
                    this.CommandName + " -> ProcessRequest", EventLogEntryType.Error, this.CommandName);
                Response.Status = vsdex.Status;
                Response.Substatus = "(" + vsdex.SubStatus + ") " + vsdex.Message + Environment.NewLine + vsdex.ToString();
                resetContext = true;
            }
            catch (Exception ex)
            {
                XimuraAppTrace.WriteLine("Unexpected protocol error: " + ex.ToString(),
                    this.CommandName + " -> ProcessRequest", EventLogEntryType.Error, this.CommandName);
                Response.Status = CH.HTTPCodes.InternalServerError_500;
                Response.Substatus = ex.Message;
                resetContext = true;
            }
            finally
            {
                if (resetContext && context != null)
                    ContextReturn(context);
            }
        }
        #endregion // ProcessRequest(SecurityManagerJob job)

        #region ContextReset(CNTX context)
        /// <summary>
        /// This method resets the session with the settings and the base command session.
        /// </summary>
        /// <param name="context">The context to reset.</param>
        protected override void ContextReset(CNTX context)
        {
            context.Reset(ContextConnection, this.mProcessSession, mRemoteContextPoolAccess);
        }
        #endregion // ContextReset(CNTX context)


        #region ContextReturn(CNTX context)
        /// <summary>
        /// This method unregisters a context and returns it to the pool.
        /// </summary>
        protected override void ContextReturn(CNTX context)
        {
            //We will initiate a call back to the server to inform it that the context has been reset.
            if (context == null)
                return;

            base.ContextReturn(context);
        }
        #endregion // ContextReturn(CNTX context)


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
                List<CNTX> closed = null;

                lock (syncObject)
                {
                    DateTime now = DateTime.Now;
                    closed = mContexts.Values
                        .Where(cx => cx.ClosePending)
                        .ToList();
                }

                //We move this in to a queue as close operations will modify the underlying mContexts collection.
                foreach (var item in closed)
                {
                    ContextReturn(item);
                }
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
 
    }
}
