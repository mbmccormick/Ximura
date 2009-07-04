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
    public class CollectionWrapperReadWrite<T> : CollectionWrapperBase<T>, ICollection<T>
    {
        #region Declarations
        ReaderWriterLockSlim syncLock;
        #endregion // Declarations

        public CollectionWrapperReadWrite()
            : base()
        {
            syncLock = new ReaderWriterLockSlim();
        }

        #region ICollection<T> Members

        public void Add(T item)
        {
            try
            {
                syncLock.EnterWriteLock();
                Collection.Add(item);
            }
            finally
            {
                syncLock.ExitWriteLock();
            }
        }

        public void Clear()
        {
            try
            {
                syncLock.EnterWriteLock();
                Collection.Clear();
            }
            finally
            {
                syncLock.ExitWriteLock();
            }
        }

        public bool Contains(T item)
        {
            try
            {
                syncLock.EnterReadLock();
                return Collection.Contains(item);
            }
            finally
            {
                syncLock.ExitReadLock();
            }
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            try
            {
                syncLock.EnterReadLock();
                Collection.CopyTo(array, arrayIndex);
            }
            finally
            {
                syncLock.ExitReadLock();
            }
        }

        public int Count
        {
            get
            {
                try
                {
                    syncLock.EnterReadLock();
                    return Collection.Count;
                }
                finally
                {
                    syncLock.ExitReadLock();
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
                syncLock.EnterWriteLock();
                return Collection.Remove(item);
            }
            finally
            {
                syncLock.ExitWriteLock();
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
