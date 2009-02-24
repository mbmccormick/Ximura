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
    public class Context<ST, SET, CONF, PERF> : Component, IXimuraPoolReturnable, IXimuraFSMContext, 
        IXimuraApplicationDefinition, IXimuraCommand
        where ST : class, IXimuraFSMState
        where SET : class, IXimuraFSMSettings<ST, CONF, PERF>, new()
        where CONF : FSMCommandConfiguration, new()
        where PERF : FSMCommandPerformance, new()
    {
        #region Declarations
        private byte[] mNonce;
        private readonly Guid mPoolTrackingID = Guid.NewGuid();
        /// <summary>
        /// This is the internal session which can be used to make requests to the rest of the 
        /// Ximura application.
        /// </summary>
        IXimuraSessionRQ mContextSession = null;
        /// <summary>
        /// The context pool.
        /// </summary>
        private IXimuraPool mContextPool = null;
        /// <summary>
        /// This is the current state.
        /// </summary>
        protected ST mState;
        /// <summary>
        /// This is the Finite State Machine.
        /// </summary>
        private SET mContextSettings;

        private Guid? mSignatureID = null;

        //private DataContentSessionWrapper mDataContentSessionWrapper;
        private CDSHelper mCDSHelper;

        private IXimuraFSMContextPoolAccess mContextPoolAccess;

        private string mDomain;
        private string mUserName;
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

        #region UserName
        /// <summary>
        /// This property contains the session username.
        /// </summary>
        public string UserName
        {
            get { return mUserName; }
        }
        #endregion // UserName
        #region Domain
        /// <summary>
        /// This property contains the session domain.
        /// </summary>
        public string Domain
        {
            get { return mDomain; }
        }
        #endregion // Domain

        #region ContextSession
        /// <summary>
        /// This is the context session.
        /// </summary>
        public virtual IXimuraSessionRQ ContextSession
        {
            get
            {
                return mContextSession;
            }
        }
        #endregion // ContextSession
        #region ContextSessionNegotiate
        /// <summary>
        /// This protected property is used for authentication services within the context.
        /// </summary>
        protected IXimuraSessionNegotiate ContextSessionNegotiate
        {
            get
            {
                return mContextSession as IXimuraSessionNegotiate;
            }
        }
        #endregion // ContextSessionNegotiate

        #region ContextSessionInitialize(string domain, string username)
        /// <summary>
        /// This method creates a context session if one doesn't exist already.
        /// </summary>
        public virtual void ContextSessionInitialize(string username)
        {
            ContextSessionInitialize(ContextSettings.DomainDefault, username);
        }
        /// <summary>
        /// This method creates a context session if one doesn't exist already.
        /// </summary>
        public virtual void ContextSessionInitialize(string domain, string username)
        {
            if (ContextSession != null)
                throw new ArgumentException("ContextSession has already been initialized.");
            mUserName = username;
            mDomain = domain;
            mContextSession = ContextSettings.SessionManager.SessionCreate(domain, username);
        }
        #endregion // ContextSessionInitialize()

        public byte[] NonceGet()
        {
            return null;
        }

        public byte[] NonceCurrent
        {
            get
            {
                return mNonce;
            }
        }

        public bool ContextSessionAuthenticate(string password)
        {
            return false;
        }

        public bool ContextSessionAuthenticate(string data, string nonce, string qop, string nc, string cnonce)
        {
            return false;
            //CH.DigestCalculateResponse(
        }


        #region CheckChangeState(string stateName)
        /// <summary>
        /// This method checks and then changes the state. If the state is not valid an exception is thrown.
        /// </summary>
        /// <param name="stateName">The state name to check.</param>
        /// <exception cref="Ximura.Helper.InvalidStateNameFSMException">This exception is thrown if the stateName cannot be resolved.</exception>
        public virtual void CheckChangeState(string stateName)
        {
            if (!CheckState(stateName))
                throw new InvalidStateNameFSMException(@"The state name """ + stateName + @""" is not recognised.");
            ChangeState(stateName);
        }
        #endregion
        #region ChangeState()
        /// <summary>
        /// This method changes the states to the initial state.
        /// </summary>
        public virtual void ChangeState()
        {
            ChangeState(null);
        }
        /// <summary>
        /// This method changes the state.
        /// </summary>
        /// <param name="stateName">The state. If this is set to null the initial state will be set.</param>
        public virtual void ChangeState(string stateName)
        {
            if (mContextSettings == null)
                return;
            if (stateName == null)
                this.CurrentState = mContextSettings.GetInitialState();
            else
                this.CurrentState = mContextSettings.GetState(stateName);
        }
        /// <summary>
        /// This method returns true if the state exists in the FSM.
        /// </summary>
        /// <param name="stateName">The state.</param>
        public virtual bool CheckState(string stateName)
        {
            if (stateName == null)
                return false;

            if (mContextSettings == null)
                return false;

            return mContextSettings.GetState(stateName) != null;
        }
        #endregion // ChangeState
        #region CurrentState
        /// <summary>
        /// This is the current purchase state.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual ST CurrentState
        {
            get
            {
                return mState;
            }
            set
            {
#if DEBUG
                if (value == null)
                    Debug.WriteLine("State set to null.");
#endif
                mState = value;
            }
        }
        #endregion // CurrentState
        #region StateCollection
        /// <summary>
        /// This method returs a collection of states.
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<ST> StateCollection()
        {
            return ContextSettings.StateCollection();
        }
        #endregion // StateCollection()

        #region IXimuraPoolableObject
        /// <summary>
        /// This method returns true if the object can be pooled.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual bool CanPool
        {
            get { return true; }
        }
        /// <summary>
        /// This is the object pool tracking ID.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Guid TrackID { get { return mPoolTrackingID; } }
        #endregion // Reset
        #region IXimuraPoolReturnable Members
        /// <summary>
        /// This property sets a reference to the object pool on the object
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IXimuraPool ObjectPool
        {
            get
            {
                return mContextPool;
            }
            set
            {
                mContextPool = value;
            }
        }
        /// <summary>
        /// This boolean property determines whether the object can return to the pool.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool ObjectPoolCanReturn
        {
            get { return mContextPool != null; }
        }
        /// <summary>
        /// This method returns the object ot the pool.
        /// </summary>
        public void ObjectPoolReturn()
        {
            mContextPool.Return(this);
        }
        #endregion

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

        #region GetObjectPool(Type objectType)
        /// <summary>
        /// This method retrieves the object pool for the particular object type.
        /// </summary>
        /// <param name="objectType">The object type.</param>
        /// <returns>Returns the object pool.</returns>
        public IXimuraPool GetObjectPool(Type objectType)
        {
            return ContextSettings.PoolManager.GetPoolManager(objectType);
        }
        /// <summary>
        /// This generic method retrieves a pool manager for the type specified.
        /// </summary>
        /// <typeparam name="T">The type specified.</typeparam>
        /// <returns>Returns the pool manager.</returns>
        public IXimuraPool<T> GetObjectPool<T>()
            where T: IXimuraPoolableObject
        {
            return ContextSettings.PoolManager.GetPoolManager<T>();
        }

        /// <summary>
        /// This generic method retrieves an object from the pool manager for the type specified.
        /// </summary>
        /// <typeparam name="T">The type specified.</typeparam>
        /// <returns>Returns the pool manager object.</returns>
        public T GetObject<T>() where T : IXimuraPoolableObject
        {
            return ContextSettings.PoolManager.GetPoolManager<T>().Get();
        }
        /// <summary>
        /// This generic method retrieves an object from the pool and deserializes the data in the info object in to the 
        /// new pool object.
        /// </summary>
        /// <typeparam name="T">The type specified.</typeparam>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The serialization context.</param>
        /// <returns>An object of the type defined in the pool definition, with the serialization data.</returns>
        public T GetObject<T>(SerializationInfo info, StreamingContext context) where T : IXimuraPoolableObject
        {
            return ContextSettings.PoolManager.GetPoolManager<T>().Get(info, context);
        }
        #endregion // GetObjectPool(Type objectType)

        #region ContextCanGet
        /// <summary>
        /// This propery determines whether the context can get other contexts.
        /// </summary>
        public virtual bool ContextPoolAccessGranted { get { return mContextPoolAccess != null; } }
        #endregion // ContextCanGet
        #region ContextGet()
        ///// <summary>
        ///// This method returns a new context from the FSM.
        ///// </summary>
        ///// <returns></returns>
        //public virtual IXimuraFSMContext ContextGet()
        //{
        //    if (!ContextCanGet)
        //        return null;
        //    IXimuraFSMContext cntx = ContextPoolAccess.ContextGetGeneric();
        //    return cntx;
        //}
        #endregion // ContextGet()

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

        #region ContextPoolAccess
        /// <summary>
        /// This method allows access the context pool
        /// </summary>
        public IXimuraFSMContextPoolAccess ContextPoolAccess
        {
            get { return mContextPoolAccess; }
        }
        #endregion // ContextPoolAccess

    }
}