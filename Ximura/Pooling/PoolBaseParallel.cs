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
using System.Linq;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.Serialization;
using System.Threading;

using Ximura;
using Ximura.Helper;
#endregion // using
namespace Ximura
{
    /// <summary>
    /// The PoolBaseParallel class is used to hold a collection of object that can be re-used.
    /// This class is optimized for multi-threaded parallel execution.
    /// </summary>
    /// <typeparam name="T">The object pool type.</typeparam>
    public abstract partial class PoolBaseParallel<T> : Component, IXimuraPoolInitialize<T>
        where T : class, IXimuraPoolableObject
    {
        #region Declarations
        #region Class -> PoolItemTracker<S>
        protected class PoolItemTracker<S>
            where S : class, IXimuraPoolableObject
        {
            public PoolItemTracker(S item)
            {
                TrackingID = item.TrackID;
                WR = new WeakReference(item);
                Item = item;
            }

            public Guid TrackingID { get; private set; }
            public WeakReference WR { get; private set; }

            public S Item { get; set; }
        }
        #endregion // Class -> PoolItemTracker<S>
        #region Enum -> PoolActionState
        private enum PoolActionState
        {
            OK,
            Miss,
            Error,
            Orphan
        }
        #endregion // PoolActionState

        /// <summary>
        /// This boolean property identifies whether the pool has been disposed.
        /// </summary>
        protected bool Disposed { get; set; }
        /// <summary>
        /// This boolean property identifies whether the pool has been initialized.
        /// </summary>
        protected bool Initialized { get; set; }

        private int mCount = 0;
        private int mAvailable = 0;

        private long mCounterChurn = 0;
        private long mCounterError = 0;
        private long mCounterMiss = 0;

        private HashSet<PoolItemTracker<T>> mHashSet;
        #endregion // Declarations
        #region Constructors
        /// <summary>
        /// The default constructor.
        /// </summary>
        public PoolBaseParallel()
        {
            Disposed = false;
            Initialized = false;
            GetUnavailableThrowException = typeof(T).IsValueType;
        }
        /// <summary>
        /// This constructor sets the pool create parameters.
        /// </summary>
        /// <param name="min">The initial set of objects.</param>
        /// <param name="max">The maximum set of object for the pool. Set this to -1 is you do not 
        /// wish to set a maximum value.</param>
        /// <param name="prefer">The prefered number of object for the pool.</param>
        public PoolBaseParallel(int min, int max, int prefer): this(min, max, prefer, Environment.ProcessorCount-1){}
        /// <summary>
        /// This constructor sets the pool create parameters.
        /// </summary>
        /// <param name="min">The initial set of objects.</param>
        /// <param name="max">The maximum set of object for the pool. Set this to -1 is you do not 
        /// wish to set a maximum value.</param>
        /// <param name="prefer">The prefered number of object for the pool.</param>
        public PoolBaseParallel(int min, int max, int prefer, int overbite)
            : this()
        {
            Max = max;
            Min = min;
            Prefered = prefer;
            Overbite = overbite<0?0:overbite;

            if (min > 0 || prefer > 0)
                InitializePool();
        }
        #endregion // Constructors
        #region Dispose(bool disposing)
        /// <summary>
        /// This override disposes of the pool.
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Disposed = true;
            }
            base.Dispose(disposing);
        }
        #endregion // Dispose(bool disposing)
        #region CheckStatus()
        /// <summary>
        /// This method checks the status of the pool.
        /// </summary>
        /// <exception cref="System.ObjectDisposedException">The pool has been disposed.</exception>
        /// <exception cref="SYstem.NotSupportedException">The pool has not been initialized</exception>
        protected virtual void CheckStatus()
        {
            if (Disposed)
            {
                throw new ObjectDisposedException(GetType().Name, "The object pool has been disposed.");
            }

            if (!Initialized)
            {
                throw new NotSupportedException("The pool is not initialized");
            }
        }
        #endregion // CheckStatus()

        #region InitializePool
        /// <summary>
        /// This method initiates the object pool with the required number of objects.
        /// </summary>
        protected virtual void InitializePool()
        {
            int total = Min;

            if (Prefered > total && (Max == -1 || Prefered <= Max))
                total = Prefered;

            for (int i = 0; i < total; i++)
            {
                T obj = CreateNewPoolObject();
                ItemAdd(obj);
            }

            mCount = 0;

            Initialized = true;
        }
        #endregion // InitializePool()

        #region Get
        #region Get()
        /// <summary>
        /// The Get() method takes an object from the pool, if 
        /// there are no objects available the pool will create a new object.
        /// </summary>
        /// <returns>An object of the type defined in the pool.</returns>
        /// <exception cref="System.ObjectDisposedException">
        /// An object disposed exception will be thrown if the pool has been disposed.
        /// </exception>
        public T Get()
        {
            return Get(null);
        }
        #endregion // Get()
        #region IXimuraPool.Get()
        /// <summary>
        /// This is the default get() accessor.
        /// </summary>
        /// <returns>An object of the type defined in the pool.</returns>
        object IXimuraPool.Get()
        {
            return Get(null) as object;
        }
        #endregion // IXimuraPool.Get()
        #region Get(SerializationInfo info, StreamingContext context)
        /// <summary>
        /// The Get() method takes an object from the pool, if 
        /// there are no objects available the pool will create a new object.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The context.</param>
        /// <returns>An object of the type defined in the pool.</returns>
        /// <exception cref="System.ObjectDisposedException">
        /// An object disposed exception will be thrown if the pool has been disposed.
        /// </exception>
        public T Get(SerializationInfo info, StreamingContext context)
        {
            return Get(info, context, null);
        }
        #endregion // Get(SerializationInfo info, StreamingContext context)
        #region IXimuraPool.Get(SerializationInfo info, StreamingContext context)
        /// <summary>
        /// The Get() method takes an object from the pool, if 
        /// there are no objects available the pool will create a new object.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The context.</param>
        /// <returns>An object of the type defined in the pool.</returns>
        /// <exception cref="System.ObjectDisposedException">
        /// An object disposed exception will be thrown if the pool has been disposed.
        /// </exception>
        object IXimuraPool.Get(SerializationInfo info, StreamingContext context)
        {
            return Get(info, context, null) as object;
        }
        #endregion // IXimuraPool.Get(SerializationInfo info, StreamingContext context)
        #region Get(Action<T> initializer)
        /// <summary>
        /// The Get() method takes an object from the pool, if 
        /// there are no objects available the pool will create a new object.
        /// </summary>
        /// <param name="initializer">This is the object initialization action. This value can be null.</param>
        /// <returns>An object of the type defined in the pool.</returns>
        /// <exception cref="System.ObjectDisposedException">
        /// An object disposed exception will be thrown if the pool has been disposed.
        /// </exception>
        public virtual T Get(Action<T> initializer)
        {
            CheckStatus();

            return GetInternal(initializer, null);
        }
        #endregion // Get(Action<T> initializer)
        #region Get(SerializationInfo info, StreamingContext context, Action<T> initializer)
        /// <summary>
        /// The Get() method takes an object from the pool, if 
        /// there are no objects available the pool will create a new object.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The context.</param>
        /// <param name="initializer">This is the object initialization action. This value can be null.</param>
        /// <returns>An object of the type defined in the pool.</returns>
        /// <exception cref="System.ObjectDisposedException">
        /// An object disposed exception will be thrown if the pool has been disposed.
        /// </exception>
        public virtual T Get(SerializationInfo info, StreamingContext context, Action<T> initializer)
        {
            CheckStatus();

            return GetInternal(initializer, (o) => { ResetPoolObject(o, info, context); });
        }
        #endregion // Get(SerializationInfo info, StreamingContext context, Action<T> initializer)

        #region GetUnavailableThrowException
        /// <summary>
        /// This property specifies whether the pool should throw an exception if there are no object available. 
        /// This is useful when the object is a value type and does not return null.
        /// The default is false when T is a class type and true when T is a value type. This value is set in the constructor.
        /// </summary>
        protected virtual bool GetUnavailableThrowException
        {
            get;
            set;
        }
        #endregion // GetUnavailableThrowException
        #region GetInternal(Action<T> initializer, Action<T> deserializer)
        /// <summary>
        /// This method gets an item from the pool internally.
        /// </summary>
        /// <param name="initializer">The initialization delegate. Leave this null if not required.</param>
        /// <param name="deserializer">The deserialization delegate. Leave this null if not required.</param>
        /// <returns>Returns an object from the pool, or default(T) is there are no objects available.</returns>
        /// <exception cref="Ximura.PoolObjectUnavailableException">This exception is thrown when there are no pool object available and 
        /// GetUnavailableThrowException is set to true.</exception>
        protected virtual T GetInternal(Action<T> initializer, Action<T> deserializer)
        {
            T obj = default(T);
            PoolActionState success = PoolActionState.OK;

            try
            {
                obj = ItemGet();

                //There are no object available in the pool.
                if (obj == null)
                {
                    success = PoolActionState.Miss;

                    if (GetUnavailableThrowException)
                        throw new PoolObjectUnavailableException();
                    else
                        return default(T);
                }

                if (deserializer != null)
                    deserializer(obj);

                if (obj is IXimuraPoolReturnable)
                    ((IXimuraPoolReturnable)obj).ObjectPool = this;

                if (PoolManager != null && obj is IXimuraPoolManagerDirectAccess)
                    ((IXimuraPoolManagerDirectAccess)obj).PoolManager = PoolManager;

                if (initializer != null)
                    initializer(obj);

                return obj;
            }
            catch (Exception ex)
            {
                if (success != PoolActionState.Miss)
                    success = PoolActionState.Error;

                if (obj != null)
                    Return(obj);

                throw ex;
            }
            finally
            {
                switch (success)
                {
                    case PoolActionState.Error: Interlocked.Increment(ref mCounterError); break;
                    case PoolActionState.Miss : Interlocked.Increment(ref mCounterMiss) ; break;
                    case PoolActionState.OK   : Interlocked.Increment(ref mCounterChurn); break;
                }
            }
        }
        #endregion // GetInternal(Action<T> initializer, Action<T> deserializer)
        #endregion

        #region Return
        #region Return(T value)
        /// <summary>
        /// This method returns an object to the pool.
        /// </summary>
        /// <param name="value">The object to return to the pool.</param>
        public virtual void Return(T value)
        {
            CheckStatus();

            ItemReturn(value, false);
        }
        #endregion // Return(T value)
        #region Return(object value)
        /// <summary>
        /// This method returns an object to the pool as an object and not being of a specific type.
        /// </summary>
        /// <param name="value">The object to return to the pool.</param>
        public void Return(object value)
        {
            CheckStatus();

            if (!(value is T))
                throw new ArgumentException("The object is not the correct type for the pool.");

            ItemReturn((T)value, false);
        }
        #endregion // Return(object value)
        #endregion // Return

        #region Available
        /// <summary>
        /// This method returns the internal available property.
        /// </summary>
        /// <returns>Returns true if the pool can return objects.</returns>
        public virtual bool Available
        {
            get { return mAvailable>0; }
        }
        #endregion // Available
        #region Count
        /// <summary>
        /// This property returns the number of active objects.
        /// </summary>
        public virtual int Count
        {
            get { return mCount; }
        }
        #endregion // Count

        #region Statistics
        #region Churn
        /// <summary>
        /// This property returns the number of Gets performed by the pool.
        /// </summary>
        public virtual long Churn
        {
            get { return mCounterChurn; }
        }
        #endregion // Churn
        #region Stats
        /// <summary>
        /// This property returns a summary of the stats for the collection.
        /// </summary>
        public virtual string Stats
        {
            get 
            {
                int alive = -1;

                return string.Format("Min={0}|Max={1}|Pre={2}|Count={3}|Available={4}|Alive={5}-{6}", Min, Max, Prefered, Count, mAvailable, alive, typeof(T).Name); 
            }
        }
        #endregion // Stats
        #endregion // Statistics
        #region Settings
        #region Max
        /// <summary>
        /// This is the maximum pool size, a value of -1 specifies no maximum size.
        /// </summary>
        public int Max
        {
            get;
            protected set;
        }
        #endregion // Max
        #region Min
        /// <summary>
        /// This is the minimum pool size.
        /// </summary>
        public int Min
        {
            get;
            protected set;
        }
        #endregion // Min
        #region Prefered
        /// <summary>
        /// This is prefered pool size.
        /// </summary>
        public int Prefered
        {
            get;
            protected set;
        }
        #endregion // Prefered
        #region Overbite
        /// <summary>
        /// This is allowable increase over the maximum value. This may be useful in multi-processor machines.
        /// </summary>
        public int Overbite
        {
            get;
            protected set;
        }
        #endregion // Prefered
        #endregion // Settings

        #region IsBuffered
        /// <summary>
        /// This property indicates whether the pool is buffered. Buffered pools are shared amongst multiple 
        /// clients and do not implement the clear method.
        /// </summary>
        public bool IsBuffered
        {
            get { return false; }
        }
        #endregion
        #region PoolManager
        /// <summary>
        /// If this property is set, it will be passed on to objects that implement the IXimuraPoolManagerDirectAccess
        /// interface to allow them to get additional pool objects of different types for their own internal use.
        /// </summary>
        public IXimuraPoolManager PoolManager
        {
            get;
            set;
        }
        #endregion

        #region CreateNewPoolObject
        /// <summary>
        /// This method creates a new pool object of type T.
        /// </summary>
        /// <returns>Returns the new object.</returns>
        protected abstract T CreateNewPoolObject();
        #endregion // CreateNewPoolObject

        #region ResetPoolObject
        /// <summary>
        /// This method is used to reset the pool object. You should override this method if
        /// you wish to set particular values.
        /// </summary>
        /// <param name="obj">The object to reset.</param>
        protected virtual void ResetPoolObject(T obj)
        {
            obj.Reset();

            if (obj is IXimuraPoolReturnable)
                //Clear the object pool reference.
                ((IXimuraPoolReturnable)obj).ObjectPool = null;

            if (obj is IXimuraPoolManagerDirectAccess)
                ((IXimuraPoolManagerDirectAccess)obj).PoolManager = null; 
        }
        #endregion // ResetPoolObject
        #region ResetPoolObject(T obj, SerializationInfo info, StreamingContext context)
        /// <summary>
        /// This method is used to reset the pool object from the serialization information.
        /// You should override this method if you wish to set particular values.
        /// </summary>
        /// <param name="obj">The object to reset.</param>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The context.</param>
        protected virtual void ResetPoolObject(T obj, SerializationInfo info, StreamingContext context)
        {
            if (obj == null)
                throw new ArgumentNullException("ResetPoolObject: obj cannot be null.");

            if (info == null)
                throw new ArgumentNullException("ResetPoolObject: serialization info cannot be null.");

            if (obj is IXimuraPoolableObjectDeserializable)
            {
                ((IXimuraPoolableObjectDeserializable)obj).Reset(info, context);
                ((IXimuraPoolableObjectDeserializable)obj).OnDeserialization(null);
                return;
            }

            throw new SerializationException("Object does not implement IXimuraPoolableObjectDeserializable.");
        }
        #endregion // ResetPoolObject(T obj, SerializationInfo info, StreamingContext context)
    }
}
