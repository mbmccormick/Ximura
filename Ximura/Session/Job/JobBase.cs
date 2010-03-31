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
using System.ComponentModel;

using Ximura;
using CH = Ximura.Common;
#endregion // using
namespace Ximura
{
	/// <summary>
	/// This is an abstract class that all job based implementations inherit from.
	/// </summary>
	public abstract class JobBase: IDisposable, IXimuraPoolableObject
	{
        #region Static IDBuffer method
        /// <summary>
        /// This static method is use to create the byte buffer
        /// containing the three ids.
        /// </summary>
        /// <param name="id1">The Session ID</param>
        /// <param name="id2">The Job ID.</param>
        /// <param name="id3">The Request ID.</param>
        /// <returns>A byte buffer</returns>
        public static byte[] IDBuffer(Guid id1, Guid id2, Guid id3)
        {
            byte[] buffer = new byte[48];

            Buffer.BlockCopy(id1.ToByteArray(), 0, buffer, 0, 16);
            Buffer.BlockCopy(id2.ToByteArray(), 0, buffer, 16, 16);
            Buffer.BlockCopy(id3.ToByteArray(), 0, buffer, 32, 16);

            return buffer;
        }
        #endregion // Static CreateBuffer method
		#region Declarations
		private Guid mTrackID=Guid.NewGuid();
        private bool mDisposed = false;
		#endregion // Declarations

		/// <summary>
		/// This is the job priority.
		/// </summary>
		public abstract JobPriority Priority{get;set;}
		/// <summary>
		/// The Session ID
		/// </summary>
		public abstract Guid? SessionID{get;}
		/// <summary>
		/// The job ID
		/// </summary>
		public abstract Guid? ID{get;}
		/// <summary>
		/// The envelope containing the request
		/// </summary>
		public abstract IXimuraRQRSEnvelope Data{get;}
		/// <summary>
		/// The job signature
		/// </summary>
		public abstract JobSignature? Signature{get;}
		/// <summary>
		/// The ID buffer
		/// </summary>
		/// <returns>The byte array containing the buffer.</returns>
		public abstract byte[] IDBuffer();

        /// <summary>
        /// This property provides access to the envelope helper.
        /// </summary>
        public abstract IXimuraEnvelopeHelper EnvelopeHelper { get; }

        #region IDisposable Members
        /// <summary>
        /// This is the dispose method for the job.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }
        /// <summary>
        /// The class destructor.
        /// </summary>
        ~JobBase()
        {
            this.Dispose(false);
        }
        /// <summary>
        /// This method should be overriden to provide specific clean up code.
        /// Specifically, any delegates references in the object should be set to null;
        /// </summary>
        /// <param name="disposing">This parameter is true if the call is from the disposable interface.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!mDisposed && disposing)
                Reset();

            mDisposed = true;
        }
        #endregion

        #region IsDisposed
        /// <summary>
        /// This property can be used to identify whether the object has been disposed.
        /// </summary>
        public bool IsDisposed
        {
            get
            {
                return mDisposed;
            }
        }
        #endregion // IsDisposed

        #region CanPool
        /// <summary>
        /// This method returns true if the object can be pooled.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual bool CanPool
        {
            get { return true; }
        }
        #endregion // CanPool
        #region TrackID
        /// <summary>
		/// This property is used to track the object through the object pool.
		/// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual Guid TrackID
        {
            get
            {
                return mTrackID;
            }
        }
        #endregion // TrackID
        #region Reset()
        /// <summary>
        /// This virtual empty method should be implemented when resources may not be 
        /// automatically cleaned up when using garbage collection, i.e. when using delegates.
        /// </summary>
        public virtual void Reset()
        {
        } 
        #endregion // Reset()
    }
}