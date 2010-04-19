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
using System.Data.SqlTypes;
using System.Reflection;
using System.Transactions;
using System.Runtime.Serialization;

using Ximura;
using Ximura.Data;
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
        protected virtual T EntityRead<T>(CDSContext context, 
            SqlConnection sqlConn, Guid ID, bool IDIsVersion, out int response)
            where T : Content
        {
            Guid TypeID = typeof(T).GetContentTypeAttributeID();

            SqlCommand sqlCmd = new SqlCommand("EntityRead", sqlConn);
            sqlCmd.CommandType = CommandType.StoredProcedure;

            sqlCmd.Parameters.Add("@Tid", SqlDbType.UniqueIdentifier).Value = TypeID;
            sqlCmd.Parameters.Add("@ID", SqlDbType.UniqueIdentifier).Value = ID;
            sqlCmd.Parameters.Add("@IDIsVersion", SqlDbType.Bit).Value = IDIsVersion;

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

            SqlBytes returnBytes = SqlBytes.Null;
            using (SqlDataReader reader = sqlCmd.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    reader.Read();
                    returnBytes = reader.GetSqlBytes(0);
                }
                reader.Close();
            }

            response = (int)paramStatus.Value;

            if (response != 200)
                return null;

            SerializationInfo info = ContentFormatter.GetSerializationInfo(returnBytes.Stream);

            T data = context.GetObjectPool<T>().Get(info, new StreamingContext(StreamingContextStates.All));

            return data;
        }

        protected virtual Content EntityRead(CDSContext context, Type contentType,
            SqlConnection sqlConn, Guid TypeID, Guid ID, bool IDIsVersion, out int response)
        {

            SqlCommand sqlCmd = new SqlCommand("EntityRead", sqlConn);
            sqlCmd.CommandType = CommandType.StoredProcedure;

            sqlCmd.Parameters.Add("@Tid", SqlDbType.UniqueIdentifier).Value = TypeID;
            sqlCmd.Parameters.Add("@ID", SqlDbType.UniqueIdentifier).Value = ID;
            sqlCmd.Parameters.Add("@IDIsVersion", SqlDbType.Bit).Value = IDIsVersion;

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

            SqlBytes returnBytes = SqlBytes.Null;
            using (SqlDataReader reader = sqlCmd.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    reader.Read();
                    returnBytes = reader.GetSqlBytes(0);
                }
                reader.Close();
            }

            //response = (int)paramReturnValue.Value;
            response = (int)paramStatus.Value;

            if (response != 200)
                return null;

            SerializationInfo info = ContentFormatter.GetSerializationInfo(returnBytes.Stream);

            Content data = (Content)context.GetObjectPool(contentType).Get(info, new StreamingContext(StreamingContextStates.All));

            return data;
        }


        protected override bool Read(CDSContext context)
        {
            string strConn = SQLConnectionResolve(context);

            Guid? typeID = context.Request.DataTypeID;

            if (!typeID.HasValue)
                typeID = context.Request.DataType.GetContentTypeAttributeID();

            if (!context.Request.DataVersionID.HasValue && !context.Request.DataContentID.HasValue)
            {
                context.Response.Status = CH.HTTPCodes.NotFound_404;
                context.Response.Substatus = "DataVersionID and DataContentID are null.";
                return true;
            }

            Guid ID = context.Request.DataVersionID.HasValue
                ? context.Request.DataVersionID.Value : context.Request.DataContentID.Value;

            Content data;
            int response;

            try
            {
                using (SqlConnection sqlConn = new SqlConnection(strConn))
                {
                    sqlConn.Open();
                    data = EntityRead(context, context.Request.DataType, sqlConn,
                        typeID.Value, ID, context.Request.DataVersionID.HasValue, out response);
                }

                if (response == 200)
                {
                    context.Response.Data = data;
                }
                else
                {
                    context.Response.Data = null;
                }
                context.Response.Status = response.ToString();
            }
            catch (Exception ex)
            {
                context.Response.Status = CH.HTTPCodes.InternalServerError_500;
                context.Response.Substatus = ex.Message;
            }

            return true;
        }
    }
}
