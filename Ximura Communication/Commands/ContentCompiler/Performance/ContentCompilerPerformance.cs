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
using System.Runtime.Serialization;
using System.IO;
using System.ComponentModel;

using Ximura;
using Ximura.Helper;
using CH = Ximura.Helper.Common;
using CDS = Ximura.Persistence.CDSHelper;
using Ximura.Server;
using Ximura.Command;
using Ximura.Communication;
#endregion // using
namespace Ximura.Communication
{
    public class ContentCompilerPerformance : FSMCommandPerformance
    {
        #region Constructors
        /// <summary>
        /// This is the default constructor for the Content object.
        /// </summary>
        public ContentCompilerPerformance()
        {
        }
        #endregion
    }
}