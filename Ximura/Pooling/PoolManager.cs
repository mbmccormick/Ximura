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
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

using Ximura;
using Ximura.Framework;
using Ximura.Helper;
using RH = Ximura.Helper.Reflection;
#endregion // using
namespace Ximura
{
    /// <summary>
    /// The pool manager is a static class which controls system wide object pooling within the 
    /// current AppDomain.
    /// </summary>
    public class PoolManager : IXimuraPoolManager
    {
        #region Declarations
        private object syncLock = new object();

        private bool mUseBuffer;
        private bool mDisposed;
        /// <summary>
        /// This collection contains the specific pool managers that hold a collection of poolable object.
        /// </summary>
        private Dictionary<Type, PoolManagerContainer> mPoolManagers = null;

        #endregion
        #region Internal Class --> PoolManagerContainer
        /// <summary>
        /// The PoolManagerContainer is a private class that is used to hold the pool and the buffered pool.
        /// </summary>
        private class PoolManagerContainer : IDisposable
        {
            #region Declarations
            bool mDisposed;
            Type mPoolType;
            IXimuraPool mPool;
            IXimuraPool mPoolBuffered;
            #endregion // Declarations
            #region Constructor
            /// <summary>
            /// This is the default constructor for the the PoolManagerContainer.
            /// </summary>
            /// <param name="poolManager">The pool manager to contain.</param>
            public PoolManagerContainer(Type objectType, IXimuraPoolManager manager)
            {
                mDisposed = false;
                mPoolType = objectType;
                mPool = CreatePool(objectType, manager);
                mPoolBuffered = CreateBufferedPool(objectType, mPool);
            }
            #endregion // Constructor

            #region Pool
            /// <summary>
            /// The pool.
            /// </summary>
            public IXimuraPool Pool
            {
                get
                {
                    CheckDisposed();
                    return mPool;
                }
            }
            #endregion // Pool
            #region PoolBuffered
            /// <summary>
            /// The buffered pool.
            /// </summary>
            public IXimuraPool PoolBuffered
            {
                get
                {
                    CheckDisposed();
                    return mPoolBuffered;
                }
            }
            #endregion // PoolBuffered

            #region CreatePool(Type objectType)
            /// <summary>
            /// This method uses generics to create the specific type of context pool.
            /// </summary>
            /// <param name="objectType">The context type.</param>
            /// <returns>Returns the pool.</returns>
            private IXimuraPool CreatePool(Type objectType, IXimuraPoolManager manager)
            {
                Type poolType = typeof(Pool<>).MakeGenericType(new Type[] { objectType });

                IXimuraPool pool = Activator.CreateInstance(poolType) as IXimuraPool;
                pool.PoolManager = manager;
                return pool;
            }
            #endregion // InternalCreatePool(Type objectType)
            #region CreateBufferedPool(Type objectType, IXimuraPool pool)
            private IXimuraPool CreateBufferedPool(Type objectType, IXimuraPool pool)
            {
                Type bufferType =
                    typeof(PoolBuffer<>).MakeGenericType(new Type[] { objectType });

                IXimuraPool buffer = Activator.CreateInstance(bufferType,
                    new object[] { pool }) as IXimuraPool;

                return buffer;
            }
            #endregion // CreateBufferedPool(Type objectType, IXimuraPool pool)

            #region IDisposable Members
            #region CheckDisposed()
            /// <summary>
            /// This method checks whether the pool manager has been disposed, and if so throws an exception.
            /// </summary>
            private void CheckDisposed()
            {
                if (mDisposed)
                    throw new ObjectDisposedException("PoolManagerContainer", "PoolManagerContainer (" + mPoolType.FullName + ")has been disposed.");
            }
            #endregion // CheckDisposed()

            /// <summary>
            /// This is the dispose method for the class.
            /// </summary>
            public void Dispose()
            {
                if (mDisposed)
                    return;

                mDisposed = true;
                mPoolBuffered.Dispose();
                mPool.Dispose();
                mPool = null;
                mPoolBuffered = null;
            }

            #endregion
        }
        #endregion // Class --> PoolManagerContainer

        #region Constructors
        /// <summary>
        /// This is the default static constructor.
        /// </summary>
        public PoolManager()
            : this(false)
        {
            mDisposed = false;
        }
        /// <summary>
        /// This is the buffered output option for the pool manager. With a buffered output the pool object is abstracted
        /// using a buffer object which does not allow the calling party to clear the pool.
        /// </summary>
        /// <param name="useBuffer">Set this to true if you wish the pool manager to output a buffered output.</param>
        public PoolManager(bool useBuffer)
        {
            mUseBuffer = useBuffer;
            mPoolManagers = new Dictionary<Type, PoolManagerContainer>();
        }
        #endregion

        #region IXimuraPoolManager
        IXimuraPool IXimuraPoolManager.GetPoolManager(Type objectType)
        {
            return ((IXimuraPoolManager)this).GetPoolManager(objectType, false);
        }

        IXimuraPool IXimuraPoolManager.GetPoolManager(Type objectType, bool buffered)
        {
            CheckDisposed();

            if (!mPoolManagers.ContainsKey(objectType))
                lock (syncLock)
                {
                    if (!RH.ValidateInterface(objectType, typeof(IXimuraPoolableObject)))
                        throw new ArgumentException("objectType must implement IXimuraPoolableObject.");

                    if (!mPoolManagers.ContainsKey(objectType))
                        mPoolManagers.Add(objectType, new PoolManagerContainer(objectType, this));
                }

            //Returns either the object pool or a buffered pool depending on the buffered parameter.
            return buffered || mUseBuffer ?
                mPoolManagers[objectType].PoolBuffered : mPoolManagers[objectType].Pool;
        }

        IXimuraPool<T> IXimuraPoolManager.GetPoolManager<T>()
        {
            return ((IXimuraPoolManager)this).GetPoolManager<T>(false);
        }

        IXimuraPool<T> IXimuraPoolManager.GetPoolManager<T>(bool buffered)
        {
            CheckDisposed();

            if (!mPoolManagers.ContainsKey(typeof(T)))
                lock (syncLock)
                {
                    if (!mPoolManagers.ContainsKey(typeof(T)))
                        mPoolManagers.Add(typeof(T), new PoolManagerContainer(typeof(T), this));
                }

            //Returns either the object pool or a buffered pool depending on the buffered parameter.
            return buffered || mUseBuffer ?
                (IXimuraPool<T>)(mPoolManagers[typeof(T)].PoolBuffered)
                : (IXimuraPool<T>)(mPoolManagers[typeof(T)].Pool);
        }
        #endregion // IXimuraPoolManager

        #region CheckDisposed()
        /// <summary>
        /// This method checks whether the pool manager has been disposed, and if so throws an exception.
        /// </summary>
        private void CheckDisposed()
        {
            if (mDisposed)
                throw new ObjectDisposedException("PoolManager", "PoolManager has been disposed.");
        }
        #endregion // CheckDisposed()

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposing && !mDisposed)
            {
                mDisposed = true;
                //DisposeBuffers();
                DisposePools();
            }
        }

        private void DisposePools()
        {
            if (mPoolManagers == null)
                return;

            foreach (PoolManagerContainer container in mPoolManagers.Values)
            {
                container.Dispose();
            }
            mPoolManagers.Clear();
            mPoolManagers = null;
        }

        #endregion
    }
}
