﻿#region Copyright
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
        [Microsoft.SqlServer.Server.SqlProcedure]
        public static SqlInt32 EntityRestore(
             [SqlFacet(IsNullable = false)]SqlGuid IDType
           , [SqlFacet(IsNullable = false)]SqlGuid IDContent
           , [SqlFacet(IsNullable = false)]SqlGuid IDVersion
           , [SqlFacet(IsNullable = true)]SqlBoolean UseTransaction
            , out SqlInt32 Status
            , [SqlFacet(MaxSize = 100)]out SqlString SubStatus
            )
        {
            Status = 501;
            SubStatus = "Not implemented.";
            return Status;
        }
    }
}
