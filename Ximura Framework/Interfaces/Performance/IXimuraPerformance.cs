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
using Ximura.Framework;
#endregion // using
namespace Ximura
{
    /// <summary>
    /// THis is the base interface for the performance architecture.
    /// </summary>
    public interface IXimuraPerformance
    {
        /// <summary>
        /// The unique performance counter ID.
        /// </summary>
        Guid PCID { get; set; }

        /// <summary>
        /// This is the application ID for the counter.
        /// </summary>
        Guid AppID { get; set; }
        /// <summary>
        /// The container id.
        /// </summary>
        Guid ID { get; set; }

        /// <summary>
        /// The friendly name.
        /// </summary>
        string Name { get; set; }
        /// <summary>
        /// The category.
        /// </summary>
        string Category { get; set; }
    }
}
