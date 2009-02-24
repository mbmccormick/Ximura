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
        public static SqlInt32 EntityResolveReference(
              [SqlFacet(IsNullable = false)]SqlGuid TypeID
            , [SqlFacet(IsNullable = false, MaxSize = 50)]string refType
            , [SqlFacet(IsNullable = false, MaxSize = 255)]string refValue
            , out SqlInt32 Status
            , [SqlFacet(MaxSize = 100)]out SqlString SubStatus
            )
        {
            try
            {
                SqlGuid cid = SqlGuid.Null;
                SqlGuid vid = SqlGuid.Null;

                using (SqlConnection cn = new SqlConnection("context connection=true"))
                {
                    cn.Open();

                    //SqlCommand sqlCmd = new SqlCommand("spx_EntityVersionCheckByReference", cn);
                    //sqlCmd.CommandType = CommandType.StoredProcedure;
                    SqlCommand sqlCmd = new SqlCommand(
                        "SELECT e.Entity_Content_ID , e.Entity_Version_ID FROM Entity e INNER JOIN Entity_Reference er ON e.Entity_Sequence_ID = er.Entity_Sequence_ID WHERE e.Status='active' AND e.Entity_Type_ID=@TypeID AND er.Reference_Type = @RefType AND er.Reference_Value = @RefValue"
                        , cn);
                    sqlCmd.CommandType = CommandType.Text;
                    sqlCmd.Parameters.Add("@TypeID", SqlDbType.UniqueIdentifier).Value = TypeID.Value;
                    sqlCmd.Parameters.Add("@RefType", SqlDbType.VarChar, 50).Value = refType;
                    sqlCmd.Parameters.Add("@RefValue", SqlDbType.NVarChar, 255).Value = refValue;

                    using (SqlDataReader reader = sqlCmd.ExecuteReader(CommandBehavior.SingleRow))
                    {
                        if (reader.HasRows && reader.Read())
                        {
                            cid = reader.GetSqlGuid(reader.GetOrdinal("Entity_Content_ID"));
                            vid = reader.GetSqlGuid(reader.GetOrdinal("Entity_Version_ID"));
                        }
                    }

                    cn.Close();
                }

                SqlDataRecord rec = new SqlDataRecord(
                      new SqlMetaData("VID", SqlDbType.UniqueIdentifier)
                    , new SqlMetaData("CID", SqlDbType.UniqueIdentifier)
                    , new SqlMetaData("TID", SqlDbType.UniqueIdentifier)
                    );
                SqlContext.Pipe.SendResultsStart(rec);

                if (!vid.IsNull)
                {
                    rec.SetGuid(0, vid.Value);
                    rec.SetGuid(1, cid.Value);
                    rec.SetGuid(2, TypeID.Value);
                }

                SqlContext.Pipe.SendResultsRow(rec);
                SqlContext.Pipe.SendResultsEnd();

                if (vid.IsNull)
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
            catch (Exception ex)
            {
                Status = 500;
                string message = ex.Message;
                SubStatus = string.Format("Exception: {0}", message.Length > 85 ? message.Substring(0, 85) : message);
            }

            return Status;
        }

    }
}
