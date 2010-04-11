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
using System.Data;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Collections;
using System.Collections.Generic;

using Ximura;
using Ximura.Data;
#endregion // using
namespace Ximura
{
    /// <summary>
    /// IXimuraRQRSEnvelope is used to hold the information needed for a system request, and is the 
    /// primary object used to transmit information arounds the Ximura system.
    /// </summary>
    [Serializable]
    public class RQRSContract<RQ, RS> : IXimuraRQRSEnvelope,
        IXimuraPoolReturnable, ISerializable, IDeserializationCallback, IXimuraPoolableObject
        where RS : RQRSFolder, new()
        where RQ : RQRSFolder, new()
    {
        #region Declarations
        /// <summary>
        /// This ID is used to track the lifecycle of the poolable object, and does not change throughout
        /// the lifetime of the object.
        /// </summary>
        private Guid mPoolTrackingID = Guid.NewGuid();
        /// <summary>
        /// This variable determines the maximum number of hops that the Envelope
        /// can pass through before it expires.
        /// </summary>
        protected int mTTL;
        /// <summary>
        /// This is the destination for the Request
        /// </summary>
        protected EnvelopeAddress destination;

        Guid mJobSessionID;
        Guid mSenderID;
        Guid? mSenderReference;
        string mJobSessionReferenceID;
        byte[] mJobSecurityIdentifier;

        private IXimuraPool mObjectPool = null;

        private RQ mRequest;
        private RS mResponse;
        #endregion
        #region Constructors
        /// <summary>
        /// The default constructor.
        /// </summary>
        public RQRSContract() 
        {
            mRequest = new RQ();
            mResponse = new RS();
            Reset();
        }

        /// <summary>
        /// The constructor that specifies the destination for the request.
        /// </summary>
        /// <param name="destination"></param>
        protected RQRSContract(EnvelopeAddress destination)
            : this()
        {
            this.destination = destination;
        }

        /// <summary>
        /// This is the deserialization constructor.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected RQRSContract(SerializationInfo info, StreamingContext context)
            : this()
        {
            Reset(info, context);
        }
        #endregion

        #region Reset()
        /// <summary>
        /// This method resets the object.
        /// </summary>
        public virtual void Reset()
        {
            mTTL = 10;
            mObjectPool = null;
            destination = EnvelopeAddress.NullDestination;

            mJobSessionID = Guid.Empty;
            mSenderID = Guid.Empty;
            mSenderReference = null;
            mJobSessionReferenceID = null;
            mJobSecurityIdentifier = null;

            mRequest.Reset();
            mResponse.Reset();
        }
        #endregion

        #region DestinationAddress
        /// <summary>
        /// The destination address for the envelope.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public EnvelopeAddress DestinationAddress
        {
            get { return destination; }
            set { destination = value; }
        }
        #endregion
        #region Sender
        /// <summary>
        /// The original sender
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Guid Sender
        {
            get { return mSenderID; }
            set { mSenderID = value; }
        }
        #endregion
        #region SenderReference
        /// <summary>
        /// The original sender internal reference 
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Guid? SenderReference
        {
            get { return mSenderReference; }
            set { mSenderReference = value; }
        }
        #endregion

        #region Folder shortcuts: Request/Response
        #region Request
        /// <summary>
        /// The Contract Request property
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public RQ ContractRequest
        {
            get { return mRequest; }
        }
        /// <summary>
        /// The Request property
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public RQRSFolder Request
        {
            get { return ContractRequest as RQRSFolder; }
        }
        #endregion
        #region Response
        /// <summary>
        /// The Response property
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public RS ContractResponse
        {
            get { return mResponse; }
        }
        /// <summary>
        /// The backwards compatible response property
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public RQRSFolder Response
        {
            get { return ContractResponse as RQRSFolder; }
        }
        /// <summary>
        /// This shortcut is used to prepare the response status and substatus.
        /// </summary>
        /// <param name="status"></param>
        /// <param name="subStatus"></param>
        public void PrepareResponse(string status, string subStatus)
        {
            ContractResponse.Status = status;
            ContractResponse.Substatus = subStatus;
        }
        #endregion
        #endregion

        #region JobUserID
        /// <summary>
        /// This is the job user ID.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Guid JobUserID
        {
            get { return mJobSessionID; }
            set { mJobSessionID = value; }
        }
        #endregion // JobUserID
        #region JobUserReferenceID
        /// <summary>
        /// This is the job user plain text reference ID.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string JobUserReferenceID
        {
            get { return mJobSessionReferenceID; }
            set { mJobSessionReferenceID = value; }
        }
        #endregion // JobUserReferenceID
        #region JobSecurityIdentifier
        /// <summary>
        /// This is the unique job security reference ID.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public byte[] JobSecurityIdentifier
        {
            get { return mJobSecurityIdentifier; }
            set { mJobSecurityIdentifier = value; }
        }
        #endregion // JobSecurityIdentifier

        #region IXimuraPoolableObjectDeserializable Members
        /// <summary>
        /// This propoerty indicates whether the object support a deserialization reset.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual bool CanResetWithDeserialization { get { return false; } }
        /// <summary>
        /// This is the deserialization reset method.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public void Reset(SerializationInfo info, StreamingContext context)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }
        #endregion

        #region IXimuraPoolReturnable Members
        /// <summary>
        /// This property contains a reference to the object pool for the IXimuraRQRSEnvelope.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IXimuraPool ObjectPool
        {
            get
            {
                return mObjectPool;
            }
            set
            {
                mObjectPool = value;
            }
        }
        /// <summary>
        /// This property returns true if the object can be returns to the pool.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool ObjectPoolCanReturn
        {
            get { return mObjectPool != null; }
        }
        /// <summary>
        /// This method returns the IXimuraRQRSEnvelope to the pool.
        /// </summary>
        public void ObjectPoolReturn()
        {
            mObjectPool.Return(this);
        }
        #endregion

        #region IXimuraPoolableObject Members
        /// <summary>
        /// This method returns true if the object can be pooled.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual bool CanPool
        {
            get { return true; }
        }
        /// <summary>
        /// This property is the pool tracking ID.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Guid TrackID
        {
            get { return mPoolTrackingID; }
        }
        #endregion

        #region ISerializable Members

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }

        #endregion

        #region IDeserializationCallback Members

        public void OnDeserialization(object sender)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }

        #endregion

        #region IDisposable Members/Finalize
        private bool disposed = false;
        ///// <summary>
        ///// The finalizer
        ///// </summary>
        //~RQRSContract()
        //{
        //    Dispose(false);
        //}
        /// <summary>
        /// The dispose method.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        /// <summary>
        /// The overrided dispose method
        /// </summary>
        /// <param name="disposing">True if this is called by dispose, false if this
        /// is called by the finalizer.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposed && disposing)
            {
                mObjectPool = null; 
                mRequest = null;
                mResponse = null;
                mSenderReference = null;
                mJobSessionReferenceID = null;
                mJobSecurityIdentifier = null;
            }
            disposed = true;
        }
        #endregion
    }
}
