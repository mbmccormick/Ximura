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
using System.Globalization;
using System.Security;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Security.Policy;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Diagnostics;

using Ximura;
using Ximura.Framework;
using Ximura.Framework;
using Ximura.Helper;
using CH=Ximura.Helper.Common;
#endregion // using
namespace Ximura.Framework
{
	/// <summary>
	/// A session is created for each is used to user connection. 
	/// The session object tracks the actions that a user has requested 
	/// and monitor time outs and other session housekeeping functions.  
	/// </summary>
    public abstract partial class SessionBase : IXimuraSession, IXimuraSessionNegotiate
	{
        #region Declarations

		private string mUserID = null;
        private string mRealm = null;

        protected CultureInfo mCulture;

        protected SessionAccessToken mSAT = null;
		#endregion
		#region Constructors
		/// <summary>
		/// This base session constructor that points to the parent session manager.
		/// </summary>
		/// <param name="sesMan">The Session Manager</param>
		/// <param name="secMan">The Security Manager</param>
        protected SessionBase(SessionAccessToken sat)
		{
            mSAT = sat;
		}
		#endregion

		#region SessionCulture
		/// <summary>
		/// The session culture
		/// </summary>
		public virtual CultureInfo SessionCulture
		{
			get
            {
                return mCulture;
            }
			set
			{
				//First of all if we aren't changing anything get out of here.
				if (mCulture == value) 
					return;

                if (!IsAuthenticated)
                    throw new Ximura.Framework.SecurityException("Culture cannot be changed.");

				mCulture=value;
			}
		}
		#endregion // SessionCulture

		#region SessionID
		/// <summary>
		/// This is the current session ID
		/// </summary>
		public Guid SessionID
		{
			get
            {
                if (mSAT == null)
                    return Guid.Empty; ;
                return mSAT.ID;
            }
		}
		#endregion // SessionID
		#region State
		/// <summary>
		/// This is the current session state.
		/// </summary>
		public SessionState State
		{
			get
            {
                if (mSAT == null)
                    return SessionState.Undefined;
                return mSAT.State;
            }
		}
		#endregion // State

		#region UserID
		/// <summary>
		/// This is the user ID for the session. This may return null is a user has not been set.
		/// </summary>
		public string UserID
		{
			get
			{
				// TODO:  Add Session.Username getter implementation
				return mUserID;
			}
			set
			{
                if (!(State == SessionState.NotAuthorized || State == SessionState.Initialized))
                    throw new SecurityException("Username cannot be changed.");

				mUserID=value;
			}
		}
		#endregion // UserID
        #region Realm
        /// <summary>
        /// The session realm.
        /// </summary>
        public string Realm
        {
            get { return mRealm; }
        }

        #endregion

        #region IIdentity Members

        public string AuthenticationType
        {
            get 
            {
                if (mSAT == null)
                    return null;
                return mSAT.AuthenticationType; 
            }
        }

        public bool IsAuthenticated
        {
            get
            {
                if (mSAT == null)
                    return false;
                return mSAT.IsAuthenticated;
            }
        }

        public string Name
        {
            get
            {
                if (mSAT == null)
                    return null;
                return mSAT.Name;
            }
        }

        #endregion

        #region AuthenticationTypes
        /// <summary>
        /// This method returns the authentication interfaces implemented by this session. You should override this
        /// method if your session supports authentication methods.
        /// </summary>
        public virtual IEnumerable<Type> AuthenticationTypes
        {
            get 
            {
                yield break;
            }
        }
        #endregion // AuthenticationTypes

    }
}