﻿#region Copyright
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
﻿#region using
using System;
using System.IO;
using System.Runtime.Serialization;
using System.ComponentModel;
using System.Collections;
using System.Text;

using Ximura;
using Ximura.Helper;
using Ximura.Data;
using CH = Ximura.Helper.Common;
#endregion
namespace Ximura.Data
{
    /// <summary>
    /// This enumeration determines the termination style for the fragment.
    /// </summary>
    public enum FragmentTerminationType
    {
        /// <summary>
        /// The fragment has a fixed number of bytes.
        /// </summary>
        ByteLength,
        /// <summary>
        /// The fragment will terminate when the byte array end matches the termination array.
        /// </summary>
        Terminator,
        /// <summary>
        /// The fragment will terminate when the byte array end matches the delimiter structure.
        /// </summary>
        Custom
    }
}