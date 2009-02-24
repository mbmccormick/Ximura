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
        private static readonly string EntityUpdateSQLVerify = @"SELECT [Entity_Sequence_ID], [Status] FROM [Entity] WHERE [Entity_Version_ID]=@VID AND [Entity_Content_ID]=@CID AND [Entity_Type_ID]=@TID AND [Status]='active'";
        private static readonly string ProcessBinarySQL = "INSERT INTO [Entity_Binary] ([Entity_Sequence_ID],[Image_Value]) VALUES (@Entity_Sequence_ID,@Data)";

        private static readonly string ProcessReferencesSQL = "INSERT INTO [Entity_Reference] ([Entity_Sequence_ID],[Reference_Type],[Reference_Value]) VALUES (@Entity_Sequence_ID,@Reference_Type,@Reference_Value)";
        private static readonly string ProcessReferencesCheckUniqueSQL = "SELECT TOP 1 [Entity_Type_ID] FROM [View_EntityReferenceUnique] WHERE [Entity_Type_ID] = @Entity_Type_ID AND [Reference_Type]=@Reference_Type AND [Reference_Value]=@Reference_Value";
        
        private static readonly string ProcessAttributesSQL = "INSERT INTO [Entity_Attribute] ([Entity_Sequence_ID],[Type],[SubType],[Language],[Value])VALUES(@Entity_Sequence_ID,@Reference_Type,@Reference_Subtype,@Reference_Language,@Reference_Value)";
        private static readonly string EntityCreateSQL = @"DECLARE @InsertID bigint;INSERT [Entity]([Entity_Version_ID],[Entity_Content_ID],[Entity_Type_ID],[Assembly_Qualified_Name],[Create_Date],[Built],[Status]) VALUES (@Entity_Version_ID,@Entity_Content_ID,@Entity_Type_ID,@Assembly_Qualified_Name,@Create_Date,1,'active');SELECT @InsertID = @@IDENTITY;SELECT @InsertID ""InsertID""";

        private static int ProcessEntity(SqlBytes data, SqlConnection cn, SqlTransaction transaction, 
            out SqlInt64 insertID, out Guid typeID)
        {
            Entity ent = new Entity(data.Stream, false);
            typeID = ent.tid;
            string[] entityType = ent.mcType.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            if (entityType.Length < 2)
                throw new InvalidAQNException(string.Format("AQN is invalid: {0}", ent.mcType));

            //Add the base entity
            SqlCommand sqlCmd;
            insertID = SqlInt64.Null;

            if (transaction!=null)
                sqlCmd = new SqlCommand(EntityCreateSQL, cn, transaction);
            else
                sqlCmd = new SqlCommand(EntityCreateSQL, cn);

            sqlCmd.CommandType = CommandType.Text;

            sqlCmd.Parameters.Add("@Entity_Version_ID", SqlDbType.UniqueIdentifier).Value = ent.vid;
            sqlCmd.Parameters.Add("@Entity_Content_ID", SqlDbType.UniqueIdentifier).Value = ent.cid;
            sqlCmd.Parameters.Add("@Entity_Type_ID", SqlDbType.UniqueIdentifier).Value = typeID;
            sqlCmd.Parameters.Add("@Assembly_Qualified_Name", SqlDbType.NVarChar, 255).Value = entityType[0] + ", " + entityType[1];
            sqlCmd.Parameters.Add("@Create_Date", SqlDbType.DateTime).Value = DateTime.Now;

            using (SqlDataReader reader = sqlCmd.ExecuteReader(CommandBehavior.SingleRow))
            {
                if (reader.HasRows && reader.Read())
                {
                    insertID = reader.GetSqlInt64(0);
                }
            }

            if (insertID.IsNull)
                return 515;

            return 200;
        }

        private static int ProcessBinary(SqlBytes data, SqlConnection cn, SqlTransaction transaction, long id)
        {
            //Add the binary data
            SqlCommand sqlCmd;
            //if (transaction != null)
            //    sqlCmd = new SqlCommand("spx_EntityBinaryAdd", cn, transaction);
            //else
            //    sqlCmd = new SqlCommand("spx_EntityBinaryAdd", cn);
            //sqlCmd.CommandType = CommandType.StoredProcedure;

            if (transaction != null)
                sqlCmd = new SqlCommand(ProcessBinarySQL, cn, transaction);
            else
                sqlCmd = new SqlCommand(ProcessBinarySQL, cn);
            sqlCmd.CommandType = CommandType.Text;

            sqlCmd.Parameters.Add("@Entity_Sequence_ID", SqlDbType.BigInt).Value = id;
            sqlCmd.Parameters.Add("@Data", SqlDbType.Binary).Value = data;

            //SqlParameter paramRet = new SqlParameter("@return_value", SqlDbType.Int);
            //paramRet.Direction = ParameterDirection.ReturnValue;
            //sqlCmd.Parameters.Add(paramRet);

            int count = sqlCmd.ExecuteNonQuery();

            return (count>0)?200:500;
        }

        private static int ProcessReferences(SqlBytes data, SqlConnection cn, SqlTransaction transaction, long id, Guid typeid)
        {
            int ret = 200;
            BinaryReader br = new BinaryReader(data.Stream, Encoding.UTF8);
            int length = br.ReadInt32();

            int status = 100;
            for (int len = 0; len < length; len++)
            {
                string refType = br.ReadString();
                string refValue = br.ReadString();

                status = ReferenceCheck(cn, transaction, typeid, refType, refValue);

                if (status == 200)
                    status = ReferenceAdd(cn, transaction, id, refType, refValue);
                if (status != 200)
                    return status;
            }

            return 200;
        }

        private static int ReferenceAdd(SqlConnection cn, SqlTransaction transaction, 
            long id, string refType, string refValue)
        {
            //Add the binary data
            SqlCommand sqlCmd;

            if (transaction != null)
                sqlCmd = new SqlCommand(ProcessReferencesSQL, cn, transaction);
            else
                sqlCmd = new SqlCommand(ProcessReferencesSQL, cn);

            sqlCmd.CommandType = CommandType.Text;
            sqlCmd.Parameters.Add("@Entity_Sequence_ID", SqlDbType.BigInt).Value = id;
            sqlCmd.Parameters.Add("@Reference_Type", SqlDbType.VarChar, 50).Value = refType;
            sqlCmd.Parameters.Add("@Reference_Value", SqlDbType.NVarChar, 255).Value = refValue;

            int count = sqlCmd.ExecuteNonQuery();

            if (count == 0)
                return 500;

            return 200;
        }

        private static int ReferenceCheck(SqlConnection cn, SqlTransaction transaction,
            Guid typeid, string refType, string refValue)
        {
            //Add the binary data
            SqlCommand sqlCmd;

            if (transaction != null)
                sqlCmd = new SqlCommand(ProcessReferencesCheckUniqueSQL, cn, transaction);
            else
                sqlCmd = new SqlCommand(ProcessReferencesCheckUniqueSQL, cn);

            sqlCmd.CommandType = CommandType.Text;
            sqlCmd.Parameters.Add("@Entity_Type_ID", SqlDbType.UniqueIdentifier).Value = typeid;
            sqlCmd.Parameters.Add("@Reference_Type", SqlDbType.VarChar, 50).Value = refType;
            sqlCmd.Parameters.Add("@Reference_Value", SqlDbType.NVarChar, 255).Value = refValue;

            object id = sqlCmd.ExecuteScalar();

            if (id == null)
                return 200;

            return 409;
        }

        private static int ProcessAttributes(SqlBytes data, SqlConnection cn, SqlTransaction transaction, long id)
        {
            int ret = 200;
            BinaryReader br = new BinaryReader(data.Stream, Encoding.UTF8);
            int length = br.ReadInt32();

            for (int len = 0; len < length; len++)
            {
                string refType = br.ReadString();
                string refSubtype = br.ReadString();
                string refLanguage = br.ReadString();
                string refValue = br.ReadString();
                //Add the binary data
                SqlCommand sqlCmd;
                //if (transaction != null)
                //    sqlCmd = new SqlCommand("spx_EntityAttributeAdd", cn, transaction);
                //else
                //    sqlCmd = new SqlCommand("spx_EntityAttributeAdd", cn);
                //sqlCmd.CommandType = CommandType.StoredProcedure;

                if (transaction != null)
                    sqlCmd = new SqlCommand(ProcessAttributesSQL, cn, transaction);
                else
                    sqlCmd = new SqlCommand(ProcessAttributesSQL, cn);

                sqlCmd.CommandType = CommandType.Text;
                sqlCmd.Parameters.Add("@Entity_Sequence_ID", SqlDbType.BigInt).Value = id;
                sqlCmd.Parameters.Add("@Reference_Type", SqlDbType.VarChar, 50).Value = refType;
                sqlCmd.Parameters.Add("@Reference_SubType", SqlDbType.VarChar, 50).Value = refSubtype;
                sqlCmd.Parameters.Add("@Reference_Language", SqlDbType.VarChar, 10).Value = refLanguage;
                sqlCmd.Parameters.Add("@Reference_Value", SqlDbType.NVarChar, 3000).Value = refValue;

                //SqlParameter paramRet = new SqlParameter("@return_value", SqlDbType.Int);
                //paramRet.Direction = ParameterDirection.ReturnValue;
                //sqlCmd.Parameters.Add(paramRet);

                int count = sqlCmd.ExecuteNonQuery();

                if (count==0)
                    return 500;
            }

            return 200;
        }

        private static int EntityVerify(SqlConnection cn, SqlTransaction transaction
            , Guid vid, Guid cid, Guid tid, out SqlInt64 recordID, out SqlString status)
        {
            SqlCommand sqlCmd;

            recordID = SqlInt64.Null;
            status = SqlString.Null;

            if (transaction != null)
                sqlCmd = new SqlCommand(EntityUpdateSQLVerify, cn, transaction);
            else
                sqlCmd = new SqlCommand(EntityUpdateSQLVerify, cn);

            sqlCmd.CommandType = CommandType.Text;

            sqlCmd.Parameters.Add("@VID", SqlDbType.UniqueIdentifier).Value = vid;
            sqlCmd.Parameters.Add("@CID", SqlDbType.UniqueIdentifier).Value = cid;
            sqlCmd.Parameters.Add("@TID", SqlDbType.UniqueIdentifier).Value = tid;

            using (SqlDataReader reader = sqlCmd.ExecuteReader(CommandBehavior.SingleRow))
            {
                if (reader.HasRows && reader.Read())
                {
                    recordID = reader.GetSqlInt64(0);
                    status = reader.GetString(1);
                }
                else
                    return 404;
            }

            if (recordID.IsNull)
                return 404;

            return 200;
        }

        private static int EntityVerifyAndRemove(SqlConnection cn, SqlTransaction transaction
            , Guid vid, Guid cid, Guid tid)
        {
            SqlInt64 recordID;
            SqlString status;
            int response = EntityVerify(cn, transaction, vid, cid, tid, out recordID, out status);

            if (response != 200)
                return response;

            SqlCommand sqlCmdRemove;

            if (transaction != null)
                sqlCmdRemove = new SqlCommand(EntityUpdateSQLRemove, cn, transaction);
            else
                sqlCmdRemove = new SqlCommand(EntityUpdateSQLRemove, cn);

            sqlCmdRemove.CommandType = CommandType.Text;

            sqlCmdRemove.Parameters.Add("@EntityRef", SqlDbType.BigInt).Value = recordID.Value;

            sqlCmdRemove.ExecuteNonQuery();

            return 200;
        }

        private static void SendCommandStatus(SqlPipe pipe, SqlInt32 status, SqlString substatus)
        {
            SqlDataRecord rec = new SqlDataRecord(
                  new SqlMetaData("Status", SqlDbType.Int)
                , new SqlMetaData("SubStatus", SqlDbType.NVarChar, 150)
                );

            SqlContext.Pipe.SendResultsStart(rec);

            rec.SetSqlInt32(0, status);
            rec.SetSqlString(1, substatus);

            pipe.SendResultsRow(rec);
            pipe.SendResultsEnd();

        }
    }
}
