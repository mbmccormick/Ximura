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
﻿#region using
using System;
using System.Data;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Xml;

using Ximura;
using Ximura.Data;

using CH = Ximura.Common;
#endregion // using
namespace Ximura.Data
{
    /// <summary>
    /// This enumeration defines the type of text to be written.
    /// </summary>
    public enum AtomTextType
    {
        /// <summary>
        /// Plain text, xml encoded.
        /// </summary>
        Text,
        /// <summary>
        /// HTML, xml encoded.
        /// </summary>
        Html,
        /// <summary>
        /// Well formed XHTML.
        /// </summary>
        Xhtml
    }
}
