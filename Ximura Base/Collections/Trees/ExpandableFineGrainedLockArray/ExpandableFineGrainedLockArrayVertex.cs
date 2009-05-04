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
    /// <summary>
    /// This vertex holds the array and the key that specifies the starting position within the collection.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ExpandableFineGrainedLockArrayVertex<T> : LockFreeRedBlackVertex<int, FineGrainedLockArray<T>>
    {
        /// <summary>
        /// This is the specific comparer for the Expandable array.
        /// </summary>
        /// <param name="vertex">The lock array vertex.</param>
        /// <param name="key">The position key.</param>
        /// <returns>
        /// Returns 0 if the key is equal to the vertex. 
        /// Returns -1 if the key is less than the vertex, and returns 1 if the key is greater than the vertex.
        /// </returns>
        protected override int Comparer(LockFreeRedBlackVertex<int, FineGrainedLockArray<T>> vertex, int key)
        {
            //Ok check if the key is lower than this key.
            if (key < vertex.Key)
                return -1;
            //Is the key within the range contained within this collection, yes then this is a match.
            if (key < vertex.Key + vertex.Value.Length)
                return 0;
            //Key must be greater.
            return 1;
        }
    }
}
