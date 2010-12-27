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
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Text;
#endregion
namespace Ximura
{
    /// <summary>
    /// 
    /// </summary>
    [Flags]
    public enum MatchTerminatorStatus
    {
        /// <summary>
        /// 
        /// </summary>
        NotSet = 0,
        /// <summary>
        /// 
        /// </summary>
        Fail = 1,
        /// <summary>
        /// 
        /// </summary>
        FailNoLength = 9,
        /// <summary>
        /// 
        /// </summary>
        Success = 2,
        /// <summary>
        /// 
        /// </summary>
        SuccessReset = 34,
        /// <summary>
        /// 
        /// </summary>
        SuccessNoLength = 10,
        /// <summary>
        /// 
        /// </summary>
        SuccessNoLengthReset = 42,
        /// <summary>
        /// 
        /// </summary>
        SuccessPartial = 4,
        /// <summary>
        /// 
        /// </summary>
        NoLength = 8,
        /// <summary>
        /// 
        /// </summary>
        Exception = 16,
        /// <summary>
        /// 
        /// </summary>
        Reset = 32
    }
}
