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
using System.Runtime.Serialization;
using System.ComponentModel;
using System.Collections.Generic;
using System.Text;

using Ximura;

using Ximura.Data;
#endregion
namespace Ximura.Communication
{
    /// <summary>
    /// This interface specifies whether the entity can be loaded.
    /// </summary>
    public interface IXimuraMessageLoad
    {
        /// <summary>
        /// This boolean property that specifies whether the message can be loaded.
        /// </summary>
        bool CanLoad { get; }
        /// <summary>
        /// This boolean property specifies whether the entity has been loaded.
        /// </summary>
        bool Loaded { get; }
    }
}
