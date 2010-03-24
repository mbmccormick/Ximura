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
﻿#region using
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Drawing;
using System.Security;
using System.Security.Cryptography;
using System.Security.Principal;

using Ximura;

using Ximura.Framework;
using Ximura.Framework;
#endregion // using
namespace Ximura.Framework
{
    public abstract partial class SessionAgentBase<S, CONF>
    {
        #region Declarations
        /// <summary>
        /// This protected hashtable contains a collection of session
        /// </summary>
        protected Dictionary<Guid, S> mSessionCollection = new Dictionary<Guid, S>();
        #endregion // Declarations

        #region SessionCreate() helpers
        /// <summary>
        /// This method creates a blank session
        /// </summary>
        /// <returns>A session object.</returns>
        public virtual S SessionCreate()
        {
            return SessionCreate(null, null);
        }
        /// <summary>
        /// This method creates a session for the valid userID.
        /// </summary>
        /// <param name="userID">The user ID of the session</param>
        /// <returns>A session object.</returns>
        public virtual S SessionCreate(string userID)
        {
            return SessionCreate(null, userID);
        }
        #endregion // createSession()
        #region SessionCreate(string domain, string userID)
        /// <summary>
        /// This method creates a session for the valid userID.
        /// </summary>
        /// <param name="domain">The session domain.</param>
        /// <param name="userID">The user ID of the session</param>
        /// <returns>A session object.</returns>
        public abstract S SessionCreate(string domain, string userID);
        //{
            ////Check whether the userID is blank and not allowed.
            //if ((userID == null || userID == "")
            //    && !Settings.SettingsRealm(domain).allowBlankSession)
            //    throw new SessionSecurityException("Blank sessions are not supported. Please supply a username.");

            ////Check whether the user ID is valid.
            //if (!Settings.SettingsRealm(domain).verifyUserID(userID))
            //    throw new SessionSecurityException("The user ID does not exist.");

            ////Create the new session
            //Session newSession = 
            //    new Session(domain, this as IXimuraSessionManagerAuth, securityMan as IXimuraSecurityManagerSession);

            ////Set the userID if set.
            //if (userID != null && userID != "")
            //    newSession.UserID=userID;

            ////Best check for extreme weirdness
            //if (mSessionCollection.ContainsKey(newSession.SessionID))
            //    throw new SessionSecurityException("Session ID already exists! This shouldn't happen.");

            ////Add the session to the session collection
            //mSessionCollection.Add(newSession.SessionID, newSession);

            ////Register the session with the security manager
            //securityMan.SessionRegister(newSession as IXimuraSessionSCM);
            //S newSession = new S();


            //return new S();
        //}
        #endregion // CreateSession(string domain, string userID)

        #region Logout(IXimuraSessionSCM theSession)
        /// <summary>
        /// This method logs out a session and resets its permission to an
        /// unathorized user.
        /// </summary>
        /// <param name="theSession">The session to logout.</param>
        public virtual void Logout(IXimuraSessionSCM theSession)
        {
            //Remove the security
            //securityMan.SessionProfileChanged(theSession, SessionProfileLevel.Unauthorized);
        }
        #endregion // IXimuraSessionManagerAuth.Logout(IXimuraSessionSCM theSession)

        #region Close(IXimuraSessionSCM theSession)
        /// <summary>
        /// This method closes a session and removes it from the session collection.
        /// </summary>
        /// <param name="theSession">The session object to close.</param>
        public virtual void Close(IXimuraSessionSCM theSession)
        {
            //First check that the session exists
            if (!mSessionCollection.ContainsKey(theSession.SessionID))
                return;
            //throw new SessionException("The session does not exist.");

            ////TODO: Handle Dispose properly.
            ////Tell the security manager that this session is being closed.
            //if (securityMan == null || mSessionCollection.Count == 0) return;

            //securityMan.SessionRelease(theSession);

            ////Remove the session
            //mSessionCollection.Remove(theSession.SessionID);
        }
        #endregion // IXimuraSessionManagerAuth.Close(IXimuraSessionSCM theSession)
    }
}
