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
using Ximura.Collections;
using Ximura.Collections.Data;
#endregion // using
namespace Ximura.Collections
{
    /// <summary>
    /// This vertex array implements the data as a skip list array.
    /// </summary>
    /// <typeparam name="T">The collection type.</typeparam>
    public class SkipListClassBasedVertexArray<T> : MultiLevelClassBasedVertexArray<T>
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
        public SkipListClassBasedVertexArray()
        {
        }
        #endregion // Constructor

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
        #region InitializeData()
        /// <summary>
        /// This override sets the probability to 50%.
        /// </summary>
        protected override void InitializeData()
        {
            mProbability = 0.5;
            base.InitializeData();
        }
        #endregion // InitializeData()

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

        #region LevelMax
        /// <summary>
        /// This is the maximum levels implemented by the skip list.
        /// </summary>
        public override int LevelMax{get{return 16;}}
        #endregion // LevelMax

        #region GetSentinelID(int hashCode, bool createSentinel, out int hashID)
        /// <summary>
        /// This method is responsible for implementing the sentinel collection for fast lookup of data.
        /// </summary>
        /// <param name="hashCode"></param>
        /// <param name="createSentinel"></param>
        /// <param name="hashID"></param>
        /// <returns></returns>
        protected override CollectionVertexClass<T> GetSentinelID(int hashCode, bool createSentinel, out int hashID)
        {
            try
            {
                hashCode &= cnLowerBitMask;
                hashID = BitReverse(hashCode);

                CollectionVertexClass<T>[] path = null;

                //OK, create a path to the root sentinel.
                if (createSentinel)
                    path = new CollectionVertexClass<T>[LevelCurrent + 1];

                int index = 0;
                CollectionVertexClass<T> currSentinel = mData[LevelCurrent - 1];
                if (createSentinel)
                    path[0] = currSentinel;

                currSentinel.LockWait();

                while (true)
                {
                    index++;

                    while (!currSentinel.IsTerminator && currSentinel.HashID < hashID)
                    {
                        if (currSentinel.Next.HashID > hashID)
                            break;

                        currSentinel = currSentinel.Next;
                        currSentinel.LockWait();
                    }

                    if (createSentinel)
                        path[index] = currSentinel;

                    if (currSentinel.Down == null)
                        break;

                    currSentinel = currSentinel.Down;
                    currSentinel.LockWait();
                }

                if (createSentinel)
                {
                    //First, do we need to insert a sentinel in the root data?
                    if (currSentinel.HashID < hashID)
                    {
                        ClassBasedVertexWindow<T> vWin = new ClassBasedVertexWindow<T>(this, currSentinel, mEqComparer, hashID, default(T));

                        //Scan for the correct position to insert the sentinel.
                        vWin.ScanAndLock();

                        //Insert the new data sentinel.
                        path[index] = vWin.InsertDataSentinel();

                        //Unlock the window data.
                        vWin.Unlock();
                    }


                    //OK, we will consecutively add the item to the levels above based on the probability function.
                    for (index--; ConvertProbabilityToBool() && index > 0; index--)
                    {
                        ClassBasedVertexWindow<T> vWin = new ClassBasedVertexWindow<T>(this, path[index], mEqComparer, hashID, default(T));

                        //Scan for the correct position to insert the sentinel.
                        vWin.ScanAndLock();

                        //Insert the new data sentinel.
                        path[index] = vWin.InsertSentinel(path[index + 1]);

                        //Unlock the window data.
                        vWin.Unlock();
                    }
                }

                return currSentinel;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Oh fuck!");
                throw ex;
            }
        }
        #endregion // GetSentinelID(int hashCode, bool createSentinel, out int hashID)

        #region FastContains
        /// <summary>
        /// This class supports a fast search algorithm
        /// </summary>
        public override bool SupportsFastContain{get{return true;}}
        /// <summary>
        /// This method implements a fast search algoritm.
        /// </summary>
        /// <param name="item">The item to search for.</param>
        /// <returns>Returns true if found, false if not found, and null if the search encountered modified data.</returns>
        public override bool? FastContains(T item)
        {
            //Is this a null or default value?
            if (mEqComparer.Equals(item, default(T)))
                return DefaultTContains();

            int currVersion = mVersion;

            //Get the initial sentinel vertex. No need to check locks as sentinels rarely change.
            int hashID;
            CollectionVertexClass<T> scanVertex = GetSentinelID(mEqComparer.GetHashCode(item), false, out hashID);

            //First we will attempt to search without locking. However, should the version ID change 
            //during the search we will need to complete a locked search to ensure consistency.
            while (mVersion == currVersion)
            {
                //Do we have a match?
                if (!scanVertex.IsSentinel &&
                    scanVertex.HashID == hashID &&
                    mEqComparer.Equals(item, scanVertex.Value))
                    return true;

                //Is this the end of the line
                if (scanVertex.IsTerminator || scanVertex.HashID > hashID)
                    return false;

                scanVertex = scanVertex.Next;
            }

            ContainScanUnlockedMiss();
            return null;
        }
        #endregion // FastContains

    }
}
