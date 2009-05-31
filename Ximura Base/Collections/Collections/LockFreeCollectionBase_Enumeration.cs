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
using System.Threading;
using System.Runtime.Serialization;
using System.Runtime.InteropServices;

using Ximura;
using Ximura.Helper;
#endregion // using
namespace Ximura.Collections
{
    public abstract partial class LockFreeCollectionBase<T>
    {
        #region InternalScan(bool changeException)
        /// <summary>
        /// This method enumerates through the collection.
        /// </summary>
        /// <param name="changeException">Set this to true if you want the method to throw an exception if the collection changes.</param>
        /// <returns>Returns a enumeration of the collection.</returns>
        /// <exception cref="System.InvalidOperationException">This exception will be thrown when the collection 
        /// changes during the scan and the changeException parameter is set to true.</exception>
        protected internal virtual IEnumerable<KeyValuePair<int, Vertex<T>>> InternalScan(bool changeException)
        {
            if (mCount == 0)
                yield break;

            long currentVersion = mVersion;

            KeyValuePair<int, Vertex<T>> item = new KeyValuePair<int, Vertex<T>>(0, mSlots[cnIndexData]);
            yield return item;

            while (!item.Value.IsTerminator)
            {
                item = new KeyValuePair<int, Vertex<T>>(item.Value.NextSlotIDPlus1 - 1, mSlots[item.Value.NextSlotIDPlus1 - 1]);

                if (changeException && currentVersion != mVersion)
                    throw new InvalidOperationException("The version has changed");

                yield return item;
            }
        }
        #endregion // InternalScan(bool changeException)

        #region GetEnumerator()
        /// <summary>
        /// This method returns an enumeration of the collection values.
        /// </summary>
        /// <returns>Returns a enumeration of the collection.</returns>
        /// <exception cref="System.InvalidOperationException">This exception will be thrown when if collection 
        /// changes during the enumeration.</exception>
        public IEnumerator<T> GetEnumerator()
        {
            //Enumerate the default(T) values.
            for (int i = mDefaultTCount; i > 0; i--)
                yield return default(T);
            //Enumerate the data.
            foreach (var item in InternalScan(true))
                if (!item.Value.IsSentinel)
                    yield return item.Value.Value;
        }
        #endregion // GetEnumerator()
        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator)GetEnumerator();
        }

        #endregion

        #region CopyToInternal(T[] array, int arrayIndex)
        /// <summary>
        /// This method copies the collection to the array specified.
        /// </summary>
        /// <param name="array">The array to copy to.</param>
        /// <param name="arrayIndex">The array index where the class should start copying to.</param>
        protected virtual void CopyToInternal(T[] array, int arrayIndex)
        {
            this.ForIndex((i, d) => array[i + arrayIndex] = d);
        }
        /// <summary>
        /// This method copies the collection to the array specified.
        /// </summary>
        /// <param name="array">The array to copy to.</param>
        /// <param name="arrayIndex">The array index where the class should start copying to.</param>
        protected virtual void CopyToInternal(Array array, int arrayIndex)
        {  
            this.ForIndex((i, d) => array.SetValue(d,i));
        }

        #endregion // CopyTo(T[] array, int arrayIndex)

        #region ToArrayInternal()
        /// <summary>
        /// This method copies the internal data to an array.
        /// </summary>
        /// <returns>Returns an array containing the internal data.</returns>
        protected virtual T[] ToArrayInternal()
        {
            T[] array = new T[CountInternal];
            CopyToInternal(array, 0);
            return array;
        }
        #endregion // ToArrayInternal()

    }
}
