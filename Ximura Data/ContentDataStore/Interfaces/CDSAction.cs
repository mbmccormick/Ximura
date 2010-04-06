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
namespace Ximura.Data
{
    /// <summary>
    /// This enum contains a list of actions that each CDSState can execute. This enumeration is used to build up
    /// an execution plan.
    /// </summary>
    public enum CDSAction : short
    {
        /// <summary>
        /// This is the default value when the specific action has not been set.
        /// </summary>
        NotSet,

        /// <summary>
        /// Construct creates a new entity and returns it to the calling party without saving it to the persistence store..
        /// </summary>
        Construct,

        /// <summary>
        /// The Persistence agent supports the create action.
        /// </summary>
        Create,
        /// <summary>
        /// The Persistence agent supports the read action.
        /// </summary>
        Read,
        /// <summary>
        /// The Persistence agent supports the updating of entities.
        /// </summary>
        Update,
        /// <summary>
        /// The Persistence agent supports the deletion of entities.
        /// </summary>
        Delete,
        /// <summary>
        /// The Persistence agent supports restore for previously deleted entities.
        /// </summary>
        Restore,

        /// <summary>
        /// The Persistence agent supports the version checking of entities.
        /// </summary>
        VersionCheck,
        /// <summary>
        /// The Persistence agent will resolve references for entities.
        /// </summary>
        ResolveReference,

        /// <summary>
        /// The Persistence agent supports caching.
        /// </summary>
        Cache,

        /// <summary>
        /// The Persistence agent supports browsing and paginationfor entity collection.
        /// </summary>
        Browse,

        /// <summary>
        /// The Persistence agent supports browsing and pagination for entity version histories.
        /// </summary>
        History,

        /// <summary>
        /// The Persistence agent supports publishing for entities. This is reserved for future use.
        /// </summary>
        Publish,
        /// <summary>
        /// The persistence agent support the readction of a published entity. This is reserved for future use.
        /// </summary>
        Redact,

        /// <summary>
        /// The persistence agent provides exclusive access to an entity for someone providing a particular access key.
        /// </summary>
        Lock,
        /// <summary>
        /// The persistence agent releases the lock on a particular entity.
        /// </summary>
        Unlock,

        /// <summary>
        /// The Persistence agent supports custom actions. This is reserved for future use.
        /// </summary>
        Custom
    }
}