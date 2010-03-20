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
    [Flags]
    public enum MatchTerminatorStatus
    {
        NotSet = 0,

        Fail = 1,
        FailNoLength = 9,

        Success = 2,
        SuccessReset = 34,
        SuccessNoLength = 10,
        SuccessNoLengthReset = 42,

        SuccessPartial = 4,
        NoLength = 8,
        Exception = 16,
        Reset = 32
    }
}
