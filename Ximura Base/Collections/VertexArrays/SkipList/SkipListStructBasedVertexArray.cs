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
    /// This vertex array implements the data as a skip list array.
    /// </summary>
    /// <typeparam name="T">The collection type.</typeparam>
    public class SkipListStructBasedVertexArray<T> : MultiLevelStructBasedVertexArray<T, int[]>
    {
        #region Static Declarations
        /// <summary>
        /// This is a thread specific value. This is to ensure that each thread gets a specific
        /// random class as Random is not thread safe.
        /// </summary>
        [ThreadStatic()]
        static Random sRand;
        #endregion // Static Declarations
        #region Declarations
        /// <summary>
        /// The probability of an item being propagated to the next level.
        /// </summary>
        private double mProbability;
        #endregion // Declarations
        #region Constructor
        /// <summary>
        /// This is the default constructor for the array.
        /// </summary>
        public SkipListStructBasedVertexArray()
        {
        }
        #endregion // Constructor

        #region ConvertProbabilityToBool()
        /// <summary>
        /// This method converts the output from the thread specific random function to a simple boolean value.
        /// </summary>
        /// <returns>Returns a boolean value based on the probability factor.</returns>
        private bool ConvertProbabilityToBool()
        {
            //Create the random function for the specific thread.
            if (sRand == null)
                sRand = new Random();

            return (sRand.NextDouble() < mProbability);
        }
        #endregion
        #region Probability
        /// <summary>
        /// The probability of an item being propagated to the next level.
        /// </summary>
        public virtual double Probability
        {
            get { return mProbability; }
            set
            {
                if (value <= 0 || value > 1)
                    throw new ArgumentOutOfRangeException("The probability must be greater than 0 and less than or equal to 1.");

                //We have to use interlock for setting this value as it is read by multiple threads at the same time.
                Interlocked.Exchange(ref mProbability, value);
            }
        }
        #endregion // Probability
        #region LevelMax
        /// <summary>
        /// This is the maximum levels implemented by the skip list.
        /// </summary>
        public override int LevelMax { get { return 16; } }
        #endregion // LevelMax


        protected override int GetSentinelID(int hashCode, bool createSentinel, out int hashID)
        {
            throw new NotImplementedException();
        }

        public override void SizeRecalculate(int total)
        {
            throw new NotImplementedException();
        }

        protected override void InitializeBucketArray()
        {
            throw new NotImplementedException();
        }
    }
}
