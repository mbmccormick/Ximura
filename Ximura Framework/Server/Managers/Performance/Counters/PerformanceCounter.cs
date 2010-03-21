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
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;

using Ximura;
using Ximura.Server;
#endregion // using
namespace Ximura
{
    /// <summary>
    /// This class is the default class for the performance counters.
    /// </summary>
    public class PerformanceCounter: PerformanceBase, IXimuraPerformanceCounter 
    {
        #region Declarations
        /// <summary>
        /// The internal value.
        /// </summary>
        protected long mInternalValue = 0;
        /// <summary>
        /// The performance counter type.
        /// </summary>
        protected PerformanceCounterType? mCatType = null;
        #endregion // Declarations
        #region PerformanceCounterBase
        /// <summary>
        /// This is the default constructor for the performance counter.
        /// </summary>
        public PerformanceCounter()
        {
        }
        #endregion // PerformanceCounterBase

        #region CounterType
        /// <summary>
        /// This is the performance counter type.
        /// </summary>
        public virtual PerformanceCounterType CounterType
        {
            get 
            { 
                throw new NotImplementedException("The method or operation is not implemented."); 
            }
        }
        #endregion // CounterType

        #region Increment()
        /// <summary>
        /// This method increments the internal counter by 1.
        /// </summary>
        /// <returns></returns>
        public long Increment()
        {
            return Interlocked.Increment(ref mInternalValue);
        }
        #endregion // Increment()
        #region IncrementBy(long value)
        /// <summary>
        /// This method increments or decrements the internal counter by the value specified.
        /// </summary>
        /// <param name="value">The positive or negative value.</param>
        /// <returns>Returns the new value of the counter.</returns>
        public long IncrementBy(long value)
        {
            return Interlocked.Add(ref mInternalValue, value);
        }
        #endregion // IncrementBy(long value)
        #region Decrement()
        /// <summary>
        /// This method decrements the internal counter by 1.
        /// </summary>
        /// <returns></returns>
        public long Decrement()
        {
            return Interlocked.Decrement(ref mInternalValue);
        }
        #endregion // Decrement()
        #region RawValue
        /// <summary>
        /// This is the performance counter raw value.
        /// </summary>
        public long RawValue
        {
            get
            {
                return mInternalValue;
            }
            set
            {
                Interlocked.Exchange(ref mInternalValue, value);
            }
        }
        #endregion // RawValue

        #region Active
        /// <summary>
        /// This property identifies whether the counter is active.
        /// </summary>
        public virtual bool Active
        {
            get;
            set;
        }
        #endregion // Active
        #region Dirty
        /// <summary>
        /// This property identifies whether the counter is dirty and needs updating.
        /// </summary>
        public bool Dirty
        {
            get;
            protected set;
        }
        #endregion // Dirty
    }
}
