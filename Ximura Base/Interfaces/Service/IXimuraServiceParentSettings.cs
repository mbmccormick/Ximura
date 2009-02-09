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
using System.Threading;

using Ximura;
#endregion // using
namespace Ximura
{
    /// <summary>
    /// This interface is implemented by commands that can be nested in other commands.
    /// </summary>
    public interface IXimuraServiceParentSettings
    {
        /// <summary>
        /// The parent command service name.
        /// </summary>
        string ParentCommandName { get;set;}
    }
}