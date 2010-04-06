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
using System.Runtime.Serialization;

using Ximura;

#endregion // using
namespace Ximura
{
    /// <summary>
    /// The pool buffer allow multiple clients to connect to the same underlying pool, but removes direct
    /// access to the underlying pool allowing more stable support.
    /// </summary>
    public sealed class PoolBuffer<T> : IXimuraPool<T>, IXimuraPoolBuffer
        where T : IXimuraPoolableObject
    {
        #region Declarations
        private IXimuraPool<T> internalPool = null;
        private bool mDisposed = false;
        #endregion
        #region Constructors
        /// <summary>
        /// This is the internal constructor. The pool buffer can only be accessed by the pool manager.
        /// </summary>
        /// <param name="pool">The internal pool.</param>
        public PoolBuffer(IXimuraPool pool)
        {
            internalPool = (IXimuraPool<T>)pool;
        }
        #endregion
        #region IDisposable Members
        /// <summary>
        /// This method disposes the pool buffer class.
        /// </summary>
        public void Dispose()
        {
            mDisposed = true;
            internalPool = null;
        }
        #endregion

        #region DisposedCheck()
        /// <summary>
        /// This method checks whether the pool buffer has been disposed.
        /// </summary>
        private void DisposedCheck()
        {
            if (mDisposed)
                throw new ObjectDisposedException("PoolBuffer", "PoolBuffer is disposed.");
        }
        #endregion // DisposedCheck()
        #region DisconnectCheck()
        /// <summary>
        /// This method checks whether the buffer has been disconnected from the pool.
        /// </summary>
        private void DisconnectCheck()
        {
            DisposedCheck();
            if (internalPool == null)
                throw new InvalidOperationException("This pool buffer has been disconnected.");
        }
        #endregion // DisconnectCheck()
        #region Disconnect()
        /// <summary>
        /// This method disconnects the buffer from the pool.
        /// </summary>
        private void Disconnect()
        {
            DisposedCheck();
            internalPool = null;
        }
        #endregion // Disconnect()

        #region IXimuraPool<T> Members

        T IXimuraPool<T>.Get()
        {
            DisconnectCheck();
            return internalPool.Get();
        }

        T IXimuraPool<T>.Get(SerializationInfo info, StreamingContext context)
        {
            DisconnectCheck();
            return internalPool.Get(info, context);
        }

        void IXimuraPool<T>.Return(T value)
        {
            DisconnectCheck();
            internalPool.Return(value);
        }

        string IXimuraPool.Stats
        {
            get
            {
                DisconnectCheck();
                return internalPool.Stats;
            }
        }

        #endregion

        #region IXimuraPool Members

        object IXimuraPool.Get()
        {
            DisconnectCheck();
            return internalPool.Get();
        }

        object IXimuraPool.Get(SerializationInfo info, StreamingContext context)
        {
            DisconnectCheck();
            return internalPool.Get(info, context);
        }

        void IXimuraPool.Return(object value)
        {
            DisconnectCheck();
            internalPool.Return(value);
        }

        bool IXimuraPool.Available
        {
            get 
            {
                DisconnectCheck();
                return internalPool.Available; 
            }
        }

        int IXimuraPool.Max
        {
            get
            {
                DisconnectCheck();
                return internalPool.Max;
            }
        }

        int IXimuraPool.Min
        {
            get
            {
                DisconnectCheck();
                return internalPool.Min;
            }
        }

        int IXimuraPool.Prefered
        {
            get
            {
                DisconnectCheck();
                return internalPool.Prefered;
            }
        }

        int IXimuraPool.Count
        {
            get
            {
                DisconnectCheck();
                return internalPool.Count;
            }
        }

        //void IXimuraPool.Clear()
        //{
        //    DisconnectCheck();
        //    throw new NotSupportedException("Clear is not supported for a buffered pool.");
        //}

        bool IXimuraPool.IsBuffered
        {
            get
            {
                DisconnectCheck();
                return true;
            }
        }
        #endregion

        #region IXimuraPoolBuffer.ResetBuffer()
        void IXimuraPoolBuffer.ResetBuffer()
        {
            Disconnect();
        }
        #endregion

        #region IXimuraPool.PoolManager

        IXimuraPoolManager IXimuraPool.PoolManager
        {
            get
            {
                throw new NotSupportedException("PoolManager is not supported in the pool buffer class.");
            }
            set
            {
                throw new NotSupportedException("PoolManager is not supported in the pool buffer class.");
            }
        }

        #endregion
    }
}
