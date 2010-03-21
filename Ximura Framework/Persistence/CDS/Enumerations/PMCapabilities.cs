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
namespace Ximura.Persistence
{
    /// <summary>
    /// This enum contains a list of actions that each CDSState can execute. This enumeration is used to build up
    /// an execution plan.
    /// </summary>
    public enum CDSStateAction: short
    {
        /// <summary>
        /// The Persistence Manager supports the create action.
        /// </summary>
        Create,
        /// <summary>
        /// The Persistence Manager supports the read action.
        /// </summary>
        Read,
        /// <summary>
        /// The Persistence Manager supports the updating of entities.
        /// </summary>
        Update,
        /// <summary>
        /// The Persistence Manager supports the deletion of entities.
        /// </summary>
        Delete,
        /// <summary>
        /// The Persistence Manager supports the version checking of entities.
        /// </summary>
        VersionCheck,
        /// <summary>
        /// The Persistence Manager will resolve references for entities.
        /// </summary>
        ResolveReference,
        /// <summary>
        /// The Persistence Manager supports caching.
        /// </summary>
        Cache,
        /// <summary>
        /// The Persistence Manager supports browsing for entity collection.
        /// </summary>
        Browse,
        /// <summary>
        /// The Persistence Manager supports publishing for entities. This is reserved for future use.
        /// </summary>
        Publish,
        /// <summary>
        /// The Persistence Manager supports restore for previously deleted entities.
        /// </summary>
        Restore,
        /// <summary>
        /// The Persistence Manager supports custom actions. This is reserved for future use.
        /// </summary>
        Custom
    }
}