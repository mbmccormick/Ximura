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
﻿#region using
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Drawing;
using System.Threading;
using System.Reflection;
using System.IO;
using System.IO.Compression;
using System.Xml;
using System.Security.Cryptography;
using System.Runtime.Serialization;

using Ximura;
using Ximura.Framework;
using Ximura.Framework;
using Ximura.Data;

using AH = Ximura.AttributeHelper;
using RH = Ximura.Reflection;
using CH = Ximura.Common;

#endregion
namespace Ximura.Framework
{
    public class AppServerCommandConfiguration : CommandConfiguration, IXimuraConfigurationManager
    {
        #region Constructor
        /// <summary>
        /// The default constructor
        /// </summary>
        public AppServerCommandConfiguration()  { }

        /// <summary>
        /// This is the deserialization constructor. 
        /// </summary>
        /// <param name="info">The Serialization info object that contains all the relevant data.</param>
        /// <param name="context">The serialization context.</param>
        public AppServerCommandConfiguration(SerializationInfo info, StreamingContext context)
            :
            base(info, context) { }
        #endregion


    }
}
