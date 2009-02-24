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
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using Microsoft.SqlServer.Server;
#endregion // using
namespace Ximura.Persistence.SQL
{

    public partial class EntityAccess
    {
        private static readonly string SQLBrowseBody = @" [Entity_Sequence_ID],[Entity_Version_ID],[Entity_Content_ID],[Entity_Type_ID],[Assembly_Qualified_Name],[Create_Date],[Built],[Expiry_Date_Time],[Create_By],[Create_By_Reference_ID_Code],[Status],[Update_Date] FROM [Entity]";

        [Microsoft.SqlServer.Server.SqlProcedure]
        public static SqlInt32 EntityBrowse(
            [SqlFacet(IsNullable = false)]SqlInt32 RecordCount
          , [SqlFacet(IsNullable = true)]SqlInt64 SequenceGreaterThanID
          , [SqlFacet(IsNullable = true)]SqlGuid TID
          , [SqlFacet(IsNullable = true)]SqlGuid CID
          , out SqlInt32 Status
          , [SqlFacet(MaxSize = 100)]out SqlString SubStatus
            )
        {
            try
            {
                using (SqlConnection cn = new SqlConnection("context connection=true"))
                {
                    StringBuilder sb = new StringBuilder();
                    bool whereOK = false;

                    sb.Append("SELECT TOP ");
                    sb.Append(RecordCount.Value);
                    sb.Append(SQLBrowseBody);

                    if (!SequenceGreaterThanID.IsNull)
                        AppendFilter(sb, ref whereOK, 
                            @"[Entity_Sequence_ID]>" + SequenceGreaterThanID.Value.ToString());

                    if (!TID.IsNull)
                        AppendFilter(sb, ref whereOK,
                            @"[Entity_Type_ID]='" + TID.Value.ToString() + "'");

                    if (!CID.IsNull)
                        AppendFilter(sb, ref whereOK,
                            @"[Entity_Content_ID]='" + CID.Value.ToString() + "'");

                    sb.Append(" ORDER BY [Entity_Sequence_ID]");

                    cn.Open();
                    SqlCommand sqlCmd = new SqlCommand();
                    sqlCmd.Connection = cn;
                    sqlCmd.CommandType = CommandType.Text;
                    sqlCmd.CommandText = sb.ToString();
                    bool hasData = false;
                    using (SqlDataReader dr = sqlCmd.ExecuteReader())
                    {
                        hasData = dr.HasRows;
                        SqlContext.Pipe.Send(dr);
                    }

                    if (hasData)
                    {
                        Status = 404;
                        SubStatus = "Not Found";
                    }
                    else
                    {
                        Status = 200;
                        SubStatus = "OK";
                    }
                }
            }
            catch (Exception ex)
            {
                LogError("500", "EntityBrowse --> " + ex.Message);
                Status = 500;
                string message = ex.Message;
                SubStatus = string.Format("Exception: {0}", message.Length > 85 ? message.Substring(0, 85) : message);
            }

            return Status;
        }

        private static void AppendFilter(StringBuilder sb, ref bool whereOK, string filter)
        {
            if (whereOK)
            {
                sb.Append(" AND ");
            }
            else
            {
                sb.Append(" WHERE ");
                whereOK = true;
            }
            sb.Append(filter);
        }
    }
}
