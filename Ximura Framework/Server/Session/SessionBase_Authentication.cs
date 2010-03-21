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
using Ximura.Command;
using Ximura.Server;
using Ximura.Helper;
using CH = Ximura.Helper.Common;
#endregion // using
namespace Ximura.Server
{
    public abstract partial class SessionBase
    {
        //#region Declarations
        ////Security stuff
        //private RSACryptoServiceProvider mSessionRSAKey = null;
        //private RSACryptoServiceProvider SCMSessionRSAPublicKey = null;
        //private MD5CryptoServiceProvider mMD5Hash = null;
        //#endregion // Declarations
        #region PasswordChangeSupported()
        /// <summary>
        /// This method identifies whether the user is able to change their password.
        /// </summary>
        /// <returns>Returns true if they can change their password.</returns>
        public virtual bool PasswordChangeSupported()
        {
            return false;
        }
        #endregion

        #region Logout()
        /// <summary>
        /// This method closes the session
        /// </summary>
        public virtual void Logout()
        {
            //mSesMan.Logout(this);
        }
        #endregion // Logout()
        #region Close()
        /// <summary>
        /// This method closes the session
        /// </summary>
        public virtual void Close()
        {
            //mSesMan.Close(this);
        }
        #endregion // Close()

        #region CalcSig(Guid jobid, IXimuraRQRSEnvelope data)
        /// <summary>
        /// This method caclculates the job signature.
        /// </summary>
        /// <param name="jobid">The job id.</param>
        /// <param name="data">The request data.</param>
        /// <returns>Returns the job signature or null if the signature is not supported.</returns>
        protected virtual JobSignature? CalcSig(Guid jobid, IXimuraRQRSEnvelope data)
        {
            //byte[] buffer = Job.IDBuffer(SessionID, jobid, data.Request.ID);

            //JobSignature sig;

            //sig.encryptHeader = this.SCMSessionRSAPublicKey.Encrypt(buffer, false);
            //sig.encryptBuffer = null;
            ////TODO: This should be signed with the private key.
            //sig.encryptedHash = this.mSessionRSAKey.SignData(buffer, MD5Hash);

            //return sig;
            return null;
        }
        #endregion // CalcSig(Guid jobid, IXimuraRQRSEnvelope data)

        #region Old code
        ///// <summary>
        ///// This method attempts to authenticate the sessio based on the hash passed.
        ///// </summary>
        ///// <param name="hash">The hash to compare.</param>
        ///// <returns>The current session state</returns>
        //public SessionState Authenticate(byte[] hash)
        //{
        //    return Authenticate(hash, this.SessionCulture);
        //}
        ///// <summary>
        ///// This method will attempt to authenticate the session.
        ///// </summary>
        ///// <param name="hash">The MD5 hash of the username and the seed</param>
        ///// <param name="sessionCulture">The sessio culture</param>
        ///// <returns>The session state after the authtication attempt</returns>
        //public SessionState Authenticate(byte[] hash, CultureInfo sessionCulture)
        //{
        //    //SessionState result = mSesMan.Authenticate(this, hash);
        //    //mState = result;

        //    //if (result == SessionState.Authenticated)
        //    //    mCulture = sessionCulture;

        //    return SessionState.Rejected;
        //}
        //#endregion // Authenticate
        //#region GetSeed()
        ///// <summary>
        ///// This method returns the seed for the current user.
        ///// </summary>
        ///// <returns>A byte array container the user ID seed.</returns>
        //public byte[] GetSeed()
        //{
        //    return null;
        //}
        ///// <summary>
        ///// This method returns the seed for the user ID.
        ///// </summary>
        ///// <param name="userID">The user ID.</param>
        ///// <returns>A byte array container the user ID seed.</returns>
        //public byte[] GetSeed(string userID)
        //{
        //    return GetSeed(userID, this.SessionCulture);
        //}
        ///// <summary>
        ///// This method returns the seed for the user ID.
        ///// </summary>
        ///// <param name="userID">The user ID.</param>
        ///// <param name="newCulture">The culture for the session.</param>
        ///// <returns>The byte array containing the seed</returns>
        //public byte[] GetSeed(string userID, CultureInfo newCulture)
        //{
        //    this.UserID = userID;
        //    this.SessionCulture = newCulture;

        //    return null;
        //}
        //#endregion // getSeed()


        //#region SetSCMPublicKey
        ///// <summary>
        ///// The session security parameters. This method returns the public keys 
        ///// of the sessions, and set stores the public key of the security manager token.
        ///// </summary>
        //public RSAParameters SetSCMPublicKey
        //{
        //    set
        //    {
        //        if (SCMSessionRSAPublicKey != null)
        //            throw new Ximura.Server.SecurityException("SCM Public Key is already set.");

        //        SCMSessionRSAPublicKey = new RSACryptoServiceProvider();
        //        SCMSessionRSAPublicKey.ImportParameters(value);
        //    }
        //}
        //#endregion // SetSCMPublicKey
        //#region GetSessionPublicKey
        ///// <summary>
        ///// This is the RSA public key of the session object.
        ///// </summary>
        //public RSAParameters GetSessionPublicKey
        //{
        //    get
        //    {
        //        return SessionRSAKey.ExportParameters(false);
        //    }
        //}
        //#endregion // GetSessionPublicKey
        //#region Crypto
        //private JobSignature? CalcSig(Guid jobid, IXimuraRQRSEnvelope data)
        //{
        //    //byte[] buffer = Job.IDBuffer(SessionID, jobid, data.Request.ID);

        //    //JobSignature sig;

        //    //sig.encryptHeader = this.SCMSessionRSAPublicKey.Encrypt(buffer, false);
        //    //sig.encryptBuffer = null;
        //    ////TODO: This should be signed with the private key.
        //    //sig.encryptedHash = this.mSessionRSAKey.SignData(buffer, MD5Hash);

        //    //return sig;
        //    return null;
        //}

        //private MD5CryptoServiceProvider MD5Hash
        //{
        //    get
        //    {
        //        if (mMD5Hash == null)
        //            mMD5Hash = new MD5CryptoServiceProvider();
        //        return mMD5Hash;
        //    }
        //}

        //private RSACryptoServiceProvider SessionRSAKey
        //{
        //    get
        //    {
        //        if (mSessionRSAKey == null)
        //            mSessionRSAKey = new RSACryptoServiceProvider();
        //        return mSessionRSAKey;
        //    }
        //}
        #endregion
    }
}
