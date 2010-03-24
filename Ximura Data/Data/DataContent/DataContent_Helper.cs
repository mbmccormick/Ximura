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
using System.Text;

using Ximura;
using Ximura.Data;
using Ximura.Framework;

using CH = Ximura.Common;
using RH = Ximura.Reflection;
#endregion // using
namespace Ximura.Data
{
    public partial class DataContent
    {
        #region DumpTables
        /// <summary>
        /// This helper method dumps the tables and columns in the data set to a string.
        /// </summary>
        /// <returns></returns>
        public string DumpTables
        {
            get
            {
                StringBuilder sb = new StringBuilder();

                if (mDataContentSet.Tables.Count > 0)
                    RecurseTable(0, sb, mDataContentSet.Tables[0]);

                return sb.ToString();
            }
        }

        private void RecurseTable(int tab, StringBuilder sb, DataTable table)
        {
            sb.AppendLine(TabIt(tab) + table.TableName + "{" + ColumnDump(table) + "}");
            DataRelationCollection relCol = table.ChildRelations;

            foreach (DataRelation rel in relCol)
            {
                if (rel.ChildTable != null)
                    RecurseTable(tab+1, sb, rel.ChildTable);
            }
        }

        private string ColumnDump(DataTable table)
        {
            DataColumnCollection columns = table.Columns;
            string output = "";

            foreach (DataColumn column in columns)
            {
                output += column.Prefix + ":" + column.Caption + ", ";
            }

            return output.Length>0?output.Substring(0,output.Length-2):"";
        }

        private string TabIt(int tab)
        {
            return "--".PadLeft(tab * 3, ' ');
        }
        #endregion // DumpTables()

    }
}
