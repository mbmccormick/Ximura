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
using Ximura.Persistence;
using CH = Ximura.Helper.Common;
using Ximura.Server;

using Ximura.Command;

#endregion // using
namespace Ximura.Command
{
    /// <summary>
    /// The context settings class hold a collection of the settings that the context will use.
    /// </summary>
    /// <typeparam name="ST">The state type.</typeparam>
    public class ContextSettings<ST, CONF, PERF> : IXimuraFSMSettings<ST, CONF, PERF>
        where ST : class,IXimuraFSMState
        where CONF : FSMCommandConfiguration, new()
        where PERF : FSMCommandPerformance, new()
    {
        #region Declarations
        private CONF mConfig;
        private PERF mPerf;
        /// <summary>
        /// This is the state extender.
        /// </summary>
        protected StateExtender<ST> mStateExtender = null;

        private CDSHelper mCDSHelper;

        private IXimuraApplicationDefinition baseApplication;
        private IXimuraCommand baseCommand;
        private IXimuraPoolManager mPoolManager;
        private IXimuraSessionManager mSessionManager;
        /// <summary>
        /// This is the process session that the base FSM command operates under.
        /// </summary>
        private IXimuraSession mProcessSession;
        #endregion // Declarations
        #region Constructors
        /// <summary>
        /// This constructor is called by the FSM when initiating the settings.
        /// </summary>
        public ContextSettings()
        {
            mStateExtender = null;
            mCDSHelper = new CDSHelper(null);
        }
        #endregion

        #region GetInitialState()
        /// <summary>
        /// This method returns the initial state.
        /// </summary>
        /// <returns>The initial state, or null if no initial state has been set.</returns>
        public ST GetInitialState()
        {
            return mStateExtender.GetInitialState();
        }
        #endregion // GetInitialState()
        #region GetState(string State)
        /// <summary>
        /// This method returns the state for the string based identifier that is passed as a parameter.
        /// </summary>
        /// <param name="State">The state identifier.</param>
        /// <returns>The state or null if the identifier is not recognised.</returns>
        public ST GetState(string State)
        {
            return mStateExtender.GetState(State);
        }
        #endregion // GetState(string State)
        #region GetStateName(ST CurrentState)
        /// <summary>
        /// This method returns the state string identifier.
        /// </summary>
        /// <param name="CurrentState">The state object.</param>
        /// <returns>The state identifier.</returns>
        public string GetStateName(ST CurrentState)
        {
            return mStateExtender.GetStateName(CurrentState);
        }
        #endregion // GetStateName(ST CurrentState)

        #region IXimuraCommand Members
        /// <summary>
        /// This is the base command ID.
        /// </summary>
        public Guid CommandID
        {
            get
            {
                return baseCommand.CommandID;
            }
        }
        /// <summary>
        /// This is the base command name.
        /// </summary>
        public string CommandName
        {
            get
            {
                return baseCommand.CommandName;
            }
            set
            {
                throw new NotImplementedException("The method or operation is not implemented.");
            }
        }
        /// <summary>
        /// This is the base command description.
        /// </summary>
        public string CommandDescription
        {
            get
            {
                return baseCommand.CommandDescription;
            }
            set
            {
                throw new NotImplementedException("The method or operation is not implemented.");
            }
        }

        #endregion
        #region PoolManager
        /// <summary>
        /// This is the pool manager for the application.
        /// </summary>
        public IXimuraPoolManager PoolManager
        {
            get
            {
                return mPoolManager;
            }
        }
        #endregion // PoolManager
        #region SessionManager
        /// <summary>
        /// This is the session manager for the FSM application.
        /// </summary>
        public IXimuraSessionManager SessionManager
        {
            get { return this.mSessionManager; }
        }
        #endregion

        public virtual void SignalCompletion(IXimuraFSMContext context)
        {

        }

        #region IXimuraApplicationDefinition Members
        /// <summary>
        /// The base application name.
        /// </summary>
        public string ApplicationName
        {
            get { return baseApplication.ApplicationName; }
        }
        /// <summary>
        /// The base application ID.
        /// </summary>
        public Guid ApplicationID
        {
            get { return baseApplication.ApplicationID; }
        }
        /// <summary>
        /// The application description.
        /// </summary>
        public string ApplicationDescription
        {
            get { return baseApplication.ApplicationDescription; }
        }
        #endregion

        #region IXimuraFSMSettings<ST> Members
        /// <summary>
        /// This method is used to initialize the FSM settings object.
        /// </summary>
        /// <param name="baseCommand">The base command information.</param>
        /// <param name="baseApplication">The base application information.</param>
        /// <param name="extender">The state extender.</param>
        /// <param name="poolManager">The pool manager.</param>
        /// <param name="sessionManager">The seesion manager.</param>
        /// <param name="processSession">The current process session that the command is running under.</param>
        public virtual void InitializeSettings(IXimuraCommand baseCommand, 
            IXimuraApplicationDefinition baseApplication,
            StateExtender<ST> extender, 
            IXimuraPoolManager poolManager, 
            IXimuraSessionManager sessionManager, 
            IXimuraSession processSession,
            CONF config,
            PERF perf
            )
        {
            this.mProcessSession = processSession;
            this.baseCommand = baseCommand;
            this.baseApplication = baseApplication;
            this.mStateExtender = extender;
            this.mPoolManager = poolManager;
            this.mSessionManager = sessionManager;
            mCDSHelper.Session = processSession;

            this.mConfig = config;
            this.mPerf = perf;
        }
        #endregion

        #region CDSHelper
        /// <summary>
        /// This is the CDS Helper for the process session object.
        /// </summary>
        protected virtual CDSHelper CDSHelper
        {
            get { return mCDSHelper; }
        }
        #endregion // CDSHelper

        #region DomainDefault
        /// <summary>
        /// This string is the default domin used for context sessions when a specific domain is not specified.
        /// </summary>
        public string DomainDefault
        {
            get 
            {
                //if (mFSMSettings == null  || !(mFSMSettings is FSMConfigSH))
                //    return null;
                //return ((FSMConfigSH)mFSMSettings).UserSessionRealm;

                return null;
            }
        }
        #endregion

        #region StateCollection()
        /// <summary>
        /// This method returs a collection of states.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ST> StateCollection()
        {
            string[] ids = mStateExtender.GetStateIDList();
            foreach (string id in ids)
            {
                yield return GetState(id);
            }
        }
        #endregion

        #region ProcessSession
        /// <summary>
        /// This is the process session used by the base command. This session may be used to submit requests for
        /// commands that do not use user sessions.
        /// </summary>
        public IXimuraSession ProcessSession
        {
            get { return mProcessSession; }
        }
        #endregion // ProcessSession

        #region Configuration
        /// <summary>
        /// This is the configuration object.
        /// </summary>
        public CONF Configuration
        {
            get { return mConfig; }
        }
        #endregion // Configuration

        #region Performance
        /// <summary>
        /// This is the performance object.
        /// </summary>
        public PERF Performance
        {
            get { return mPerf; }
        }
        #endregion // Performance

        /// <summary>
        /// This method resolves the SQL connection string.
        /// </summary>
        /// <param name="connID"></param>
        /// <returns></returns>
        public virtual string ResolveConnectionString(string connID)
        {
            throw new NotImplementedException("ResolveConnectionString is not implemented.");
        }
    }
}
