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
using CH = Ximura.Common;
using Ximura.Framework;

#endregion // using
namespace Ximura.Framework
{
    public partial class Context<ST, SET, CONF, PERF>
    {
        #region ContextPoolAccess
        /// <summary>
        /// This method allows access the context pool
        /// </summary>
        public IXimuraFSMContextPoolAccess ContextPoolAccess
        {
            get { return mContextPoolAccess; }
        }
        #endregion // ContextPoolAccess

        #region GetObjectPool
        /// <summary>
        /// This method retrieves the object pool for the particular object type.
        /// </summary>
        /// <param name="objectType">The object type.</param>
        /// <returns>Returns the object pool.</returns>
        public IXimuraPool GetObjectPool(Type objectType)
        {
            return ContextSettings.PoolManager.GetPoolManager(objectType);
        }
        /// <summary>
        /// This generic method retrieves a pool manager for the type specified.
        /// </summary>
        /// <typeparam name="T">The type specified.</typeparam>
        /// <returns>Returns the pool manager.</returns>
        public IXimuraPool<T> GetObjectPool<T>() where T : IXimuraPoolableObject
        {
            return ContextSettings.PoolManager.GetPoolManager<T>();
        }
        #endregion // GetObjectPool(Type objectType)
        #region GetObject
        /// <summary>
        /// This generic method retrieves an object from the pool manager for the type specified.
        /// </summary>
        /// <typeparam name="T">The type specified.</typeparam>
        /// <returns>Returns the pool manager object.</returns>
        public T GetObject<T>() where T : IXimuraPoolableObject
        {
            return ContextSettings.PoolManager.GetPoolManager<T>().Get();
        }
        /// <summary>
        /// This generic method retrieves an object from the pool and deserializes the data in the info object in to the 
        /// new pool object.
        /// </summary>
        /// <typeparam name="T">The type specified.</typeparam>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The serialization context.</param>
        /// <returns>An object of the type defined in the pool definition, with the serialization data.</returns>
        public T GetObject<T>(SerializationInfo info, StreamingContext context) where T : IXimuraPoolableObject
        {
            return ContextSettings.PoolManager.GetPoolManager<T>().Get(info, context);
        }
        #endregion // GetObject

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
        /// This is the object pool tracking ID.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Guid TrackID { get { return mPoolTrackingID; } }
        #endregion // TrackID

        #region ObjectPool
        /// <summary>
        /// This property sets a reference to the object pool on the object
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IXimuraPool ObjectPool
        {
            get
            {
                return mContextPool;
            }
            set
            {
                mContextPool = value;
            }
        }
        #endregion // ObjectPool
        #region ObjectPoolCanReturn
        /// <summary>
        /// This boolean property determines whether the object can return to the pool.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool ObjectPoolCanReturn
        {
            get { return mContextPool != null; }
        }
        #endregion // ObjectPoolCanReturn
        #region ObjectPoolReturn()
        /// <summary>
        /// This method returns the object ot the pool.
        /// </summary>
        public void ObjectPoolReturn()
        {
            mContextPool.Return(this);
        }
        #endregion // ObjectPoolReturn()

        #region EnvelopeHelper
        /// <summary>
        /// This is the envelope helper for the context.
        /// </summary>
        public IXimuraEnvelopeHelper EnvelopeHelper
        {
            get
            {
                return ContextSettings.EnvelopeHelper;
            }
        }
        #endregion // EnvelopeHelper
    }
}
