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
using Ximura.Helper;
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
        private bool mDisposed;
        private object syncPoolBuffer = new object();
        #endregion
        #region Constructors
        /// <summary>
        /// This is the internal constructor. The pool buffer can only be accessed by the pool manager.
        /// </summary>
        /// <param name="pool">The internal pool.</param>
        public PoolBuffer(IXimuraPool pool)
        {
            internalPool = (IXimuraPool<T>)pool;
            mDisposed = false;
        }
        #endregion

        #region IXimuraPool<T> Members

        private void DisposedCheck()
        {
            if (mDisposed)
                throw new ObjectDisposedException("PoolBuffer", "PoolBuffer is disposed.");

            if (Disconnected)
                throw new ObjectDisposedException("PoolBuffer", "This pool buffer has been disconnected.");
        }

        internal bool Disconnected
        {
            get { return internalPool == null; }
        }

        internal void Disconnect()
        {
            lock (syncPoolBuffer)
            {
                internalPool = null;
            }
        }

        T IXimuraPool<T>.Get()
        {
            DisposedCheck();
            return internalPool.Get();
        }

        T IXimuraPool<T>.Get(SerializationInfo info, StreamingContext context)
        {
            DisposedCheck();
            return internalPool.Get(info, context);
        }

        void IXimuraPool<T>.Return(T value)
        {
            DisposedCheck();
            internalPool.Return(value);
        }

        #endregion

        #region IXimuraPool Members

        object IXimuraPool.Get()
        {
            DisposedCheck();
            return internalPool.Get();
        }

        object IXimuraPool.Get(SerializationInfo info, StreamingContext context)
        {
            DisposedCheck();
            return internalPool.Get(info, context);
        }

        void IXimuraPool.Return(object value)
        {
            DisposedCheck();
            internalPool.Return(value);
        }

        bool IXimuraPool.Available
        {
            get { return !Disconnected && internalPool.Available; }
        }

        int IXimuraPool.Max
        {
            get
            {
                DisposedCheck();
                return internalPool.Max;
            }
        }

        int IXimuraPool.Min
        {
            get
            {
                DisposedCheck();
                return internalPool.Min;
            }
        }

        int IXimuraPool.Prefered
        {
            get
            {
                DisposedCheck();
                return internalPool.Prefered;
            }
        }

        int IXimuraPool.Count
        {
            get
            {
                DisposedCheck();
                return internalPool.Count;
            }
        }

        void IXimuraPool.Clear()
        {
            DisposedCheck();
            throw new NotSupportedException("Clear is not supported for a buffered pool.");
        }

        bool IXimuraPool.IsBuffered
        {
            get
            {
                DisposedCheck();
                return true;
            }
        }

        #endregion

        #region IXimuraPoolBuffer Members
        void IXimuraPoolBuffer.ResetBuffer()
        {
            lock (this)
            {
                internalPool = null;
            }
        }
        #endregion

        #region IXimuraPool Members
        /// <summary>
        /// This method is not implemented in the buffer.
        /// </summary>
        public IXimuraPoolManager PoolManager
        {
            get
            {
                throw new NotImplementedException("The method or operation is not implemented in the pool buffer class.");
            }
            set
            {
                throw new NotImplementedException("The method or operation is not implemented in the pool buffer class.");
            }
        }
        #endregion

        #region IDisposable Members
        /// <summary>
        /// This method disposes the pool buffer class.
        /// </summary>
        public void Dispose()
        {
            lock (syncPoolBuffer)
            {
                internalPool = null;
                mDisposed = true;
            }
        }
        #endregion



        #region IXimuraPool<T> Members


        public string Stats
        {
            get
            {
                DisposedCheck();
                return internalPool.Stats;
            }
        }

        #endregion
    }
}
