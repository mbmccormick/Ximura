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
using System.Runtime.Serialization;
using System.Linq;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;

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
    public class CacheManagerCDSConfiguration: CommandConfiguration
    {
        #region Constructor
        /// <summary>
        /// The default constructor
        /// </summary>
        public CacheManagerCDSConfiguration() { }

        /// <summary>
        /// This is the deserialization constructor. 
        /// </summary>
        /// <param name="info">The Serialization info object that contains all the relevant data.</param>
        /// <param name="context">The serialization context.</param>
        public CacheManagerCDSConfiguration(SerializationInfo info, StreamingContext context)
            :
            base(info, context) { }
        #endregion

    }
}
