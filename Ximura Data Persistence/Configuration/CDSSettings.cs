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
using System.Threading;

using Ximura;
using Ximura.Data;

using CH = Ximura.Common;
using RH = Ximura.Reflection;
using Ximura.Framework;


using Ximura.Framework;
#endregion // using
namespace Ximura.Data
{
    /// <summary>
    /// The settings class is used by the CDS context and states to provide information for the States.
    /// </summary>
    public class CDSSettings : ContextSettings<ICDSState, CDSConfiguration, CDSPerformance>
    {
        #region Declarations
        private Dictionary<CDSStateAction, Dictionary<string, string[]>> mStateExecutionPlanCache;

        private ICDSCacheManagerBridge mCacheManagerBridge = null;

        /// <summary>
        /// This is the default timeout value for read operation for the CDSState Execution plan cache.
        /// </summary>
        protected const int TIMEOUTREAD = 100;
        /// <summary>
        /// This is the default timeout value for write operation to the CDSState Execution plan cache.
        /// </summary>
        protected const int TIMEOUTWRITE = 250;
        /// <summary>
        /// This readwriter lock is used to allow multiple threads to access the CDSState execution plan cache.
        /// </summary>
        protected ReaderWriterLock rwl = new ReaderWriterLock();
        #endregion // Declarations
        #region Constructors
        /// <summary>
        /// This constructor is called by the FSM when initiating the settings.
        /// </summary>
        public CDSSettings()
        {
            InitializeStateExecutionPlan();
        }
        #endregion
        #region InitializeStateExecutionPlan()
        /// <summary>
        /// This method initializes the state execution plan.
        /// </summary>
        protected virtual void InitializeStateExecutionPlan()
        {
            mStateExecutionPlanCache = new Dictionary<CDSStateAction, Dictionary<string, string[]>>();

            mStateExecutionPlanCache.Add(CDSStateAction.Cache, new Dictionary<string, string[]>());
            mStateExecutionPlanCache.Add(CDSStateAction.Create, new Dictionary<string, string[]>());
            mStateExecutionPlanCache.Add(CDSStateAction.Custom, new Dictionary<string, string[]>());
            mStateExecutionPlanCache.Add(CDSStateAction.Delete, new Dictionary<string, string[]>());
            mStateExecutionPlanCache.Add(CDSStateAction.Read, new Dictionary<string, string[]>());
            mStateExecutionPlanCache.Add(CDSStateAction.ResolveReference, new Dictionary<string, string[]>());
            mStateExecutionPlanCache.Add(CDSStateAction.Update, new Dictionary<string, string[]>());
            mStateExecutionPlanCache.Add(CDSStateAction.VersionCheck, new Dictionary<string, string[]>());
            mStateExecutionPlanCache.Add(CDSStateAction.Browse, new Dictionary<string, string[]>());
            mStateExecutionPlanCache.Add(CDSStateAction.Restore, new Dictionary<string, string[]>());
        }
        #endregion // InitializeStateExecutionPlan()

        #region ResolveExecutionPlan(CDSStateActions action, Type objectType)
        /// <summary>
        /// This method is used to resolve the CDSState execution plan for the required entity type and action.
        /// </summary>
        /// <param name="objectType">The entity object type.</param>
        /// <returns>Returns a string array containing the state name collection.</returns>
        public string[] ResolveExecutionPlan(CDSStateAction action, Type objectType)
        {
            try
            {
                //Ok, get a read lock.
                rwl.AcquireReaderLock(TIMEOUTREAD);

                string objectTypeReduced = objectType.FullName;
                try
                {
                    //This shouldn't happen, but it is best to check in case a new action is added and not supported.
                    if (!mStateExecutionPlanCache.ContainsKey(action))
                        throw new ArgumentOutOfRangeException("action", "The requested CDSStateAction is not supported.");

                    //If the data is already cached, then return the plan.
                    if (mStateExecutionPlanCache[action].ContainsKey(objectTypeReduced))
                        return mStateExecutionPlanCache[action][objectTypeReduced];

                    //No, then get the plan and attempt to update it. This may not work is the writer lock timeouts,
                    //but the execution plan will still be returned as it is generated each time a request is made before the 
                    //write lock is attempted.
                    return GetAndCacheExecutionPlan(action, objectType);
                }
                finally 
                {
                    rwl.ReleaseReaderLock();
                }
            }
            catch (ArgumentException aex)
            {
                XimuraAppTrace.WriteLine("ResolveExecutionPlan -> Read timeout: " + aex.Message, 
                    "CDSSettings", EventLogEntryType.Warning);
                throw aex;
            }
        }
        #endregion
        #region GetAndCacheExecutionPlan(CDSStateAction action, Type objectType)
        /// <summary>
        /// This method builds the CDSState execution plan for the particular type and action.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="objectType">The object type.</param>
        /// <returns>The CDSState execution plan.</returns>
        private string[] GetAndCacheExecutionPlan(CDSStateAction action, Type objectType)
        {
            string[] executionPlan = GetExecutionPlan(action, objectType);
            string objectTypeReduced = objectType.FullName;

            //Ok, try and get a write lock and update the cache for next time.
            try
            {
                LockCookie lc = rwl.UpgradeToWriterLock(TIMEOUTWRITE);
                try
                {
                    if (!mStateExecutionPlanCache[action].ContainsKey(objectTypeReduced))
                        mStateExecutionPlanCache[action].Add(objectTypeReduced,executionPlan);
                }
                finally
                {
                    rwl.DowngradeFromWriterLock(ref lc);
                }
            }
            catch (ApplicationException aex) 
            {
                XimuraAppTrace.WriteLine("GetAndCacheExecutionPlan -> Write timeout: " + aex.Message,
                    "CDSSettings", EventLogEntryType.Warning);
            }

            return executionPlan;
        }
        #endregion // BuildExecutionPlan(CDSStateAction action, Type objectType)

        #region GetExecutionPlan(CDSStateAction action, Type objectType)
        /// <summary>
        /// This method calculates the CDSState Execution plan from the state collection.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="objectType">The object type.</param>
        /// <returns>The CDSState execution plan.</returns>
        private string[] GetExecutionPlan(CDSStateAction action, Type objectType)
        {
            SortedList<int, string> stateOrder = new SortedList<int, string>();
            foreach (string stateName in mStateExtender.GetStateIDList())
            {
                ICDSState state = mStateExtender.GetState(stateName);
                int order = state.SupportsEntityAction(action, objectType);
                if (order > -1)
                    stateOrder.Add(order, stateName);
            }
            //Ok, if nothing else is returned return the empty execution plan.
            if (stateOrder.Count==0)
                return new string[] { };

            string[] output = new string[stateOrder.Count];
            int item = 0;
            foreach (string state in stateOrder.Values)
            {
                output[item] = state;
                item++;
            }
            return output;
        }
        #endregion

        #region CacheManagerBridge
        /// <summary>
        /// This is the cache manager bridge.
        /// </summary>
        public virtual ICDSCacheManagerBridge CacheManagerBridge
        {
            get
            {
                return mCacheManagerBridge;
            }
        }
        #endregion // CacheManagerBridge

        #region IXimuraFSMSettings<ST> Members
        /// <summary>
        /// This method is used to initialize the FSM settings object.
        /// </summary>
        /// <param name="baseCommand">The base command information.</param>
        /// <param name="baseApplication">The base application information.</param>
        /// <param name="extender">The state extender.</param>
        /// <param name="poolManager">The pool manager.</param>
        /// <param name="sessionManager">The session manager.</param>
        /// <param name="processSession">The current process session that the command is running under.</param>
        /// <param name="cacheManagerBridge">The cache manager bridge, which allows cache managers to share information.</param>
        public virtual void InitializeSettings(IXimuraCommand baseCommand, 
            IXimuraApplicationDefinition baseApplication, StateExtender<ICDSState> extender,
                IXimuraPoolManager poolManager, IXimuraEnvelopeHelper envelopeHelper, IXimuraSessionManager sessionManager, 
                    IXimuraSession processSession, CDSConfiguration config, CDSPerformance perf, ICDSCacheManagerBridge cacheManagerBridge)
        {
            base.InitializeSettings(baseCommand, baseApplication, extender, poolManager, envelopeHelper, sessionManager, processSession, config, perf);
            this.mCacheManagerBridge = cacheManagerBridge;
        }
        #endregion
    }
}
