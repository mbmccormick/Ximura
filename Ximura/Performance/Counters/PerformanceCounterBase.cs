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

using Ximura;
using Ximura.Server;
#endregion // using
namespace Ximura.Performance
{
    /// <summary>
    /// This class is the default class for the performance counters.
    /// </summary>
    public abstract class PerformanceCounterBase: IXimuraPerformanceCounter 
    {
        #region Declarations
        private long mInternalValue = 0;
        private bool mActive = false;
        private bool mDirty = false;
        private string mName = "";
        private string mCategory = "";
        #endregion // Declarations
        #region PerformanceCounterBase
        /// <summary>
        /// This is the default constructor for the performance counter.
        /// </summary>
        public PerformanceCounterBase()
        {
        }
        #endregion // PerformanceCounterBase

        #region IXimuraPerformanceCounter Members
        public Guid AppID 
        {
            get
            {
                return Guid.Empty;
            }
        }

        public Guid CommandID
        {
            get
            {
                return Guid.Empty;
            }
        }

        public Guid PCID
        {
            get
            {
                return Guid.Empty;
            }
        }
        /// <summary>
        /// This property is the performance counter name.
        /// </summary>
        public string Name
        {
            get { return mName; }
        }
        /// <summary>
        /// This property is the performance counter category.
        /// </summary>
        public string Category
        {
            get { return mCategory; }
        }
        /// <summary>
        /// This is the performance counter type.
        /// </summary>
        public virtual PerformanceCounterType CatType
        {
            get { throw new NotImplementedException("The method or operation is not implemented."); }
        }
        /// <summary>
        /// This method increments the internal counter by 1.
        /// </summary>
        /// <returns></returns>
        public long Increment()
        {
            return ++mInternalValue;
        }
        /// <summary>
        /// This method increments or decrements the internal counter by the value specified.
        /// </summary>
        /// <param name="value">The positive or negative value.</param>
        /// <returns>Returns the new value of the counter.</returns>
        public long IncrementBy(long value)
        {
            mInternalValue += value;

            return mInternalValue;
        }
        /// <summary>
        /// This method decrements the internal counter by 1.
        /// </summary>
        /// <returns></returns>
        public long Decrement()
        {
            return --mInternalValue;
        }
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
                mInternalValue = value;
            }
        }

        /// <summary>
        /// This property identifies whether the counter is active.
        /// </summary>
        public bool Active
        {
            get
            {
                return mActive;
            }
            set
            {
                mActive = value;
            }
        }
        /// <summary>
        /// This property identifies whether the counter is dirty and needs updating.
        /// </summary>
        public bool Dirty
        {
            get { return mDirty; }
        }

        #endregion
    }
}
