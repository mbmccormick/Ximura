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
namespace Ximura.Collections.Data
{
    /// <summary>
    /// This structure is used to hold the item in the collection.
    /// </summary>
    /// <typeparam name="T">The container object.</typeparam>
    [Serializable, StructLayout(LayoutKind.Sequential)]
    public struct CollectionVertexStruct<T> : ICollectionVertex<T>
    {
        #region Constants
        /// <summary>
        /// This is the empty vertex.
        /// </summary>
        public static readonly CollectionVertexStruct<T> Empty;

        private const int cnSentinelMaskSet = 0x40000000;
        private const int cnSentinelMaskRemove = 0x3FFFFFFF;
        private const int cnMarkedMaskSet = unchecked((int)0x80000000);
        private const int cnMarkedMaskRemove = 0x7FFFFFFF;
        #endregion // Constants
        #region Static methods
        /// <summary>
        /// This is the static constructor.
        /// </summary>
        static CollectionVertexStruct()
        {
            Empty = new CollectionVertexStruct<T>();
        }
        /// <summary>
        /// This static method creates a sentinel vertex. Sentinel vertexes are vertexes that do not include data,
        /// but are used by the hash table to mark a shortcut to data sets based on their hashcode.
        /// </summary>
        /// <param name="hashID">The hashID.</param>
        /// <param name="nextSlotIDPlus1">The ID of the next vertex in the chain (plus 1).</param>
        /// <returns>Returns a new sentinel for the specific hash ID.</returns>
        public static CollectionVertexStruct<T> Sentinel(int hashID, int nextSlotIDPlus1)
        {
            return new CollectionVertexStruct<T>(hashID, nextSlotIDPlus1);
        }
        #endregion // Static methods

        #region Declarations
        /// <summary>
        /// The internal hashid.
        /// </summary>
        private int mHashID;
        #endregion // Declarations
        #region Constructor
        /// <summary>
        /// This constructor creates a slot as a sentinel, with only the next parameter set.
        /// </summary>
        /// <param name="hashID">The item hashcode.</param>
        /// <param name="nextSlotIDPlus1">The next item in the list.</param>
        public CollectionVertexStruct(int hashID, int nextSlotIDPlus1)
        {
            mHashID = hashID | cnSentinelMaskSet;
            Value = default(T);
            NextSlotIDPlus1 = nextSlotIDPlus1;
        }
        /// <summary>
        /// This constructor sets the value for the slot.
        /// </summary>
        /// <param name="hashID">The item hashcode.</param>
        /// <param name="value">The slot value.</param>
        /// <param name="nextSlotIDPlus1">The next item in the list.</param>
        public CollectionVertexStruct(int hashID, T value, int nextSlotIDPlus1)
        {
            mHashID = hashID;
            Value = value;
            NextSlotIDPlus1 = nextSlotIDPlus1;
        }
        #endregion // Constructor

        #region HashID
        /// <summary>
        /// The item hashid.
        /// </summary>
        public int HashID { get { return mHashID & cnSentinelMaskRemove; } }
        #endregion
        #region NextSlotIDPlus1
        /// <summary>
        /// The next item in the list.
        /// </summary>
        public int NextSlotIDPlus1;
        #endregion

        #region Value
        /// <summary>
        /// The slot value.
        /// </summary>
        public T Value;
        #endregion // Value
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
        public bool IsSentinel { get { return (mHashID & cnSentinelMaskSet) > 0; } }
        #endregion // IsSentinel

        public bool IsMarked { get { return (mHashID & cnMarkedMaskSet) > 0; } }

        public bool TryMark()
        {
            return Interlocked.CompareExchange(ref mHashID, unchecked(cnMarkedMaskSet | mHashID), mHashID) == mHashID;
        }

        #region ToString()
        /// <summary>
        /// This override provides quick and easy debugging support.
        /// </summary>
        /// <returns>Returns a string representation of the vertex.</returns>
        public override string ToString()
        {
            if (IsSentinel)
                return string.Format("V->{0}  H{1:X} [SNTL]", IsTerminator ? "END" : (NextSlotIDPlus1 - 1).ToString(), HashID);
            else
                return string.Format("V->{0}  H{1:X} =[{2}]", IsTerminator ? "END" : (NextSlotIDPlus1 - 1).ToString(), HashID, Value.ToString());
        }
        #endregion // ToString()

        #region Data
        /// <summary>
        /// This is the information contained in the vertex.
        /// </summary>
        public T Data
        {
            get { return Value; }
        }
        #endregion
    }
}
