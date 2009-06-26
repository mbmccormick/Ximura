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
using System.Text;

using Ximura;
using Ximura.Helper;
#endregion // using
namespace Ximura.Collections
{
    public interface IVertexArray<T> : ICollectionState, IEnumerable<KeyValuePair<int, ICollectionVertex<T>>>
    {
        void Initialize(IEqualityComparer<T> eqComparer, bool isFixedSize, int capacity, bool allowNullValues, bool allowMultipleEntries);

        IVertexWindow<T> VertexWindowGet();
        IVertexWindow<T> VertexWindowGet(T item, bool createSentinel);

        bool SupportsFastContain { get; }
        bool? FastContains(T item);
        bool? FastContains(IEqualityComparer<T> eqComparer, T key, out T value);

        bool SupportsFastAdd { get; }
        bool FastAdd(T item, bool add);

        bool SupportsFastRemove { get; }
        bool FastRemove(T item);

        bool SupportsFastClear { get; }
        void FastClear();
    }
}
