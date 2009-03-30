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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;

using Ximura;
using Ximura.Data;
using Ximura.Helper;
using RH = Ximura.Helper.Reflection;
using CH=Ximura.Helper.Common;
using Ximura.Server;
using Ximura.Command;

#endregion // using
namespace Ximura.Server
{
	/// <summary>
	/// The CommandBridge is used by the application server to isolate itself from the
	/// command control collection, and to manage the command collection.
	/// </summary>
	public class CommandBridge: XimuraComponentBase, 
        IXimuraCommandBridge, IXimuraSessionManager, IXimuraPerformanceManager, IXimuraPoolManager, IXimuraConfigurationManager
	{
		#region Declarations
        private IXimuraPoolManager mPoolManager;
        private IXimuraPerformanceManager mPerformanceManager;
        private IXimuraConfigurationManager mConfigurationManager;

        private Dictionary<Guid, IXimuraCommandRQ> cmdCollection;

		private SessionCreateRemote remoteSessionInvoke;
		#endregion
		#region Constructors
		/// <summary>
		/// This is the default constructor for the command bridge.
		/// </summary>
		/// <param name="sessionInvoke">This is callback to the session manager that
		/// allows a command to create a session.</param>
        /// <param name="performanceManager"></param>
        public CommandBridge(
            SessionCreateRemote sessionInvoke, 
            IXimuraConfigurationManager configurationManager,
            IXimuraPerformanceManager performanceManager, 
            IXimuraPoolManager poolManager
            ): base()
		{
			remoteSessionInvoke = sessionInvoke;
            mPerformanceManager = performanceManager;
            mConfigurationManager = configurationManager;
            mPoolManager = poolManager;

            cmdCollection = new Dictionary<Guid, IXimuraCommandRQ>();

            //Start the Envelope pooling.
            RQRSEnvelopeHelper.Start(mPoolManager);
		}
		#endregion

        #region Dispose
        /// <summary>
        /// This override stops the envelope pooling.
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            RQRSEnvelopeHelper.Stop();
        } 
        #endregion

        #region IXimuraCommandBridge Members

        public IXimuraEnvelopeHelper EnvelopeHelper { get { return null; } }
        /// <summary>
		/// This method is used to register a command in the command collection.
		/// </summary>
		/// <param name="command">The command to register.</param>
		/// <returns>Returns true if the command is registered successfully.</returns>
		public bool Register(IXimuraCommandRQ command)
		{
			if (cmdCollection.ContainsKey(command.CommandID))
				return false;

			cmdCollection.Add(command.CommandID,command);

            //Register the contract type with the envelope helper.
            RQRSEnvelopeHelper.RegisterCommand(command.CommandID, command.CommandName, 
                command.EnvelopeContractType, command.EnvelopeCallbackContractType);

			return true;
		}
		/// <summary>
		/// This method is used to unregister a command.
		/// </summary>
		/// <param name="command">The commands to remove.</param>
		/// <returns>Returns true if the command is successfully removed
		/// from the collection. Returns false if the command was not a member
		/// of the collection.</returns>
		public bool Unregister(IXimuraCommandRQ command)
		{
			if (!cmdCollection.ContainsKey(command.CommandID))
				return false;

			cmdCollection.Remove(command.CommandID);
			return true;
		}
		#endregion

        #region IXimuraSessionManager Members
        /// <summary>
        /// This method creates an empty session.
        /// </summary>
        /// <returns>A session object or null if the session manager is configured
        /// not to provide a session without a username.</returns>
        IXimuraSession IXimuraSessionManager.SessionCreate()
        {
            if (remoteSessionInvoke == null) 
                return null;
            return remoteSessionInvoke(null, null);
        }
        /// <summary>
        /// This method creates a session and sets the associated username.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <returns>A session object.</returns>
        IXimuraSession IXimuraSessionManager.SessionCreate(string username)
        {
            if (remoteSessionInvoke == null) 
                return null;

            return remoteSessionInvoke(null, username);
        }
        /// <summary>
        /// This method creates a session and sets the associated username.
        /// </summary>
        /// <param name="domain">The session domain.</param>
        /// <param name="username">The username.</param>
        /// <returns>A session object.</returns>
        IXimuraSession IXimuraSessionManager.SessionCreate(string domain, string username)
        {
            if (remoteSessionInvoke == null) 
                return null;

            return remoteSessionInvoke(domain, username);
        }
        #endregion

		#region IXimuraSecurityManager Members
		/// <summary>
		/// This method resolves and returns a command based on its Guid. 
		/// </summary>
		/// <param name="commandID">The command ID</param>
		/// <returns>A command object if the command is part of the command
		/// collection; otherwise this method will return null.</returns>
		internal IXimuraCommandRQ ResolveCommand(Guid commandID)
		{
			if (cmdCollection.ContainsKey(commandID))
				return cmdCollection[commandID];

			throw new SCMCommandNotFoundException("Command not found: " + commandID.ToString());
		}

        internal bool AddContainerService(Type service, object serviceobj)
        {
            if (!ServiceAllowedCheck(service, true)) return false;
            this.SiteExtended.ServiceContainer.AddService(service, serviceobj);
            return true;
        }

        internal object GetContainerService(Type service)
        {
            if (this.SiteExtended == null) return null;
            return this.SiteExtended.GetService(service);
        }

        internal bool RemoveContainerService(Type service)
        {
            if (!ServiceAllowedCheck(service, true)) return false;
            this.SiteExtended.ServiceContainer.RemoveService(service);
            return true;
        }

        private bool ServiceAllowedCheck(Type service, bool serviceType)
        {
            if (this.SiteExtended == null) return false;

            return true;
        }

		#endregion

        #region IXimuraPoolManager Members

        IXimuraPool IXimuraPoolManager.GetPoolManager(Type objectType)
        {
            return ((IXimuraPoolManager)this).GetPoolManager(objectType, true);
        }

        IXimuraPool IXimuraPoolManager.GetPoolManager(Type objectType, bool buffered)
        {
            if (RH.ValidateInterface(objectType,typeof(IXimuraRQRSEnvelope)))
                return RQRSEnvelopeHelper.GetPoolManager(objectType);

            //All pool managers returned by the command bridge are buffered.
            IXimuraPool pool = mPoolManager.GetPoolManager(objectType, true);

            return pool;
        }

        IXimuraPool<T> IXimuraPoolManager.GetPoolManager<T>()
        {
            return ((IXimuraPoolManager)this).GetPoolManager<T>(false);
        }

        IXimuraPool<T> IXimuraPoolManager.GetPoolManager<T>(bool buffered)
        {
            if (RH.ValidateInterface(typeof(T), typeof(IXimuraRQRSEnvelope)))
                return (IXimuraPool<T>)RQRSEnvelopeHelper.GetPoolManager(typeof(T));

            //All pool managers returned by the command bridge are buffered.
            IXimuraPool<T> pool = (IXimuraPool<T>)mPoolManager.GetPoolManager(typeof(T), true);

            return pool;
        }
        #endregion

        #region IXimuraPerformanceManager Members

        void IXimuraPerformanceManager.PerformanceCounterCollectionRegister(IXimuraPerformanceCounterCollection collection)
        {
            mPerformanceManager.PerformanceCounterCollectionRegister(collection);
        }

        void IXimuraPerformanceManager.PerformanceCounterCollectionUnregister(IXimuraPerformanceCounterCollection collection)
        {
            mPerformanceManager.PerformanceCounterCollectionUnregister(collection);
        }

        #endregion
    }
}