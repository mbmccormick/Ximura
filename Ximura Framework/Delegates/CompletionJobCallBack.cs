﻿#region Copyright
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
using System.Collections;
using System.Configuration;
#endregion // using
namespace Ximura.Framework
{
    /// <summary>
    /// This delegate is used to signal the completion of a set of jobs.
    /// </summary>
    public delegate void CompletionJobCallBack(SecurityManagerJob parentJob,
        CompletionJobStatus status, object state);
}