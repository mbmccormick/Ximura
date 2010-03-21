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
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;

using Ximura;
using Ximura.Persistence;
using CH = Ximura.Helper.Common;
using Ximura.Server;

#endregion // using
namespace Ximura.Command
{
    /// <summary>
    /// Context is the base object for the Finite State Machine.
    /// </summary>
    [ToolboxBitmap(typeof(XimuraResourcePlaceholder), "Ximura.Resources.Context.bmp")]
    public partial class Context<ST, SET, CONF, PERF> : Component, IXimuraPoolReturnable, IXimuraFSMContext, 
        IXimuraApplicationDefinition, IXimuraCommand
        where ST : class, IXimuraFSMState
        where SET : class, IXimuraFSMSettings<ST, CONF, PERF>, new()
        where CONF : FSMCommandConfiguration, new()
        where PERF : FSMCommandPerformance, new()
    {
        #region Declarations
        private byte[] mNonce;
        private readonly Guid mPoolTrackingID = Guid.NewGuid();
        private Guid? mSignatureID = null;
        /// <summary>
        /// The context pool.
        /// </summary>
        private IXimuraPool mContextPool = null;
        /// <summary>
        /// This is the Finite State Machine.
        /// </summary>
        private SET mContextSettings;

        //private DataContentSessionWrapper mDataContentSessionWrapper;
        private CDSHelper mCDSHelper;

        private IXimuraFSMContextPoolAccess mContextPoolAccess;
        #endregion // Declarations
        #region Constructor
        /// <summary>
        /// This is the default constructor
        /// </summary>
        public Context()
        {
            //mDataContentSessionWrapper = new DataContentSessionWrapper(null);
            mCDSHelper = new CDSHelper(null);

            Reset();
        }
        #endregion // Constructor

        #region Reset()
        /// <summary>
        /// This is the default reset method as implemented by the IXimuraPoolableObject interface.
        /// </summary>
        public virtual void Reset()
        {
            mContextSession = null;

            //DCWrapper.Session = null;
            CDSHelper.Session = null;

            mContextSettings = null;
            mState = null;
            mSignatureID = Guid.NewGuid();
            mContextPoolAccess = null;
            mUserName = null;
            mDomain = null;

            mNonce = null;
        }
        #endregion // Reset()
        #region Reset(SET fsm, IXimuraSession contextSession, DelContextGet cntxGet)
        /// <summary>
        /// This method resets a connection and sets the context to the default start state..
        /// </summary>
        /// <param name="fsm">A reference to the finite state machine.</param>
        /// <param name="contextSession">This is the context session.</param>
        public virtual void Reset(IXimuraFSMSettingsBase fsm, IXimuraSessionRQ contextSession, IXimuraFSMContextPoolAccess cntxGet)
        {
            Reset();
            mContextSettings = (SET)fsm;
            //Reset the session parameters.
            mContextSession = contextSession;
            //DCWrapper.Session = contextSession;
            CDSHelper.Session = contextSession;
            mContextPoolAccess = cntxGet;
            //Set the initial state for the connection
            this.ChangeState();
        }
        #endregion // Reset(SET fsm, IXimuraSession contextSession)

        #region CDSHelper
        /// <summary>
        /// This is the CDS Helper, that simplifies access to the Content Data Store
        /// </summary>
        public virtual CDSHelper CDSHelper
        {
            get { return mCDSHelper; }
        }
        #endregion // CDSHelper

        #region ContextSettings
        /// <summary>
        /// This is the connection to the FSM settings and services.
        /// </summary>
        public virtual SET ContextSettings
        {
            get { return mContextSettings; }
        }
        #endregion // FSM

        #region SenderIdentityConfirm
        /// <summary>
        /// This method is used to validate that a callback is actually for this current instance.
        /// We need to do this as we recycle the contexts and it is possible that the callback is for a
        /// previous session.
        /// </summary>
        /// <param name="args">The callback arguments.</param>
        /// <returns>Returns true if the callback is for this specific instance.</returns>
        protected bool SenderIdentityConfirm(CommandRSEventArgs args)
        {
            return args.Data.SenderReference.HasValue
                && args.Data.SenderReference.Value == this.SignatureID
                && args.Data.Sender == this.ContextSettings.CommandID;
        }
        #endregion // SenderIdentityConfirm
        #region SenderIdentitySet
        /// <summary>
        /// This method sets the context sender identity in the Envelope.
        /// </summary>
        /// <param name="Env">The envelope to set the identity.</param>
        public virtual void SenderIdentitySet(IXimuraRQRSEnvelope Env)
        {
            Env.Sender = this.ContextSettings.CommandID;
            Env.SenderReference = this.SignatureID;
        }
        #endregion // SenderIdentitySet
        #region SignatureID
        /// <summary>
        /// This is the unique id for the context. 
        /// This will change each time the object is reset.
        /// </summary>
        public Guid? SignatureID
        {
            get
            {
                return mSignatureID;
            }
        }
        #endregion // SignatureID

        #region ContextCanGet
        /// <summary>
        /// This propery determines whether the context can get other contexts.
        /// </summary>
        public virtual bool ContextPoolAccessGranted { get { return mContextPoolAccess != null; } }
        #endregion // ContextCanGet

        #region IXimuraApplicationDefinition Members
        /// <summary>
        /// The application name.
        /// </summary>
        public string ApplicationName
        {
            get { return ContextSettings.ApplicationName; }
        }
        /// <summary>
        /// The application ID.
        /// </summary>
        public Guid ApplicationID
        {
            get { return ContextSettings.ApplicationID; }
        }
        /// <summary>
        /// The application description.
        /// </summary>
        public string ApplicationDescription
        {
            get { return ContextSettings.ApplicationDescription; }
        }
        #endregion
        #region IXimuraCommand Members
        /// <summary>
        /// The command ID
        /// </summary>
        public Guid CommandID
        {
            get { return ContextSettings.CommandID; }
        }
        /// <summary>
        /// Te command name
        /// </summary>
        public string CommandName
        {
            get { return ContextSettings.CommandName; }
            set
            {
                throw new NotImplementedException("CommandName is not implemented.");
            }
        }
        /// <summary>
        /// The command description
        /// </summary>
        public string CommandDescription
        {
            get { return ContextSettings.CommandDescription; }
            set
            {
                throw new NotImplementedException("CommandDescription is not implemented.");
            }
        }
        #endregion
    }
}