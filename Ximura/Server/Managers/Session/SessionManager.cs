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
	public partial class SessionManager :
        AppServerAgentManager<IXimuraSessionAgent, SSMConfiguration, SSMPerformance>, IXimuraSessionManager
	{
		#region Declarations
        private PoolInvocator<SessionJob> mPoolSessionJob = null;
        private Dictionary<Guid, SessionToken> mSessionTokens = null;
        private PoolInvocator<SessionToken> mSessionTokenPool = null;
        #endregion
		#region Constructor
		/// <summary>
		/// This is the constructor used by the Ximura Application model.
		/// </summary>
		/// <param name="container">The control container.</param>
        public SessionManager(IContainer container): base(container)
        {

        }
		#endregion

        protected override void InternalStart()
        {
            base.InternalStart();

            mSessionTokenPool = new PoolInvocator<SessionToken>(delegate() { return new SessionToken(); });

            mSessionTokens = new Dictionary<Guid, SessionToken>();
        }

        protected override void InternalStop()
        {
            mSessionTokenPool.Clear();
            mSessionTokenPool = null;

            mSessionTokens.Clear();
            mSessionTokens = null;

            base.InternalStop();
        }


        #region IXimuraSessionManager Members

        IXimuraSession IXimuraSessionManager.SessionCreate()
        {
            throw new NotImplementedException();
        }

        IXimuraSession IXimuraSessionManager.SessionCreate(string username)
        {
            throw new NotImplementedException();
        }

        IXimuraSession IXimuraSessionManager.SessionCreate(string domain, string username)
        {
            throw new NotImplementedException();
        }

        #endregion

        protected override IXimuraSessionAgent AgentCreate(XimuraServerAgentHolder holder)
        {
            throw new NotImplementedException();
        }
    }
}