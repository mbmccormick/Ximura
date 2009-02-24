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
        [Microsoft.SqlServer.Server.SqlProcedure]
        public static SqlInt32 EntityVersionCheck(
              [SqlFacet(IsNullable = false)]SqlGuid IDType
            , [SqlFacet(IsNullable = true)]SqlGuid IDContent
            , [SqlFacet(IsNullable = true)]SqlGuid IDVersion
            , out SqlInt32 Status
            , [SqlFacet(MaxSize = 100)]out SqlString SubStatus
           )
        {
            //Both paramters cannot be null.
            if (IDContent.IsNull && IDVersion.IsNull)
            {
                Status = 400;
                SubStatus = "Missing parameter.";
                return Status;
            }

            try
            {
                using (SqlConnection cn = new SqlConnection("context connection=true"))
                {
                    cn.Open();
                    SqlCommand sqlCmd;

                    if (!IDContent.IsNull)
                        sqlCmd = EntityVersionCheckOnIDContent(cn, IDType, IDContent, IDVersion);
                    else
                        sqlCmd = EntityVersionCheckOnIDVersion(cn, IDType, IDVersion);

                    using (SqlDataReader sqlReader = sqlCmd.ExecuteReader())
                    {
                        if (!sqlReader.HasRows || !sqlReader.Read())
                        {
                            Status = 404;
                            SubStatus = "Not found.";
                            return Status;
                        }

                        SqlGuid vid = sqlReader.GetSqlGuid(0);
                        SqlGuid cid = sqlReader.GetSqlGuid(1);

                        SqlDataRecord rec = new SqlDataRecord(
                              new SqlMetaData("VID", SqlDbType.UniqueIdentifier)
                            , new SqlMetaData("CID", SqlDbType.UniqueIdentifier));

                        SqlContext.Pipe.SendResultsStart(rec);
                        rec.SetGuid(0, vid.Value);
                        rec.SetGuid(1, cid.Value);
                        SqlContext.Pipe.SendResultsRow(rec);
                        SqlContext.Pipe.SendResultsEnd();

                        if (!IDVersion.IsNull && vid != IDVersion)
                        {
                            Status = 412;
                            SubStatus = "VersionID has changed.";
                            return Status;
                        }

                        Status = 200;
                        SubStatus = "OK.";
                    }
                }
            }
            catch (Exception ex)
            {
                LogError("500", "EntityVersionCheck --> " + ex.Message);
                Status = 500;
                string message = ex.Message;
                SubStatus = string.Format("Exception: {0}", message.Length > 85 ? message.Substring(0, 85) : message);
            }

            return Status;
        }

        private static SqlCommand EntityVersionCheckOnIDVersion(SqlConnection cn, SqlGuid IDType, SqlGuid IDVersion)
        {
            SqlCommand sqlCmd = new SqlCommand("SELECT e.[Entity_Version_ID], e.[Entity_Content_ID] FROM [Entity] e INNER JOIN [Entity] e2 ON e2.[Entity_Type_ID] = @TID AND e2.[Entity_Version_ID]=@VIDOld WHERE e.[Entity_Type_ID] = @TID AND e.[Entity_Content_ID]=e2.[Entity_Content_ID] AND e.[Status]='active'", cn);
            sqlCmd.CommandType = CommandType.Text;
            sqlCmd.Parameters.Add("@TID", SqlDbType.UniqueIdentifier).Value = IDType.Value;
            sqlCmd.Parameters.Add("@VIDOld", SqlDbType.UniqueIdentifier).Value = IDVersion.Value;

            return sqlCmd;
        }

        private static SqlCommand EntityVersionCheckOnIDContent(SqlConnection cn, SqlGuid IDType, SqlGuid IDContent, SqlGuid IDVersion)
        {
            SqlCommand sqlCmd = new SqlCommand("SELECT [Entity_Version_ID], [Entity_Content_ID] FROM [Entity] WHERE [Entity_Type_ID] = @TID AND [Entity_Content_ID]=@CID AND [Status]='active'", cn);
            sqlCmd.CommandType = CommandType.Text;
            sqlCmd.Parameters.Add("@TID", SqlDbType.UniqueIdentifier).Value = IDType.Value;
            sqlCmd.Parameters.Add("@CID", SqlDbType.UniqueIdentifier).Value = IDContent.Value;

            return sqlCmd;
        }
    }
}
