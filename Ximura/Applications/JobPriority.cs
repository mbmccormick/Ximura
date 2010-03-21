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
#endregion // using
namespace Ximura
{
    /// <summary>
    /// Thread pool priority is used to assign the job position within the 
    /// Priority Job queue.
    /// </summary>
    [Serializable()]
    public enum JobPriority : int
    {
        /// <summary>
        /// This is the highest priority
        /// </summary>
        Realtime = 3,
        /// <summary>
        /// This is high priority
        /// </summary>
        High = 2,
        /// <summary>
        /// This is above normal priority
        /// </summary>
        AboveNormal = 1,
        /// <summary>
        /// This is the default priority
        /// </summary>
        Normal = 0,
        /// <summary>
        /// This is below normal priority
        /// </summary>
        BelowNormal = -1,
        /// <summary>
        /// This is the lowest priority
        /// </summary>
        Low = -2,
        /// <summary>
        /// This is the not set state.
        /// </summary>
        NotSet = int.MinValue
    }
}
