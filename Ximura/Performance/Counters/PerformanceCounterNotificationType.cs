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
    /// This method is used to determine the notification logic that the counter wishes to use.
    /// </summary>
    public enum PerformanceCounterNotificationType
    {
        /// <summary>
        /// The counter will fire an event when the value changes. Generally this should only
        /// be used for counters that change infrequently.
        /// </summary>
        EventDriven,
        /// <summary>
        /// The timer poll method should be used for counters that need to be updated at a regular
        /// interval.
        /// </summary>
        TimerPoll
    }
}
