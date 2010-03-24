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
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Reflection;
using System.Transactions;

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
        #region Create(CDSContext context)
        /// <summary>
        /// This method creates a new entity.
        /// </summary>
        /// <param name="context"></param>
        /// <returns>Returns true if the process is complete.</returns>
        protected override bool Create(CDSContext context)
        {
            CONT data = context.Request.Data as CONT;

            string strConn = SQLConnectionResolve(context);

            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    using (SqlConnection sqlConn = new SqlConnection(strConn))
                    {
                        sqlConn.Open();
                        int response = EntityCreate(data, sqlConn, null, false);

                        if (response == 200)
                            scope.Complete();
                        context.Response.Status = response.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                context.Response.Status = CH.HTTPCodes.InternalServerError_500;
                context.Response.Substatus = ex.Message;
                return true;
            }

            return true;
        }
        #endregion // Create(CDSContext context)

        #region EntityCreate
        protected virtual int EntityCreate(Content data, SqlConnection sqlConn, SqlTransaction sqlTran)
        {
            return EntityCreate(data, sqlConn, sqlTran, true);
        }

        protected virtual int EntityCreate(Content data, SqlConnection sqlConn, SqlTransaction sqlTran, bool useTran)
        {
            TypeConverter conv = GetTypeConverter();
            try
            {
                SqlCommand sqlCmdBinary = new SqlCommand("EntityCreate", sqlConn, sqlTran);
                sqlCmdBinary.CommandType = CommandType.StoredProcedure;

                sqlCmdBinary.Parameters.Add("@Data", SqlDbType.Binary).Value = ParseEntity(data);
                sqlCmdBinary.Parameters.Add("@references", SqlDbType.Binary).Value = ParseReferences(data, conv);
                sqlCmdBinary.Parameters.Add("@attributes", SqlDbType.Binary).Value = ParseAttributes(data, conv);
                sqlCmdBinary.Parameters.Add("@useTransaction", SqlDbType.Bit).Value = useTran ? 1 : 0;

                SqlParameter paramStatus = new SqlParameter();
                paramStatus.ParameterName = "@Status";
                paramStatus.SqlDbType = SqlDbType.Int;
                paramStatus.IsNullable = true;
                paramStatus.Direction = ParameterDirection.InputOutput;
                paramStatus.Value = System.DBNull.Value;
                sqlCmdBinary.Parameters.Add(paramStatus);

                SqlParameter paramSubstatus = new SqlParameter();
                paramSubstatus.ParameterName = "@Substatus";
                paramSubstatus.SqlDbType = SqlDbType.NVarChar;
                paramSubstatus.Size = 100;
                paramSubstatus.IsNullable = true;
                paramSubstatus.Direction = ParameterDirection.InputOutput;
                paramSubstatus.Value = System.DBNull.Value;
                sqlCmdBinary.Parameters.Add(paramSubstatus);

                SqlParameter paramReturnValue = new SqlParameter();
                paramReturnValue.ParameterName = "@return_value";
                paramReturnValue.SqlDbType = SqlDbType.Int;
                paramReturnValue.Direction = ParameterDirection.ReturnValue;

                sqlCmdBinary.Parameters.Add(paramReturnValue);

                int response = 100;

                //int countBin = sqlCmdBinary.ExecuteNonQuery();
                using (SqlDataReader reader = sqlCmdBinary.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        reader.Read();
                        response = reader.GetInt32(0);
                        //cid = reader.GetSqlValue(1);
                    }
                    else
                        response = 500;

                    reader.Close();
                }

                //int response = (int)paramStatus.Value;

                return response;
            }
            catch (Exception ex)
            {
                return 500;
            }
        }
        #endregion // EntitySave

    }
}
