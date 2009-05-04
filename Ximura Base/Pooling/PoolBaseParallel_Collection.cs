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
    public abstract partial class PoolBaseParallel<T>
    {
        #region PoolItem<U>
        /// <summary>
        /// This internal class is used to store the pool data.
        /// </summary>
        /// <typeparam name="U"></typeparam>
        protected class PoolItem<U>: IDisposable
            where U: class
        {
            #region Constructor
            /// <summary>
            /// This is the default constructor and creates a new PoolItem.
            /// </summary>
            /// <param name="val">The item value.</param>
            public PoolItem(U val): this(val, (PoolItem<U>)null)
            {
            }
            /// <summary>
            /// This constructor creates a new PoolItem and also sets the next item in the chain.
            /// </summary>
            /// <param name="val">The item value.</param>
            /// <param name="next">The next item in the chain.</param>
            public PoolItem(U val, PoolItem<U> next) 
            { 
                Value = val;
                Hashcode = val.GetHashCode();
                WR = new WeakReference(val);
                Next = next;
                mLocked = 0;
                Removed = false;
            }
            #endregion // Constructor


            private int mLocked;

            public U Value;

            public int Hashcode;

            public WeakReference WR { get; private set; }

            public PoolItem<U> Next;

            public bool Locked
            {
                get { return Interlocked.CompareExchange(ref mLocked, 1, 1)==1; }
            }

            public bool Removed;

            public void Lock()
            {

            }

            #region IDisposable Members

            public void Dispose()
            {
                throw new NotImplementedException();
            }

            #endregion




            #region IDisposable Members

            void IDisposable.Dispose()
            {
                throw new NotImplementedException();
            }

            #endregion

        }
        #endregion // PoolItem<U>

        PoolItem<T> mHead = null;
        PoolItem<T> mTail = null;

        #region ItemGet()
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected virtual T ItemGet()
        {
            
            throw new NotImplementedException();
        }
        #endregion // ItemGet()

        /// <summary>
        /// This method adds a new item to the pool.
        /// </summary>
        /// <param name="obj">The item to add to the pool.</param>
        protected virtual void ItemAdd(T obj)
        {
            if (obj == null)
                throw new ArgumentNullException("obj", "The pool item to be added is null.");

            ResetPoolObject(obj);

            PoolItem<T> item = new PoolItem<T>(obj);

            PoolItem<T> tail;
            do
            {
                tail = mTail;
                tail.Lock();
            }
            while (mTail != tail ||
                Interlocked.CompareExchange<PoolItem<T>>(ref mTail, item, tail) != tail);
        }

        #region ItemReturn(T obj, bool initialize)
        /// <summary>
        /// This method returns or adds an object to the pool.
        /// </summary>
        /// <param name="obj">The pool object to be returned.</param>
        /// <param name="initialize">This property specifies whether the pool is being initialized. 
        /// If the pool is being initialized, a new item will be added to the pool.</param>
        /// <exception cref="System.ArgumentNullException">The pool item to be returned is null.</exception>
        protected virtual void ItemReturn(T obj, bool initialize)
        {
            if (obj == null)
                throw new ArgumentNullException("obj", "The pool item to be returned is null.");

            ResetPoolObject(obj);




            throw new NotImplementedException();
        }
        #endregion // ItemReturn(T obj, bool initialize)
    }
}
