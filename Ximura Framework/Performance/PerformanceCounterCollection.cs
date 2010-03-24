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
using System.ComponentModel;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;

using Ximura;
using Ximura.Helper;
using Ximura.Framework;
#endregion // using
namespace Ximura
{
    /// <summary>
    /// The PerformanceCounterCollection class provides group functionality
    /// for related performance counters.
    /// </summary>
    public class PerformanceCounterCollection : PerformanceBase, IXimuraPerformanceCounterCollection
    {
        #region Declarations
        /// <summary>
        /// This is the base performance counter collection.
        /// </summary>
        protected List<IXimuraPerformanceCounter> mPerformanceCounters;
        /// <summary>
        /// This is the base synchronization object.
        /// </summary>
        private object syncObject = new object();
        #endregion // Declarations
        #region Constructors
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        public PerformanceCounterCollection()
        {
            mPerformanceCounters = new List<IXimuraPerformanceCounter>();
            PerformanceCountersCreate();
        }
        #endregion // Constructors

        #region PerformanceCountersCreate()
        /// <summary>
        /// This method creates the performance counters for the collection. You should override this method to add your own counters.
        /// </summary>
        public virtual void PerformanceCountersCreate()
        {

        }
        #endregion

        #region Add(IXimuraPerformanceCounter item)
        /// <summary>
        /// This method adds a counter to the collection.
        /// </summary>
        /// <param name="item"></param>
        public void Add(IXimuraPerformanceCounter item)
        {
            lock (syncObject)
            {
                mPerformanceCounters.Add(item);
            }
        }
        #endregion
        #region Clear()
        /// <summary>
        /// This method clears the collection.
        /// </summary>
        public void Clear()
        {
            lock (syncObject)
            {
                mPerformanceCounters.Clear();
            }
        }
        #endregion // Clear()
        #region Contains(IXimuraPerformanceCounter item)
        /// <summary>
        /// This method returns true if the item in in the collection.
        /// </summary>
        /// <param name="item">The item to check.</param>
        /// <returns>Returns true if the item is in the collection.</returns>
        public bool Contains(IXimuraPerformanceCounter item)
        {
            lock (syncObject)
            {
                return mPerformanceCounters.Contains(item);
            }
        }
        #endregion // Contains(IXimuraPerformanceCounter item)
        #region CopyTo(IXimuraPerformanceCounter[] array, int arrayIndex)
        /// <summary>
        /// This method copies the collection to the array passed.
        /// </summary>
        /// <param name="array">The array.</param>
        /// <param name="arrayIndex">The position in the index to begin copying.</param>
        public void CopyTo(IXimuraPerformanceCounter[] array, int arrayIndex)
        {
            lock (syncObject)
            {
                mPerformanceCounters.CopyTo(array, arrayIndex);
            }
        }
        #endregion // CopyTo(IXimuraPerformanceCounter[] array, int arrayIndex)
        #region Count
        /// <summary>
        /// This is the collection length.
        /// </summary>
        public int Count
        {
            get
            {
                lock (syncObject)
                {
                    return mPerformanceCounters.Count;
                }
            }
        }
        #endregion // Count

        #region IsReadOnly
        /// <summary>
        /// This method returns false as the performance counter collection is not locked.
        /// </summary>
        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }
        #endregion // IsReadOnly
        #region Remove(IXimuraPerformanceCounter item)
        /// <summary>
        /// This method removes an item from the collection.
        /// </summary>
        /// <param name="item">The item to remove.</param>
        /// <returns>Return true if the item is successfully removed.</returns>
        public bool Remove(IXimuraPerformanceCounter item)
        {
            lock (syncObject)
            {
                return mPerformanceCounters.Remove(item);
            }
        }
        #endregion // Remove(IXimuraPerformanceCounter item)

        #region IEnumerable<IXimuraPerformanceCounter> Members
        /// <summary>
        /// This is the generic enumerator for the collection.
        /// </summary>
        /// <returns>Returns an lock-free enumerator for the collection.</returns>
        public IEnumerator<IXimuraPerformanceCounter> GetEnumerator()
        {
            IXimuraPerformanceCounter[] array = null;
            int len = 0;

            lock (syncObject)
            {
                len = mPerformanceCounters.Count;

                if (len > 0)
                {
                    array = new IXimuraPerformanceCounter[len];
                    mPerformanceCounters.CopyTo(array, len);
                }
            }

            if (len == 0)
                yield break;

            foreach (IXimuraPerformanceCounter counter in array)
                yield return counter;
        }

        #endregion

        #region IEnumerable Members
        /// <summary>
        /// This is the standard enumerator.
        /// </summary>
        /// <returns>Returns an lock-free enumerator for the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator)this.GetEnumerator();
        }

        #endregion
    }
}
