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
    /// The DataContentIDMappingAttribute is used to map the primary ID of the entity to the relevant field
    /// and table in the DataContent collection.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class XimuraDataContentIDMappingAttribute : XimuraDataContentBaseMappingAttribute
    {
        #region Declarations
        private bool mlistenEvents = true;

        #endregion
        #region Constructors
        /// <summary>
        /// This is the primary constructor.
        /// </summary>
        /// <param name="table">The ID primary table.</param>
        /// <param name="field">The ID field.</param>
        public XimuraDataContentIDMappingAttribute(string table, string field) : base(table, field) { }
        #endregion

        /// <summary>
        /// This property determines whether the DataContent should signal
        /// changes to this table.
        /// </summary>
        public bool listenEvents
        {
            get { return mlistenEvents; }
        }
    }
}
