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
        [Microsoft.SqlServer.Server.SqlProcedure]
        public static SqlInt32 EntityCreate(
            [SqlFacet(IsNullable = false)] SqlBytes data
            , [SqlFacet(IsNullable = true)]SqlBytes references
            , [SqlFacet(IsNullable = true)]SqlBytes attributes
            , [SqlFacet(IsNullable = true)]SqlBoolean useTransaction
            , out SqlInt32 Status
            , [SqlFacet(MaxSize = 100)]out SqlString SubStatus
            )
        {
            bool useTran = useTransaction.IsNull || useTransaction.Value;

            Status = 100;
            string eMessage = "Internal error";

            using (SqlConnection cn = new SqlConnection("context connection=true"))
            {
                SqlTransaction transaction = null;
                try
                {
                    cn.Open();
                    if (useTran)
                        transaction = cn.BeginTransaction("Save");

                    Status = EntityCreateInternal(cn, transaction, data, references, attributes, out eMessage);

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

            SubStatus = eMessage;

            //if (Status != 200)
            //{
            //    LogError(Status.ToString(), eMessage);
            //    //SqlContext.Pipe.Send("Error " + Status.ToString() + ": " + eMessage);
            //}

            SendCommandStatus(SqlContext.Pipe, Status, SubStatus);

            return Status;
        }



        private static int EntityCreateInternal(SqlConnection cn, SqlTransaction transaction,
            SqlBytes data, SqlBytes references, SqlBytes attributes, out string eMessage)
        {
            int eResponse = 100;
            SqlInt64 insertID;
            Guid typeID;
            eMessage = "OK";

            eResponse = ProcessEntity(data, cn, transaction, out insertID, out typeID);
            if (eResponse != 200)
            {
                eMessage = "ProcessEntity error";
                return eResponse;
            }

            eResponse = ProcessBinary(data, cn, transaction, insertID.Value);
            if (eResponse != 200)
            {
                eMessage = "ProcessBinary error";
                return eResponse;
            }

            //Add the references
            if (!references.IsNull)
            {
                eResponse = ProcessReferences(references, cn, transaction, insertID.Value, typeID);
                if (eResponse != 200)
                {
                    eMessage = "ProcessReferences error";
                    return eResponse;
                }
            }

            //Add the attributes
            if (!attributes.IsNull)
            {
                eResponse = ProcessAttributes(attributes, cn, transaction, insertID.Value);
                if (eResponse != 200)
                    eMessage = "ProcessAttributes error";
            }

            return eResponse;
        }
    }
}
