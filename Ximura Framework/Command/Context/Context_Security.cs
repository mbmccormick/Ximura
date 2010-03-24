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
using Ximura.Data;
using CH = Ximura.Helper.Common;
using Ximura.Framework;

#endregion // using
namespace Ximura.Framework
{
    public partial class Context<ST, SET, CONF, PERF>
    {
        #region Declarations
        /// <summary>
        /// This is the internal session which can be used to make requests to the rest of the 
        /// Ximura application.
        /// </summary>
        IXimuraSessionRQ mContextSession = null;

        private string mDomain;
        private string mUserName;
        #endregion // Declarations

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

    }
}
