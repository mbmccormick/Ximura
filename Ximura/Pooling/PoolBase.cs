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

using Ximura;
using Ximura.Helper;
#endregion // using
namespace Ximura
{
    /// <summary>
    /// The PoolBase class is used to hold a collection of objects that can be re-used.
    /// </summary>
    /// <typeparam name="T">The object pool type.</typeparam>
    public abstract class PoolBase<T> : Component, IXimuraPoolInitialize<T>
        where T : class, IXimuraPoolableObject
    {
        #region Declarations
        private bool mDisposed = false;
        private bool mInitialized = true;

        HashSet<T> mHashSet;

        private Dictionary<Guid, WeakReference> mObjects;
        private Dictionary<Guid, T> mObjectsAvailable;
        /// <summary>
        /// This queue contains a list of available object waiting to be assigned.
        /// </summary>
        private Queue<Guid> mAvailable;

        private int mMax = -1;
        private int mMin = 0;
        private int mPrefered = 0;
        protected int mCount = 0;

        private object syncObj = new object();

        #endregion // Declarations
        #region Constructors
        /// <summary>
        /// The default constructor.
        /// </summary>
        public PoolBase()
        {
            mObjects = new Dictionary<Guid, WeakReference>();
            mObjectsAvailable = new Dictionary<Guid, T>();
            mAvailable = new Queue<Guid>();

            StatisticsInitialize();
        }
        /// <summary>
        /// This constructor sets the pool create parameters.
        /// </summary>
        /// <param name="min">The initial set of objects.</param>
        /// <param name="max">The maximum set of object for the pool. Set this to -1 is you do not 
        /// wish to set a maximum value.</param>
        /// <param name="prefer">The prefered number of object for the pool.</param>
        public PoolBase(int min, int max, int prefer)
            : this()
        {
            mMax = max;
            mMin = min;
            mPrefered = prefer;

            if (min > 0 || prefer > 0)
                InitializePool();
        }
        #endregion // Constructors

        #region InitializePool
        /// <summary>
        /// This method initiates the object pool with the required number of objects.
        /// </summary>
        protected virtual void InitializePool()
        {
            lock (syncObj)
            {
                int total = mMin;

                if (mPrefered > total && (mMax == -1 || mPrefered <= mMax))
                    total = mPrefered;

                for (int i = 0; i < total; i++)
                {
                    T obj = CreateNewPoolObject();
                    ReturnInternal(obj, true);
                }

                mCount = 0;

                mInitialized = true;
            }

            StatisticsUpdate();
        }
        #endregion // InitializePool()

        #region IDisposable / Destructor
        /// <summary>
        /// This is the component model Dispose method.
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            lock (syncObj)
            {
                if (!disposing || mDisposed)
                {
                    base.Dispose(disposing);
                    return;
                }

                mDisposed = true;
                ClearInternal();
                base.Dispose(disposing);
            }
        }

        private void CheckStatus()
        {
            if (mDisposed)
            {
                throw new ObjectDisposedException(GetType().ToString(), "The object pool has been disposed.");
            }

            if (!mInitialized)
            {
                throw new NotSupportedException("The pool is not initialized");
            }
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
        }
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
        #endregion // ResetPoolObject

        #region Get
        /// <summary>
        /// This is the default get() accessor.
        /// </summary>
        /// <returns>An object of the type defined in the pool.</returns>
        object IXimuraPool.Get()
        {
            return Get() as object;
        }

        object IXimuraPool.Get(SerializationInfo info, StreamingContext context)
        {
            return Get(info, context) as object;
        }

        /// <summary>
        /// The Get() method takes an object from the pool, if 
        /// there are no objects available the pool will create a new object.
        /// </summary>
        /// <returns>An object of the type defined in the pool.</returns>
        /// <exception cref="System.ObjectDisposedException">
        /// An object disposed exception will be thrown if the pool has been disposed.
        /// </exception>
        public virtual T Get()
        {
            return Get(null);
        }

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

            T obj = default(T);

            try
            {
                lock (syncObj)
                {
                    GetInternal(out obj);
                }

                if (initializer != null)
                    initializer(obj);
            }
            catch (Exception ex)
            {
                //Remember to clean up in case there is an error, so that we don't have any memory leaks.
                if (!object.Equals(obj, default(T)))
                    Return(obj);

                throw ex;
            }
            finally
            {
                StatisticsUpdate();
            }

            return obj;
        }

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

            T obj = default(T);

            try
            {
                lock (syncObj)
                {
                    GetInternal(out obj);

                    ResetPoolObject(obj, info, context);

                    if (obj is IXimuraPoolReturnable)
                        ((IXimuraPoolReturnable)obj).ObjectPool = this;

                    if (PoolManager != null && obj is IXimuraPoolManagerDirectAccess)
                        ((IXimuraPoolManagerDirectAccess)obj).PoolManager = PoolManager;
                }

                if (initializer != null)
                    initializer(obj);
            }
            catch (Exception ex)
            {
                //Remember to clean up in case there is an error, so that we don't have any memory leaks.
                if (!object.Equals(obj, default(T)))
                    Return(obj);

                throw ex;
            }
            finally
            {
                StatisticsUpdate();
            }

            return obj;
        }

        protected virtual void GetInternal(out T obj)
        {
            obj = default(T);

            if (!InternalAvailable())
                return;

            if (mAvailable.Count > 0)
            {
                Guid next = mAvailable.Dequeue();
                obj = mObjectsAvailable[next];
                mObjectsAvailable.Remove(next);
                ResetPoolObject(obj);
            }
            else
            {
                obj = CreateNewPoolObject();
                mObjects.Add(obj.TrackID, new WeakReference(obj));
            }
            //Note that we do not reset a newly created object. It is up to the object
            //itself to ensure that it is in its default state when created.

            if (obj is IXimuraPoolReturnable)
                ((IXimuraPoolReturnable)obj).ObjectPool = this;

            if (PoolManager != null && obj is IXimuraPoolManagerDirectAccess)
                ((IXimuraPoolManagerDirectAccess)obj).PoolManager = PoolManager;


            mCount++;
        }
        #endregion // Get
        #region Return
        /// <summary>
        /// This method returns an object to the pool as an object and not being of a specific type.
        /// </summary>
        /// <param name="value">The object to return to the pool.</param>
        public virtual void Return(object value)
        {
            if (!(value is T))
                throw new ArgumentException("The object is not the correct type for the pool.");

            Return((T)value);
        }
        /// <summary>
        /// This method returns an object to the pool.
        /// </summary>
        /// <param name="value">The object to return to the pool.</param>
        public virtual void Return(T value)
        {
            CheckStatus();

            lock (syncObj)
            {
                if (!mObjects.ContainsKey(value.TrackID))
                    throw new ArgumentException("object is not in the pool: " + value.TrackID.ToString());
                if (mObjectsAvailable.ContainsKey(value.TrackID))
                    throw new ArgumentException("object is already returned: " + value.TrackID.ToString());

                ReturnInternal(value, false);
            }

            StatisticsUpdate();

        }

        protected virtual void ReturnInternal(T value, bool initialize)
        {
            value.Reset();

            if (value is IXimuraPoolReturnable)
                //Clear the object pool reference.
                ((IXimuraPoolReturnable)value).ObjectPool = null;

            if (value is IXimuraPoolManagerDirectAccess)
                ((IXimuraPoolManagerDirectAccess)value).PoolManager = null;

            mCount--;

            if (initialize)
                mObjects.Add(value.TrackID, new WeakReference(value));

            mObjectsAvailable.Add(value.TrackID, value);
            mAvailable.Enqueue(value.TrackID);

        }
        #endregion // Return

        #region Available
        /// <summary>
        /// This method returns the internal available property.
        /// </summary>
        /// <returns>Returns true if the pool can return objects.</returns>
        private bool InternalAvailable()
        {
            if (mMax == -1 || mAvailable.Count > 0 || ((mMax - mObjects.Count) > 0))
                return true;

            return false;
        }
        /// <summary>
        /// This method returns true if there are objects available in the pool.
        /// </summary>
        public bool Available
        {
            get
            {
                CheckStatus();

                lock (syncObj)
                {
                    return InternalAvailable();
                }
            }
        }
        #endregion // Available

        #region Max
        /// <summary>
        /// This is the maximum pool size, a value of -1 specifies no maximum size.
        /// </summary>
        public int Max { get { return mMax; } }
        #endregion // Max
        #region Min
        /// <summary>
        /// This is the minimum pool size.
        /// </summary>
        public int Min { get { return mMin; } }
        #endregion // Min
        #region Prefered
        /// <summary>
        /// This is prefered pool size.
        /// </summary>
        public int Prefered { get { return mPrefered; } }
        #endregion // Prefered
        #region Count
        /// <summary>
        /// This property returns the number of active objects.
        /// </summary>
        public int Count
        {
            get
            {
                lock (syncObj)
                {
                    return mCount;
                }
            }
        }
        #endregion // Count

        #region Clear
        /// <summary>
        /// This method is used to clear the pool of all objects.
        /// </summary>
        public virtual void Clear()
        {
            CheckStatus();
            ClearInternal();
        }
        /// <summary>
        /// This method removes all waiting objects in the pool. If the objects support IDisposable
        /// the objects will be disposed.
        /// </summary>
        protected virtual void ClearInternal()
        {
            lock (syncObj)
            {
                while (mAvailable.Count > 0)
                {
                    Guid next = mAvailable.Dequeue();
                    T obj = mObjectsAvailable[next];
                    mObjectsAvailable.Remove(next);

                    ResetPoolObject(obj);
                    if (obj is IDisposable)
                        ((IDisposable)obj).Dispose();
                }
            }

            StatisticsUpdate();
        }
        #endregion // Clear
        #region IsBuffered
        /// <summary>
        /// This property indicates whether the pool is buffered. Buffered pools are shared amongst multiple 
        /// clients and do not implement the clear method.
        /// </summary>
        public virtual bool IsBuffered
        {
            get { return false; }
        }
        #endregion

        #region PoolManager
        /// <summary>
        /// If this property is set, it will be passed on to objects that implement the IXimuraPoolManagerDirectAccess
        /// interface to allow them to get additional pool objects of different types for their own internal use.
        /// </summary>
        public virtual IXimuraPoolManager PoolManager
        {
            get;
            set;
        }
        #endregion

        #region StatisticsUpdate()
        /// <summary>
        /// This method will update any performance counters.
        /// </summary>
        protected virtual void StatisticsUpdate()
        {

        }
        #endregion // StatisticsUpdate()

        #region StatisticsInitialize()
        /// <summary>
        /// This method will initialize any performance counters.
        /// </summary>
        protected virtual void StatisticsInitialize()
        {
        }
        #endregion // StatisticsInitialize()

        #region Stats/StatsInternalNoLock
        /// <summary>
        /// This property returns a summary of the stats for the collection.
        /// </summary>
        public string Stats
        {
            get
            {
                lock (syncObj)
                {
                    return StatsInternalNoLock;
                }
            }
        }
        /// <summary>
        /// This method returns a string
        /// </summary>
        protected string StatsInternalNoLock
        {
            get
            {
                int alive = 0;
                foreach (WeakReference wr in mObjects.Values)
                    if (wr.IsAlive)
                        alive++;
                return String.Format("Available={0}/Total={1}/Alive={2}", mAvailable.Count, mObjects.Count, alive);
            }
        }
        #endregion // Stats/StatsInternalNoLock

    }
}
