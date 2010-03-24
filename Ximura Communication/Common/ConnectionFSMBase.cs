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
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Reflection;
using System.Drawing;
using System.Diagnostics;
using System.Collections.Generic;

using Ximura;

using CH = Ximura.Common;
using Ximura.Framework;
using Ximura.Framework;

#endregion // using
namespace Ximura.Communication
{
    /// <summary>
    /// This is the base command for connection based commands such as ProtocolCommand and ServerCommand. 
    /// It is used to hold common code.
    /// </summary>
    /// <typeparam name="RQ"></typeparam>
    /// <typeparam name="RS"></typeparam>
    /// <typeparam name="CBRQ"></typeparam>
    /// <typeparam name="CBRS"></typeparam>
    /// <typeparam name="CNTX"></typeparam>
    /// <typeparam name="ST"></typeparam>
    /// <typeparam name="SET"></typeparam>
    public class ConnectionFSMBase<RQ, RS, CBRQ, CBRS, CNTX, ST,SET, CONF, PERF> :
        FiniteStateMachine<RQ, RS, CBRQ, CBRS, CNTX, ST, SET, CONF, PERF>
        where RQ : RQRSFolder, new()
        where RS : RQRSFolder, new()
        where CBRQ : RQRSFolder, new()
        where CBRS : RQRSFolder, new()
        where CNTX : class, IXimuraFSMContext, new()
        where ST : class,IXimuraFSMState
        where SET : class, IXimuraFSMSettings<ST, CONF, PERF>, new()
        where CONF : FSMCommandConfiguration, new()
        where PERF : FSMCommandPerformance, new()
    {
        #region Declarations
        private readonly Guid mTrackID = Guid.NewGuid();
        /// <summary>
        /// This is a list of the active protocol contexts listed by their unique identifier.
        /// </summary>
        protected Dictionary<Guid, CNTX> mContexts = new Dictionary<Guid, CNTX>();

        protected object syncObject = new object();
        #endregion // Declarations
        #region Constructors
        /// <summary>
        /// Empty constructor for use by the component model.
        /// </summary>
        public ConnectionFSMBase() : this(null) { }
        /// <summary>
        /// Default constructor for use by the component model.
        /// </summary>
        /// <param name="container">The container that the transport should be added to.</param>
        public ConnectionFSMBase(System.ComponentModel.IContainer container)
            : base(container)
        {
        }
        #endregion
        #region InternalStart()/InternalStop()
        /// <summary>
        /// This override add specific context support.
        /// </summary>
        protected override void InternalStart()
        {
            base.InternalStart();
        }
        /// <summary>
        /// This override removes the context collection.
        /// </summary>
        protected override void InternalStop()
        {
            lock (syncObject)
            {
                if (mContexts != null)
                    mContexts.Clear();
                mContexts = null;
            }
            base.InternalStop();
        }
        #endregion // InternalStart/Stop

        #region ContextGet()
        protected override CNTX ContextGetContext()
        {
            return ContextGet();
        }

        /// <summary>
        /// This method gets a new protocol context and set its security to run under the process session of the command.
        /// </summary>
        /// <returns>The new context object.</returns>
        protected virtual CNTX ContextGet()
        {
            try
            {
                lock (syncObject)
                {
                    CNTX context = ContextPool.Get();
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
        protected override void ContextReturnContext(CNTX context)
        {
            ContextReturn(context);
        }
        /// <summary>
        /// This method unregisters a context and returns it to the pool.
        /// </summary>
        protected virtual void ContextReturn(CNTX context)
        {
            try
            {
                lock (syncObject)
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
        #region ContextReset(CNTX context)
        /// <summary>
        /// This method resets the session with the settings and a new empty session.
        /// </summary>
        /// <param name="context"></param>
        protected virtual void ContextReset(CNTX context)
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
        protected virtual CNTX ContextResolve(Guid connID)
        {
            lock (syncObject)
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
            if (this.ParentCommandName !=null && this.ParentCommandName !="")
                service.ParentCommandName = this.ParentCommandName + '/' + this.CommandName;
            else
                service.ParentCommandName = this.CommandName;
        }
        #endregion // ServiceParentSettingsSet(IXimuraService service)
    }
}
