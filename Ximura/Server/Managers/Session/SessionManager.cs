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
using System.Collections.Specialized;
using System.Runtime.Serialization;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Drawing;
using System.Security;
using System.Security.Cryptography;
using System.Security.Principal;

using Ximura;
using Ximura.Helper;
using AH=Ximura.Helper.AttributeHelper;
using RH=Ximura.Helper.Reflection;
using Ximura.Server;
using Ximura.Command;
#endregion // using
namespace Ximura.Server
{
	/// <summary>
	/// SessionManager is the default class that all Session Manager services 
	/// should derive from. The SessionManager class authenticates and provides 
	/// session objects to requesting parties.
	/// </summary>
	[ToolboxBitmap(typeof(XimuraResourcePlaceholder),"Ximura.Resources.SessionManager.bmp")]
    [XimuraAppModule("0C3768DE-DB2D-4da7-80F2-11D2C8264D71", "SessionManager")]
    public partial class SessionManager :
        AppServerAgentManager<IXimuraSessionAgent, SSMConfiguration, SSMPerformance>, IXimuraSessionManagerService
	{
		#region Declarations
        private PoolInvocator<SessionJob> mPoolSessionJob = null;
        private Dictionary<Guid, SessionToken> mSessionTokens = null;
        private PoolInvocator<SessionToken> mSessionTokenPool = null;

        IContainer parentContainer;
        #endregion
		#region Constructor
		/// <summary>
		/// This is the constructor used by the Ximura Application model.
		/// </summary>
		/// <param name="container">The control container.</param>
        public SessionManager(IContainer container): base(container)
        {
            parentContainer = container;
        }
		#endregion

        #region InternalStart()/InternalStop()
        /// <summary>
        /// This override creates the session token collection and pools.
        /// </summary>
        protected override void InternalStart()
        {
            base.InternalStart();

            mSessionTokenPool = new PoolInvocator<SessionToken>(delegate() { return new SessionToken(); });

            mSessionTokens = new Dictionary<Guid, SessionToken>();
        }
        /// <summary>
        /// This override cleats the session token collection and session token pool.
        /// </summary>
        protected override void InternalStop()
        {
            mSessionTokenPool.Clear();
            mSessionTokenPool = null;

            mSessionTokens.Clear();
            mSessionTokens = null;

            base.InternalStop();
        }
        #endregion // InternalStop()
        #region ServicesProvide/ServicesRemove
        /// <summary>
        /// This override adds the IXimuraSessionManagerService service to the control container.
        /// </summary>
        protected override void ServicesProvide()
        {
            base.ServicesProvide();

            AddService<IXimuraSessionManagerService>(this);
        }
        /// <summary>
        /// This override removes the IXimuraSessionManagerService service to the control container.
        /// </summary>
        protected override void ServicesRemove()
        {
            RemoveService<IXimuraSessionManagerService>();

            base.ServicesRemove();
        }
        #endregion // ServicesProvide/ServicesRemove


        #region IXimuraSessionManager Members

        IXimuraSession IXimuraSessionManager.SessionCreate()
        {
            return ((IXimuraSessionManager)this).SessionCreate(null, null);
        }

        IXimuraSession IXimuraSessionManager.SessionCreate(string username)
        {
            return ((IXimuraSessionManager)this).SessionCreate(null, username);
        }

        IXimuraSession IXimuraSessionManager.SessionCreate(string domain, string username)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region AgentCreate(XimuraServerAgentHolder holder)
        /// <summary>
        /// This override created the session agent.
        /// </summary>
        /// <param name="holder"></param>
        /// <returns></returns>
        protected override IXimuraSessionAgent AgentCreate(XimuraServerAgentHolder holder)
        {
            if (!RH.ValidateInterface(holder.AgentType, typeof(IXimuraSessionAgent)))
                throw new AgentInvalidTypeException(
                    string.Format("The agent type '{0}' does not implement the IXimuraSessionAgent interface", holder.AgentType.Name));

            IXimuraSessionAgent agent = (IXimuraSessionAgent)RH.CreateObjectFromType(holder.AgentType,new object[]{parentContainer});
            return agent;
        }
        #endregion 
    }
}