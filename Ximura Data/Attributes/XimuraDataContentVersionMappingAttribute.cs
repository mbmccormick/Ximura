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
using System.Reflection;
using System.IO;
#endregion // using
namespace Ximura.Data
{
    /// <summary>
    /// The XimuraDataContentVersionMappingAttribute is used to map the version ID of the entity 
    /// to the relevant field and table in the DataContent collection.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class XimuraDataContentVersionMappingAttribute : XimuraDataContentBaseMappingAttribute
    {
        #region Constructors
        /// <summary>
        /// This is the primary constructor.
        /// </summary>
        /// <param name="table">The Version ID primary table.</param>
        /// <param name="field">The Version ID field.</param>
        public XimuraDataContentVersionMappingAttribute(string table, string field)
            : base(table, field) { }
        #endregion
    }
}
