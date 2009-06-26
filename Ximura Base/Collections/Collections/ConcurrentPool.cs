#region Copyright
// *******************************************************************************
// Copyright (c) 2000-2009 Paul Stancer.
// All rights reserved. This program and the accompanying materials
// are made available under the terms of the Eclipse Public License v1.0
// which accompanies this distribution, and is available at
// http://www.eclipse.org/legal/epl-v10.html
//
// For more details see http://ximura.org
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
namespace Ximura.Collections
{
    public class ConcurrentPool<T, A> : CollectionHelperBase<T, A>, IPoolInitialization<T>
        where T: class
        where A : class, IVertexArray<T>, new()
    {
        #region Declarations
        private int mMax = -1;
        private int mMin = 0;
        private int mPrefered = 0;

        private bool mIsFixedSize = false;
        #endregion // Declarations

        /// <summary>
        /// This is the default constructor. The collection will be constructed with a base capacity of 1000.
        /// </summary>
        public ConcurrentPool() 
            : base(null, 1000, null, false) { }

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

        #region IsFixedSize
        /// <summary>
        /// This property determines whether the collection will dynamically expand when new values are added. 
        /// This property can only be set from the constructor, although this property can be overriden in derived classes to ensure
        /// a particular value.
        /// </summary>
        public virtual bool IsFixedSize { get { return mIsFixedSize; } }
        #endregion

        #region Available
        /// <summary>
        /// This method returns true if there are objects available in the pool.
        /// </summary>
        public bool Available
        {
            get { throw new NotImplementedException(); }
        }
        #endregion // Available
        #region Count
        /// <summary>
        /// This property returns the number of active objects.
        /// </summary>
        public int Count
        {
            get { return mData.Count; }
        }
        #endregion // Count

        #region Return/TryReturn
        /// <summary>
        /// This method returns an item to the pool.
        /// </summary>
        /// <param name="value">The item to return to the pool.</param>
        public void Return(T value)
        {
            if (!TryReturn(value))
                throw new InvalidOperationException("The item does not belong to the pool.");
        }
        /// <summary>
        /// This method attemtps to return an object to the pool.
        /// </summary>
        /// <param name="value">The item to return.</param>
        /// <returns>Returns true if the item was successfully returned.</returns>
        public bool TryReturn(T value)
        {
            throw new NotImplementedException();
        }
        #endregion // Return/TryReturn

        #region ICollectionBase<T> Members

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(T item)
        {
            throw new NotImplementedException();
        }

        #region CopyTo(T[] array, int arrayIndex)
        /// <summary>
        /// This method copies the collection to the array specified.
        /// </summary>
        /// <param name="array">The array to copy to.</param>
        /// <param name="arrayIndex">The array index where the class should start copying to.</param>
        public void CopyTo(T[] array, int arrayIndex)
        {
            DisposedCheck();
            CopyToInternal(array, arrayIndex);
        }
        #endregion // CopyTo(T[] array, int arrayIndex)

        public T[] ToArray()
        {
            throw new NotImplementedException();
        }

        public void TrimExcess()
        {
            throw new NotImplementedException();
        }

        public T Peek()
        {
            throw new NotImplementedException();
        }

        public bool TryPeek(out T item)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IEnumerable Members

        public new IEnumerator GetEnumerator()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region ICollection Members

        public void CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }



        public bool IsSynchronized
        {
            get { return false; }
        }

        public object SyncRoot
        {
            get { throw new NotImplementedException("SyncRoot is not implemented."); }
        }

        #endregion

        #region Get helper methods
        /// <summary>
        /// This method retrieves an item from the pool.
        /// </summary>
        /// <returns>Returns an item from the pool.</returns>
        public T Get()
        {
            T value;
            if (!TryGet(out value))
                throw new InvalidOperationException("The pool does not have any items available.");

            return value;
        }
        /// <summary>
        /// The Get() method takes an object from the pool, if 
        /// there are no objects available the pool will create a new object.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The serialization context.</param>
        /// <returns>An object of the type defined in the pool definition, with the serialization data.</returns>
        public T Get(SerializationInfo info, StreamingContext context)
        {
            T value;
            if (!TryGet(info, context, out value))
                throw new InvalidOperationException("The pool does not have any items available.");

            return value;
        }
        /// <summary>
        /// The Get() method takes an object from the pool, if 
        /// there are no objects available the pool will create a new object.
        /// </summary>
        /// <param name="initializer">This action will be performed on the object after the data has been deserialized.</param>
        /// <returns>An object of the type defined in the pool definition.</returns>
        public T Get(Action<T> initializer)
        {
            T value;
            if (!TryGet(initializer, out value))
                throw new InvalidOperationException("The pool does not have any items available.");

            return value;
        }
        /// <summary>
        /// The Get() method takes an object from the pool, if 
        /// there are no objects available the pool will create a new object.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The serialization context.</param>
        /// <param name="initializer">This action will be performed on the object after the data has been deserialized.</param>
        /// <returns>An object of the type defined in the pool definition, with the serialization data.</returns>
        public T Get(SerializationInfo info, StreamingContext context, Action<T> initializer)
        {
            T value;
            if (!TryGet(info, context, initializer, out value))
                throw new InvalidOperationException("The pool does not have any items available.");

            return value;
        }
        #endregion
        #region TryGet helper methods
        /// <summary>
        /// The Get() method takes an object from the pool, if 
        /// there are no objects available the pool will create a new object.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The serialization context.</param>
        /// <param name="value">An object of the type defined in the pool definition, with the serialization data.</param>
        /// <returns>Returns true if an item has been returned from the pool.</returns>
        public virtual bool TryGet(SerializationInfo info, StreamingContext context, out T value)
        {
            if (!TryGet(out value))
                return false;

            ResetPoolObject(value, info, context);

            return true;
        }
        /// <summary>
        /// The Get() method takes an object from the pool, if 
        /// there are no objects available the pool will create a new object.
        /// </summary>
        /// <param name="initializer">This action will be performed on the object after the data has been deserialized.</param>
        /// <param name="value">An object of the type defined in the pool definition, with the serialization data.</param>
        /// <returns>Returns true if an item has been returned from the pool.</returns>
        public virtual bool TryGet(Action<T> initializer, out T value)
        {
            if (!TryGet(out value))
                return false;

            if (initializer != null)
                initializer(value);

            return true;
        }
        /// <summary>
        /// The Get() method takes an object from the pool, if 
        /// there are no objects available the pool will create a new object.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The serialization context.</param>
        /// <param name="initializer">This action will be performed on the object after the data has been deserialized.</param>
        /// <param name="value">An object of the type defined in the pool definition, with the serialization data.</param>
        /// <returns>Returns true if an item has been returned from the pool.</returns>
        public virtual bool TryGet(SerializationInfo info, StreamingContext context, Action<T> initializer, out T value)
        {
            if (!TryGet(out value))
                return false;

            ResetPoolObject(value, info, context);

            if (initializer != null)
                initializer(value);

            return true;
        }
        #endregion // TryGet

        #region TryGet(out T value)
        /// <summary>
        /// This method attempts to take an item from the pool.
        /// </summary>
        /// <param name="value">The item from the pool.</param>
        /// <returns>Returns true if an item has been returned from the pool.</returns>
        public virtual bool TryGet(out T value)
        {
            throw new NotImplementedException();
        }
        #endregion // TryGet(out T value)
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

            //if (obj is IXimuraPoolableObjectDeserializable)
            //{
            //    ((IXimuraPoolableObjectDeserializable)obj).Reset(info, context);
            //    ((IXimuraPoolableObjectDeserializable)obj).OnDeserialization(null);
            //    return;
            //}

            //throw new SerializationException("Object does not implement IXimuraPoolableObjectDeserializable.");
            throw new NotImplementedException("ResetPoolObject is not implemented.");
        }
        #endregion // ResetPoolObject(T obj, SerializationInfo info, StreamingContext context)
    }
}
