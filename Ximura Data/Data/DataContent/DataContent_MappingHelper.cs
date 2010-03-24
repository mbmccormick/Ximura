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
using System.Data;

using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Runtime.Serialization;
using System.Diagnostics;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.Xml.XPath;
using System.Xml.Xsl;
using System.Reflection;

using Ximura;
using Ximura.Data;
using Ximura.Framework;
using Ximura.Helper;
using CH = Ximura.Helper.Common;
using RH = Ximura.Helper.Reflection;
#endregion // using
namespace Ximura.Data
{
    public partial class DataContent
    {
        #region MappingHelperObjectGet
        /// <summary>
        /// This method returns the field as an object.
        /// </summary>
        /// <param name="table">The table name.</param>
        /// <param name="field">The field.</param>
        /// <returns>Returns the object.</returns>
        protected object MappingHelperObjectGet(string table, string field)
        {
            return MappingHelperObjectGet(table, field, null);
        }
        /// <summary>
        /// This method returns the field as an object.
        /// </summary>
        /// <param name="table">The table name.</param>
        /// <param name="field">The field.</param>
        /// <param name="DBNullValue">The value to return if the field cannot be found.</param>
        /// <returns>Returns the object or the null value if this cannot be found.</returns>
        protected object MappingHelperObjectGet(string table, string field, object DBNullValue)
        {
            try
            {
                DataTable theTable = mDataContentSet.Tables[table];

                return MappingHelperObjectGet(theTable, field, DBNullValue);
            }
            catch
            {
                return DBNullValue;
            }
        }
        /// <summary>
        /// This method returns the field as an object.
        /// </summary>
        /// <param name="theTable">The table object.</param>
        /// <param name="field">The field.</param>
        /// <returns>Returns the object.</returns>
        protected object MappingHelperObjectGet(DataTable theTable, string field)
        {
            return MappingHelperObjectGet(theTable, field, null);
        }
        /// <summary>
        /// This method returns the field as an object.
        /// </summary>
        /// <param name="theTable">The table object.</param>
        /// <param name="field">The field.</param>
        /// <param name="DBNullValue">The value to return if the field cannot be found.</param>
        /// <returns>Returns the object or the null value if this cannot be found.</returns>
        protected object MappingHelperObjectGet(DataTable theTable, string field, object DBNullValue)
        {
            try
            {
                DataRow theRow = theTable.Rows[0];

                return MappingHelperObjectGet(theRow, field, DBNullValue);
            }
            catch
            {
                return DBNullValue;
            }
        }
        /// <summary>
        /// This method returns the field as an object.
        /// </summary>
        /// <param name="theRow">The row object.</param>
        /// <param name="field">The field.</param>
        /// <returns>Returns the object.</returns>
        protected object MappingHelperObjectGet(DataRow theRow, string field)
        {
            return MappingHelperObjectGet(theRow, field, null);
        }
        /// <summary>
        /// This method returns the field as an object.
        /// </summary>
        /// <param name="theRow">The row object.</param>
        /// <param name="field">The field.</param>
        /// <param name="DBNullValue">The value to return if the field cannot be found.</param>
        /// <returns>Returns the object or the null value if this cannot be found.</returns>
        protected object MappingHelperObjectGet(DataRow theRow, string field, object DBNullValue)
        {
            try
            {
                if (!theRow.Table.Columns.Contains(field))
                    return DBNullValue;

                object item = theRow[field];

                return item == System.DBNull.Value ? DBNullValue : item;
            }
            catch
            {
                return DBNullValue;
            }
        }
        #endregion

        #region MappingHelperObjectSet
        /// <summary>
        /// This method sets the dataset field with the object.
        /// </summary>
        /// <param name="table">The table name.</param>
        /// <param name="field">The field.</param>
        /// <param name="value">The value to set.</param>
        protected void MappingHelperObjectSet(string table, string field, object value)
        {
            DataTable theTable = mDataContentSet.Tables[table];

            MappingHelperObjectSet(theTable, field, value);
        }

        /// <summary>
        /// This method sets the dataset field with the object.
        /// </summary>
        /// <param name="theTable">The DataTable object.</param>
        /// <param name="field">The field.</param>
        /// <param name="value">The value to set.</param>
        protected void MappingHelperObjectSet(DataTable theTable, string field, object value)
        {
            DataRow theRow = theTable.Rows[0];

            MappingHelperObjectSet(theRow, field, value);
        }

        /// <summary>
        /// This method sets the dataset field with the object.
        /// </summary>
        /// <param name="theRow">The row object.</param>
        /// <param name="field">The field.</param>
        /// <param name="value">The value to set.</param>
        protected void MappingHelperObjectSet(DataRow theRow, string field, object value)
        {
            theRow[field] = value;
        }
        #endregion

        #region MappingHelperGuidGet
        /// <summary>
        /// This protected method gets a Guid grom a dataset record.
        /// </summary>
        /// <param name="table">The table.</param>
        /// <param name="field">The field</param>
        /// <returns>The guid value or Guid.Empty if not present.</returns>
        protected Guid MappingHelperGuidGet(string table, string field)
        {
            object item = MappingHelperObjectGet(table, field);

            if (item == null || !(item is Guid))
                return Guid.Empty;

            return (Guid)item;
        }
        #endregion

        #region MappingHelperGuidSet
        /// <summary>
        /// This protected method sets a Guid in to a dataset record.
        /// </summary>
        /// <param name="table">The table.</param>
        /// <param name="field">The field</param>
        /// <param name="value">The guid value.</param>
        protected void MappingHelperGuidSet(string table, string field, Guid value)
        {
            MappingHelperObjectSet(table, field, value);
        }
        #endregion

        #region MappingHelperGet
        /// <summary>
        /// This method returns the Guid value specified in the attribute.
        /// </summary>
        /// <param name="attr">The attribute.</param>
        /// <returns>The Guid valu, or</returns>
        protected Guid MappingHelperGet(XimuraDataContentBaseMappingAttribute attr)
        {
            try
            {
                if (mDataContentSet == null)
                    return Guid.Empty;

                DataTable theTable = mDataContentSet.Tables[attr.Table];

                if (theTable == null || theTable.Rows.Count == 0)
                    return Guid.Empty;

                string id = (string)theTable.Rows[0][attr.Field];

                if (id == null || id == String.Empty)
                    return Guid.Empty;

                return new Guid(id);
            }
            catch
            {
                return Guid.Empty;
            }
        }
        #endregion

        #region MappingHelperSet
        /// <summary>
        /// This method is used to set the selected property within the dataset.
        /// </summary>
        /// <param name="attr">The attribute containing the db parameters</param>
        /// <param name="value">The GUID value to set.</param>
        protected void MappingHelperSet(XimuraDataContentBaseMappingAttribute attr, Guid value)
        {
            try
            {
                DataTable theTable = mDataContentSet.Tables[attr.Table];

                if (theTable == null)
                    return;

                if (theTable.Rows.Count == 0) return;
                DataRow theRow = theTable.Rows[0];

                theRow[attr.Field] = value;
            }
            catch (Exception ex)
            {
                //XimuraAppTrace.WriteLine(ex.Message);
            }
        }
        #endregion

        #region MappingHelperGet<T>
        #region MappingHelperGet<T>(string table, string field)
        /// <summary>
        /// This method returns the field as an object.
        /// </summary>
        /// <param name="table">The table name.</param>
        /// <param name="field">The field.</param>
        /// <returns>Returns the object.</returns>
        protected T MappingHelperGet<T>(string table, string field)
        {
            return MappingHelperGet<T>(table, field, default(T));
        }
        #endregion // MappingHelperGet<T>(string table, string field)
        #region MappingHelperGet<T>(string table, string field, T DBNullValue)
        /// <summary>
        /// This method returns the field as an object.
        /// </summary>
        /// <param name="table">The table name.</param>
        /// <param name="field">The field.</param>
        /// <param name="DBNullValue">The value to return if the field cannot be found.</param>
        /// <returns>Returns the object or the null value if this cannot be found.</returns>
        protected T MappingHelperGet<T>(string table, string field, T DBNullValue)
        {
            try
            {
                DataTable theTable = mDataContentSet.Tables[table];

                return MappingHelperGet<T>(theTable, field, DBNullValue);
            }
            catch
            {
                return DBNullValue;
            }
        }
        #endregion // MappingHelperGet<T>(string table, string field, T DBNullValue)
        #region MappingHelperGet<T>(DataTable theTable, string field)
        /// <summary>
        /// This method returns the field as an object.
        /// </summary>
        /// <param name="theTable">The table object.</param>
        /// <param name="field">The field.</param>
        /// <returns>Returns the object.</returns>
        protected T MappingHelperGet<T>(DataTable theTable, string field)
        {
            return MappingHelperGet<T>(theTable, field, default(T));
        }
        #endregion // MappingHelperGet<T>(DataTable theTable, string field)
        #region MappingHelperGet<T>(DataTable theTable, string field, T DBNullValue)
        /// <summary>
        /// This method returns the field as an object.
        /// </summary>
        /// <param name="theTable">The table object.</param>
        /// <param name="field">The field.</param>
        /// <param name="DBNullValue">The value to return if the field cannot be found.</param>
        /// <returns>Returns the object or the null value if this cannot be found.</returns>
        protected T MappingHelperGet<T>(DataTable theTable, string field, T DBNullValue)
        {
            try
            {
                DataRow theRow = theTable.Rows[0];

                return MappingHelperGet<T>(theRow, field, DBNullValue);
            }
            catch
            {
                return DBNullValue;
            }
        }
        #endregion // MappingHelperGet<T>(DataTable theTable, string field, T DBNullValue)
        #region MappingHelperGet<T>(DataRow theRow, string field)
        /// <summary>
        /// This method returns the field as an object.
        /// </summary>
        /// <param name="theRow">The row object.</param>
        /// <param name="field">The field.</param>
        /// <returns>Returns the object.</returns>
        protected T MappingHelperGet<T>(DataRow theRow, string field)
        {
            return MappingHelperGet<T>(theRow, field, default(T));
        }
        #endregion // MappingHelperGet<T>(DataRow theRow, string field)
        #region MappingHelperGet<T>(DataRow theRow, string field, T DBNullValue)
        /// <summary>
        /// This method returns the field as an object.
        /// </summary>
        /// <param name="theRow">The row object.</param>
        /// <param name="field">The field.</param>
        /// <param name="DBNullValue">The value to return if the field cannot be found.</param>
        /// <returns>Returns the object or the null value if this cannot be found.</returns>
        protected T MappingHelperGet<T>(DataRow theRow, string field, T DBNullValue)
        {
            try
            {
                if (!theRow.Table.Columns.Contains(field))
                    return DBNullValue;

                T item = (T)theRow[field];
                
                return item.Equals(default(T)) ? DBNullValue : item;
            }
            catch
            {
                return DBNullValue;
            }
        }
        #endregion // MappingHelperGet<T>(DataRow theRow, string field, T DBNullValue)
        #endregion

    }
}
