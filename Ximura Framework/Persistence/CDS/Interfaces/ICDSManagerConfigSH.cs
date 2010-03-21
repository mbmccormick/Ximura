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
using System.Collections;
using System.Xml;

using Ximura.Server;
using Ximura.Command;
#endregion // using
namespace Ximura.Persistence
{
    /// <summary>
    /// This interface is used to register configuration services to protocols within a
    /// ProtocolServerCommand container.
    /// </summary>
    public interface ICDSPersistenceManagerConfigSH : IXimuraConfigSH
    {
        /// <summary>
        /// Access Right
        /// </summary>
        /// <param name="Type">access type</param>
        /// <returns>access right</returns>
        bool AccessRight(string Type);
        /// <summary>
        /// Resolve Connection Mapping for specific Catalog
        /// </summary>
        /// <param name="Catalog">catalog Name</param>
        /// <returns>connection mapping string</returns>
        string ResolveConnectionMapping(string Catalog);
    }
}