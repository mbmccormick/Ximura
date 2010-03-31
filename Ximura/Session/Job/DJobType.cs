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
    /// This enumeration is the Dispatcher Job Types
    /// </summary>
    public enum DJobType
    {
        /// <summary>
        /// The job type is a command.
        /// </summary>
        Command,
        /// <summary>
        /// The job type is a straight call back.
        /// </summary>
        Callback
    }
}
