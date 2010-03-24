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
        protected override bool Delete(CDSContext context)
        {
            string strConn = SQLConnectionResolve(context);

            Guid? IDType = context.Request.DataTypeID;

            if (!IDType.HasValue)
                IDType = Content.GetContentTypeAttributeID(context.Request.DataType);

            if (!context.Request.DataVersionID.HasValue || !context.Request.DataContentID.HasValue)
            {
                context.Response.Status = CH.HTTPCodes.BadRequest_400;
                context.Response.Substatus = "DataVersionID or DataContentID are null.";
                return true;
            }

            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    using (SqlConnection sqlConn = new SqlConnection(strConn))
                    {
                        sqlConn.Open();

                        int response = EntityDelete(sqlConn
                            , IDType.Value
                            , context.Request.DataContentID.Value
                            , context.Request.DataVersionID.Value
                            );

                        if (response == 200)
                        {
                            scope.Complete();
                        }

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

        protected virtual int EntityDelete(SqlConnection sqlConn, 
            Guid IDType, Guid IDContent, Guid IDVersion)
        {
            SqlCommand sqlCmd = new SqlCommand("EntityDelete", sqlConn);
            sqlCmd.CommandType = CommandType.StoredProcedure;

            sqlCmd.Parameters.Add("@IDType", SqlDbType.UniqueIdentifier).Value = IDType;
            sqlCmd.Parameters.Add("@IDContent", SqlDbType.UniqueIdentifier).Value = IDContent;
            sqlCmd.Parameters.Add("@IDVersion", SqlDbType.UniqueIdentifier).Value = IDVersion;
            sqlCmd.Parameters.Add("@UseTransaction", SqlDbType.Bit, 1).Value = 0;

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

            sqlCmd.ExecuteNonQuery();

            return (int)paramStatus.Value;
        }

    }
}
