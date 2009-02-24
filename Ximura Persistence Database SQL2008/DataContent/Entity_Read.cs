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
        /// <summary>
        /// This stored procedure reads a entity from the SQL store.
        /// </summary>
        /// <param name="Tid">The entity type id.</param>
        /// <param name="ID">The content or version ID.</param>
        /// <param name="IDIsVersion">A parameter specifying whether the ID is the content or the version. True specifies that this is the version ID.</param>
        /// <param name="Status">The output status.</param>
        /// <param name="SubStatus">A friendly description of the output status.</param>
        /// <returns>Returns the status code which is the same as Status. n.b. Status was introduced due to problems with the return parameter which would returns null in certain circumstances.</returns>
        /// <remarks>
        /// Returns status codes are as follows:
        ///     200 - OK
        ///     404 - Not found.
        ///     500 - Unexpected error, see the SubStatus for a reason.
        /// </remarks>
        [Microsoft.SqlServer.Server.SqlProcedure]
        public static SqlInt32 EntityRead(
            [SqlFacet(IsNullable = false)]SqlGuid Tid
          , [SqlFacet(IsNullable = false)]SqlGuid ID
          , [SqlFacet(IsNullable = true)]SqlBoolean IDIsVersion
          , out SqlInt32 Status
          , [SqlFacet(MaxSize=100)]out SqlString SubStatus
            )
        {

            if (IDIsVersion.IsNull)
                IDIsVersion = true;

            try
            {
                using (SqlConnection cn = new SqlConnection("context connection=true"))
                {
                    cn.Open();
                    SqlCommand sqlCmd = new SqlCommand();
                    sqlCmd.Connection = cn;
                    sqlCmd.CommandType = CommandType.Text;

                    if (IDIsVersion.IsTrue)
                        sqlCmd.CommandText = @"SELECT eb.[Image_Value] FROM Entity_Binary eb INNER JOIN Entity e ON eb.Entity_Sequence_ID = e.Entity_Sequence_ID WHERE (e.Entity_Type_ID = @tid) AND (e.Entity_Version_ID = @id)";
                    else
                        sqlCmd.CommandText = @"SELECT eb.[Image_Value] FROM Entity_Binary eb INNER JOIN Entity e ON eb.Entity_Sequence_ID = e.Entity_Sequence_ID WHERE (e.Entity_Type_ID = @tid) AND (e.Entity_Content_ID = @id) AND (e.Status='active')";

                    sqlCmd.Parameters.Add("@tid", SqlDbType.UniqueIdentifier).Value = Tid;
                    sqlCmd.Parameters.Add("@id", SqlDbType.UniqueIdentifier).Value = ID;

                    bool hasData = false;

                    using (SqlDataReader dr = sqlCmd.ExecuteReader())
                    {
                        hasData = dr.HasRows;
                        SqlContext.Pipe.Send(dr);
                    }

                    if (hasData)
                    {
                        Status = 200;
                        SubStatus = "OK";
                    }
                    else
                    {
                        Status = 404;
                        SubStatus = "Not Found";
                    }
                }
            }
            catch (Exception ex)
            {
                Status = 500;
                string message = ex.Message;
                SubStatus = string.Format("Exception: {0}", message.Length>85?message.Substring(0,85):message);
            }

            return Status;
        }
    }
}
