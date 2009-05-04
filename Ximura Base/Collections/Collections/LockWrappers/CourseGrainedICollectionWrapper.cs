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
namespace Ximura.Collections
{
    public class CourseGrainedICollectionWrapper<T> : WrapperBase<T>, ICollection<T>
    {
        #region Declarations
        object syncLock = new object();
        #endregion // Declarations

        public CourseGrainedICollectionWrapper()
            : base()
        {

        }

        #region ICollection<T> Members

        public void Add(T item)
        {
            lock (syncLock)
            {
                Collection.Add(item);
            }
        }

        public void Clear()
        {
            lock (syncLock)
            {
                Collection.Clear();
            }
        }

        public bool Contains(T item)
        {
            lock (syncLock)
            {
                return Collection.Contains(item);
            }
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            lock (syncLock)
            {
                Collection.CopyTo(array, arrayIndex);
            }
        }

        public int Count
        {
            get
            {
                lock (syncLock)
                {
                    return Collection.Count;
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
            lock (syncLock)
            {
                return Collection.Remove(item);
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
