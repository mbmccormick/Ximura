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
using Ximura.Collections;
using Ximura.Collections.Data;
#endregion // using
namespace Ximura.Collections
{
    /// <summary>
    /// This structure is used to hold the item in the collection.
    /// </summary>
    [Serializable, StructLayout(LayoutKind.Sequential)]
    public struct SkipListSentinelVertexStruct
    {
        #region Constants
        /// <summary>
        /// This is the empty vertex.
        /// </summary>
        private const int cnSentinelMaskSet = 0x40000000;
        private const int cnSentinelMaskRemove = 0x3FFFFFFF;
        #endregion // Constants

        #region Constructor
        /// <summary>
        /// This constructor creates a slot as a sentinel, with only the next parameter set.
        /// </summary>
        /// <param name="hashID">The item hashcode.</param>
        /// <param name="nextSlotIDPlus1">The next item in the list.</param>
        public SkipListSentinelVertexStruct(int hashID, int nextSlotIDPlus1)
        {
            HashID = hashID;
            NextSlotIDPlus1 = nextSlotIDPlus1;
            DownIDPlus1 = 0;
        }
        #endregion // Constructor

        #region HashID
        /// <summary>
        /// The item hashid.
        /// </summary>
        public int HashID;
        #endregion
        #region NextSlotIDPlus1
        /// <summary>
        /// The next item in the list.
        /// </summary>
        public int NextSlotIDPlus1;
        #endregion
        #region DownIDPlus1
        /// <summary>
        /// The next item in the list.
        /// </summary>
        public int DownIDPlus1;
        #endregion

        #region IsTerminator
        /// <summary>
        /// This property identifies whether the vertex is the last item in the data chain.
        /// </summary>
        public bool IsTerminator { get { return NextSlotIDPlus1 == 0; } }
        #endregion // IsTerminator
        #region IsSentinel
        /// <summary>
        /// This property identifies whether the vertex is a sentinel vertex.
        /// </summary>
        public bool IsSentinel { get { return (DownIDPlus1 & cnSentinelMaskSet) > 0; } }
        #endregion // IsSentinel

        #region ToString()
        /// <summary>
        /// This override provides quick and easy debugging support.
        /// </summary>
        /// <returns>Returns a string representation of the vertex.</returns>
        public override string ToString()
        {
            return string.Format("V->{0}  H{1:X} [SNTL]", IsTerminator ? "END" : (NextSlotIDPlus1 - 1).ToString(), HashID);
        }
        #endregion // ToString()
    }
}
