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
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Drawing;
using System.Threading;
using System.Reflection;
using System.Linq;
using System.IO;
using System.IO.Compression;
using System.Xml;
using System.Security.Cryptography;

using Ximura;
using Ximura.Data;

using Ximura.Framework;
using Ximura.Framework;
using AH = Ximura.AttributeHelper;
using RH = Ximura.Reflection;
using CH = Ximura.Common;
#endregion
namespace Ximura.Framework
{
    public partial class AppServer<CONFSYS, CONFCOM, PERF>
    {
        #region Declarations
        /// <summary>
        /// This dictionary holds the session tokens.
        /// </summary>
        protected Dictionary<Guid, SessionToken> mSessionTokens;
        #endregion // Declarations

        #region SessionService
        /// <summary>
        /// This is the session manager for the application.
        /// </summary>
        protected virtual SessionManager SessionService
        {
            get;
            set;
        }
        #endregion // Sessions

        #region SessionManagerCreate()
        /// <summary>
        /// This method creates the session manager and adds the session agents.
        /// </summary>
        protected virtual void SessionManagerCreate()
        {
            SessionService = new SessionManager(ControlContainer);
            AgentsAdd<XimuraSessionManagerAttribute>(SessionAgentsDefault, SessionService);
        }
        #endregion // SessionCreate()
        #region SessionManagerDispose()
        /// <summary>
        /// This method disposes of the session manager and its agents.
        /// </summary>
        protected virtual void SessionManagerDispose()
        {
            SessionService.Dispose();
            SessionService = null;
        }
        #endregion // SessionDispose()

        #region SessionManagerStart()
        /// <summary>
        /// This method starts the session managers, and related services.
        /// </summary>
        protected virtual void SessionManagerStart()
        {
            mSessionTokens = new Dictionary<Guid, SessionToken>();

            SessionService.Start();
        }
        #endregion // SessionManagersStart()
        #region SessionManagerStop()
        /// <summary>
        /// This method stops the session managers and related services.
        /// </summary>
        protected virtual void SessionManagerStop()
        {
            mSessionTokens.Clear();
        }
        #endregion // SessionManagersStop()

        #region SessionAgentsDefault
        /// <summary>
        /// This property returns a collection of default session managers for the system. Typically this collection only
        /// contains the SessionManagerSystem. You should override this method if you require additional session managers.
        /// </summary>
        protected virtual IEnumerable<XimuraServerAgentHolder> SessionAgentsDefault
        {
            get
            {
                yield return new XimuraServerAgentHolder(typeof(SessionAgentSystem), "SYSTEM", "");
            }
        }
        #endregion // SessionManagersDefault

        #region SessionManagerAction
        /// <summary>
        /// This method is used to process actions from the session manager.
        /// </summary>
        /// <param name="actionType"></param>
        /// <param name="sessionManager"></param>
        /// <param name="session"></param>
        /// <param name="data"></param>
        protected virtual void SessionManagerAction(
            SessionActionType actionType, IXimuraSessionManager sessionManager, IXimuraSessionSCM session, object data)
        {
            switch (actionType)
            {
                case SessionActionType.Register:
                    SessionRegister(sessionManager, session);
                    break;
                case SessionActionType.Release:
                    SessionRelease(sessionManager, session);
                    break;
                case SessionActionType.ProfileChanged:
                    SessionProfileChanged(sessionManager, session, (SessionProfileLevel)data);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("actionType", "The SessionActionType parameter is out of range.");
            }
        }
        #endregion // SessionManagerAction

        #region SessionRegister
        /// <summary>
        /// This method registers a new session with the Security Manager
        /// </summary>
        /// <param name="theSession">The session</param>
        protected virtual void SessionRegister(
            IXimuraSessionManager sessionManager, IXimuraSessionSCM theSession)
        {
            //Check to see whether the session already exists
            if (mSessionTokens.ContainsKey(theSession.SessionID))
                throw new Ximura.Framework.SecurityException("SessionRegister: the session already exists.");

            //Create the new SessionEvidence object
            //theSession.SetSCMPublicKey = SCMRSAPublicKey;
            //SessionToken token = new SessionToken(theSession as Session, JobProcess, JobCancel, JobComplete);
            SessionToken token = new SessionToken();

            //Add the key and the evidence to the collection
            mSessionTokens.Add(theSession.SessionID, token);
        }
        #endregion // SessionRegister
        #region SessionRelease
        /// <summary>
        /// This method releases a session from the Session Manager
        /// </summary>
        /// <param name="theSession">The session</param>
        protected virtual void SessionRelease(
            IXimuraSessionManager sessionManager, IXimuraSessionSCM theSession)
        {
            SessionToken token = mSessionTokens[theSession.SessionID];
            PurgeExistingJobs(token);
            mSessionTokens.Remove(theSession.SessionID);
        }
        #endregion // SessionRelease
        #region SessionProfileChange
        /// <summary>
        /// 
        /// </summary>
        /// <param name="theSession"></param>
        /// <param name="theLevel"></param>
        protected virtual void SessionProfileChanged(
            IXimuraSessionManager sessionManager, IXimuraSessionSCM theSession, SessionProfileLevel theLevel)
        {
            SessionToken token = mSessionTokens[theSession.SessionID];
            //token.setAccessProfile(theLevel);
        }
        #endregion // SessionProfileChange

        #region SessionCreateRemote
        /// <summary>
        /// This command is called by the Command bridge delegate initiated by 
        /// commands that wish to create a session object.
        /// </summary>
        /// <param name="domain">The domain for the session or null for the default domain.</param>
        /// <param name="username">The username for the session or null.</param>
        /// <returns>A session object, or null if the session manager cannot be resolved.</returns>
        private IXimuraSession SessionCreateRemote(string domain, string username)
        {
            //Set the domain to lower case to ensure compatibility.
            if (domain != null)
                domain = domain.ToLowerInvariant();

            ////If a domain is specified, but the manager is not registered, return null.
            //if (domain != null && domainSessionManager.ContainsKey(domain))
            //    return domainSessionManager[domain].createSession(username);

            ////Either a domain was specified and is not available, or no domain was specified
            ////but there is no default session manager, so return null.
            //if (domain != null || sessionManagerDefault == null)
            //    return null;

            //OK, no domain was specified and there is a default session manager,
            //do return a session using the default session manager.
            return null;// mSessionManagers.Values.Single().CreateSession(domain, username);
        }
        #endregion // internalCreateRemote
    }
}
