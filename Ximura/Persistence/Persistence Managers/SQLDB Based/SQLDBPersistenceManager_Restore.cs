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
﻿#region using
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

using Ximura;
using Ximura.Persistence;
using Ximura.Data;
using Ximura.Server;
using Ximura.Command;

using CH = Ximura.Helper.Common;
using AH = Ximura.Helper.AttributeHelper;
#endregion
namespace Ximura.Persistence
{
    public partial class SQLDBPersistenceManager<CONT, DCONT> 
    {
        protected override bool Restore(CDSContext context)
        {
            return base.Restore(context);
        }        
    }
}
