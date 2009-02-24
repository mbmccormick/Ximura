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
        #region LogError(string status, string error)
        /// <summary>
        /// This method is used to catch an error and log it to the database.
        /// </summary>
        /// <param name="status">The error status</param>
        /// <param name="error">The error description.</param>
        public static void LogError(string status, string error)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection("context connection=true"))
                {
                    cn.Open();
                    SqlCommand sqlCmd = new SqlCommand("INSERT INTO [_Error]([Status],[Description])VALUES(@status,@error)", cn);
                    sqlCmd.CommandType = CommandType.Text;
                    sqlCmd.Parameters.Add("@status", SqlDbType.VarChar, 5).Value = status;
                    sqlCmd.Parameters.Add("@error", SqlDbType.VarChar, 1000).Value = error;

                    sqlCmd.ExecuteNonQuery();

                    cn.Close();
                }
            }
            catch
            {
                //Do nothing as there is no point.
            }
        }
        #endregion // LogError(string status, string error)

    }
}
