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
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Reflection;
using System.Data.SqlTypes;

using Ximura;
using Ximura.Data;
using Ximura.Data;
using Ximura.Framework;
using Ximura.Framework;

using CH = Ximura.Common;
using AH = Ximura.AttributeHelper;
#endregion
namespace Ximura.Data
{
    public partial class SQLDBPersistenceManager<CONT, DCONT> 
    {
        /// <summary>
        /// This method checks the data store to see whether the references to the entity are current.
        /// </summary>
        /// <param name="context">The current CDS context.</param>
        /// <returns>
        /// The status codes for the response are as follows:
        ///     400 = missing parameter, either the contentID or versionID is null
        ///     404 = the content ID was not found
        ///     412 = the version ID is not the current version.
        ///     200 = OK, the contentID and versionID are correct.
        ///     500 = there has been an internal system error. check the SubStatus parameter for the exception description.
        /// </returns>
        protected override bool VersionCheck(CDSContext context)
        {
            string strConn = SQLConnectionResolve(context);
            Guid? typeID = context.Request.DataTypeID;
            if (!typeID.HasValue)
                typeID = context.Request.DataType.GetContentTypeAttributeID();

            try
            {
                using (SqlConnection sqlConn = new SqlConnection(strConn))
                {
                    sqlConn.Open();
                    SqlCommand sqlCmd = new SqlCommand("EntityVersionCheck", sqlConn);
                    sqlCmd.CommandType = CommandType.StoredProcedure;

                    sqlCmd.Parameters.Add("@IDType", SqlDbType.UniqueIdentifier).Value = typeID.Value;

                    if (context.Request.DataContentID.HasValue)
                        sqlCmd.Parameters.Add("@IDContent", SqlDbType.UniqueIdentifier).Value = context.Request.DataContentID.Value;
                    else
                        sqlCmd.Parameters.Add("@IDContent", SqlDbType.UniqueIdentifier).Value = System.DBNull.Value;

                    if (context.Request.DataVersionID.HasValue)
                        sqlCmd.Parameters.Add("@IDVersion", SqlDbType.UniqueIdentifier).Value = context.Request.DataVersionID.Value;
                    else
                        sqlCmd.Parameters.Add("@IDVersion", SqlDbType.UniqueIdentifier).Value = System.DBNull.Value;

                    SqlParameter paramStatus = new SqlParameter();
                    paramStatus.ParameterName = "@Status";
                    paramStatus.SqlDbType = SqlDbType.Int;
                    paramStatus.IsNullable = true;
                    paramStatus.Direction = ParameterDirection.InputOutput;
                    paramStatus.Value = System.DBNull.Value;
                    sqlCmd.Parameters.Add(paramStatus);

                    SqlParameter paramSubstatus = new SqlParameter();
                    paramSubstatus.ParameterName = "@Substatus";
                    paramSubstatus.SqlDbType = SqlDbType.NVarChar;
                    paramSubstatus.Size = 100;
                    paramSubstatus.IsNullable = true;
                    paramSubstatus.Direction = ParameterDirection.InputOutput;
                    paramSubstatus.Value = System.DBNull.Value;
                    sqlCmd.Parameters.Add(paramSubstatus);

                    SqlParameter paramReturnValue = new SqlParameter();
                    paramReturnValue.ParameterName = "@return_value";
                    paramReturnValue.SqlDbType = SqlDbType.Int;
                    paramReturnValue.Direction = ParameterDirection.ReturnValue;
                    sqlCmd.Parameters.Add(paramReturnValue);

                    SqlGuid vid = SqlGuid.Null;
                    SqlGuid cid = SqlGuid.Null;

                    using (SqlDataReader reader = sqlCmd.ExecuteReader())
                    {
                        if (reader.HasRows && reader.Read())
                        {
                            vid = reader.GetSqlGuid(0);
                            cid = reader.GetSqlGuid(1);
                        }
                        reader.Close();
                    }

                    int response = (int)paramStatus.Value;

                    context.Response.CurrentContentID = cid.IsNull ? (Guid?)null : cid.Value; 
                    context.Response.CurrentVersionID = vid.IsNull ? (Guid?)null : vid.Value;

                    context.Response.Status = response.ToString();
                }
            }
            catch (Exception ex)
            {
                context.Response.CurrentVersionID = null;
                context.Response.Status = CH.HTTPCodes.InternalServerError_500;
                context.Response.Substatus = ex.Message;
            }

            return true;
        }
    }
}
