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
    public abstract class CommandContainerBase<COMM> : XimuraComponentService, IXimuraCommand, IXimuraSessionRQ
        where COMM : class, IXimuraCommandBase, IXimuraCommandRQ
    {
        #region Declarations
        /// <summary>
        /// This is the instance of the command.
        /// </summary>
        protected COMM mCommand;
        #endregion
        #region Constructor
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        public CommandContainerBase()
        {
            CommandCreate();
            if (mCommand == null)
                throw new ArgumentNullException("mCommand", "The command object has not been created.");
        }
        #endregion

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
            mCommand.Start();
        }

        protected override void InternalStop()
        {
            mCommand.Stop();
        }

        protected override void InternalPause()
        {
            mCommand.Pause();
        }

        protected override void InternalContinue()
        {
            mCommand.Continue();
        }
        #endregion 

        #region IXimuraSessionRQ Members

        public void CancelRequest(Guid jobID)
        {
            throw new NotImplementedException();
        }

        public void ProcessRequest(IXimuraRQRSEnvelope Data)
        {
            throw new NotImplementedException();
        }

        public void ProcessRequest(IXimuraRQRSEnvelope Data, JobPriority priority)
        {
            throw new NotImplementedException();
        }

        public void ProcessRequest(IXimuraRQRSEnvelope Data, CommandProgressCallback ProgressCallback)
        {
            throw new NotImplementedException();
        }

        public void ProcessRequest(IXimuraRQRSEnvelope Data, JobPriority priority, CommandProgressCallback ProgressCallback)
        {
            throw new NotImplementedException();
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
    }
}
