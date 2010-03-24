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
namespace Ximura.Data.SQL
{
    public partial class EntityAccess
    {
        private static readonly string EntityUpdateSQLRemove = @"UPDATE [Entity] SET [Status] = 'inactive' WHERE [Entity_Sequence_ID]=@EntityRef;DELETE FROM [Entity_Attribute] WHERE [Entity_Sequence_ID]=@EntityRef;DELETE FROM [Entity_Reference] WHERE [Entity_Sequence_ID]=@EntityRef;";

        [Microsoft.SqlServer.Server.SqlProcedure]
        public static SqlInt32 EntityUpdate(
              [SqlFacet(IsNullable = false)]SqlGuid currentVid,
              [SqlFacet(IsNullable = false)]SqlBytes data
            , [SqlFacet(IsNullable = true)]SqlBytes references
            , [SqlFacet(IsNullable = true)]SqlBytes attributes
            , [SqlFacet(IsNullable = true)]SqlBoolean useTransaction
            , out SqlInt32 Status
            , [SqlFacet(MaxSize = 100)]out SqlString SubStatus
              )
        {
            Entity ent = new Entity(data.Stream, false);
            string[] entityType = ent.mcType.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            bool useTran = useTransaction.IsNull || useTransaction.Value;
            Status = 100;
            string eMessage = null;

            if (entityType.Length < 2)
                throw new InvalidAQNException(string.Format("AQN is invalid: {0}", ent.mcType));

            using (SqlConnection cn = new SqlConnection("context connection=true"))
            {
                SqlTransaction transaction = null;
                try
                {
                    cn.Open();

                    if (useTran)
                        transaction = cn.BeginTransaction("Save");

                    Status = EntityVerifyAndRemove(cn, transaction, currentVid.Value, ent.cid, ent.tid);

                    if (Status == 200)
                    {
                        data.Stream.Position = 0;
                        Status = EntityCreateInternal(cn, transaction, data, references, attributes, out eMessage);
                    }
                    else
                        eMessage = "EntityVerifyAndRemove error";

                    if (useTran)
                    {
                        if (Status == 200)
                            transaction.Commit();
                        else
                            transaction.Rollback();
                    }

                    cn.Close();
                }
                catch (Exception ex)
                {
                    Status = 500;
                    eMessage = ex.ToString();
                    if (useTran && transaction != null)
                        transaction.Rollback();
                    cn.Close();
                }
            }

            //if (Status != 200)
            //{
            //    SqlContext.Pipe.Send("Error " + Status.ToString() + ": " + eMessage);
            //    LogError(Status.ToString(), eMessage);
            //}
            SubStatus = eMessage==null?"":eMessage;

            SendCommandStatus(SqlContext.Pipe, Status, SubStatus);

            return Status;
        }

    }
}
