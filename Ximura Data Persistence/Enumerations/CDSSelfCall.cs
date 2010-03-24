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
#endregion // using
namespace Ximura.Data
{
    /// <summary>
    /// This enumeration identifies the type of self call command for the CDS to process.
    /// </summary>
    public enum CDSSelfCall
    {
        /// <summary>
        /// The value has not been set, this will return an error.
        /// </summary>
        NotSet,
        /// <summary>
        /// The enumeration is a cache initialization command
        /// </summary>
        CacheInitialize,
        /// <summary>
        /// The enumeration is a cache update command
        /// </summary>
        CacheUpdate
    }
}
