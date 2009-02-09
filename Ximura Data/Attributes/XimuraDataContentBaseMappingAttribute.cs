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
    /// XimuraDataContentBaseMappingAttribute is the astract class for the DataContent 
    /// table mapping attributes.
    /// </summary>
    public abstract class XimuraDataContentBaseMappingAttribute : Attribute
    {
        #region Declarations

        /// <summary>
        /// The table.
        /// </summary>
        protected string mTable = null;
        /// <summary>
        /// The field
        /// </summary>
        protected string mField = null;

        #endregion
        #region Constructors
        /// <summary>
        /// This is the primary constructor.
        /// </summary>
        /// <param name="table">The ID table.</param>
        /// <param name="field">The ID field.</param>
        public XimuraDataContentBaseMappingAttribute(string table, string field)
        {
            mTable = table;
            mField = field;
        }
        #endregion

        #region Table
        /// <summary>
        /// This is the dataset table name.
        /// </summary>
        public string Table
        {
            get
            {
                return mTable;
            }
        }
        #endregion // Table
        #region Field
        /// <summary>
        /// This is the table field.
        /// </summary>
        public string Field
        {
            get
            {
                return mField;
            }
        }
        #endregion // Field
    }
}