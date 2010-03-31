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
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using Ximura;
#endregion
namespace Ximura
{
    /// <summary>
    /// This interface is used by performance counter classes.
    /// </summary>
    public interface IXimuraPerformanceCounter: IXimuraPerformance
    {
        /// <summary>
        /// This is the system counter type.
        /// </summary>
        PerformanceCounterType CounterType { get;}

        /// <summary>
        /// This method increments the counter by 1.
        /// </summary>
        /// <returns></returns>
        long Increment();
        /// <summary>
        /// This method increments or decrements the counter by the value specified.
        /// </summary>
        /// <param name="value"></param>
        /// <returns>Returns the new value of the counter.</returns>
        long IncrementBy(long value);
        /// <summary>
        /// This method decrements the counter by 1.
        /// </summary>
        /// <returns>Returns the new value of the counter.</returns>
        long Decrement();
        /// <summary>
        /// This property gets or sets the raw value of the counter directly.
        /// </summary>
        long RawValue { get; set; }
        /// <summary>
        /// This property indicates whether the counter is active.
        /// </summary>
        bool Active { get;set; }
        /// <summary>
        /// This readonly property determines whether the counter has been read since it was last changed.
        /// </summary>
        bool Dirty { get;}

    }
}
