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
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class XimuraDataContentSummaryAttribute : XimuraSchemaAttribute
    {
        #region Declarations
        private DataContentSummaryType msummaryType;
        private string mid = null;
        #endregion

        #region Constructors
        /// <summary>
        /// This is extended constructor that should be used when you want to 
        /// provide a new record.
        /// </summary>
        /// <param name="styleSheet">The schema name.</param>
        public XimuraDataContentSummaryAttribute(string styleSheet)
            :
            this(DataContentSummaryType.Text, styleSheet, null) { }
        /// <summary>
        /// This is extended constructor that should be used when you want to 
        /// provide a new record.
        /// </summary>
        /// <param name="summaryType">The schema name.</param>
        /// <param name="styleSheet">The name of the default content.</param>
        public XimuraDataContentSummaryAttribute(DataContentSummaryType summaryType,
            string styleSheet)
            : this(summaryType, styleSheet, null) { }
        /// <summary>
        /// This is extended constructor that should be used when you want to 
        /// provide a new record.
        /// </summary>
        /// <param name="summaryType">The schema name.</param>
        /// <param name="styleSheet">The name of the default content.</param>
        /// <param name="id">The id.</param>
        public XimuraDataContentSummaryAttribute(DataContentSummaryType summaryType,
            string styleSheet, string id)
            : base(styleSheet)
        {
            msummaryType = summaryType;
            mid = id;
        }
        #endregion

        /// <summary>
        /// This is the schema name for the DataContent object.
        /// </summary>
        public string Id
        {
            get { return mid; }
        }

        /// <summary>
        /// The new record content location.
        /// </summary>
        public DataContentSummaryType SummaryType
        {
            get { return msummaryType; }
        }
    }
}
