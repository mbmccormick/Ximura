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
using System.Data;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Runtime.Serialization;
using System.Diagnostics;
using System.IO;
using System.Reflection;

using Ximura;
using Ximura.Data;
using Ximura.Framework;
using Ximura.Helper;
using CH = Ximura.Helper.Common;
#endregion // using
namespace Ximura.Data
{
    /// <summary>
    /// This is the base XML content class using the new .NET 3.5 Linq XML classes.
    /// </summary>
    public partial class XContent : Content
    {
        protected override byte[] ContentBody
        {
            get
            {
                return null;
            }
        }

        public override void OnDeserialization(object sender)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }
    }
}
