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
using System.IO;
using System.Diagnostics;
using System.ComponentModel;
using System.ComponentModel.Design;

#endregion //
namespace Ximura
{
    /// <summary>
    /// This is the default state property
    /// </summary>
    public interface IXimuraFSMStateIdentifier
    {
        /// <summary>
        /// This is the state identifier string.
        /// </summary>
        string Identifier { get; set; }
    }
}
