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
using System.Collections;
using System.Threading;

using Ximura;
using Ximura.Helper;
using CH = Ximura.Helper.Common;

#endregion // using
namespace Ximura.Server
{
    /// <summary>
    /// This structure is used hold the job signature.
    /// </summary>
    public struct JobSignature
    {
        /// <summary>
        /// The header
        /// </summary>
        public byte[] encryptHeader;
        /// <summary>
        /// The buffer
        /// </summary>
        public byte[] encryptBuffer;
        /// <summary>
        /// The hash.
        /// </summary>
        public byte[] encryptedHash;
        /// <summary>
        /// This creates the signature. You may pass null as a parameter.
        /// </summary>
        /// <param name="encryptHeader">The header.</param>
        /// <param name="encryptBuffer">The buffer.</param>
        /// <param name="encryptedHash">The hash.</param>
        public JobSignature(
            byte[] encryptHeader, byte[] encryptBuffer, byte[] encryptedHash)
        {
            this.encryptHeader = encryptHeader;
            this.encryptBuffer = encryptBuffer;
            this.encryptedHash = encryptedHash;
        }

        #region Empty
        /// <summary>
        /// This is the empty job signature.
        /// </summary>
        public static readonly JobSignature Empty;

        /// <summary>
        /// This is the static constructor.
        /// </summary>
        static JobSignature()
        {
            Empty = new JobSignature(null, null, null);
        }
        #endregion // Empty
    }
}