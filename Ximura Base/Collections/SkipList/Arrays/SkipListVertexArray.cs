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
using System.Runtime.Serialization;
using System.Threading;

using Ximura;
using Ximura.Helper;
#endregion // using
namespace Ximura.Collections
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T">The collection type.</typeparam>
    public class SkipListVertexArray<T> : VertexArray<T>
    {
        #region Declarations
        /// <summary>
        /// The probability of an item being propagated to the next level.
        /// </summary>
        private double mProbability;
        /// <summary>
        /// The maximum number of levels.
        /// </summary>
        private int mMaxLevel;
        #endregion // Declarations
        #region Constructor
        /// <summary>
        /// This is the default constructor for the array.
        /// </summary>
        /// <param name="isFixedSize">A boolean value indicating whether the data collection is fixed size.</param>
        /// <param name="capacity">The array capacity.</param>
        /// <param name="probability">The probability of an item being propagated to the next level, which should be between 0 and 1.</param>
        /// <param name="maxLevel">The maximum number of levels.</param>
        public SkipListVertexArray(bool isFixedSize, int capacity, double probability, int maxLevel)
        {
            mProbability = probability;
            mMaxLevel = maxLevel > 32 ? 32 : maxLevel;
            Initialize(isFixedSize, capacity);
        }
        #endregion // Constructor

        #region Initialize(bool isFixedSize, int capacity)
        /// <summary>
        /// This method initializes the data collection.
        /// </summary>
        /// <param name="isFixedSize">Specifies whether the collection is a fixed size.</param>
        /// <param name="capacity">The initial capacity.</param>
        protected override void Initialize(bool isFixedSize, int capacity)
        {
            base.Initialize(isFixedSize, capacity);

            mProbability = 0.5;

            int first = EmptyGet();

            this[first] = CollectionVertex<T>.Sentinel(0, 0);
        }
        #endregion // Initialize(bool isFixedSize, int capacity)

        #region GetSentinelID(int hashCode, bool create, out int sentIndexID, out int hashID)
        /// <summary>
        /// This method returns the sentinel ID and the hashID for the hashcode passed.
        /// </summary>
        /// <param name="hashCode">The hashcode to search for the sentinel position.</param>
        /// <param name="create">This property determine whether any missing sentinels will be created.</param>
        /// <param name="sentIndexID">The largest sentinel index ID.</param>
        /// <param name="hashID">The hashID for the hashCode that passed.</param>
        public override void GetSentinelID(int hashCode, bool create, out int sentIndexID, out int hashID)
        {
            hashCode &= cnLowerBitMask;

            //hashID = BitReverse(hashCode);
            hashID = hashCode;

            sentIndexID = 0;
        }
        #endregion // GetSentinelID(int hashCode, bool create, out int sentIndexID, out int hashID)

        #region SizeRecalculate(int total)
        /// <summary>
        /// 
        /// </summary>
        /// <param name="total"></param>
        public override void SizeRecalculate(int total)
        {

        }
        #endregion // SizeRecalculate(int total)

    }
}
