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
using System.Drawing;

using Ximura;

using Ximura.Framework;
#endregion //
namespace Ximura.Framework
{
    /// <summary>
    /// This interface should be implemented by components that can be hosted as a state in a finite 
    /// state machine.
    /// </summary>
    public interface IXimuraFSMState : IXimuraFSMStateIdentifier, IComponent
    {

    }


}
