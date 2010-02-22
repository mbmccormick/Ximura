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
using System.IO;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Diagnostics;

using System.Text;

using Ximura;
using Ximura.Data;
using Ximura.Data.Serialization;
using Ximura.Helper;
using CH = Ximura.Helper.Common;
#endregion // using
namespace Ximura.Data
{
    public abstract partial class Content : IXimuraPoolManagerDirectAccess
    {
        #region Declarations
        /// <summary>
        /// This is the unique track id for the content used for pooling.
        /// </summary>
        private readonly Guid mTrackID = Guid.NewGuid();

        private IXimuraPool mPool;
        private IXimuraPoolManager mPoolManager;
        #endregion

        #region Reset()
        /// <summary>
        /// This is the reset method to set the content to it's default state.
        /// </summary>
        public virtual void Reset()
        {
            mPoolManager = null;
            mPool = null;

            mCanLoad = true;
            mEntityType = "";
            mEntitySubType = "";
            mInfo = null;

            mIDVersion = Guid.Empty;
            mIDContent = Guid.Empty;
            
            mDirty = false;

            mOnInitialized = null;
            mInitialized = false;
            mInitializing = false;
        }
        #endregion

        #region Dispose(bool disposing)
        private bool mDisposed = false;
        /// <summary>
        /// This is the dispose override.
        /// </summary>
        /// <param name="disposing">True when this is called by the dispose method.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!mDisposed)
            {
                if (disposing)
                {
                    mOnInitialized = null;
                    mInfo = null;
                    mEntityType = null;
                    mEntitySubType = null;
                    mPoolManager = null;
                    mPool = null;

                    //base.Dispose(disposing);
                }
                mDisposed = true;
            }
        }
        #endregion // Dispose(bool disposing)

        #region CanPool
        /// <summary>
        /// This method indicates whether the object can be pooled. By default this is set to true. 
        /// You should override this method to return false if you do not wish your content object to be poolable.
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
        /// This is the tracking ID for the object.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Guid TrackID
        {
            get { return mTrackID; }
        }
        #endregion // TrackID

        #region CanResetWithDeserialization
        /// <summary>
        /// This property indicates whether the object support a deserialization reset.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual bool CanResetWithDeserialization { get { return true; } }
        #endregion // CanResetWithDeserialization
        #region Reset(SerializationInfo info, StreamingContext context)
        /// <summary>
        /// This is the reset method to set the content to the state specified in the
        /// deserialization paramters.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The serialization context.</param>
        public virtual void Reset(SerializationInfo info, StreamingContext context)
        {
            Reset();
            DeserializeIncoming(info, context);
        }
        #endregion // Reset(SerializationInfo info, StreamingContext context)

        #region ObjectPool
        /// <summary>
        /// This is the object pool that the message belongs to.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IXimuraPool ObjectPool
        {
            get
            {
                return mPool;
            }
            set
            {
                mPool = value;
            }
        }
        #endregion // ObjectPool
        #region ObjectPoolCanReturn
        /// <summary>
        /// This property specifices whether the object can be returns to the pool.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool ObjectPoolCanReturn
        {
            get { return mPool != null; }
        }
        #endregion // ObjectPoolCanReturn
        #region ObjectPoolReturn()
        /// <summary>
        /// This method returns the object to the pool.
        /// </summary>
        public virtual void ObjectPoolReturn()
        {
            mPool.Return(this);
        }
        #endregion // ObjectPoolReturn()

        #region PoolManager
        /// <summary>
        /// This is the pool manager that can be used by the IXimuraPoolManagerDirectAccess interface.
        /// Although the methods and properties are implemented in the base class the actual interface is
        /// not implemented.
        /// </summary>
        public IXimuraPoolManager PoolManager
        {
            get
            {
                return mPoolManager;
            }
            set
            {
                mPoolManager = value;
            }
        }
        #endregion
        #region PoolGet(Type objectType)
        /// <summary>
        /// This method returns the specific pool manager for the type.
        /// </summary>
        /// <param name="objectType"></param>
        /// <returns>Returns the pool for the object type.</returns>
        public IXimuraPool PoolGet(Type objectType)
        {
            return mPoolManager.GetPoolManager(objectType);
        }
        #endregion
        #region PoolGetObject(Type objectType)
        /// <summary>
        /// Returns a new object of the type specified.
        /// </summary>
        /// <param name="objectType">The object type required.</param>
        /// <returns>Returns a poolable object of the type defined.</returns>
        public object PoolGetObject(Type objectType)
        {
            return PoolGet(objectType).Get();
        }
        #endregion // PoolGetObject(Type objectType)
        #region PoolGetReturn(object poolObject)
        /// <summary>
        /// This method returns the object to the appropriate pool.
        /// </summary>
        /// <param name="poolObject">The object to return.</param>
        public void PoolGetReturn(object poolObject)
        {
            PoolGet(poolObject.GetType()).Return(poolObject);
        }
        #endregion // PoolGetReturn(object poolObject)
    }
}
