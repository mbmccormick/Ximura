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
        protected override bool Update(CDSContext context)
        {
            CONT data = context.Request.Data as CONT;
            Content newData = null;

            string strConn = SQLConnectionResolve(context);

            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    using (SqlConnection sqlConn = new SqlConnection(strConn))
                    {
                        Guid? newVersionID;

                        sqlConn.Open();
                        int response = EntityUpdate(data, sqlConn, null, out newVersionID, out newData);

                        if (response == 200)
                        {
                            context.Response.Data = newData;
                            context.Response.NewVersionID = newVersionID.Value;
                            scope.Complete();
                        }
                        else
                        {
                            context.Response.Data = null;
                            context.Response.NewVersionID = null;
                            if (newData != null && newData.ObjectPoolCanReturn)
                                newData.ObjectPoolReturn();
                        }

                        context.Response.Status = response.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                if (newData != null && newData.ObjectPoolCanReturn)
                    newData.ObjectPoolReturn();

                context.Response.Data = null;
                context.Response.NewVersionID = null;
                context.Response.Status = CH.HTTPCodes.InternalServerError_500;
                context.Response.Substatus = ex.Message;
                return true;
            }

            return true;
        }

        protected virtual int EntityUpdate(Content data, SqlConnection sqlConn, SqlTransaction sqlTran, out Guid? newVersionID, out Content newData)
        {
            return EntityUpdate(data, sqlConn, sqlTran, false, out newVersionID, out newData);
        }

        protected virtual int EntityUpdate(Content data, SqlConnection sqlConn, SqlTransaction sqlTran, bool useTran, out Guid? newVersionID, out Content newData)
        {
            newVersionID = null;
            newData = null;
            TypeConverter conv = GetTypeConverter();

            try
            {
                //Create the cloned item and set the new version id
                newData = (Content)data.Clone();
                newData.IDVersion = Guid.NewGuid();
                newVersionID = newData.IDVersion;

                SqlCommand sqlCmdBinary = new SqlCommand("EntityUpdate", sqlConn, sqlTran);
                sqlCmdBinary.CommandType = CommandType.StoredProcedure;

                sqlCmdBinary.Parameters.Add("@currentVid", SqlDbType.UniqueIdentifier).Value = data.IDVersion;

                sqlCmdBinary.Parameters.Add("@Data", SqlDbType.Binary).Value = ParseEntity(newData);
                sqlCmdBinary.Parameters.Add("@references", SqlDbType.Binary).Value = ParseReferences(newData, conv);
                sqlCmdBinary.Parameters.Add("@attributes", SqlDbType.Binary).Value = ParseAttributes(newData, conv);
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

                return response;
            }
            catch (Exception ex)
            {
                return 500;
            }
        }
    }
}
