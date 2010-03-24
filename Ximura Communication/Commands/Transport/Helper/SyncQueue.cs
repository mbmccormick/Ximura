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
﻿#region using
using System;
using System.IO;
using System.Collections;
using System.Diagnostics;
using System.Data;
using System.Reflection;
using System.ComponentModel;
using System.Collections.Generic;

using Ximura;
using Ximura.Data;
using CH = Ximura.Common;

using Ximura.Framework;


using Ximura.Framework;

#endregion // using
namespace Ximura.Communication
{
    /// <summary>
    /// This class provides a simplyfied queue that provides basic queue functionality that is thread safe.
    /// </summary>
    public class SyncQueue<T>
    {
        private Queue<T> mQueue;
        private object syncObject = new object();

        public SyncQueue()
        {
            mQueue = new Queue<T>();
        }

        public int Count
        {
            get
            {
                lock (syncObject)
                {
                    return mQueue.Count;
                }
            }
        }

        // Summary:
        //     Removes all objects from the System.Collections.Generic.Queue<T>.
        public void Clear()
        {
            lock (syncObject)
            {
                mQueue.Clear();
            }
        }
        //
        // Summary:
        //     Removes and returns the object at the beginning of the System.Collections.Generic.Queue<T>.
        //
        // Returns:
        //     The object that is removed from the beginning of the System.Collections.Generic.Queue<T>.
        //
        // Exceptions:
        //   System.InvalidOperationException:
        //     The System.Collections.Generic.Queue<T> is empty.
        public T Dequeue()
        {
            lock (syncObject)
            {
                return mQueue.Dequeue();
            }
        }
        //
        // Summary:
        //     Adds an object to the end of the System.Collections.Generic.Queue<T>.
        //
        // Parameters:
        //   item:
        //     The object to add to the System.Collections.Generic.Queue<T>. The value can
        //     be null for reference types.
        public void Enqueue(T item)
        {
            lock (syncObject)
            {
                mQueue.Enqueue(item);
            }
        }
        //
        // Summary:
        //     Returns the object at the beginning of the System.Collections.Generic.Queue<T>
        //     without removing it.
        //
        // Returns:
        //     The object at the beginning of the System.Collections.Generic.Queue<T>.
        //
        // Exceptions:
        //   System.InvalidOperationException:
        //     The System.Collections.Generic.Queue<T> is empty.
        public T Peek()
        {
            lock (syncObject)
            {
                return mQueue.Peek();
            }
        }
    }

}
