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
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.Serialization;

using Ximura;
using Ximura.Helper;
#endregion // using
namespace Ximura
{
    /// <summary>
    /// This the base class for the returnable pool object. These objects retain a 
    /// reference to the pool manager that created them, allowing them to be returned to 
    /// the pool without a need for the application to keep an external reference of the pool
    /// manager.
    /// </summary>
    public class PoolableReturnableObjectBase : PoolableObjectBase, IXimuraPoolReturnable
    {
        #region Declarations
        private object syncPoolCollection = new object();
        private IXimuraPool mPool;
        private IXimuraPoolManager mPoolManager;
        private Dictionary<Type, IXimuraPool> mFragmentPoolCache = null;
        #endregion // Declarations
        #region Constructor
        /// <summary>
        /// This is the default constructor. 
        /// </summary>
        public PoolableReturnableObjectBase()
            : base()
        {
        }
        #endregion // Constructor

        #region Reset()
        /// <summary>
        /// This is the reset override which removes the reference to the object pool.
        /// </summary>
        public override void Reset()
        {
            if (mFragmentPoolCache != null)
                mFragmentPoolCache.Clear();
            mPoolManager = null;
            mPool = null;
            base.Reset();
        }
        #endregion // Reset()

        #region IXimuraPoolReturnable Members
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
        #endregion

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
                lock (syncPoolCollection)
                {
                    mPoolManager = value;
                    if (mFragmentPoolCache == null)
                        mFragmentPoolCache = new Dictionary<Type, IXimuraPool>();
                }
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
            lock (syncPoolCollection)
            {
                if (!mFragmentPoolCache.ContainsKey(objectType))
                {
                    IXimuraPool newPool = mPoolManager.GetPoolManager(objectType);
                    mFragmentPoolCache.Add(objectType, newPool);
                }
                return mFragmentPoolCache[objectType];
            }
        }
        #endregion
        #region IXimuraPool<T> PoolGet<T>()
        /// <summary>
        /// This method returns the specific pool manager for the type.
        /// </summary>
        /// <param name="objectType"></param>
        /// <returns>Returns the pool for the object type.</returns>
        public IXimuraPool<T> PoolGet<T>() where T : IXimuraPoolableObject
        {
            lock (syncPoolCollection)
            {
                if (!mFragmentPoolCache.ContainsKey(typeof(T)))
                {
                    IXimuraPool<T> newPool = mPoolManager.GetPoolManager<T>();
                    mFragmentPoolCache.Add(typeof(T), newPool);
                }
                return (IXimuraPool<T>)mFragmentPoolCache[typeof(T)];
            }
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
        #region T PoolGetObject<T>()
        /// <summary>
        /// Returns a new object of the type specified.
        /// </summary>
        /// <param name="objectType">The object type required.</param>
        /// <returns>Returns a poolable object of the type defined.</returns>
        public T PoolGetObject<T>() where T : IXimuraPoolableObject
        {
            return PoolGet<T>().Get();
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
        #region PoolGetReturn<T>(T poolObject)
        /// <summary>
        /// This method returns the object to the appropriate pool.
        /// </summary>
        /// <param name="poolObject">The object to return.</param>
        public void PoolGetReturn<T>(T poolObject) where T : IXimuraPoolableObject
        {
            PoolGet<T>().Return(poolObject);
        }
        #endregion // PoolGetReturn(object poolObject)
    }
}
