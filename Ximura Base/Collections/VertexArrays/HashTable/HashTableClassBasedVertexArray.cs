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
    /// <summary>
    /// This class contains the combined buckets and slots in a single class.
    /// </summary>
    /// <typeparam name="T">The collection type.</typeparam>
    public class HashTableClassBasedVertexArray<T> : MultiLevelClassBasedVertexArray<T>
    {
        #region Constants
        private const double elog2 = 0.693147181;
        #endregion // Declarations




        protected override VertexClass<T> GetSentinelID(int hashCode, bool createSentinel, out int hashID)
        {
            hashID = BitReverse(hashCode & cnLowerBitMask);

            return Root;
        }
    }
}
