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

using CH = Ximura.Common;
using RH = Ximura.Reflection;
using AH = Ximura.AttributeHelper;
using Ximura.Framework;


using Ximura.Framework;
#endregion // using
namespace Ximura.Data
{
    /// <summary>
    /// This class is the base state for the Content Data Store. Persistence Managers, 
    /// Cache Managers and Custom Commands are derived from this base state.
    /// </summary>
    public class CDSState<CONF, PERF> : AppCommandBase<CONF, PERF>, ICDSState
        where CONF : CommandConfiguration, new()
        where PERF : CDSStatePerformance, new()
    {
        #region Declarations
        private CDSStatePriorityGroup mPriorityGroup = CDSStatePriorityGroup.Standard;
        private short mPriorityGroupID = 1;
        #endregion // Declarations
        #region Constructors
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        public CDSState() : this(null) { }
        /// <summary>
        /// This is the component model constructor.
        /// </summary>
        /// <param name="container">The container</param>
        public CDSState(IContainer container)
            : base(container)
        {

        }
        #endregion // Constructors

        #region IN --> Initialize
        /// <summary>
        /// This method can be used to initialize the request.
        /// </summary>
        /// <param name="context">The job context.</param>
        public virtual void Initialize(CDSContext context)
        {
            throw new NotImplementedException("Initialize is not implemented in this CDS state");
        }
        #endregion
        #region IN --> ProcessAction(CDSStateAction action, CDSContext context)
        /// <summary>
        /// This method is used to process the specific REST actions for the CDS State/
        /// </summary>
        /// <param name="action">The request action, i.e. Read, Create etc</param>
        /// <param name="context">The current context.</param>
        /// <returns>
        /// The boolean return value indicates whether the request was successfully resolved. A true response indicates
        /// that the request was resolved by this Persistence Manager and that execution is complete. A false response indicates that this
        /// Persistence Manager could not resolve the request and that the Content Data Store should continue with the execution plan.
        /// </returns>
        /// <exception cref="Ximura.Data.CDSStateException">
        /// A CDSStateException will be thrown if a request is made that this persistence manager does not support.
        /// </exception>
        public virtual bool ProcessAction(CDSStateAction action, CDSContext context)
        {
            switch (action)
            {
                case CDSStateAction.ResolveReference:
                    return ResolveReference(context);
                case CDSStateAction.Browse:
                    return Browse(context);
                case CDSStateAction.Create:
                    return Create(context);
                case CDSStateAction.Custom:
                    return Custom(context);
                case CDSStateAction.Delete:
                    return Delete(context);
                case CDSStateAction.Read:
                    return Read(context);
                case CDSStateAction.Restore:
                    return Restore(context);
                case CDSStateAction.Update:
                    return Update(context);
                case CDSStateAction.Cache:
                    return Cache(context);
                case CDSStateAction.VersionCheck:
                    return VersionCheck(context);
            }

            throw new CDSStateException("Invalid action type in ProcessAction: " + action.ToString());
        }
        #endregion // ProcessAction(CDSStateAction action, CDSContext context)
        #region IN --> Finish
        /// <summary>
        /// This method can be used to modify a response before it is sent back to a user.
        /// </summary>
        /// <param name="context">The job context.</param>
        public virtual void Finish(CDSContext context)
        {
            throw new NotImplementedException("Finish is not implemented in this CDS state");
        }
        #endregion

        #region IN --> SupportsEntityAction(CDSStateAction action, Type objectType)
        /// <summary>
        /// This method should return true when the action and entity are supported. This method is used by the CDS to build
        /// the Execution plan for specific Entity types and actions.
        /// </summary>
        /// <returns>Returns -1 is the action is not supported, otherwise the combined order is returned.</returns>
        public virtual int SupportsEntityAction(CDSStateAction action, Type objectType)
        {
            return -1;
        }
        #endregion // SupportsEntityAction(CDSStateAction action, Type objectType)

        #region IN --> PriorityGroup
        /// <summary>
        /// This is the priority group for the CDSState. This property determines the grouping order in which 
        /// states will be polled for incoming requests.
        /// </summary>
        [Description("This is the priority group for the CDSState. This property determines the grouping order in which states will be polled for incoming requests.")]
        [Category("CDS State Metadata")]
        [DefaultValue("CDSStatePriorityGroup.Standard")]
        public virtual CDSStatePriorityGroup PriorityGroup
        {
            get
            {
                return mPriorityGroup;
            }
            set
            {
                mPriorityGroup = value;
            }
        }
        #endregion // PriorityGroup
        #region IN --> PriorityGroupID
        /// <summary>
        /// This property determines the polling order for CDSStates within the specific group.
        /// </summary>
        [Description("This property determines the polling order for CDSStates within the specific group.")]
        [Category("CDS State Metadata")]
        [DefaultValue(1)]
        public virtual short PriorityGroupID
        {
            get
            {
                return mPriorityGroupID;
            }
            set
            {
                mPriorityGroupID = value;
            }
        }
        #endregion // PriorityGroupID

        #region PriorityCombined
        /// <summary>
        /// This protected method returns the combined priority of the PriorityGroup and the PriorityGroupID.
        /// </summary>
        protected virtual int PriorityCombined
        {
            get
            {
                return ((int)PriorityGroupID) | (((int)PriorityGroup) << 16);
            }
        }
        #endregion // PriorityCombined

        #region Create
        /// <summary>
        /// This is the Create method.
        /// </summary>
        /// <param name="context">The job context.</param>
        /// <returns>
        /// The boolean return value should indicate whether the request was successfully resolved. A true response indicates
        /// that the request was resolved by this Persistence Manager and that execution is complete. A false response indicates that this
        /// Persistence Manager could not resolve the request and that the Content Data Store should continue with the execution plan.
        /// </returns>
        protected virtual bool Create(CDSContext context)
        {
            throw new NotImplementedException("Create is not implemented in this CDS state");
        }
        #endregion
        #region Read
        /// <summary>
        /// This is the read method.
        /// </summary>
        /// <param name="context">The job context.</param>
        /// <returns>
        /// The boolean return value should indicate whether the request was successfully resolved. A true response indicates
        /// that the request was resolved by this Persistence Manager and that execution is complete. A false response indicates that this
        /// Persistence Manager could not resolve the request and that the Content Data Store should continue with the execution plan.
        /// </returns>
        protected virtual bool Read(CDSContext context)
        {
            throw new NotImplementedException("Read is not implemented in this CDS state");
        }
        #endregion
        #region Update
        /// <summary>
        /// This is the update method.
        /// </summary>
        /// <param name="context">The job context.</param>
        /// <returns>
        /// The boolean return value should indicate whether the request was successfully resolved. A true response indicates
        /// that the request was resolved by this Persistence Manager and that execution is complete. A false response indicates that this
        /// Persistence Manager could not resolve the request and that the Content Data Store should continue with the execution plan.
        /// </returns>
        protected virtual bool Update(CDSContext context)
        {
            throw new NotImplementedException("Update is not implemented in this CDS state");
        }
        #endregion        
        #region Delete
        /// <summary>
        /// This is the delete method.
        /// </summary>
        /// <param name="context">The job context.</param>
        /// <returns>
        /// The boolean return value should indicate whether the request was successfully resolved. A true response indicates
        /// that the request was resolved by this Persistence Manager and that execution is complete. A false response indicates that this
        /// Persistence Manager could not resolve the request and that the Content Data Store should continue with the execution plan.
        /// </returns>
        protected virtual bool Delete(CDSContext context)
        {
            throw new NotImplementedException("Delete is not implemented in this CDS state");
        }
        #endregion
        #region VersionCheck
        /// <summary>
        /// This method checks the data store to see whether the references to the entity are current.
        /// </summary>
        /// <param name="context">The current CDS context.</param>
        /// <remarks>
        /// The status codes for the response are as follows:
        ///     200 = OK, the contentID and versionID are correct.
        ///     400 = missing parameter, either the contentID or versionID is null
        ///     404 = the content ID was not found
        ///     412 = the version ID is not the current version.
        ///     500 = there has been an internal system error. check the SubStatus parameter for the exception description.
        /// </remarks>
        /// <returns>
        /// The boolean return value should indicate whether the request was successfully resolved. A true response indicates
        /// that the request was resolved by this Persistence Manager and that execution is complete. A false response indicates that this
        /// Persistence Manager could not resolve the request and that the Content Data Store should continue with the execution plan.
        /// </returns>
        protected virtual bool VersionCheck(CDSContext context)
        {
            throw new NotImplementedException("VersionCheck is not implemented in this CDS state");
        }
        #endregion
        #region Browse
        /// <summary>
        /// This is the browse method.
        /// </summary>
        /// <param name="context">The job context.</param>
        /// <returns>
        /// The boolean return value should indicate whether the request was successfully resolved. A true response indicates
        /// that the request was resolved by this Persistence Manager and that execution is complete. A false response indicates that this
        /// Persistence Manager could not resolve the request and that the Content Data Store should continue with the execution plan.
        /// </returns>
        protected virtual bool Browse(CDSContext context)
        {
            throw new NotImplementedException("Browse is not implemented in this CDS state");
        }
        #endregion
        #region Restore
        /// <summary>
        /// This is the restore method that reactivates a previously deleted entity or restores a previous version.
        /// </summary>
        /// <param name="context">The job context.</param>
        /// <returns>
        /// The boolean return value should indicate whether the request was successfully resolved. A true response indicates
        /// that the request was resolved by this Persistence Manager and that execution is complete. A false response indicates that this
        /// Persistence Manager could not resolve the request and that the Content Data Store should continue with the execution plan.
        /// </returns>
        protected virtual bool Restore(CDSContext context)
        {
            throw new NotImplementedException("Restore is not implemented in this CDS state");
        }
        #endregion
        #region Custom
        /// <summary>
        /// This is the custom command method.
        /// </summary>
        /// <param name="context">The job context.</param>
        /// <returns>
        /// The boolean return value should indicate whether the request was successfully resolved. A true response indicates
        /// that the request was resolved by this Persistence Manager and that execution is complete. A false response indicates that this
        /// Persistence Manager could not resolve the request and that the Content Data Store should continue with the execution plan.
        /// </returns>
        protected virtual bool Custom(CDSContext context)
        {
            throw new NotImplementedException("Custom is not implemented in this CDS state");
        }
        #endregion

        #region ResolveReference
        /// <summary>
        /// This method resolves reference parameters for the current request.
        /// </summary>
        /// <param name="context">The job context.</param>
        /// <returns>
        /// The boolean return value should indicate whether the request was successfully resolved. A true response indicates
        /// that the request was resolved by this Persistence Manager and that execution is complete. A false response indicates that this
        /// Persistence Manager could not resolve the request and that the Content Data Store should continue with the execution plan.
        /// </returns>
        protected virtual bool ResolveReference(CDSContext context)
        {
            throw new NotImplementedException("Custom is not implemented in this CDS state");
        }
        #endregion
        #region Cache
        /// <summary>
        /// This is the cache method, and is called after the primary action has been processed.
        /// </summary>
        /// <param name="context">The job context.</param>
        /// <returns>
        /// The boolean return value should indicate whether the request was successfully resolved. A true response indicates
        /// that the request was resolved by this Persistence Manager and that execution is complete. A false response indicates that this
        /// Persistence Manager could not resolve the request and that the Content Data Store should continue with the execution plan.
        /// </returns>
        protected virtual bool Cache(CDSContext context)
        {
            throw new NotImplementedException("Cache is not implemented in this CDS state");
        }
        #endregion

        #region Identifier
        /// <summary>
        /// This is the Content Data Store state identifier string.
        /// </summary>
        [Category("State")]
        [DefaultValue(null)]
        [Description("This is the CDS State identifier string.")]
        [RefreshProperties(RefreshProperties.All)]
        public string Identifier
        {
            get;
            set;
        }
        #endregion
        #region ToString()
        /// <summary>
        /// This override is primarily used in debugging to make the CDSState easy to identify.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return " CDSState -> " + Identifier + " -> " + this.GetType().Name;
        }
        #endregion // ToString()

    }
}
