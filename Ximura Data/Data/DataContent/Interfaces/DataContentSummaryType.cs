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
using System.ComponentModel;
using System.Collections;
using System.Runtime.Serialization;
using System.Data;

using Ximura;
#endregion // using
namespace Ximura.Data
{
    /// <summary>
    /// The data content summary type.
    /// </summary>
    public enum DataContentSummaryType
    {
        /// <summary>
        /// Summary should be in text format.
        /// </summary>
        Text,
        /// <summary>
        /// Sumary should be in HTML format.
        /// </summary>
        Html,
        /// <summary>
        /// Summary is custom.
        /// </summary>
        Custom
    }
}
