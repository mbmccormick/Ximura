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
    /// This is the completion behaviour of the security manager job.
    /// </summary>
    public enum SCMJobCompletionType
    {
        /// <summary>
        /// The job will complete when the job leaves the main calling method.
        /// </summary>
        OnExit,
        /// <summary>
        /// The job will complete when the SignalCompletion method is called.
        /// </summary>
        ManualSignal,
        /// <summary>
        /// This property specifies 
        /// </summary>
        SignalOrTimeout
    }
}
