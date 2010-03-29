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
using Ximura.Framework;


using Ximura.Framework;
#endregion // using
namespace Ximura.Data
{
    /// <summary>
    /// This delegate is used to access the CDS directly from the current context.
    /// </summary>
    /// <param name="job">The current job.</param>
    /// <param name="Data">The request.</param>
    public delegate void DelCDSProcessRQ(SecurityManagerJob job, RQRSContract<CDSRequestFolder, CDSResponseFolder> Data);

    /// <summary>
    /// This is the context used to process CDS requests.
    /// </summary>
    public class CDSContext : JobContext<ICDSState, CDSSettings, CDSRequestFolder, CDSResponseFolder, CDSConfiguration, CDSPerformance>
    {
        #region Declarations
        private bool mContentIsCacheable;
        private ContentCacheOptions mCacheStyle;
        private int mCacheTimeOut;
        private CDSHelperDirect mCDSHelperDirect = null;
        private CDSDirectAccess mCDSda = null;
        #endregion // Declarations
        #region Constructor
        /// <summary>
        /// This is the default constructor
        /// </summary>
        public CDSContext():base()
        {
        }
        #endregion // Constructor

        #region Reset()
        /// <summary>
        /// This method will reset specific request values for the CDS context.
        /// </summary>
        public override void Reset()
        {
            base.Reset();
            mContentIsCacheable = false;
            mCacheTimeOut = -1;
            mCacheStyle = ContentCacheOptions.CannotCache;

            if (mCDSda == null)
                mCDSda = new CDSDirectAccess();
            else
            {
                mCDSda.Job = null;
                mCDSda.DelProcessRequest = null;
            }

            if (mCDSHelperDirect == null)
                mCDSHelperDirect = new CDSHelperDirect(mCDSda);
        }
        #endregion // Reset()

        #region CacheStyle
        /// <summary>
        /// This is the cache style.
        /// </summary>
        public ContentCacheOptions CacheStyle
        {
            get { return mCacheStyle; }
        }
        #endregion // CacheStyle
        #region CacheTimeOut
        /// <summary>
        /// This is the cache timeout in seconds.
        /// </summary>
        public int CacheTimeOut
        {
            get { return mCacheTimeOut; }
        }
        #endregion // CacheTimeOut

        #region RequestIsByReference
        /// <summary>
        /// This property returns true if the request is by reference.
        /// </summary>
        public bool RequestIsByReference
        {
            get
            {
                return Data.ContractRequest.ByReference;
            }
        }
        #endregion // RequestIsByReference

        #region Initialize()
        /// <summary>
        /// This method initializes the context.
        /// </summary>
        public void Initialize()
        {
            XimuraContentCachePolicyAttribute attr = ContextSettings.CacheManagerBridge.CacheAttribute(Request.DataType);
            mCacheStyle = attr.CacheStyle;
            mCacheTimeOut = attr.TimeOut;

            ContentIsCacheable = mCacheStyle != ContentCacheOptions.CannotCache;

            //OK, set the initial state.
            this.ChangeState();
            CurrentState.Initialize(this);
        }
        #endregion
        #region CDSStateActionResolve()
        /// <summary>
        /// This method maps the PMCapability request to its corresponding CDS Action type.
        /// </summary>
        /// <returns></returns>
        public virtual CDSAction CDSStateActionResolve()
        {
            return (CDSAction)Data.DestinationAddress.SubCommand;
            
            //throw new InvalidCommandCDSException("The PMCapability is not recognised: " + Data.DestinationAddress.subcommand.ToString());
        }
        #endregion // CDSStateActionResolve()
        #region CDSStateProcessDirective()
        /// <summary>
        /// This method processes the specific CRUD execution plan for the request.
        /// </summary>
        public virtual bool CDSStateProcessDirective(CDSAction action)
        {
            bool resolved = false;

            try
            {
                string[] exPath = ContextSettings.ResolveExecutionPlan(action, Request.DataType);

                foreach (string state in exPath)
                {
                    if (!this.CheckState(state))
                        throw new CDSStateException("Specified state does not exist: " + state);

                    this.ChangeState(state);
                    resolved = CurrentState.ProcessAction(action, this);
                    if (resolved)
                        break;
                }
                return resolved;
            }
            catch (Exception)
            {
                //oops
                return false;
            }
            finally
            {
                this.ChangeState();
            }
        }
        #endregion // ProcessDirective()
        #region Finish()
        /// <summary>
        /// This method provides any clean up or finalization of the response.
        /// </summary>
        public virtual void Finish()
        {
            this.ChangeState("Finish");
            this.CurrentState.Finish(this);
        }
        #endregion // Finalize()

        #region ContentIsCacheable
        /// <summary>
        /// This request returns true if the content is cacheable. 
        /// This will be used to indicate whether the Cache method should
        /// be called after the action has been completed.
        /// </summary>
        public bool ContentIsCacheable
        {
            get
            {
                return mContentIsCacheable;
            }
            set
            {
                mContentIsCacheable = value;
            }
        }
        #endregion // ContentIsCacheable
        #region ContentIsFragment
        /// <summary>
        /// This method checks whether the entity being passed is a fragment object.
        /// </summary>
        /// <param name="Request">The request containing the entity.</param>
        /// <returns>Returns true if the object is an entity.</returns>
        protected virtual bool ContentIsFragment
        {
            get
            {
                Content content = Request.Data as Content;
                if (content != null)
                    return content.IsFragment();
                else
                    return false;
            }
        }
        #endregion // FragmentCheck
        #region ContentProcessFragment
        /// <summary>
        /// This method processes a fragment object and merges the updates with the base content.
        /// </summary>
        /// <param name="job">The current request.</param>
        /// <param name="action">The current action. This may be changed depending on the merge action performed.</param>
        protected virtual void ContentProcessFragment(SecurityManagerJob job,
            RQRSContract<CDSRequestFolder, CDSResponseFolder> Data, ref CDSAction action)
        {
            CDSRequestFolder Request = Data.ContractRequest;
            CDSResponseFolder Response = Data.ContractResponse;

            //Get the active content.
            Content content = Request.Data;

            Content parentContent = null;

            string status = CH.HTTPCodes.InternalServerError_500;

            //if (content.FragmentIDIsByReference())
            //    parentContent = DCWrapper.DataContentReadByReference(job, content.FragmentBaseType(),
            //        content.FragmentReferenceID(), content.FragmentReferenceType(), job.Priority, out status);
            //else
            //    parentContent = DCWrapper.DataContentRead(job, content.FragmentBaseType(),
            //        content.ID, IDType.ContentID, job.Priority, out status);

            //n.b. Job priority has been removed, as CDSHelperDirect operates on the same thread as 
            //the original request so is no longer necessary.
            if (content.FragmentIDIsByReference())
                status = CDSHelperDirect.Execute(content.FragmentBaseType(),
                    CDSData.Get(CDSAction.Read, content.FragmentReferenceID(), content.FragmentReferenceType()),
                        out parentContent);
            else
                status = CDSHelperDirect.Execute(content.FragmentBaseType(),
                    CDSData.Get(CDSAction.Read, content.IDContent, null),
                        out parentContent);

            if (status == CH.HTTPCodes.InternalServerError_500)
                throw new ContentDataStoreException("Cannot get parent content to merge.");

            if (parentContent != null)
                content.MergeContent(parentContent);
            else
            {
                content.ConvertFragmentToPrimaryEntity();
                parentContent = content;
                //				action = PMCapabilities.Create;
            }

            Request.DataContentID = parentContent.IDContent;
            Request.Data = parentContent;
        }
        #endregion // ProcessFragment

        #region CDSHelperDirect
        /// <summary>
        /// This CDSHelper wrapper access the CDS directly using the current thread and security settings. 
        /// </summary>
        public virtual CDSHelperDirect CDSHelperDirect
        {
            get { return mCDSHelperDirect; }
        }
        #endregion // CDSHelper

        #region Reset(IXimuraFSMSettingsBase fsm, SecurityManagerJob job,...
        /// <summary>
        /// This method resets a connection and sets the connection state to ClosedRBPState.
        /// </summary>
        /// <param name="fsm">A reference to the finite state machine.</param>
        /// <param name="job">The job request.</param>
        public virtual void Reset(IXimuraFSMSettingsBase fsm, SecurityManagerJob job, RQRSContract<CDSRequestFolder, CDSResponseFolder> data,
            IXimuraFSMContextPoolAccess contextGet, DelCDSProcessRQ mDel)
        {
            base.Reset(fsm, job, data, contextGet);
            mCDSda.Job = job;
            mCDSda.DelProcessRequest = mDel;
        }
        #endregion // Reset(IXimuraFSMSettingsBase fsm, SecurityManagerJob job, RQRSContract<RQ, RS> data, IXimuraFSMContextPoolAccess contextGet, IXimuraSessionRQ directSession)

        #region CDSDirectAccess --> Class
        /// <summary>
        /// The CDSDirectAccess class is used to allow CDSContexts to call internally to retrieve content without 
        /// having to utilize the multi-threaded session mananger. This may introduce security risks for certain system
        /// and should be disabled if not required.
        /// </summary>
        protected class CDSDirectAccess : IXimuraSessionRQ
        {
            #region Declarations
            SecurityManagerJob mJob = null;
            DelCDSProcessRQ mDel = null;
            #endregion // Declarations
            #region Constructors
            public CDSDirectAccess()
            {

            }
            #endregion // Constructors

            #region IXimuraSessionRQ Members

            public void CancelRequest(Guid jobID)
            {
                throw new NotSupportedException();
            }

            public void ProcessRequest(IXimuraRQRSEnvelope Data)
            {
                ProcessRequest(Data, JobPriority.Normal, null);
            }

            public void ProcessRequest(IXimuraRQRSEnvelope Data, JobPriority priority)
            {
                ProcessRequest(Data, priority, null);
            }

            public void ProcessRequest(IXimuraRQRSEnvelope Data, CommandProgressCallback ProgressCallback)
            {
                ProcessRequest(Data, JobPriority.Normal, ProgressCallback);
            }

            public void ProcessRequest(IXimuraRQRSEnvelope Data, JobPriority priority, CommandProgressCallback ProgressCallback)
            {
                mDel(mJob, (RQRSContract<CDSRequestFolder, CDSResponseFolder>)Data);
            }

            #endregion

            #region IXimuraSessionRQAsync Members

            public Guid ProcessRequestAsync(IXimuraRQRSEnvelope data, CommandRSCallback RSCallback)
            {
                throw new NotSupportedException("Async calls are not supported in the CDSHelperDirect class. Use the CDSHelper class instead.");
            }

            public Guid ProcessRequestAsync(IXimuraRQRSEnvelope data, CommandRSCallback RSCallback, CommandProgressCallback ProgressCallback, JobPriority priority)
            {
                throw new NotSupportedException("Async calls are not supported in the CDSHelperDirect class. Use the CDSHelper class instead.");
            }

            public Guid ProcessRequestAsync(Guid jobID, IXimuraRQRSEnvelope data, CommandRSCallback RSCallback)
            {
                throw new NotSupportedException("Async calls are not supported in the CDSHelperDirect class. Use the CDSHelper class instead.");
            }

            public Guid ProcessRequestAsync(Guid jobID, IXimuraRQRSEnvelope data, CommandRSCallback RSCallback, CommandProgressCallback ProgressCallback, JobPriority priority)
            {
                throw new NotSupportedException("Async calls are not supported in the CDSHelperDirect class. Use the CDSHelper class instead.");
            }

            #endregion

            #region Job
            /// <summary>
            /// This is the job for the current context;
            /// </summary>
            public SecurityManagerJob Job
            {
                get { return mJob; }
                set { mJob = value; }
            }
            #endregion // Job


            public DelCDSProcessRQ DelProcessRequest
            {
                get { return mDel; }
                set { mDel = value; }
            }

            #region EnvelopeHelper
            /// <summary>
            /// This property returns the envelope helper from the current job.
            /// </summary>
            public IXimuraEnvelopeHelper EnvelopeHelper
            {
                get { return mJob.EnvelopeHelper; }
            }
            #endregion
        }
        #endregion // CDSDirectAccess --> Class

    }
}
