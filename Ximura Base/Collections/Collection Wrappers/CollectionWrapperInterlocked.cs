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
    public class CollectionWrapperInterlocked<T> : CollectionWrapperBase<T>, ICollection<T>
    {
        #region Declarations

        int mLock = 0;

        #endregion // Declarations

        public Predicate<ICollection<T>> DebugStopPoint { get; set; }

        public T StopPoint { get; set; }

        public CollectionWrapperInterlocked()
            : base()
        {
            DebugStopPoint = null;
        }

        protected void EnterLock()
        {
            while (Interlocked.CompareExchange(ref mLock, 1, 0) == 1)
            {
                if ((Environment.ProcessorCount > 1))
                {
                    Thread.SpinWait(20);
                }
                else
                {
                    Thread.Sleep(0);
                }
            }

        }

        protected void ExitLock()
        {
            mLock = 0;
            //Interlocked.Exchange(ref mLock, 0);
        }

        #region ICollection<T> Members

        bool check = false;

        public void Add(T item)
        {
            int enter = Environment.TickCount;
            try
            {
                EnterLock();

                //if (!check && DebugStopPoint != null)
                //{
                //    check = DebugStopPoint(Collection);

                //    if (check)
                //        return;
                //}

                //if (!check)
                //{
                //    check = mCollection.Contains(StopPoint);

                //    if (check)
                //        return;
                //}

                Collection.Add(item);
            }
            finally
            {
                ExitLock();
            }
        }

        public void Clear()
        {
            try
            {
                EnterLock();
                Collection.Clear();
            }
            finally
            {
                ExitLock();
            }
        }

        public bool Contains(T item)
        {
            try
            {
                EnterLock();
                return Collection.Contains(item);
            }
            finally
            {
                ExitLock();
            }
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            try
            {
                EnterLock();
                Collection.CopyTo(array, arrayIndex);
            }
            finally
            {
                ExitLock();
            }
        }

        public int Count
        {
            get
            {
                try
                {
                    EnterLock();
                    return Collection.Count;
                }
                finally
                {
                    ExitLock();
                }
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        public bool Remove(T item)
        {
            try
            {
                EnterLock();
                return Collection.Remove(item);
            }
            finally
            {
                ExitLock();
            }
        }

        #endregion

        #region IEnumerable<T> Members

        public IEnumerator<T> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator)GetEnumerator();
        }

        #endregion
    }

}
