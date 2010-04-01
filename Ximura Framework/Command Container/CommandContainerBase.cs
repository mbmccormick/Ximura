#region Copyright
// *******************************************************************************
// Copyright (c) 2000-2010 Paul Stancer.
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
#endregion // using
namespace Ximura.Framework
{
    /// <summary>
    /// This class is used to hold command objects and run them without the need for the server environment.
    /// </summary>
    public abstract class CommandContainerBase<COMM> : FrameworkComponentBase, IXimuraCommand, IXimuraSessionRQ
        where COMM : class, IXimuraCommandBase, IXimuraCommandRQ
    {
        #region Declarations
        private IContainer components = null;
        /// <summary>
        /// This is the instance of the command.
        /// </summary>
        protected COMM mCommand;
        /// <summary>
        /// This pool contains the collection of security manager jobs used by the command container.
        /// </summary>
        protected PoolInvocator<SecurityManagerJob> mPoolSecurityManagerJob = null;
        /// <summary>
        /// This pool contains the actual job information.
        /// </summary>
        protected PoolInvocator<Job> mPoolJob = null;

        protected XimuraServiceContainer mServContainer;

        #endregion
        #region Constructor
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        public CommandContainerBase()
        {
            ServerComponentsInitialize();

            InitializeComponents();

            CommandCreate();
            if (mCommand == null)
                throw new ArgumentNullException("mCommand", "The command object has not been created.");
            components.Add(mCommand);

            CommandExtenderInitialize();

            CommandExtender.SetPriority(mCommand, 9);

            RegisterContainer(components);            
        }
        #endregion
        #region InitializeComponents()
        private void InitializeComponents()
        {
            components = new System.ComponentModel.Container();
        }
        #endregion // InitializeComponents()
        #region CommandCreate()
        /// <summary>
        /// This virtual method creates the command and sets the mCommand property. You should override this method
        /// if you wish to use a different constructor.
        /// </summary>
        protected abstract void CommandCreate();
        #endregion

        #region IXimuraCommand Members
        /// <summary>
        /// This is command identifier.
        /// </summary>
        public virtual Guid CommandID
        {
            get { return mCommand.CommandID; }
        }
        /// <summary>
        /// This is the command name.
        /// </summary>
        public virtual string CommandName
        {
            get
            {
                return mCommand.CommandName;
            }
            set
            {
                throw new NotSupportedException();
            }
        }
        /// <summary>
        /// This is the command description.
        /// </summary>
        public virtual string CommandDescription
        {
            get
            {
                return mCommand.CommandDescription;
            }
            set
            {
                throw new NotSupportedException();
            }
        }
        #endregion

        #region Service control
        protected override void InternalStart()
        {
            //Connect the components to the messaging architecture.
            ConnectComponents();

            CommandsStart();

            CommandExtender.CommandsNotify(typeof(XimuraServiceStatus), XimuraServiceStatus.Started);
        }

        protected override void InternalStop()
        {

            CommandExtender.CommandsNotify(typeof(XimuraServiceStatus), XimuraServiceStatus.Stopping);

            Exception aex = null;

            try
            {
                //Close all the commands
                CommandsStop();
                //ComponentsStatusChange(XimuraServiceStatusAction.Stop, ServiceComponents);		
            }
            catch (Exception ex)
            {
                aex = ex;
            }

        }
        #endregion 

        #region IXimuraSessionRQ Members

        public void CancelRequest(Guid jobID)
        {
            throw new NotSupportedException("CancelRequest is not currently supported.");
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
            Job rqJob = null;
            SecurityManagerJob scmJob = null;
            try
            {
                SessionToken token = new SessionToken();

                rqJob = mPoolJob.Get(j => j.Initialize(Guid.Empty, Guid.NewGuid(), Data, JobSignature.Empty, priority, null));

                scmJob = mPoolSecurityManagerJob.Get(j => j.Initialize(rqJob, token, null, null, ProgressCallback));

                mCommand.ProcessRequestSCM(scmJob);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (scmJob != null && scmJob.ObjectPoolCanReturn)
                    scmJob.ObjectPoolReturn();

                if (rqJob != null)
                    mPoolJob.Return(rqJob);
            }
        }

        #endregion
        #region IXimuraSessionRQAsync Members

        public Guid ProcessRequestAsync(IXimuraRQRSEnvelope data, CommandRSCallback RSCallback)
        {
            throw new NotImplementedException();
        }

        public Guid ProcessRequestAsync(IXimuraRQRSEnvelope data, CommandRSCallback RSCallback, CommandProgressCallback ProgressCallback, JobPriority priority)
        {
            throw new NotImplementedException();
        }

        public Guid ProcessRequestAsync(Guid jobID, IXimuraRQRSEnvelope data, CommandRSCallback RSCallback)
        {
            throw new NotImplementedException();
        }

        public Guid ProcessRequestAsync(Guid jobID, IXimuraRQRSEnvelope data, CommandRSCallback RSCallback, CommandProgressCallback ProgressCallback, JobPriority priority)
        {
            throw new NotImplementedException();
        }

        public IXimuraEnvelopeHelper EnvelopeHelper
        {
            get { throw new NotImplementedException(); }
        }

        #endregion

        #region ServerComponentsInitialize()
        /// <summary>
        /// This method is used to create the server components.
        /// </summary>
        protected virtual void ServerComponentsInitialize()
        {
            PoolManagerCreate();

            mPoolSecurityManagerJob = new PoolInvocator<SecurityManagerJob>(() => { return new SecurityManagerJob(); });
            mPoolJob = new PoolInvocator<Job>(() => { return new Job(); });

        }
        #endregion // InitializeServerComponents()

        #region PoolManagerCreate()
        /// <summary>
        /// This method creates the pool manager for the system.
        /// </summary>
        protected virtual void PoolManagerCreate()
        {
            PoolManager = new PoolManager(true);
        }
        #endregion // PoolManagerCreate()
    }
}
