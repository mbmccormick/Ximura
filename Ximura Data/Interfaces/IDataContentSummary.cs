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
    /// Entities that implement this interface provide a summary of the content
    /// </summary>
    public interface IDataContentSummary
    {
        /// <summary>
        /// This method retrieves a summary based on the summary type.
        /// </summary>
        /// <param name="type">The summary type.</param>
        /// <returns>An object containing the summary.</returns>
        object GetSummary(DataContentSummaryType type);
        /// <summary>
        /// This method retrieves a summary based on the summary type.
        /// </summary>
        /// <param name="type">The summary type.</param>
        /// <param name="id">An identifier.</param>
        /// <returns>An object containing the summary.</returns>
        object GetSummary(DataContentSummaryType type, string id);
    }
}
