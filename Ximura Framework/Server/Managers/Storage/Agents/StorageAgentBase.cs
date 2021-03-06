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
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Runtime.Serialization;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Drawing;
using System.Security;
using System.Security.Cryptography;
using System.Security.Principal;

using Ximura;
using Ximura.Helper;
using Ximura.Server;
using Ximura.Command;
#endregion // using
namespace Ximura.Server
{
    public class StorageAgentBase : AppServerAgentBase<StorageManagerConfiguration,StorageManagerPerformance>, IXimuraStorageAgent
    {
        #region Constructor
		/// <summary>
		/// This is the default constructor
		/// </summary>
		public StorageAgentBase():this(null){}
		/// <summary>
		/// This is the base constructor for a Ximura command
		/// </summary>
		/// <param name="container">The container to be added to</param>
        public StorageAgentBase(System.ComponentModel.IContainer container)
            : base(container) 
        {
        }
        #endregion
    }
}
