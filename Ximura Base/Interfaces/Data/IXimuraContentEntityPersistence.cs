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
    /// This interface implements the basic entity properties for persistence storage.
    /// </summary>
    public interface IXimuraContentEntityPersistence
    {
        /// <summary>
        /// This is the entity type used for searching.
        /// </summary>
        string EntityType { get;}
        /// <summary>
        /// This is the entity subtype used for searching
        /// </summary>
        string EntitySubtype { get;}
        /// <summary>
        /// This is the assembly qualified name used for seaching. This
        /// may differ from the actual name as it will not have the specific
        /// version number to enable consistency across multiple version.
        /// </summary>
        string EntityAQN { get;}
        /// <summary>
        /// This is the entity name used for search display.
        /// </summary>
        string EntityName { get;}
        /// <summary>
        /// This is the entity description used for search display.
        /// </summary>
        string EntityDescription { get;}
    }
}
