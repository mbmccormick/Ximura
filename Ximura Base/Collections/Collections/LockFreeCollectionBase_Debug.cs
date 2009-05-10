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
using System.Threading;
using System.Runtime.Serialization;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;

using Ximura;
using Ximura.Helper;
#endregion // using
namespace Ximura.Collections
{
    public abstract partial class LockFreeCollectionBase<T>
    {
        #region DebugData/DebugEmpty
#if (DEBUG)
        #region DebugData
        /// <summary>
        /// This debug method enumerates through the collection.
        /// </summary>
        public IEnumerable<KeyValuePair<int, Vertex<T>>> DebugData
        {
            get
            {
                return InternalScan(false);
            }
        }
        #endregion // DebugData

        #region DebugEmpty
        /// <summary>
        /// This debug method enumerates through the collection.
        /// </summary>
        public IEnumerable<KeyValuePair<int, Vertex<T>>> DebugEmpty
        {
            get
            {
                int currentVersion = mVersion;

                KeyValuePair<int, Vertex<T>> item = new KeyValuePair<int, Vertex<T>>(0, mSlots[cnIndexEmptyQueue]);
                yield return item;

                while (!item.Value.IsTerminator)
                {
                    item = new KeyValuePair<int, Vertex<T>>(item.Value.NextIDPlus1 - 1, mSlots[item.Value.NextIDPlus1 - 1]);
                    yield return item;
                }
            }
        }
        #endregion // DebugEmpty

        public string DebugDump
        {
            get
            {
                StringBuilder sb = new StringBuilder();
#if (PROFILING)
                sb.AppendLine(ProfileStats);
#endif
                return sb.ToString();
            }
        }
#endif
        #endregion // DebugScan

        public void DebugReset()
        {
#if (PROFILING)
            ProfilingSetup();
#endif
        }
    }
}
