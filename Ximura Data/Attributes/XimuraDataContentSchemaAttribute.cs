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
    /// The DataContentSchema attribute is used to specify a data schema for 
    /// design time integration.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class XimuraDataContentSchemaAttribute : XimuraDataContentSchemaReferenceAttribute
    {
        #region Constructors
        /// <summary>
        /// This is extended constructor that should be used when you want to 
        /// provide a new record.
        /// </summary>
        /// <param name="schemaName">The schema name.</param>
        /// <param name="dataName">The name of the default content.</param>
        public XimuraDataContentSchemaAttribute(string uriPath,
            string resPath)
            : base(uriPath, resPath)
        {
        }
        #endregion


    }
}
