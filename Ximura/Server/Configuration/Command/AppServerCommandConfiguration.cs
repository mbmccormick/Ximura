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
using Ximura.Server;
using Ximura.Command;
using Ximura.Data;
using Ximura.Helper;
using AH = Ximura.Helper.AttributeHelper;
using RH = Ximura.Helper.Reflection;
using CH = Ximura.Helper.Common;

#endregion
namespace Ximura.Server
{
    public class AppServerCommandConfiguration : CommandConfiguration, IXimuraConfigurationManager
    {
        #region Constructor
        /// <summary>
        /// The default constructor
        /// </summary>
        public AppServerCommandConfiguration() : this((IContainer)null) { }
        /// <summary>
        /// This constructor is called by .NET when it added as new to a container.
        /// </summary>
        /// <param name="container">The container this component should be added to.</param>
        public AppServerCommandConfiguration(System.ComponentModel.IContainer container)
            :
            base(container) { }
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
