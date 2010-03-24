//#region Copyright
//// *******************************************************************************
//// Copyright (c) 2000-2009 Paul Stancer.
//// All rights reserved. This program and the accompanying materials
//// are made available under the terms of the Eclipse Public License v1.0
//// which accompanies this distribution, and is available at
//// http://www.eclipse.org/legal/epl-v10.html
////
//// Contributors:
////     Paul Stancer - initial implementation
//// *******************************************************************************
//#endregion
//﻿#region using
//using System;
//using System.ComponentModel;
//using System.Linq;
//using System.Collections;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.Drawing;
//using System.Threading;
//using System.Security;
//using System.Security.Cryptography;
//using System.Security.Principal;
//using System.Security.Permissions;

//using Ximura;
//
//using Ximura.Framework;

//using Ximura.Framework;
//#endregion // using
//namespace Ximura.Framework
//{
//    public partial class SecurityManager
//    {
//        #region Declarations
//        private SessionManagerCollection mSessionManagerRegistration;

//        private Dictionary<Guid, SessionToken> mSessionTokens = null;

//        private PoolInvocator<SessionToken> mSessionTokenPool = null;
//        #endregion // Declarations

//        #region SessionManagersStart()
//        /// <summary>
//        /// This method starts the session managers, and related services.
//        /// </summary>
//        protected virtual void SessionManagersStart()
//        {
//            mSessionTokenPool = new PoolInvocator<SessionToken>(delegate(){return new SessionToken();});

//            mSessionTokens = new Dictionary<Guid, SessionToken>();

//            mSessionManagerRegistration = new SessionManagerCollection();

//            AddService<IXimuraSessionManagerRegistration>(mSessionManagerRegistration);
//        }
//        #endregion // SessionManagersStart()
//        #region SessionManagersStop()
//        /// <summary>
//        /// This method stops the session managers and related services.
//        /// </summary>
//        protected virtual void SessionManagersStop()
//        {
//            RemoveService<IXimuraSessionManagerRegistration>();
//            mSessionManagerRegistration = null;

//            mSessionTokens.Clear();
//            mSessionTokens = null;
//        }
//        #endregion // SessionManagersStop()

//        #region SessionManagerAction
//        /// <summary>
//        /// This method is used to process actions from the session manager.
//        /// </summary>
//        /// <param name="actionType"></param>
//        /// <param name="sessionManager"></param>
//        /// <param name="session"></param>
//        /// <param name="data"></param>
//        protected virtual void SessionManagerAction(
//            SessionActionType actionType, IXimuraSessionManager sessionManager, IXimuraSessionSCM session, object data)
//        {
//            switch (actionType)
//            {
//                case SessionActionType.Register:
//                    SessionRegister(sessionManager, session);
//                    break;
//                case SessionActionType.Release:
//                    SessionRelease(sessionManager, session);
//                    break;
//                case SessionActionType.ProfileChanged:
//                    SessionProfileChanged(sessionManager, session, (SessionProfileLevel)data);
//                    break;
//                default:
//                    throw new ArgumentOutOfRangeException("actionType", "The SessionActionType parameter is out of range.");
//            }
//        }
//        #endregion // SessionManagerAction

//        #region SessionRegister
//        /// <summary>
//        /// This method registers a new session with the Security Manager
//        /// </summary>
//        /// <param name="theSession">The session</param>
//        protected virtual void SessionRegister(
//            IXimuraSessionManager sessionManager, IXimuraSessionSCM theSession)
//        {
//            //Check to see whether the session already exists
//            if (mSessionTokens.ContainsKey(theSession.SessionID))
//                throw new Ximura.Framework.SecurityException("SessionRegister: the session already exists.");

//            //Create the new SessionEvidence object
//            theSession.SetSCMPublicKey = SCMRSAPublicKey;
//            //SessionToken token = new SessionToken(theSession as Session, JobProcess, JobCancel, JobComplete);
//            SessionToken token = new SessionToken();

//            //Add the key and the evidence to the collection
//            mSessionTokens.Add(theSession.SessionID, token);
//        }
//        #endregion // SessionRegister
//        #region SessionRelease
//        /// <summary>
//        /// This method releases a session from the Session Manager
//        /// </summary>
//        /// <param name="theSession">The session</param>
//        protected virtual void SessionRelease(
//            IXimuraSessionManager sessionManager, IXimuraSessionSCM theSession)
//        {
//            SessionToken token = mSessionTokens[theSession.SessionID];
//            PurgeExistingJobs(token);
//            mSessionTokens.Remove(theSession.SessionID);
//        }
//        #endregion // SessionRelease
//        #region SessionProfileChange
//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="theSession"></param>
//        /// <param name="theLevel"></param>
//        protected virtual void SessionProfileChanged(
//            IXimuraSessionManager sessionManager, IXimuraSessionSCM theSession, SessionProfileLevel theLevel)
//        {
//            SessionToken token = mSessionTokens[theSession.SessionID];
//            //token.setAccessProfile(theLevel);
//        }
//        #endregion // SessionProfileChange


//        #region SessionCreateRemote
//        /// <summary>
//        /// This command is called by the Command bridge delegate initiated by 
//        /// commands that wish to create a session object.
//        /// </summary>
//        /// <param name="domain">The domain for the session or null for the default domain.</param>
//        /// <param name="username">The username for the session or null.</param>
//        /// <returns>A session object, or null if the session manager cannot be resolved.</returns>
//        private IXimuraSession SessionCreateRemote(string domain, string username)
//        {
//            //Set the domain to lower case to ensure compatibility.
//            if (domain != null)
//                domain = domain.ToLowerInvariant();

//            ////If a domain is specified, but the manager is not registered, return null.
//            //if (domain != null && domainSessionManager.ContainsKey(domain))
//            //    return domainSessionManager[domain].createSession(username);

//            ////Either a domain was specified and is not available, or no domain was specified
//            ////but there is no default session manager, so return null.
//            //if (domain != null || sessionManagerDefault == null)
//            //    return null;

//            //OK, no domain was specified and there is a default session manager,
//            //do return a session using the default session manager.
//            return null;// mSessionManagers.Values.Single().CreateSession(domain, username);
//        }
//        #endregion // internalCreateRemote

//        #region Class -> SessionManagerCollection
//        /// <summary>
//        /// This method holds the session managers.
//        /// </summary>
//        private class SessionManagerCollection : IXimuraSessionManagerRegistration
//        {
//            #region Declarations
//            private Dictionary<string, IXimuraSessionManager> mSessionManagerDomainCache;
//            private List<IXimuraSessionManager> mSessionManagers;
//            private object syncCollectionLock = new object();
//            #endregion // Declarations

//            #region Constructor
//            internal SessionManagerCollection()
//            {
//                mSessionManagers = new List<IXimuraSessionManager>();
//            }
//            #endregion // Constructor

//            #region IXimuraSessionManagerRegistration Members

//            void IXimuraSessionManagerRegistration.RegisterSessionManager(IXimuraSessionManager sessionManager)
//            {
//                lock (syncCollectionLock)
//                {
//                    if (!mSessionManagers.Contains(sessionManager))
//                    {
//                        mSessionManagers.Add(sessionManager);
//                    }
//                    else
//                        throw new ArgumentOutOfRangeException("sessionManager", "The session manager has already been registered");
//                }
//            }

//            void IXimuraSessionManagerRegistration.UnregisterSessionManager(IXimuraSessionManager sessionManager)
//            {
//                lock (syncCollectionLock)
//                {
//                    if (mSessionManagers.Contains(sessionManager))
//                    {
//                        mSessionManagers.Remove(sessionManager);
//                    }
//                    else
//                        throw new ArgumentOutOfRangeException("sessionManager", "The session manager is not registered");
//                }
//            }

//            #endregion
//        }
//        #endregion // SessionManagerCollection

//    }
//}
